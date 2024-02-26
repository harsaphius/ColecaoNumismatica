using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string script;

            if (Session["Logado"] == null)
            {
                Response.Redirect("NumiMainPage.aspx");
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

        protected void btn_alterarPw_Click(object sender, EventArgs e)
        {
            if (tb_pwn.Text != tb_pwnr.Text)
            {
                lbl_message.Text = "A password nova e a sua repetição têm que ser iguais!";
            }
            else
            {
                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@Utilizador", Session["User"].ToString());
                myCommand.Parameters.AddWithValue("@PWAtual", Classes.MyFunctions.EncryptString(tb_pw.Text));
                myCommand.Parameters.AddWithValue("@PWNova", Classes.MyFunctions.EncryptString(tb_pwn.Text));

                //Variável de Output para SP verificar se o utilizador e pw estão corretos
                SqlParameter UserExists = new SqlParameter();
                UserExists.ParameterName = "@UserExists";
                UserExists.Direction = ParameterDirection.Output;
                UserExists.SqlDbType = SqlDbType.Int;

                myCommand.Parameters.Add(UserExists);

                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiChangePw"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
                int AnswUserExists = Convert.ToInt32(myCommand.Parameters["@UserExists"].Value);

                myCon.Close(); //Fechar a conexão

                if (AnswUserExists == 1)
                {
                    lbl_message.Text = "Palavra-passe alterada com sucesso!";
                    lbl_message.CssClass = "added";
                }
                else { 
                    lbl_message.Text = "Palavra-passe atual incorreta!";
                    lbl_message.CssClass = "removed";

                }

            }
        }
    }

}