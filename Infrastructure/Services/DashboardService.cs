using Cinema_MVC_App.Areas.Admin.ViewModels.Dashboard;
using Core.Enums;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly CinemaAppDbContext _context;

    public DashboardService(CinemaAppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var model = new DashboardViewModel
        {
            TotalMovies = await _context.Movies.CountAsync(),
            ActiveMovies = await _context.Movies.CountAsync(m => m.IsActive),

            TotalHalls = await _context.Halls.CountAsync(),
            TotalSeats = await _context.Seats.CountAsync(),

            TotalSessions = await _context.Sessions.CountAsync(),
            TodaySessions = await _context.Sessions
                .CountAsync(s => s.StartTime.Date == DateTime.Today),
            UpcomingSessions = await _context.Sessions
                .CountAsync(s => s.StartTime > DateTime.Now),

            TotalTickets = await _context.Tickets
                .CountAsync(t => t.Order.Status == OrderStatus.Paid),
            TotalOrders = await _context.Orders.CountAsync(),
            PendingOrders = await _context.Orders
                .CountAsync(o => o.Status == OrderStatus.Pending),
            CompletedOrders = await _context.Orders
                .CountAsync(o => o.Status == OrderStatus.Paid),

            TotalRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Success)
                .SumAsync(p => p.Amount),
            TodayRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Success &&
                            p.PaymentDate.Date == DateTime.Today)
                .SumAsync(p => p.Amount),
            ThisMonthRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Success &&
                            p.PaymentDate.Month == DateTime.Now.Month &&
                            p.PaymentDate.Year == DateTime.Now.Year)
                .SumAsync(p => p.Amount),

            TopMovies = await GetTopMoviesAsync(),

            RecentOrders = await GetRecentOrdersAsync(),

            HallStats = await GetHallStatsAsync(),

            DailySales = await GetDailySalesDataAsync()
        };

        return model;
    }

    private async Task<List<MovieStatsViewModel>> GetTopMoviesAsync()
    {
        return await _context.Movies
            .Select(m => new MovieStatsViewModel
            {
                MovieTitle = m.Title,
                TicketsSold = _context.Tickets
                    .Count(t => t.Session.MovieId == m.Id &&
                                t.Order.Status == OrderStatus.Paid),
                Revenue = _context.Payments
                    .Where(p => p.Status == PaymentStatus.Success &&
                                p.Order.Tickets.Any(t => t.Session.MovieId == m.Id))
                    .Sum(p => p.Amount)
            })
            .Where(m => m.TicketsSold > 0)
            .OrderByDescending(m => m.TicketsSold)
            .Take(5)
            .ToListAsync();
    }

    private async Task<List<RecentOrderViewModel>> GetRecentOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Tickets)
            .ThenInclude(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(o => o.Payment)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new RecentOrderViewModel
            {
                OrderId = o.Id,
                UserName = o.User.FirstName + " " + o.User.LastName,
                OrderDate = o.CreatedAt,
                TotalAmount = o.Payment != null ? o.Payment.Amount : 0,
                Status = o.Status.ToString(),
                TicketsCount = o.Tickets.Count,
                MovieTitle = o.Tickets.Any()
                    ? o.Tickets.First().Session.Movie.Title
                    : "N/A"
            })
            .ToListAsync();
    }

    private async Task<List<HallStatsViewModel>> GetHallStatsAsync()
    {
        return await _context.Halls
            .Select(h => new HallStatsViewModel
            {
                HallName = h.Name,
                TotalSeats = h.Seats.Count,
                SessionsCount = _context.Sessions.Count(s => s.HallId == h.Id),
                OccupancyRate = _context.Sessions.Any(s => s.HallId == h.Id) && h.Seats.Any()
                    ? (double)_context.Tickets
                        .Count(t => t.Session.HallId == h.Id &&
                                    t.Order.Status == OrderStatus.Paid) /
                    (double)(_context.Sessions.Count(s => s.HallId == h.Id) * h.Seats.Count) * 100.0
                    : 0
            })
            .ToListAsync();
    }

    private async Task<List<DailySalesViewModel>> GetDailySalesDataAsync()
    {
        var fromDate = DateTime.Today.AddDays(-6);

        var paymentsData = await _context.Payments
            .Where(p =>
                p.Status == PaymentStatus.Success &&
                p.PaymentDate.Date >= fromDate)
            .Select(p => new
            {
                Date = p.PaymentDate.Date,
                Amount = p.Amount,
                TicketsCount = p.Order.Tickets.Count
            })
            .ToListAsync();

        var dailySales = paymentsData
            .GroupBy(p => p.Date)
            .Select(g => new DailySalesViewModel
            {
                Date = g.Key,
                Revenue = g.Sum(p => p.Amount),
                TicketsSold = g.Sum(p => p.TicketsCount)
            })
            .OrderBy(d => d.Date)
            .ToList();

        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.Today.AddDays(-i))
            .Reverse()
            .ToList();

        var result = last7Days.Select(day =>
                dailySales.FirstOrDefault(s => s.Date == day) ??
                new DailySalesViewModel
                {
                    Date = day,
                    Revenue = 0,
                    TicketsSold = 0
                })
            .ToList();

        return result;
    }
}