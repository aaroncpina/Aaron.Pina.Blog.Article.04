namespace Aaron.Pina.Blog.Article._04.Api;

public class TokenEntity
{
    public Guid     Id           { get; set; } = Guid.NewGuid();
    public string   Token        { get; set; } = string.Empty;
    public string   RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt    { get; set; }
    public DateTime CreatedAt    { get; set; } = DateTime.UtcNow;
}
