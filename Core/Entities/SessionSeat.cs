namespace Core.Entities
{
    public class SessionSeat
    {
        public int SessionId { get; set; }
        public int SeatId { get; set; }
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }

        public Session Session { get; set; } = null!;
        public Seat Seat { get; set; } = null!;

        public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();

    }
}
