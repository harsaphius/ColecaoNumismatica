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
    public partial class NumiManageUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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
                string query = "DELETE FROM produtos WHERE ";

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