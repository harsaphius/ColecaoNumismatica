using ASPSnippets.FaceBookAPI;
using ColecaoNumismatica.Classes;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace ColecaoNumismatica
{
    public partial class NumiRegisterUser : System.Web.UI.Page
    {
        string clientid = ConfigurationManager.AppSettings["clientid"];
        string clientsecret = ConfigurationManager.AppSettings["clientsecret"];
        string redirectionURL = ConfigurationManager.AppSettings["redirection_url"];
        string url = ConfigurationManager.AppSettings["url"];


        protected void Page_Load(object sender, EventArgs e)
        {
            FaceBookConnect.API_Key = ConfigurationManager.AppSettings["facebookkey"];
            FaceBookConnect.API_Secret = ConfigurationManager.AppSettings["facebooksecret"];
            FaceBookConnect.Version = ConfigurationManager.AppSettings["facebookversion"];

            string script = @"
                            document.getElementById('btn_home').classList.remove('hidden');";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);

            if (!IsPostBack)
            {
                if (Request.QueryString["code"] != null)
                {
                    if (Session["Google"].ToString() == "Yes")
                    {
                        GetToken(Request.QueryString["code"].ToString());
                    }
                    else if (Session["Facebook"].ToString() == "Yes")
                    {
                        string data = FaceBookConnect.Fetch(Request.QueryString["code"], "me", "id,name,email");
                        FaceBookUser faceBookUser = new JavaScriptSerializer().Deserialize<FaceBookUser>(data);
                        tb_user.Text = faceBookUser.Name;
                        tb_email.Text = faceBookUser.Email;
                        
                    }
                }

                if (Request.QueryString["error"] == "access_denied")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User has denied access.')", true);
                    return;
                }
            }

        }

        protected void btn_registar_Click(object sender, EventArgs e)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@user", tb_user.Text); //Adicionar o valor da tb_user ao parâmetro @nome
            myCommand.Parameters.AddWithValue("@pw", EncryptString(tb_pw.Text));
            myCommand.Parameters.AddWithValue("@email", tb_email.Text);


            SqlParameter UserRegister = new SqlParameter();
            UserRegister.ParameterName = "@UserRegister";
            UserRegister.Direction = ParameterDirection.Output;
            UserRegister.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(UserRegister);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiRegister"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            int AnswUserRegister = Convert.ToInt32(myCommand.Parameters["@UserRegister"].Value);

            myCon.Close(); //Fechar a conexão

            if (AnswUserRegister == 1)
            {
                lbl_message.Text = "Foi enviado um e-mail para ativação da conta!";
                //Tratamento de exceção.... Try_Catch
                //Try_Catch para a messagem para prevenir que a mensagem está correta
                try
                {
                    SmtpClient servidor = new SmtpClient();//Servidor SMTP - pass, user, porto

                    MailMessage emails = new MailMessage(); //Email
                    emails.Subject = "Ativação da conta"; //Nome de quem é o email (tb_mail)
                    emails.From = new MailAddress(ConfigurationManager.AppSettings["SMTP_MailUser"]); //Sender
                    emails.To.Add(new MailAddress(tb_email.Text)); //Destinatário

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
                lbl_message.Text = "Utilizador e/ou e-mail já existe!";
            }

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

        public void GetToken(string code)
        {
            string poststring = "grant_type=authorization_code&code=" + code + "&client_id=" + clientid + "&client_secret=" + clientsecret + "&redirect_uri=" + redirectionURL + "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            UTF8Encoding utfenc = new UTF8Encoding();
            byte[] bytes = utfenc.GetBytes(poststring);
            Stream outputstream = null;
            try
            {
                request.ContentLength = bytes.Length;
                outputstream = request.GetRequestStream();
                outputstream.Write(bytes, 0, bytes.Length);
            }
            catch { }
            var response = (HttpWebResponse)request.GetResponse();
            var streamReader = new StreamReader(response.GetResponseStream());
            string responseFromServer = streamReader.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            Classes.Tokenclass obj = js.Deserialize<Classes.Tokenclass>(responseFromServer);
            GetuserProfile(obj.access_token);
        }
        public void GetuserProfile(string accesstoken)
        {
            string url = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=" + accesstoken + "";
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            JavaScriptSerializer js = new JavaScriptSerializer();
            Classes.Userclass userinfo = js.Deserialize<Classes.Userclass>(responseFromServer);
            tb_user.Text = userinfo.name;
            tb_email.Text = userinfo.email;
        }
    }
}