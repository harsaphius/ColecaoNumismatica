using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiMyCollection : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string script, isAdmin, user;
            List<Money> LstMoney = new List<Money>();

            if (Session["Logado"] == null)
            {
                Response.Redirect("NumiMainPage.aspx");
            }
            else if (Session["Logado"].ToString() == "Yes" && !Page.IsPostBack == true)
            {
                isAdmin = Session["Admin"].ToString();
                user = Session["User"].ToString();

                int collection = HasCollection(Convert.ToInt32(Session["CodUtilizador"]));

                if (collection == 0)
                {
                    lbl_message.Text = "Ainda não criaste uma coleção! :) Cria já! Vai à nossa Main Page e começa a adicionar! <3!";
                    lbl_message.CssClass = "added";
                    btn_export.Visible = false;
                }
                else
                {
                    lbl_message.Text = "";
                    btn_export.Visible = true;
                }

                Label lblMessage = Master.FindControl("lbl_message") as Label;
                if (lblMessage != null)
                {
                    lblMessage.Text = "Bem-vindo " + user;
                }

                script = @"
                            document.getElementById('navBarDropDown').classList.remove('hidden');
                            document.getElementById('btn_home').classList.remove('hidden');
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
                             document.getElementById('btn_manageUsers').classList.remove('hidden');
                             document.getElementById('btn_statistics').classList.remove('hidden');
                             document.getElementById('btn_registerNewUser').classList.remove('hidden');";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAdminButtons", script, true);
                }
                LstMoney = MyCollection();
                Session["LstMoney"] = LstMoney;
                BindData();

            }
            else if (Page.IsPostBack == true)
            {
                isAdmin = Session["Admin"].ToString();
                user = Session["User"].ToString();

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
        protected void rpt_mycollection_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.Equals("lbtn_plus"))
            {
                List<Money> LstMoney;
                List<object> List = RetrieveValorAtual(Convert.ToInt32(e.CommandArgument));

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);
                myCommand.Parameters.AddWithValue("@CodMN", e.CommandArgument);
                myCommand.Parameters.AddWithValue("@CodEstado", List[1]);
                myCommand.Parameters.AddWithValue("@Quantidade", 1);

                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiCollectionMore"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                myCon.Close();

                LstMoney = MyCollection();

                rpt_mycollection.DataSource = LstMoney;
                rpt_mycollection.DataBind();
            }

            if (e.CommandName.Equals("lbtn_minus"))
            {
                List<object> List = RetrieveValorAtual(Convert.ToInt32(e.CommandArgument));
                List<Money> LstMoney;

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);
                myCommand.Parameters.AddWithValue("@CodMN", e.CommandArgument);
                myCommand.Parameters.AddWithValue("@CodEstado", List[1]);
                myCommand.Parameters.AddWithValue("@Quantidade", 1);

                SqlParameter UserDoesnHaveCoin = new SqlParameter();
                UserDoesnHaveCoin.ParameterName = "@UserDoesntHaveCoin";
                UserDoesnHaveCoin.Direction = ParameterDirection.Output;
                UserDoesnHaveCoin.SqlDbType = SqlDbType.Int;

                myCommand.Parameters.Add(UserDoesnHaveCoin);

                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiCollectionLess"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                myCon.Close();

                LstMoney = MyCollection();

                rpt_mycollection.DataSource = LstMoney;
                rpt_mycollection.DataBind();

            }

            if (e.CommandName.Equals("lbtn_remove"))
            {
                List<Money> LstMoney;
                List<object> List = RetrieveValorAtual(Convert.ToInt32(e.CommandArgument));

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);
                myCommand.Parameters.AddWithValue("@CodMN", e.CommandArgument);

                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiCoinRemoveFromCollection"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                myCon.Close();

                LstMoney = MyCollection();

                rpt_mycollection.DataSource = LstMoney;
                rpt_mycollection.DataBind();
            }
        }
        protected List<Money> MyCollection()
        {
            List<Money> LstMoney = new List<Money>();

            string query = $"SELECT NCC.CodMN, NCC.CodCollection, NCI.Imagem, NCS.Estado, NCM.Titulo,  NCM.ValorCunho, NCSMN.ValorAtual, NCMT.Tipo, NCC.Quantidade FROM NumiCoinCollection AS NCC LEFT JOIN  NumiCoinMoney AS NCM ON NCC.CodMN = NCM.CodMN INNER JOIN NumiCoinMNType AS NCMT ON NCM.CodTipoMN = NCMT.CodTipoMN INNER JOIN NumiCoinState AS NCS ON NCC.CodEstado=NCS.CodEstado LEFT JOIN NumiCoinStateMN AS NCSMN ON NCC.CodMN=NCSMN.CodMN OUTER APPLY (SELECT TOP 1 Imagem FROM NumiCoinMNImage WHERE CodMN = NCC.CodMN ORDER BY CodImagem) AS NCI WHERE NCC.CodUtilizador={Session["CodUtilizador"]};";

            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);
            SqlCommand myCommand = new SqlCommand(query, myCon);
            myCon.Open();

            SqlDataReader dr = myCommand.ExecuteReader();

            while (dr.Read())
            {
                Money record = new Money();
                record.cod = Convert.ToInt32(dr["CodMN"]);
                record.codC = Convert.ToInt32(dr["CodCollection"]);
                record.titulo = dr["Titulo"].ToString();
                record.estado = dr["Estado"].ToString();
                record.tipo = dr["Tipo"].ToString();
                record.valorCunho = dr["ValorCunho"].ToString();
                record.valorAtual = Convert.ToDecimal(dr["ValorAtual"]);
                record.quantidade = Convert.ToInt32(dr["Quantidade"]);
                record.imagem = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dr["Imagem"]);
                record.imagemC = Convert.ToBase64String((byte[])dr["Imagem"]);

                LstMoney.Add(record);
            }

            myCon.Close();

            return LstMoney;

        }
        protected int HasCollection(int user)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@CodUtilizador", Session["CodUtilizador"]);

            SqlParameter HasCollection = new SqlParameter();
            HasCollection.ParameterName = "@HasCollection";
            HasCollection.Direction = ParameterDirection.Output;
            HasCollection.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(HasCollection);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiCoinHasCollection"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            myCon.Close();

            int AnswHasCollection = Convert.ToInt32(myCommand.Parameters["@HasCollection"].Value);

            return AnswHasCollection;
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
        protected void BindData()
        {
            List<Money> LstMoney = Session["LstMoney"] as List<Money>;

            PagedDataSource pagedDataSource = new PagedDataSource();
            pagedDataSource.DataSource = LstMoney;
            pagedDataSource.AllowPaging = true;
            pagedDataSource.PageSize = 6;
            pagedDataSource.CurrentPageIndex = PageNumberCount;
            int PageNumber = PageNumberCount + 1;
            lbl_pageNumber.Text = (PageNumber).ToString();

            rpt_mycollection.DataSource = pagedDataSource;
            rpt_mycollection.DataBind();

            lbtn_previous.Enabled = !pagedDataSource.IsFirstPage;
            lbtn_next.Enabled = !pagedDataSource.IsLastPage;
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

        /// <summary>
        /// Função para criação e exportação do pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_export_Click(object sender, EventArgs e)
        {
            List<Money> money;

            if (Session["User"] != null)
            {
                money = MyCollection();
                List<string> pdfFiles = new List<string>();
                string pathPDFs = ConfigurationManager.AppSettings["PathPDFs"];  //Caminho dos PDFs colocado no WebConfig de modo a ser facilmente acessado e modificado

                string pdfTemplate = pathPDFs + "Template\\NumiCoinPdf_Template.pdf"; //Caminho final do template
                string pathTemps = pathPDFs + $"Temps\\";
                //string pathFinal = pathPDFs + $"Gerados\\Collection_{Session["User"]}";

                int num = 10;
                int pageNumber = 1;

                foreach (Money m in money)
                {
                    string nomePDF = Classes.MyFunctions.EncryptString(num.ToString()) + ".pdf"; //Gera o nome do pdf através da encriptação da data e hora do dia em que o pdf foi criado - encriptação MD5

                    string novoFile = pathPDFs + "Temps\\" + nomePDF;

                    PdfReader pdfReader = new PdfReader(pdfTemplate); //Instancia pdfReader para ler o pdfTemplate
                    PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(novoFile, FileMode.Create)); //Instancia o pdfStamper para ir buscar o ficheiro pdfTemplate de modo a que o AcroFields (a seguir) possa preencher os campos assinalados num novoFile

                    AcroFields pdfFields = pdfStamper.AcroFields; //Encontra os AcroFields no pdfStamper
                    pdfFields.SetField("tb_user", Session["User"].ToString()); //Escreve no novoFile no campo do pdf nome o texto da tb_nome
                    pdfFields.SetField("tb_codMN", m.cod.ToString());
                    pdfFields.SetField("tb_collection", m.codC.ToString());
                    pdfFields.SetField("tb_titulo", m.titulo);
                    pdfFields.SetField("tb_tipo", m.tipo);
                    pdfFields.SetField("tb_estado", m.estado);
                    pdfFields.SetField("tb_valorCunho", m.valorCunho.ToString());
                    pdfFields.SetField("tb_valorAtual", m.valorAtual.ToString());
                    pdfFields.SetField("tb_quantidade", m.quantidade.ToString());
                    pdfFields.SetField("pageNumber", pageNumber.ToString());
                    byte[] imageData = Convert.FromBase64String(m.imagemC);

                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageData);

                    // Scale the image if necessary
                    image.ScaleToFit(100f, 100f); // Adjust width and height as needed

                    image.SetAbsolutePosition(100, 250);
                    // Add the image to the document
                    PdfContentByte pdfContentByte = pdfStamper.GetOverContent(1); // Page number to add the image to
                    pdfContentByte.AddImage(image);

                    pdfStamper.Close(); //Fecha o pdfStamper
                    pdfFiles.Add(novoFile);
                    num++;
                    pageNumber++;
                }

                byte[] mergedPdf = MergePdfFilesInFolder(pathTemps);

                //Ficheiro guardado na pasta Gerados com o nome do Cliente
                string outputPath = Path.Combine(Server.MapPath("~"), "Pdfs\\Gerados", $"{Session["User"]}.pdf");
                File.WriteAllBytes(outputPath, mergedPdf);

                string[] tempPdfFiles = Directory.GetFiles(pathTemps, "*.pdf");
                foreach (string tempPdfFile in tempPdfFiles)
                {
                    File.Delete(tempPdfFile);
                }

                //Download do ficheiro para o cliente
                if (mergedPdf != null)
                {
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", $"attachment; filename={Session["User"]}.pdf");
                    Response.BinaryWrite(mergedPdf);
                    Response.End();
                }

            }
        }

        /// <summary>
        /// Função para merge dos pdfs com template
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public byte[] MergePdfFilesInFolder(string folderPath)
        {
            //Lista de pdf da pasta folderPath
            string[] pdfFiles = Directory.GetFiles(folderPath, "*.pdf");

            // Create a MemoryStream to hold the merged PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Create a Document and PdfCopy instance
                using (Document document = new Document())
                using (PdfSmartCopy copy = new PdfSmartCopy(document, ms))
                {
                    document.Open();

                    foreach (string pdfFile in pdfFiles)
                    {
                        // Add each PDF file to the merged PDF
                        using (PdfReader reader = new PdfReader(pdfFile))
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                copy.AddPage(copy.GetImportedPage(reader, i));
                            }
                        }
                    }
                }

                // Return the merged PDF as a byte array
                return ms.ToArray();
            }
        }
    }
}

