
using Filmstudion.Models;
using Filmstudion.Models.Film;
using Filmstudion.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Filmstudion
{
    public class AppDbContext : DbContext
    {
        public DbSet<FilmStudio> FilmStudios { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<FilmCopy> FilmCopies { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<User> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //---------------FILMSTUDIOS-----------------------//
            builder.Entity<FilmStudio>().HasData(new FilmStudio
            {
                FilmStudioId = 1,
                Name = "Röda Kvarn",
                City = "Helsingborg"

            });

            builder.Entity<FilmStudio>().HasData(new FilmStudio
            {
                FilmStudioId = 2,
                Name = "Sagabiografen",
                City = "Höganäs"

            });

            //---------------USERS-----------------------//
            builder.Entity<User>().HasData(new User
            {
                FilmStudioId = 1,
                Email = "olbin@data.com",
                IsAdmin = false,
                Id = 1,
                Password = "P@ssw0rd!1"
            });

            builder.Entity<User>().HasData(new User
            {
                FilmStudioId = 2,
                Email = "rafiq@data.com",
                IsAdmin = false,
                Id = 2,
                Password = "P@ssw0rd!2"
            });

            //---------------MOVIES-------------------//
            builder.Entity<Film>().HasData(new Film
            {
                FilmId = 1,
                Name = "La jetée (Terassen)",
                Director = "Chris Marker",
                Country = "Frankrike",
                ReleaseDate = System.DateTime.Now,
                NumberOfCopies = 2
            });

            builder.Entity<Film>().HasData(new Film
            {
                FilmId = 2,
                Name = "For Sama (Till min dotter)",
                Director = "Waad Al-Kateab, Edward Watts",
                Country = "Storbritannien",
                ReleaseDate = System.DateTime.Now,
                NumberOfCopies = 4
            });

            builder.Entity<Film>().HasData(new Film
            {
                FilmId = 3,
                Name = "In the Mood for Love",
                Director = "Wong Kar-Wai",
                Country = "Kina",
                ReleaseDate = System.DateTime.Now,
                NumberOfCopies = 3
            });

            //---------------FILMCOPIES---------------------//
            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 1.1,
                FilmId = 1,
                RentedOut = true,
                FilmStudioId = 1
            });

            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 1.2,
                FilmId = 1,
                RentedOut = false,
                FilmStudioId = 0
            });

            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 2.1,
                FilmId = 2,
                RentedOut = false,
                FilmStudioId = 0
            });
            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 2.2,
                FilmId = 2,
                RentedOut = false,
                FilmStudioId = 0
            });
            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 2.3,
                FilmId = 2,
                RentedOut = true,
                FilmStudioId = 3
            });
            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 2.4,
                FilmId = 2,
                RentedOut = false,
                FilmStudioId = 0
            });

            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 3.1,
                FilmId = 3,
                RentedOut = false,
                FilmStudioId = 0
            });
            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 3.2,
                FilmId = 3,
                RentedOut = true,
                FilmStudioId = 2
            });
            builder.Entity<FilmCopy>().HasData(new FilmCopy
            {
                FilmCopyId = 3.3,
                FilmId = 3,
                RentedOut = true,
                FilmStudioId = 1
            });
        }
    }
}
