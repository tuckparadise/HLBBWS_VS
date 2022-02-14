using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HLBBWS2
{
    public partial class checkscanstatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HLBBWS2.AppWS ws = new HLBBWS2.AppWS();
            string error = "";
            //ws.StartScan(ref error);

            ws.CheckJobStatus(ref error);

            //ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "closePage", "window.close();", true);
        }
    }
}