using PagedList;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize (Roles ="Boss, HR")]
    public class AdvancesController : EAController
    {
        public AdvancesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: Advances
        public ActionResult Index(int EmpID, int? page)
        {
            var advance = db.Advance.Where(a => a.EmpID == EmpID).OrderByDescending(a =>a.AdvDate);
            ViewBag.EmpID = EmpID;
            ViewBag.foremp = db.Employees.FirstOrDefault(e => e.EmpID == EmpID).Name.ToString();

            int pageSize = db.Config.FirstOrDefault().RowsPerPage;
            int pageNumber = (page ?? 1);
            return View(advance.ToPagedList(pageNumber, pageSize));
            
        }

        // GET: Advances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advance advance = db.Advance.Find(id);
            if (advance == null)
            {
                return HttpNotFound();
            }
            return View(advance);
        }

        // GET: Advances/Create
        public ActionResult Create(int id)
        {
            ViewBag.EmpID = id;
            return View();
        }

        // POST: Advances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AdvanceID,EmpID,AdvDate,Amount")] Advance advance)
        {
            if (ModelState.IsValid)
            {
                db.Advance.Add(advance);
                db.SaveChanges();
                return RedirectToAction("Index", new { EmpID = advance.EmpID });
            }
                        
            return View(advance);
        }

        // GET: Advances/Edit/5
        [Authorize(Roles = "Boss")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advance advance = db.Advance.Find(id);
            if (advance == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", advance.EmpID);
            return View(advance);
        }

        // POST: Advances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdvanceID,EmpID,AdvDate,Amount")] Advance advance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(advance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { EmpID = advance.EmpID });
            }
            
            return View(advance);
        }

        // GET: Advances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advance advance = db.Advance.Find(id);
            if (advance == null)
            {
                return HttpNotFound();
            }
            return View(advance);
        }

        // POST: Advances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Advance advance = db.Advance.Find(id);
            db.Advance.Remove(advance);
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
