using TicketApp.Application.Accounts;
using Xunit;

namespace TicketApp.Application.Tests.Accounts;

public class RegisterAccountHandlerTests
{
  private readonly InMemoryAccountRepository _accounts = new();
  private readonly RegisterAccountHandler _handler;

  public RegisterAccountHandlerTests()
  {
    _handler = new RegisterAccountHandler(_accounts, new FakePasswordHasher());
  }

  private static RegisterAccountCommand Command(
      string email = "mario@example.com",
      string password = "password123") =>
      new("Mario", "Rossi", email, password);

  [Fact]
  public async Task HandleAsync_WithValidData_SavesAccountAndReturnsItsId()
  {
    var id = await _handler.HandleAsync(Command(), CancellationToken.None);

    var saved = Assert.Single(_accounts.Saved);
    Assert.Equal(id, saved.Id);
  }

  [Fact]
  public async Task HandleAsync_StoresTheHash_NotThePlainPassword()
  {
    await _handler.HandleAsync(Command(password: "password123"), CancellationToken.None);

    Assert.Equal("hashed:password123", _accounts.Saved[0].PasswordHash);
  }

  [Fact]
  public async Task HandleAsync_WhenEmailIsTakenWithDifferentCase_ThrowsAndSavesNothingNew()
  {
    await _handler.HandleAsync(Command(email: "mario@example.com"), CancellationToken.None);

    await Assert.ThrowsAsync<EmailAlreadyRegisteredException>(() =>
        _handler.HandleAsync(Command(email: "  Mario@Example.COM "), CancellationToken.None));

    Assert.Single(_accounts.Saved);
  }

  [Theory]
  [InlineData("")]
  [InlineData("short")]
  [InlineData("1234567")] // one character below the minimum
  public async Task HandleAsync_WithTooShortPassword_ThrowsAndSavesNothing(string password)
  {
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _handler.HandleAsync(Command(password: password), CancellationToken.None));

    Assert.Empty(_accounts.Saved);
  }

  [Fact]
  public async Task HandleAsync_WithInvalidEmail_ThrowsAndSavesNothing()
  {
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _handler.HandleAsync(Command(email: "not-an-email"), CancellationToken.None));

    Assert.Empty(_accounts.Saved);
  }
}