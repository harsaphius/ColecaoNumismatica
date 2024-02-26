using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiManageCoins : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string script;
            lbl_message.Text = "";
            lbl_message.CssClass = "";

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

        /// <summary>
        /// Função de DataBound do repeater rpt_managaCoins com os dados da BD; Preenche também o nested repeater rpt_imagens com as informações da função GetImagePerCod()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rpt_manageCoins_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView dataRow = (DataRowView)e.Item.DataItem; //Acesso campo a campo

                Session["CodMN"] = Convert.ToInt32(dataRow["CodMN"]);

                ((Label)e.Item.FindControl("lbl_cod")).Text = dataRow["CodMN"].ToString();
                ((TextBox)e.Item.FindControl("tb_titulo")).Text = dataRow["Titulo"].ToString();
                ((TextBox)e.Item.FindControl("tb_descricao")).Text = dataRow["Descricao"].ToString();
                ((DropDownList)e.Item.FindControl("ddl_estado")).SelectedValue = dataRow["CodEstado"].ToString();
                ((DropDownList)e.Item.FindControl("ddl_tipo")).SelectedValue = dataRow["CodTipoMN"].ToString();
                ((TextBox)e.Item.FindControl("tb_valorAtual")).Text = dataRow["ValorAtual"].ToString();

                Repeater rpt_imagens = (Repeater)e.Item.FindControl("rpt_imagens");

                foreach (RepeaterItem item in rpt_imagens.Items)
                {
                    HiddenField hiddenCodImage = (HiddenField)item.FindControl("hiddenCodImage");
                    if (dataRow["CodImagem"] != null && int.TryParse(dataRow["CodImagem"].ToString(), out int codImagem))
                    {
                        hiddenCodImage.Value = codImagem.ToString();
                    }
                }

                List<ImageInfo> images = GetImagesPerCodMN();

                rpt_imagens.DataSource = images;
                rpt_imagens.DataBind();

                ((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument = dataRow["CodMN"].ToString();
                ((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument = dataRow["CodMN"].ToString();
            }

        }

        /// <summary>
        /// Função de ItemCommand do repeater rpt_manageCoins que utiliza as funções NumiCoinUpdateCoin e NumiCoinRemoveCoin
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void rpt_manageCoins_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.Equals("imgBtn_grava"))
            {
                int CodMN = Convert.ToInt32(e.CommandArgument);

                FileUpload fileUploadInner = (FileUpload)e.Item.FindControl("fileUploadNewImage");

                if (fileUploadInner.HasFile)
                {
                    foreach (HttpPostedFile postedFile in fileUploadInner.PostedFiles)
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

                            sqlCommand3.Parameters.AddWithValue("@Imagem", fileData);
                            sqlCommand3.Parameters.AddWithValue("@CodMN", CodMN);

                            sqlCommand3.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                            sqlCommand3.CommandText = "NumiCoinInsertImage"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                            connection.Open();
                            sqlCommand3.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }

                Repeater rpt_imagens = (Repeater)e.Item.FindControl("rpt_imagens");

                foreach (RepeaterItem item in rpt_imagens.Items)
                {
                    FileUpload fileUpload = (FileUpload)item.FindControl("fileUploadInner");
                    HiddenField hiddenCodImage = (HiddenField)item.FindControl("hiddenCodImage");

                    if (fileUpload.HasFile)
                    {
                        foreach (HttpPostedFile postedFile in fileUpload.PostedFiles)
                        {
                            string fileName = postedFile.FileName;
                            string fileContentType = postedFile.ContentType;
                            int fileSize = postedFile.ContentLength;

                            // Ler o conteúdo do ficheiro para o array de bytes
                            byte[] fileData = new byte[fileSize];
                            postedFile.InputStream.Read(fileData, 0, fileSize);

                            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString))
                            {
                                SqlCommand sqlCommand3 = new SqlCommand();
                                sqlCommand3.Connection = connection;
                                sqlCommand3.Parameters.AddWithValue("@Imagem", fileData);
                                sqlCommand3.Parameters.AddWithValue("@CodMN", CodMN);
                                sqlCommand3.Parameters.AddWithValue("@CodImagem", int.Parse(hiddenCodImage.Value));

                                sqlCommand3.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                                sqlCommand3.CommandText = "NumiCoinUpdateImage"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                                connection.Open();
                                sqlCommand3.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                    }
                }


                List<string> Values = new List<string>();
                Values.Add(((ImageButton)e.Item.FindControl("imgBtn_grava")).CommandArgument);
                Values.Add(((TextBox)e.Item.FindControl("tb_titulo")).Text);
                Values.Add(((TextBox)e.Item.FindControl("tb_descricao")).Text);
                Values.Add(((DropDownList)e.Item.FindControl("ddl_estado")).SelectedValue);
                Values.Add(((DropDownList)e.Item.FindControl("ddl_tipo")).SelectedValue);
                Values.Add(((TextBox)e.Item.FindControl("tb_valorAtual")).Text);

                int AnswCoinExists = NumiCoinUpdateCoin(Values);

                if (AnswCoinExists == 0)
                {
                    lbl_message.Text = "Update efetuado com sucesso!";
                    lbl_message.CssClass = "added";
                }
                else if (AnswCoinExists == 1)
                {
                    lbl_message.Text = "Não foi possível efetuar o update porque já existe essa moeda nesse estado!";
                    lbl_message.CssClass = "removed";
                }

                rpt_manageCoins.DataBind();
            }

            if (e.CommandName.Equals("imgBtn_apaga"))
            {
                NumiCoinRemoveCoin(((ImageButton)e.Item.FindControl("imgBtn_apaga")).CommandArgument);

                rpt_manageCoins.DataBind();
            }

        }

        /// <summary>
        /// Função de ItemCommand do repeater rpt_imagens apagar uma determinada imagem da BD
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void rpt_imagens_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteImage")
            {
                string codImageToDelete = e.CommandArgument.ToString();

                // Your logic to delete the image associated with codImageToDelete
                // For example, you might have a method to delete the image from the server
                string query = "DELETE FROM NumiCoinMNImage WHERE ";

                SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);

                myConn.Open();

                query += "CodImagem=" + codImageToDelete;

                SqlCommand myCommand = new SqlCommand(query, myConn);
                myCommand.ExecuteNonQuery();
                myConn.Close();

                // Rebind the repeater to reflect the changes
                rpt_manageCoins.DataBind(); // Assuming you have a method to bind data to the repeater
            }
        }

        /// <summary>
        /// Função para fazer o Update a uma Coin
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int NumiCoinUpdateCoin(List<string> values)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados

            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@CodMN", values[0]);
            myCommand.Parameters.AddWithValue("@Titulo", values[1]);
            myCommand.Parameters.AddWithValue("@Descricao", values[2]);
            myCommand.Parameters.AddWithValue("@CodEstado", values[3]);
            myCommand.Parameters.AddWithValue("@CodTipoMN", values[4]);
            if (values[5].Contains("."))
            {
                values[5] = values[5].Replace(".", ",");
                myCommand.Parameters.AddWithValue("@ValorAtual", Convert.ToDecimal(values[5]));
            }
            else
            {
                myCommand.Parameters.AddWithValue("@ValorAtual", Convert.ToDecimal(values[5]));
            }

            SqlParameter CoinExists = new SqlParameter();
            CoinExists.ParameterName = "@CoinExists";
            CoinExists.Direction = ParameterDirection.Output;
            CoinExists.SqlDbType = SqlDbType.Int;

            myCommand.Parameters.Add(CoinExists);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiUpdateCoin"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados

            myCon.Close();
            int AnswCoinExists = Convert.ToInt32(myCommand.Parameters["@CoinExists"].Value);

            return AnswCoinExists;
        }

        /// <summary>
        /// Função para Remover uma Coin
        /// </summary>
        /// <param name="code"></param>
        public static void NumiCoinRemoveCoin(string code)
        {
            SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString); //Definir a conexão à base de dados
            SqlCommand myCommand = new SqlCommand(); //Novo commando SQL 
            myCommand.Parameters.AddWithValue("@CodMN", code);

            myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
            myCommand.CommandText = "NumiRemoveCoin"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

            myCommand.Connection = myCon; //Definição de que a conexão do meu comando é a minha conexão definida anteriormente
            myCon.Open(); //Abrir a conexão
            myCommand.ExecuteNonQuery(); //Executar o Comando Non Query dado que não devolve resultados - Não efetua query à BD - Apenas insere dados
            myCon.Close();
        }

        /// <summary>
        /// Função que retorna uma lista de ImageInfos com a ImageBase64 e o Código da Imagem
        /// </summary>
        /// <returns></returns>
        public List<ImageInfo> GetImagesPerCodMN()
        {
            List<ImageInfo> imageList = new List<ImageInfo>();
            string query = $"SELECT CodImagem, Imagem FROM [NumiCoinMNImage] WHERE CodMN = {Convert.ToInt32(Session["CodMN"])}";

            using (SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString))
            {
                SqlCommand myCommand = new SqlCommand(query, myCon);
                myCon.Open();

                SqlDataReader dr = myCommand.ExecuteReader();
                while (dr.Read())
                {
                    int codImagem = Convert.ToInt32(dr["CodImagem"]);
                    string imageBase64 = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dr["Imagem"]);
                    ImageInfo imageInfo = new ImageInfo
                    {
                        CodImagem = codImagem,
                        ImageBase64 = imageBase64
                    };

                    imageList.Add(imageInfo);
                }
            }

            return imageList;
        }

        /// <summary>
        /// Classe ImageInfo que contém o CodImagem e respetiva ImageBase64
        /// </summary>
        public class ImageInfo
        {
            public int CodImagem { get; set; }
            public string ImageBase64 { get; set; }
        }

    }
}
