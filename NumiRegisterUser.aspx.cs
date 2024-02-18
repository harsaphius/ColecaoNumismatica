//using ASPSnippets.FaceBookAPI;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
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
            //FaceBookConnect.API_Key = ConfigurationManager.AppSettings["facebookkey"];
            //FaceBookConnect.API_Secret = ConfigurationManager.AppSettings["facebooksecret"];
            //FaceBookConnect.Version = ConfigurationManager.AppSettings["facebookversion"];

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
                    //    else if (Session["Facebook"].ToString() == "Yes")
                    //    {
                    //        string data = FaceBookConnect.Fetch(Request.QueryString["code"], "me", "id,name,email");
                    //        FaceBookUser faceBookUser = new JavaScriptSerializer().Deserialize<FaceBookUser>(data);
                    //        tb_user.Text = faceBookUser.Name;
                    //        tb_email.Text = faceBookUser.Email;

                    //    }
                    //}

                    //if (Request.QueryString["error"] == "access_denied")
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User has denied access.')", true);
                    //    return;
                    //}
                }

            }
        }
        protected void btn_registar_Click(object sender, EventArgs e)
        {
            if (chkBoxAccept.Checked == true)
            {
                string email, body, subject;

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@User", tb_user.Text); //Adicionar o valor da tb_user ao parâmetro @nome
                myCommand.Parameters.AddWithValue("@Pw", Classes.MyFunctions.EncryptString(tb_pw.Text));
                myCommand.Parameters.AddWithValue("@Email", tb_email.Text);

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

                    email = tb_email.Text;
                    subject = "E-mail de ativação";
                    body = $"<b>Obrigado pela sua inscrição. Para ativar a sua conta clique <a href='https://localhost:44399/NumiAtivationPage.aspx?user={Classes.MyFunctions.EncryptString(tb_user.Text)}'> aqui</a>!";
                    Classes.MyFunctions.SendEmail(email, body, subject);

                }
                else
                {
                    lbl_message.Text = "Utilizador e/ou e-mail já existe!";
                }
            }
            else
            {
                lbl_message.Text = "A aceitação dos termos é obrigatória. Por favor, leia e aceite os termos.";
            }

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
            Classes.TokenClass obj = js.Deserialize<Classes.TokenClass>(responseFromServer);
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
            Classes.UserClass userinfo = js.Deserialize<Classes.UserClass>(responseFromServer);
            tb_user.Text = userinfo.name;
            tb_email.Text = userinfo.email;
        }
    }
}