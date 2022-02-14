using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.DirectoryServices;
//using SourceCode.Hosting.Client;
//using SourceCode.Workflow.Client;
//using LOADS_API;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using RestSharp;
//using json.net;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.Hosting;

namespace HLBBWS2
{
    /// <summary>
    /// Summary description for AppWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AppWS : System.Web.Services.WebService
    {        

        [WebMethod]
        public void StartScan(ref string error)
        {
            error = "";

            try
            {                
                DataSet ds = null;
                DataTable dt = null;
                SqlConnection conn = null;
                SqlDataAdapter sqlDA = null;
                
                string strDataSource2 = clsGlobal.MG_SQL_DATA_SOURCE2;
                string strDBName2 = clsGlobal.MG_SQL_DB_NAME2;
                string strID2 = clsGlobal.MG_SQL_ID2;
                string strPassword2 = clsGlobal.MG_SQL_PASSWORD2;
                bool blnIsWinAuth2 = clsGlobal.MG_SQL_IS_WIN_AUTH2;
                
                string connstr2 = @"Data Source=" + strDataSource2 + ";Initial Catalog=" + strDBName2 + ";Persist Security Info=True;User ID=" + strID2 + ";Password=" + strPassword2;
                if (blnIsWinAuth2)
                {
                    connstr2 = @"Data Source=" + strDataSource2 + ";Initial Catalog=" + strDBName2 + ";Integrated Security=True;";
                }
                
                conn = new SqlConnection(connstr2);

                sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = new SqlCommand("dbo.[usp_ws2_search_available_arn_to_start] @error OUTPUT", conn);
                sqlDA.SelectCommand.Parameters.AddWithValue("@error", "");
                //SqlCommand sqlcommand = new SqlCommand("dbo.usp_search_available_arn_to_start @error OUTPUT", conn);                            
                //sqlcommand.Parameters.AddWithValue("@error", "");

                ds = new DataSet("ds");
                sqlDA.Fill(ds);
                
                
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string arn = dt.Rows[i]["arn"].ToString();
                        string workflow = dt.Rows[i]["workflow"].ToString();

                        // lock the case by workflow and arn 
                        /*
                         create or alter procedure [dbo].[usp_ws2_lock_master_by_workflow_arn]
                            @workflow nvarchar(max) = null,
                            @arn nvarchar(max) = null
                        */
                        
                        SqlConnection conn_LockCase = new SqlConnection(connstr2);

                        SqlDataAdapter sqlDA_LockCase = new SqlDataAdapter();
                        sqlDA_LockCase.SelectCommand = new SqlCommand("dbo.[usp_ws2_lock_master_by_workflow_arn] @workflow,@arn", conn_LockCase);
                        sqlDA_LockCase.SelectCommand.Parameters.AddWithValue("@arn", arn);
                        sqlDA_LockCase.SelectCommand.Parameters.AddWithValue("@workflow", workflow);                        
                        
                        DataSet ds_LockCase = new DataSet("ds");
                        sqlDA_LockCase.Fill(ds_LockCase);

                        // start process the selected arn and workflow

                        var token = "";

                        SqlConnection conn1 = new SqlConnection(connstr2);

                        SqlDataAdapter sqlDA1 = new SqlDataAdapter();
                        sqlDA1.SelectCommand = new SqlCommand("dbo.[usp_ws2_list_attachment_to_scan] @arn, @workflow, @error OUTPUT", conn1);
                        sqlDA1.SelectCommand.Parameters.AddWithValue("@arn", arn);
                        sqlDA1.SelectCommand.Parameters.AddWithValue("@workflow", workflow);
                        sqlDA1.SelectCommand.Parameters.AddWithValue("@error", "");

                        //SqlCommand sqlcommand1 = new SqlCommand("dbo.usp_list_attachment_to_scan @arn, @workflow, @error OUTPUT", conn1);
                        //sqlcommand1.Parameters.AddWithValue("@arn", arn);
                        //sqlcommand1.Parameters.AddWithValue("@workflow", workflow);
                        //sqlcommand1.Parameters.AddWithValue("@error", "");

                        

                        DataSet ds1 = new DataSet("ds");
                        sqlDA1.Fill(ds1);
                        
                        if (ds1.Tables.Count > 0)
                        {
                            DataTable dt1 = ds1.Tables[0];
                            for (int j = 0; j < dt1.Rows.Count; j++)
                            {
                                string filename = dt1.Rows[j]["filename"].ToString();
                                string filecontent = dt1.Rows[j]["filecontent"].ToString();
                                string filetype = dt1.Rows[j]["filetype"].ToString();
                                var sha2_256 = dt1.Rows[j]["sha2_256"];
                                var attachmentuploaddate = dt1.Rows[j]["attachmentuploaddate"];

                                string fe_id = "";
                                string fe_url = "";
                                // start get config data 

                                string currentenvironment = ConfigurationManager.AppSettings["CurrentEnvironment"].ToString();
                                //_elements = ((KeyValueConfigurationSection)config.GetSection(_environment)).Elements;
                                //NameValueCollection config = (NameValueCollection)ConfigurationManager.GetSection(currentenvironment);
                                KeyValueConfigurationCollection elements;

                                //Configuration config1 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~", null, null, System.Environment.MachineName, System.Environment.MachineName + @"\WebConfigUser", "WebConfigPassword");                                
                                    
                                elements = ((KeyValueConfigurationSection)ConfigurationManager.GetSection(currentenvironment)).Elements;
                                //string savefilepath = elements["savefilepath"].Value;
                                string savefilepath = HttpContext.Current.Server.MapPath("~/");                               
                                savefilepath += "files\\";

                                //string savefileurl = elements["savefileurl"].Value;
                                //string savefileurl = Request.Url.AbsoluteUri;
                                string savefileurl = HttpContext.Current.Request.Url.ToString();
                                string filename1 = Path.GetFileName(savefileurl);
                                savefileurl = savefileurl.Replace(filename1, "");
                                savefileurl += "files/";

                                string fe_rest_url = elements["fe_rest_url"].Value;
                                string fe_rest_loginrequest_endpoint = elements["fe_rest_loginrequest_endpoint"].Value;                                
                                string fe_authorixationkeyvalue = elements["fe_authorixationkeyvalue"].Value;
                                string fe_rest_submiturlrequest_endpoint = elements["fe_rest_submiturlrequest_endpoint"].Value;
                                string fe_rest_submiturlrequest_analysistype = elements["fe_rest_submiturlrequest_analysistype"].Value;
                                string fe_rest_submiturlrequest_application = elements["fe_rest_submiturlrequest_application"].Value;
                                string fe_rest_submiturlrequest_force = elements["fe_rest_submiturlrequest_force"].Value;
                                string fe_rest_submiturlrequest_prefetch = elements["fe_rest_submiturlrequest_prefetch"].Value;
                                string fe_rest_submiturlrequest_priority = elements["fe_rest_submiturlrequest_priority"].Value;
                                string fe_rest_submiturlrequest_os = elements["fe_rest_submiturlrequest_os"].Value;
                                string fe_rest_submiturlrequest_timeout = elements["fe_rest_submiturlrequest_timeout"].Value;

                                string fe_rest_logoutrequest_endpoint = elements["fe_rest_logoutrequest_endpoint"].Value;

                                // end get config data 

                                // start save the file to web server for scanning                                
                                long long_datetime = long.Parse(System.DateTime.Now.ToString("yyyyMMddHHmmssff"));
                                //string newfilename = workflow + "_" + arn + "_" + long_datetime.ToString() + "." + filetype;
                                string newfilename = workflow + "_"  + long_datetime.ToString() + "." + filetype;

                                byte[] byteFileContent = Convert.FromBase64String(filecontent);
                                
                                File.WriteAllBytes(savefilepath + newfilename, byteFileContent);
                                // end save the file to web server for scanning
                                
                                if (File.Exists(savefilepath + newfilename))
                                {
                                    
                                    // generate url for the file 
                                    fe_url = savefileurl + newfilename;

                                    // start init scanning process 
                                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                                    System.Net.ServicePointManager.Expect100Continue = true;
                                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                    
                                    var client = new RestClient(fe_rest_url);
                                   
                                    // start get token
                                    var request = new RestRequest(fe_rest_loginrequest_endpoint, Method.POST);

                                    request.AddHeader("Authorization", fe_authorixationkeyvalue);

                                    string token_alpha;

                                    IRestResponse response = client.Execute(request);

                                    // 20211015- start log fireeye returned result 
                                    SqlConnection connFireEyeLog_Login = new SqlConnection(connstr2);

                                    SqlDataAdapter sqlDAFireEyeLog_Login = new SqlDataAdapter();
                                    //sqlDAFireEyeLog.SelectCommand = new SqlCommand("dbo.usp_save_fireeyelog @arn, @workflow, @filename, @sha2_256, @logs, @StatusCode, @StatusDescription, @ErrorMessage, @ErrorException, @IsSuccessful, @ResponseStatus, @Request, @Headers", connFireEyeLog);
                                    sqlDAFireEyeLog_Login.SelectCommand = new SqlCommand("dbo.usp_save_fireeyelog_login @arn, @workflow, @filename, @sha2_256, @logs, @StatusCode, @StatusDescription, @IsSuccessful, @ResponseStatus, @Request, @Headers", connFireEyeLog_Login);

                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@arn", arn.ToString());
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@workflow", workflow.ToString());
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@filename", filename.ToString());
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@sha2_256", sha2_256.ToString());
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@logs", response.Content.ToString());

                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@StatusCode", response.StatusCode.ToString());
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@StatusDescription", response.StatusDescription.ToString());                                                                        
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@IsSuccessful", response.IsSuccessful.ToString());
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@ResponseStatus", response.ResponseStatus.ToString());
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@Request", response.Request.ToString());
                                    sqlDAFireEyeLog_Login.SelectCommand.Parameters.AddWithValue("@Headers", response.Headers[3].Value.ToString());
                                    
                                    DataSet dsFireEyeLog_Login = new DataSet("ds");
                                    sqlDAFireEyeLog_Login.Fill(dsFireEyeLog_Login);
                                    //  20211015- end log fireeye returned result 

                                    //token = response.Headers[2].Value.ToString(); // raw content as string
                                    token = response.Headers[3].Value.ToString();
                                   // Array arr_alpha = token_alpha.Split('=');
                                   // token = ((string[])arr_alpha)[0];
                                    // end get token

                                    // start submit request 
                                    var request2 = new RestRequest(fe_rest_submiturlrequest_endpoint, Method.POST);

                                    request2.AddHeader("X-FeApi-Token", token.ToString());

                                    request2.RequestFormat = DataFormat.Json;
                                    urlsubmission_requestbody rb = new urlsubmission_requestbody();
                                    rb.analysistype = fe_rest_submiturlrequest_analysistype;
                                    rb.application = fe_rest_submiturlrequest_application;
                                    rb.force = fe_rest_submiturlrequest_force;
                                    rb.prefetch = fe_rest_submiturlrequest_prefetch;
                                    rb.priority = fe_rest_submiturlrequest_priority;

                                    string[] arr_os = fe_rest_submiturlrequest_os.Split(',');
                                    
                                    List<string> os = new List<string>();
                                    
                                    for (int x = 0; x < arr_os.Length; x++)
                                    {
                                        os.Add(arr_os[x].ToString());
                                    }
                                                                        
                                    rb.profiles = os;
                                    rb.timeout = fe_rest_submiturlrequest_timeout;
                                    List<string> iurl = new List<string>();
                                    iurl.Add(fe_url);
                                    //iurl.Add("http://kapps2:83/fireeye/");
                                    rb.urls = iurl;

                                    request2.AddJsonBody(rb);
                                    
                                    IRestResponse response2 = client.Execute(request2);
									
									// 20210813- start log fireeye returned result 
                                    SqlConnection connFireEyeLog = new SqlConnection(connstr2);

                                    SqlDataAdapter sqlDAFireEyeLog = new SqlDataAdapter();
                                    //sqlDAFireEyeLog.SelectCommand = new SqlCommand("dbo.usp_save_fireeyelog @arn, @workflow, @filename, @sha2_256, @logs, @StatusCode, @StatusDescription, @ErrorMessage, @ErrorException, @IsSuccessful, @ResponseStatus, @Request, @Headers", connFireEyeLog);
                                    sqlDAFireEyeLog.SelectCommand = new SqlCommand("dbo.usp_save_fireeyelog @arn, @workflow, @filename, @sha2_256, @logs, @StatusCode, @StatusDescription, @IsSuccessful, @ResponseStatus, @Request, @Headers", connFireEyeLog);

                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@arn", arn.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@workflow", workflow.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@filename", filename.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@sha2_256", sha2_256.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@logs", response2.Content.ToString());

                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@StatusCode", response2.StatusCode.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@StatusDescription", response2.StatusDescription.ToString());
                                    //sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ErrorMessage", response2.ErrorMessage.ToString());
                                    //sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ErrorException", response2.ErrorException.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@IsSuccessful", response2.IsSuccessful.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ResponseStatus", response2.ResponseStatus.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@Request", response2.Request.ToString());
                                    sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@Headers", response2.Headers.ToString());

                                    DataSet dsFireEyeLog = new DataSet("ds");
                                    sqlDAFireEyeLog.Fill(dsFireEyeLog);
                                    //  20210813- end log fireeye returned result 
                                    
                                    if (response2.ErrorMessage == null)
                                    {
                                        
                                        string content2 = response2.Content;
                                        /*
                                        content2 = content2.Replace("\"", "");

                                        int pFrom = content2.IndexOf(@"\id\:") + @"\id\:".Length;
                                        int pTo = content2.LastIndexOf(",");

                                        fe_id = content2.Substring(pFrom, pTo - pFrom);
                                        */

                                        /*
                                        Array arr = content2.Split(',');
                                        string s3 = ((string[])arr)[7];
                                        s3 = s3.Replace("\\", "");
                                        s3 = s3.Replace("\"", "");
                                        Array arr2 = s3.Split(':');
                                        fe_id = ((string[])arr2)[1];    
                                        */

                                        Array arr = content2.Split(',');
                                        string s3 = ((string[])arr)[7];
                                        s3 = s3.Replace("\\", "");
                                        s3 = s3.Replace("\"", "");
                                        Array arr2 = s3.Split(':');                                        

                                        if (((string[])arr2)[0] == "id")
                                        {
                                            fe_id = ((string[])arr2)[1];
                                        }
                                        else
                                        {
                                            string s8 = ((string[])arr)[8];
                                            s8 = s8.Replace("\\", "");
                                            s8 = s8.Replace("\"", "");
                                            Array arr8 = s8.Split(':');

                                            fe_id = ((string[])arr8)[1];
                                        }
                                    }// end init scanning process  

                                    //20211015- start logout process                                    
                                    var requestLogout = new RestRequest(fe_rest_logoutrequest_endpoint, Method.POST);
                                    
                                    requestLogout.AddHeader("X-FeApi-Token", token.ToString());

                                    IRestResponse responseLogout = client.Execute(requestLogout);

                                    //20211015- end logout process

                                    // 20211015- start log fireeye returned result 
                                    SqlConnection connFireEyeLog_Logout = new SqlConnection(connstr2);

                                    SqlDataAdapter sqlDAFireEyeLog_Logout = new SqlDataAdapter();
                                    //sqlDAFireEyeLog.SelectCommand = new SqlCommand("dbo.usp_save_fireeyelog @arn, @workflow, @filename, @sha2_256, @logs, @StatusCode, @StatusDescription, @ErrorMessage, @ErrorException, @IsSuccessful, @ResponseStatus, @Request, @Headers", connFireEyeLog);
                                    sqlDAFireEyeLog_Logout.SelectCommand = new SqlCommand("dbo.usp_save_fireeyelog_logout @arn, @workflow, @filename, @sha2_256, @logs, @StatusCode, @StatusDescription, @IsSuccessful, @ResponseStatus, @Request, @Headers", connFireEyeLog_Logout);

                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@arn", arn.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@workflow", workflow.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@filename", filename.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@sha2_256", sha2_256.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@logs", responseLogout.Content.ToString());

                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@StatusCode", responseLogout.StatusCode.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@StatusDescription", responseLogout.StatusDescription.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@IsSuccessful", responseLogout.IsSuccessful.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@ResponseStatus", responseLogout.ResponseStatus.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@Request", responseLogout.Request.ToString());
                                    sqlDAFireEyeLog_Logout.SelectCommand.Parameters.AddWithValue("@Headers", responseLogout.Headers.ToString());
                                    
                                    DataSet dsFireEyeLog_Logout = new DataSet("ds");
                                    sqlDAFireEyeLog_Logout.Fill(dsFireEyeLog_Logout);
                                    //  20211015- end log fireeye returned result 
                                }

                                // start save scan details to db 
                                SqlConnection conn2 = new SqlConnection(connstr2);
                                DataSet ds2 = new DataSet("ds");

                                SqlDataAdapter sqlDA2 = new SqlDataAdapter();

                                sqlDA2.SelectCommand = new SqlCommand("dbo.[usp_ws2_save_submission_detail] @arn, @workflow, @filename_original, @filename_new, @filecontent,@filetype, @attachmentuploaddate, @fe_file_url, @fe_fireeye_id, @md5checksum, @error OUTPUT", conn2);
                                //sqlDA1.SelectCommand.Parameters.AddWithValue("@arn", arn);

                                //SqlCommand sqlcommand2 = new SqlCommand("dbo.usp_save_submission_detail @arn, @workflow, @error OUTPUT", conn2);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@arn", arn);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@workflow", workflow);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@filename_original", filename);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@filename_new", newfilename);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@filecontent", filecontent);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@filetype", filetype);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@attachmentuploaddate", attachmentuploaddate);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@fe_file_url", fe_url);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@fe_fireeye_id", fe_id);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@md5checksum", sha2_256);
                                sqlDA2.SelectCommand.Parameters.AddWithValue("@error", "");
                                
                                sqlDA2.Fill(ds2);
                                conn2.Close();
                            }

                            // start update status for the arn and workflow to in progress
                            SqlConnection conn3 = new SqlConnection(connstr2);

                            SqlDataAdapter sqlDA3 = new SqlDataAdapter();
                            sqlDA3.SelectCommand = new SqlCommand("dbo.[usp_ws2_update_masterattachment_status_to_inprogress] @arn, @workflow, @fe_token, @error OUTPUT", conn3);
                            // SqlCommand sqlcommand3 = new SqlCommand("dbo.usp_update_masterattachment_status_to_inprogress @arn, @workflow, @error OUTPUT", conn3);
                            sqlDA3.SelectCommand.Parameters.AddWithValue("@arn", arn);
                            sqlDA3.SelectCommand.Parameters.AddWithValue("@workflow", workflow);
                            sqlDA3.SelectCommand.Parameters.AddWithValue("@fe_token", token);
                            sqlDA3.SelectCommand.Parameters.AddWithValue("@error", "");

                            DataSet ds3 = new DataSet("ds");
                            sqlDA3.Fill(ds3);

                            conn3.Close();
                            // end update status for the arn and workflow to in progress
                        }
                        conn1.Close();                        
                        // end process the selected arn and workflow
                    }
                }
                conn.Close();

            }
            catch (Exception ex)
            {
                error = "StartScan failed with exception: " + ex.Message.ToString();
                string errorDetail;
                errorDetail = "Input Param: N/A";
                LogErrorToDB("StartScan", "Exception", error, errorDetail);
            }
        }

        [WebMethod]
        public void CheckJobStatus(ref string error)
        {
            error = "";

            try
            {
                // log the result in db
                DataSet ds = null;
                DataTable dt = null;
                SqlConnection conn = null;
                SqlDataAdapter sqlDA = null;

                string strDataSource2 = clsGlobal.MG_SQL_DATA_SOURCE2;
                string strDBName2 = clsGlobal.MG_SQL_DB_NAME2;
                string strID2 = clsGlobal.MG_SQL_ID2;
                string strPassword2 = clsGlobal.MG_SQL_PASSWORD2;
                bool blnIsWinAuth2 = clsGlobal.MG_SQL_IS_WIN_AUTH2;

                string connstr2 = @"Data Source=" + strDataSource2 + ";Initial Catalog=" + strDBName2 + ";Persist Security Info=True;User ID=" + strID2 + ";Password=" + strPassword2;
                if (blnIsWinAuth2)
                {
                    connstr2 = @"Data Source=" + strDataSource2 + ";Initial Catalog=" + strDBName2 + ";Integrated Security=True;";
                }

                conn = new SqlConnection(connstr2);

                sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = new SqlCommand("dbo.[usp_ws2_search_available_arn_to_check] @error OUTPUT", conn);
                sqlDA.SelectCommand.Parameters.AddWithValue("@error", "");
                //SqlCommand sqlcommand = new SqlCommand("dbo.usp_search_available_arn_to_start @error OUTPUT", conn);                            
                //sqlcommand.Parameters.AddWithValue("@error", "");

                ds = new DataSet("ds");
                sqlDA.Fill(ds);               

               

                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string arn = "";
                        string workflow = "";

                        arn = dt.Rows[i]["arn"].ToString();
                         workflow = dt.Rows[i]["workflow"].ToString();

                        // lock the case by workflow and arn                         
                        SqlConnection conn_LockCase = new SqlConnection(connstr2);

                        SqlDataAdapter sqlDA_LockCase = new SqlDataAdapter();
                        sqlDA_LockCase.SelectCommand = new SqlCommand("dbo.[usp_ws2_lock_master_by_workflow_arn] @workflow,@arn", conn_LockCase);
                        sqlDA_LockCase.SelectCommand.Parameters.AddWithValue("@arn", arn);
                        sqlDA_LockCase.SelectCommand.Parameters.AddWithValue("@workflow", workflow);

                        DataSet ds_LockCase = new DataSet("ds");
                        sqlDA_LockCase.Fill(ds_LockCase);

                        // start process the selected arn and workflow
                        SqlConnection conn1 = new SqlConnection(connstr2);

                        SqlDataAdapter sqlDA1 = new SqlDataAdapter();
                        sqlDA1.SelectCommand = new SqlCommand("dbo.[usp_list_attachment_to_check] @arn, @workflow, @error OUTPUT", conn1);
                        sqlDA1.SelectCommand.Parameters.AddWithValue("@arn", arn);
                        sqlDA1.SelectCommand.Parameters.AddWithValue("@workflow", workflow);
                        sqlDA1.SelectCommand.Parameters.AddWithValue("@error", "");

                        //SqlCommand sqlcommand1 = new SqlCommand("dbo.usp_list_attachment_to_scan @arn, @workflow, @error OUTPUT", conn1);
                        //sqlcommand1.Parameters.AddWithValue("@arn", arn);
                        //sqlcommand1.Parameters.AddWithValue("@workflow", workflow);
                        //sqlcommand1.Parameters.AddWithValue("@error", "");

                        var token = "";

                        DataSet ds1 = new DataSet("ds");
                        sqlDA1.Fill(ds1);
                        
                        if (ds1.Tables.Count > 0)
                        {
                            DataTable dt1 = ds1.Tables[0];
                            for (int j = 0; j < dt1.Rows.Count; j++)
                            {
                                string filename_new = dt1.Rows[j]["filename_new"].ToString();
                                string fe_id = dt1.Rows[j]["fe_fireeye_id"].ToString();
                                string filetype = dt1.Rows[j]["filetype"].ToString();
                                var SHA2_256 = dt1.Rows[j]["SHA2_256"];
                                // string attachmentuploaddate = dt1.Rows[j]["attachmentuploaddate"].ToString();
                                //string fe_id = "";
                                //string fe_url = "";
                                // start get config data 

                                string currentenvironment = ConfigurationManager.AppSettings["CurrentEnvironment"].ToString();
                                //_elements = ((KeyValueConfigurationSection)config.GetSection(_environment)).Elements;
                                //NameValueCollection config = (NameValueCollection)ConfigurationManager.GetSection(currentenvironment);
                                KeyValueConfigurationCollection elements;

                                //Configuration config1 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~", null, null, System.Environment.MachineName, System.Environment.MachineName + @"\WebConfigUser", "WebConfigPassword");                                

                                elements = ((KeyValueConfigurationSection)ConfigurationManager.GetSection(currentenvironment)).Elements;
                                
                                //string savefileurl = elements["savefileurl"].Value;
                                string fe_rest_url = elements["fe_rest_url"].Value;
                                string fe_rest_loginrequest_endpoint = elements["fe_rest_loginrequest_endpoint"].Value;
                                string fe_rest_submiturlgetresult_endpoint = elements["fe_rest_submiturlgetresult_endpoint"].Value;
                                string fe_authorixationkeyvalue = elements["fe_authorixationkeyvalue"].Value;

                                string fe_rest_logoutrequest_endpoint = elements["fe_rest_logoutrequest_endpoint"].Value;
                                // end get config data 

                                // start checking process 
                                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                                System.Net.ServicePointManager.Expect100Continue = true;
                                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                
                                var client = new RestClient(fe_rest_url);

                                // start get token
                                var request = new RestRequest(fe_rest_loginrequest_endpoint, Method.POST);

                                request.AddHeader("Authorization", fe_authorixationkeyvalue);

                                IRestResponse response = client.Execute(request);
                                token = response.Headers[3].Value.ToString(); // raw content as string
                                // end get token

                                
                                var request2 = new RestRequest(fe_rest_submiturlgetresult_endpoint + fe_id, Method.GET);

                                request2.AddHeader("X-FeApi-Token", token.ToString());                                

                                IRestResponse response3 = client.Execute(request2);

                                // 20210823- start log fireeye returned result 
                                SqlConnection connFireEyeLog = new SqlConnection(connstr2);

                                SqlDataAdapter sqlDAFireEyeLog = new SqlDataAdapter();
                                //sqlDAFireEyeLog.SelectCommand = new SqlCommand("dbo.usp_save_fireeyelog2 @arn, @workflow, @filename, @sha2_256, @logs, @StatusCode, @StatusDescription, @ErrorMessage, @ErrorException, @IsSuccessful, @ResponseStatus, @Request, @Headers", connFireEyeLog);
                                sqlDAFireEyeLog.SelectCommand = new SqlCommand("dbo.usp_save_fireeyelog2 @arn, @workflow, @filename, @sha2_256, @logs, @StatusCode, @StatusDescription,  @IsSuccessful, @ResponseStatus, @Request, @Headers", connFireEyeLog);
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@arn", arn.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@workflow", workflow.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@filename", filename_new.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@sha2_256", SHA2_256.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@logs", response3.Content.ToString());

                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@StatusCode", response3.StatusCode.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@StatusDescription", response3.StatusDescription.ToString());
                                //sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ErrorMessage", response3.ErrorMessage.ToString());
                                //sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ErrorException", response3.ErrorException.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@IsSuccessful", response3.IsSuccessful.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ResponseStatus", response3.ResponseStatus.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@Request", response3.Request.ToString());
                                sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@Headers", response3.Headers.ToString());

                                DataSet dsFireEyeLog = new DataSet("ds");
                                sqlDAFireEyeLog.Fill(dsFireEyeLog);
                                //  20210823- end log fireeye returned result                                 

                                //StatusReply sr = new StatusReply();
                                string strResponse3 = response3.Content;
                                strResponse3 = strResponse3.Replace("\\", "");
                                strResponse3 = strResponse3.Replace("\"", "");

                                Array arr3 = strResponse3.Split(',');
                                string strSubmissionStatusFullString = ((string[])arr3)[0];
                                Array arrSubmissionStatus = strSubmissionStatusFullString.Split(':');
                                string strSubmissionStatus = ((string[])arrSubmissionStatus)[1];
                                string strverdicts = "";
                                string stranalysisStatus = "";

                                if (strSubmissionStatus == "Done")
                                {
                                    string strverdictFullString = ((string[])arr3)[1];
                                    Array arrverdict = strverdictFullString.Split(':');
                                    strverdicts = ((string[])arrverdict)[1];

                                    string stranalysisStatusFullString = ((string[])arr3)[2];
                                    Array arranalysisStatus = stranalysisStatusFullString.Split(':');
                                    stranalysisStatus = ((string[])arranalysisStatus)[1];

                                    string strErrorCode = "";
                                    string strErrorDesc = "";

                                    if (strverdicts != "non-malicious")
                                    {
                                         strErrorCode = "failed";
                                         strErrorDesc = strResponse3;
                                    }

                                    // start save results to detail table 
                                    SqlConnection conn2 = new SqlConnection(connstr2);
                                    DataSet ds2 = new DataSet("ds");

                                    SqlDataAdapter sqlDA2 = new SqlDataAdapter();

                                    sqlDA2.SelectCommand = new SqlCommand("dbo.[usp_ws2_save_result] @arn, @workflow, @fe_fireeye_id, @errorcode, @errordesc,@SHA2_256,  @error OUTPUT", conn2);                                    
                                    sqlDA2.SelectCommand.Parameters.AddWithValue("@arn", arn);
                                    sqlDA2.SelectCommand.Parameters.AddWithValue("@workflow", workflow);                                    
                                    sqlDA2.SelectCommand.Parameters.AddWithValue("@fe_fireeye_id", fe_id);
                                    sqlDA2.SelectCommand.Parameters.AddWithValue("@errorcode", strErrorCode);
                                    sqlDA2.SelectCommand.Parameters.AddWithValue("@errordesc", strErrorDesc);
                                    sqlDA2.SelectCommand.Parameters.AddWithValue("@SHA2_256", SHA2_256);                                    
                                    sqlDA2.SelectCommand.Parameters.AddWithValue("@error", "");

                                    sqlDA2.Fill(ds2);
                                    conn2.Close();

                                    // end save results to detail table 

                                    // start delete file
                                    string savefilepath = elements["savefilepath"].Value;
                                    //File.Delete(savefilepath + filename_new + "." + filetype);
                                    File.Delete(savefilepath + filename_new );
                                    // end delete file 

                                }

                                // end checking process   

                                //20211015- start logout process
                                // start get token
                                var requestLogout = new RestRequest(fe_rest_logoutrequest_endpoint, Method.POST);

                                requestLogout.AddHeader("X-FeApi-Token", token.ToString());

                                IRestResponse responseLogout = client.Execute(requestLogout);

                                //20211015- end logout process
                            }

                        }
                        conn1.Close();
                        // end process the selected arn and workflow

                        // start update master data                    
                        SqlConnection conn3 = new SqlConnection(connstr2);

                        SqlDataAdapter sqlDA3 = new SqlDataAdapter();
                        sqlDA3.SelectCommand = new SqlCommand("dbo.[usp_ws2_update_masterattachment_status_to_complete] @arn, @workflow, @error OUTPUT", conn3);
                        sqlDA3.SelectCommand.Parameters.AddWithValue("@arn", arn);
                        sqlDA3.SelectCommand.Parameters.AddWithValue("@workflow", workflow);
                        sqlDA3.SelectCommand.Parameters.AddWithValue("@error", "");

                        DataSet ds3 = new DataSet("ds");
                        sqlDA3.Fill(ds3);

                        conn3.Close();
                        // end update master data
                    }


                }
                conn.Close();

            }
            catch (Exception ex)
            {
                error = "CheckJobStatus failed with exception: " + ex.Message.ToString();
                string errorDetail;
                errorDetail = "Input Param: N/A";
                LogErrorToDB("CheckJobStatus", "Exception", error, errorDetail);
            }
        }

        private void LogErrorToDB(string APIName, string errorType, string errorMessage, string errorDetail)
        {
            DataSet ds = null;
            //DataTable dt = null;
            SqlConnection conn = null;
            SqlDataAdapter sqlDA = null;

            string strDataSource2 = clsGlobal.MG_SQL_DATA_SOURCE2;
            string strDBName2 = clsGlobal.MG_SQL_DB_NAME2;
            string strID2 = clsGlobal.MG_SQL_ID2;
            string strPassword2 = clsGlobal.MG_SQL_PASSWORD2;
            bool blnIsWinAuth2 = clsGlobal.MG_SQL_IS_WIN_AUTH2;

            string connstr = @"Data Source=" + strDataSource2 + ";Initial Catalog=" + strDBName2 + ";Persist Security Info=True;User ID=" + strID2 + ";Password=" + strPassword2;
            if (blnIsWinAuth2)
            {
                connstr = @"Data Source=" + strDataSource2 + ";Initial Catalog=" + strDBName2 + ";Integrated Security=True;";
            }
            conn = new SqlConnection(connstr);
            conn.Open();

            sqlDA = new SqlDataAdapter();
            sqlDA.SelectCommand = new SqlCommand("usp_ws_errorLog @APIName, @errorType, @errorMessage, @errorDetail, @APIErrorDt", conn);
            sqlDA.SelectCommand.Parameters.AddWithValue("@APIName", APIName);
            sqlDA.SelectCommand.Parameters.AddWithValue("@errorType", errorType);
            sqlDA.SelectCommand.Parameters.AddWithValue("@errorMessage", errorMessage);
            sqlDA.SelectCommand.Parameters.AddWithValue("@errorDetail", errorDetail);
            sqlDA.SelectCommand.Parameters.AddWithValue("@APIErrorDt", DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));

            ds = new DataSet("ds");
            sqlDA.Fill(ds);

            conn.Close();
        }

        public class urlsubmission_requestbody
        {
            public string analysistype;
            public string priority;
            public List<string> profiles;
            public string force;
            public string application;
            public string prefetch;
            public string timeout;
            //public string filename;
            public List<string> urls;
        }

        public class urlsubmission_responsebody
        {
            public string rawType;
            public string type;
            public Entity en;

            public string force;
            public string application;
            public string prefetch;
            public string timeout;
            //public string filename;
            public List<string> urls;
        }

        public class Entity
        {
            public List<string> response;
        }
    }
}
