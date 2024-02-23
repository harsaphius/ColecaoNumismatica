using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace ColecaoNumismatica
{
    public partial class NumiAtivationPage : System.Web.UI.Page
    {
        /// <summary>
        /// Página de Ativação da Conta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["redirected"] != null && Request.QueryString["redirected"] == "true")
            {
                string utilizador = Classes.MyFunctions.DecryptString(Request.QueryString["user"].ToString());

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

                SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
                myCommand.Parameters.AddWithValue("@User", utilizador); //Adicionar o valor da tb_user ao parâmetro @nome

                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiActivate"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
                myCon.Open(); //Abrir a conexão
                myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados

                myCon.Close(); //Fechar a conexão

                Session["ActivatedUser"] = "Conta ativada com sucesso!";

                Response.Redirect("NumiLoginUser.aspx?redirected=true");
            }
            else
            {
                Response.Redirect("NumiMainPage.aspx");
            }

        }
    }
}
