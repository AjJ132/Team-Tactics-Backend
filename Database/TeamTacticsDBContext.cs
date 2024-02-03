using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Team_Tactics_Backend.Models.Users;
using Team_Tactics_Backend.Models.Teams;
using TeamTacticsBackend.Models.CalendarEvents;

namespace Team_Tactics_Backend.Database
{
    public class TeamTacticsDBContext : IdentityDbContext
    {

        public TeamTacticsDBContext(DbContextOptions<TeamTacticsDBContext> options) : base(options)
        {
        }

         public TeamTacticsDBContext()
        {
        }

        public virtual DbSet<User> Users { get; set; }
    
        public virtual DbSet<Team> Teams { get; set; }

        public virtual DbSet<CalendarEvent> CalendarEvents{get;set;}

        public virtual DbSet<EventsAssigned> EventsAssigneds {get;set;}
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
