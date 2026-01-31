using Microsoft.EntityFrameworkCore;
using AzureFunctionSample.Models;

namespace AzureFunctionSample.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Todo> Todos { get; set; }
}
