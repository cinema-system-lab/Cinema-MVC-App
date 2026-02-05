using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cinema_MVC_App.Areas.Admin.ViewModels.Dashboard;

namespace Cinema_MVC_App.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        private readonly CinemaAppDbContext _context;

        public DashboardController(CinemaAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                TotalMovies = await _context.Movies.CountAsync(),
                ActiveMovies = await _context.Movies.CountAsync(m => m.ReleaseDate <= DateTime.Now),
                TotalHalls = await _context.Halls.CountAsync(),
                TotalSeats = await _context.Seats.CountAsync(),
                
                TotalSessions = await _context.Sessions.CountAsync(),
                TodaySessions = await _context.Sessions
                    .CountAsync(s => s.StartTime.Date == DateTime.Today),
                UpcomingSessions = await _context.Sessions
                    .CountAsync(s => s.StartTime > DateTime.Now),
                
                TotalTickets = await _context.Tickets.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                PendingOrders = await _context.Orders
                    .CountAsync(o => o.Status == Core.Enums.OrderStatus.Pending),
                CompletedOrders = await _context.Orders
                    .CountAsync(o => o.Status == Core.Enums.OrderStatus.Paid),
                
                TotalRevenue = await _context.Payments
                    .Where(p => p.Status == Core.Enums.PaymentStatus.Success)
                    .SumAsync(p => p.Amount),
                TodayRevenue = await _context.Payments
                    .Where(p => p.Status == Core.Enums.PaymentStatus.Success 
                           && p.PaymentDate.Date == DateTime.Today)
                    .SumAsync(p => p.Amount),
                ThisMonthRevenue = await _context.Payments
                    .Where(p => p.Status == Core.Enums.PaymentStatus.Success 
                           && p.PaymentDate.Month == DateTime.Now.Month 
                           && p.PaymentDate.Year == DateTime.Now.Year)
                    .SumAsync(p => p.Amount),
                
                TopMovies = await _context.Movies
                    .Select(m => new MovieStatsViewModel
                    {
                        MovieTitle = m.Title,
                        TicketsSold = _context.Tickets
                            .Count(t => t.Session.MovieId == m.Id),
                        Revenue = _context.Tickets
                            .Where(t => t.Session.MovieId == m.Id)
                            .Sum(t => t.Session.BasePrice * (t.Seat.Type == Core.Enums.SeatType.Premium ? 1.5m : 1.0m))
                    })
                    .Where(m => m.TicketsSold > 0)
                    .OrderByDescending(m => m.TicketsSold)
                    .Take(5)
                    .ToListAsync(),
                
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Tickets)
                        .ThenInclude(t => t.Session)
                            .ThenInclude(s => s.Movie)
                    .Include(o => o.Tickets)
                        .ThenInclude(t => t.Seat)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .Select(o => new RecentOrderViewModel
                    {
                        OrderId = o.Id,
                        UserName = o.User.FirstName + " " + o.User.LastName,
                        OrderDate = o.CreatedAt,
                        TotalAmount = o.Tickets.Sum(t => t.Session.BasePrice * (t.Seat.Type == Core.Enums.SeatType.Premium ? 1.5m : 1.0m)),
                        Status = o.Status.ToString(),
                        TicketsCount = o.Tickets.Count,
                        MovieTitle = o.Tickets.FirstOrDefault() != null 
                            ? o.Tickets.First().Session.Movie.Title 
                            : "N/A"
                    })
                    .ToListAsync(),
                
                HallStats = await _context.Halls
                    .Select(h => new HallStatsViewModel
                    {
                        HallName = h.Name,
                        TotalSeats = h.Seats.Count,
                        SessionsCount = _context.Sessions.Count(s => s.HallId == h.Id),
                        OccupancyRate = _context.Sessions.Any(s => s.HallId == h.Id) && h.Seats.Any()
                            ? (double)_context.Tickets
                                .Count(t => t.Session.HallId == h.Id) / 
                                (double)(_context.Sessions.Count(s => s.HallId == h.Id) * h.Seats.Count) * 100.0
                            : 0
                    })
                    .ToListAsync(),
                
                DailySales = await GetDailySalesData()
            };

            return View(model);
        }

        private async Task<List<DailySalesViewModel>> GetDailySalesData()
        {
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .Reverse()
                .ToList();

            var salesData = new List<DailySalesViewModel>();

            foreach (var day in last7Days)
            {
                var dayRevenue = await _context.Payments
                    .Where(p => p.Status == Core.Enums.PaymentStatus.Success 
                           && p.PaymentDate.Date == day.Date)
                    .SumAsync(p => p.Amount);

                var ticketsSold = await _context.Tickets
                    .Where(t => t.Order.CreatedAt.Date == day.Date 
                           && t.Order.Status == Core.Enums.OrderStatus.Paid)
                    .CountAsync();

                salesData.Add(new DailySalesViewModel
                {
                    Date = day,
                    Revenue = dayRevenue,
                    TicketsSold = ticketsSold
                });
            }

            return salesData;
        }
    }
}