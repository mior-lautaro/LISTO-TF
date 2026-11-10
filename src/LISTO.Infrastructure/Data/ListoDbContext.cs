using Microsoft.EntityFrameworkCore;

namespace LISTO.Infrastructure.Data;

public class ListoDbContext : DbContext
{
    public ListoDbContext(DbContextOptions<ListoDbContext> options)
        : base(options)
    {
    }
}