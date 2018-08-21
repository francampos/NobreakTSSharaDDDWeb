using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NobreakTSSharaDDDWeb.UI.Models;

namespace NobreakTSSharaDDDWeb.UI.Controllers
{
    [Authorize]
    public class NobreakEventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NobreakEvents
        public ActionResult Index(int? page)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var nobreakEvents = user.Nobreaks.SelectMany(n => n.NobreakEvents).OrderByDescending(n => n.CreationData).ToList();


            //var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            //var onePageOfProducts = nobreakEvents.ToPagedList(pageNumber, 15); // will only contain 25 products max because of the pageSize

            //ViewBag.OnePageOfProducts = onePageOfProducts;
            //return View(db.NobreakEvents.ToList());
            return View(nobreakEvents.ToList());
        }

        // GET: NobreakEvents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NobreakEvent nobreakEvent = db.NobreakEvents.Find(id);
            if (nobreakEvent == null)
            {
                return HttpNotFound();
            }
            return View(nobreakEvent);
        }

        // GET: NobreakEvents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NobreakEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,EventReasons,InputVoltage,OutputVoltage,Load,Battery,Frequency,Temperature,CreationData")] NobreakEvent nobreakEvent)
        {
            if (ModelState.IsValid)
            {
                db.NobreakEvents.Add(nobreakEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nobreakEvent);
        }

        // GET: NobreakEvents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NobreakEvent nobreakEvent = db.NobreakEvents.Find(id);
            if (nobreakEvent == null)
            {
                return HttpNotFound();
            }
            return View(nobreakEvent);
        }

        // POST: NobreakEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EventReasons,InputVoltage,OutputVoltage,Load,Battery,Frequency,Temperature,CreationData")] NobreakEvent nobreakEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nobreakEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nobreakEvent);
        }

        // GET: NobreakEvents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NobreakEvent nobreakEvent = db.NobreakEvents.Find(id);
            if (nobreakEvent == null)
            {
                return HttpNotFound();
            }
            return View(nobreakEvent);
        }

        // POST: NobreakEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NobreakEvent nobreakEvent = db.NobreakEvents.Find(id);
            db.NobreakEvents.Remove(nobreakEvent);
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
