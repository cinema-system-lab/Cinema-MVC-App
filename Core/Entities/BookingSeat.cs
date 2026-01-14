namespace Core.Entities
{
    public class BookingSeat
    {
        public int BookingId { get; set; }
        public int SeatId { get; set; }
        public Booking Booking { get; set; } = null!;
        public SessionSeat SessionSeat { get; set; } = null!;
    }
}
