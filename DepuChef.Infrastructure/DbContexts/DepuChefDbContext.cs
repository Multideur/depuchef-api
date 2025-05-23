﻿using DepuChef.Application.Constants;
using DepuChef.Application.Models;
using DepuChef.Application.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DepuChef.Infrastructure.DbContexts;

public class DepuChefDbContext(DbContextOptions<DepuChefDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RecipeProcess> RecipeProcesses { get; set; } = null!;
    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<AdminUser> AdminUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(user => user.Id);
            user
            .HasMany(user => user.Recipes)
            .WithOne()
            .HasForeignKey(recipe => recipe.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            user.Property(user => user.Email).IsRequired();
            user.Property(user => user.SubscriptionLevel)
            .HasConversion(
                sub => sub.Value,
                sub => SubscriptionLevel.FromValue(sub));
            user.Property(user => user.ChefPreference)
            .HasConversion(
                choice => choice.Value,
                choice => ChefChoice.FromValue(choice));

            user.Property(u => u.CreatedAt)
                        .HasDefaultValueSql("(GETUTCDATE())")
                        .IsRequired()
                        .Metadata
                        .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            user.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("(GETUTCDATE())")
            .IsRequired();
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
            recipe.HasMany(recipe => recipe.Ingredients)
                .WithOne()
                .HasForeignKey(ingredient => ingredient.RecipeId)
                .IsRequired();
            recipe.HasMany(recipe => recipe.Instructions)
                .WithOne()
                .HasForeignKey(instruction => instruction.RecipeId)
                .IsRequired();
            recipe.HasMany(recipe => recipe.Notes)
                .WithOne()
                .HasForeignKey(note => note.RecipeId);

            recipe.Property(u => u.CreatedAt)
            .HasDefaultValueSql("(GETUTCDATE())")
            .IsRequired()
            .Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            recipe.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("(GETUTCDATE())")
            .IsRequired();
        });

        modelBuilder.Entity<Ingredient>(ingredient =>
        {
            ingredient.Navigation(ingredient => ingredient.Items).AutoInclude();
            ingredient.Navigation(ingredient => ingredient.HealthySubstitutions).AutoInclude();
            ingredient.HasKey(ingredient => ingredient.Id);
            ingredient.HasMany(ingredient => ingredient.Items).WithOne().IsRequired();
            ingredient.HasMany(ingredient => ingredient.HealthySubstitutions).WithOne().IsRequired();
        });

        modelBuilder.Entity<RecipeProcess>(process =>
        {
            process.HasKey(process => process.Id);
            process.Property(process => process.ThreadId).IsRequired();

            process.Property(u => u.CreatedAt)
            .HasDefaultValueSql("(GETUTCDATE())")
            .IsRequired()
            .Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            process.Property(u => u.UpdatedAt)
            .HasDefaultValueSql("(GETUTCDATE())")
            .IsRequired();
        });
    }
}
