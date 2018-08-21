using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NobreakTSSharaDDDWeb.UI.Models;

namespace NobreakTSSharaDDDWeb.UI.Controllers
{
    [Authorize]
    public class NobreaksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Nobreaks
        public ActionResult Index()
        {
            //var user = db.Users.Find(User.Identity.GetUserId());
            //return View(user.Nobreaks.ToList());
            return View();
        }

        public ActionResult List()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            return PartialView("_List", user.Nobreaks.ToList());
        }

        // GET: Nobreaks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nobreak nobreak = db.Nobreaks.Find(id);
            if (nobreak == null)
            {
                return HttpNotFound();
            }
            return View(nobreak);
        }

        // GET: Nobreaks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Nobreaks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Label,Serial,CompanyName,UPSModel,Version")] Nobreak nobreak)
        {
            if (ModelState.IsValid)
            {
                db.Nobreaks.Add(nobreak);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nobreak);
        }

        public ActionResult AddToAccount()
        {
            return PartialView("_AddToAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToAccount(string serialNobreak)
        {
            var nobreak = db.Nobreaks.Where(nb => nb.Serial.Equals(serialNobreak)).FirstOrDefault();
            if (nobreak != null)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                nobreak.Users.Add(user);
                db.SaveChanges();
                string url = Url.Action("List", "Nobreaks");
                return Json(new { success = true, url = url });

                //return RedirectToAction("Index");
            }

            ModelState.AddModelError("Serial", $"Serial {serialNobreak} não encontrado");
            
            //return View(nobreak);
            return PartialView("_AddToAccount");


        }

        // GET: Nobreaks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nobreak nobreak = db.Nobreaks.Find(id);
            if (nobreak == null)
            {
                return HttpNotFound();
            }
            return View(nobreak);
        }

        // POST: Nobreaks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Label,Serial,CompanyName,UPSModel,Version")] Nobreak nobreak)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nobreak).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nobreak);
        }

        // GET: Nobreaks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nobreak nobreak = db.Nobreaks.Find(id);
            if (nobreak == null)
            {
                return HttpNotFound();
            }
            return View(nobreak);
        }

        public ActionResult Monitoring()
        {
            return View();
        }

        // POST: Nobreaks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nobreak nobreak = db.Nobreaks.Find(id);
            //db.Nobreaks.Remove(nobreak);
            var user = db.Users.Find(User.Identity.GetUserId());
            user.Nobreaks.Remove(nobreak);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
