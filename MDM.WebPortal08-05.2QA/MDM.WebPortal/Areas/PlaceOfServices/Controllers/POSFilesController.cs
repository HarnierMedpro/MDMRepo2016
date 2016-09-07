using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Areas.PlaceOfServices.Tools;
using MDM.WebPortal.Models.FromDB;


namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class POSFilesController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();
        private const string ServerLocation = @"\\fl-nas02\MDMFiles\";
        private string[] MediaExtensions = { ".jpeg", ".jpg", ".png", ".pdf",".rtf",".xls", ".xsls", ".doc"};

        // GET: PlaceOfServices/POSFiles
        public async Task<ActionResult> Index(int? locationPOSID)
        {
            if (locationPOSID == null)
            {
                return RedirectToAction("Index", "Error", new {area = "Error"});
            }
            var currentLocationPos = await db.LocationsPOS.FindAsync(locationPOSID);
            if (currentLocationPos == null)
            {
                return RedirectToAction("Index", "Error", new {area = "Error"});
            }
            /*Se pasa a la vista el ID del POS al cual se le quiere adjuntar ficheros*/
            ViewBag.Facitity_DBs_IDPK = locationPOSID;

            /*Se pasan los Tipos de los ficheros que estan almacenados en la base de datos*/
            ViewBag.FileTypeID = new SelectList(db.FileTypeIs.OrderBy(x => x.FileType_Name), "FileTypeID", "FileType_Name");

            var filesOfThisPos = currentLocationPos.POSFiles.OrderBy(x => x.FileName);
            return View(filesOfThisPos.ToList());
        }

        // GET: PlaceOfServices/POSFiles/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            POSFile pOSFile = await db.POSFiles.FindAsync(id);
            if (pOSFile == null)
            {
                return HttpNotFound();
            }
            return View(pOSFile);
        }

        // GET: PlaceOfServices/POSFiles/Create
        public ActionResult Create()
        {
            ViewBag.Facitity_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName");
            return View();
        }

        // POST: PlaceOfServices/POSFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "FileID,ServerLocation,FileName,FileExtension,Facitity_DBs_IDPK,Description,UploadTime")] POSFile pOSFile)
        {
            if (ModelState.IsValid)
            {
                pOSFile.FileID = Guid.NewGuid();
                db.POSFiles.Add(pOSFile);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Facitity_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName", pOSFile.Facitity_DBs_IDPK);
            return View(pOSFile);
        }

        // GET: PlaceOfServices/POSFiles/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            POSFile pOSFile = await db.POSFiles.FindAsync(id);
            if (pOSFile == null)
            {
                return HttpNotFound();
            }
            ViewBag.Facitity_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName", pOSFile.Facitity_DBs_IDPK);
            return View(pOSFile);
        }

        // POST: PlaceOfServices/POSFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "FileID,ServerLocation,FileName,FileExtension,Facitity_DBs_IDPK,Description,UploadTime")] POSFile pOSFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pOSFile).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Facitity_DBs_IDPK = new SelectList(db.LocationsPOS, "Facitity_DBs_IDPK", "PosName", pOSFile.Facitity_DBs_IDPK);
            return View(pOSFile);
        }

        // GET: PlaceOfServices/POSFiles/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            POSFile pOSFile = await db.POSFiles.FindAsync(id);
            if (pOSFile == null)
            {
                return HttpNotFound();
            }
            return View(pOSFile);
        }

        // POST: PlaceOfServices/POSFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            POSFile pOSFile = await db.POSFiles.FindAsync(id);
            db.POSFiles.Remove(pOSFile);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult SaveFile()
        {
            ViewBag.FileTypeID = new SelectList(db.FileTypeIs.OrderBy(x => x.FileType_Name), "FileTypeID", "FileType_Name");
            return View();
        }

        // Retrieve the account token from the current WindowsIdentity object
        // instead of calling the unmanaged LogonUser method in the advapi32.dll.
        private static IntPtr LogonUser()
        {
            IntPtr accountToken = WindowsIdentity.GetCurrent().Token;
            return accountToken;
        }

        public async Task<ActionResult> Save(HttpPostedFileBase fichero, [Bind(Include = "FileID, Facitity_DBs_IDPK, FileTypeID, Description")] VMPOSFile posFile)
        {
            // The Name of the Upload component is "files"
            if (fichero != null || fichero.ContentLength > 0)
            {
                if (ModelState.IsValid)
                {
                    string name;
                    Guid primaryKey = Guid.NewGuid();
                    var fileExt = Path.GetExtension(fichero.FileName);
                    //using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                    //{
                        try
                        {
                            var toStore = new POSFile
                            {
                                FileID = primaryKey,
                                Facitity_DBs_IDPK = posFile.Facitity_DBs_IDPK,
                                Description = posFile.Description,
                                FileTypeID = posFile.FileTypeID,
                                FileExtension = fileExt,
                                FileName = primaryKey.ToString(),
                                ServerLocation = ServerLocation
                            };

                            db.POSFiles.Add(toStore);
                            await db.SaveChangesAsync();

                           
                            //toStore.FileName = toStore.FileID.ToString();
                            //db.POSFiles.Attach(toStore);
                            //db.Entry(toStore).State = EntityState.Modified;
                            //await db.SaveChangesAsync();
                            name = toStore.FileName;

                            //commit transaction
                            //dbTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            //Rollback transaction if exception occurs
                           // dbTransaction.Rollback();
                            TempData["Error"] = "Something failed. Please try again!";
                            return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" });
                        }
                  //  }

                    // Retrieve the Windows account token for the current user.
                    //IntPtr logonToken = LogonUser();

                    // Retrieve the Windows account token for specific user.
                    IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("hsuarez", "Medpro705!", "MEDPROBILL");

                    WindowsIdentity wi = new WindowsIdentity(logonToken, "WindowsAuthentication");
                    WindowsImpersonationContext wic = null;
                    try
                    {
                        wic = wi.Impersonate();
                        // Thread is now impersonating you can call the backend operations here...

                        //var fileName = Path.GetFileName(fichero.FileName);

                        var fileExtension = Path.GetExtension(fichero.FileName);
                        var fileName = name + fileExtension;
                        var physicalPath = Path.Combine(ServerLocation, fileName);
                        fichero.SaveAs(physicalPath);
                    }
                    catch (Exception)
                    {
                        TempData["Access"] = "Something failed. You need to have permissions to read/write in this directory.";
                        return RedirectToAction("Index","LocationsPOS", new { area= "PlaceOfServices" }); 
                    }
                    finally
                    {
                        if (wic != null)
                        {
                            wic.Undo();
                        }
                    }

                    return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices" }); 
                }
            }
            TempData["Error"] = "You have to upload a file.";
            return RedirectToAction("Index", "LocationsPOS", new { area = "PlaceOfServices"});
        }

        public ActionResult DownloadsTest()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath(ServerLocation));
            System.IO.FileInfo[] fileNames = dir.GetFiles("*.*");
            List<string> items = new List<string>();

            foreach (var file in fileNames)
            {
                items.Add(file.Name);
            }

            return View(items);
        }  

         public FileResult Download(string ImageName)
            {
                return File(ServerLocation + ImageName, System.Net.Mime.MediaTypeNames.Application.Octet, ImageName);
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
