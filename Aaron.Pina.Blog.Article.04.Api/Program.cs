using Microsoft.AspNetCore.Authentication.JwtBearer;
using Aaron.Pina.Blog.Article._04.Api;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Claims;

const double expiresIn = 1;

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
            return Results.BadRequest(new
            {
                Error = "User already has an active token",
                Message = "Use the /refresh endpoint with your refresh token to get a new token"
            });
        }
        var now = DateTime.UtcNow;
        var exp = now.AddMinutes(expiresIn);
        var entity = new TokenEntity
        {
            UserId = userId,
            ExpiresAt = exp,
            CreatedAt = now,
            RefreshToken = TokenGenerator.GenerateRefreshToken(),
            Token = TokenGenerator.GenerateToken(rsaKey, userId, now, expiresIn)
        };
        repository.SaveToken(entity);
        return Results.Ok(entity.ToResponse());
    })
   .AllowAnonymous();

app.MapPost("/refresh", (TokenRepository repository, HttpContext context) =>
    {
        var refreshToken = context.Request.Form["refresh_token"].FirstOrDefault();
        if (string.IsNullOrEmpty(refreshToken)) return Results.BadRequest();
        var existing = repository.TryGetByRefreshToken(refreshToken);
        if (existing is null) return Results.Unauthorized();
        var now = DateTime.UtcNow;
        var exp = now.AddMinutes(expiresIn);
        existing.ExpiresAt = exp;
        existing.CreatedAt = now;
        existing.RefreshToken = TokenGenerator.GenerateRefreshToken();
        existing.Token = TokenGenerator.GenerateToken(rsaKey, existing.UserId, now, expiresIn);
        repository.UpdateToken(existing);
        return Results.Ok(existing.ToResponse());
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
