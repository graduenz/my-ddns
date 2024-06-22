namespace MyDDNS.Registrar.Cloudflare.Configuration;

public class AuthConfiguration
{
    public AuthConfiguration(string email, string token)
    {
        Email = email;
        Token = token;
    }

    public string Email { get; }
    public string Token { get; }
}