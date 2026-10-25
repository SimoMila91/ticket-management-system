using TicketApp.Application.Abstractions;
using TicketApp.Domain;

namespace TicketApp.Application.Accounts;

public sealed class RegisterAccountHandler(IAccountRepository accounts, IPasswordHasher hasher)
{
  public const int MinPasswordLength = 8;

  public async Task<Guid> HandleAsync(RegisterAccountCommand command, CancellationToken ct)
  {
    // 1. Password rule. It lives here because the Domain never sees the plain password.
    if (string.IsNullOrEmpty(command.Password) || command.Password.Length < MinPasswordLength)
      throw new ArgumentException($"Password must be at least {MinPasswordLength} characters.");

    // 2. Hash it. The plain password is never stored.
    var hash = hasher.Hash(command.Password);

    // 3. The Domain creates the account and rejects invalid data (ArgumentException).
    var account = new Account(command.Name, command.Surname, command.Email, hash);

    // 4. Uniqueness check on the normalized email (trimmed and lower-case by the Domain).
    if (await accounts.EmailExistsAsync(account.Email, ct))
      throw new EmailAlreadyRegisteredException();

    // 5. Save.
    await accounts.AddAsync(account, ct);

    return account.Id;
  }
}