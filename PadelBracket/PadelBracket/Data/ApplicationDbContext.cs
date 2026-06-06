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
    public DbSet<Organizer> Organizers => Set<Organizer>();
    public DbSet<OrganizerAccount> OrganizerAccounts => Set<OrganizerAccount>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<TournamentCategory> TournamentCategories => Set<TournamentCategory>();

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

        modelBuilder.Entity<Organizer>(entity =>
        {
            entity.ToTable("Organizers");

            entity.HasKey(organizer => organizer.Id);

            entity.Property(organizer => organizer.RealName)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(organizer => organizer.Email)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(organizer => organizer.ClubName)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(organizer => organizer.City)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(organizer => organizer.Phone)
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(organizer => organizer.CreatedAt)
                .IsRequired();

            entity.Ignore(organizer => organizer.HasCompleteProfile);
        });

        modelBuilder.Entity<OrganizerAccount>(entity =>
        {
            entity.ToTable("OrganizerAccounts");

            entity.HasKey(account => account.OrganizerId);

            entity.Property(account => account.Email)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(account => account.PasswordHash)
                .IsRequired();

            entity.Property(account => account.PasswordSalt)
                .IsRequired();
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.ToTable("Tournaments");

            entity.HasKey(tournament => tournament.Id);

            entity.Property(tournament => tournament.OrganizerId);

            entity.Property(tournament => tournament.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(tournament => tournament.CreatedAt)
                .IsRequired();

            entity.Property(tournament => tournament.Status)
                .IsRequired();

            entity.Property(tournament => tournament.ClubName)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(tournament => tournament.City)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(tournament => tournament.Address)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(tournament => tournament.StartDate)
                .IsRequired();

            entity.HasMany(tournament => tournament.TournamentCategories)
                .WithOne()
                .HasForeignKey("TournamentId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(tournament => tournament.Groups);
            entity.Ignore(tournament => tournament.Registrations);
            entity.Ignore(tournament => tournament.StatusLabel);
        });

        modelBuilder.Entity<TournamentCategory>(entity =>
        {
            entity.ToTable("TournamentCategories");

            entity.HasKey(category => category.Id);

            entity.Property(category => category.Category)
                .IsRequired();

            entity.Property(category => category.MaxPairs)
                .IsRequired();

            entity.Property(category => category.RegistrationFee)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });
    }
}