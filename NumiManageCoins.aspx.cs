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

                string script = @"
                            document.getElementById('btn_home').classList.remove('hidden');
                            document.getElementById('btn_mycollection').classList.remove('hidden');
                            document.getElementById('searchbar').classList.add('d-flex');
                            document.getElementById('searchbar').classList.remove('hidden');
                            document.getElementById('logoutbutton').classList.remove('hidden');
                            document.getElementById('Admin').classList.remove('hidden');";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);

                if (isAdmin == "Yes")
                {
                    string script2 = @"
                            document.getElementById('btn_insertNewCoin').classList.remove('hidden');
                            document.getElementById('btn_manageCoins').classList.remove('hidden');
                            document.getElementById('btn_manageUsers').classList.remove('hidden');
                            document.getElementById('btn_registerNewUser').classList.remove('hidden');";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAdminButtons", script2, true);
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
                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados
                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@CodMN", ((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument);
                myCommand.Parameters.AddWithValue("@Titulo", ((TextBox)e.Item.FindControl("tb_titulo")).Text);
                myCommand.Parameters.AddWithValue("@Descricao", ((TextBox)e.Item.FindControl("tb_descricao")).Text);
                myCommand.Parameters.AddWithValue("@CodEstado", ((DropDownList)e.Item.FindControl("ddl_estado")).SelectedValue);
                myCommand.Parameters.AddWithValue("@CodTipoMN", ((DropDownList)e.Item.FindControl("ddl_tipo")).SelectedValue);
                if (((TextBox)e.Item.FindControl("tb_valorAtual")).Text.Contains("."))
                {
                    ((TextBox)e.Item.FindControl("tb_valorAtual")).Text = ((TextBox)e.Item.FindControl("tb_valorAtual")).Text.Replace(".", ",");
                    myCommand.Parameters.AddWithValue("@ValorAtual", Convert.ToDecimal(((TextBox)e.Item.FindControl("tb_valorAtual")).Text));
                }
                else
                {
                    myCommand.Parameters.AddWithValue("@ValorAtual", Convert.ToDecimal(((TextBox)e.Item.FindControl("tb_valorAtual")).Text));
                }

                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiUpdateCoin"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                myCon.Close();
            }

            if (e.CommandName.Equals("imgBtn_apaga"))
            {
                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados
                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@CodMN", ((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument);
                
                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiRemoveCoin"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                myCon.Close();

                rpt_manageCoins.DataBind();
            }
        }
    }
}