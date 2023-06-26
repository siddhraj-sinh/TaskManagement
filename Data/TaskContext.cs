using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {

        }
        public DbSet<TaskModel> Tasks { get; set; } = null!;
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TaskModel>()
               .HasOne<User>(t => t.User)
               .WithMany(u => u.Tasks)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TaskModel>().ToTable("Tasks");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Comment>().ToTable("Comment");

        }
    }
}
