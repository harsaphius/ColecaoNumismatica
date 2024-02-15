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
            string query = "";

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
                            document.getElementById('logoutbutton').classList.remove('hidden');
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

                if (!Page.IsPostBack)
                {
                    ddl_preco.Items.Insert(0, new ListItem("Todos", "0"));
                    ddl_tipo.Items.Insert(0, new ListItem("Todos", "0"));
                }

                if (ddl_preco.SelectedIndex == 0 && ddl_tipo.SelectedIndex == 0)
                {
                    query = "SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.ValorAtual, NCI.Imagem FROM NumiCoinMoney AS NCM OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN; ";
                    Listar(query);
                }
                else if (ddl_preco.SelectedIndex == 1 && ddl_tipo.SelectedIndex == 0)
                {
                    query = "SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.ValorAtual, NCI.Imagem FROM NumiCoinMoney AS NCM OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN ORDER BY NCS.ValorAtual ASC;";
                    Listar(query);
                }
                else if (ddl_preco.SelectedIndex == 2 && ddl_tipo.SelectedIndex == 0)
                {
                    query = "SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.ValorAtual, NCI.Imagem FROM NumiCoinMoney AS NCM OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN ORDER BY NCS.ValorAtual DESC;";
                    Listar(query);
                }
                else if (ddl_preco.SelectedIndex == 0 && ddl_tipo.SelectedIndex != 0)
                {
                    query = $"SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.ValorAtual, NCI.Imagem FROM NumiCoinMoney AS NCM OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN WHERE CodTipoMN={ddl_tipo.SelectedValue};";
                    Listar(query);
                }
                else if (ddl_preco.SelectedIndex == 1 && ddl_tipo.SelectedIndex != 0)
                {
                    query = $"SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.ValorAtual, NCI.Imagem FROM NumiCoinMoney AS NCM OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN WHERE CodTipoMN={ddl_tipo.SelectedValue} ORDER BY NCS.ValorAtual ASC;";
                    Listar(query);
                }
                else if (ddl_preco.SelectedIndex == 2 && ddl_tipo.SelectedIndex != 0)
                {
                    query = $"SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.ValorAtual, NCI.Imagem FROM NumiCoinMoney AS NCM OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN WHERE CodTipoMN={ddl_tipo.SelectedValue} ORDER BY NCS.ValorAtual DESC;";
                    Listar(query);
                }


            }
        }

        protected void rpt_mainpage_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
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

            Session["AnswCodCollection"] = AnswCodCollection;
            Session["AnswUserHasCollection"] = AnswUserHasCollection;

            if (e.CommandName.Equals("like") || e.CommandName.Equals("dislike"))
            {
                string modalScript = @"$('#exampleModal').modal('show');";

                //Register script to show the modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "exampleModalScript", modalScript, true);
                Session["ModalCommand"] = e.CommandName;
                Session["ItemID"] = e.CommandArgument;
            }
        }

        protected void Listar(string query)
        {
            List<Money> LstMoney = new List<Money>();

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
                record.valorAtual = Convert.ToDecimal(dr["ValorAtual"]);
                record.imagem = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dr["Imagem"]);
                LstMoney.Add(record);
            }

            myCon.Close();
            rpt_mainpage.DataSource = LstMoney;
            rpt_mainpage.DataBind();
        }

        protected void btn_add_Click(object sender, EventArgs e)
        {
            if (Session["ModalCommand"] != null && Session["ModalCommand"].ToString() == "like")
            {
                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                int itemID = Convert.ToInt32(Session["ItemID"]);

                SqlCommand myCommand2 = new SqlCommand(); //Novo commando SQL 
                myCommand2.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);
                myCommand2.Parameters.AddWithValue("@CodMN", itemID);
                myCommand2.Parameters.AddWithValue("@CodCollection", Session["AnswCodCollection"]);
                myCommand2.Parameters.AddWithValue("@CodEstado", ddl_estado.SelectedValue);
                myCommand2.Parameters.AddWithValue("@Quantidade", tb_quantidade.Text);

                myCommand2.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand2.CommandText = "NumiCollectionAdd"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand2.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand2.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                myCon.Close();


                //var linkButton = rpt_mainpage.Items.Cast<RepeaterItem>()
                //                             .Where(item => item.FindControl("ltbn_like") != null && (item.FindControl("ltbn_like") as LinkButton).CommandArgument.ToString() == itemID.ToString())
                //                             .Select(item => item.FindControl("ltbn_like") as LinkButton)
                //                             .FirstOrDefault();
                //if (linkButton != null)
                //{
                //    linkButton.ForeColor = System.Drawing.Color.Red; // Change color to red
                //}

                string script = @"
                                var itemID = '" + Session["ItemID"].ToString() + @"';
                                document.getElementById('messageAR').classList.remove('hidden');
                                document.getElementById('messageAR').classList.add('removed');

                                var like = document.getElementById('like_' + itemID);
                                if (like) {
                                    like.style.color = 'red';
                                }
                            ";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateMessageAndLike_" + Session["ItemID"].ToString(), script, true);

                if (Session["AnswUserHasCollection"] != null)
                {
                    if (Convert.ToInt32(Session["AnswUserHasCollection"]) == 1)
                        lbl_message.Text = "Adicionado à sua coleção.";
                    if (Convert.ToInt32(Session["AnswUserHasCollection"]) == 0)
                        lbl_message.Text = $"Coleção criada!<br /> Item adicionado à sua coleção. Continue a adicionar mais items.";
                }
            }
        }
        protected void btn_remove_Click(object sender, EventArgs e)
        {
            if (Session["ModalCommand"] != null && Session["ModalCommand"].ToString() == "dislike")
            {
                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados
                int itemID = Convert.ToInt32(Session["ItemID"]);

                SqlCommand myCommand2 = new SqlCommand(); //Novo commando SQL 
                myCommand2.Parameters.AddWithValue("@CodMN", itemID);
                myCommand2.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);
                myCommand2.Parameters.AddWithValue("@CodCollection", Session["AnswCodCollection"]);
                myCommand2.Parameters.AddWithValue("@CodEstado", ddl_estado.SelectedValue);
                myCommand2.Parameters.AddWithValue("@Quantidade", tb_quantidade.Text);

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
                else if (AnswUserDoesntHaveCoin == -2)
                {
                    string script = @"
                                    document.getElementById('messageAR').classList.remove('hidden');
                                    document.getElementById('messageAR').classList.add('notcollected');
                                ";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateMessageAndLike", script, true);

                    lbl_message.Text = "Quantidade a remover superior à da coleção!";
                }
                else if (AnswUserDoesntHaveCoin == -1)
                {
                    string script = @"
                                    document.getElementById('messageAR').classList.remove('hidden');
                                    document.getElementById('messageAR').classList.add('notcollected');
                                ";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateMessageAndLike", script, true);

                    lbl_message.Text = "Este artigo não faz parte da sua coleção!";
                }
                else if (AnswUserDoesntHaveCoin == 0)
                {
                    string script = @"
                                    document.getElementById('messageAR').classList.remove('hidden');
                                    document.getElementById('messageAR').classList.add('notcollected');
                                ";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "UpdateMessageAndLike", script, true);

                    lbl_message.Text = "Este artigo não faz parte da sua coleção, com essas características!";
                }
            }
        }
    }

}


