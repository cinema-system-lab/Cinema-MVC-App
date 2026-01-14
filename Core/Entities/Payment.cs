namespace Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int StatusId { get; set; }

        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal BasePrice { get; set; }

        public Booking Booking { get; set; } = null!;
        public PaymentStatus Status { get; set; } = null!;
        public Movie Movie { get; set; } = null!;   
        public Hall Hall { get; set; } = null!;
    }
}
