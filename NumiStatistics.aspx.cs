using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiStatistics : System.Web.UI.Page
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

                SqlConnection myConn = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);
                SqlCommand myCommand = new SqlCommand();

                SqlParameter TotalUsers = new SqlParameter();
                TotalUsers.ParameterName = "@TotalUsers";
                TotalUsers.Direction = ParameterDirection.Output;
                TotalUsers.SqlDbType = SqlDbType.Int;

                myCommand.Parameters.Add(TotalUsers);

                SqlParameter TotalUsersWithCollection = new SqlParameter();
                TotalUsersWithCollection.ParameterName = "@TotalUsersWithCollection";
                TotalUsersWithCollection.Direction = ParameterDirection.Output;
                TotalUsersWithCollection.SqlDbType = SqlDbType.Int;

                myCommand.Parameters.Add(TotalUsersWithCollection);

                SqlParameter AvgCoinsPerUser = new SqlParameter();
                AvgCoinsPerUser.ParameterName = "@AvgCoinsPerUser";
                AvgCoinsPerUser.Direction = ParameterDirection.Output;
                AvgCoinsPerUser.SqlDbType = SqlDbType.Decimal;
                AvgCoinsPerUser.Precision = 18;
                AvgCoinsPerUser.Scale = 2;

                myCommand.Parameters.Add(AvgCoinsPerUser);

                SqlParameter UserWithMostCoin = new SqlParameter();
                UserWithMostCoin.ParameterName = "@UserWithMostCoin";
                UserWithMostCoin.Direction = ParameterDirection.Output;
                UserWithMostCoin.SqlDbType = SqlDbType.VarChar;
                UserWithMostCoin.Size = 50;

                myCommand.Parameters.Add(UserWithMostCoin);

                SqlParameter QuantityOfCoinsOfUser = new SqlParameter();
                QuantityOfCoinsOfUser.ParameterName = "@QuantityOfCoinsOfUser";
                QuantityOfCoinsOfUser.Direction = ParameterDirection.Output;
                QuantityOfCoinsOfUser.SqlDbType = SqlDbType.Int;

                myCommand.Parameters.Add(QuantityOfCoinsOfUser);

                myCommand.CommandType = CommandType.StoredProcedure; //Diz que o command type é uma SP
                myCommand.CommandText = "NumiCoinStatistics"; //Comando SQL Insert para inserir os dados acima na respetiva tabela

                myCommand.Connection = myConn;
                myConn.Open();
                myCommand.ExecuteNonQuery();

                int AnswTotalUsers = Convert.ToInt32(myCommand.Parameters["@TotalUsers"].Value);
                int AnswTotalUsersWithCollection = Convert.ToInt32(myCommand.Parameters["@TotalUsersWithCollection"].Value);
                decimal AnswAvgCoinsPerUser = Convert.ToDecimal(myCommand.Parameters["@AvgCoinsPerUser"].Value);
                string AnswUserWithMostCoin = myCommand.Parameters["@UserWithMostCoin"].Value.ToString();
                int AnswQuantityOfCoinsOfUser = Convert.ToInt32(myCommand.Parameters["@QuantityOfCoinsOfUser"].Value);

                lbl_TotalUsers.Text = AnswTotalUsers.ToString();
                lbl_TotalUsersCollection.Text = AnswTotalUsersWithCollection.ToString();
                lbl_AvgCoins.Text = AnswAvgCoinsPerUser.ToString();
                lbl_UserCoin.Text = AnswUserWithMostCoin + " - " + AnswQuantityOfCoinsOfUser.ToString();
                     
                myConn.Close();

            }
        }
    }
}