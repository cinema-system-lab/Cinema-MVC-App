namespace Core.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public int StatusId { get; set; }

        public DateTime CreatedAt { get; set; }

        public Session Session { get; set; } = null!;
        public BookingStatus Status { get; set; } = null!;

        public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
