using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiManageUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string script;

            if (Session["Logado"] == null)
            {
                Session["Logado"] = null;
                Response.Redirect("NumiMainPage.aspx");
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

                script = @"
                             document.getElementById('navBarDropDown').classList.remove('hidden');
                            document.getElementById('btn_home').classList.remove('hidden');
                            document.getElementById('btn_mycollection').classList.remove('hidden');
                            document.getElementById('btn_alterarpw').classList.remove('hidden');
                            document.getElementById('btn_logout').classList.remove('hidden');
                            document.getElementById('Admin').classList.remove('hidden');";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);

                if (isAdmin == "Yes")
                {
                    script = @"
                             document.getElementById('btn_insertNewCoin').classList.remove('hidden');
                             document.getElementById('divider1').classList.remove('hidden');
                             document.getElementById('divider2').classList.remove('hidden');
                             document.getElementById('divider3').classList.remove('hidden');
                             document.getElementById('btn_manageCoins').classList.remove('hidden');
                             document.getElementById('btn_statistics').classList.remove('hidden');
                             document.getElementById('btn_registerNewUser').classList.remove('hidden');";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAdminButtons", script, true);
                }
            }

        }

        protected void rpt_manageUsers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView dataRow = (DataRowView)e.Item.DataItem; //Acesso campo a campo

                ((Label)e.Item.FindControl("lbl_cod")).Text = dataRow["CodUtilizador"].ToString();
                ((Label)e.Item.FindControl("lbl_user")).Text = dataRow["Utilizador"].ToString();
                CheckBox CheckAtiva = ((CheckBox)e.Item.FindControl("ckb_ativa"));
                CheckAtiva.Checked = Convert.ToBoolean(dataRow["ContaAtiva"]);
                ((TextBox)e.Item.FindControl("tb_email")).Text = dataRow["Email"].ToString();
                CheckBox CheckAdmin = ((CheckBox)e.Item.FindControl("ckb_admin"));
                CheckAdmin.Checked = Convert.ToBoolean(dataRow["NumiAdmin"]);

                ((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument = dataRow["CodUtilizador"].ToString();
                ((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument = dataRow["CodUtilizador"].ToString();
            }
        }

        protected void rpt_manageUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.Equals("imgBtn_grava"))
            {
                string query = "UPDATE NumiCoinUser SET ";

                SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);

                myConnection.Open();

                query += "ContaAtiva ='" + (((CheckBox)e.Item.FindControl("ckb_ativa")).Checked ? "1" : "0") + "', ";
                query += "Email ='" + ((TextBox)e.Item.FindControl("tb_email")).Text + "', ";
                query += "NumiAdmin ='" + (((CheckBox)e.Item.FindControl("ckb_admin")).Checked ? "1" : "0") + "'";
                query += "WHERE CodUtilizador=" + ((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument;

                SqlCommand myCommand = new SqlCommand(query, myConnection);
                myCommand.ExecuteNonQuery();
                myConnection.Close();
            }

            if (e.CommandName.Equals("imgBtn_apaga"))
            {
                if (Convert.ToInt32(e.CommandArgument) == Convert.ToInt32(((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument))
                {
                    lbl_message.Text = "Não é possível excluir-se a si próprio da lista de utilizadores! Solicite o auxílio de outro administrador!";
                    lbl_message.CssClass = "removed";
                }
                else
                {
                    string query = "DELETE FROM NumiCoinUser WHERE ";
                    query += "CodUtilizador=" + ((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument + "; ";
                    query += "DELETE FROM NumiCoinCollection WHERE ";

                    SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);

                    myConnection.Open();

                    query += "CodUtilizador=" + ((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument;

                    SqlCommand myCommand = new SqlCommand(query, myConnection);
                    myCommand.ExecuteNonQuery();
                    myConnection.Close();

                    rpt_manageUsers.DataBind();
                }
            }

        }
    }
}