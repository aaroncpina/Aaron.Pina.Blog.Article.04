namespace Aaron.Pina.Blog.Article._04.Client;

public class TokenStore
{
    public string?  Token        { get; set; }
    public string?  RefreshToken { get; set; }
    public DateTime Expiry       { get; set; }
}
