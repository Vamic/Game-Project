using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace GameProject.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("DisplayName", this.DisplayName));
            userIdentity.AddClaim(new Claim("Level", this.Level.ToString()));
            
            return userIdentity;
        }

        public string DisplayName { get; set; }
        public int Experience { get; set; }
        public int Level {
            get {
                return 1 + (int)((double)Experience / 1000);
            }
        }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Deaths { get; set; }

        public UserScore Score { get; set; }

        public void GainExp(int exp)
        {
            Experience += exp;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gladiator>()
                .HasMany(g => g.MatchesAsGladiator)
                .WithRequired(m => m.Gladiator)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Gladiator>()
                .HasMany(g => g.MatchesAsOpponent)
                .WithRequired(m => m.Opponent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasRequired(f => f.Score)
                .WithRequiredPrincipal(s => s.User);

            modelBuilder.Entity<Gladiator>()
                .HasRequired(f => f.Score)
                .WithRequiredPrincipal(s => s.Gladiator);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Gladiator> Gladiators { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Turn> Turns { get; set; }
        public DbSet<UserScore> UserScores { get; set; }
        public DbSet<GladiatorScore> GladiatorScores { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}