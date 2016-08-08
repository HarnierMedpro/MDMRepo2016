using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MDM.WebPortal.Tools
{
    public class CDAL : ConfiguratioDAL { }
    public class ConfiguratioDAL
    {
        private readonly static bool isProd = GetSettingsValue("IsProd", false);
        public static bool IsProd
        {
            get
            {
                return isProd;
            }
        }

        private readonly static bool isTimeAntiCache = GetSettingsValue("IsTimeAntiCache", false);
        public static bool IsTimeAntiCache
        {
            get
            {
                return isTimeAntiCache;
            }

        }

        private readonly static bool isVersionAntiCache = GetSettingsValue("IsVersionAntiCache", false);
        public static bool IsVersionAntiCache
        {
            get
            {
                return isVersionAntiCache;
            }
        }

        private readonly static bool isAntiCache = GetSettingsValue("IsAntiCache", false);
        public static bool IsAntiCache
        {
            get
            {
                return isAntiCache;
            }
        }

        private readonly static bool isLogDBSQL = GetSettingsValue("IsLogDBSQL", false);
        public static bool IsLogDBSQL
        {
            get
            {
                return isLogDBSQL;
            }
        }

        private readonly static bool isLog = GetSettingsValue("IsLog", false);
        public static bool IsLog
        {
            get
            {
                return isLog;
            }
        }

        private readonly static bool isWCF = GetSettingsValue("isWCF", false);
        public static bool IsWCF
        {
            get
            {
                return isWCF;
            }
        }

        private readonly static string wcfServiceRoot = GetSettingsValue("WCFServiceRoot", "http://localhost/MedPro.WebPortal.Host/");
        public static string WCFServiceRoot
        {
            get
            {
                return wcfServiceRoot;
            }
        }

        private readonly static string emailSettingsHost = GetSettingsValue("EmailSettings.host", "smtp.gmail.com");
        public static string EmailSettingsHost
        {
            get
            {
                return emailSettingsHost;
            }
        }

        private readonly static int emailSettingsPort = GetSettingsValue("EmailSettings.port", 587);
        public static int EmailSettingsPort
        {
            get
            {
                return emailSettingsPort;
            }
        }

        private readonly static bool emailSettingsSsl = GetSettingsValue("EmailSettings.ssl", true);
        public static bool EmailSettingsSsl
        {
            get
            {
                return emailSettingsSsl;
            }
        }

        private readonly static string emailSettingsFrom = GetSettingsValue("EmailSettings.from", "");
        public static string EmailSettingsFrom
        {
            get
            {
                return emailSettingsFrom;
            }
        }

        private readonly static string emailSettingsPassword = GetSettingsValue("EmailSettings.password", "");
        public static string EmailSettingsPassword
        {
            get
            {
                return emailSettingsPassword;
            }
        }

        public static string GetSetting(string name)
        {
            return GetSettingsValue<string>(name, null);
        }

        static public string GetConnectionString(string connectionStringName)
        {
            try
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            }
            catch (Exception e)
            {
                LDAL.Log.Fatal("exception", e);
                throw new Exception(e.Message, e);
            }
        }

        private readonly static int enPort = GetSettingsValue("Port", 0);
        public static int EnPort
        {
            get
            {
                return enPort;
            }
        }

        private readonly static string enHost = GetSettingsValue("Host", "");
        public static string EnHost
        {
            get
            {
                return enHost;
            }
        }

        private readonly static string enEmailsToSystemAdmin = GetSettingsValue("EmailsToSystemAdmin", "");
        public static string EnEmailsToSystemAdmin
        {
            get
            {
                return enEmailsToSystemAdmin;
            }
        }

        private readonly static string enEmailsTo = GetSettingsValue("EmailsTo", "");
        public static string EnEmailsTo
        {
            get
            {
                return enEmailsTo;
            }
        }

        private readonly static string enCredentialsMailFrom = GetSettingsValue("CredentialsMailFrom", "");
        public static string EnCredentialsMailFrom
        {
            get
            {
                return enCredentialsMailFrom;
            }
        }

        private readonly static string enCredentialsPass = GetSettingsValue("CredentialsPass", "");
        public static string EnCredentialsPass
        {
            get
            {
                return enCredentialsPass;
            }
        }

        private readonly static int slaApprovalIntervalMin = GetSettingsValue("SLAapprovalIntervalMin", 1);
        public static int EnSLAapprovalIntervalMin
        {
            get
            {
                return slaApprovalIntervalMin;
            }
        }

        private readonly static int pollingIntervalSec = GetSettingsValue("PollingIntervalSec", 60);
        public static int EnPollingIntervalSec
        {
            get
            {
                return pollingIntervalSec;
            }
        }

        private readonly static string enDefaultPdfPath = GetSettingsValue("DefaultPdfPath", "");
        public static string EnDefaultPdfPath
        {
            get
            {
                return enDefaultPdfPath;
            }
        }

        private readonly static bool enLogMemoryUsage = GetSettingsValue("LogMemoryUsage", false);
        public static bool EnLogMemoryUsage
        {
            get
            {
                return enLogMemoryUsage;
            }
        }

        public static T GetSettingsValue<T>(string key, T defaultValue)
        {
            object result = null;
            if (ConfigurationManager.AppSettings[key] != null)
            {
                var configValue = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(configValue))
                    return defaultValue;

                return (T)Convert.ChangeType(configValue, typeof(T));
            }

            if (result == null)
                return defaultValue;

            return (T)result;
        }
    }
}