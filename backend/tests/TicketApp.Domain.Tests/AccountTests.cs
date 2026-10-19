using Xunit;

namespace TicketApp.Domain.Tests;

public class AccountTests
{

  // helper 
  private static Account CreateAccount(
    string name = "Mario",
    string surname = "Rossi",
    string email = "mario@rossi.com",
    string passwordHash = "hash"
  ) => new(name, surname, email, passwordHash);

  [Fact]
  public void Constructor_ValidData_StoreValues()
  {
    var account = CreateAccount();

    Assert.Equal("Mario", account.Name);
    Assert.Equal("Rossi", account.Surname);
    Assert.Equal("mario@rossi.com", account.Email);
    Assert.Equal("hash", account.PasswordHash);
  }

  [Fact]
  public void Constructor_NonEmptyId()
  {
    var account = CreateAccount();

    Assert.NotEqual(Guid.Empty, account.Id);
  }
  [Fact]
  public void Constructor_SetsCreatedAtToCurrentUtcTime()
  {
    var before = DateTime.UtcNow;

    var account = CreateAccount();

    var after = DateTime.UtcNow;
    Assert.Equal(DateTimeKind.Utc, account.CreatedAtUtc.Kind);
    Assert.InRange(account.CreatedAtUtc, before, after);
  }

  // ---------- Normalization ----------

  [Fact]
  public void Constructor_NormalizesEmailToLowercaseAndTrimmed()
  {
    var account = CreateAccount(email: "  Mario@Example.COM ");

    Assert.Equal("mario@example.com", account.Email);
  }

  [Fact]
  public void Constructor_TrimsNameAndSurname()
  {
    var account = CreateAccount(name: "  Mario ", surname: " Rossi  ");

    Assert.Equal("Mario", account.Name);
    Assert.Equal("Rossi", account.Surname);
  }

  // ---------- Invalid input ----------

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData("\t")]
  public void Constructor_WithBlankName_Throws(string name)
  {
    var exception = Assert.Throws<ArgumentException>(() => CreateAccount(name: name));

    Assert.Equal("name", exception.ParamName);
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void Constructor_WithBlankSurname_Throws(string surname)
  {
    var exception = Assert.Throws<ArgumentException>(() => CreateAccount(surname: surname));

    Assert.Equal("surname", exception.ParamName);
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  [InlineData("not-an-email")]
  public void Constructor_WithInvalidEmail_Throws(string email)
  {
    var exception = Assert.Throws<ArgumentException>(() => CreateAccount(email: email));

    Assert.Equal("email", exception.ParamName);
  }

  [Theory]
  [InlineData("")]
  [InlineData("   ")]
  public void Constructor_WithBlankPasswordHash_Throws(string passwordHash)
  {
    var exception = Assert.Throws<ArgumentException>(() => CreateAccount(passwordHash: passwordHash));

    Assert.Equal("passwordHash", exception.ParamName);
  }

}