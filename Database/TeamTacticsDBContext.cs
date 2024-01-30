using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Team_Tactics_Backend.Models.Users;
using Team_Tactics_Backend.Models.Teams;

namespace Team_Tactics_Backend.Database
{
    public class TeamTacticsDBContext : IdentityDbContext
    {

        public TeamTacticsDBContext(DbContextOptions<TeamTacticsDBContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
