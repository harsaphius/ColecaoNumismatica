using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                            document.getElementById('btn_logout').classList.remove('hidden');
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
            }

            List<Money> LstMoney = new List<Money>();

            string query = $"SELECT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCS.ValorAtual, NCI.Imagem, NCS.CodEstado, NCM.CodTipoMN, NCC.Quantidade FROM NumiCoinMoney AS NCM OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN LEFT JOIN NumiCoinCollection AS NCC ON NCM.CodMN=NCC.CodMN WHERE CodUtilizador={Session["CodUtilizador"]};";

            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);
            SqlCommand myCommand = new SqlCommand(query, myCon);
            myCon.Open();

            SqlDataReader dr = myCommand.ExecuteReader();

            while (dr.Read())
            {
                Money record = new Money();
                record.cod = Convert.ToInt32(dr["CodMN"]);
                record.titulo = dr["Titulo"].ToString();
                record.imagem = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dr["Imagem"]);
                record.valorAtual = Convert.ToDecimal(dr["ValorAtual"]);
                record.estado = dr["CodEstado"].ToString();
                record.quantidade = Convert.ToInt32(dr["Quantidade"]);
                LstMoney.Add(record);
            }

            myCon.Close();

            rpt_mycollection.DataSource = LstMoney;
            rpt_mycollection.DataBind();
        }

        protected void btn_export_Click(object sender, EventArgs e)
        {
            string pathPDFs = ConfigurationManager.AppSettings["PathPDFs"];  //Caminho dos PDFs colocado no WebConfig de modo a ser facilmente acessado e modificado

            string siteURL = ConfigurationManager.AppSettings["SiteURL"]; //Caminho do URL colocado no WebConfig de modo a ser facilmente acessado e modificado

            string pdfTemplate = pathPDFs + "template\\cartao_template.pdf"; //Caminho final do template

            string nomePDF = Classes.MyFunctions.EncryptString(DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "")) + ".pdf"; //Gera o nome do pdf através da encriptação da data e hora do dia em que o pdf foi criado - encriptação MD5

            string novoFile = pathPDFs + "gerados\\" + nomePDF;

            PdfReader pdfReader = new PdfReader(pdfTemplate); //Instancia pdfReader para ler o pdfTemplate
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(novoFile, FileMode.Create)); //Instancia o pdfStamper para ir buscar o ficheiro pdfTemplate de modo a que o AcroFields (a seguir) possa preencher os campos assinalados num novoFile

            AcroFields pdfFields = pdfStamper.AcroFields; //Encontra os AcroFields no pdfStamper
            pdfFields.SetField("tb_name", tb_nome.Text); //Escreve no novoFile no campo do pdf nome o texto da tb_nome
            pdfFields.SetField("tb_nr", myCommand.Parameters["@Nr"].Value.ToString());

            pdfStamper.Close(); //Fecha o pdfStamper
        }
    }
}
