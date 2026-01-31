using Aaron.Pina.Blog.Article._04.Shared;

namespace Aaron.Pina.Blog.Article._04.Api;

public static class TokenExtensions
{
    extension(TokenEntity token)
    {
        public TokenResponse ToResponse() => 
            new(token.Token, token.RefreshToken, token.ExpiresAt.Subtract(DateTime.UtcNow).TotalSeconds);
    }
    
    extension(TokenResponse response)
    {
        public TokenEntity ToEntity(Guid userId = default) => 
            new()
            {
                UserId = userId,
                Token = response.Token,
                RefreshToken = response.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(response.ExpiresIn),
            };
    }
}
