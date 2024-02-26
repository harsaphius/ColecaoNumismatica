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
            string isAdmin, user, script;
            List<Money> LstMoney = new List<Money>();

            if (Master is Numismatic masterPage)
            {
                masterPage.BtnSearchInMasterPage += Master_BtnSearchInMasterPage;
            }

            if (Session["Logado"] == null || Session["Logado"].ToString() == "No")
            {
                script = @"
                            document.getElementById('navBarDropDown').classList.remove('hidden');
                            document.getElementById('btn_home').classList.remove('hidden');
                            document.getElementById('btn_login').classList.remove('hidden');   
                            document.getElementById('searchbar').classList.add('d-flex');
                            document.getElementById('searchbar').classList.remove('hidden');";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);
                LoadData();
                BindData();
            }
            else if (Session["Logado"] != null && Session["Logado"].ToString() == "Yes" || Page.IsPostBack == true)
            {
                isAdmin = Session["Admin"].ToString();
                user = Session["User"].ToString();
                Session["LstMoney"] = LstMoney;
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
                LoadData();
            }
             

        }

        public int PageNumberCount
        {
            get
            {
                if (ViewState["PageNumber"] != null) return Convert.ToInt32(ViewState["PageNumber"]);
                else return 0;
            }
            set
            {
                ViewState["PageNumber"] = value;
            }
        }
        protected void rpt_mainpage_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (Session["Logado"] == null)
            {
                Response.Redirect("NumiLoginUser.aspx");
            }
            else {
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

            btn_add.Visible = true;

            if (e.CommandName.Equals("like"))
            {
                string modalScript = @"$('#exampleModal').modal('show');";

                //Register script to show the modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "exampleModalScript", modalScript, true);
                Session["ModalCommand"] = e.CommandName;
                Session["ItemID"] = Convert.ToInt32(e.CommandArgument);
                }
            }
        }
        protected void btn_add_Click(object sender, EventArgs e)
        {
            int itemID = Convert.ToInt32(Session["ItemID"]);
            List<object> List = RetrieveValorAtual(itemID);
            
            if (Session["ModalCommand"] != null && Session["ModalCommand"].ToString() == "like" && Session["Logado"].ToString() == "Yes")
            {
                if (tb_quantidade.Text.Length == 0 || Convert.ToInt32(tb_quantidade.Text) < 1)
                {
                    lbl_quantidade.Text = "A quantidade não pode ser inferior a 1!";
                    lbl_quantidade.CssClass = "removed";

                    string modalScript = @"$('#exampleModal').modal('show');";

                    //Register script to show the modal
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "exampleModalScript", modalScript, true);
                }
                else
                {
                    SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                    SqlCommand myCommand2 = new SqlCommand(); //Novo commando SQL 
                    myCommand2.Parameters.AddWithValue("@CodUtilizador", Convert.ToInt32(Session["CodUtilizador"]));
                    myCommand2.Parameters.AddWithValue("@CodMN", itemID);
                    myCommand2.Parameters.AddWithValue("@CodCollection", Convert.ToInt32(Session["AnswCodCollection"]));
                    myCommand2.Parameters.AddWithValue("@CodEstado", List[1]);
                    myCommand2.Parameters.AddWithValue("@Quantidade", tb_quantidade.Text);

                    myCommand2.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                    myCommand2.CommandText = "NumiCollectionAdd"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                    myCommand2.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                    myCon.Open(); //Abrir a conexão
                    myCommand2.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                    myCon.Close();


                    string script = @"
                                var itemID = '" + Session["ItemID"].ToString() + @"';
                                document.getElementById('messageAR').classList.remove('hidden');
                                document.getElementById('messageAR').classList.add('added');
                        

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
        }
        protected void BindData()
        {
            List<Money> LstMoney = Session["LstMoney"] as List<Money>;

            PagedDataSource pagedDataSource = new PagedDataSource();
            pagedDataSource.DataSource = LstMoney;
            pagedDataSource.AllowPaging = true;
            pagedDataSource.PageSize = 8;
            pagedDataSource.CurrentPageIndex = PageNumberCount;
            int PageNumber = PageNumberCount + 1;
            lbl_pageNumber.Text = (PageNumber).ToString();

            rpt_mainpage.DataSource = pagedDataSource;
            rpt_mainpage.DataBind();

            lbtn_previous.Enabled = !pagedDataSource.IsFirstPage;
            lbtn_next.Enabled = !pagedDataSource.IsLastPage;
        }
        protected void LoadData()
        {
            string query;
            List<Money> LstMoney = new List<Money>();
            if (!Page.IsPostBack)
            {
                ddl_preco.Items.Insert(0, new ListItem("Todos", "0"));
                ddl_tipo.Items.Insert(0, new ListItem("Todos", "0"));
                query = "SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.Estado, NCSMN.ValorAtual, NCS.CodEstado, NCI.Imagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI GROUP BY NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem, NCM.CodTipoMN, NCS.Estado,NCS.CodEstado, NCSMN.ValorAtual;";
                LstMoney = Classes.MyFunctions.Listar(query);
                Session["LstMoney"] = LstMoney;
                BindData();
            }
            else if (ddl_preco.SelectedIndex == 0 && ddl_tipo.SelectedIndex == 0)
            {
                query = "SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.Estado,NCSMN.ValorAtual,NCS.CodEstado, NCI.Imagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI GROUP BY NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem, NCM.CodTipoMN, NCS.Estado, NCS.CodEstado, NCSMN.ValorAtual;";
                LstMoney = Classes.MyFunctions.Listar(query);
                Session["LstMoney"] = LstMoney;
                BindData();
            }
            else if (ddl_preco.SelectedIndex == 1 && ddl_tipo.SelectedIndex == 0)
            {
                query = "SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCSMN.ValorAtual, NCS.Estado, NCS.CodEstado, NCI.Imagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI GROUP BY NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem, NCM.CodTipoMN, NCSMN.ValorAtual, NCS.CodEstado, NCS.Estado ORDER BY NCSMN.ValorAtual ASC;";
                LstMoney = Classes.MyFunctions.Listar(query);
                Session["LstMoney"] = LstMoney;
                BindData();
            }
            else if (ddl_preco.SelectedIndex == 2 && ddl_tipo.SelectedIndex == 0)
            {
                query = "SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.Estado, NCS.CodEstado, NCSMN.ValorAtual, NCI.Imagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI GROUP BY NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem, NCM.CodTipoMN, NCSMN.ValorAtual,NCS.CodEstado, NCS.Estado ORDER BY NCSMN.ValorAtual DESC;";
                LstMoney = Classes.MyFunctions.Listar(query);
                Session["LstMoney"] = LstMoney;
                BindData();
            }
            else if (ddl_preco.SelectedIndex == 0 && ddl_tipo.SelectedIndex != 0)
            {
                query = $"SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.Estado,NCSMN.ValorAtual,NCS.CodEstado, NCI.Imagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI WHERE NCM.CodTipoMN = {ddl_tipo.SelectedValue} GROUP BY NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem, NCM.CodTipoMN, NCSMN.ValorAtual,NCS.CodEstado ,NCS.Estado;";
                LstMoney = Classes.MyFunctions.Listar(query);
                Session["LstMoney"] = LstMoney;
                BindData();
            }
            else if (ddl_preco.SelectedIndex == 1 && ddl_tipo.SelectedIndex != 0)
            {
                query = $"SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.Estado, NCS.CodEstado, NCSMN.ValorAtual, NCI.Imagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI WHERE NCM.CodTipoMN = {ddl_tipo.SelectedValue} GROUP BY NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem, NCM.CodTipoMN, NCSMN.ValorAtual,NCS.CodEstado, NCS.Estado ORDER BY NCSMN.ValorAtual ASC;";

                LstMoney = Classes.MyFunctions.Listar(query);
                Session["LstMoney"] = LstMoney;
                BindData();

            }
            else if (ddl_preco.SelectedIndex == 2 && ddl_tipo.SelectedIndex != 0)
            {
                query = $"SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.Estado,NCS.CodEstado,NCSMN.ValorAtual, NCI.Imagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI WHERE NCM.CodTipoMN = {ddl_tipo.SelectedValue} GROUP BY NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem, NCM.CodTipoMN, NCSMN.ValorAtual,NCS.CodEstado, NCS.Estado ORDER BY NCSMN.ValorAtual DESC;";

                LstMoney = Classes.MyFunctions.Listar(query);
                Session["LstMoney"] = LstMoney;
                BindData();
            }
        }
        protected void lbtn_previous_Click(object sender, EventArgs e)
        {
            PageNumberCount -= 1;
            BindData();
        }
        protected void lbtn_next_Click(object sender, EventArgs e)
        {
            PageNumberCount += 1;
            BindData();
        }
        protected void Master_BtnSearchInMasterPage(object sender, EventArgs e)
        {
            List<Money> LstMoney;
            if (Master is Numismatic masterPage)
            {
                TextBox textBox = masterPage.FindControl("tb_search") as TextBox;
                if (textBox != null)
                {
                    string query = $"SELECT NCM.CodMN, NCM.Titulo,NCS.CodEstado, NCM.ValorCunho,NCS.Estado,NCSMN.ValorAtual, NCI.Imagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI WHERE NCM.Titulo LIKE '%{textBox.Text}%' GROUP BY NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem,NCS.CodEstado, NCM.CodTipoMN,NCSMN.ValorAtual,NCS.Estado";
                    LstMoney = Classes.MyFunctions.Listar(query);
                    Session["LstMoney"] = LstMoney;
                    Session["Pesquisa"] = "Yes";
                    BindData();
                }
            }
  
        }
        protected List<object> RetrieveValorAtual(int itemID)
        {
            List<object> ListO = new List<object>();

            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados
            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@CodMN", itemID);

            SqlParameter ValorAtual = new SqlParameter();
            ValorAtual.ParameterName = "@ValorAtual";
            ValorAtual.Direction = ParameterDirection.Output;
            ValorAtual.SqlDbType = SqlDbType.Decimal;
            ValorAtual.Precision = 8;
            ValorAtual.Scale = 2;

            myCommand.Parameters.Add(ValorAtual);

            SqlParameter CodEstado = new SqlParameter();
            CodEstado.ParameterName = "@CodEstado";
            CodEstado.Direction = ParameterDirection.Output;
            CodEstado.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(CodEstado);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiCoinValorAtual"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            myCon.Close();

            decimal AnswValorAtual = Convert.ToDecimal(myCommand.Parameters["@ValorAtual"].Value);
            ListO.Add(AnswValorAtual);
            int AnswCodEstado = Convert.ToInt32(myCommand.Parameters["@CodEstado"].Value);
            ListO.Add(AnswCodEstado);

            return ListO;
        }
    }
}


