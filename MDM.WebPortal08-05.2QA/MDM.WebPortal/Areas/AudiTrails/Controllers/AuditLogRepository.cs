using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Kendo.Mvc.Extensions;
using MDM.WebPortal.Areas.AudiTrails.Models;
using MDM.WebPortal.Models.FromDB;
using MDM.WebPortal.Tools;

namespace MDM.WebPortal.Areas.AudiTrails.Controllers
{
    public class AuditLogRepository
    {
        public void AddAuditLogs(AuditToStore auditToStore)
        {
            if (auditToStore != null && auditToStore.tableInfos.Any())
            {
                using (var context = new MedProDBEntities())
                {
                    IList<AuditLog> auditLogS = new List<AuditLog>();
                    auditLogS = auditToStore.tableInfos.Select(tableinfo => new AuditLog
                    {
                        TableName = auditToStore.TableName,
                        UserLogons = auditToStore.UserLogons,
                        AuditDateTime = auditToStore.AuditDateTime,
                        Field_ColumName = tableinfo.Field_ColumName,
                        OldValue = tableinfo.OldValue,
                        NewValue = tableinfo.NewValue,
                        AuditAction = auditToStore.AuditAction,
                        ModelPKey = auditToStore.ModelPKey
                    }).ToList();
                    context.AuditLogs.AddRange(auditLogS);
                    context.SaveChanges();
                }
            }
        }

        public void SaveLogs(List<AuditToStore> toStore)
        {
            if (toStore.Any())
            {
                IList<AuditLog> auditLogS = new List<AuditLog>();
                foreach (var aux in toStore.Select(item => item.tableInfos.Select(x => new AuditLog
                {
                    UserLogons = item.UserLogons,
                    AuditDateTime = item.AuditDateTime,
                    TableName = item.TableName,
                    AuditAction = item.AuditAction,
                    ModelPKey = item.ModelPKey,
                    Field_ColumName = x.Field_ColumName,
                    OldValue = x.OldValue,
                    NewValue = x.NewValue
                })))
                {
                    auditLogS.AddRange(aux);
                }
                using (var context = new MedProDBEntities())
                {
                    context.AuditLogs.AddRange(auditLogS);
                    context.SaveChanges();
                }
            } 
        }
    }
}