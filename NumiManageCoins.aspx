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
                         <td><b>Gravar / Apagar></b> </td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="background-color:lightblue;"> 
                         <td><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                         <td><asp:TextBox ID="tb_titulo" runat="server"></asp:TextBox></td>
                         <td>
                         <%--<script src="ckeditor/ckeditor.js"></script>--%>
                         <asp:TextBox ID="tb_descricao" runat="server" TextMode="MultiLine"/>
                       <%--  <script type="text/javascript">
                             CKEDITOR.replace('<%=tb_descricao.ClientID%>'),
                             {
                                 customConfig: 'custom/editor_config.js'
                             }
                             );
                         </script></td>--%>
                         <td><asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDSEstado" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList></td>
                         <td><asp:DropDownList ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipo" DataTextField="Tipo" DataValueField="CodTipoMN"></asp:DropDownList></td>
                         <td><asp:TextBox ID="tb_valorAtual" runat="server"/></asp:TextBox></td> 
                         <td><asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                         &nbsp;&nbsp;<asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr style="background-color:ghostwhite;"> 
                        <td><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                         <td><asp:TextBox ID="tb_titulo" runat="server"></asp:TextBox></td>
                        <%-- <script src="ckeditor/ckeditor.js"></script>--%>
                         <td><asp:TextBox ID="tb_descricao" runat="server" TextMode="MultiLine"/></td>
                        <%-- <script type="text/javascript">
                             CKEDITOR.replace('<%=tb_descricao.ClientID%>',
                                 {
                                     customConfig: 'custom/editor_config.js'

                                 }
                             );
                         </script>--%>
                         <td><asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDSEstado" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList></td>
                         <td><asp:DropDownList ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipo"  DataTextField="Tipo" DataValueField="CodTipoMN"></asp:DropDownList></td>
                         <td><asp:TextBox ID="tb_valorAtual" runat="server"/></asp:TextBox></td>  
                         <td><asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                         &nbsp;&nbsp;<asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                </table>
                </FooterTemplate>
    </asp:Repeater>
         <asp:SqlDataSource ID="SQLDSMoney" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT NCM.CodMN, NCM.Titulo, NCM.Descricao, NCS.ValorAtual, NCS.CodEstado, NCM.CodTipoMN FROM NumiCoinMoney AS NCM OUTER APPLY ( SELECT TOP 1 NCI2.Imagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI LEFT JOIN NumiCoinStateMN AS NCS ON NCM.CodMN = NCS.CodMN;"></asp:SqlDataSource>
         <asp:SqlDataSource ID="SQLDSTipo" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinMNType]"></asp:SqlDataSource>
         <asp:SqlDataSource ID="SQLDSEstado" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinState]"></asp:SqlDataSource>
      </div>
</asp:Content>
