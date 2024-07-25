<%@ Page Title="Manage Coins" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiManageCoins.aspx.cs" Inherits="ColecaoNumismatica.NumiManageCoins"  ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="display:flex;justify-content:center;padding:5px;"><asp:Label ID="lbl_message" runat="server" Text=""></asp:Label></div>
    <div class="container-fluid w-auto h-auto">
        <asp:Repeater ID="rpt_manageCoins" runat="server" DataSourceID="SQLDSMoney" OnItemDataBound="rpt_manageCoins_ItemDataBound" OnItemCommand="rpt_manageCoins_ItemCommand">
                <HeaderTemplate>
                        <table class="table" border="0" style="border-collapse:collapse; text-align:center;">
                            <tr class=" row bg-primary" >
                                 <td class="col-sm"><b>Código Money</b></td>
                                 <td class="col-sm"><b>Título</b></td>
                                 <td class="col-sm"><b>Descrição</b></td>
                                 <td class="col-sm"><b>Estado</b></td>
                                 <td class="col-sm"><b>Tipo</b></td>
                                 <td class="col-sm"><b>Valor Atual</b></td>
                                 <td class="col-sm"><b>Imagens</b></td>
                                 <td class="col-sm"><b>Gravar</b> &nbsp;<b>Apagar</b></td>
                            </tr>
                        </HeaderTemplate>
                        <ItemTemplate>                           
                            <tr class="row" style="background-color:lightblue;"> 
                                 <td class="col-sm"><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                                 <td class="col-sm"><asp:TextBox ID="tb_titulo" runat="server" ></asp:TextBox></td>
                                 <td class="col-sm"><asp:TextBox ID="tb_descricao" runat="server" TextMode="MultiLine"/></td>
                                 <td class="col-sm"><asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDSEstado" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList></td>
                                 <td class="col-sm"><asp:DropDownList ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipo" DataTextField="Tipo" DataValueField="CodTipoMN"></asp:DropDownList></td>
                                 <td class="col-sm"><asp:TextBox ID="tb_valorAtual" runat="server"></asp:TextBox></td> 
                                 <td class="col-sm">
                                     <asp:Repeater ID="rpt_imagens" runat="server" OnItemCommand="rpt_imagens_ItemCommand">
                                         <ItemTemplate>
                                           <div style="padding:2px">
                                               <asp:Image ID="imgInner" runat="server" ImageUrl='<%# Eval("ImageBase64") %>' Width="100" Height="100" />
                                               <asp:FileUpload ID="fileUploadInner" runat="server" />
                                               <asp:HiddenField ID="hiddenCodImage" runat="server" Value='<%# Eval("CodImagem") %>'/>
                                               <asp:Button ID="btn_apagaImagem" runat="server" Text="Delete" CommandName="DeleteImage" CommandArgument='<%# Eval("CodImagem") %>'/>
                                           </div>
                                         </ItemTemplate>
                                     </asp:Repeater>
                                     New Image? <div style="padding:2px"><asp:FileUpload ID="fileUploadNewImage" runat="server" AllowMultiple="true"/></div>
                                   </td>
                                 <td class="col-sm">
                                 <asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>                 
                                 <asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> 
                                </td>
                            </tr>                          
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr class="row" style="background-color:ghostwhite;"> 
                                 <td class="col-sm"><asp:Label ID="lbl_cod" runat="server" Text=""></asp:Label></td>
                                 <td class="col-sm"><asp:TextBox ID="tb_titulo" runat="server"></asp:TextBox></td>
                                 <td class="col-sm"><asp:TextBox ID="tb_descricao" runat="server" TextMode="MultiLine"/></td>
                                 <td class="col-sm"><asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDSEstado" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList></td>
                                 <td class="col-sm"><asp:DropDownList ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipo"  DataTextField="Tipo" DataValueField="CodTipoMN"></asp:DropDownList></td>
                                 <td class="col-sm"><asp:TextBox ID="tb_valorAtual" runat="server" ></asp:TextBox></td>  
                                 <td class="col-sm">
                                     <asp:Repeater ID="rpt_imagens" runat="server" OnItemCommand="rpt_imagens_ItemCommand">
                                         <ItemTemplate>
                                             <div>
                                               <asp:Image ID="imgInner" runat="server" ImageUrl='<%# Eval("ImageBase64") %>' Width="100" Height="100"/>
                                               <asp:FileUpload ID="fileUploadInner" runat="server" />
                                               <asp:HiddenField ID="hiddenCodImage" runat="server" Value='<%# Eval("CodImagem") %>'/>
                                               <asp:Button ID="btn_apagaImagem" runat="server" Text="Delete" CommandName="DeleteImage" CommandArgument='<%# Eval("CodImagem") %>'/>
                                             </div>
                                         </ItemTemplate>
                                     </asp:Repeater>
                                     New Image? <div style="padding:2px"><asp:FileUpload ID="fileUploadNewImage" runat="server" AllowMultiple="true"/></div>
                                </td>                         
                                <td class="col-sm">
                                 <asp:ImageButton ID="imgBtn_grava" runat="server" ImageURL="~/Icons/save.ico" CommandName="imgBtn_grava"/>
                                 <asp:ImageButton ID="imgBtn_apaga" runat="server" ImageURL="~/icons/delete.ico" CommandName="imgBtn_apaga"/> 
                                </td>
                            </tr>
                            
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                        </table>
                        </FooterTemplate>
                </asp:Repeater>
                 <asp:SqlDataSource ID="SQLDSMoney" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT NCM.CodMN, NCM.Titulo, NCM.Descricao, NCS.CodEstado, NCSMN.ValorAtual, NCI.Imagem,NCI.CodImagem, NCM.CodTipoMN FROM NumiCoinMoney AS NCM LEFT JOIN NumiCoinStateMN AS NCSMN ON NCM.CodMN = NCSMN.CodMN LEFT JOIN NumiCoinState AS NCS ON NCSMN.CodEstado = NCS.CodEstado OUTER APPLY ( SELECT TOP 1 NCI2.Imagem,NCI2.CodImagem FROM NumiCoinMNImage AS NCI2 WHERE NCM.CodMN = NCI2.CodMN ORDER BY NCI2.CodImagem) AS NCI GROUP BY NCM.CodMN, NCM.Titulo, NCM.Descricao, NCM.ValorCunho, NCI.Imagem, NCM.CodTipoMN, NCS.CodEstado, NCSMN.ValorAtual, NCI.CodImagem;"></asp:SqlDataSource>            
                 <asp:SqlDataSource ID="SQLDSTipo" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinMNType]"></asp:SqlDataSource>
                 <asp:SqlDataSource ID="SQLDSEstado" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinState]"></asp:SqlDataSource>
</div>
</asp:Content>
