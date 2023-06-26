using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {

        }
        public DbSet<TaskManagement.Models.TaskModel> Tasks { get; set; } = null!;
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TaskModel>()
               .HasOne<User>(t => t.User)
               .WithMany(u => u.Tasks)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TaskManagement.Models.TaskModel>().ToTable("Tasks");
            modelBuilder.Entity<User>().ToTable("Users");

        }
    }
}
