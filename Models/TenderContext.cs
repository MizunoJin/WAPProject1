using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WADProject1.Models;

public class TenderContext : IdentityDbContext<IdentityUser>
{
    public TenderContext(DbContextOptions<TenderContext> options)
        : base(options)
    { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Swipe> Swipes { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Chat> Chats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Chat>()
            .HasOne(e => e.Sender)
            .WithMany(e => e.SentMessages)
            .HasForeignKey(cu => cu.SenderId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder
            .Entity<Chat>()
            .HasOne(e => e.Receiver)
            .WithMany(e => e.ReceivedMessages)
            .HasForeignKey(cu => cu.ReceiverId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder
            .Entity<Match>()
            .HasOne(e => e.Sender)
            .WithMany(e => e.SentMatches)
            .HasForeignKey(cu => cu.SenderId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder
            .Entity<Match>()
            .HasOne(e => e.Receiver)
            .WithMany(e => e.ReceivedMatches)
            .HasForeignKey(cu => cu.ReceiverId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder
            .Entity<Swipe>()
            .HasOne(e => e.Sender)
            .WithMany(e => e.SentSwipes)
            .HasForeignKey(cu => cu.SenderId)
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder
            .Entity<Swipe>()
            .HasOne(e => e.Receiver)
            .WithMany(e => e.ReceivedSwipes)
            .HasForeignKey(cu => cu.ReceiverId)
            .OnDelete(DeleteBehavior.ClientCascade);

        base.OnModelCreating(modelBuilder);
    }
}
