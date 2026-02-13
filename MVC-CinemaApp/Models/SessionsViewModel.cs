using Core.DTOs;

namespace Cinema_MVC_App.Models
{
    public class SessionsListVM
    {
        public IEnumerable<SessionDTO> Sessions { get; set; } = new List<SessionDTO>();
        public Dictionary<int, MovieDTO> Movies { get; set; } = new Dictionary<int, MovieDTO>();
        public Dictionary<int, HallDTO> Halls { get; set; } = new Dictionary<int, HallDTO>();

        public IOrderedEnumerable<IGrouping<DateTime, SessionDTO>> GroupedByDate =>
            Sessions.GroupBy(s => s.StartTime.Date).OrderBy(g => g.Key);
    }
}
