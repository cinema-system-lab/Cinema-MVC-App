using Core.Enums;

namespace Core.Entities;

public class Hall
{
    public int Id { get; set; }

    //[MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public HallType Type { get; set; }
    /* Прибрав virtual у навігаційних властивостях. В EF Core є Eager Loading через .Include().
     Приклад властивості до змін:
         public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>(); */
    
    public ICollection<Seat> Seats { get; set; } = new List<Seat>(); //-virtual
}
