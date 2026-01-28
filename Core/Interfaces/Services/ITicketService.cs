using Core.DTOs;

namespace Core.Interfaces.Services
{
    public interface ITicketService
    {
        Task<List<TicketDTO>> GetAllTicketsAsync();

        Task<List<TicketDTO>> GetTicketsByOrderAsync(Guid orderId);

        Task<TicketDTO?> GetTicketByIdAsync(Guid orderId, int sessionId, int seatId);

        Task CreateTicketAsync(TicketCreateDTO ticketDto);

        Task DeleteTicketAsync(Guid orderId, int sessionId, int seatId);
    }
}
