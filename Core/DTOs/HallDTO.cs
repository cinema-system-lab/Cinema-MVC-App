using Core.Enums;

namespace Core.DTOs;

public class HallDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public HallType Type { get; set; }
}
