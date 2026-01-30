using System.Security.Cryptography;
using System.Buffers.Text;

namespace Aaron.Pina.Blog.Article._04.Api;

public static class TokenGenerator
{
    public static string GenerateRefreshToken(int length = 32) =>
        Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(length));
}
