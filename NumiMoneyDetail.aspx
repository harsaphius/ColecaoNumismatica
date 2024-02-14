<%@ Page Title="Money Detail" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiMoneyDetail.aspx.cs" Inherits="ColecaoNumismatica.NumiMoneyDetail"  ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        img{
        width:150px;
        height:150px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="display:block;text-align:center;padding:20px;margin:20px;">
    <div><h4><b>Título:</b>&nbsp;<asp:Label ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:Label></h4></div>
    <br />
    <div><b>Valor Cunho:</b>&nbsp;<asp:Label ID="lbl_valorCunho" runat="server" Text=""><%# Eval("valorCunho") %></asp:Label></div>
    <br />
    <div><b>Valor Atual:</b>&nbsp;<asp:Label ID="lbl_valorAtual" runat="server" Text=""><%# Eval("valorAtual") %></asp:Label></div>
    <br />
    <div><b>Estado:</b>&nbsp;<asp:Label ID="lbl_estado" runat="server" Text=""><%# Eval("estado") %></asp:Label></div>
    <br />
    <div><b>Tipo:</b>&nbsp;<asp:Label ID="lbl_tipo" runat="server" Text=""> <%# Eval("tipo") %></asp:Label></div>
    <br />
    <div>
    <asp:Panel ID="imagePanel" runat="server"></asp:Panel><br />
    </div>
    <div><b>Descrição:</b>&nbsp;<asp:Literal ID="lt_descricao" runat="server"></asp:Literal></div>
    <br /><br />
    <asp:Button ID="btn_back" runat="server" Text="Voltar" OnClick="btn_back_Click"/>
</div>
</asp:Content>
