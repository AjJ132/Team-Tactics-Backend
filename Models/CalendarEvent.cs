using System;
using System.ComponentModel.DataAnnotations;
﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Team_Tactics_Backend.Models.Teams;
using Team_Tactics_Backend.Models.Users;

namespace TeamTacticsBackend.Models.CalendarEvents;
    public class CalendarEvent
    {
        [Key]
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate {get; set; }
        public DateTime EndDate { get; set; }
        public string Color { get; set; }
        public string CreatorId { get; set; }
        public DateTime DateCreated { get; set; }

        public CalendarEvent()
        {
            EventId = Guid.NewGuid();
            DateCreated = DateTime.Now;
        }

        public virtual User User { get; set; }
        public virtual Team Team { get; set; }

        public CalendarEvent(IFormCollection form, User user)
        {
            User = user;
            Title = form["CalendarEvent.Title"].ToString();
            Description = form["CalendarEvent.Description"].ToString();
            StartDate = DateTime.Parse(form["CalendarEvent.StartTime"].ToString());
            EndDate = DateTime.Parse(form["CalendarEvent.EndTime"].ToString());

        }

         public void UpdateEvent(IFormCollection form, User user)
        {
            User = user;
            Title = form["CalendarEvent.Title"].ToString();
            Description = form["CalendarEvent.Description"].ToString();
            StartDate = DateTime.Parse(form["CalendarEvent.StartDate"].ToString());
            EndDate = DateTime.Parse(form["CalendarEvent.EndDate"].ToString());
        }

    }