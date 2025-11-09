namespace App;

// minimalt user-objekt. vi behöver bara kunna logga in.
public class User
{
    private string _email;     // jag kör email som id. funkar fint för reception.
    private string _password;  // plain text här. jag skulle vilja hasha men får inte det enligt uppgiften eftersom vi inte har lärt oss det. SHA256 via System.Security.Cryptography 
    // using var sha = SHA256.Create();
    // var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
    // hade varit kul med en uppgift där jag fick vara helt unleashed med det jag kan programmera från tidigare erfarenhet.
    
    public User(string email, string password)
    {
        _email = email;          // lagra rakt av
        _password = password;
    }

    public string GetEmail() { return _email; }
    public string GetPassword() { return _password; }

    // enkel jämförelse. funkar fint for tillfället
    public bool TryLogin(string email, string password)
    {
        return email == _email && password == _password;
    }
}
