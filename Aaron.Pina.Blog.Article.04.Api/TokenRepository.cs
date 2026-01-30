namespace Aaron.Pina.Blog.Article._04.Api;

public class TokenRepository(TokenDbContext dbContext)
{
    public void SaveToken(TokenEntity token)
    {
        dbContext.Add(token);
        dbContext.SaveChanges();
    }

    public void UpdateToken(TokenEntity token)
    {
        dbContext.Update(token);
        dbContext.SaveChanges();
    }

    public TokenEntity? TryGetByUserId(Guid userId) =>
        dbContext.Tokens.FirstOrDefault(t => t.UserId == userId);

    public TokenEntity? TryGetByRefreshToken(string refreshToken) =>
        dbContext.Tokens.FirstOrDefault(t => t.RefreshToken == refreshToken);
}
