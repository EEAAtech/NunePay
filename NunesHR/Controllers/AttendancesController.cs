using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss, HR")]
    public class AttendancesController : EAController
    {
        public AttendancesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: Attendances: For EmpID
        public ActionResult RollCall(int id)
        {
            int StartMonth = DateTime.Today.AddMonths(-1).Month;
            int StartYear = DateTime.Today.AddMonths(-1).Year;
            ViewBag.Mon1 = MyExtensions.MonthFromInt(StartMonth) + " " + StartYear;

            DateTime StartDate = DateTime.Parse(String.Concat(1, " ", StartMonth, " ", StartYear));
            var existlds = db.Attendance.Where(a => a.EmpID == id && a.LeaveDate>= StartDate).ToList();

            List<LeaveDays> result = new List<LeaveDays>();
            ViewBag.DaysInStartMon = DateTime.DaysInMonth(StartYear, StartMonth);

            //For past month
            for (Byte i = 1; i <= ViewBag.DaysInStartMon; i++)
            {
               LeaveDays ld = new LeaveDays { EmpID = id, Dayt = DateTime.Parse(String.Concat(i, " ", StartMonth, " ", StartYear)) };
                var d = existlds.Where(a => a.LeaveDate == DateTime.Parse(String.Concat(i, " ", StartMonth, " ", StartYear)));

               if (d.Count()>0 )
                    ld.IsLeave = true;
                result.Add(ld);
            }

            //Prepare to enter objects for current month
            if (StartMonth == 12)
            {
                StartYear++;
                StartMonth = 1;
            }
            else
                StartMonth++; 

            ViewBag.Mon2 =  MyExtensions.MonthFromInt(StartMonth) + " " + StartYear;

            ViewBag.DaysInCurrMon = DateTime.DaysInMonth(StartYear, StartMonth);
            for (Byte i = 1; i <= ViewBag.DaysInCurrMon; i++)
            {
                LeaveDays ld = new LeaveDays { EmpID = id, Dayt = DateTime.Parse(String.Concat(i, " ", StartMonth, " ", StartYear)) };
                var d = existlds.Where(a => a.LeaveDate == DateTime.Parse(String.Concat(i, " ", StartMonth, " ", StartYear)));

                if (d.Count() > 0)
                    ld.IsLeave = true;
                result.Add(ld);
            }

            ViewBag.Emp = db.Employees.FirstOrDefault(e => e.EmpID == id);

            return View("Index" ,result);         
        }

      
        // POST: Attendances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection fc)
        {
            int StartMonth = DateTime.Today.AddMonths(-1).Month;
            int StartYear = DateTime.Today.AddMonths(-1).Year;
            DateTime StartDate = DateTime.Parse(String.Concat(1, " ", StartMonth, " ", StartYear));
            int EmpID = int.Parse(fc["EmpId"]);

            var existlds = db.Attendance.Where(a => a.EmpID == EmpID && a.LeaveDate >= StartDate);
            var cleanup = db.Attendance.RemoveRange(existlds);
                                    
            if (ModelState.IsValid)
            {
                for (int i = 2; i < fc.Keys.Count; i++)
                {
                    Attendance a = new Attendance { EmpID = EmpID, LeaveDate = DateTime.Parse(fc.Keys[i].ToString()) };
                    db.Attendance.Add(a);
                    db.SaveChanges();
                }
                
                return RedirectToAction("Index","Employees");
            }

            //ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", attendance.EmpID);
            return View();
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
