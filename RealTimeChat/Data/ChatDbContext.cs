using Microsoft.EntityFrameworkCore;
using RealTimeChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RealTimeChat.Data
{
    public class ChatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserMessages> UsersMessages { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<Message>()
               .HasKey(m => m.Id);
            modelBuilder.Entity<UserMessages>()
               .HasKey(m => m.Id);

            modelBuilder.Entity<UserMessages>().Property(p => p.Messages)
            .HasConversion(
                v => JsonSerializer.Serialize(v, default),
                v => JsonSerializer.Deserialize<List<string>>(v, default));
        }
    }
}
