using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class LoginUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRecoverPasswordFE_Click(object sender, EventArgs e)
        {
            //Recuperação de password com envio de email
            
        }

        protected void btnLoginBE_Click(object sender, EventArgs e)
        {
            Response.Redirect("MainPageFE.aspx");
        }

        protected void btn_facebook_Click(object sender, EventArgs e)
        {

        }

        protected void btn_google_Click(object sender, EventArgs e)
        {
            string clientid = ConfigurationManager.AppSettings["clientid"];
            string redirectionURL = ConfigurationManager.AppSettings["redirection_url"];

            string url = "https://accounts.google.com/o/oauth2/v2/auth?scope=profile&include_granted_scopes=true&redirect_uri=" + redirectionURL + "&response_type=code&client_id=" + clientid + "";
            Response.Redirect(url);
        }

        public void SendEmail()
        {
            string novaPasse = Membership.GeneratePassword(8, 3);

            //Tratamento de exceção.... Try_Catch
            //Try_Catch para a messagem para prevenir que a mensagem está correta
            try
            {
                SmtpClient servidor = new SmtpClient();//Servidor SMTP - pass, user, porto

                MailMessage emails = new MailMessage(); //Email
                emails.Subject = "E-mail de recuperação"; //Nome de quem é o email (tb_mail)
                emails.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_MailUser"]); //Sender
                emails.To.Add(new MailAddress(tb_email.Text)); //Destinatário

                emails.Body = $"Ex.mo(s) Sr.(s), A sua nova palavra-passe para o e-mail {tb_email.Text} é {novaPasse}";
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
    }
}