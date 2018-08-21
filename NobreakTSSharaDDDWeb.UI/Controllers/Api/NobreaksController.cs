using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NobreakTSSharaDDDWeb.UI.ViewModels;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NobreakTSSharaDDDWeb.UI.Controllers.Api
{
    public class NobreaksController : ApiController
    {
        private ApplicationDbContext db = new NobreakTSShara.Webapp.Models.ApplicationDbContext();       

        [Authorize]
        [HttpPost]        
        public IHttpActionResult PostAddNobreakToAccount(Nobreak nobreak)
        {
            var user = Authentication.User.Identity;
            var u = db.Users.Find(User.Identity.GetUserId());

            //Busca por nobreaks com o serial passado
            var nb = db.Nobreaks.Where(n => n.Serial.Equals(nobreak.Serial)).FirstOrDefault();

            try
            {
                if (nb == null) //Verifica se é um novo nobreak
                {
                    nobreak.Users.Add(u); //Adiciona o usuario autenticado pela api na lista de usuarios do nb
                    db.Nobreaks.Add(nobreak);//Aciciona o nobreak ao repositorio
                }
                else  //Caso o nobreak ja exista
                { 
                    nb.Users.Add(u); //Adiciona o usuario autenticado pela api na lista do nobreak encontrado
                }

                
                
                db.SaveChanges();
                return Ok("Nobreak adicionado a conta.");

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }            

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

        private IAuthenticationManager Authentication
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }





        /*
        // GET: api/Nobreaks
        [Authorize]
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

            db.Nobreaks.AddOrUpdate(n => n.Serial, nobreak);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = nobreak.ID }, nobreak);
        } 

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
        */
    }
}

