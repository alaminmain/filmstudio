using Filmstudion.Authentication;
using Filmstudion.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Filmstudion
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<User> ApplicationUsers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //---------------SEED TESTUSERS-----------------------//
            builder.Entity<User>().HasData(new User
            {
                UserName = "test@test.se",
                Email = "test@test.se",
                IsAdmin = false,
                Password = "P@ssw0rd!1"
            });
        }
    }
}
