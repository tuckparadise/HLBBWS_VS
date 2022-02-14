using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HLBBWS2
{
    public class clsGlobal
    {
        public static object objLockLog = new object();
        public static object objLockApp = new object();

        public static bool SYSTEM_SAVE_LOG = false;
        public static string SYSTEM_LOG_PATH = "";


        public static string TSW_SQL_DATA_SOURCE = "";
        public static string TSW_SQL_DB_NAME = "";
        public static string TSW_SQL_ID = "";
        public static string TSW_SQL_PASSWORD = "";
        public static bool TSW_SQL_IS_WIN_AUTH = false;

        public static string MG_SQL_DATA_SOURCE = "";
        public static string MG_SQL_DB_NAME = "";
        public static string MG_SQL_ID = "";
        public static string MG_SQL_PASSWORD = "";
        public static bool MG_SQL_IS_WIN_AUTH = false;

        public static string MG_SQL_DATA_SOURCE2 = "";
        public static string MG_SQL_DB_NAME2 = "";
        public static string MG_SQL_ID2 = "";
        public static string MG_SQL_PASSWORD2 = "";
        public static bool MG_SQL_IS_WIN_AUTH2 = false;

        public static string EDMS_HOST_URL = "";
        public static string EDMS_HOST_TIMEOUT = "";
        public static string EDMS_HOST_ID = "";

        public static string EAI_HOST_URL = "";
        public static string EAI_HOST_TIMEOUT = "";
        public static string EAI_HOST_ID = "";
        public static string EAI_HOST_PWD = "";
        public static string EAI_TRANS_USER_ID = "";
        public static string EAI_WEB_SERVICE_ID = "";
        public static string EAI_MBASE_SERVICE_ID = "";
        public static string EAI_CUSTOM_BIZ_DATE = "";

        public const string EAI_REQUEST_TELLER_ID = "42115";
        public const string EAI_REQUEST_BRANCH_CODE = "401";
        public const string EAI_REQUEST_CONTROLLER_ID = "6H";

        public const string LDAP_DOMAIN_NAME = "HLBHO";
        public const string LDAP_USER_NAME = "tsp_dms_appPool";
        public const string LDAP_USER_PWD = "user123!";
    }
}