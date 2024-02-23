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
            string script;

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

                script = @"
                            document.getElementById('navBarDropDown').classList.remove('hidden');
                            document.getElementById('btn_home').classList.remove('hidden');
                            document.getElementById('btn_mycollection').classList.remove('hidden');
                            document.getElementById('btn_alterarpw').classList.remove('hidden');
                            document.getElementById('searchbar').classList.add('d-flex');
                            document.getElementById('searchbar').classList.remove('hidden');
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
                             document.getElementById('btn_manageUsers').classList.remove('hidden');
                             document.getElementById('btn_statistics').classList.remove('hidden');
                             document.getElementById('btn_registerNewUser').classList.remove('hidden');";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAdminButtons", script, true);
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
                Panel panelControl = (Panel)e.Item.FindControl("imagePanel"); // Adjust control ID as needed

                // Check if the control is found and is of the expected type
                //if (panelControl != null)
                //{
                //    List<Money> LstMoney = GetDataForItem(dataRow);

                //    // Loop through each Money object and add the corresponding image to the Panel
                //    foreach (Money money in LstMoney)
                //    {
                //        Image img = new Image();
                //        img.ImageUrl = money.imagem;
                //        panelControl.Controls.Add(img);

                //        // Optionally, add a line break after each image
                //        panelControl.Controls.Add(new LiteralControl("<br />"));
                //    }
                //}

                ((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument = dataRow["CodMN"].ToString();
                ((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument = dataRow["CodMN"].ToString();
            }
        }

        //private List<Money> GetDataForItem(DataRowView dataRow)
        //{
        //    List<Money> moneyList = new List<Money>();

        //    // Get data from the DataRowView and construct Money objects
        //    Money money = new Money();
        //    money.imagem = dataRow["Imagem"].ToString(); // Assuming "Imagem" is the column containing image URLs
        //    moneyList.Add(money);

        //    // You can add more Money objects if needed, depending on your data structure

        //    return moneyList;
        //}

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

                SQLConnect(Values);
            }

            if (e.CommandName.Equals("imgBtn_apaga"))
            {
                SQLConnect(((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument);

                rpt_manageCoins.DataBind();
            }
        }
        public static void SQLConnect(string code)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados
            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@CodMN", code);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiRemoveCoin"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            myCon.Close();
        }
        public static void SQLConnect(List<string> values)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@CodMN", values[0]);
            myCommand.Parameters.AddWithValue("@Titulo", values[1]);
            myCommand.Parameters.AddWithValue("@Descricao", values[2]);
            myCommand.Parameters.AddWithValue("@CodEstado", values[3]);
            myCommand.Parameters.AddWithValue("@CodTipoMN", values[4]);
            if (values[5].Contains("."))
            {
                values[5] = values[5].Replace(".", ",");
                myCommand.Parameters.AddWithValue("@ValorAtual", Convert.ToDecimal(values[5]));
            }
            else
            {
                myCommand.Parameters.AddWithValue("@ValorAtual", Convert.ToDecimal(values[5]));
            }

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiUpdateCoin"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            myCon.Close();

        }
    }
}