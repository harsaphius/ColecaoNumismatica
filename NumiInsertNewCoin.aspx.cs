using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiInsertNewCoin : System.Web.UI.Page
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
        }

        protected void btn_insert_Click(object sender, EventArgs e)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados
            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL

            myCommand.Parameters.AddWithValue("@Titulo", tb_titulo.Text);
            myCommand.Parameters.AddWithValue("@CodTipoMN", ddl_tipo.SelectedValue);
            myCommand.Parameters.AddWithValue("@CodEstado", ddl_estado.SelectedValue);
            myCommand.Parameters.AddWithValue("@Descricao", tb_descricao.Text);
            if(tb_valorCunho.Text.Contains("."))
            {
                tb_valorCunho.Text = tb_valorCunho.Text.Replace(".", ",");
                myCommand.Parameters.AddWithValue("@ValorCunho", Convert.ToDecimal(tb_valorCunho.Text));
            }
            else
            {
                myCommand.Parameters.AddWithValue("@ValorCunho", Convert.ToDecimal(tb_valorCunho.Text));
            }
            
            //Devolver o código da moeda/nota
            SqlParameter CodMN = new SqlParameter();
            CodMN.ParameterName = "@CodMN";
            CodMN.Direction = ParameterDirection.Output;
            CodMN.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(CodMN);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiInsertCoin"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados

            int AnswCodMN = Convert.ToInt32(myCommand.Parameters["@CodMN"].Value);

            //Insert Coin State
            SqlCommand sqlCommand2 = new SqlCommand();

            sqlCommand2.Connection = myCon;
            sqlCommand2.Parameters.AddWithValue("@CodMN", AnswCodMN);
            sqlCommand2.Parameters.AddWithValue("@CodEstado", ddl_estado.SelectedValue);
            if (tb_valorAtual.Text.Contains("."))
            {
                tb_valorAtual.Text = tb_valorAtual.Text.Replace(".", ","); 
                sqlCommand2.Parameters.AddWithValue("@ValorAtual", Convert.ToDecimal(tb_valorAtual.Text));
            }
            else
            {
                sqlCommand2.Parameters.AddWithValue("@ValorAtual", Convert.ToDecimal(tb_valorAtual.Text));
            }
           
            sqlCommand2.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            sqlCommand2.CommandText = "NumiCoinStateMNInsert"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            sqlCommand2.ExecuteNonQuery();
            myCon.Close();

            //Insert Coin MNImage
            //Imagens
            foreach (HttpPostedFile postedFile in fu_imagens.PostedFiles)
            {
                string fileName = Path.GetFileName(postedFile.FileName);
                string fileContentType = postedFile.ContentType;
                int fileSize = postedFile.ContentLength;

                // Ler o conteúdo do ficheiro para o array de bytes
                byte[] fileData = new byte[fileSize];
                postedFile.InputStream.Read(fileData, 0, fileSize);

                //Gravar o ficheiro na base de dados
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString))
                {
                    SqlCommand sqlCommand3 = new SqlCommand();
                    sqlCommand3.Connection = connection;

                    if ((fu_imagens.HasFile))
                    {
                        sqlCommand3.Parameters.AddWithValue("@Imagem", fileData);
                    }
                    else
                    {
                        string imagePath = "C:\\Users\\pcris\\source\\repos\\ColecaoNumismatica\\Images\\NumiDefault.png";

                        // Read the image file into a byte array
                        byte[] binaryData;
                        using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                        {
                            binaryData = new byte[fileStream.Length];
                            fileStream.Read(binaryData, 0, (int)fileStream.Length);
                        }

                        sqlCommand3.Parameters.AddWithValue("@Imagem", binaryData);
                    }

                    sqlCommand3.Parameters.AddWithValue("@CodMN", AnswCodMN);

                    SqlParameter CodImagem = new SqlParameter();
                    CodImagem.ParameterName = "@CodImagem";
                    CodImagem.Direction = ParameterDirection.Output;
                    CodImagem.SqlDbType = SqlDbType.Int;

                    sqlCommand3.Parameters.Add(CodImagem);

                    sqlCommand3.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                    sqlCommand3.CommandText = "NumiCoinMNImageInsert"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                    connection.Open();
                    sqlCommand3.ExecuteNonQuery();
                    connection.Close();
                }              
            }
            myCon.Close();

            lbl_message.Text = "Money inserido com sucesso!";
        }
    }
}