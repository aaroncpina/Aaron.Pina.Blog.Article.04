namespace Aaron.Pina.Blog.Article._04.Api;

public class TokenEntity
{
    public Guid     Id           { get; init; } = Guid.NewGuid();
    public Guid     UserId       { get; init; }
    public string   Token        { get; set; } = string.Empty;
    public string   RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt    { get; set; }
    public DateTime CreatedAt    { get; set; }
}
