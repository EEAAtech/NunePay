using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss,HR")]
    public class BonusesController : EAController
    {
        public BonusesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: Bonuses
        public ActionResult Index(int? id, int? page)
        {
            
            //var bonus = db.Bonus.Where(b=>b.Year==DateTime.Now.Year).Include(b => b.Employees).OrderBy(b => b.Employees.Name);
                      
            var bonus = db.Get5yrBonus(DateTime.Today.Year).ToList();

            int pageSize = db.Config.FirstOrDefault().RowsPerPage;
            int pageNumber = (page ?? 1);
            return View(bonus.ToPagedList(pageNumber, pageSize));
            
        }

        public ActionResult OldBonus(int? yr, int? id, int? page)
        {
            if (yr == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Dont get ahead of yourself
            yr = yr == DateTime.Now.Year ? DateTime.Now.Year-1 : yr;
            
            var bonus = db.Bonus.Where(b => b.Year == yr).Include(b => b.Employees).OrderBy(b => b.Employees.Name);
            ViewBag.yr = yr;
            int pageSize = db.Config.FirstOrDefault().RowsPerPage;
            int pageNumber = (page ?? 1);
            return View(bonus.ToPagedList(pageNumber, pageSize));
        }

        // GET: Bonuses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bonus bonus = db.Bonus.Find(id);
            if (bonus == null)
            {
                return HttpNotFound();
            }
            return View(bonus);
        }

        // GET: Bonuses/Create
        public ActionResult Create()
        {
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name");
            return View();
        }

        // POST: Bonuses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmpID,Year,SysBonus,UsrBonus")] Bonus bonus)
        {
            if (ModelState.IsValid)
            {
                db.Bonus.Add(bonus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", bonus.EmpID);
            return View(bonus);
        }

        // GET: Bonuses/Edit/5
        public ActionResult Edit(int? id, int yr)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bonus bonus = db.Bonus.FirstOrDefault(b => b.EmpID == id && b.Year == yr);
            if (bonus == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", bonus.EmpID);
            return View(bonus);
        }

        // POST: Bonuses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmpID,Year,SysBonus,UsrBonus")] Bonus bonus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bonus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", bonus.EmpID);
            return View(bonus);
        }

        // GET: Bonuses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bonus bonus = db.Bonus.Find(id);
            if (bonus == null)
            {
                return HttpNotFound();
            }
            return View(bonus);
        }

        // POST: Bonuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bonus bonus = db.Bonus.Find(id);
            db.Bonus.Remove(bonus);
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
