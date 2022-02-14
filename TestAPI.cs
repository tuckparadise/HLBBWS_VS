using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;

namespace HLBBWS2
{
    public class TestAPI
    {
        public void main()
        {
            string url = "https://gorest.co.in/public/v1/posts";

            var client = new RestClient(url);
            
            var request2 = new RestRequest(url, Method.GET);

            //request2.AddHeader("X-FeApi-Token", token.ToString());

            IRestResponse response3 = client.Execute(request2);

            string Content = response3.Content.ToString();
            string StatusCode = response3.StatusCode.ToString();
            string StatusDescription = response3.StatusDescription.ToString();
           // string ErrorMessage = response3.ErrorMessage.ToString();
            //string ErrorException = response3.ErrorException.ToString();
            string IsSuccessful = response3.IsSuccessful.ToString();
            string ResponseStatus = response3.ResponseStatus.ToString();
            string Request = response3.Request.ToString();
            string Headers = response3.Headers.ToString();
           

            /*
            string Content = response3.Content.ToString();


            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@logs", response2.Content.ToString());

            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@StatusCode", response2.StatusCode.ToString());
            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@StatusDescription", response2.StatusDescription.ToString());
            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ErrorMessage", response2.ErrorMessage.ToString());
            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ErrorException", response2.ErrorException.ToString());
            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@IsSuccessful", response2.IsSuccessful.ToString());
            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@ResponseStatus", response2.ResponseStatus.ToString());
            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@Request", response2.Request.ToString());
            sqlDAFireEyeLog.SelectCommand.Parameters.AddWithValue("@Headers", response2.Headers.ToString());
            */

        }
    }
}