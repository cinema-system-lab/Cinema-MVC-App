using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces.Services;
using Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Core.Helpers;
using Core.Validators;
using Cinema_MVC_App.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add local configuration file (not tracked in git)
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.Local.json",
    optional: true,
    reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddValidatorsFromAssemblyContaining<MovieValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddDbContext<CinemaAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityConfiguration();

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

var app = builder.Build();
await app.SeedDatabaseAsync();

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
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
