using Microsoft.EntityFrameworkCore;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Domain.Aggregates.Diagnoses;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Infrastructure.Services;
using FluentResponse;

namespace CesiZen.Infrastructure.Core;
public sealed class ApplicationDbContext : DbContext {

    #region PROPERTIES

        private readonly IEncryptionService encryptionService = null!;

        public DbSet<Admin> Admins { get; init; }
        public DbSet<User>  Users  { get; init; }

        public DbSet<Category> Categories { get; init; }
        public DbSet<Page>     Pages      { get; init; }

        public DbSet<DiagnosisAnalysis> DiagnosisAnalyses { get; init; }
        public DbSet<DiagnosisItem>     DiagnosisItems    { get; init; }

    #endregion
    #region CONSTRUCTORS

        public ApplicationDbContext() {}
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IEncryptionService                     encryptionService
        ) : base(options) {
            this.encryptionService = encryptionService;
        }

    #endregion
    #region METHODS

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(
                    "Host=localhost:5432;Database=root;Username=root;Password=root"
                );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            base.OnModelCreating(modelBuilder);

            // Automatically ignore aggregate roots' domain events from the mapping.
            foreach (var entityType in modelBuilder.Model
                .GetEntityTypes()
                .Where(x => x.ClrType.BaseType?.IsGenericType == true && (x.ClrType.BaseType?.GetGenericTypeDefinition() == typeof(AggregateRoot<>)))
            ) entityType.AddIgnored(nameof(AggregateRoot<>.DomainEvents));

            modelBuilder.Entity<Admin>(e => {
                e.HasIndex(x => x.MailAddress).IsUnique();
                e.Property(x => x.MailAddress).HasConversion(
                    x => this.encryptionService.Encrypt(x.Address),
                    x => new (this.encryptionService.Decrypt(x))
                );
                e.Property(x => x.Password).HasConversion(x => x.Hash, x => new (x));
            });

            modelBuilder.Entity<User>(e => {
                e.HasIndex(x => x.MailAddress).IsUnique();
                e.Property(x => x.MailAddress).HasConversion(
                    x => x != null ? this.encryptionService.Encrypt(x.Address) : null,
                    x => x != null ? new (this.encryptionService.Decrypt(x)) : null
                );

                e.Property(x => x.Password).HasConversion(x => x.Hash, x => new (x));
                e.ComplexProperty(x => x.FirstDiagnosisResult, result => {
                    result.Property(x => x.Score).HasConversion(
                        x => this.encryptionService.Encrypt(x.ToString()),
                        x => int.Parse(this.encryptionService.Decrypt(x))
                    );
                });

                e.ComplexProperty(x => x.LastDiagnosisResult, result => {
                    result.Property(x => x.Score).HasConversion(
                        x => this.encryptionService.Encrypt(x.ToString()),
                        x => int.Parse(this.encryptionService.Decrypt(x))
                    );
                });
            });


            modelBuilder.Entity<Category>(e => {
                e.HasIndex(x => x.Title).IsUnique();
                e.Property(x => x.Title).HasConversion(x => x.Value, x => new (x));
            });

            modelBuilder.Entity<Page>().Property(x => x.Title).HasConversion(x => x.Value, x => new (x));
            modelBuilder.Entity<DiagnosisItem>().Property(x => x.EventLabel).HasConversion(x => x.Value, x => new (x));
            modelBuilder.Entity<DiagnosisAnalysis>().HasIndex(x => x.ScoreThreshold).IsUnique();

        }

    #endregion
}