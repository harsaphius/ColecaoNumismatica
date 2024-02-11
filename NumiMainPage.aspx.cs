using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiMainPage : System.Web.UI.Page
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

                lbl_messageUser.Text = "Bem-vindo " + user; 

                //if (!Page.IsPostBack)
                //{
                //string script1 = "document.getElementById('btn_home').classList.remove('hidden')";
                //string script2 = "document.getElementById('searchbar').classList.add('d-flex');";
                //string script3 = "document.getElementById('searchbar').classList.remove('hidden');";
                //string script4 = "document.getElementById('logoutbutton').classList.remove('hidden');";
                //string script5 = "document.getElementById('Admin').classList.remove('hidden');";

                //ScriptManager.RegisterStartupScript(this, GetType(), "RemoveClasses",
                //    script1 + script2 + script3 + script4 + script5, true);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "", "document.getElementById('btn_home').classList.remove('hidden')", true);
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "", "document.getElementById('searchbar').classList.add('d-flex')", true);
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "", "document.getElementById('searchbar').classList.remove('hidden')", true);
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "", "document.getElementById('logoutbutton').classList.remove('hidden')", true);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "", "document.getElementById('Admin').classList.remove('hidden')", true);
                //}
                /* else*/
                if (isAdmin == "Yes")
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "", "document.getElementById('btn_insertNewCoin').classList.remove('hidden')", true);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "", "document.getElementById('btn_manageCoins').classList.remove('hidden')", true);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "", "document.getElementById('btn_manageUsers').classList.remove('hidden')", true);
                }

                List<Money> LstMoney = new List<Money>();

                string query = "SELECT NCM.CodMN,NCM.Titulo, NCM.ValorCunho, NCI.Imagem FROM NumiCoinMoney AS NCM LEFT JOIN( SELECT NCI2.CodMN, NCI2.Imagem FROM (SELECT CodMN, MIN(CodImagem) AS FirstImage FROM NumiCoinMNImage GROUP BY CodMN) AS NCI JOIN NumiCoinMNImage AS NCI2 ON NCI.FirstImage = NCI2.CodImagem) AS NCI ON NCM.CodMN = NCI.CodMN;";

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);

                SqlCommand myCommand = new SqlCommand(query, myCon);

                myCon.Open();

                SqlDataReader dr = myCommand.ExecuteReader();

                while (dr.Read())
                {
                    Money record = new Money();
                    record.cod = Convert.ToInt32(dr["CodMN"]);
                    record.titulo = dr["Titulo"].ToString();
                    record.valorCunho = Convert.ToDecimal(dr["ValorCunho"]);
                    record.imagem = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dr["Imagem"]);
                    LstMoney.Add(record);
                }

                myCon.Close();
                rpt_mainpage.DataSource = LstMoney;
                rpt_mainpage.DataBind();

            }
        }


        protected void rpt_mainpage_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int cod = Convert.ToInt32(e.CommandArgument);
            ((Button)e.Item.FindControl("lbtn_like")).CommandArgument = cod.ToString();

            if (e.CommandName.Equals("like"))
            {
                //// Get the index of the item where the command was triggered
                
                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@CodMN", cod); //Adicionar o valor da tb_user ao parâmetro @nome
                myCommand.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);


                SqlParameter CodCollection = new SqlParameter();
                CodCollection.ParameterName = "@CodCollection";
                CodCollection.Direction = ParameterDirection.Output;
                CodCollection.SqlDbType = SqlDbType.Int;

                myCommand.Parameters.Add(CodCollection);

                SqlParameter UserHasCollection = new SqlParameter();
                UserHasCollection.ParameterName = "@UserHasCollection";
                UserHasCollection.Direction = ParameterDirection.Output;
                UserHasCollection.SqlDbType = SqlDbType.Int;

                myCommand.Parameters.Add(UserHasCollection);

                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiCollection"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                int AnswCodCollection = Convert.ToInt32(myCommand.Parameters["@CodCollection"].Value);
                int AnswUserHasCollection = Convert.ToInt32(myCommand.Parameters["@UserHasCollection"].Value);

                myCon.Close(); //Fechar a conexão

                if (AnswUserHasCollection == 0)
                {
                    lbl_message.Text = "Adicionado à sua coleção.";
                }
                else
                {
                    lbl_message.Text = "Utilizador já tem coleção.";
                }

            }
            //else if (e.CommandName == "dislike")
            //{
            //    // Get the index of the item where the command was triggered
            //    int index = e.Item.ItemIndex;

            //    // Get the LinkButton that triggered the command
            //    LinkButton lbtn_dislike = (LinkButton)rpt_mainpage.Items[index].FindControl("lbtn_dislike");

            //    // Get the icon inside the LinkButton and change its color
            //    HtmlGenericControl icon_dislike = (HtmlGenericControl)lbtn_dislike.Controls[0];
            //    icon_dislike.Style["color"] = "black"; // Change color to blue
            //}
        }
    }
}