using ArisSkyve.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArisSkyve.Infrastructure.Data.Context
{
    public class ArisDBContext : DbContext
    {
        public ArisDBContext(DbContextOptions<ArisDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<EmployesAccount> Profiles { get; set; }
        public DbSet<BussinessAccount> BussinessAccounts { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostMedias> PostMedias { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<Experiences> Experiences { get; set; }
        public DbSet<ContactMethod> ContactMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // JobPosting <-> BussinessAccount
            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.BusinessAccount)
                .WithMany()
                .HasForeignKey(j => j.BusinessAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            // JSON fields
            modelBuilder.Entity<JobPosting>()
                .Property(j => j.Locations)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.Responsibilities)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.Requirements)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.Benefits)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.Tags)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.FullText)
                .HasColumnType("nvarchar(max)");
        }
    }
}