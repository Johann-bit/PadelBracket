using Microsoft.EntityFrameworkCore;
using PadelBracket.Domain.Entities;

namespace PadelBracket.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerAccount> PlayerAccounts => Set<PlayerAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Players");

            entity.HasKey(player => player.Id);

            entity.Property(player => player.Name)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(player => player.Email)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(player => player.DominantHand);

            entity.Property(player => player.PreferredSide);

            entity.Property(player => player.Category);

            entity.Property(player => player.VerificationStatus)
                .IsRequired();

            entity.Property(player => player.RankingPoints)
                .IsRequired();

            entity.Ignore(player => player.IsVerified);
            entity.Ignore(player => player.HasCompleteProfile);
        });
        modelBuilder.Entity<PlayerAccount>(entity =>
        {
            entity.ToTable("PlayerAccounts");

            entity.HasKey(account => account.PlayerId);

            entity.Property(account => account.Email)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(account => account.PasswordHash)
                .IsRequired();

            entity.Property(account => account.PasswordSalt)
                .IsRequired();

            entity.Property(account => account.PasswordResetCode)
                .HasMaxLength(6);

            entity.Property(account => account.PasswordResetRequestedAt);
        });
    }
}