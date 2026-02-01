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

    public PaymentService(CinemaAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
    {
        var payments = await _context.Payments
            .Include(p => p.Order)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<PaymentDTO>>(payments);
    }

    public async Task<PaymentDTO> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id);

        return _mapper.Map<PaymentDTO>(payment);
    }

    public async Task<PaymentDTO> GetPaymentByOrderIdAsync(Guid orderId)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        return _mapper.Map<PaymentDTO>(payment);
    }

    public async Task<Guid> CreatePaymentAsync(Guid orderId, decimal amount, PaymentStatus status = PaymentStatus.Pending)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0", nameof(amount));

        var orderExists = await _context.Orders.AnyAsync(o => o.Id == orderId);
        if (!orderExists)
            throw new KeyNotFoundException($"Order with ID {orderId} not found");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Amount = amount,
            Status = status,
            PaymentDate = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        if (status == PaymentStatus.Success)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = OrderStatus.Paid;
                await _context.SaveChangesAsync();
            }
        }

        return payment.Id;
    }

    public async Task UpdatePaymentStatusAsync(Guid id, PaymentStatus newStatus)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null)
            throw new KeyNotFoundException($"Payment with ID {id} not found");

        if (payment.Status == newStatus)
            return;

        payment.Status = newStatus;

        if (payment.Order != null)
        {
            payment.Order.Status = newStatus switch
            {
                PaymentStatus.Success => OrderStatus.Paid,
                PaymentStatus.Failed => OrderStatus.Cancelled,
                PaymentStatus.Refunded => OrderStatus.Refunded,
                _ => payment.Order.Status
            };
        }

        await _context.SaveChangesAsync();
    }
}