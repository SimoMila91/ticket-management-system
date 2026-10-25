namespace TicketApp.Api.Contracts;

public record RegisterRequest(string Name, string Surname, string Email, string Password);