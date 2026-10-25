using TicketApp.Application.Abstractions;
using TicketApp.Domain;

namespace TicketApp.Application.Tests.Accounts;

// Fast, predictable stand-in for bcrypt: no 250 ms per test, and the result is easy to check.
internal sealed class FakePasswordHasher : IPasswordHasher
{
  public string Hash(string password) => $"hashed:{password}";
  public bool Verify(string password, string hash) => hash == $"hashed:{password}";
}

// In-memory stand-in for the database.
internal sealed class InMemoryAccountRepository : IAccountRepository
{
  public List<Account> Saved { get; } = [];

  public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
      Task.FromResult(Saved.Any(a => a.Email == email));

  public Task AddAsync(Account account, CancellationToken ct)
  {
    Saved.Add(account);
    return Task.CompletedTask;
  }
}