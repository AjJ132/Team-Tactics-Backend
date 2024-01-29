using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Team_Tactics_Backend.Models.DB_Testing;

namespace Team_Tactics_Backend.Database
{
    public class TeamTacticsDBContext : IdentityDbContext
    {

        public TeamTacticsDBContext(DbContextOptions<TeamTacticsDBContext> options) : base(options)
        {
        }

        public virtual DbSet<DBTesting> DBTestings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
