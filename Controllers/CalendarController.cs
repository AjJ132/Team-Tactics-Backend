
﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Team_Tactics_Backend.Database;
using Team_Tactics_Backend.Models.Users;
using TeamTacticsBackend.Models.CalendarEvents;
using Team_Tactics_Backend.CalendarEventsData;
using Team_Tactics_Backend.Controllers;


namespace Team_Tactics_Backend.CalendarControllers{

    [ApiController]
    [Route("[controller]")]
    public class CalendarController : ControllerBase
    {

        private readonly CALDAL dal;
         private readonly UserManager<User> usermanage;

       public CalendarController(CALDAL dal, UserManager<User> UserManager)
        {
            dal = dal;
            usermanage = UserManager;
        }
         public IActionResult Description(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = dal.GetEvent((Guid)id);
            if (@event == null)
            {
                return NotFound();
            }

           // return event in the calendar;
        }
        
          public IActionResult Create()
        {
            //return new event in calendar;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(CalendarEvent ce, IFormCollection form)
        {
            try
            {
               //create an event dal.CreateEvent(form);
                //send an alert saying event was created TempData["Alert"] = "Success! An event was created!";
                return RedirectToAction("Index");
            } catch(Exception ex)
            {
               //send an error message if an error occured ViewData["Alert"] = "An error occurred: " + ex.Message;
               // return the original event;
            }
        }

        // GET: Event/Edit/
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = dal.GetEvent((Guid)id);
            if (@event == null)
            {
                return NotFound();
            }
           // var ce = new CalendarEvent(@event, User.FindFirstValue(ClaimTypes.NameIdentifier));
           // return View(ce);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(Guid id, IFormCollection form)
        {
            try
            {
                //update the form dal.UpdateEvent(form);
               // TempData["Alert"] = "Success! The event has been updated!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //ViewData["Alert"] = "An error occurred: " + ex.Message;
                //var ce = new CalendarEvent(dal.GetEvent(id), User.FindFirstValue(ClaimTypes.NameIdentifier));
                //return the original event in calendar;
            }
        }

        // GET: Event/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var @event = dal.GetEvent((Guid)id);
            if (@event == null)
            {
                return NotFound();
            }

           // return even in calendar;
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            //to delete an event: dal.DeleteEvent(id);
           // TempData["Alert"] = "You deleted an event.";
            return RedirectToAction(nameof(Index));
        }
    }

    }
