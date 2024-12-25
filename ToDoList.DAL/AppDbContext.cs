using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Entity;

namespace ToDoList.DAL
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasOne(a => a.Task)
                .WithOne(t => t.AppUser)
                .HasForeignKey<TaskEntity>(t => t.AppUserId)
                .IsRequired(false); 
        }

        public DbSet<TaskEntity> Tasks { get; set; }
        
        
    }
}