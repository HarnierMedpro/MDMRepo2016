using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using log4net.Config;
using log4net;
using log4net.Repository.Hierarchy;

namespace MDM.WebPortal.Tools
{
    public class LDAL : LogDAL { }
    public class LogDAL
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        public static ILog Log
        {
            get { return log; }
        }
        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }

        private static readonly LINQSQLSTATEMENTWriter linqLog = new LINQSQLSTATEMENTWriter();
        public static LINQSQLSTATEMENTWriter LinqLog
        {
            get { return linqLog; }
        }


        private static Dictionary<string, long> perfLog = new Dictionary<string, long>();
        private class Profile
        {
            public Profile() { total = 0; min = 10000000; max = 0; count = 0; }
            public long total { get; set; }
            public long min { get; set; }
            public long max { get; set; }
            public long count { get; set; }
        }
        private static Dictionary<string, Profile> perfCollLog = new Dictionary<string, Profile>();

        public static void StartPerf(string key, bool collect = false)
        {
            if (CDAL.IsLog)
            {
                perfLog[key] = DateTime.Now.Ticks;
                if (collect && !perfCollLog.ContainsKey(key)) perfCollLog[key] = new Profile();
            }
        }


        public static void CollectPerf(string key)
        {
            if (CDAL.IsLog)
            {
                var profile = perfCollLog[key];
                long time = DateTime.Now.Ticks - perfLog[key];
                profile.total += time;
                profile.count++;
                if (profile.max < time) profile.max = time;
                if (profile.min > time) profile.min = time;
            }
        }

        public static void EndPerf(string key, string info = "")
        {
            if (CDAL.IsLog)
            {
                long now = DateTime.Now.Ticks;
                var method = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod();
                log.Info(method.DeclaringType.Name + "." + method.Name + "  " + key + ": " + (now - perfLog[key]) / 10000 + (info == "" ? "" : ("      <-  " + info)));
            }
        }
        public static void ShowPerf(string key, string info = "")
        {
            if (CDAL.IsLog)
            {
                var method = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod();
                log.Info(method.DeclaringType.Name + "." + method.Name + "  " + key + ":: total: " + perfCollLog[key].total / 10000 + ", count:" + perfCollLog[key].count + ", avg:" + perfCollLog[key].total / perfCollLog[key].count / 10000 + ", min: " + perfCollLog[key].min / 10000 + ", max: " + perfCollLog[key].max / 10000 + (info == "" ? "" : ("      <-  " + info)));
            }
        }
    }

    public class LINQSQLSTATEMENTWriter : System.IO.TextWriter
    {
        public override void Write(char[] buffer, int index, int count)
        {
            string value = new String(buffer, index, count);
            WriteToLog(value);
        }

        public override void Write(string value)
        {
            WriteToLog(value);
        }
        private void WriteToLog(string value)
        {
            if (!(string.IsNullOrEmpty(value.Trim()) || value.Contains("Context: SqlProvider")))
            {
                value = value.Substring(0, value.Length - 1);
                if (!value.StartsWith("--"))
                    value = "\n---------------------- SQL STATEMENT\n" + value;
                LDAL.Log.Warn(value);
            }
            //System.Diagnostics.Debug.Write(value);
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }

    }
}