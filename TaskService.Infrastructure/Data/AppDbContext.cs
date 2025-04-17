using Microsoft.EntityFrameworkCore;
using TaskService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Core.Models.Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Core.Models.Task>()
                .HasIndex(t => t.AssignedTo);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
