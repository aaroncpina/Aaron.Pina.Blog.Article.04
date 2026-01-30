using Microsoft.EntityFrameworkCore;

namespace Aaron.Pina.Blog.Article._04.Api;

public class TokenContext : DbContext
{
    public DbSet<TokenEntity> Tokens => Set<TokenEntity>();
}
