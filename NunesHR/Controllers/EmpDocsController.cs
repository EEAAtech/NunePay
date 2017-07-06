using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace NunesHR.Controllers
{
    [Authorize(Roles = "Boss, HR,Anon")]
    public class EmpDocsController : EAController
    {
        public EmpDocsController()
        {
            this.db = new NTHRPayEntities1();

        }

        // GET: EmpDocs
        public ActionResult Index(string searchString, int? EmpID)
        {

            var empDocs = db.EmpDocs.Include(e => e.EDocTypes).Include(e => e.Employees);

            if (!String.IsNullOrEmpty(searchString))
            {
                empDocs = empDocs.Where(p => p.Employees.Name.Contains(searchString)
                                       || p.Employees.NickName.Contains(searchString));
                                
            }

            if (EmpID != null)
            {
                empDocs = empDocs.Where(e => e.EmpID == EmpID);
                ViewBag.EmpID = EmpID;
            }

            empDocs = empDocs.OrderBy(e => e.Employees.Name).ThenByDescending(e=> e.ExpiryDate);
            return View(empDocs.ToList());
        }

        // GET: EmpDocs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDocs empDocs = db.EmpDocs.Find(id);
            if (empDocs == null)
            {
                return HttpNotFound();
            }
            ViewBag.fn = "../../" + db.Config.FirstOrDefault().ImageSavePath + "/" + empDocs.Image;
            return View(empDocs);
        }

        // GET: EmpDocs/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                Session.Add("emp", 0);
                Session.Add("caller", "../EmpDocs/Create");
            }
            else
            {
                Session.Remove("emp");
                Session.Remove("caller");
                EmpDocsImg ed = new EmpDocsImg { EmpID = (int)id };
                ViewBag.EDocTypeID = new SelectList(db.EDocTypes, "EDocTypeID", "EDocType");

                return View(ed);
            }
            return RedirectToAction("../Employees");

            
            
        }

        // POST: EmpDocs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EDID,EmpID,EDocTypeID,UploadedFile,ExpiryDate")] EmpDocsImg empDocs)
        {
            if (ModelState.IsValid)
            {
                if (empDocs.UploadedFile != null)
                {
                    string fn = empDocs.UploadedFile.FileName.Substring(empDocs.UploadedFile.FileName.LastIndexOf('\\')+1);
                    fn = empDocs.EmpID + "_" + fn;
                    //string SavePath = System.IO.Path.Combine(Server.MapPath("~/" + db.Config.FirstOrDefault().ImageSavePath), fn);
                    //empDocs.UploadedFile.SaveAs(SavePath);

                    System.Drawing.Bitmap upimg = new System.Drawing.Bitmap(empDocs.UploadedFile.InputStream);
                    System.Drawing.Bitmap svimg = MyExtensions.CropUnwantedBackground(upimg);
                    svimg.Save(System.IO.Path.Combine(Server.MapPath("~/" + db.Config.FirstOrDefault().ImageSavePath), fn));

                    EmpDocs ed = new EmpDocs
                    {
                        EDID = empDocs.EDID,
                        EmpID = empDocs.EmpID,
                        EDocTypeID = empDocs.EDocTypeID,
                        Image = fn,
                        ExpiryDate = empDocs.ExpiryDate
                    };

                    db.EmpDocs.Add(ed);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { EmpID = empDocs.EmpID });
                }
                else
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                
            }

            ViewBag.EDocTypeID = new SelectList(db.EDocTypes, "EDocTypeID", "EDocType", empDocs.EDocTypeID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", empDocs.EmpID);
            return View(empDocs);
        }

        // GET: EmpDocs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDocs empDocs = db.EmpDocs.Find(id);
            if (empDocs == null)
            {
                return HttpNotFound();
            }
            ViewBag.EDocTypeID = new SelectList(db.EDocTypes, "EDocTypeID", "EDocType", empDocs.EDocTypeID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", empDocs.EmpID);
            return View(empDocs);
        }

        // POST: EmpDocs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EDID,EmpID,EDocTypeID,Image,ExpiryDate, Renewed")] EmpDocs empDocs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empDocs).State = EntityState.Modified;

                if(empDocs.Renewed == true)
                if (!db.EmpDocs.Any(d => d.EmpID == empDocs.EmpID && d.EDocTypeID == empDocs.EDocTypeID && d.ExpiryDate > empDocs.ExpiryDate && d.Renewed==false))
                {
                    throw new Exception("Please upload a new valid document before hiding this one");
                }

                db.SaveChanges();
                return RedirectToAction("Index", new { EmpID = empDocs.EmpID });
            }
            ViewBag.EDocTypeID = new SelectList(db.EDocTypes, "EDocTypeID", "EDocType", empDocs.EDocTypeID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "Name", empDocs.EmpID);
            return View(empDocs);
        }

        // GET: EmpDocs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDocs empDocs = db.EmpDocs.Find(id);
            if (empDocs == null)
            {
                return HttpNotFound();
            }
            return View(empDocs);
        }

        // POST: EmpDocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmpDocs empDocs = db.EmpDocs.Find(id);
            db.EmpDocs.Remove(empDocs);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult DocsExpire()
        {
            int dbe = db.Config.First().DocExpirePreWarning ?? 0;
            DateTime WarnDate = DateTime.Today.AddDays(dbe);
            var de = db.EmpDocs.Where(d => d.Renewed == false && d.ExpiryDate< WarnDate ).OrderBy(d => d.ExpiryDate).ToList();
            return PartialView("_DocsExpire", de);
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
