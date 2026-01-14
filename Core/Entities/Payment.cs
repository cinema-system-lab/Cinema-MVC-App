namespace Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public int StatusId { get; set; }
        public PaymentStatus Status { get; set; } = null!;

        public Booking Booking { get; set; } = null!;
    }
}