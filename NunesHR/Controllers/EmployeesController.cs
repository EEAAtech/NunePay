using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss, HR,Anon")]
    public class EmployeesController : EAController
    {
        public EmployeesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: Employees
        public ActionResult Index(int? id, string searchString, int? page, int? EmpTypeID)
        {
            var employees = db.Employees.Include(e => e.EmpTypes);
            ViewBag.EmpTypeID = new SelectList(db.EmpTypes, "EmpTypeID", "EmpType");
                       

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(p => p.Name.Contains(searchString)
                                       || p.NickName.Contains(searchString));

                page = 1;
            }

            if (EmpTypeID != null)
            {
                employees = employees.Where(e => e.EmpTypeID == EmpTypeID);
                ViewBag.EID = EmpTypeID;
            }
            

            employees = employees.OrderBy(e => e.Name);

            int pageSize = db.Config.FirstOrDefault().RowsPerPage;
            int pageNumber = (page ?? 1);
            return View(employees.ToPagedList(pageNumber, pageSize));            
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewData["emp.EmpTypeID"] = new SelectList(db.EmpTypes, "EmpTypeID", "EmpType");
            
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewEmp employees)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employees.emp);
                db.EmploymentHistory.Add(new EmploymentHistory() { JoinDate = employees.JoinDate, EmpID = employees.emp.EmpID });

                //Insert a Drivers Licence expiry date for drivers
                if (employees.emp.EmpTypeID == 1) //EmpTypeID =1 has to be driver  and EDocTypeID = 1 has to be Driving License
                    db.EmpDocs.Add(new EmpDocs() { EmpID = employees.emp.EmpID, EDocTypeID = 1, ExpiryDate = employees.DrivLicExp });

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewData["emp.EmpTypeID"] = new SelectList(db.EmpTypes, "EmpTypeID", "EmpType", employees.emp.EmpTypeID);
            
            return View(employees);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmpTypeID = new SelectList(db.EmpTypes, "EmpTypeID", "EmpType", employees.EmpTypeID);
            return View(employees);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmpID,Name,NickName,EmpTypeID,JobTitle,Mobile,EmContactNo,EmContactName,EmContactReln,CatAB, IsHDFC,RegBankAc,NRegBankAc,NRegBank,NRegIFSC,BonusPayMonth,PFno,ESIno")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmpTypeID = new SelectList(db.EmpTypes, "EmpTypeID", "EmpType", employees.EmpTypeID);
            return View(employees);
        }
        /// <summary>
        /// daily allowance EA format
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public ActionResult DailyAllow(DateTime dt)
        {
            string MyQry = "SELECT e.EmpID,e.Name as EmpName, d.SaveTime, d.AllowDate FROM Employees e INNER JOIN EmploymentHistory eh ON e.EmpID = eh.EmpID INNER JOIN EmpTypes et ON e.EmpTypeID=et.EmpTypeID " +
                  "  LEFT OUTER JOIN DailyAllowance d ON e.EmpID = d.EmpID and d.AllowDate = '" + dt.ToString("yyyy-MM-dd") +
                  "'  WHERE eh.ExitDate IS NULL AND et.HasDailyAllowance=1 ORDER BY e.Name";
            List<DA> todt = db.Database.SqlQuery<DA>(MyQry).ToList();

            ViewBag.dt = dt;

            return View(todt);
        }
        /// <summary>
        /// daily allowance calendar format
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public ActionResult DailyAllowCal(int? Id)
        {
            ViewBag.EmpName = db.Employees.Find(Id).Name;
            ViewBag.EmpID = Id;
            return View();
        }
        
        //Fetch the records for daily allowance to put in the calendar for the chosen emp
        public JsonResult DailyAllowCalData(int EmpID, string start, string end)
        {
            DateTime St = DateTime.Parse(start);
            DateTime Ed = DateTime.Parse(end);
            var da = db.DailyAllowance.Where(d => d.EmpID == EmpID && d.AllowDate>=St && d.AllowDate<=Ed).Select(e => new { title = "paid", start = e.AllowDate, allDay = true });

            List<DailyAllowEvent> dae = new List<DailyAllowEvent>();

            foreach (var item in da)
            {
                dae.Add(new DailyAllowEvent { title = item.title, start = item.start.ToString("s"), allDay = item.allDay });
            }
            
            //Fetch Attendance data
            var att = db.Attendance.Where(a => a.EmpID==EmpID && a.LeaveDate >= St && a.LeaveDate<= Ed).Select(e => new { title = "Absent", start = e.LeaveDate , allDay = true });
            foreach (var item in att)
            {
                dae.Add(new DailyAllowEvent { title = item.title, start = item.start.ToString("s"), allDay = item.allDay });
            }

            return Json(dae.ToArray(), JsonRequestBehavior.AllowGet);
        }

        //This method is used by the Calendar format for saving Daily Allowances
        [HttpPost]        
        public bool SaveDApay(string dt, int emp)
        {
            DateTime dat = DateTime.Parse(dt);

            //toggle the record
            var addel = db.DailyAllowance.Any(d => d.EmpID == emp && d.AllowDate == dat);
            if (addel)
            {
                var del = db.DailyAllowance.FirstOrDefault(d => d.EmpID == emp && d.AllowDate == dat);
                db.DailyAllowance.Remove(del);
            } else {
                db.DailyAllowance.Add(new DailyAllowance { EmpID = emp, AllowDate = dat, SaveTime = DateTime.Now });
            }
            db.SaveChanges();
            return true;
        }

        // This method is used by the EA format for saving Daily Allowances
        public ActionResult SaveDailyAllow(int id, DateTime dt)
        {
            db.DailyAllowance.Add(new DailyAllowance { EmpID = id, AllowDate=dt, SaveTime=DateTime.Now});
            db.SaveChanges();

            return RedirectToAction("DailyAllow", new { dt = dt });
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
