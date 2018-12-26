using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MasterPage
{
    public partial class Contenedor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ifAngular.Src = "http://localhost:4200/b2b-proyectos/consulta/jcortes";
            ClientScript.RegisterStartupScript(GetType(), "storage", "fnLocalStorage('"+ "JCORTES" +"')", true);

        }
    }
}