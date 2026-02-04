using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs;

public class CreateOrderRequest
{
    public int SessionId { get; set; }
    public List<int> SeatIds { get; set; } = new List<int>();
}
