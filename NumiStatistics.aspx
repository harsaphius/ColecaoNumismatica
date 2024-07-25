<%@ Page Title="Statistics" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiStatistics.aspx.cs" Inherits="ColecaoNumismatica.NumiStatistics" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="bg-light py-3 py-md-5">
  <div class="container">
    <div class="row justify-content-md-center">
      <div class="col-12 col-md-10 col-lg-8 col-xl-7 col-xxl-6">
        <h3 class="fs-6 text-secondary mb-2 text-uppercase text-center">NumiCoin</h3>
        <h2 class="mb-4 display-5 text-center">Área de Estatísticas</h2>
        <hr class="w-50 mx-auto mb-5 mb-xl-9 border-dark-subtle">
      </div>
    </div>
  </div>
<div style="display:flex">
  <div class="container">
    <div class="flex-row gy-4 gy-lg-0 align-items-lg-center">
      <div class="col-12 col-lg-6" style="padding-left:80px">
          <asp:Chart ID="Chart1" runat="server" DataSourceID="SQLDSForChart" Width="650px" Height="550px" IsSoftShadows="False">
              <Series>
                  <asp:Series Name="Series1" XValueMember="Moeda" YValueMembers="Valor" IsValueShownAsLabel="true" YValuesPerPoint="1"></asp:Series>
              </Series>
              <ChartAreas>
                  <asp:ChartArea Name="ChartArea1">
                      <AxisX Title="Nome da Moeda"></AxisX>
                      <AxisY Title="Valor Atual da Moeda"></AxisY>
                  </asp:ChartArea>
              </ChartAreas>
              <BorderSkin BackColor="Transparent" />
          </asp:Chart>
          <asp:SqlDataSource ID="SQLDSForChart" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT TOP 10 NCS.ValorAtual AS Valor, NCM.Titulo AS Moeda FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN ORDER BY Valor DESC;"></asp:SqlDataSource></div>
      </div>
      
   </div>
    <br /><br />
      <div class="col-12 col-lg-6">
        <div class="flex-row justify-content-xl-end">
          <div class="col-12 col-xl-11">
            <div class="row gy-4 gy-sm-0 overflow-hidden">
              <div class="col-12 col-sm-6">
                <div class="card border-0 border-bottom border-primary shadow-sm mb-4">
                  <div class="card-body text-center p-4 p-xxl-5">
                    <h3 class="display-2 fw-bold mb-2"><asp:Label runat="server" ID="lbl_TotalUsers"></asp:Label></h3>
                    <p class="fs-5 mb-0 text-secondary">Total de Utilizadores
                  </div>
                </div>
                <div class="card border-0 border-bottom border-primary shadow-sm">
                  <div class="card-body text-center p-4 p-xxl-5">
                    <h3 class="display-2 fw-bold mb-2"><asp:Label runat="server" ID="lbl_TotalUsersCollection"></asp:Label></h3>
                    <p class="fs-5 mb-0 text-secondary">Total de Utilizadores com Coleção</p>
                  </div>
                </div>
              </div>
              <div class="col-12 col-sm-6">
                <div class="card border-0 border-bottom border-primary shadow-sm mt-lg-6 mt-xxl-8 mb-4">
                  <div class="card-body text-center p-6 p-xxl-4">
                    <h4 class="display-2 fw-bold mb-2"><asp:Label runat="server" ID="lbl_AvgCoins"></asp:Label></h4>
                    <p class="fs-5 mb-0 text-secondary">Média de Moedas Por Utilizador</p>
                  </div>
                </div>
                <div class="card border-0 border-bottom border-primary shadow-sm">
                  <div class="card-body text-center p-5 p-xxl-6">
                    <h3 class="display-5 fw-bold mb-2"><asp:Label runat="server" ID="lbl_UserCoin"></asp:Label></h3>
                    <p class="fs-5 mb-0 text-secondary">Utilizador com Mais Moedas - Quantidade de Moedas</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</section>
</asp:Content>
