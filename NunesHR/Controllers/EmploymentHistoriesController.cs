using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss, HR,Anon")]
    public class EmploymentHistoriesController : EAController
    {
        public EmploymentHistoriesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: EmploymentHistories
        public ActionResult Index(string searchString, int? EmpID)
        {
            var employmentHistory = db.EmploymentHistory.Include(e => e.Employees);

            if (!String.IsNullOrEmpty(searchString))
            {
                employmentHistory = employmentHistory.Where(p => p.Employees.Name.Contains(searchString)
                                       || p.Employees.NickName.Contains(searchString));

            }
            ViewBag.NoCreate = true;
            if (EmpID != null)
            {
                employmentHistory = employmentHistory.Where(e => e.EmpID == EmpID);
                ViewBag.EmpID = EmpID;

                //Check if he is a current employee
                var iscurr = employmentHistory.Any(h => h.ExitDate == null);

                if (iscurr == false)
                    ViewBag.NoCreate = false;
            }

            employmentHistory = employmentHistory.OrderBy(e => e.Employees.Name).ThenBy(e => e.EmpID).ThenByDescending(e => e.JoinDate);

            return View(employmentHistory.ToList());
        }

        // GET: EmploymentHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmploymentHistory employmentHistory = db.EmploymentHistory.Find(id);
            if (employmentHistory == null)
            {
                return HttpNotFound();
            }
            return View(employmentHistory);
        }

        // GET: EmploymentHistories/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                Session.Add("emp", 0);
                Session.Add("caller", "../EmploymentHistories/Create");
            }
            else
            {
                Session.Remove("emp");
                Session.Remove("caller");
                EmploymentHistory ed = new EmploymentHistory { EmpID = (int)id };
                
                return View(ed);
            }

            return RedirectToAction("../Employees");
        }

        // POST: EmploymentHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EHID,EmpID,JoinDate,RegistrationDate,ExitDate,ExitReason")] EmploymentHistory employmentHistory)
        {
            if (ModelState.IsValid)
            {
                db.EmploymentHistory.Add(employmentHistory);
                db.SaveChanges();
                return RedirectToAction("Index", new { EmpID = employmentHistory.EmpID });
            }
                        
            return View(employmentHistory);
        }

        // GET: EmploymentHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmploymentHistory employmentHistory = db.EmploymentHistory.Find(id);
            if (employmentHistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", employmentHistory.EmpID);
            return View(employmentHistory);
        }

        // POST: EmploymentHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EHID,EmpID,JoinDate,RegistrationDate,ExitDate,ExitReason")] EmploymentHistory employmentHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employmentHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { EmpID = employmentHistory.EmpID });
            }
            //ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", employmentHistory.EmpID);
            return View(employmentHistory);
        }

        // GET: EmploymentHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmploymentHistory employmentHistory = db.EmploymentHistory.Find(id);
            if (employmentHistory == null)
            {
                return HttpNotFound();
            }
            return View(employmentHistory);
        }

        // POST: EmploymentHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmploymentHistory employmentHistory = db.EmploymentHistory.Find(id);
            db.EmploymentHistory.Remove(employmentHistory);
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
