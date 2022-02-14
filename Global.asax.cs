using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;

namespace HLBBWS2
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            /*
            clsGlobal.TSW_SQL_DATA_SOURCE = ConfigurationManager.AppSettings["TSW_SQL_Server"].ToString();
            clsGlobal.TSW_SQL_DB_NAME = ConfigurationManager.AppSettings["TSW_SQL_Database"].ToString();
            clsGlobal.TSW_SQL_ID = ConfigurationManager.AppSettings["TSW_SQL_ID"].ToString();
            clsGlobal.TSW_SQL_PASSWORD = ConfigurationManager.AppSettings["TSW_SQL_Pwd"].ToString();
            clsGlobal.TSW_SQL_IS_WIN_AUTH = Convert.ToBoolean(Convert.ToInt32(ConfigurationManager.AppSettings["TSW_SQL_Auth_Mode"].ToString()));

            clsGlobal.MG_SQL_DATA_SOURCE = ConfigurationManager.AppSettings["MG_SQL_Server"].ToString();
            clsGlobal.MG_SQL_DB_NAME = ConfigurationManager.AppSettings["MG_SQL_Database"].ToString();
            clsGlobal.MG_SQL_ID = ConfigurationManager.AppSettings["MG_SQL_ID"].ToString();
            clsGlobal.MG_SQL_PASSWORD = ConfigurationManager.AppSettings["MG_SQL_Pwd"].ToString();
            clsGlobal.MG_SQL_IS_WIN_AUTH = Convert.ToBoolean(Convert.ToInt32(ConfigurationManager.AppSettings["MG_SQL_Auth_Mode"].ToString()));
            */
            /*
            clsGlobal.MG_SQL_DATA_SOURCE2 = ConfigurationManager.AppSettings["MG_SQL_Server2"].ToString();
            clsGlobal.MG_SQL_DB_NAME2 = ConfigurationManager.AppSettings["MG_SQL_Database2"].ToString();
            clsGlobal.MG_SQL_ID2 = ConfigurationManager.AppSettings["MG_SQL_ID2"].ToString();
            clsGlobal.MG_SQL_PASSWORD2 = ConfigurationManager.AppSettings["MG_SQL_Pwd2"].ToString();
            clsGlobal.MG_SQL_IS_WIN_AUTH2 = Convert.ToBoolean(Convert.ToInt32(ConfigurationManager.AppSettings["MG_SQL_Auth_Mode2"].ToString()));
            */
            /*
            clsGlobal.MG_SQL_DATA_SOURCE2 = ConfigurationManager.AppSettings["MG_SQL_Server2"].ToString();
            clsGlobal.MG_SQL_DB_NAME2 = ConfigurationManager.AppSettings["MG_SQL_Database2"].ToString();
            clsGlobal.MG_SQL_ID2 = ConfigurationManager.AppSettings["MG_SQL_ID2"].ToString();
            clsGlobal.MG_SQL_PASSWORD2 = ConfigurationManager.AppSettings["MG_SQL_Pwd2"].ToString();
            clsGlobal.MG_SQL_IS_WIN_AUTH2 = Convert.ToBoolean(Convert.ToInt32(ConfigurationManager.AppSettings["MG_SQL_Auth_Mode2"].ToString()));
            */

            KeyValueConfigurationCollection elements;
            string currentenvironment = ConfigurationManager.AppSettings["CurrentEnvironment"].ToString();
            elements = ((KeyValueConfigurationSection)ConfigurationManager.GetSection(currentenvironment)).Elements;

            clsGlobal.MG_SQL_DATA_SOURCE2 = elements["MG_SQL_Server2"].Value;
            clsGlobal.MG_SQL_DB_NAME2 = elements["MG_SQL_Database2"].Value;
            clsGlobal.MG_SQL_ID2 = elements["MG_SQL_ID2"].Value;
            clsGlobal.MG_SQL_PASSWORD2 = elements["MG_SQL_Pwd2"].Value;
            clsGlobal.MG_SQL_IS_WIN_AUTH2 = Convert.ToBoolean(Convert.ToInt32(elements["MG_SQL_Auth_Mode2"].Value));

            /*
            clsGlobal.SYSTEM_SAVE_LOG = Convert.ToBoolean(Convert.ToInt32(ConfigurationManager.AppSettings["System_Save_Log"].ToString()));
            clsGlobal.SYSTEM_LOG_PATH = ConfigurationManager.AppSettings["System_Log_Path"].ToString();

            clsGlobal.EDMS_HOST_URL = ConfigurationManager.AppSettings["EDMS_Host_URL"].ToString();
            clsGlobal.EDMS_HOST_TIMEOUT = ConfigurationManager.AppSettings["EDMS_Host_Timeout"].ToString();
            clsGlobal.EDMS_HOST_ID = ConfigurationManager.AppSettings["EDMS_Host_ID"].ToString();

            clsGlobal.EAI_HOST_URL = ConfigurationManager.AppSettings["EAI_Host_URL"].ToString();
            clsGlobal.EAI_HOST_TIMEOUT = ConfigurationManager.AppSettings["EAI_Host_Timeout"].ToString();
            clsGlobal.EAI_HOST_ID = ConfigurationManager.AppSettings["EAI_Host_ID"].ToString();
            clsGlobal.EAI_HOST_PWD = ConfigurationManager.AppSettings["EAI_Host_Pwd"].ToString();
            clsGlobal.EAI_TRANS_USER_ID = ConfigurationManager.AppSettings["EAI_Trans_User_ID"].ToString();
            clsGlobal.EAI_WEB_SERVICE_ID = ConfigurationManager.AppSettings["EAI_Web_Service_ID"].ToString();
            clsGlobal.EAI_MBASE_SERVICE_ID = ConfigurationManager.AppSettings["EAI_MBase_Service_ID"].ToString();
            clsGlobal.EAI_CUSTOM_BIZ_DATE = ConfigurationManager.AppSettings["EAI_Custom_Biz_Date"].ToString();
            */
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //// Create byte array to hold request bytes
            //byte[] inputStream = new byte[HttpContext.Current.Request.ContentLength];
            //// Read entire request inputstream
            //HttpContext.Current.Request.InputStream.Read(inputStream, 0, inputStream.Length);
            ////Set stream back to beginning
            //HttpContext.Current.Request.InputStream.Position = 0;
            ////Get  XML request
            //string requestString = System.Text.ASCIIEncoding.ASCII.GetString(inputStream);
            //clsLog.WriteLog(clsLog.ChannelType.EAI  requestString);

            /*
            //Log the web request
            byte[] buffer = null;
            try
            {
                buffer = new byte[HttpContext.Current.Request.InputStream.Length];
                HttpContext.Current.Request.InputStream.Read(buffer, 0, buffer.Length);
                HttpContext.Current.Request.InputStream.Position = 0;
                string strSoapRequestMsg = System.Text.Encoding.ASCII.GetString(buffer);
                if (!String.IsNullOrEmpty(strSoapRequestMsg))
                {
                    clsLog.WriteSystemLog(clsLog.MessageType.Info, "Request", strSoapRequestMsg.Replace("    ", " "));
                }
            }
            catch(Exception ex)
            {
                try
                {
                    clsLog.WriteSystemLog(clsLog.MessageType.Info, ex.Message, ex.StackTrace);
                }
                catch(Exception ex1)
                {
                    throw ex1;
                }
            }
            finally
            {
                buffer = null;
            }
            */
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}