using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Areas.AudiTrails.Models
{
    public class AuditToStore
    {
       
        public string TableName { get; set; }

       
        public string UserLogons { get; set; }

      
        public DateTime AuditDateTime { get; set; }

       
        public string AuditAction { get; set; }

       
        public int ModelPKey { get; set; }

        public IList<TableInfo> tableInfos { get; set; }

        public AuditToStore()
        {
            tableInfos = new List<TableInfo>();
        }
    }
}