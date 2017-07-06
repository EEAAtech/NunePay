using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss")]
    public class EmpTypesController : EAController
    {
        public EmpTypesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: EmpTypes
        public ActionResult Index()
        {
            return View(db.EmpTypes.ToList());
        }

        // GET: EmpTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpTypes empTypes = db.EmpTypes.Find(id);
            if (empTypes == null)
            {
                return HttpNotFound();
            }
            return View(empTypes);
        }

        // GET: EmpTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmpTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmpTypeID,EmpType,HasDailyAllowance")] EmpTypes empTypes)
        {
            if (ModelState.IsValid)
            {
                db.EmpTypes.Add(empTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(empTypes);
        }

        // GET: EmpTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpTypes empTypes = db.EmpTypes.Find(id);
            if (empTypes == null)
            {
                return HttpNotFound();
            }
            return View(empTypes);
        }

        // POST: EmpTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmpTypeID,EmpType,HasDailyAllowance")] EmpTypes empTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empTypes);
        }

        // GET: EmpTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpTypes empTypes = db.EmpTypes.Find(id);
            if (empTypes == null)
            {
                return HttpNotFound();
            }
            return View(empTypes);
        }

        // POST: EmpTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmpTypes empTypes = db.EmpTypes.Find(id);
            db.EmpTypes.Remove(empTypes);
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
