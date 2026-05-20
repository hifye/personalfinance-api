using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Data.Context;

public class FinTrackContext(DbContextOptions<FinTrackContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}