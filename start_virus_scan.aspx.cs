using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.IO;

namespace HLBBWS2
{
    public partial class start_virus_scan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*
            string path;           
            path = HttpContext.Current.Server.MapPath("~/");
            path += "files\\";

            path =Request.Url.AbsoluteUri;
            path = HttpContext.Current.Request.Url.ToString();
            string filename = Path.GetFileName(path);
            path = path.Replace(filename, "");
            path += "files/";
            */

            HLBBWS2.AppWS ws = new HLBBWS2.AppWS();
            string error = "";
            ws.StartScan(ref error);
            
            //ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "closePage", "window.close();", true);
        }
    }
}