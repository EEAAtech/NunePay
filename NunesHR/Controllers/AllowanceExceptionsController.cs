using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NunesHR;

namespace NunesHR.Controllers
{
    public class AllowanceExceptionsController : Controller
    {
        private NTHRPayEntities1 db = new NTHRPayEntities1();

        // GET: AllowanceExceptions
        public ActionResult Index()
        {
            var allowanceExceptions = db.AllowanceExceptions.Include(a => a.AllowanceTypes).Include(a => a.Employees);
            return View(allowanceExceptions.ToList());
        }

        // GET: AllowanceExceptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllowanceExceptions allowanceExceptions = db.AllowanceExceptions.Find(id);
            if (allowanceExceptions == null)
            {
                return HttpNotFound();
            }
            return View(allowanceExceptions);
        }

        // GET: AllowanceExceptions/Create
        public ActionResult Create()
        {
            ViewBag.ATID = new SelectList(db.AllowanceTypes, "ATID", "AllowanceType");
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name");
            return View();
        }

        // POST: AllowanceExceptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AEID,EmpID,WEF,ATID,Stop")] AllowanceExceptions allowanceExceptions)
        {
            if (ModelState.IsValid)
            {
                db.AllowanceExceptions.Add(allowanceExceptions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ATID = new SelectList(db.AllowanceTypes, "ATID", "AllowanceType", allowanceExceptions.ATID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", allowanceExceptions.EmpID);
            return View(allowanceExceptions);
        }

        // GET: AllowanceExceptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllowanceExceptions allowanceExceptions = db.AllowanceExceptions.Find(id);
            if (allowanceExceptions == null)
            {
                return HttpNotFound();
            }
            ViewBag.ATID = new SelectList(db.AllowanceTypes, "ATID", "AllowanceType", allowanceExceptions.ATID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", allowanceExceptions.EmpID);
            return View(allowanceExceptions);
        }

        // POST: AllowanceExceptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AEID,EmpID,WEF,ATID,Stop")] AllowanceExceptions allowanceExceptions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(allowanceExceptions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ATID = new SelectList(db.AllowanceTypes, "ATID", "AllowanceType", allowanceExceptions.ATID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", allowanceExceptions.EmpID);
            return View(allowanceExceptions);
        }

        // GET: AllowanceExceptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllowanceExceptions allowanceExceptions = db.AllowanceExceptions.Find(id);
            if (allowanceExceptions == null)
            {
                return HttpNotFound();
            }
            return View(allowanceExceptions);
        }

        // POST: AllowanceExceptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AllowanceExceptions allowanceExceptions = db.AllowanceExceptions.Find(id);
            db.AllowanceExceptions.Remove(allowanceExceptions);
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
