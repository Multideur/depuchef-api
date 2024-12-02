using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DepuChef.Infrastructure.DbContexts;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<DepuChefDbContext>
{
    public DepuChefDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DepuChefDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost;Database=DepuChef;User Id=sa;Password=Password123;");

        return new DepuChefDbContext(optionsBuilder.Options);
    }
}