using Microsoft.EntityFrameworkCore;
using TicketApp.Api.Contracts;
using TicketApp.Domain;
using TicketApp.Infrastructure.Persistence;

// This creates the builder. 
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => "OK champion").WithName("GetHealth");

// --- POST --- // 

app.MapPost("/auth/register", async (RegisterRequest request, AppDbContext dbContext, CancellationToken ct) =>
{
    if (request.Password.Length < 8)
    {
        return Results.BadRequest("The password is shorter than 8 characters");
    }

    var hash = BCrypt.Net.BCrypt.HashPassword(request.Password, 12);

    Account account;

    try
    {
        account = new Account(request.Name, request.Surname, request.Email, request.Password);

    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }

    // check if the account already exists 
    var emailTaken = await dbContext.Accounts.AnyAsync(a => a.Email == account.Email, ct);
    if (emailTaken)
        return Results.Conflict("This email is already registered");

    // save account 
    dbContext.Accounts.Add(account);
    await dbContext.SaveChangesAsync(ct);

    return Results.Created($"/accounts/{account.Id}", new { account.Id });
});

app.Run();


