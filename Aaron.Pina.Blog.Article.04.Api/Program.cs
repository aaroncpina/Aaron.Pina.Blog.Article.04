using static System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using TokenContext = Aaron.Pina.Blog.Article._04.Api.TokenContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Aaron.Pina.Blog.Article._04.Api;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Claims;

using var rsa = RSA.Create(2048);
var rsaKey = new RsaSecurityKey(rsa);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(Configuration.JwtBearer.Options(rsa));
builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenRepository>();
builder.Services.AddDbContext<TokenContext>(Configuration.DbContext.Options);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/user", (HttpContext context) => context.User.FindFirstValue(Sub))
   .RequireAuthorization();

app.MapGet("/token", (TokenRepository repository) =>
{
    var now = DateTime.UtcNow;
    var userId = Guid.NewGuid(); 
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        IssuedAt = now,
        Expires = now.AddMinutes(5),
        Issuer = "https://localhost",
        Audience = "https://localhost",
        Subject = new ClaimsIdentity([new Claim(Sub, userId.ToString())]),
        SigningCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256)
    };
    var handler = new JwtSecurityTokenHandler();
    var token = handler.CreateToken(tokenDescriptor);
    var entity = new TokenEntity
    {
        UserId = userId,
        Token = handler.WriteToken(token),
        ExpiresAt = tokenDescriptor.Expires.Value,
        CreatedAt = tokenDescriptor.IssuedAt.Value,
        RefreshToken = TokenGenerator.GenerateRefreshToken()
    };
    repository.SaveToken(entity);
    return Results.Ok(entity.ToResponse());
}).AllowAnonymous();

app.Run();
