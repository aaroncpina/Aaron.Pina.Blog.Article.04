namespace Aaron.Pina.Blog.Article._04.Api;

public record TokenResponse(string Token, string RefreshToken, double ExpiresIn);
