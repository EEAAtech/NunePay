using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss, HR")]
    public class PayrollsControllerCopy : EAController
    {
        public PayrollsControllerCopy()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: Payrolls/Pay register
        [Authorize(Roles = "Boss, HR,Anon")]
        public ActionResult PayRegRq()
        {
            MakeMonYrRq();
            ViewBag.Title = "Pay Register";
            ViewBag.action = "PayRegRq";
            
            return View("PayRegRq");            
        }


        // POST: redirects to Index to show Pay Register
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Boss, HR,Anon")]
        public ActionResult PayRegRq(FormCollection fm)
        {
            if (ModelState.IsValid)
            {

                return RedirectToAction("Index", new { mon = fm["PayMonth"], yr = fm["PayYear"], Cat = fm["CatAB"] });
            }


            return View();
        }

        // GET: Pay Register
        [Authorize(Roles = "Boss, HR,Anon")]
        public ActionResult Index(int? mon, int? yr, int? page, string Cat)
        {
            if (mon == null || yr == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var payroll = db.Payroll.Include(p => p.Employees).Where(p => p.GenMonth == mon && p.GenYear == yr);

            if (Cat != null)
                payroll = payroll.Where(p => p.Employees.CatAB == Cat);

            payroll = payroll.OrderBy(p => p.Employees.EmpTypeID).ThenBy(p => p.Employees.Name);

            ViewBag.MonYr = MyExtensions.MonthFromInt((int)mon) + " " + yr.ToString();
            ViewBag.mon = mon;
            ViewBag.yr = yr;

            page = page ?? 1;
            int pageSize = db.Config.FirstOrDefault().RowsPerPage;
            int pageNumber = (page ?? 1);
            return View(payroll.ToPagedList(pageNumber, pageSize));

        }

        [HttpPost]
        public ActionResult ExportTypeBPayRegister(int? PayMonth, int? PayYear)
        {
            if (PayMonth == null || PayYear == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string monyr = MyExtensions.MonthFromInt((int)PayMonth) + ' ' + PayYear + " salary";
            string ExportDate = DateTime.Today.ToString("yyyyMMdd");

            var payroll = db.Payroll.Include(p => p.Employees).Where(p => p.GenMonth == PayMonth && p.Employees.CatAB == "B" && p.GenYear == PayYear).OrderBy(p => p.Employees.EmpTypeID).ThenBy(p => p.Employees.Name).Select(p => new { p.Employees.Name, p.EmpID, p.Wages,p.DaysWorked,p.Allowance,p.Advance,p.LoanAmt,p.LoanCmt,p.Total,p.AdjAmt,p.AdjRemark });
            ViewBag.MonYr = MyExtensions.MonthFromInt((int)PayMonth) + " " + PayYear.ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=TypeB-Pay-Register " + monyr + ".xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(payroll, Response.Output);
            Response.End();


            return RedirectToAction("Index", new { mon = PayMonth, yr = PayYear });

        }

        // Only export Type B employees that have had adjustments to thier salary
        [HttpPost]
        public ActionResult ExportTypeBPayRegisterAdj(int? PayMonth, int? PayYear)
        {
            if (PayMonth == null || PayYear == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string monyr = MyExtensions.MonthFromInt((int)PayMonth) + ' ' + PayYear + " salary";
            string ExportDate = DateTime.Today.ToString("yyyyMMdd");

            var payroll = db.Payroll.Include(p => p.Employees).Where(p => p.GenMonth == PayMonth && p.GenYear == PayYear && p.Employees.CatAB=="B" && p.AdjAmt>0)
                .OrderBy(p => p.Employees.EmpTypeID).ThenBy(p => p.Employees.Name).Select(p => new { p.Employees.Name, p.EmpID, p.Wages, p.DaysWorked, p.Allowance, p.Advance, p.LoanAmt, p.LoanCmt, p.Total, p.AdjAmt, p.AdjRemark });
            ViewBag.MonYr = MyExtensions.MonthFromInt((int)PayMonth) + " " + PayYear.ToString();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=TypeB-PayRegister-Adjustments " + monyr + ".xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(payroll, Response.Output);
            Response.End();


            return RedirectToAction("Index", new { mon = PayMonth, yr = PayYear });

        }

        // GET: Payrolls/Type B PayRegister export before adjustments
        public ActionResult ExportTypeBPayRegisterRq()
        {
            MakeMonYrRq();
            ViewBag.Title = "Type B PayRegister export before adjustments";
            ViewBag.action = "ExportTypeBPayRegister";

            return View("MonYrRq");
        }
        // GET: Payrolls/Type B PayRegister export only adjustments
        public ActionResult ExportTypeBPayRegisterAdjRq()
        {
            MakeMonYrRq();
            ViewBag.Title = "Type B PayRegister export only adjustments";
            ViewBag.action = "ExportTypeBPayRegisterAdj";

            return View("MonYrRq");
        }

        // GET: Payrolls/Pay Slips
        [Authorize(Roles = "Boss, HR,Anon")]
        public ActionResult PaySlipsMRq()
        {
            MakeMonYrRq();
            ViewBag.Title = "Multiple Payslips";
            ViewBag.action = "PaySlips";

            return View("MonYrRq");
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Boss, HR,Anon")]
        public ActionResult PaySlips(FormCollection fm)
        {
            int mon = int.Parse(fm["PayMonth"]);
            int yr = int.Parse(fm["PayYear"]);

            var payroll = db.Payroll.Include(p => p.Employees).Where(p => p.GenMonth == mon && p.GenYear == yr).OrderBy(p => p.Employees.EmpTypeID).ThenBy(p => p.Employees.Name);
            ViewBag.MonYr = MyExtensions.MonthFromInt(mon) + " " + yr.ToString();
            ViewBag.Allw = db.PayrollAllowance.Where(a => a.GenMonth == mon && a.GenYear == yr);
            return View("PaySlipMany", payroll.ToList());

        }



        // GET: Payrolls/Single Payslip
        [Authorize(Roles = "Boss, HR,Anon")]
        public ActionResult PaySlipsSRq(int EmpID)
        {
            MakeMonYrRq();
            ViewBag.EmpID = EmpID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Boss, HR,Anon")]
        public ActionResult PaySlip(FormCollection fm)
        {
            int mon = int.Parse(fm["PayMonth"]);
            int yr = int.Parse(fm["PayYear"]);
            int EmpID = int.Parse(fm["EmpID"]);
            NunesHR.Payroll payroll = (NunesHR.Payroll) db.Payroll.Include(p => p.Employees).FirstOrDefault(p => p.GenMonth == mon && p.GenYear == yr && p.EmpID == EmpID);
            ViewBag.MonYr = MyExtensions.MonthFromInt(mon) + " " + yr.ToString();
            ViewBag.Allw = db.PayrollAllowance.Where(a => a.EmpID == EmpID && a.GenMonth == mon && a.GenYear == yr);

            return View(payroll);

        }

        // GET: Payrolls/Run Payroll
        public ActionResult RunPayroll()
        {
            MakeMonYrRq();
            ViewBag.Title = "Generate Payroll";
            ViewBag.action = "GenPay";

            return View("MonYrRq");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenPay(FormCollection fm)
        {
            int mon = int.Parse(fm["PayMonth"]);
            int yr = int.Parse(fm["PayYear"]);

            var genpay = db.GenPayroll(mon, yr);
            return RedirectToAction("Index", new { mon = mon, yr = yr });

        }

        // GET: Payrolls/Freeze Payroll
        public ActionResult FreezePayroll()
        {
            MakeMonYrRq();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IcePay(FormCollection fm)
        {
            int mon = int.Parse(fm["PayMonth"]);
            int yr = int.Parse(fm["PayYear"]);

            var genpay = db.FreezePayroll(mon, yr);
            return RedirectToAction("Index", new { mon = mon, yr = yr });

        }

        public ActionResult LoanHistory(int id)
        {
            EmpLoanHistory lhdata = new EmpLoanHistory();
            lhdata.Ln = db.Loans.Include(l => l.Employees).Include(l => l.LoanPay).Where(e => e.EmpID == id).Where(l => l.Default == true || l.PaidOff == true).ToList();
            lhdata.Lhist = db.GetLoanHistory(id).ToList();
            
            ViewBag.EmpID = id;
                        
            return View(lhdata);
        }

        [ChildActionOnly]
        public ActionResult LoanPay(int LoanID)
        {
            return PartialView("_LoanPay",db.LoanPay.Where(l => l.LoanID == LoanID && l.Frozen==true).ToList());
        }

        [Authorize(Roles = "Boss, HR,Anon")]
        public ActionResult LoanOutstanding()
        {   
            IEnumerable<GetLoanHistory_Result> lhdata = db.GetLoanHistory(0);
            lhdata = lhdata.OrderByDescending(l => l.Amount).ToList();
            return View(lhdata);
        }

        public ActionResult EmployeeCard(int id, bool Print = false)
        {
            EmployeeCard ec = new EmployeeCard();

            ec.emp = db.Employees.Find(id);
            ec.empdocs = db.EmpDocs.Where(e => e.EmpID == id).OrderByDescending(e => e.ExpiryDate).ToList();
            ec.emphist = db.EmploymentHistory.Where(e => e.EmpID == id).OrderByDescending(e => e.JoinDate).ToList();
            ec.wages = db.Wages.Where(e => e.EmpID == id).ToList();
            
            EmpLoanHistory lhdata = new EmpLoanHistory();
            lhdata.Ln = db.Loans.Include(l => l.Employees).Include(l => l.LoanPay).Where(e => e.EmpID == id).Where(l => l.Default == true || l.PaidOff == true).ToList();
            lhdata.Lhist = db.GetLoanHistory(id).ToList();
            ec.elh = lhdata;

            ViewBag.EmpID = id;
            ViewBag.Print = Print ? null: "MastDet";

            return View(ec);
        }

        /// <summary>
        /// Attendance register with partial view
        /// </summary>
        /// <returns></returns>
        public ActionResult AttendanceRegister()
        {
            MakeMonYrRq();
            ViewBag.Title = "Attendance Register";
            ViewBag.action = "AttendanceRegister";
            ViewBag.EmpTypeId = new SelectList(db.EmpTypes, "EmpTypeID", "EmpType");
            return View("AttRegRq");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AttendanceRegister(FormCollection fm)
        {
            int mon = int.Parse(fm["PayMonth"]);
            int yr = int.Parse(fm["PayYear"]);
            int EmpTypeID = int.Parse(fm["EmpTypeID"]);

            string MyQry = "SELECT e.EmpID FROM Employees  e, EmploymentHistory eh " +
                            " WHERE e.EmpID = eh.EmpID  AND e.EmpTypeID = " + EmpTypeID +
                            " AND eh.ExitDate IS NULL ORDER BY e.Name";
            var Emps = db.Database.SqlQuery<int>(MyQry).ToList();

            ViewBag.mon = mon;
            ViewBag.yr = yr;
            ViewBag.EmpType = db.EmpTypes.Find(EmpTypeID).EmpType;
            return View(Emps.ToList());
        }

        [ChildActionOnly]
        public ActionResult AttendanceReg(int EmpID,int mon, int yr)
        {
            ViewBag.mon = mon;
            ViewBag.yr = yr;
            ViewBag.emp = db.Employees.Find(EmpID).Name;
            return PartialView("_AttendanceReg", db.Attendance.Where(a => a.EmpID==EmpID && a.LeaveDate.Month ==mon && a.LeaveDate.Year ==yr).ToList());
        }

        /// <summary>
        /// Excel Export functions
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportRegEmps()
        {
            MakeMonYrRq();
            ViewBag.Title = "Type A > HDFC > Export";
            ViewBag.action = "ExportRegEmps";

            return View("MonYrRq");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportRegEmps(FormCollection fm)
        {
            string mon = fm["PayMonth"];
            string yr = fm["PayYear"];
            string monyr = ' ' + mon + yr.Substring(2);
            
            string MyQry = "SELECT e.RegBankAc as AccountNo,'c' as c,p.Total as Amount, CONCAT(SUBSTRING(e.Name, 0, 35), " + monyr + ") as Narration " +
                            " FROM Employees  e, EmploymentHistory eh, Payroll p " +
                             " WHERE e.EmpID = eh.EmpID AND e.EmpID = p.EmpID AND e.CatAB = 'A' AND e.IsHDFC = 1 " +
                             " AND p.GenMonth = " + mon + " AND p.GenYear =  " + yr +
                             " AND p.Frozen = 1 AND eh.ExitDate IS NULL AND p.ExcludeExcel=0 ORDER BY e.Name ";
            IEnumerable<RegExport> todt = db.Database.SqlQuery<RegExport>(MyQry).ToList();
            

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=HDFCEmps"+ monyr +".xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(todt, Response.Output);
            Response.End();

            return RedirectToAction("Index", "Home");

        }

        public ActionResult ExportNonRegEmps()
        {
            MakeMonYrRq();
            ViewBag.Title = "Type A > Non HDFC > Export ";
            ViewBag.action = "ExportNonRegEmps";

            return View("MonYrRq");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportNonRegEmps(FormCollection fm)
        {
            int mon = int.Parse(fm["PayMonth"]);
            string yr = fm["PayYear"];
            string monyr =  MyExtensions.MonthFromInt(mon) + ' ' + yr.Substring(2) + " salary";
            string ExportDate = DateTime.Today.ToString("yyyyMMdd");

            string MyQry = "SELECT '"+ monyr +"' as ForMonth, e.Name, e.NRegBankAc as AccountNo, e.NRegIFSC as IFSC,  p.Total as Amount, '" + ExportDate + "' as ExportDate " +
                            " FROM Employees  e, EmploymentHistory eh, Payroll p " +
                             " WHERE e.EmpID = eh.EmpID AND e.EmpID = p.EmpID AND e.IsHDFC = 0 AND e.CatAB='A' " +
                             " AND p.GenMonth = " + mon + " AND p.GenYear =  " + yr +
                             " AND p.Frozen = 1 AND eh.ExitDate IS NULL AND p.ExcludeExcel=0 ORDER BY e.Name ";
            IEnumerable<NonRegExport> todt = db.Database.SqlQuery<NonRegExport>(MyQry).ToList();


            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=NonHDFCEmps " + monyr + ".xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(todt, Response.Output);
            Response.End();

            return RedirectToAction("Index", "Home");

        }


        /// <summary>
        /// Used for export to Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="output"></param>
        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }

        /// <summary>
        /// Payroll adjustments. Happens only after freezing a payroll
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="yr"></param>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public ActionResult Adjustment(int? mon, int? yr, string SearchString)
        {
            if (mon == null || yr == null)
            { //default to last month 
                mon = DateTime.Today.Month - 1;
                yr = DateTime.Today.AddMonths(-1).Year;
            }

            ViewBag.mon = mon; ViewBag.yr = yr;
            MakeMonYrRq();

            List<AdjustPay> payr = db.Payroll.Where(p => p.GenMonth == mon && p.GenYear == yr && p.Frozen == true).Include(p => p.Employees).Where(p => p.Employees.Name.Contains(SearchString)
                                       || p.Employees.NickName.Contains(SearchString)).Select(p => new AdjustPay { EmpID = p.EmpID,EmpName = p.Employees.Name, GenMonth = p.GenMonth, GenYear = p.GenYear, AdjAmt = p.AdjAmt ?? 0, AdjRemark = p.AdjRemark }).ToList();


            //var payr = db.Payroll.Where(p => p.GenMonth == mon && p.GenYear == yr).Include(p => p.Employees).Where(p => p.Employees.Name.Contains(SearchString)
            //                           || p.Employees.NickName.Contains(SearchString)).ToList();

            ViewBag.Adjs = db.Payroll.Where(p => p.GenMonth == mon && p.GenYear == yr && p.AdjAmt > 0).Include(p => p.Employees).ToList();

            return View(payr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAdjustments(List<AdjustPay> sa)
        {
            Payroll tpr = new Payroll() { GenMonth = DateTime.Today.Month - 1, GenYear = DateTime.Today.AddMonths(-1).Year };
            if (ModelState.IsValid)
            {
                foreach (AdjustPay pr in sa)
                {
                    tpr = db.Payroll.FirstOrDefault(p => p.GenMonth == pr.GenMonth && p.GenYear == pr.GenYear && p.EmpID == pr.EmpID);
                    tpr.AdjAmt = pr.AdjAmt;
                    tpr.AdjRemark = pr.AdjRemark;
                    db.Entry(tpr).State = EntityState.Modified;
                    db.SaveChanges();
                }                
            }
            return RedirectToAction("Adjustment",new { mon = tpr.GenMonth, yr=tpr.GenYear});
        }


        // GET: Payrolls/Type A export adjustments HDFC
        public ActionResult ExportAdjustmentsHDFCRq()
        {
            MakeMonYrRq();
            ViewBag.Title = "Type A export adjustments > HDFC";
            ViewBag.action = "ExportAdjustmentsHDFC";

            return View("MonYrRq");
        }
        // GET: Payrolls/Type A export adjustments Non HDFC
        public ActionResult ExportAdjustmentsNonHDFCRq()
        {
            MakeMonYrRq();
            ViewBag.Title = "Type A export adjustments Non HDFC";
            ViewBag.action = "ExportAdjustmentsNonHDFC";

            return View("MonYrRq");
        }

        [HttpPost]
        public ActionResult ExportAdjustmentsHDFC(int? PayMonth, int? PayYear)
        {
            if (PayMonth == null || PayYear == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string ExportDate = DateTime.Today.ToString("yyyyMMdd");
            string monyr = ' ' + PayMonth.ToString() + PayYear.ToString().Substring(2);
            string MyQry = "SELECT e.RegBankAc as AccountNo,'c' as c,p.AdjAmt as Amount, CONCAT(SUBSTRING(e.Name, 0, 35), " + monyr + ") as Narration " +
                            " FROM Employees  e, EmploymentHistory eh, Payroll p " +
                             " WHERE e.EmpID = eh.EmpID AND e.EmpID = p.EmpID AND e.CatAB = 'A' AND e.IsHDFC = 1 " +
                             " AND p.GenMonth = " + PayMonth + " AND p.GenYear =  " + PayYear +
                             " AND p.Frozen = 1 AND eh.ExitDate IS NULL AND p.ExcludeExcel=0 ORDER BY e.Name ";
            IEnumerable<RegExport> todth = db.Database.SqlQuery<RegExport>(MyQry).ToList();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Adjustments-HDFC" + monyr + ".xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(todth, Response.Output);
            Response.End();

            return RedirectToAction("Adjustment", new { mon = PayMonth, yr = PayYear });

        }

        [HttpPost]
        public ActionResult ExportAdjustmentsNonHDFC(int? PayMonth, int? PayYear)
        {
            if (PayMonth == null || PayYear == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string monyr = MyExtensions.MonthFromInt((int)PayMonth) + ' ' + PayYear + " salary";
            string ExportDate = DateTime.Today.ToString("yyyyMMdd");

            string MyQry = "SELECT '" + monyr + "' as ForMonth, e.Name, e.NRegBankAc as AccountNo, e.NRegIFSC as IFSC,  p.AdjAmt as Amount, '" + ExportDate + "' as ExportDate " +
                            " FROM Employees  e, EmploymentHistory eh, Payroll p " +
                             " WHERE e.EmpID = eh.EmpID AND e.EmpID = p.EmpID AND COALESCE(e.IsHDFC,0) = 0 AND e.CatAB='A' " +
                             " AND p.GenMonth = " + PayMonth + " AND p.GenYear =  " + PayYear +
                             " AND p.Frozen = 1 AND eh.ExitDate IS NULL AND p.ExcludeExcel=0 ORDER BY e.Name ";
            IEnumerable<NonRegExport> todt = db.Database.SqlQuery<NonRegExport>(MyQry).ToList();


            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Adjustments-NonHDFC" + monyr + ".xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(todt, Response.Output);
            Response.End();


            return RedirectToAction("Adjustment", new { mon = PayMonth, yr = PayYear });

        }


        /// <summary>
        /// Payroll Remarks. Happens anytime
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="yr"></param>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public ActionResult Instructions(int? mon, int? yr, string SearchString)
        {
            if (mon == null || yr == null)
            { //default to last month 
                mon = DateTime.Today.Month - 1;
                yr = DateTime.Today.AddMonths(-1).Year;
            }

            ViewBag.mon = mon; ViewBag.yr = yr;
            MakeMonYrRq();

            List<Payroll> payr = db.Payroll.Where(p => p.GenMonth == mon && p.GenYear == yr ).Include(p => p.Employees).Where(p => p.Employees.Name.Contains(SearchString)
                                       || p.Employees.NickName.Contains(SearchString)).ToList();


            //var payr = db.Payroll.Where(p => p.GenMonth == mon && p.GenYear == yr).Include(p => p.Employees).Where(p => p.Employees.Name.Contains(SearchString)
            //                           || p.Employees.NickName.Contains(SearchString)).ToList();

            ViewBag.Adjs = db.Payroll.Where(p => p.GenMonth == mon && p.GenYear == yr && (p.Instructions.Length>0 || p.ExcludeExcel ==true)).Include(p => p.Employees).ToList();

            return View(payr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Instructions(List<Payroll> sa)
        {
            Payroll tpr = new Payroll() { GenMonth = DateTime.Today.Month - 1, GenYear = DateTime.Today.AddMonths(-1).Year };
            if (ModelState.IsValid)
            {
                foreach (Payroll pr in sa)
                {
                    tpr = db.Payroll.FirstOrDefault(p => p.GenMonth == pr.GenMonth && p.GenYear == pr.GenYear && p.EmpID == pr.EmpID);
                    tpr.Instructions = pr.Instructions;
                    tpr.ExcludeExcel = pr.ExcludeExcel;
                    db.Entry(tpr).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Instructions", new { mon = tpr.GenMonth, yr = tpr.GenYear });
        }


        /// <summary>
        /// Used to form a generic request screen for reports that need only mon and yr
        /// </summary>
        private void MakeMonYrRq()
        {
            ViewBag.MonthBox = NunesHR.MyExtensions.MonthList();
            List<SelectListItem> slyr = new List<SelectListItem>();
            for (int i = 0; i < 10; i++)
            {
                slyr.Add(new SelectListItem { Value = DateTime.Today.AddYears(-i).Year.ToString(), Text = DateTime.Today.AddYears(-i).Year.ToString() });
            }
            ViewBag.YearBox = slyr;

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
