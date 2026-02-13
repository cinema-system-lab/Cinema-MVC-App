using Core.DTOs;

namespace Cinema_MVC_App.Models
{
    public class MovieDetailsVM
    {
        public MovieDTO Movie { get; set; }
        public Dictionary<DateTime, List<SessionDTO>> SessionsByDate { get; set; } = new();
        public Dictionary<int, string> HallNames { get; set; } = new();
    }
}
