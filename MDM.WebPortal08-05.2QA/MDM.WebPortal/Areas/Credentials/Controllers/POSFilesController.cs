

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.Credentials.Models.ViewModel;
using MDM.WebPortal.Areas.PlaceOfServices.Tools;
using MDM.WebPortal.Data_Annotations;
using MDM.WebPortal.Models.FromDB;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.Credentials.Controllers
{
    [SetPermissions]
    public class POSFilesController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();
        //private const string ServerLocation = @"\\fl-nas02\MDMFiles\";
        private const string ServerLocation = @"\\fl-nas02\TestShares\";
        
        private string[] MediaExtensions = { ".jpeg", ".jpg", ".png", ".pdf", ".rtf", ".xls", ".xsls", ".doc" };

        public async Task<ActionResult> PosFiles(int? masterPOSID)
        {
            if (masterPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            var currentLocationPos = await db.MasterPOS.FindAsync(masterPOSID);
            if (currentLocationPos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            var result = new List<VMPOSFile>();
            if (currentLocationPos.MasterFiles.Any())
            {
                result = currentLocationPos.MasterFiles.Select(x => new VMPOSFile
                {
                    MasterPOSID = masterPOSID.Value,
                    FileName = x.FileName,
                    FileID = x.FileID,
                    Description = x.Description,
                    FileExtension = x.FileExtension,
                    FileTypeID = x.FileTypeID
                }).ToList();
            }
            ViewBag.MasterPOS = masterPOSID;
            return View(result);
        }

        public ActionResult Saveds(HttpPostedFileBase files)
        {
            // The Name of the Upload component is "files"
            if (files != null)
            {
               
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                var fileName = Path.GetFileName(files.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // The files are not actually saved in this demo
                    files.SaveAs(physicalPath);
             
            }

            // Return an empty string to signify success
            return Content("");
        }


        public async Task<ActionResult> Upload_File([DataSourceRequest] DataSourceRequest request,
            HttpPostedFileBase files, [Bind(Include = "FileID, MasterPOSID, FileTypeID, Description")] VMPOSFile posFile, int ParentID)
        {
            if (ModelState.IsValid)
            {
                if (files == null || files.ContentLength == 0)
                {
                    ModelState.AddModelError("","You need to choose a valid file. Please try again!");
                    return Json(new[] {posFile}.ToDataSourceResult(request, ModelState));
                }
                string fileName = Path.GetFileName(files.FileName);
                try
                {
                   
                    Guid primaryKey = Guid.NewGuid();
                    var fileExtension = Path.GetExtension(files.FileName);

                    var toStore = new MasterFile
                    {
                        MasterPOS_MasterPOSID = posFile.MasterPOSID,
                        Description = posFile.Description,
                        FileTypeID = posFile.FileTypeID,
                        FileExtension = fileExtension,
                        FileName = primaryKey.ToString() + fileExtension,
                        ServerLocation = ServerLocation,
                        UploadTime = DateTime.Now
                    };

                    db.MasterFiles.Add(toStore);
                    await db.SaveChangesAsync();
                    posFile.FileID = toStore.FileID;
                    posFile.FileName = toStore.FileName;
                    posFile.FileExtension = toStore.FileExtension;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Somethig failed. Please try again!");
                    return Json(new[] { posFile }.ToDataSourceResult(request, ModelState));
                }

                // Retrieve the Windows account token for specific user.
                IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("hsuarez", "Medpro705!", "MEDPROBILL"); //USER WITH PERMISSIONS TO CREATE AND READ
                //IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("MDMTest", "Medpro1", "MEDPROBILL");  //USER WITHOUT PERMISSIONS

                WindowsIdentity wi = new WindowsIdentity(logonToken, "WindowsAuthentication");
                WindowsImpersonationContext wic = null;
                try
                {
                    wic = wi.Impersonate();
                    // Thread is now impersonating you can call the backend operations here...
                    var physicalPath = Path.Combine(ServerLocation, fileName);
                    files.SaveAs(physicalPath);
                }
                catch (Exception)
                {
                    MasterFile currentStoredInDb = db.MasterFiles.FirstOrDefault(x => x.FileName == fileName);
                    if (currentStoredInDb != null)
                    {
                        db.MasterFiles.Remove(currentStoredInDb);
                        db.SaveChanges();
                    }
                    ModelState.AddModelError("", "Something failed. You need to have permissions to read/write in this directory.");
                    return Json(new[] { posFile }.ToDataSourceResult(request, ModelState));
                }
                finally
                {
                    if (wic != null)
                    {
                        wic.Undo();
                    }
                }
            }
            return Json(new[] { posFile }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Read_POSFiles([DataSourceRequest] DataSourceRequest request, int? MasterPOSID)
        {
            var result = db.MasterFiles.OrderBy(x => x.FileName).ToList();
            if (MasterPOSID != null)
            {
                result = result.Where(x => x.MasterPOS_MasterPOSID == MasterPOSID.Value).ToList();
            }
            return Json(result.ToDataSourceResult(request, x => new VMPOSFile
            {
                MasterPOSID = x.MasterPOS_MasterPOSID,
                FileID = x.FileID, 
                Description = x.Description,
                FileExtension = x.FileExtension,
                FileTypeID = x.FileTypeID,
                FileName = x.FileName
            }), JsonRequestBehavior.AllowGet);
        }

        // GET: PlaceOfServices/POSFiles
        public async Task<ActionResult> Index(int? locationPOSID)
        {
            if (locationPOSID == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            var currentLocationPos = await db.MasterPOS.FindAsync(locationPOSID);
            if (currentLocationPos == null)
            {
                return RedirectToAction("Index", "Error", new { area = "BadRequest" });
            }
            /*Se pasa a la vista el ID del POS al cual se le quiere adjuntar ficheros*/
            ViewBag.Facitity_DBs_IDPK = locationPOSID;

            /*Se pasan los Tipos de los ficheros que estan almacenados en la base de datos*/
            ViewBag.FileTypeID = new SelectList(db.FileTypeIs.OrderBy(x => x.FileType_Name).Where(x => x.FileLevel.Equals("pos",StringComparison.InvariantCultureIgnoreCase)), "FileTypeID", "FileType_Name");

            var filesOfThisPos = currentLocationPos.MasterFiles.OrderBy(x => x.Description).Where(x => x.FileTypeI.FileLevel.Equals("pos", StringComparison.InvariantCultureIgnoreCase));
            return View(filesOfThisPos.ToList());
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

        public async Task<ActionResult> Save(HttpPostedFileBase fichero, [Bind(Include = "FileID, MasterPOSID, FileTypeID, Description")] VMPOSFile posFile)
        {
            // The Name of the Upload component is "files"
            if (fichero != null || fichero.ContentLength > 0)
            {
                if (ModelState.IsValid)
                {
                    string fileName = Path.GetFileName(fichero.FileName);
                    Guid primaryKey = Guid.NewGuid();
                    var fileExtension = Path.GetExtension(fichero.FileName);
                    //using (DbContextTransaction dbTransaction = db.Database.BeginTransaction())
                    //{
                    try
                    {
                        var toStore = new MasterFile
                        {
                            MasterPOS_MasterPOSID = posFile.MasterPOSID,
                            Description = posFile.Description,
                            FileTypeID = posFile.FileTypeID,
                            FileExtension = fileExtension,
                            FileName = primaryKey.ToString() + fileExtension,
                            ServerLocation = ServerLocation,
                            UploadTime = DateTime.Now
                        };

                        db.MasterFiles.Add(toStore);
                        await db.SaveChangesAsync();
                        posFile.FileID = toStore.FileID;

                        //toStore.FileName = toStore.FileID.ToString();
                        //db.POSFiles.Attach(toStore);
                        //db.Entry(toStore).State = EntityState.Modified;
                        //await db.SaveChangesAsync();
                        fileName = toStore.FileName;

                        //commit transaction
                        //dbTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        //Rollback transaction if exception occurs
                        // dbTransaction.Rollback();
                        TempData["Error"] = "Something failed. Please try again!";
                        return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                    }
                    //  }

                    // Retrieve the Windows account token for the current user.
                    //IntPtr logonToken = LogonUser();

                    // Retrieve the Windows account token for specific user.
                    //IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("MMDFiles", "Medpro123!", "MEDPROBILL");
                    IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("mpadmin", "Southbeach5050!", "MEDPROBILL");

                    WindowsIdentity wi = new WindowsIdentity(logonToken, "WindowsAuthentication");
                    WindowsImpersonationContext wic = null;
                    try
                    {
                        wic = wi.Impersonate();
                        // Thread is now impersonating you can call the backend operations here...
                        var physicalPath = Path.Combine(ServerLocation, fileName);
                        fichero.SaveAs(physicalPath);
                    }
                    catch (Exception)
                    {
                        TempData["Access"] = "Something failed. You need to have permissions to read/write in this directory.";
                        MasterFile currentStoredInDb = db.MasterFiles.FirstOrDefault(x => x.FileName == fileName);
                        if (currentStoredInDb != null)
                        {
                            db.MasterFiles.Remove(currentStoredInDb);
                            db.SaveChanges();
                        }
                        return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                    }
                    finally
                    {
                        if (wic != null)
                        {
                            wic.Undo();
                        }
                    }


                    AuditToStore auditLog = new AuditToStore
                    {
                        UserLogons = User.Identity.GetUserName(),
                        AuditDateTime = DateTime.Now,
                        AuditAction = "I",
                        TableName = "POSFile",
                        ModelPKey = posFile.FileID,
                        tableInfos = new List<TableInfo>
                        {
                            new TableInfo{Field_ColumName = "MasterPOSID", NewValue = posFile.MasterPOSID.ToString()},
                            new TableInfo{Field_ColumName = "Description", NewValue = posFile.Description},
                            new TableInfo{Field_ColumName = "FileTypeID", NewValue = posFile.FileTypeID.ToString()},
                            new TableInfo{Field_ColumName = "FileName", NewValue = primaryKey.ToString() + fileExtension},
                            new TableInfo{Field_ColumName = "ServerLocation", NewValue = ServerLocation},
                            new TableInfo{Field_ColumName = "UploadTime", NewValue = DateTime.Now.ToShortDateString()+DateTime.Now.ToShortTimeString()},
                        }
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);

                    return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
                }
            }
            TempData["Error"] = "You have to upload a file.";
            return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
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

       
        //public ActionResult Download(string ImageName)
        //{
           
        //    //IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("MMDFiles", "Medpro123!", "MEDPROBILL"); //Have permissions
        //    IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("mpadmin", "Southbeach5050!", "MEDPROBILL");

        //    WindowsIdentity wi = new WindowsIdentity(logonToken, "WindowsAuthentication");
        //    WindowsImpersonationContext wic = null;
        //    try
        //    {
        //        wic = wi.Impersonate();
               
                 

        //            MasterFile currentStoredInDb = db.MasterFiles.FirstOrDefault(x => x.FileName == ImageName);
        //            if (currentStoredInDb != null)
        //            {
        //                AuditToStore auditLog = new AuditToStore
        //                {
        //                    UserLogons = User.Identity.GetUserName(),
        //                    AuditDateTime = DateTime.Now,
        //                    AuditAction = "D",
        //                    TableName = "POSFile",
        //                    ModelPKey = currentStoredInDb.FileID,
        //                    tableInfos = new List<TableInfo>
        //                         {
        //                             new TableInfo{Field_ColumName = "FileName", OldValue = ImageName, NewValue = ImageName}
        //                         }
        //                };

        //                new AuditLogRepository().AddAuditLogs(auditLog);
        //            }
                   
        //            try
        //            {
        //                return File(ServerLocation + ImageName, System.Net.Mime.MediaTypeNames.Application.Octet, ImageName);
        //            }
        //            catch (Exception)
        //            {
        //                TempData["Error"] = "Downloading.......";
        //                return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
        //            }
        //    }
        //    catch (Exception)
        //    {
        //        TempData["Access"] = "Something failed. You need to have permissions to read/write in this directory.";
        //        return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
        //    }
        //    finally
        //    {
        //        if (wic != null)
        //        {
        //            wic.Undo();
        //        }
        //    }
        //}


        public ActionResult Download(string ImageName)
        {
            IntPtr logonToken = ActiveDirectoryHelper.GetAuthenticationHandle("mpadmin", "Southbeach5050!", "MEDPROBILL");

            WindowsIdentity wi = new WindowsIdentity(logonToken, "WindowsAuthentication");
            WindowsImpersonationContext ctx = null;

            try
            {
                ctx = wi.Impersonate();
                return File(ServerLocation + ImageName, System.Net.Mime.MediaTypeNames.Application.Octet, ImageName);
            }
            catch
            {
                TempData["Access"] = "Something failed. You need to have permissions to read/write in this directory.";
                return RedirectToAction("Index_MasterPOS", "MasterPOS", new { area = "Credentials" });
            }
            finally
            {
                ctx.Undo();
            }
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
