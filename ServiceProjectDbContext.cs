using Microsoft.EntityFrameworkCore;
using ServiceProject_ServerSide.Models;
using System.Runtime.CompilerServices;

namespace ServiceProject_ServerSide
{
    public class ServiceProjectDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ServiceProjectDbContext(DbContextOptions<ServiceProjectDbContext> context) : base(context)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed data with Users
            modelBuilder.Entity<User>().HasData(new User[]
            {
        new User {Id = 1, FirstName = "Maggie", LastName= "Chafee", Uid="4d56asd6", Email="User1@yahoo.com", PhoneNumber="615-456-4544", ProfilePic="Url.com", isStaff = true},
        new User {Id = 2, FirstName = "Kyle", LastName= "Blunt", Uid="4d56asd6", Email="User2@yahoo.com", PhoneNumber="615-456-8956", ProfilePic="Url.com", isStaff = true},
        new User {Id = 3, FirstName = "Joey", LastName= "Ebach", Uid="4d56asd6", Email="User3@yahoo.com", PhoneNumber="615-658-1452", ProfilePic="Url.com", isStaff = true},
            });

            modelBuilder.Entity<Project>().HasData(new Project[]
    {
         new Project {Id = 1, Name = "Project One", Description = "This is the first Project", Location ="Nashville", Duration=3, Date="12/01/2023", CategoryId=1, Image="url.com"},
         new Project {Id = 2, Name = "Project Two", Description = "This is the second Project", Location ="Clarksville", Duration=3, Date="11/01/2023", CategoryId=2, Image="url.com"},

    });
            modelBuilder.Entity<Category>().HasData(new Category[]
{
        new Category {Id = 1, Type = "Category One"},
        new Category {Id = 2, Type = "Category Two"},
});

        }
    }
}
