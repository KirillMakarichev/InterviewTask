using InterviewTask.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InterviewTask.DataBase;

public class UsersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Image> Images { get; set; }
    
    public UsersContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendship>()
            .HasKey(f => new { f.UserRespondingId, f.UserRequestingId });

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.UserRequesting)
            .WithMany(u => u.Friendships)
            .HasForeignKey(f => f.UserRequestingId);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.UserResponding)
            .WithMany()
            .HasForeignKey(f => f.UserRespondingId);
        
        modelBuilder.Entity<User>()
            .HasMany(x => x.Images)
            .WithOne(x => x.Owner)
            .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}