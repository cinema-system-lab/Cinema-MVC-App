using Core.Enums;

namespace Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; } //Конфлікт імен: BookingId vs Order тому використано OrderId
        public virtual Order Order { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }

    }
}