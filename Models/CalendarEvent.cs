using System;
using System.ComponentModel.DataAnnotations;
﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamTacticsBackend.Models.CalendarEvents;
    public class CalendarEvent
    {
        [Key]
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartDate {get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Color { get; set; }
        public string CreatorId { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public CalendarEvent()
        {
            
        }
    }