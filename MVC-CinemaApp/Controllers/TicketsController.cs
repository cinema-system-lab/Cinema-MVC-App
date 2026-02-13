using Cinema_MVC_App.Models;
using Core.Enums;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Cinema_MVC_App.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IMovieService _movieService;
    private readonly IHallService _hallService;
    private readonly ISeatService _seatService;
    private readonly ITicketService _ticketService;
    private readonly IOrderService _orderService;

    private const int HoldMinutes = 15;

    public TicketsController(
    ISessionService sessionService,
    IMovieService movieService,
    IHallService hallService,
    ISeatService seatService,
    ITicketService ticketService,
    IOrderService orderService)
    {
        _sessionService = sessionService;
        _movieService = movieService;
        _hallService = hallService;
        _seatService = seatService;
        _ticketService = ticketService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Book(int sessionId, bool reset = false)
    {
        var session = await _sessionService.GetSessionAsync(sessionId);
        if (session == null) return NotFound();

        var movie = await _movieService.GetMovieAsync(session.MovieId);
        var hall = await _hallService.GetHallAsync(session.HallId);

        if (movie == null || hall == null) return NotFound();

        var seats = await _seatService.GetSeatsByHallIdAsync(hall.Id);
        var occupiedSeatIds = await _ticketService.GetOccupiedSeatIdsAsync(sessionId);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // Only release holds if explicitly requested (coming fresh from Movie Details)
        if (reset && !string.IsNullOrEmpty(userId))
        {
            SeatHoldStore.ReleaseAllForSession(userId, sessionId);
        }

        var heldByOthers = SeatHoldStore.GetHeldSeatIdsByOthers(sessionId, userId);
        
        // Get user's current hold expiration (if any seats are held)
        var userHeldSeatIds = SeatHoldStore.GetHeldSeatIdsByUser(userId, sessionId);
        DateTime? holdExpiresAtUtc = null;
        if (userHeldSeatIds.Length > 0)
        {
            holdExpiresAtUtc = SeatHoldStore.GetHoldExpiration(userId, sessionId, userHeldSeatIds);
        }

        var viewModel = new SeatSelectionVM
        {
            SessionId = session.Id,
            MovieId = movie.Id,
            MovieTitle = movie.Title,
            PosterUrl = movie.PosterUrl ?? string.Empty,
            StartTime = session.StartTime,
            BasePrice = session.BasePrice,
            HallId = hall.Id,
            HallName = hall.Name,
            HallType = hall.Type,
            Seats = seats,
            OccupiedSeatIds = occupiedSeatIds
                .Concat(heldByOthers)
                .ToHashSet()
        };
        
        ViewBag.HoldExpiresAtUtc = holdExpiresAtUtc;
        ViewBag.UserHeldSeatIds = userHeldSeatIds;
        ViewBag.IsReset = reset;

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid orderId, int sessionId, int seatId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);

        if (order == null) return NotFound();

        if (order.Status != OrderStatus.Paid)
        {
            TempData["ErrorMessage"] = "Ticket is not available until the order is paid.";
            return RedirectToAction("Details", "Orders", new { id = orderId });
        }

        // 3. Если все ок, получаем билет
        var ticket = await _ticketService.GetTicketByIdAsync(orderId, sessionId, seatId);

        if (ticket == null) return NotFound();

        return View(ticket);
    }

    public sealed record HoldSeatsRequest(int SessionId, int[] SeatIds);
    public sealed record HoldSeatsResponse(bool Success, int[] HeldSeatIds, int[] RejectedSeatIds, DateTime? ExpiresAtUtc);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult HoldSeats([FromBody] HoldSeatsRequest request)
    {
        if (request.SeatIds == null || request.SeatIds.Length == 0)
            return BadRequest("SeatIds is required.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var (held, rejected, expiresAtUtc) = SeatHoldStore.TryHold(
            userId: userId,
            sessionId: request.SessionId,
            seatIds: request.SeatIds,
            holdFor: TimeSpan.FromMinutes(HoldMinutes));

        var success = rejected.Length == 0;

        return Ok(new HoldSeatsResponse(
            Success: success,
            HeldSeatIds: held,
            RejectedSeatIds: rejected,
            ExpiresAtUtc: expiresAtUtc));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ReleaseSeats([FromBody] HoldSeatsRequest request)
    {
        if (request.SeatIds == null || request.SeatIds.Length == 0)
            return BadRequest("SeatIds is required.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        SeatHoldStore.Release(userId, request.SessionId, request.SeatIds);
        return Ok();
    }
}

public static class SeatHoldStore
{
    private sealed class HoldEntry
    {
        public required string UserId { get; init; }
        public required DateTime ExpiresAtUtc { get; set; }
    }

    private static readonly ConcurrentDictionary<(int SessionId, int SeatId), HoldEntry> Holds = new();

    private static void PurgeExpired()
    {
        var now = DateTime.UtcNow;
        foreach (var kv in Holds)
        {
            if (kv.Value.ExpiresAtUtc <= now)
                Holds.TryRemove(kv.Key, out _);
        }
    }

    public static int[] GetHeldSeatIdsByOthers(int sessionId, string currentUserId)
    {
        PurgeExpired();

        return Holds
            .Where(kv => kv.Key.SessionId == sessionId && kv.Value.UserId != currentUserId)
            .Select(kv => kv.Key.SeatId)
            .Distinct()
            .ToArray();
    }

    public static bool IsHeldByUser(string userId, int sessionId, IEnumerable<int> seatIds)
    {
        PurgeExpired();

        foreach (var seatId in seatIds)
        {
            if (!Holds.TryGetValue((sessionId, seatId), out var entry)) return false;
            if (!string.Equals(entry.UserId, userId, StringComparison.Ordinal)) return false;
            if (entry.ExpiresAtUtc <= DateTime.UtcNow) return false;
        }

        return true;
    }

    public static (int[] Held, int[] Rejected, DateTime? ExpiresAtUtc) TryHold(
        string userId,
        int sessionId,
        IEnumerable<int> seatIds,
        TimeSpan holdFor)
    {
        PurgeExpired();

        var now = DateTime.UtcNow;
        var newExpiry = now.Add(holdFor);

        var held = new List<int>();
        var rejected = new List<int>();

        foreach (var seatId in seatIds.Distinct())
        {
            var key = (sessionId, seatId);

            var ok = Holds.AddOrUpdate(
                key,
                addValueFactory: _ => new HoldEntry { UserId = userId, ExpiresAtUtc = newExpiry },
                updateValueFactory: (_, existing) =>
                {
                    if (existing.ExpiresAtUtc <= now)
                        return new HoldEntry { UserId = userId, ExpiresAtUtc = newExpiry };

                    if (string.Equals(existing.UserId, userId, StringComparison.Ordinal))
                    {
                        existing.ExpiresAtUtc = newExpiry;
                        return existing;
                    }

                    return existing;
                });

            if (string.Equals(ok.UserId, userId, StringComparison.Ordinal) && ok.ExpiresAtUtc == newExpiry)
                held.Add(seatId);
            else if (string.Equals(ok.UserId, userId, StringComparison.Ordinal) && ok.ExpiresAtUtc > now)
                held.Add(seatId);
            else
                rejected.Add(seatId);
        }

        DateTime? expiresAtUtc = null;
        var myEntries = Holds
            .Where(kv => kv.Key.SessionId == sessionId && kv.Value.UserId == userId)
            .Select(kv => kv.Value.ExpiresAtUtc)
            .ToArray();

        if (myEntries.Length > 0)
            expiresAtUtc = myEntries.Min();

        return (held.ToArray(), rejected.ToArray(), expiresAtUtc);
    }

    public static void Release(string userId, int sessionId, IEnumerable<int> seatIds)
    {
        PurgeExpired();

        foreach (var seatId in seatIds.Distinct())
        {
            var key = (sessionId, seatId);

            if (Holds.TryGetValue(key, out var entry) &&
                string.Equals(entry.UserId, userId, StringComparison.Ordinal))
            {
                Holds.TryRemove(key, out _);
            }
        }
    }

    public static DateTime? GetHoldExpiration(string userId, int sessionId, IEnumerable<int> seatIds)
    {
        PurgeExpired();

        DateTime? earliest = null;
        foreach (var seatId in seatIds.Distinct())
        {
            var key = (sessionId, seatId);
            if (Holds.TryGetValue(key, out var entry) &&
                string.Equals(entry.UserId, userId, StringComparison.Ordinal))
            {
                if (earliest == null || entry.ExpiresAtUtc < earliest)
                    earliest = entry.ExpiresAtUtc;
            }
        }
        return earliest;
    }

    public static void ExtendHold(string userId, int sessionId, IEnumerable<int> seatIds, TimeSpan extendBy)
    {
        PurgeExpired();
        var newExpiry = DateTime.UtcNow.Add(extendBy);

        foreach (var seatId in seatIds.Distinct())
        {
            var key = (sessionId, seatId);
            if (Holds.TryGetValue(key, out var entry) &&
                string.Equals(entry.UserId, userId, StringComparison.Ordinal))
            {
                entry.ExpiresAtUtc = newExpiry;
            }
        }
    }

    public static void ReleaseAllForSession(string userId, int sessionId)
    {
        PurgeExpired();

        var keysToRemove = Holds
            .Where(kv => kv.Key.SessionId == sessionId && 
                         string.Equals(kv.Value.UserId, userId, StringComparison.Ordinal))
            .Select(kv => kv.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            Holds.TryRemove(key, out _);
        }
    }

    public static int[] GetHeldSeatIdsByUser(string userId, int sessionId)
    {
        PurgeExpired();

        return Holds
            .Where(kv => kv.Key.SessionId == sessionId && 
                         string.Equals(kv.Value.UserId, userId, StringComparison.Ordinal))
            .Select(kv => kv.Key.SeatId)
            .ToArray();
    }
}