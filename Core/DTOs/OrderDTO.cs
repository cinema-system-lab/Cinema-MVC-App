using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.DTOs;

public class OrderDTO
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public int SessionId { get; set; }

    public string MovieTitle { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public DateTime SessionStartTime { get; set; }
    public decimal TotalPrice { get; set; }

    public List<TicketDTO> Tickets { get; set; } = new List<TicketDTO>();
}