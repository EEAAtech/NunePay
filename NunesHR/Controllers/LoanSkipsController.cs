using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss,HR")]
    public class LoanSkipsController : EAController
    {
        public LoanSkipsController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: LoanSkips
        public ActionResult Index()
        {
            var loanSkip = db.LoanSkip.Include(l => l.Loans).Include(l => l.Loans.Employees).OrderByDescending(l => l.LoanID);
            return View(loanSkip.ToList());
        }

        // GET: LoanSkips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanSkip loanSkip = db.LoanSkip.Find(id);
            if (loanSkip == null)
            {
                return HttpNotFound();
            }
            return View(loanSkip);
        }

        // GET: LoanSkips/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ToPay = db.Loans.Where(l => l.LoanID == id).Select(x => x.Amount / x.PayMonths).FirstOrDefault();

            ViewBag.LoanID = id;
            return View();
        }

        // POST: LoanSkips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LoanSkipID,LoanID,PayDate,Amount,Reason")] LoanSkip loanSkip)
        {
            if (ModelState.IsValid)
            {
                db.LoanSkip.Add(loanSkip);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LoanID = new SelectList(db.Loans, "LoanID", "LoanID", loanSkip.LoanID);
            return View(loanSkip);
        }

        // GET: LoanSkips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanSkip loanSkip = db.LoanSkip.Find(id);
            if (loanSkip == null)
            {
                return HttpNotFound();
            }
            ViewBag.LoanID = new SelectList(db.Loans, "LoanID", "LoanID", loanSkip.LoanID);
            return View(loanSkip);
        }

        // POST: LoanSkips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoanSkipID,LoanID,PayDate,Amount,Reason")] LoanSkip loanSkip)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loanSkip).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LoanID = new SelectList(db.Loans, "LoanID", "LoanID", loanSkip.LoanID);
            return View(loanSkip);
        }

        // GET: LoanSkips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanSkip loanSkip = db.LoanSkip.Find(id);
            if (loanSkip == null)
            {
                return HttpNotFound();
            }
            return View(loanSkip);
        }

        // POST: LoanSkips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoanSkip loanSkip = db.LoanSkip.Find(id);
            db.LoanSkip.Remove(loanSkip);
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
