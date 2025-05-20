using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

   
    public DbSet<ToDoItem> ToDoItems { get; set; }


    public DbSet<Status> Statuses { get; set; }

    public async Task TransferTasksToArchiveAsync(string userId)
    {
        await Database.ExecuteSqlRawAsync("EXEC TransferTasksToArchive @UserId = {0}", userId);
    }

    public async Task TransferTasksToActiveAsync(string userId)
    {
        await Database.ExecuteSqlRawAsync("EXEC TransferTasksToActive @UserId = {0}", userId);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        builder.Entity<ToDoItem>()
            .HasOne(item => item.User)
            .WithMany()
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<ToDoItem>()
            .HasOne(item => item.Status)
            .WithMany()
            .HasForeignKey(item => item.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}