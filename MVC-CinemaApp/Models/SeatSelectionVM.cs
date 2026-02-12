using System.Collections.Generic;
using System.Linq;
using Core.DTOs;
using Core.Enums;

namespace Cinema_MVC_App.Models
{
    public class SeatSelectionVM
    {
        public int SessionId { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public decimal BasePrice { get; set; }

        public int HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public HallType HallType { get; set; }

        public List<SeatDTO> Seats { get; set; } = new();

        public HashSet<int> OccupiedSeatIds { get; set; } = new();

        public ILookup<byte, SeatDTO> SeatsByRow =>
            Seats.ToLookup(s => s.RowNumber);

        public decimal GetSeatPrice(SeatDTO seat) =>
            seat.Type == SeatType.Premium ? BasePrice * 1.5m : BasePrice;
    }
}

