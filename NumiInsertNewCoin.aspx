﻿<%@ Page Title="Insert New Coin" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiInsertNewCoin.aspx.cs" Inherits="ColecaoNumismatica.NumiInsertNewCoin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="display:flex; justify-content:center; padding:20px;">
<div class="form align-content-center-md-6" style="justify-content:center;padding:5px;background-color:azure;">
  <!-- Row for Título | Tipo | Estado -->
  <div class="form-row">
    <div class="form-group col-md-6">
      <label for="tb_titulo">Título</label>
      <asp:TextBox ID="tb_titulo" runat="server" class="form-control" placeholder="Título"></asp:TextBox>
    </div>
      
    <div class="form-group col-md-3">
      <label for="ddl_tipo">Tipo</label>
      <asp:DropDownList class="form-control" ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipoMN" DataTextField="Tipo" DataValueField="CodTipoMN"></asp:DropDownList>
        <asp:SqlDataSource ID="SQLDSTipoMN" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinMNType]"></asp:SqlDataSource>
    </div>

    <div class="form-group col-md-3">
        <label for="ddl_estado">Estado</label>
        <asp:DropDownList class="form-control" ID="ddl_estado" runat="server" DataSourceID="SQLDSStateMN" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList>
        <asp:SqlDataSource ID="SQLDSStateMN" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinState]"></asp:SqlDataSource>
     </div>
  </div>

    <!-- Row for Descrição -->
  <div class="form-row">   
      <div class="form-group col-md-12">
        <label for="tb_descricao">Descrição</label>
          <asp:TextBox ID="tb_descricao" runat="server" class="form-control" placeholder="Descrição da moeda ou nota" TextMode="MultiLine"></asp:TextBox>
      </div>
  </div>
     <!-- Row for Valor Cunho | Valor Atual | Imagens da Nota | Moeda -->
 <div class="form-row">
    <div class="form-group col-md-3">
      <label for="tb_valorCunho">Valor Cunho</label>
      <asp:TextBox ID="tb_valorCunho" runat="server" class="form-control"></asp:TextBox>
    </div>
     
     <div class="form-group col-md-3">
      <fieldset disabled>
          <label for="tb_valorCunho">Valor Atual</label>
          <asp:TextBox ID="tb_valorAtual" runat="server" class="form-control"></asp:TextBox> 
       </fieldset>
    </div>
   
    <div class="form-group col-md-6">
      <label for="fu_imagens">Imagens da Moeda | Nota</label>
      <asp:FileUpload ID="fu_imagens" class="form-control" runat="server" AllowMultiple="true"/>
      <input type="text" class="form-control" id="inputZip">
    </div>
  </div>
  <asp:Button ID="btn_insert" runat="server" class="btn btn-primary" Text="Insert Coin" OnClick="btn_insert_Click"/><br /><br />
  <asp:Label ID="lbl_message" runat="server" Text=""></asp:Label>
</div>
</div>
</asp:Content>