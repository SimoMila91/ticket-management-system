using TicketApp.Domain;

namespace TicketApp.Application.Abstractions;


public interface IAccountRepository
{
  Task<bool> EmailExistsAsync(string email, CancellationToken ct);
  Task AddAsync(Account account, CancellationToken ct);
}
