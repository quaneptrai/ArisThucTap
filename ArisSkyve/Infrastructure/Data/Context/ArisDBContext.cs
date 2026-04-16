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
        public DbSet<JobLocation> JobLocations { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserRecommendations> UserRecommendations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // JobPosting <-> BussinessAccount
            modelBuilder.Entity<JobPosting>()
                .HasOne(j => j.BusinessAccount)
                .WithMany()
                .HasForeignKey(j => j.BusinessAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            // JSON string fields
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
                .Property(j => j.FullText)
                .HasColumnType("nvarchar(max)");
            modelBuilder.Entity<JobPosting>()
                .Property(j => j.LocationTags)
                .HasColumnType("nvarchar(max)"); 
            modelBuilder.Entity<JobLocation>()
                .HasOne(jl => jl.JobPosting)
                .WithMany(j => j.Locations)
                .HasForeignKey(jl => jl.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JobLocation>()
                .HasIndex(jl => jl.City);


            modelBuilder.Entity<ContactMethod>()
                .HasOne(c => c.EmployesAccount)
                .WithMany(p => p.ContactMethods)
                .HasForeignKey(c => c.idEmployesAccount)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Resume>()
                .HasIndex(r => r.EmployeeId)
                .IsUnique();
            // Discriminator cho Profiles
            modelBuilder.Entity<EmployesAccount>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<EmployesAccount>("EmployesAccount")
                .HasValue<BussinessAccount>("BussinessAccount");

            modelBuilder.Entity<Like>()
                .HasKey(l => new { l.UserId, l.PostId });

            // ✅ Like → Post (N:1)
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            // ✅ Like → User (N:1)
            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User <-> Profile
            modelBuilder.Entity<User>()
                    .HasOne(u => u.Profile)
                    .WithOne(p => p.User)
                    .HasForeignKey<EmployesAccount>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                //Like constraint 
                modelBuilder.Entity<Like>()
                    .HasKey(l => new { l.UserId, l.PostId });
        }
    }
}