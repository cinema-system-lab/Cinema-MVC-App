using Core.Enums;

namespace Core.Entities
{
    public class Ticket
    {
        public int OrderId { get; set; } //Конфлікт імен: BookingId vs Order, тому використано OrderId
        public virtual Order Order { get; set; }
        public int SessionId { get; set; }
        public virtual Session Session { get; set; }
        public int SeatId { get; set; }
        public virtual Seat Seat { get; set; }
    }
}
