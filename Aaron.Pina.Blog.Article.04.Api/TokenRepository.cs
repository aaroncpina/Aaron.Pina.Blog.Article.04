namespace Aaron.Pina.Blog.Article._04.Api;

public class TokenRepository
{
    private readonly TokenContext _context = new();
    
    public void SaveToken(TokenEntity token)
    {
        _context.Add(token);
        _context.SaveChanges();
    }
}
