namespace TicketApp.Application.Accounts;

public sealed class EmailAlreadyRegisteredException()
    : Exception("This email is already registered.");