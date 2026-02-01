using Core.Entities;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces.Services;
using Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Core.Helpers;
using Core.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddValidatorsFromAssemblyContaining<MovieValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddDbContext<CinemaAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<CinemaAppDbContext>()
    .AddDefaultTokenProviders();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MovieProfile).Assembly);

// DI for MovieService
builder.Services.AddScoped<IMovieService, MovieService>();

// DI for HallService
builder.Services.AddScoped<IHallService, HallService>();

// DI for SeatService
builder.Services.AddScoped<ISeatService, SeatService>();

// DI for SessionService
builder.Services.AddScoped<ISessionService, SessionService>();

// DI for TicketService
builder.Services.AddScoped<ITicketService, TicketService>();

// DI for OrderService
builder.Services.AddScoped<IOrderService, OrderService>();

// DI for OrderService
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seeding тестових даних
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<CinemaAppDbContext>();
        
        if (!context.Payments.Any())
        {
            Console.WriteLine("🔄 Creating test payments...");
            
            // ✅ Знайти реального користувача або використати існуючий Order
            var existingOrder = context.Orders.FirstOrDefault();
            
            if (existingOrder != null)
            {
                // Якщо є Order - використати його
                Console.WriteLine($"Using existing order: {existingOrder.Id}");
                
                context.Payments.AddRange(new[]
                {
                    new Payment 
                    { 
                        Id = Guid.NewGuid(), 
                        OrderId = existingOrder.Id, 
                        Amount = 25.50m, 
                        PaymentDate = DateTime.UtcNow.AddHours(-2), 
                        Status = PaymentStatus.Success 
                    },
                    new Payment 
                    { 
                        Id = Guid.NewGuid(), 
                        OrderId = existingOrder.Id, 
                        Amount = 15.00m, 
                        PaymentDate = DateTime.UtcNow.AddHours(-5), 
                        Status = PaymentStatus.Pending 
                    },
                    new Payment 
                    { 
                        Id = Guid.NewGuid(), 
                        OrderId = existingOrder.Id, 
                        Amount = 30.00m, 
                        PaymentDate = DateTime.UtcNow.AddDays(-1), 
                        Status = PaymentStatus.Failed 
                    },
                    new Payment 
                    { 
                        Id = Guid.NewGuid(), 
                        OrderId = existingOrder.Id, 
                        Amount = 50.00m, 
                        PaymentDate = DateTime.UtcNow.AddDays(-2), 
                        Status = PaymentStatus.Refunded 
                    }
                });
                
                context.SaveChanges();
                Console.WriteLine("✅ Test payments created!");
            }
            else
            {
                Console.WriteLine("⚠️ No orders found in database. Please create an order first.");
            }
        }
        else
        {
            Console.WriteLine($"ℹ️ Database already has {context.Payments.Count()} payments");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error seeding database: {ex.Message}");
    }
}

app.Run();