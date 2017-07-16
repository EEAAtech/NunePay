using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss,HR")]
    public class WagesController : EAController
    {
        public WagesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: Wages
        public ActionResult Index(int? EmpID)
        {
            
            if (EmpID != null)
            {
                var wages = db.Wages.Where(e => e.EmpID == EmpID).OrderByDescending(w => w.WEF);
                ViewBag.EmpID = EmpID;
                return View(wages.ToList());
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Wages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wages wages = db.Wages.Find(id);
            if (wages == null)
            {
                return HttpNotFound();
            }
            return View(wages);
        }

        // GET: Wages/Create
        public ActionResult Create(int id)
        {
            ViewBag.EmpID = id;
            return View();
        }

        // POST: Wages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WageID,EmpID,WEF,Amount")] Wages wages)
        {
            if (ModelState.IsValid)
            {
                db.Wages.Add(wages);
                db.SaveChanges();
                return RedirectToAction("Index", new { EmpID = wages.EmpID } );
            }

            
            return View(wages);
        }

        // GET: Wages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wages wages = db.Wages.Find(id);
            if (wages == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", wages.EmpID);
            return View(wages);
        }

        // POST: Wages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WageID,EmpID,WEF,Amount")] Wages wages)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wages).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { EmpID = wages.EmpID });
            }
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", wages.EmpID);
            return View(wages);
        }

        // GET: Wages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wages wages = db.Wages.Find(id);
            if (wages == null)
            {
                return HttpNotFound();
            }
            return View(wages);
        }

        // POST: Wages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Wages wages = db.Wages.Find(id);
            db.Wages.Remove(wages);
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
