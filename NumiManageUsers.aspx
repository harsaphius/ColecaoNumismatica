<%@ Page Title="Manage Users" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiManageUsers.aspx.cs" Inherits="ColecaoNumismatica.NumiManageUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="display:flex;justify-content:center;padding:5px;"> <asp:Label ID="lbl_message" runat="server" Text=""></asp:Label></div>
    <div class="container-fluid w-auto h-auto">
    <asp:Repeater ID="rpt_manageUsers" runat="server" DataSourceID="SQLDSUsers" OnItemDataBound="rpt_manageUsers_ItemDataBound" OnItemCommand="rpt_manageUsers_ItemCommand">
                <HeaderTemplate>
                <table class="table" border="0" style="border-collapse:collapse; text-align:center;">
                    <tr class="row bg-primary"  style="background-color:dodgerblue;">
                         <td class="col-sm"><b>Código de Utilizador</b></td>
                         <td class="col-sm"><b>Utilizador</b></td>
                         <td class="col-sm"><b>Conta Ativa</b></td>
                         <td class="col-sm"><b>E-mail</b></td>
                         <td class="col-sm"><b>Numi Admin</b></td>
                         <td class="col-sm"><b>Gravar</b> </td>
                         <td class="col-sm"><b>Apagar</b> </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="row" style="background-color:lightblue;"> 
                         <td class="col-sm"><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                         <td class="col-sm"><asp:Label ID="lbl_user" runat="server"></asp:Label></td>
                         <td class="col-sm"><asp:CheckBox ID="ckb_ativa" runat="server" /></td>
                         <td class="col-sm"><asp:TextBox ID="tb_email" runat="server"></asp:TextBox></td>
                         <td class="col-sm"><asp:CheckBox ID="ckb_admin" runat="server" /></td>
                         <td class="col-sm">
                         <asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                        </td>
                        <td class="col-sm">
                         <asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> 
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="row" style="background-color:ghostwhite;"> 
                         <td class="col-sm"><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                         <td class="col-sm"><asp:Label ID="lbl_user" runat="server"></asp:Label></td>
                         <td class="col-sm"><asp:CheckBox ID="ckb_ativa" runat="server" /></td>
                         <td class="col-sm"><asp:TextBox ID="tb_email" runat="server"></asp:TextBox></td>
                         <td class="col-sm"><asp:CheckBox ID="ckb_admin" runat="server" /></td>
                         <td class="col-sm">
                         <asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                        </td>
                        <td class="col-sm">
                         <asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> 
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </table>
                </FooterTemplate>
    </asp:Repeater>
    <asp:SqlDataSource ID="SQLDSUsers" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinUser]"></asp:SqlDataSource>
    </div>

</asp:Content>
