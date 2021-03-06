﻿using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Mvc4WebRole.Models;

namespace Mvc4WebRole
{
    public class RecipeDbContext : DbContext
    {
        public DbSet<RecipeModel> Recipes { get; set; }
        public DbSet<IngredientModel> Ingredients { get; set; }
        public DbSet<TagModel> Tags { get; set; }

        public RecipeDbContext()
            : base("DefaultConnection")
        {
            //   this.Database.Log = SessionLogger.AddLog;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //----------------------------------------------------------------
            //Creating a Association (Intermediate Table) which
            //will hold M2M relations 
            //----------------------------------------------------------------
            modelBuilder.Entity<TagModel>()
                .HasMany(c => c.Recipes)
                .WithMany(s => s.Tags)
                .Map(mc =>
                    {
                        mc.ToTable("RecipeTagMap");
                        mc.MapLeftKey("RecipeID");
                        mc.MapRightKey("TagID");
                    });
        }
    }
}