using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.Linq.Mapping;
using System.IO;

namespace EpisodesRatingModel
{
    public partial class EpisodesRatingContext : DbContext
    {
        public DbSet<Series> Series { get; set; }

        public DbSet<Episode> Episodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("applicationsettings.json")
                   .Build();

                var connectionString = configuration.GetConnectionString("EpisodesRating");

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Series>()
                .HasIndex(u => u.ImdbId).IsUnique();
        }
    }
}
