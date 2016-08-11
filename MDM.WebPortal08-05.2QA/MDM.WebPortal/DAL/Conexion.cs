using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.DAL
{
    public class Conexion
    {
        //public static string Cnx = @"Data Source=medpro-devbox;Initial Catalog=MDMPermissionIdentityv1; User id = mdmsql; Password = 1;Integrated Security=True";
        //public static string Cnx1 = @"data source=medpro-devbox;initial catalog=MedProDB;user id=mdmsql;password=1;MultipleActiveResultSets=True;App=EntityFramework&quot;";
        public static string Cnx = @"Data Source=FL-SQL02;Initial Catalog=MDMPermissionIdentityv1; User id = vobsql; Password = 1;Integrated Security=True";
        public static string Cnx1 = @"data source=FL-SQL02;initial catalog=MedProDB;user id=vobsql;password=1;MultipleActiveResultSets=True;App=EntityFramework&quot;";
    }
}