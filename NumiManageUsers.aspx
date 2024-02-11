<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiManageUsers.aspx.cs" Inherits="ColecaoNumismatica.NumiManageUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="display:flex;justify-content:center;padding:20px;">
    <asp:Repeater ID="rpt_manageUsers" runat="server" DataSourceID="SQLDSUsers" OnItemDataBound="rpt_manageUsers_ItemDataBound" OnItemCommand="rpt_manageUsers_ItemCommand">
                <HeaderTemplate>
                <table border="0" style="border-collapse:collapse; text-align:center;" width="900px">
                    <tr style="background-color:dodgerblue;">
                         <td><b>Código de Utilizador</b></td>
                         <td><b>Utilizador</b></td>
                         <td><b>Conta Ativa</b></td>
                         <td><b>E-mail</b></td>
                         <td><b>Numi Admin</b></td>
                         <td><b>Gravar / Apagar></b> </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="background-color:lightblue;"> 
                         <td><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                         <td><asp:Label ID="lbl_user" runat="server"></asp:Label></td>
                         <td><asp:CheckBox ID="ckb_ativa" runat="server" /></td>
                         <td><asp:TextBox ID="tb_email" runat="server"></asp:TextBox></td>
                         <td><asp:CheckBox ID="ckb_admin" runat="server" /></td>
                         <td><asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                         &nbsp;&nbsp;<asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr style="background-color:ghostwhite;"> 
                         <td><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                         <td><asp:Label ID="lbl_user" runat="server"></asp:Label></td>
                         <td><asp:TextBox ID="tb_ativa" runat="server"></asp:TextBox></td>
                         <td><asp:TextBox ID="tb_email" runat="server"></asp:TextBox></td>
                         <td><asp:TextBox ID="tb_admin" runat="server"></asp:TextBox></td>
                         <td><asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                         &nbsp;&nbsp;<asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </table>
                </FooterTemplate>
    </asp:Repeater>
    <asp:SqlDataSource ID="SQLDSUsers" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinUser]"></asp:SqlDataSource>
    </div>
</asp:Content>
