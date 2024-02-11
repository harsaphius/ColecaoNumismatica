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

                ((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument = dataRow["CodMN"].ToString();
                ((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument = dataRow["CodMN"].ToString();
            }
        }

    }
}