using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class TesteCalendario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string script = @"                      
                            document.getElementById('calendar')";
                           

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);
        }
    }
}