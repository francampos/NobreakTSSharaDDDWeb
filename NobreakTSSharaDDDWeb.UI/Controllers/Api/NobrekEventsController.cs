using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NobreakTSSharaDDDWeb.UI.Models;
using NobreakTSSharaDDDWeb.Domain.Enuns;


namespace NobreakTSSharaDDDWeb.UI.Controllers.Api
{
    //[Authorize]
    public class NobreakEventsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/NobreakEvents
        //[Authorize]
        public IList<NobreakEvent> GetNobreakEvents(string serial)
        {
            return db.NobreakEvents.Where(e => e.Nobreak.Serial.Equals(serial)).ToList();
        }

        // GET: api/NobreakEvents/5
        [ResponseType(typeof(NobreakEvent))]
        public IHttpActionResult GetNobreakEvent(int id)
        {
            NobreakEvent nobreakEvent = db.NobreakEvents.Find(id);
            if (nobreakEvent == null)
            {
                return NotFound();
            }

            return Ok(nobreakEvent);
        }

        // PUT: api/NobreakEvents/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNobreakEvent(int id, NobreakEvent nobreakEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != nobreakEvent.ID)
            {
                return BadRequest();
            }

            db.Entry(nobreakEvent).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NobreakEventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/NobreakEvents
        [ResponseType(typeof(NobreakEvent))]
        public IHttpActionResult PostNobreakEvent(NobreakEvent nobreakEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NobreakEvents.Add(nobreakEvent);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = nobreakEvent.ID }, nobreakEvent);
        }

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult PostNobreakEvent(NobreakEvent nobreakEvent, string serial)
        {
            try
            {
                var nobreak = db.Nobreaks.Where(nb => nb.Serial.Equals(serial)).FirstOrDefault();
                nobreakEvent.Nobreak = nobreak;
                db.NobreakEvents.Add(nobreakEvent);
                db.SaveChanges();
                return CreatedAtRoute("DefaultApi", new { id = nobreakEvent.ID }, nobreakEvent);
            }
            catch
            {
                return BadRequest("Nao foi possivel salvar o evento");
            }

        }

        // DELETE: api/NobreakEvents/5
        [ResponseType(typeof(NobreakEvent))]
        public IHttpActionResult DeleteNobreakEvent(int id)
        {
            NobreakEvent nobreakEvent = db.NobreakEvents.Find(id);
            if (nobreakEvent == null)
            {
                return NotFound();
            }

            db.NobreakEvents.Remove(nobreakEvent);
            db.SaveChanges();

            return Ok(nobreakEvent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NobreakEventExists(int id)
        {
            return db.NobreakEvents.Count(e => e.ID == id) > 0;
        }
    }
}