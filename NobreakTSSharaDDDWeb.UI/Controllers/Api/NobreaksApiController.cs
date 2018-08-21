using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NobreakTSSharaDDDWeb.UI.Models;
using Nobreak = NobreakTSSharaDDDWeb.Domain.Entities.Nobreak;
using NobreakEvent = NobreakTSSharaDDDWeb.Domain.Entities.NobreakEvent;

namespace NobreakTSSharaDDDWeb.UI.Controllers.Api
{
    public class NobreaksController : ApiController
    {
        private ApplicationDbContext db = new NobreakTSSharaDDDWeb.UI.Models.ApplicationDbContext();

        // GET: api/Nobreaks
        public IQueryable<Nobreak> GetNobreaks()
        {
            return db.Nobreaks;
        }

        [ResponseType(typeof(IList<NobreakEvent>))]
        public IHttpActionResult GetEvents(string serial)
        {
            if (!NobreakExists(serial))
            {
                return NotFound();
            }

            return Ok(db.NobreakEvents.Where(e => e.Nobreak.Serial.Equals(serial)).ToList());
        }



        // GET: api/Nobreaks/5
        [ResponseType(typeof(Nobreak))]
        public IHttpActionResult GetNobreak(int id)
        {
            Nobreak nobreak = db.Nobreaks.Find(id);
            if (nobreak == null)
            {
                return NotFound();
            }

            return Ok(nobreak);
        }

        // PUT: api/Nobreaks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNobreak(int id, Nobreak nobreak)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != nobreak.ID)
            {
                return BadRequest();
            }

            db.Entry(nobreak).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NobreakExists(id))
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

        // POST: api/Nobreaks
        [ResponseType(typeof(Nobreak))]
        public IHttpActionResult PostNobreak(Nobreak nobreak)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Nobreaks.Add(nobreak);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = nobreak.ID }, nobreak);
        }

        //public IHttpActionResult Post(JObject objData)
        //{

        //    var nobreak = objData.ToObject<Nobreak>();
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Nobreaks.AddOrUpdate(nb => nb.Serial, nobreak);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = nobreak.ID }, nobreak);

        //}

        // DELETE: api/Nobreaks/5
        [ResponseType(typeof(Nobreak))]
        public IHttpActionResult DeleteNobreak(int id)
        {
            Nobreak nobreak = db.Nobreaks.Find(id);
            if (nobreak == null)
            {
                return NotFound();
            }

            db.Nobreaks.Remove(nobreak);
            db.SaveChanges();

            return Ok(nobreak);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NobreakExists(int id)
        {
            return db.Nobreaks.Count(e => e.ID == id) > 0;
        }

        private bool NobreakExists(string serial)
        {
            return db.Nobreaks.Count(e => e.Serial.Equals(serial)) > 0;
        }
    }
}

