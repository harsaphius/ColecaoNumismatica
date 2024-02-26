using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiCoinTerms : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string script = @"                      
                            document.getElementById('navBarDropDown').classList.remove('hidden');
                            document.getElementById('btn_home').classList.remove('hidden');
                            document.getElementById('btn_login').classList.remove('hidden');";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);
        }
    }
}