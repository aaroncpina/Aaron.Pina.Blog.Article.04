using Microsoft.EntityFrameworkCore;

namespace Aaron.Pina.Blog.Article._04.Server;

public class TokenDbContext(DbContextOptions<TokenDbContext> options) : DbContext(options)
{
    public DbSet<TokenEntity> Tokens => Set<TokenEntity>();
}
