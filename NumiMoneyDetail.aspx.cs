﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ColecaoNumismatica
{
    public partial class NumiMoneyDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] == null)
            {

                Response.Redirect("NumiMainPage.aspx");

            }
            else
            {

                List<Money> LstMoney = new List<Money>();

                string query = $"SELECT NC.[CodMN],NC.[Titulo],NC.[Descricao],NS.[Estado],NC.[ValorCunho],NSMN.[ValorAtual],NCI.[Imagem],NCT.[Tipo] AS [Tipo] FROM [dbo].[NumiCoinMoney] NC INNER JOIN [dbo].[NumiCoinStateMN] NSMN ON NC.[CodMN] = NSMN.[CodMN] INNER JOIN [dbo].[NumiCoinState] NS ON NSMN.[CodEstado] = NS.[CodEstado] INNER JOIN [dbo].[NumiCoinMNImage] NCI ON NSMN.[CodImagem] = NCI.[CodImagem] INNER JOIN[dbo].[NumiCoinMNType] NCT ON NC.[CodTipoMN] = NCT.[CodTipoMN] WHERE NC.[CodMN] = {Request.QueryString["id"]}; ";

                SqlConnection myCon = new SqlConnection(ConfigurationManager.ConnectionStrings["NumiCoinConnectionString"].ConnectionString);

                SqlCommand myCommand = new SqlCommand(query, myCon);

                myCon.Open();

                SqlDataReader dr = myCommand.ExecuteReader();

                while (dr.Read())
                {   
                    
                    lbl_titulo.Text = dr["Titulo"].ToString();
                    lt_descricao.Text = dr["Descricao"].ToString();
                    lbl_estado.Text = dr["Estado"].ToString();
                    lbl_tipo.Text = dr["Tipo"].ToString();
                    lbl_valorCunho.Text = (dr["ValorCunho"]).ToString();
                    lbl_valorAtual.Text = (dr["ValorAtual"]).ToString();

                    Money record = new Money();
                    record.imagem = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])dr["Imagem"]);
                    LstMoney.Add(record);  
                }

                myCon.Close();

                foreach (Money coin in LstMoney)
                {
                    Image img = new Image();
                    img.ImageUrl = coin.imagem;

                    // Add the Image control to the container
                    imagePanel.Controls.Add(img);

                    // Optionally, add a line break after each image
                    imagePanel.Controls.Add(new LiteralControl("&nbsp;"));
                }


            }
        }

        protected void btn_back_Click(object sender, EventArgs e)
        {
            Response.Redirect("NumiMainPage.aspx");
        }
    }
}