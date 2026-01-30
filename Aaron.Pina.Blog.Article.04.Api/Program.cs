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
builder.Services.AddDbContext<TokenDbContext>(Configuration.DbContext.Options);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<TokenDbContext>().Database.EnsureCreated();

app.MapGet("/register", () => Results.Ok(Guid.NewGuid()))
   .AllowAnonymous();

app.MapGet("/token", (TokenRepository repository, Guid userId) =>
    {
        var existing = repository.TryGetByUserId(userId);
        if (existing is not null)
        {
            if (existing.ExpiresAt < DateTime.UtcNow)
            {
                return Results.BadRequest(new
                {
                    Error = "User already has an active token",
                    Message = "Use the /refresh endpoint to get a new token"
                });
            }
            return Results.Ok(existing.ToResponse());
        }
        var now = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = now,
            Expires = now.AddMinutes(1),
            Issuer = "https://localhost",
            Audience = "https://localhost",
            Subject = new ClaimsIdentity([new Claim("sub", userId.ToString())]),
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
    })
   .AllowAnonymous();

app.MapGet("/user", (HttpContext context) =>
    {
        var expiry = long.TryParse(context.User.FindFirstValue("exp"), out var num)
            ? DateTimeOffset.FromUnixTimeSeconds(num)
            : DateTimeOffset.MinValue;
        return Results.Ok(new
        {
            UserId = context.User.FindFirstValue("sub"),
            Now = DateTime.UtcNow.ToString("o"),
            Exp = expiry.ToString("o")
        });
    })
   .RequireAuthorization();

app.Run();
