using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiManageCoins : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logado"] == null)
            {
                Response.Redirect("NumiLoginUser.aspx");
            }
            else if (Session["Logado"].ToString() == "Yes" || Page.IsPostBack == true)
            {
                string isAdmin = Session["Admin"].ToString();
                string user = Session["User"].ToString();

                Label lblMessage = Master.FindControl("lbl_message") as Label;

                if (lblMessage != null)
                {
                    lblMessage.Text = "Bem-vindo " + user;
                }

                string script2 = @"
                            document.getElementById('btn_home').classList.remove('hidden');
                            document.getElementById('btn_mycollection').classList.remove('hidden');
                            document.getElementById('btn_alterarpw').classList.remove('hidden');
                            document.getElementById('searchbar').classList.add('d-flex');
                            document.getElementById('searchbar').classList.remove('hidden');
                            document.getElementById('btn_logout').classList.remove('hidden');
                            document.getElementById('Admin').classList.remove('hidden');";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script2, true);

                if (isAdmin == "Yes")
                {
                    string script3 = @"
                              document.getElementById('btn_insertNewCoin').classList.remove('hidden');
                              document.getElementById('btn_manageCoins').classList.remove('hidden');
                              document.getElementById('btn_manageUsers').classList.remove('hidden');
                              document.getElementById('btn_statistics').classList.remove('hidden');
                              document.getElementById('btn_registerNewUser').classList.remove('hidden');";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAdminButtons", script3, true);
                }
            }
        }

        protected void rpt_manageCoins_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView dataRow = (DataRowView)e.Item.DataItem; //Acesso campo a campo

                ((Label)e.Item.FindControl("lbl_cod")).Text = dataRow["CodMN"].ToString();
                ((TextBox)e.Item.FindControl("tb_titulo")).Text = dataRow["Titulo"].ToString();
                ((TextBox)e.Item.FindControl("tb_descricao")).Text = dataRow["Descricao"].ToString();
                ((DropDownList)e.Item.FindControl("ddl_estado")).SelectedValue = dataRow["CodEstado"].ToString();
                ((DropDownList)e.Item.FindControl("ddl_tipo")).SelectedValue = dataRow["CodTipoMN"].ToString();
                ((TextBox)e.Item.FindControl("tb_valorAtual")).Text = dataRow["ValorAtual"].ToString();

                ((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument = dataRow["CodMN"].ToString();
                ((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument = dataRow["CodMN"].ToString();
            }
        }

        protected void rpt_manageCoins_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.Equals("imgBtn_grava"))
            {
                List<string> Values = new List<string>();
                Values.Add(((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument);
                Values.Add(((TextBox)e.Item.FindControl("tb_titulo")).Text);
                Values.Add(((TextBox)e.Item.FindControl("tb_descricao")).Text);
                Values.Add(((DropDownList)e.Item.FindControl("ddl_estado")).SelectedValue);
                Values.Add(((DropDownList)e.Item.FindControl("ddl_tipo")).SelectedValue);
                Values.Add(((TextBox)e.Item.FindControl("tb_valorAtual")).Text);

                Classes.MyFunctions.SQLConnect(Values);
            }

            if (e.CommandName.Equals("imgBtn_apaga"))
            {
                Classes.MyFunctions.SQLConnect(((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument);

                rpt_manageCoins.DataBind();
            }
        }
    }
}