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

                string script2 = @"
                            document.getElementById('btn_home').classList.remove('hidden');
                            document.getElementById('btn_mycollection').classList.remove('hidden');
                            document.getElementById('searchbar').classList.add('d-flex');
                            document.getElementById('searchbar').classList.remove('hidden');
                            document.getElementById('logoutbutton').classList.remove('hidden');
                            document.getElementById('Admin').classList.remove('hidden');";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script2, true);

                if (isAdmin == "Yes")
                {
                    string script3 = @"
                            document.getElementById('btn_insertNewCoin').classList.remove('hidden');
                            document.getElementById('btn_manageCoins').classList.remove('hidden');
                            document.getElementById('btn_manageUsers').classList.remove('hidden');
                            document.getElementById('btn_registerNewUser').classList.remove('hidden');";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAdminButtons", script3, true);
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
            //// Get the index of the item where the command was triggered
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados
            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
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
            myCommand.CommandText = "NumiCollectionExists"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            myCon.Close();

            int AnswCodCollection = Convert.ToInt32(myCommand.Parameters["@CodCollection"].Value);
            int AnswUserHasCollection = Convert.ToInt32(myCommand.Parameters["@UserHasCollection"].Value);

            if (e.CommandName.Equals("like"))
            {
                string modalScript = @"$('#exampleModal').modal('show');";

                //Register script to show the modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "exampleModalScript", modalScript, true);
                Session["ModalOpened"] = true;
            }
            if (Session["ModalOpened"] != null && (bool)Session["ModalOpened"])
            {
                string value = ddl_estado.SelectedValue;

                if (Session["AddClicked"] != null && (bool)Session["AddClicked"])
                {
                    //((LinkButton)e.Item.FindControl("lbtn_like")).CommandArgument = e.CommandArgument.ToString();
                    SqlCommand myCommand2 = new SqlCommand(); //Novo commando SQL 
                    myCommand2.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);
                    myCommand2.Parameters.AddWithValue("@CodMN", e.CommandArgument);
                    myCommand2.Parameters.AddWithValue("@CodCollection", AnswCodCollection);

                    myCommand2.Parameters.AddWithValue("@CodEstado", ddl_estado.SelectedValue);

                    myCommand2.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                    myCommand2.CommandText = "NumiCollectionAdd"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                    myCommand2.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                    myCon.Open(); //Abrir a conexão
                    myCommand2.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                    myCon.Close();

                    Session["AddClicked"] = false;

                    Session["ModalOpened"] = false;

                    if (AnswUserHasCollection == 0)
                    {
                        lbl_message.Text = "Criada a sua coleção! Continue a adicionar items!";
                    }
                }
            }

            if (e.CommandName.Equals("dislike"))
            {

                SqlCommand myCommand2 = new SqlCommand(); //Novo commando SQL 
                myCommand2.Parameters.AddWithValue("@CodMN", e.CommandArgument);
                myCommand2.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);
                myCommand2.Parameters.AddWithValue("@CodCollection", AnswCodCollection);
                SqlParameter UserDoesnHaveCoin = new SqlParameter();
                UserDoesnHaveCoin.ParameterName = "@UserDoesntHaveCoin";
                UserDoesnHaveCoin.Direction = ParameterDirection.Output;
                UserDoesnHaveCoin.SqlDbType = SqlDbType.Int;

                myCommand2.Parameters.Add(UserDoesnHaveCoin);

                myCommand2.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand2.CommandText = "NumiCollectionRemove"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand2.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand2.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                myCon.Close();

                int AnswUserDoesntHaveCoin = Convert.ToInt32(myCommand2.Parameters["@UserDoesntHaveCoin"].Value);

                if (AnswUserDoesntHaveCoin == 1)
                {
                    string script = @"
                                    document.getElementById('messageAR').classList.remove('hidden');
                                    document.getElementById('messageAR').classList.add('removed');
                                    document.getElementById('dislike').style.color = 'black';
                                ";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateMessageAndLike", script, true);

                    lbl_message.Text = "Removido da sua coleção.";
                }
                else
                {
                    string script = @"
                                    document.getElementById('messageAR').classList.remove('hidden');
                                    document.getElementById('messageAR').classList.add('notcollected');
                                ";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateMessageAndLike", script, true);

                    lbl_message.Text = "Este artigo não faz parte da sua coleção!";
                }
            }
        }

        protected void btn_add_Click(object sender, EventArgs e)
        {
            Session["AddClicked"] = true;

            string script = @"
                                    document.getElementById('messageAR').classList.remove('hidden');
                                    document.getElementById('messageAR').classList.add('added');
                                    document.getElementById('like').style.color = 'red';
                                ";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateMessageAndLike", script, true);

            lbl_message.Text = "Adicionado à sua coleção.";
        }

    }
}

