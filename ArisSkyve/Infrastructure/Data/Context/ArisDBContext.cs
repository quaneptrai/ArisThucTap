using ArisSkyve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ArisSkyve.Infrastructure.Data.Context
{
    public class ArisDBContext:DbContext
    {
        public ArisDBContext(DbContextOptions<ArisDBContext> options) : base(options) { 
        

        }
        public DbSet<EmployesAccount> UserAccounts { get; set; }
        public DbSet<BussinessAccount> BussinessAccounts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostMedia> PostMedias { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Language> Languages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure one-to-one relationship between User and EmployesAccount
            modelBuilder.Entity<User>()
                .HasOne(u => u.employesAccount)
                .WithOne(e => e.User)
                .HasForeignKey<EmployesAccount>(e => e.UserId);
            // Configure one-to-one relationship between User and BussinessAccount
            modelBuilder.Entity<User>()
                .HasOne(u => u.bussinessAccount)
                .WithOne(b => b.User)
                .HasForeignKey<BussinessAccount>(b => b.UserId);
            // Configure one-to-many relationship between User and Post
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);
            // Configure one-to-many relationship between Post and PostMedia
            modelBuilder.Entity<Post>()
                .HasMany(p => p.postMedias)
                .WithOne(pm => pm.Post)
                .HasForeignKey(pm => pm.PostId);
        }   
    }
}
