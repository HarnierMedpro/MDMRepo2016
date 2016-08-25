using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
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

                using (var context = new MedProDBEntities())
                {
                    try
                    {
                        context.AuditLogs.AddRange(auditLogS);
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        string body = string.Format(CultureInfo.InvariantCulture,
                                @"<center>
                                     <h3> Hi!</h3>
                                     <div> Unavailable to store Logs for: </div>
                                     <div> Table Name: {0} </div>
                                     <div> Audit Action: {1} </div>
                                     <div> EntityID: {2} </div>   
                                     <div> DateTime: {3} </div> 
                                     <div> User: {4} </div>                                                                     
                                  </center>",
                                  auditToStore.TableName, auditToStore.AuditAction, auditToStore.ModelPKey, auditToStore.AuditDateTime, auditToStore.UserLogons);

                        string Subject = "Audit Log Issues.";

                        Mail email = new Mail();
                        //email.To.Add("mpadmin@medprobill.com");
                        email.To.Add("hsuarez@medprobill.com");
                        email.Subject = Subject;
                        email.Body = body;
                        email.SendAsync();
                    }
                }
            }
        }



    }
}