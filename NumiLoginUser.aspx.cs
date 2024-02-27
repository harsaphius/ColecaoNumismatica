using ASPSnippets.FaceBookAPI;
using ASPSnippets.GoogleAPI;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;

namespace ColecaoNumismatica
{
    public partial class NumiLoginUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Google Data
            GoogleConnect.ClientId = ConfigurationManager.AppSettings["clientid"];
            GoogleConnect.ClientSecret = ConfigurationManager.AppSettings["clientsecret"];
            GoogleConnect.RedirectUri = ConfigurationManager.AppSettings["redirection_url"];

            //Facebook Data
            FaceBookConnect.API_Key = ConfigurationManager.AppSettings["FacebookKey"];
            FaceBookConnect.API_Secret = ConfigurationManager.AppSettings["FacebookSecret"];
            FaceBookConnect.Version = ConfigurationManager.AppSettings["FacebookVersion"];

            if(Session["Logado"] == null)
            {
                string script = @"                      
                            document.getElementById('navBarDropDown').classList.remove('hidden');
                            document.getElementById('btn_home').classList.remove('hidden');"                           ;

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);
            }
          
            if (Request.QueryString["redirected"] != null && Request.QueryString["redirected"] == "true")
            {
                lbl_ActivatedUser.Text = Session["ActivatedUser"].ToString();
                lbl_ActivatedUser.CssClass = "added";
            }
        }

        /// <summary>
        /// Função para a recuperação de palavra-passe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRecoverPasswordFE_Click(object sender, EventArgs e)
        {
            string email, body, subject;

            //Recuperação de password com envio de email
            string novaPasse = Membership.GeneratePassword(8, 2);

            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@Email", tb_emailpwrecover.Text); //Adicionar o valor da tb_user ao parâmetro @nome
            myCommand.Parameters.AddWithValue("@PwNova", Classes.MyFunctions.EncryptString(novaPasse));

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
            int AnswAccountActive = Convert.ToInt32(myCommand.Parameters["@AccountActive"].Value);

            myCon.Close(); //Fechar a conexão

            //Caso o utilizador exista e a conta esteja ativa
            if (AnswUserExist == 1 && AnswAccountActive == 1)
            {
                email = tb_emailpwrecover.Text;
                subject = "E-mail de recuperação";
                body = $"Ex.mo(s) Sr.(s), <br /> A sua nova palavra-passe para o e-mail {tb_emailpwrecover.Text} é a seguinte: {novaPasse} <br /> Proceda à sua alteração através do seguinte <a href='https://localhost:44399/NumiChangePassword.aspx?user={Classes.MyFunctions.EncryptString(tb_user.Text)}&redirected=true'> link </a>!<br />";

                lbl_message.Text = $"E-mail enviado para a recuperação da sua conta!";
                lbl_message.CssClass = "removed";

                Classes.MyFunctions.SendEmail(email, body, subject);

            }
            else if (AnswAccountActive == 0) //Caso a conta não esteja ativa
            {
                Session["ActivatedUser"] = "OK";

                email = tb_emailpwrecover.Text;
                subject = "E-mail de ativação";
                body = $"Ex.mo(s) Sr(s), <br /><b>Obrigado pela sua inscrição.</b><br />Para ativar a sua conta clique <a href='https://localhost:44399/NumiAtivationPage.aspx?user={Classes.MyFunctions.EncryptString(tb_user.Text)}&redirected=true'>aqui</a>!";

                lbl_message.Text = $"Este e-mail está registado, mas a conta não se encontra ativa! Foi enviado um e-mail para que proceda à ativação da sua conta!";
                lbl_message.CssClass = "notcollected";

                Classes.MyFunctions.SendEmail(email, body, subject);
            }
            else //Caso o e-mail não esteja associado a nenhuma conta
            {
                lbl_message.Text = "Este e-mail não está associado a nenhuma conta! Registe-se.";
                lbl_message.CssClass = "removed";
            }
        }

        /// <summary>
        /// Login na página NumiLogin através dos dados de cliente: utilizador e pw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLoginBE_Click(object sender, EventArgs e)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@User", tb_user.Text); //Adicionar o valor da tb_user ao parâmetro @nome
            myCommand.Parameters.AddWithValue("@Pw", Classes.MyFunctions.EncryptString(tb_pw.Text));

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

            if (AnswUserExist == 1 && AnswAccountActive == 1)
            {
               
                if (AnswNumiAdmin == 1) Session["Admin"] = "Yes";
                else Session["Admin"] = "No";

                Session["User"] = tb_user.Text;
                Session["CodUtilizador"] = AnswNumiCodUser;
                Session["Logado"] = "Yes";
                Response.Redirect("NumiMainPage.aspx");
            }
            else if (AnswUserExist == 1 && AnswAccountActive == 0)
            {
                Session["ActivatedUser"] = "Conta ativada com sucesso!";

                lbl_message.Text = $"A sua conta ainda não se encontra ativa! <a href='https://localhost:44399/NumiAtivationPage.aspx?user={Classes.MyFunctions.EncryptString(tb_user.Text)}&redirected=true'> Clique aqui para a ativar </a>!";
                lbl_message.CssClass = "notcollected";
            }
            else if (AnswUserExist == -1 && AnswAccountActive == -1)
            {
                lbl_message.Text = "Este utilizador não está associado a nenhuma conta! Registe-se.";
                lbl_message.CssClass = "removed";
            }
            else
            {
                lbl_message.Text = "O seu utilizador ou palavra-passe estão errados! Tente novamente ou recupere a password!";
                lbl_message.CssClass = "removed";
            }

        }

        /// <summary>
        /// Login a partir do Facebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_facebook_Click(object sender, EventArgs e)
        {
            Session["Facebook"] = "Yes";
            FaceBookConnect.Authorize("public_profile,email", ConfigurationManager.AppSettings["redirection_url"]);
        }

        /// <summary>
        /// Login a partir do Google
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_google_Click(object sender, EventArgs e)
        {
            Session["Google"] = "Yes";
            GoogleConnect.Authorize("profile", "email");
        }

    }
}