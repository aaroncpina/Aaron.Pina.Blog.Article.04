using System.Security.Cryptography;
using System.Buffers.Text;

namespace Aaron.Pina.Blog.Article._04.Api;

public static class RefreshTokenGenerator
{
    public static string Generate(int length = 32) =>
        Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(length));
}
