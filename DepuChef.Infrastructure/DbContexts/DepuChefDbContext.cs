using DepuChef.Application.Constants;
using DepuChef.Application.Models;
using DepuChef.Application.Models.User;
using Microsoft.EntityFrameworkCore;

namespace DepuChef.Infrastructure.DbContexts;

public class DepuChefDbContext(DbContextOptions<DepuChefDbContext> options) : DbContext(options)
{
    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(user => user.Id);
            user.Property(user => user.Email).IsRequired();
            user.Property(user => user.SubscriptionLevel)
            .HasConversion(
                sub => sub.Value,
                sub => SubscriptionLevel.FromValue(sub));
            user.Property(user => user.ChefChoice)
            .HasConversion(
                choice => choice.Value,
                choice => ChefChoice.FromValue(choice));
        });
    }
}
