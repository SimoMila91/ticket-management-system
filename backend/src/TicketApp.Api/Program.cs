using Microsoft.EntityFrameworkCore;
using TicketApp.Api.Contracts;
using TicketApp.Application.Abstractions;
using TicketApp.Application.Accounts;
using TicketApp.Infrastructure.Persistence;

// This creates the builder. 
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<IPasswordHasher, TicketApp.Infrastructure.Security.BcryptPasswordHasher>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<RegisterAccountHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => "OK champion").WithName("GetHealth");

// --- POST --- // 


app.MapPost("/auth/register", async (RegisterRequest request, RegisterAccountHandler handler, CancellationToken ct) =>
{
    try
    {
        var id = await handler.HandleAsync(
            new RegisterAccountCommand(request.Name, request.Surname, request.Email, request.Password), ct);

        return Results.Created($"/accounts/{id}", new { id });
    }
    catch (EmailAlreadyRegisteredException ex) { return Results.Conflict(ex.Message); }
    catch (ArgumentException ex) { return Results.BadRequest(ex.Message); }
});

app.Run();


