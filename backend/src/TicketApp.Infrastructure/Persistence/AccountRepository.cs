using Microsoft.EntityFrameworkCore;
using Npgsql;
using TicketApp.Application.Abstractions;
using TicketApp.Application.Accounts;
using TicketApp.Domain;

namespace TicketApp.Infrastructure.Persistence;

public sealed class AccountRepository(AppDbContext db) : IAccountRepository
{
  public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
      db.Accounts.AnyAsync(a => a.Email == email, ct);

  public async Task AddAsync(Account account, CancellationToken ct)
  {
    db.Accounts.Add(account);

    try
    {
      await db.SaveChangesAsync(ct);
    }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException
    { SqlState: PostgresErrorCodes.UniqueViolation })
    {
      // Two requests with the same email can both pass EmailExistsAsync at the same time.
      // The unique index stops the second one; we turn that into the same domain error.
      throw new EmailAlreadyRegisteredException();
    }
  }
}