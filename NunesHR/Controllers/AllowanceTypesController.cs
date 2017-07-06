using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss")]
    public class AllowanceTypesController : EAController
    {
        public AllowanceTypesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: AllowanceTypes
        public ActionResult Index()
        {
            return View(db.AllowanceTypes.ToList());
        }

        // GET: AllowanceTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllowanceTypes allowanceTypes = db.AllowanceTypes.Find(id);
            if (allowanceTypes == null)
            {
                return HttpNotFound();
            }
            return View(allowanceTypes);
        }

        // GET: AllowanceTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AllowanceTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ATID,AllowanceType")] AllowanceTypes allowanceTypes)
        {
            if (ModelState.IsValid)
            {
                db.AllowanceTypes.Add(allowanceTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(allowanceTypes);
        }

        // GET: AllowanceTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllowanceTypes allowanceTypes = db.AllowanceTypes.Find(id);
            if (allowanceTypes == null)
            {
                return HttpNotFound();
            }
            return View(allowanceTypes);
        }

        // POST: AllowanceTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ATID,AllowanceType")] AllowanceTypes allowanceTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(allowanceTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(allowanceTypes);
        }

        // GET: AllowanceTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AllowanceTypes allowanceTypes = db.AllowanceTypes.Find(id);
            if (allowanceTypes == null)
            {
                return HttpNotFound();
            }
            return View(allowanceTypes);
        }

        // POST: AllowanceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AllowanceTypes allowanceTypes = db.AllowanceTypes.Find(id);
            db.AllowanceTypes.Remove(allowanceTypes);
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
