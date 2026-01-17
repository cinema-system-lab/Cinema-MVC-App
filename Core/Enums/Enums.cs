namespace Core.Enums
{
    // Той самий бітовий енам, про який казав тімлід
    [Flags]
    public enum GenreType
    {
        None = 0,
        Action = 1,
        Drama = 2,
        Comedy = 4,
        Horror = 8,
        SciFi = 16,
        Documentary = 32,
        Thriller = 64,
        Fantasy = 128,
        Animation = 256
        // Можна додавати далі: 512, 1024...
    }
    public enum HallType : byte
    {
        Standard = 1,
        IMAX = 2,
        VIP = 3,
        // 3D, 4DX тощо
    }
    public enum SeatType : byte
    {
        Regular = 1,
        Premium = 2
    }
    public enum OrderStatus : byte
    {
        Pending = 1,
        Paid = 2,
        Cancelled = 3,
        Refunded = 4
    }
    public enum PaymentStatus : byte
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }
}
