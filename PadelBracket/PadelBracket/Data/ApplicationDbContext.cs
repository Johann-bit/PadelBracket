using Microsoft.EntityFrameworkCore;
using PadelBracket.Domain.Entities;
using System.Text.Json;

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
    public DbSet<Pair> Pairs => Set<Pair>();
    public DbSet<TournamentRegistration> TournamentRegistrations => Set<TournamentRegistration>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<KnockoutBracket> KnockoutBrackets => Set<KnockoutBracket>();
    public DbSet<KnockoutMatch> KnockoutMatches => Set<KnockoutMatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Players");
            entity.HasKey(player => player.Id);

            entity.Property(player => player.Name).HasMaxLength(80).IsRequired();
            entity.Property(player => player.Email).HasMaxLength(120).IsRequired();
            entity.Property(player => player.DominantHand);
            entity.Property(player => player.PreferredSide);
            entity.Property(player => player.Category);
            entity.Property(player => player.VerificationStatus).IsRequired();
            entity.Property(player => player.RankingPoints).IsRequired();

            entity.Ignore(player => player.IsVerified);
            entity.Ignore(player => player.HasCompleteProfile);
        });

        modelBuilder.Entity<PlayerAccount>(entity =>
        {
            entity.ToTable("PlayerAccounts");
            entity.HasKey(account => account.PlayerId);

            entity.Property(account => account.Email).HasMaxLength(120).IsRequired();
            entity.Property(account => account.PasswordHash).IsRequired();
            entity.Property(account => account.PasswordSalt).IsRequired();
            entity.Property(account => account.PasswordResetCode).HasMaxLength(6);
            entity.Property(account => account.PasswordResetRequestedAt);
        });

        modelBuilder.Entity<Organizer>(entity =>
        {
            entity.ToTable("Organizers");
            entity.HasKey(organizer => organizer.Id);

            entity.Property(organizer => organizer.RealName).HasMaxLength(80).IsRequired();
            entity.Property(organizer => organizer.Email).HasMaxLength(120).IsRequired();
            entity.Property(organizer => organizer.ClubName).HasMaxLength(120).IsRequired();
            entity.Property(organizer => organizer.City).HasMaxLength(80).IsRequired();
            entity.Property(organizer => organizer.Phone).HasMaxLength(40).IsRequired();
            entity.Property(organizer => organizer.CreatedAt).IsRequired();

            entity.Ignore(organizer => organizer.HasCompleteProfile);
        });

        modelBuilder.Entity<OrganizerAccount>(entity =>
        {
            entity.ToTable("OrganizerAccounts");
            entity.HasKey(account => account.OrganizerId);

            entity.Property(account => account.Email).HasMaxLength(120).IsRequired();
            entity.Property(account => account.PasswordHash).IsRequired();
            entity.Property(account => account.PasswordSalt).IsRequired();
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.ToTable("Tournaments");
            entity.HasKey(tournament => tournament.Id);

            entity.Property(tournament => tournament.OrganizerId);
            entity.Property(tournament => tournament.Name).HasMaxLength(120).IsRequired();
            entity.Property(tournament => tournament.CreatedAt).IsRequired();
            entity.Property(tournament => tournament.Status).IsRequired();
            entity.Property(tournament => tournament.ClubName).HasMaxLength(120).IsRequired();
            entity.Property(tournament => tournament.City).HasMaxLength(80).IsRequired();
            entity.Property(tournament => tournament.Address).HasMaxLength(200).IsRequired();
            entity.Property(tournament => tournament.StartDate).IsRequired();

            entity.HasMany(tournament => tournament.TournamentCategories)
                .WithOne()
                .HasForeignKey("TournamentId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(tournament => tournament.Registrations)
                .WithOne()
                .HasForeignKey(registration => registration.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(tournament => tournament.Groups)
                .WithOne()
                .HasForeignKey("TournamentId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(tournament => tournament.StatusLabel);
        });

        modelBuilder.Entity<TournamentCategory>(entity =>
        {
            entity.ToTable("TournamentCategories");
            entity.HasKey(category => category.Id);

            entity.Property(category => category.Category).IsRequired();
            entity.Property(category => category.MaxPairs).IsRequired();
            entity.Property(category => category.RegistrationFee)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        modelBuilder.Entity<TournamentRegistration>(entity =>
        {
            entity.ToTable("TournamentRegistrations");
            entity.HasKey(registration => registration.Id);

            entity.Property(registration => registration.TournamentId).IsRequired();

            entity.HasOne(registration => registration.Pair)
                .WithMany()
                .HasForeignKey("PairId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Property(registration => registration.Category).IsRequired();
            entity.Property(registration => registration.Status).IsRequired();
            entity.Property(registration => registration.PaymentStatus).IsRequired();
            entity.Property(registration => registration.RegisteredAt).IsRequired();
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.ToTable("Groups");
            entity.HasKey(group => group.Id);

            entity.Property(group => group.Name).HasMaxLength(80).IsRequired();
            entity.Property(group => group.Category).IsRequired();

            entity.Ignore(group => group.CategoryLabel);

            entity.HasMany(group => group.Matches)
                .WithOne()
                .HasForeignKey("GroupId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(group => group.Pairs)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "GroupPairs",
                    right => right.HasOne<Pair>()
                        .WithMany()
                        .HasForeignKey("PairId")
                        .OnDelete(DeleteBehavior.Restrict),
                    left => left.HasOne<Group>()
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("GroupPairs");
                        join.HasKey("GroupId", "PairId");
                    });
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.ToTable("Matches");
            entity.HasKey(match => match.Id);

            entity.HasOne(match => match.PairOne)
                .WithMany()
                .HasForeignKey("PairOneId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(match => match.PairTwo)
                .WithMany()
                .HasForeignKey("PairTwoId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Property(match => match.Result)
                .HasConversion(
                    result => SerializeMatchResult(result),
                    value => DeserializeMatchResult(value))
                .HasColumnName("ResultJson");

            entity.Ignore(match => match.HasResult);
            entity.Ignore(match => match.Winner);
            entity.Ignore(match => match.Loser);
        });

        modelBuilder.Entity<KnockoutBracket>(entity =>
        {
            entity.ToTable("KnockoutBrackets");
            entity.HasKey(bracket => bracket.Id);

            entity.Property(bracket => bracket.TournamentId).IsRequired();
            entity.Property(bracket => bracket.Category).IsRequired();
            entity.Property(bracket => bracket.CreatedAt).IsRequired();

            entity.HasIndex(bracket => new { bracket.TournamentId, bracket.Category })
                .IsUnique();

            entity.HasMany(bracket => bracket.Matches)
                .WithOne()
                .HasForeignKey("KnockoutBracketId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<KnockoutMatch>(entity =>
        {
            entity.ToTable("KnockoutMatches");
            entity.HasKey(match => match.Id);

            entity.Property(match => match.RoundName).HasMaxLength(80).IsRequired();
            entity.Property(match => match.SortOrder).IsRequired();

            entity.HasOne(match => match.PairOne)
                .WithMany()
                .HasForeignKey("PairOneId")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(match => match.PairTwo)
                .WithMany()
                .HasForeignKey("PairTwoId")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(match => match.Result)
                .HasConversion(
                    result => SerializeMatchResult(result),
                    value => DeserializeMatchResult(value))
                .HasColumnName("ResultJson");

            entity.Ignore(match => match.HasPairOne);
            entity.Ignore(match => match.HasPairTwo);
            entity.Ignore(match => match.IsReadyToPlay);
            entity.Ignore(match => match.HasResult);
            entity.Ignore(match => match.Winner);
            entity.Ignore(match => match.Loser);
        });

        modelBuilder.Entity<Pair>(entity =>
        {
            entity.ToTable("Pairs");
            entity.HasKey(pair => pair.Id);

            entity.HasOne(pair => pair.PlayerOne)
                .WithMany()
                .HasForeignKey("PlayerOneId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.HasOne(pair => pair.PlayerTwo)
                .WithMany()
                .HasForeignKey("PlayerTwoId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Property(pair => pair.Category);

            entity.Ignore(pair => pair.DisplayName);
        });
    }

    private static string? SerializeMatchResult(MatchResult? result)
    {
        if (result == null)
            return null;

        var sets = result.Sets
            .Select(set => new StoredMatchSet
            {
                PairOneScore = set.PairOneScore,
                PairTwoScore = set.PairTwoScore,
                IsSuperTieBreak = set.IsSuperTieBreak
            })
            .ToList();

        return JsonSerializer.Serialize(sets);
    }

    private static MatchResult? DeserializeMatchResult(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var sets = JsonSerializer.Deserialize<List<StoredMatchSet>>(value)
            ?? new List<StoredMatchSet>();

        return new MatchResult(sets
            .Select(set => new MatchSet(
                set.PairOneScore,
                set.PairTwoScore,
                set.IsSuperTieBreak))
            .ToList());
    }

    private sealed class StoredMatchSet
    {
        public int PairOneScore { get; set; }
        public int PairTwoScore { get; set; }
        public bool IsSuperTieBreak { get; set; }
    }
}