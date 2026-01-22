using System;

namespace Core.Enums
{
    public enum OrderStatus : byte
    {
        Pending = 1,
        Paid = 2,
        Cancelled = 3,
        Refunded = 4
    }
}