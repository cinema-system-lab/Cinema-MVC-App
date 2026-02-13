namespace Core.Enums
{
    public enum PaymentStatus : byte
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        Refunded = 4
    }
}