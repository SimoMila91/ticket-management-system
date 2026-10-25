namespace TicketApp.Application.Accounts;

public record RegisterAccountCommand(string Name, string Surname, string Email, string Password);