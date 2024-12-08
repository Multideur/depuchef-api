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
            user.Property(user => user.ChefPreference)
            .HasConversion(
                choice => choice.Value,
                choice => ChefChoice.FromValue(choice));
        });

        modelBuilder.Entity<Recipe>(recipe =>
        {
            recipe.Navigation(recipe => recipe.Ingredients).AutoInclude();
            recipe.Navigation(recipe => recipe.Instructions).AutoInclude();
            recipe.Navigation(recipe => recipe.Notes).AutoInclude();
            recipe.HasKey(recipe => recipe.Id);
            recipe.Property(recipe => recipe.Title).IsRequired();
            recipe.Property(recipe => recipe.Description).IsRequired();
            recipe.Property(recipe => recipe.PrepTime).IsRequired();
            recipe.Property(recipe => recipe.CookTime).IsRequired();
            recipe.Property(recipe => recipe.TotalTime).IsRequired();
            recipe.Property(recipe => recipe.Servings).IsRequired();
            recipe.Property(recipe => recipe.Confidence).HasPrecision(4,3);
            recipe.Property(recipe => recipe.Rating);
            recipe.HasMany(recipe => recipe.Ingredients).WithOne().IsRequired();
            recipe.HasMany(recipe => recipe.Instructions).WithOne().IsRequired();
            recipe.HasMany(recipe => recipe.Notes).WithOne();
        });
    }
}
