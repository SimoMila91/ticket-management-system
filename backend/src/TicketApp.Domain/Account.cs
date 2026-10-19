namespace TicketApp.Domain;

public class Account
{
  public Guid Id { get; private set; }
  public string Name { get; private set; }
  public string Surname { get; private set; }
  public string Email { get; private set; }
  public string PasswordHash { get; private set; }
  public DateTime CreatedAtUtc { get; private set; }

  public Account(string name, string surname, string email, string passwordHash)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Name is required.", nameof(name));
    if (string.IsNullOrWhiteSpace(surname))
      throw new ArgumentException("Surname is required.", nameof(surname));
    if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
      throw new ArgumentException("A valid email is required.", nameof(email));

    if (string.IsNullOrWhiteSpace(passwordHash))
      throw new ArgumentException("Password hash is required.", nameof(passwordHash));

    Id = Guid.NewGuid();
    Name = name.Trim();
    Surname = surname.Trim();
    Email = email.Trim().ToLowerInvariant();
    PasswordHash = passwordHash;
    CreatedAtUtc = DateTime.UtcNow;
  }
}