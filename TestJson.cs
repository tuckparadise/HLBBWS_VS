using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace HLBBWS2
{
    public class TestJson
    {
        public void main()
        {

            string savefilepath = HttpContext.Current.Server.MapPath("~/");
            savefilepath += "files\\2.txt";

            System.IO.StreamReader file =
            new System.IO.StreamReader(savefilepath);
            string line = "";

            while ((line = file.ReadLine()) != null)
            {
                //System.Console.WriteLine(line);
                /*
                line = line.Replace("\"", "");                

                int pFrom = line.IndexOf(@"\id\:") + @"\id\:".Length;
                int pTo = line.LastIndexOf(",");

                string result = line.Substring(pFrom, pTo - pFrom);
                */

                Array arr = line.Split(',');
                string s3 = ((string[])arr)[7];
                s3 = s3.Replace("\\", "");
                s3 = s3.Replace("\"", "");
                Array arr2 = s3.Split(':');
                string fe_id = "";

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
                
            }

            file.Close();
            //System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.  
            //System.Console.ReadLine();

            /*
            //File.WriteAllBytes(savefilepath + newfilename, byteFileContent);

            string str = @"{rawType:com.fireeye.v200.rest.model.SubmitUrlResponse,type:com.fireeye.v200.rest.model.RestModelBase,entity:{response:[{id:L483,link:{rel:status,href:/submissions/status/L483},submission_details:[{\vnc_port\:[0],\job_ids\:[null,6059],\id\:16773,\uuid\:\46d3b571-47bc-47d8-b439-a9c3d32e7208\}]}]}}";

            string abc = "\"submission_details\"";

            string asd = "\"id";

            //string abc = string.Concat(@""submission_details":"[{\"vnc_port\":[0],\"job_ids\":[6060],\"id\":16775,\"uuid\":\"13f1d160-be79-4dc6-b23f-20ead1bf148d\"}]"");

            string index = "\"id\":";


            int pFrom = str.IndexOf("\"id\":") + "\"id\":".Length;
            int pTo = str.LastIndexOf(",");

            string result = str.Substring(pFrom, pTo - pFrom);

            string St = "super exemple of string key : text I want to keep - end of my string";

            pFrom = St.IndexOf("key : ") + "key : ".Length;
            pTo = St.LastIndexOf(" - ");

            result = St.Substring(pFrom, pTo - pFrom);
            */
        }

      

    }
}