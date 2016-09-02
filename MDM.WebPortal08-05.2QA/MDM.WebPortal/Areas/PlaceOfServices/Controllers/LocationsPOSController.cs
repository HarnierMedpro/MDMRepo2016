using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MDM.WebPortal.Areas.AudiTrails.Controllers;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Areas.PlaceOfServices.Models.ViewModels;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Models.ViewModel.Delete;
using Microsoft.AspNet.Identity;

namespace MDM.WebPortal.Areas.PlaceOfServices.Controllers
{
    public class LocationsPOSController : Controller
    {
        private MedProDBEntities db = new MedProDBEntities();

        public ActionResult Index()
        {
            ViewData["FvPLists"] = db.FvPLists.OrderBy(x => x.FvPName).Select(x => new VMFvpList {id = x.FvPID, FvpName = x.FvPName});

            ViewData["TimeZone"] = new List<SelectListItem>
            {
                new SelectListItem{Value = "CST", Text = "CST"},
                new SelectListItem{Text = "EST", Value = "EST"}, 
                new SelectListItem{Text = "MST",Value = "MST"},
                new SelectListItem{Value = "PST", Text = "PST"},
            };

            ViewData["LevOfCare"] = db.Lev_of_Care.OrderBy(x => x.LevOfCareName).Select(x => new VMLevelOfCare {LevOfCareID = x.LevOfCareID, LevOfCareName = x.LevOfCareName});

            ViewData["Service"] = db.MPServices.OrderBy(x => x.ServName).Select(x => new VMMPService { MPServID = x.MPServID, ServName = x.ServName });
            
            return View();
        }

        public ActionResult Read_LocationPOS([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.LocationsPOS.OrderBy(x => x.PosName).ToList();
            return Json(result.ToDataSourceResult(request, x => new VMLocationsPOS
            {
                Facitity_DBs_IDPK = x.Facitity_DBs_IDPK, //PK
                PosName = x.PosName,
                FvPList_FvPID = x.FvPList_FvPID, //FK from dbo.FvPLists
                TaxID = x.TaxID,
                DBA_Name = x.DBA_Name,
                POSFAC_Manager = x.POSFAC_Manager
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update_LocationPOS([DataSourceRequest] DataSourceRequest request, [Bind(Include = "Facitity_DBs_IDPK, FvPList_FvPID, PosName, TaxID, DBA_Name, POSFAC_Manager")] VMLocationsPOS locToStore)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var storedInDb = await db.LocationsPOS.FindAsync(locToStore.Facitity_DBs_IDPK);

                    List<TableInfo> tableColumInfos = new List<TableInfo>();

                    if (! storedInDb.PosName.Equals(locToStore.PosName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "PosName", OldValue = storedInDb.PosName, NewValue = locToStore.PosName});
                        storedInDb.PosName = locToStore.PosName;
                    }
                    if (storedInDb.FvPList_FvPID != locToStore.FvPList_FvPID)
                    {
                        tableColumInfos.Add(new TableInfo { Field_ColumName = "FvPList_FvPID", OldValue = storedInDb.FvPList_FvPID.ToString(), NewValue = locToStore.FvPList_FvPID.ToString()});
                        storedInDb.FvPList_FvPID = locToStore.FvPList_FvPID;
                        /*Si el usuario decide modificar el tipo de POS, se necesita saber si era del tipo "PHY" y ahora va a ser "FAC"; para este caso 
                         si el POS ya tenia asignado un grupo de Doctores con esta nueva actualizacion hay que eliminar tal asignacion. Pues los POS de 
                         tipo FAC nunca van a tener un grupo de Doctores.*/
                        if (db.FvPLists.Find(locToStore.FvPList_FvPID).FvPName == "FAC" && storedInDb.PHYGroups_PHYGrpID != null)
                        {
                            storedInDb.PHYGroups_PHYGrpID = null;
                        }
                    }

                    if (storedInDb.TaxID != locToStore.TaxID)
                     {
                         if (storedInDb.TaxID == null)
                         {
                             tableColumInfos.Add(new TableInfo { Field_ColumName = "TaxID", NewValue = locToStore.TaxID });
                             storedInDb.TaxID = locToStore.TaxID;
                         }
                         else
                         {
                             tableColumInfos.Add(new TableInfo { Field_ColumName = "TaxID", OldValue = storedInDb.TaxID, NewValue = locToStore.TaxID });
                             storedInDb.TaxID = locToStore.TaxID;
                         }
                     }
                    if (storedInDb.DBA_Name != locToStore.DBA_Name)
                    {
                        if (storedInDb.DBA_Name == null)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "DBA_Name", NewValue = locToStore.DBA_Name });
                            storedInDb.DBA_Name = locToStore.DBA_Name;
                        }
                        else
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "DBA_Name", OldValue = storedInDb.DBA_Name, NewValue = locToStore.DBA_Name });
                            storedInDb.DBA_Name = locToStore.DBA_Name;
                        }
                    }
                    if (storedInDb.POSFAC_Manager != locToStore.POSFAC_Manager)
                    {
                        if (storedInDb.POSFAC_Manager == null)
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "POSFAC_Manager", NewValue = locToStore.POSFAC_Manager });
                            storedInDb.POSFAC_Manager = locToStore.POSFAC_Manager;
                        }
                        else
                        {
                            tableColumInfos.Add(new TableInfo { Field_ColumName = "POSFAC_Manager", OldValue = storedInDb.POSFAC_Manager, NewValue = locToStore.POSFAC_Manager });
                            storedInDb.POSFAC_Manager = locToStore.POSFAC_Manager;
                        }
                    }
                   
                    db.LocationsPOS.Attach(storedInDb);
                    db.Entry(storedInDb).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    AuditToStore auditLog = new AuditToStore
                    {
                        AuditDateTime = DateTime.Now,
                        UserLogons = User.Identity.GetUserName(),
                        AuditAction = "U",
                        TableName = "LocationsPOS",
                        ModelPKey = locToStore.Facitity_DBs_IDPK,
                        tableInfos = tableColumInfos
                    };

                    new AuditLogRepository().AddAuditLogs(auditLog);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("","Something failed. Please try again!");
                    return Json(new[] {locToStore}.ToDataSourceResult(request, ModelState));
                }
            }
            return Json(new[] { locToStore }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Read_LevelsOfCareByLocPOS(int locPOSID, [DataSourceRequest] DataSourceRequest request)
        {
            var result = db.LocPOS_LevOfCare.Where(pos => pos.LocationsPOS_Facitity_DBs_IDPK == locPOSID);
           
            return Json(result.ToDataSourceResult(request, x => new VMLocPOS_LevOfCare
            {
                LocPosLocID = x.LocPosLocID, //PK
                LocationsPOS_Facitity_DBs_IDPK = x.LocationsPOS_Facitity_DBs_IDPK, //LocationsPOS
                Lev_of_Care_LevOfCareID = x.Lev_of_Care_LevOfCareID //LevelOfCare
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read_ServicesByLocPOS(int locPOSID, [DataSourceRequest] DataSourceRequest request)
        {
            var result = db.LocPOS_MPServ.Where(x => x.LocationsPOS_Facitity_DBs_IDPK == locPOSID);
            return Json(result.ToDataSourceResult(request, x => new VMLocPOS_MPServ
            {
                LocPosMPServID = x.LocPosMPServID, //PK
                MPServices_MPServID = x.MPServices_MPServID, //Servicios
                LocationsPOS_Facitity_DBs_IDPK = x.LocationsPOS_Facitity_DBs_IDPK //LocationsPOS
            }), JsonRequestBehavior.AllowGet);
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
