namespace Aaron.Pina.Blog.Article._04.Api;

public class TokenEntity
{
    public Guid     Id           { get; init; } = Guid.NewGuid();
    public Guid     UserId       { get; init; }
    public string   Token        { get; init; } = string.Empty;
    public string   RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt    { get; init; }
    public DateTime CreatedAt    { get; init; }
}
