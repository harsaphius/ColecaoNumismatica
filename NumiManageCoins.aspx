<%@ Page Title="" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiManageCoins.aspx.cs" Inherits="ColecaoNumismatica.NumiManageCoins"  ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="display:flex;justify-content:center;padding:20px;">
    <asp:Repeater ID="rpt_manageCoins" runat="server" DataSourceID="SQLDSMoney" OnItemDataBound="rpt_manageCoins_ItemDataBound" OnItemCommand="rpt_manageCoins_ItemCommand">
                <HeaderTemplate>
                <table border="0" style="border-collapse:collapse; text-align:center;" width="90%">
                    <tr style="background-color:dodgerblue;">
                         <td><b>Código Money</b></td>
                         <td><b>Título</b></td>
                         <td><b>Descrição</b></td>
                         <td><b>Estado</b></td>
                         <td><b>Tipo</b></td>
                         <td><b>Valor Atual</b></td>
                         <td><b>Imagens</b></td>
                         <td><b>Gravar / Apagar></b> </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="background-color:lightblue;"> 
                         <td><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                         <td><asp:TextBox ID="tb_titulo" runat="server"></asp:TextBox></td>
                         <td><asp:TextBox ID="tb_descricao" runat="server" TextMode="MultiLine"/></td>
                         <td><asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDSEstado" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList></td>
                         <td><asp:DropDownList ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipo" DataTextField="Tipo" DataValueField="CodTipoMN"></asp:DropDownList></td>
                         <td><asp:TextBox ID="tb_valorAtual" runat="server"/></asp:TextBox></td> 
                         <td><asp:Panel ID="imagePanel" runat="server"></asp:Panel><br />
                             <asp:FileUpload ID="fu_imagens" runat="server" /></td>
                         <td><asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                         &nbsp;&nbsp;<asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr style="background-color:ghostwhite;"> 
                        <td><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                         <td><asp:TextBox ID="tb_titulo" runat="server"></asp:TextBox></td>
                         <td><asp:TextBox ID="tb_descricao" runat="server" TextMode="MultiLine"/></td>
                         <td><asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDSEstado" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList></td>
                         <td><asp:DropDownList ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipo"  DataTextField="Tipo" DataValueField="CodTipoMN"></asp:DropDownList></td>
                         <td><asp:TextBox ID="tb_valorAtual" runat="server"/></asp:TextBox></td>  
                         <td><asp:FileUpload ID="fu_imagens" runat="server" /></td>
                         <td><asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                         &nbsp;&nbsp;<asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </table>
                </FooterTemplate>
    </asp:Repeater>
         <asp:SqlDataSource ID="SQLDSMoney" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT NCM.CodMN, NCM.Titulo, NCM.Descricao, NCSMN.CodEstado, NCSMN.ValorAtual, NCM.CodTipoMN FROM NumiCoinMoney NCM INNER JOIN NumiCoinStateMN NCSMN ON NCM.CodMN = NCSMN.CodMN INNER JOIN NumiCoinMNImage NCI ON NCM.CodMN = NCI.CodMN GROUP BY NCM.CodMN, NCM.CodTipoMN, NCM.Titulo, NCM.Descricao, NCSMN.ValorAtual, NCSMN.CodEstado ORDER BY NCM.CodMN"></asp:SqlDataSource>
         <asp:SqlDataSource ID="SQLDSTipo" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinMNType]"></asp:SqlDataSource>
         <asp:SqlDataSource ID="SQLDSEstado" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinState]"></asp:SqlDataSource>
      </div>
</asp:Content>
