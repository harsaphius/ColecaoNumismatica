using ASPSnippets.FaceBookAPI;
using ASPSnippets.GoogleAPI;
using ColecaoNumismatica.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiRegisterUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logado"] == null)
            {
                string script = @"                      
                            document.getElementById('navBarDropDown').classList.remove('hidden');
                            document.getElementById('btn_home').classList.remove('hidden');
                            document.getElementById('btn_login').classList.remove('hidden');";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);
            }
            else if (Session["Logado"].ToString() == "Yes" || Page.IsPostBack == true)
            {
                string isAdmin = Session["Admin"].ToString();
                string user = Session["User"].ToString();
                string script;

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
                             document.getElementById('btn_statistics').classList.remove('hidden')";

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAdminButtons", script, true);
                }
            }
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["code"]))
                {
                    string email, subject, body, utilizador = "";

                    if (Session["Google"] != null)
                    {
                        if (Session["Google"].ToString() == "Yes")
                        {
                            GoogleConnect.RedirectUri = Request.Url.AbsoluteUri.Split('?')[0];
                            string code = Request.QueryString["code"];
                            string json = GoogleConnect.Fetch("me", code);
                            GoogleProfile profile = new JavaScriptSerializer().Deserialize<GoogleProfile>(json);
                            tb_user.Text = profile.Name;
                            tb_email.Text = profile.Email;

                            List<object> Answers = CheckEmail();

                            int AccountActive = Convert.ToInt32(Answers[4]);

                            if (Convert.ToInt32(Answers[0]) == 1 && Convert.ToInt32(Answers[1]) == 1 && AccountActive == 1)
                            {
                                Session["Logado"] = "Yes";
                                Session["Admin"] = "Yes";
                                Session["User"] = Convert.ToString(Answers[2]);
                                utilizador = Session["User"].ToString();
                                Session["CodUtilizador"] = Convert.ToInt32(Answers[3]);
                                Session["Google"] = null;
                                Response.Redirect("NumiMainPage.aspx");
                            }
                            if (Convert.ToInt32(Answers[0]) == 1 && AccountActive == 0)
                            {
                                lbl_message.Text = "A sua conta não está ativa!";
                                lbl_message.CssClass = "added";

                                email = tb_email.Text;
                                subject = "E-mail de ativação";
                                body = $"<b>Obrigado pela sua inscrição. Para ativar a sua conta clique <a href='https://localhost:44399/NumiAtivationPage.aspx?user={MyFunctions.EncryptString(utilizador)}&redirected=true'> aqui</a>!";
                                MyFunctions.SendEmail(email, body, subject);
                                Session["Google"] = null;
                            }
                            if (Convert.ToInt32(Answers[0]) == 1 && Convert.ToInt32(Answers[1]) == 0)
                            {
                                Session["Logado"] = "Yes";
                                Session["Admin"] = "No";
                                Session["User"] = Convert.ToString(Answers[2]);
                                Session["CodUtilizador"] = Convert.ToInt32(Answers[3]);
                                Response.Redirect("NumiMainPage.aspx");
                                Session["Google"] = null;
                            }
                        }
                    }

                    if (Session["Facebook"] != null)
                    {
                        if (Session["Facebook"].ToString() == "Yes")
                        {
                            string data = FaceBookConnect.Fetch(Request.QueryString["code"], "me", "id, name, email");
                            FaceBookUser faceBookUser = new JavaScriptSerializer().Deserialize<FaceBookUser>(data);
                            tb_user.Text = faceBookUser.Name;
                            tb_email.Text = faceBookUser.Email;

                            List<object> Answers = CheckEmail();

                            int AccountActive = Convert.ToInt32(Answers[4]);

                            if (Convert.ToInt32(Answers[0]) == 1 && Convert.ToInt32(Answers[1]) == 1 && AccountActive == 1)
                            {
                                Session["Logado"] = "Yes";
                                Session["Admin"] = "Yes";
                                Session["User"] = Convert.ToString(Answers[2]);
                                Session["CodUtilizador"] = Convert.ToInt32(Answers[3]);
                                Session["Facebook"] = null;

                                Response.Redirect("NumiMainPage.aspx");
                            }
                            if (Convert.ToInt32(Answers[0]) == 1 && AccountActive == 0)
                            {
                                lbl_message.Text = "A sua conta não está ativa!";
                                lbl_message.CssClass = "added";

                                email = tb_email.Text;
                                subject = "E-mail de ativação";
                                body = $"<b>Obrigado pela sua inscrição. Para ativar a sua conta clique <a href='https://localhost:44399/NumiAtivationPage.aspx?user={MyFunctions.EncryptString(tb_user.Text)}&redirected=true'> aqui</a>!";
                                MyFunctions.SendEmail(email, body, subject);

                                Session["Facebook"] = null;
                            }
                            if (Convert.ToInt32(Answers[0]) == 1 && Convert.ToInt32(Answers[1]) == 0)
                            {
                                Session["Logado"] = "Yes";
                                Session["Admin"] = "No";
                                Session["User"] = Convert.ToString(Answers[2]);
                                Session["CodUtilizador"] = Convert.ToInt32(Answers[3]);
                                Session["Facebook"] = null;

                                Response.Redirect("NumiMainPage.aspx");
                            }
                        }
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
            if (chkBoxAccept.Checked == true)
            {
                Session["Registo"] = "Yes";

                string email, body, subject;

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@User", tb_user.Text); //Adicionar o valor da tb_user ao parâmetro @nome
                myCommand.Parameters.AddWithValue("@Pw", MyFunctions.EncryptString(tb_pw.Text));
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
                    lbl_message.CssClass = "added";

                    email = tb_email.Text;
                    subject = "E-mail de ativação";
                    body = $"<b>Obrigado pela sua inscrição. Para ativar a sua conta clique <a href='https://localhost:44399/NumiAtivationPage.aspx?user={MyFunctions.EncryptString(tb_user.Text)}&redirected=true'> aqui</a>!";
                    MyFunctions.SendEmail(email, body, subject);

                }
                else
                {
                    lbl_message.Text = "Utilizador e/ou e-mail já existe!";
                    lbl_message.CssClass = "removed";
                }
            }
            else
            {
                lbl_message.Text = "A aceitação dos termos é obrigatória. Por favor, leia e aceite os termos.";
                lbl_message.CssClass = "notcollected";
            }

        }

        /// <summary>
        /// Função para verificar se o email associado às contas já se encontra registado, o nome de utilizador e se o mesmo é admin ou não 
        /// </summary>
        /// <returns></returns>
        protected List<object> CheckEmail()
        {
            List<object> Answers = new List<object>();

            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@Email", tb_email.Text);

            SqlParameter UserExists = new SqlParameter();
            UserExists.ParameterName = "@UserExists";
            UserExists.Direction = ParameterDirection.Output;
            UserExists.SqlDbType = SqlDbType.Int;
            myCommand.Parameters.Add(UserExists);

            //Variável de Output para SP verificar o perfil
            SqlParameter NumiAdmin = new SqlParameter();
            NumiAdmin.ParameterName = "@NumiAdmin";
            NumiAdmin.Direction = ParameterDirection.Output;
            NumiAdmin.Direction = ParameterDirection.Output;
            NumiAdmin.SqlDbType = SqlDbType.Int;
            myCommand.Parameters.Add(NumiAdmin);

            SqlParameter Utilizador = new SqlParameter();
            Utilizador.ParameterName = "@Utilizador";
            Utilizador.Direction = ParameterDirection.Output;
            Utilizador.Direction = ParameterDirection.Output;
            Utilizador.SqlDbType = SqlDbType.VarChar;
            Utilizador.Size = 50;
            myCommand.Parameters.Add(Utilizador);

            //CodUtilizador
            SqlParameter CodUtilizador = new SqlParameter();
            CodUtilizador.ParameterName = "@CodUtilizador";
            CodUtilizador.Direction = ParameterDirection.Output;
            CodUtilizador.SqlDbType = SqlDbType.Int;
            myCommand.Parameters.Add(CodUtilizador);

            //AccountActive
            SqlParameter AccountActive = new SqlParameter();
            AccountActive.ParameterName = "@AccountActive";
            AccountActive.Direction = ParameterDirection.Output;
            AccountActive.SqlDbType = SqlDbType.Int;
            myCommand.Parameters.Add(AccountActive);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiCheckEmail"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            myCon.Close(); //Fechar a conexão
            int AnswUserExists = Convert.ToInt32(myCommand.Parameters["@UserExists"].Value);
            Answers.Add(AnswUserExists);
            int AnswNumiAdmin = Convert.ToInt32(myCommand.Parameters["@NumiAdmin"].Value);
            Answers.Add(AnswNumiAdmin);
            string AnswUtilizador = Convert.ToString(myCommand.Parameters["@Utilizador"].Value);
            Answers.Add(AnswUtilizador);
            int AnswCodUtilizador = Convert.ToInt32(myCommand.Parameters["@CodUtilizador"].Value);
            Answers.Add(AnswCodUtilizador);
            int AnswAccountActive = Convert.ToInt32(myCommand.Parameters["@AccountActive"].Value);
            Answers.Add(AnswAccountActive);

            return Answers;
        }
    }
}