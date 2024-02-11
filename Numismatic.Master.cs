using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class Numismatic : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_logout_Click(object sender, EventArgs e)
        {
            Response.Redirect("NumiLoginUser.aspx");
        }

        protected void btn_home_Click(object sender, EventArgs e)
        {
            Response.Redirect("NumiMainPage.aspx");
        }

        protected void novoAdmin_Click(object sender, EventArgs e)
        {
            // Tratamento de exceção....Try_Catch
            //Try_Catch para a messagem para prevenir que a mensagem está correta
            try
            {
                SmtpClient servidor = new SmtpClient();//Servidor SMTP - pass, user, porto

                MailMessage emails = new MailMessage(); //Email
                emails.Subject = "Ativação da conta"; //Nome de quem é o email (tb_mail)
                emails.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_MailUser"]); //Sender
                emails.To.Add(new MailAddress("atuaprincesapatty@gmail.com")); //Destinatário

                emails.Body = $"O utilizador {Session["User"]} deseja contribuir para a melhoria do website! Para conceder permissões de administrador, por favor, aceda à área de manutenção de utilizadores!";
                emails.IsBodyHtml = true; //Reconhece a formatação do HTML, tags, p.e., links,<b>,<p>,etc. Poderemos usar plain-text IsBodyHtml= false.

                servidor.Host = ConfigurationManager.AppSettings["SMTP_URL"]; //SMTP URL configurada no WebConfig
                servidor.Port = int.Parse(ConfigurationManager.AppSettings["SMTP_Port"]); //SMTP Port configurada no WebConfig

                string user = ConfigurationManager.AppSettings["SMTP_MailUser"]; //SMTP User Mail configurado no WebConfig
                string pw = ConfigurationManager.AppSettings["SMTP_Pass"]; //SMTP Pass Mail configurado no WebConfig

                servidor.Credentials = new NetworkCredential(user, pw); //Indicar as credenciais do utilizador

                servidor.EnableSsl = true; //Habilitar o SSL - o SMTP Client usa o SSL para criptografar a ligação

                servidor.Send(emails); //O objeto servidor envia o mail

            }
            catch (Exception ex)
            {
                lbl_message.Text = ex.Message; //Mensagem de insucesso conforme a exceção encontrada.
            }

        }

        protected void btn_insertNewCoin_Click(object sender, EventArgs e)
        {
            Response.Redirect("NumiInsertNewCoin.aspx");
        }

        protected void btn_manageCoins_Click(object sender, EventArgs e)
        {
            Response.Redirect("NumiManageCoins.aspx");
        }

        protected void btn_manageUsers_Click(object sender, EventArgs e)
        {
            Response.Redirect("NumiManageUsers.aspx");
        }

        protected void btn_search_Click(object sender, EventArgs e)
        {
            List<Money> LstMoney = new List<Money>();

            string query = $"SELECT NCM.Titulo, NCM.ValorCunho, NCI.Imagem FROM NumiCoinMoney AS NCM LEFT JOIN(SELECT NCI2.CodMN, NCI2.Imagem FROM(SELECT CodMN, MIN(CodImagem) AS FirstImage FROM NumiCoinMNImage GROUP BY CodMN) AS NCI JOIN NumiCoinMNImage AS NCI2 ON NCI.FirstImage = NCI2.CodImagem) AS NCI ON NCM.CodMN = NCI.CodMN WHERE NCM.Titulo LIKE '%{tb_search.Text}%'";

            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);

            SqlCommand myCommand = new SqlCommand(query, myCon);

            myCon.Open();

            SqlDataReader dr = myCommand.ExecuteReader();

            while (dr.Read())
            {
                Money record = new Money();
                record.titulo = dr["Titulo"].ToString();
                record.valorCunho = Convert.ToDecimal(dr["ValorCunho"]);
                record.imagem = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dr["Imagem"]);
                LstMoney.Add(record);
            }

            myCon.Close();
        }
    }
}