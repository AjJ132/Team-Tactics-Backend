using TeamTacticsBackend.Models.CalendarEvents;
using Team_Tactics_Backend.Database;
using Team_Tactics_Backend.Models.Teams;
using Team_Tactics_Backend.Models.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;


namespace Team_Tactics_Backend.CalendarEventsData
{

public interface CALDAL{
     public List<CalendarEvent> GetEvents();
        public List<CalendarEvent> Getcalevents(string userid);
        public CalendarEvent GetEvent(Guid id);
        public void CreateEvent(IFormCollection form);
        public void UpdateEvent(IFormCollection form, Guid Id);
        public void DeleteEvent(Guid id);
}

public class DAL : CALDAL
    {
        private TeamTacticsDBContext db = new TeamTacticsDBContext();

        public List<CalendarEvent> GetEvents()
        {
            return db.CalendarEvents.ToList();
        }

        public List<CalendarEvent> Getcalevents(string userid)
        {
            return db.CalendarEvents.Where(x => x.User.Id == userid).ToList();
        }

        public CalendarEvent GetEvent(Guid id)
        {
            return db.CalendarEvents.FirstOrDefault(x => x.EventId == id);
        }

        public void CreateEvent(IFormCollection form)
        {
            var title = form["Title"].ToString();
            var user = db.Users.FirstOrDefault(x => x.Id == form["UserId"].ToString());
            var newevent = new CalendarEvent(form, user);
            db.CalendarEvents.Add(newevent);
            db.SaveChanges();
        }

        public void UpdateEvent(IFormCollection form, Guid Id)
        {
            var eventid = int.Parse(form["Event.Id"]);
            var calevent = db.CalendarEvents.FirstOrDefault(x => x.EventId == Id);
            var user = db.Users.FirstOrDefault(x => x.Id == form["UserId"].ToString());
            calevent.UpdateEvent(form, user);
            db.Entry(calevent).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteEvent(Guid id)
        {
            var calevent = db.CalendarEvents.Find(id);
            db.CalendarEvents.Remove(calevent);
            db.SaveChanges();
        }

    }
}

