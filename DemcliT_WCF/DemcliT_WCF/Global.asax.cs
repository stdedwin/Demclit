using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace DemcliT_WCF
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;

            if (request.HttpMethod == "OPTIONS")
            {
                string origin = request.Headers["origin"];
                string header = request.Headers["Access-Control-Request-Headers"];
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST,PUT,DELETE");
                response.AddHeader("Access-Control-Allow-Origin", origin!=null ? origin : "*");
                if (header != null) {
                    response.AddHeader("Access-Control-Allow-Headers", header);
                }
                response.End();
            }
            else
            {
                string origin = request.Headers["origin"];
                if (!string.IsNullOrEmpty(origin))
                {
                    response.AddHeader("Access-Control-Allow-Origin", origin);
                }
            }
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