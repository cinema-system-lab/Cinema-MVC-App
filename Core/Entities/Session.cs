using Core.Enums;

namespace Core.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        
        public Movie Movie { get; set; } //-virtual
        public int HallId { get; set; }
        
        public Hall Hall { get; set; }//-virtual
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        public decimal BasePrice { get; set; }
    }
}
