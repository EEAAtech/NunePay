using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss")]
    public class EDocTypesController : EAController
    {
        public EDocTypesController()
        {
            this.db = new NTHRPayEntities1();
        }

        // GET: EDocTypes
        public ActionResult Index()
        {
            return View(db.EDocTypes.ToList());
        }

        // GET: EDocTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EDocTypes eDocTypes = db.EDocTypes.Find(id);
            if (eDocTypes == null)
            {
                return HttpNotFound();
            }
            return View(eDocTypes);
        }

        // GET: EDocTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EDocTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EDocTypeID,EDocType")] EDocTypes eDocTypes)
        {
            if (ModelState.IsValid)
            {
                db.EDocTypes.Add(eDocTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eDocTypes);
        }

        // GET: EDocTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EDocTypes eDocTypes = db.EDocTypes.Find(id);
            if (eDocTypes == null)
            {
                return HttpNotFound();
            }
            return View(eDocTypes);
        }

        // POST: EDocTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EDocTypeID,EDocType")] EDocTypes eDocTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eDocTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eDocTypes);
        }

        // GET: EDocTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EDocTypes eDocTypes = db.EDocTypes.Find(id);
            if (eDocTypes == null)
            {
                return HttpNotFound();
            }
            return View(eDocTypes);
        }

        // POST: EDocTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EDocTypes eDocTypes = db.EDocTypes.Find(id);
            db.EDocTypes.Remove(eDocTypes);
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
