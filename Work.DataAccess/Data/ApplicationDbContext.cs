using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkManagementApp.Models;

namespace WorkManagementApp.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Projects> Projects { get; set; }
        public DbSet<TaskDetails> TaskDetail { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubTask>()
                .HasOne(st => st.Task)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(st => st.TaskID)
                .OnDelete(DeleteBehavior.Restrict);  // Avoid cascade paths

            modelBuilder.Entity<SubTask>()
                .HasOne(st => st.Projects)
                .WithMany()
                .HasForeignKey(st => st.ProjectID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskDetails>()
                .HasOne(td => td.Projects)
                .WithMany()
                .HasForeignKey(td => td.ProjectID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
