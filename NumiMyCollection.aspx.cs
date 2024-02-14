using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace ColecaoNumismatica
{
    public partial class NumiMyCollection : System.Web.UI.Page
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

                string script = @"
                            document.getElementById('btn_home').classList.remove('hidden');
                             document.getElementById('btn_mycollection').classList.remove('hidden');
                            document.getElementById('searchbar').classList.add('d-flex');
                            document.getElementById('searchbar').classList.remove('hidden');
                            document.getElementById('logoutbutton').classList.remove('hidden');
                            document.getElementById('Admin').classList.remove('hidden');";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPageElements", script, true);

                if (isAdmin == "Yes")
                {
                    string script2 = @"
                            document.getElementById('btn_insertNewCoin').classList.remove('hidden');
                            document.getElementById('btn_manageCoins').classList.remove('hidden');
                            document.getElementById('btn_manageUsers').classList.remove('hidden');
                            document.getElementById('btn_registerNewUser').classList.remove('hidden');";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowAdminButtons", script2, true);
                }

                List<Money> LstMoney = new List<Money>();

                string query = $"SELECT DISTINCT NCM.CodMN, NCM.Titulo, NCM.ValorCunho, NCI.Imagem FROM NumiCoinMoney AS NCM LEFT JOIN( SELECT NCI2.CodMN, NCI2.Imagem FROM (SELECT CodMN, MIN(CodImagem) AS FirstImage FROM NumiCoinMNImage GROUP BY CodMN) AS NCI JOIN NumiCoinMNImage AS NCI2 ON NCI.FirstImage = NCI2.CodImagem) AS NCI ON NCM.CodMN = NCI.CodMN LEFT JOIN NumiCoinCollection AS NCC ON NCM.CodMN=NCC.CodMN WHERE CodUtilizador={Session["CodUtilizador"]};";

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);
                SqlCommand myCommand = new SqlCommand(query, myCon);
                myCon.Open();

                SqlDataReader dr = myCommand.ExecuteReader();

                while (dr.Read())
                {
                    Money record = new Money();
                    record.cod = Convert.ToInt32(dr["CodMN"]);
                    record.titulo = dr["Titulo"].ToString();
                    record.valorCunho = Convert.ToDecimal(dr["ValorCunho"]);
                    //record.valorAtual = Convert.ToDecimal(dr["ValorAtual"]);
                    //record.estado = dr["Estado"].ToString();
                    LstMoney.Add(record);
                }

                myCon.Close();
                rpt_mycollection.DataSource = LstMoney;
                rpt_mycollection.DataBind();
            }
        }
    }
}