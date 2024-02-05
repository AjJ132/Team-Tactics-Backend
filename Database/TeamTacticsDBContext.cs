using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using TeamTacticsBackend.Models.Users;
using TeamTacticsBackend.Models.Teams;
using TeamTacticsBackend.Models.CalendarEvents;

namespace TeamTacticsBackend.Database
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
