using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly CinemaAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IOrderService _orderService;

    public PaymentService(CinemaAppDbContext context, IMapper mapper, IOrderService orderService)
    {
        _context = context;
        _mapper = mapper;
        _orderService = orderService;
    }

    public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
    {
        return await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Session)
            .Include(p => p.Order)
            .ThenInclude(o => o.User)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentDTO
            {
                Id = p.Id,
                OrderId = p.OrderId,
                UserId = p.Order.UserId,
                UserEmail = p.Order.User.Email,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                Status = p.Status,
                SessionStartTime = p.Order.Session.StartTime
            })
            .ToListAsync();
    }

    public async Task<PaymentDTO> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Session)
            .FirstOrDefaultAsync(p => p.Id == id);

        var dto = _mapper.Map<PaymentDTO>(payment);

        if (payment?.Order?.Session != null)
        {
            dto.SessionStartTime = payment.Order.Session.StartTime;
        }

        return dto;
    }

    public async Task<PaymentDTO> GetPaymentByOrderIdAsync(Guid orderId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        return _mapper.Map<PaymentDTO>(payment);
    }

    public async Task<Guid> CreatePaymentAsync(CreatePaymentDTO createPaymentDto)
    {
        if (createPaymentDto.Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0", nameof(createPaymentDto.Amount));

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = await _context.Orders.FindAsync(createPaymentDto.OrderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {createPaymentDto.OrderId} not found");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Payment can only be created for pending orders");

            var existingPayment = await _context.Payments
                .AnyAsync(p => p.OrderId == createPaymentDto.OrderId);

            if (existingPayment)
                throw new InvalidOperationException("Payment already exists for this order");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = createPaymentDto.OrderId,
                Amount = createPaymentDto.Amount,
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.Now
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return payment.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdatePaymentStatusAsync(Guid id, PaymentStatus newStatus)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Session)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
            throw new KeyNotFoundException("Payment not found");

        if (payment.Status == newStatus)
            return;

        var isFinalStatus = payment.Status == PaymentStatus.Refunded ||
                            payment.Status == PaymentStatus.Failed;

        if (isFinalStatus)
        {
            throw new InvalidOperationException(
                $"Cannot modify payment in final status: {payment.Status}");
        }

        if (payment.Order?.Session == null)
        {
            throw new InvalidOperationException(
                "Session information is not available.");
        }

        if (payment.Order.Session.StartTime <= DateTime.Now)
        {
            throw new InvalidOperationException(
                $"Modification forbidden: session already started at {payment.Order.Session.StartTime:yyyy-MM-dd HH:mm}.");
        }

        var isValidTransition = (payment.Status, newStatus) switch
        {
            (PaymentStatus.Pending, PaymentStatus.Success) => true,
            (PaymentStatus.Pending, PaymentStatus.Failed) => true,
            (PaymentStatus.Success, PaymentStatus.Refunded) => true,
            _ => false
        };

        if (!isValidTransition)
            throw new InvalidOperationException(
                $"Invalid status transition from {payment.Status} to {newStatus}");

        var newOrderStatus = newStatus switch
        {
            PaymentStatus.Success => OrderStatus.Paid,
            PaymentStatus.Failed => OrderStatus.Cancelled,
            PaymentStatus.Refunded => OrderStatus.Refunded,
            _ => payment.Order.Status
        };

        if (payment.Order != null && newOrderStatus != payment.Order.Status)
        {
            await _orderService.UpdateStatusAsync(payment.OrderId, newOrderStatus);
        }

        payment.Status = newStatus;
        await _context.SaveChangesAsync();
    }
}