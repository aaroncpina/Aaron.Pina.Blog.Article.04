using Microsoft.EntityFrameworkCore;

namespace Aaron.Pina.Blog.Article._04.Api;

public class TokenRepository(DbContextOptions<TokenDbContext> options)
{
    private readonly TokenDbContext _dbContext = new(options);
    
    public void SaveToken(TokenEntity token)
    {
        _dbContext.Add(token);
        _dbContext.SaveChanges();
    }

    public TokenEntity? TryGetByUserId(Guid userId) =>
        _dbContext.Tokens.FirstOrDefault(t => t.UserId == userId);
}
