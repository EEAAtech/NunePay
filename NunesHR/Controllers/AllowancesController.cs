using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss, HR,Anon")]
    public class AllowancesController : EAController
    {
        public AllowancesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: Allowances
        public ActionResult Index(int id)
        {
            var allowance = db.Allowance.Where(a => a.EmpTypeID==id).Include(a => a.AllowanceTypes);
            ViewBag.foret = db.EmpTypes.FirstOrDefault(e => e.EmpTypeID == id).EmpType.ToString();
            ViewBag.EmpTypeID = id;
            return View(allowance.ToList());
        }

        // GET: Allowances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Allowance allowance = db.Allowance.Find(id);
            if (allowance == null)
            {
                return HttpNotFound();
            }
            return View(allowance);
        }

        // GET: Allowances/Create
        public ActionResult Create(int id)
        {
            ViewBag.ATID = new SelectList(db.AllowanceTypes, "ATID", "AllowanceType");
            ViewBag.EmpTypeID = id;
            ViewBag.EmpType = db.EmpTypes.Find(id).EmpType;
            return View();
        }

        // POST: Allowances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AllowanceID,ATID,WEF,EmpTypeID,PercOfWage,Amount")] Allowance allowance)
        {
            if (ModelState.IsValid)
            {
                db.Allowance.Add(allowance);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = allowance.EmpTypeID });
            }

            ViewBag.ATID = new SelectList(db.AllowanceTypes, "ATID", "AllowanceType", allowance.ATID);            
            return View(allowance);
        }

        // GET: Allowances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Allowance allowance = db.Allowance.Find(id);
            if (allowance == null)
            {
                return HttpNotFound();
            }
            ViewBag.ATID = new SelectList(db.AllowanceTypes, "ATID", "AllowanceType", allowance.ATID);
            ViewBag.EmpTypeID = new SelectList(db.EmpTypes, "EmpTypeID", "EmpType", allowance.EmpTypeID);
            return View(allowance);
        }

        // POST: Allowances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AllowanceID,ATID,WEF,EmpTypeID,PercOfWage,Amount")] Allowance allowance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(allowance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = allowance.EmpTypeID});
            }
            ViewBag.ATID = new SelectList(db.AllowanceTypes, "ATID", "AllowanceType", allowance.ATID);
            ViewBag.EmpTypeID = new SelectList(db.EmpTypes, "EmpTypeID", "EmpType", allowance.EmpTypeID);
            return View(allowance);
        }

        // GET: Allowances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Allowance allowance = db.Allowance.Find(id);
            if (allowance == null)
            {
                return HttpNotFound();
            }
            return View(allowance);
        }

        // POST: Allowances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Allowance allowance = db.Allowance.Find(id);
            db.Allowance.Remove(allowance);
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
