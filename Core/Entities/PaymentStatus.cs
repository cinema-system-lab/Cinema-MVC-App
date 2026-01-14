namespace Core.Entities
{
    public class PaymentStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
