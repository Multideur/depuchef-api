using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DepuChef.Infrastructure.DbContexts;

public class DepuChefDbContextFactory : IDesignTimeDbContextFactory<DepuChefDbContext>
{
    public DepuChefDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DepuChefDbContext>();

        optionsBuilder.UseSqlServer("Server=database;Database=myApp;User=sa;Password=MyPassword1;TrustServerCertificate=True;");

        return new DepuChefDbContext(optionsBuilder.Options);
    }
}