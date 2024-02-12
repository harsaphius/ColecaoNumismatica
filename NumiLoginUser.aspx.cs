using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;

namespace ColecaoNumismatica
{
    public partial class NumiLoginUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_emailpwrecover.Text))
            {
                rvf_email.Enabled = false;
            }

            //if (string.IsNullOrEmpty(tb_user.Text) && string.IsNullOrEmpty(tb_pw.Text))
            //{
            //    rfv_password.Enabled = false;
            //    rfv_tbuser.Enabled = false;
            //}

            if (Request.QueryString["redirected"] != null && Request.QueryString["redirected"] == "true")
            {
                lbl_ActivatedUser.Text = Session["ActivatedUser"].ToString();
            }
        }
        protected void btnRecoverPasswordFE_Click(object sender, EventArgs e)
        {
            //Recuperação de password com envio de email
            string novaPasse = Membership.GeneratePassword(8, 2);

            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@Email", tb_emailpwrecover.Text); //Adicionar o valor da tb_user ao parâmetro @nome
            myCommand.Parameters.AddWithValue("@PwNova", EncryptString(novaPasse));

            SqlParameter UserExist = new SqlParameter();
            UserExist.ParameterName = "@UserExist";
            UserExist.Direction = ParameterDirection.Output;
            UserExist.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(UserExist);

            SqlParameter AccountActive = new SqlParameter();
            AccountActive.ParameterName = "@AccountActive";
            AccountActive.Direction = ParameterDirection.Output;
            AccountActive.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(AccountActive);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiRecoverPw"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            int AnswUserExist = Convert.ToInt32(myCommand.Parameters["@UserExist"].Value);
            int AnswAccountAtive = Convert.ToInt32(myCommand.Parameters["@AccountActive"].Value);

            myCon.Close(); //Fechar a conexão

            if (AnswUserExist == 1 && AnswAccountAtive == 1)
            {
                //Tratamento de exceção.... Try_Catch
                //Try_Catch para a messagem para prevenir que a mensagem está correta
                try
                {
                    SmtpClient servidor = new SmtpClient();//Servidor SMTP - pass, user, porto

                    MailMessage emails = new MailMessage(); //Email
                    emails.Subject = "E-mail de recuperação"; //Nome de quem é o email (tb_mail)
                    emails.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_MailUser"]); //Sender
                    emails.To.Add(new MailAddress(tb_emailpwrecover.Text)); //Destinatário

                    emails.Body = $"Ex.mo(s) Sr.(s), A sua nova palavra-passe para o e-mail {tb_emailpwrecover.Text} é a seguinte: {novaPasse}. Proceda à sua alteração através do seguinte <a href='https://localhost:44399/NumiLoginUser.aspx?user={EncryptString(tb_user.Text)}'> link </a>!";
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
            else if (AnswAccountAtive == 0)
            {
                Session["ActivatedUser"] = "OK";
                lbl_message.Text = $"Este e-mail está registado, mas a conta não se encontra ativa! Foi enviado um e-mail para que proceda à ativação da sua conta!";

                //Tratamento de exceção.... Try_Catch
                //Try_Catch para a messagem para prevenir que a mensagem está correta
                try
                {
                    SmtpClient servidor = new SmtpClient();//Servidor SMTP - pass, user, porto

                    MailMessage emails = new MailMessage(); //Email
                    emails.Subject = "Ativação da conta"; //Nome de quem é o email (tb_mail)
                    emails.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_MailUser"]); //Sender
                    emails.To.Add(new MailAddress(tb_emailpwrecover.Text)); //Destinatário

                    emails.Body = $"<b>Obrigado pela sua inscrição. Para ativar a sua conta clique <a href='https://localhost:44399/NumiAtivationPage.aspx?user={EncryptString(tb_user.Text)}'> aqui</a>!";
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
            else
            {
                lbl_message.Text = "Este e-mail não está associado a nenhuma conta! Registe-se.";
            }
        }

        protected void btnLoginBE_Click(object sender, EventArgs e)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@User", tb_user.Text); //Adicionar o valor da tb_user ao parâmetro @nome
            myCommand.Parameters.AddWithValue("@Pw", EncryptString(tb_pw.Text));

            //Variável de Output para SP verificar se o utilizador e pw estão corretos
            SqlParameter UserExists = new SqlParameter();
            UserExists.ParameterName = "@UserExists";
            UserExists.Direction = ParameterDirection.Output;
            UserExists.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(UserExists);

            //Variável de Output para SP verificar o perfil
            SqlParameter AccountActive = new SqlParameter();
            AccountActive.ParameterName = "@AccountActive";
            AccountActive.Direction = ParameterDirection.Output;
            AccountActive.Direction = ParameterDirection.Output;
            AccountActive.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(AccountActive);

            //Variável de Output para SP verificar o perfil
            SqlParameter NumiAdmin = new SqlParameter();
            NumiAdmin.ParameterName = "@NumiAdmin";
            NumiAdmin.Direction = ParameterDirection.Output;
            NumiAdmin.Direction = ParameterDirection.Output;
            NumiAdmin.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(NumiAdmin);

            SqlParameter CodUtilizador = new SqlParameter();
            CodUtilizador.ParameterName = "@CodUtilizador";
            CodUtilizador.Direction = ParameterDirection.Output;
            CodUtilizador.Direction = ParameterDirection.Output;
            CodUtilizador.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(CodUtilizador);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiLogin"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            int AnswUserExist = Convert.ToInt32(myCommand.Parameters["@UserExists"].Value);
            int AnswAccountActive = Convert.ToInt32(myCommand.Parameters["@AccountActive"].Value);
            int AnswNumiAdmin = Convert.ToInt32(myCommand.Parameters["@NumiAdmin"].Value);
            int AnswNumiCodUser = Convert.ToInt32(myCommand.Parameters["@CodUtilizador"].Value);

            myCon.Close(); //Fechar a conexão

            if (AnswUserExist == 1)
            {
                if (chkBoxRemember.Checked)
                {
                    // Create a new authentication ticket
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                        1,                              // version
                        tb_user.Text,           // username
                        DateTime.Now,                   // issue time
                        DateTime.Now.AddMinutes(30),    // expiration time
                        false,                          // persistent
                        "user"                          // user data (optional)
                    );

                    // Encrypt the ticket
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                    // Create a new cookie and set its value to the encrypted ticket
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                    // Set cookie expiration
                    authCookie.Expires = authTicket.Expiration;

                    // Add the cookie to the response
                    Response.Cookies.Add(authCookie);
                }

                if (AnswNumiAdmin == 1) Session["Admin"] = "Yes";
                else Session["Admin"] = "No";

                Session["User"] = tb_user.Text;
                Session["CodUtilizador"] = AnswNumiCodUser;
                Session["Logado"] = "Yes";          
                Response.Redirect("NumiMainPage.aspx");
            }
            else if (AnswAccountActive == 2)
            {
                lbl_message.Text = $"A sua conta ainda não se encontra ativa! <a href='https://localhost:44399/NumiActivationPage.aspx?user={EncryptString(tb_user.Text)}'> Clique aqui para a ativar </a>!";
            }
            else
            {
                lbl_message.Text = "O seu utilizador ou palavra-passe estão errados! Tente novamente ou recupere a password!";
            }

        }

        /// <summary>
        /// Login a partir do Facebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_facebook_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Login a partir do Google
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_google_Click(object sender, EventArgs e)
        {
            string clientid = ConfigurationManager.AppSettings["clientid"];
            string redirectionURL = ConfigurationManager.AppSettings["redirection_url"];
            string url = "https://accounts.google.com/o/oauth2/v2/auth?scope=profile&include_granted_scopes=true&redirect_uri=" + redirectionURL + "&response_type=code&client_id=" + clientid + "";
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                       1,                              // version
                       clientid,                       // username
                       DateTime.Now,                   // issue time
                       DateTime.Now.AddMinutes(30),    // expiration time
                       false,                          // persistent
                       "user"                          // user data (optional)
                   );

            // Encrypt the ticket
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            // Create a new cookie and set its value to the encrypted ticket
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            // Set cookie expiration
            authCookie.Expires = authTicket.Expiration;

            // Add the cookie to the response
            Response.Cookies.Add(authCookie);

            Response.Redirect(url);
        }

        /// <summary>
        /// Função de Encriptação de Dados MD5
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string EncryptString(string Message)
        {
            string Passphrase = "Patrícia Rocha";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string

            string enc = Convert.ToBase64String(Results);
            enc = enc.Replace("+", "KKK");
            enc = enc.Replace("/", "JJJ");
            enc = enc.Replace("\\", "III");
            return enc;
        }

        /// <summary>
        /// Função de Desencritação de Dados MD5
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string DecryptString(string Message)
        {
            string Passphrase = "Patrícia Rocha";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]

            Message = Message.Replace("KKK", "+");
            Message = Message.Replace("JJJ", "/");
            Message = Message.Replace("III", "\\");


            byte[] DataToDecrypt = Convert.FromBase64String(Message);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(Results);
        }
    }
}