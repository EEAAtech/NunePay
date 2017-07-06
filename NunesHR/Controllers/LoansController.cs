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
    public class LoansController : EAController
    {
        public LoansController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: Loans
        public ActionResult Index(int? id, string searchString, int? page, int? EmpID)
        {
            var loans = db.Loans.Include(l => l.Employees);
            if (!String.IsNullOrEmpty(searchString))
            {
                loans = loans.Where(p => p.Employees.Name.Contains(searchString)
                                       || p.Employees.NickName.Contains(searchString));

                page = 1;
            }

            TempData["SpecificEmp"] = null;
            if (EmpID != null)
            {
                loans = loans.Where(e => e.EmpID == EmpID);
                ViewBag.EmpID = EmpID;
                TempData["SpecificEmp"] = EmpID;//used to guide the flow back to specific OR all emps in Index
            }
            loans = loans.OrderBy(e => e.Employees.Name).ThenBy(e=>e.EmpID);



            int pageSize = db.Config.FirstOrDefault().RowsPerPage;
            int pageNumber = (page ?? 1);
            return View(loans.ToPagedList(pageNumber, pageSize));
            
        }

        // GET: Loans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loans loans = db.Loans.Find(id);
            if (loans == null)
            {
                return HttpNotFound();
            }

            return View(loans);
        }

        // GET: Loans/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                Session.Add("emp", 0);
                Session.Add("caller", "../Loans/Create");
            }
            else
            {
                Session.Remove("emp");
                Session.Remove("caller");
                Loans ed = new Loans { EmpID = (int)id, LoanDate=DateTime.Today.Date };
                
                return View(ed);
            }

            return RedirectToAction("../Employees");
            
        }

        // POST: Loans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LoanID,EmpID,LoanDate,Amount,PayMonths,Default")] Loans loans)
        {
            if (ModelState.IsValid)
            {
                db.Loans.Add(loans);
                db.SaveChanges();
                if (TempData["SpecificEmp"]!=null)
                    return RedirectToAction("Index", new { EmpID = loans.EmpID });
                else
                    return RedirectToAction("Index");
            }

            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", loans.EmpID);
            return View(loans);
        }

        // GET: Loans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loans loans = db.Loans.Find(id);
            if (loans == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", loans.EmpID);
            return View(loans);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoanID,EmpID,LoanDate,Amount,PayMonths,Default, DefaultReason")] Loans loans)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loans).State = EntityState.Modified;
                db.SaveChanges();
                if (TempData["SpecificEmp"] != null)
                    return RedirectToAction("Index", new { EmpID = loans.EmpID });
                else
                    return RedirectToAction("Index");
            }
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", loans.EmpID);
            return View(loans);
        }

        // GET: Loans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loans loans = db.Loans.Find(id);
            if (loans == null)
            {
                return HttpNotFound();
            }
            return View(loans);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Loans loans = db.Loans.Find(id);
            db.Loans.Remove(loans);
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
