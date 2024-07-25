<%@ Page Title="Main Page" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiMainPage.aspx.cs" Inherits="ColecaoNumismatica.NumiMainPage" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="messageAR" class="hidden"><asp:Label ID="lbl_message" runat="server" Text=""></asp:Label></div>
<div class="container">

<div class="row">
    <div class="col-lg">
        <div class="selectpicker" style="display:flex;justify-content:center;padding:10px">
            <asp:DropDownList ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipo" DataTextField="Tipo" DataValueField="CodTipoMN" AutoPostBack="true" AppendDataBoundItems="true"></asp:DropDownList>
            <asp:SqlDataSource ID="SQLDSTipo" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinMNType]"></asp:SqlDataSource>
            &nbsp;&nbsp;
            <asp:DropDownList ID="ddl_preco" runat="server" AutoPostBack="True">
                <asp:ListItem Value="Preço Ascendente"></asp:ListItem>
                <asp:ListItem Value="Preço Descendente"></asp:ListItem>
            </asp:DropDownList>
        </div>
<div style="display: flex; justify-content: center; padding: 20px;">
        <asp:Repeater ID="rpt_mainpage" runat="server" OnItemCommand="rpt_mainpage_ItemCommand">
            <HeaderTemplate>
                <div class="container">
                    <div class="row">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="col-xl-2 col-lg-3 col-sm-6 card" style="background-color: lightblue; height:300px; min-width:250px; padding: 10px; margin: 1px; text-align: center">
                    <div><b>
                        <asp:LinkButton href='<%# "NumiMoneyDetail.aspx?id=" + Eval("cod") + "&estado=" + Eval("estado") %>' ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:LinkButton>
                    </b></div>
                    <br />
                    <div style="padding: 5px;">
                        <img src="<%# Eval("imagem") %>" style="width: 150px; height: 150px;"><br />
                    </div>
                    <div style="color:darkslategrey">
                       <i>Estado: <asp:label runat="server" ID="lbl_estado"><%# Eval("estado") %></asp:label></i>
                    </div>
                        <div style="color:darkslategrey">
                       <i>Valor Atual: <asp:label runat="server" ID="lbl_valorAtual"><%# Eval("valorAtual") %></asp:label></i>
                    </div>
                    <div>
                        <asp:LinkButton runat="server" ID="lbtn_like" class="btn btn-mini" CommandName="like" CommandArgument='<%# Eval("cod") %>'><i id="like" class="fa fa-heart fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                    </div>
                </div>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <div class="col-xl-2 col-lg-3 col-sm-6 card" style="background-color: ghostwhite; height:300px; min-width:250px; padding: 10px; margin: 1px; text-align: center">
                    <div><b>
                        <asp:LinkButton href='<%# "NumiMoneyDetail.aspx?id=" + Eval("cod") + "&estado=" + Eval("estado") %>' ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:LinkButton>
                    </b></div>
                    <br />
                    <div style="padding: 5px;">
                        <img src="<%# Eval("imagem") %>" style="width: 150px; height: 150px;"><br />
                    </div>
                     <div style="color:darkslategrey">
                       <i>Estado: <asp:label runat="server" ID="lbl_estado"><%# Eval("estado") %></asp:label></i>
                    </div>
                        <div style="color:darkslategrey">
                       <i>Valor Atual: <asp:label runat="server" ID="lbl_valorAtual"><%# Eval("valorAtual") %></asp:label></i>
                    </div>
                    <div>
                        <asp:LinkButton runat="server" ID="lbtn_like" class="btn btn-mini" CommandName="like" CommandArgument='<%# Eval("cod") %>'><i id="like" class="fa fa-heart fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                    </div>
                </div>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </div>
              </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="display: flex; justify-content: center;">
        <asp:LinkButton ID="lbtn_previous" runat="server" OnClick="lbtn_previous_Click">Previous</asp:LinkButton>&nbsp;&nbsp;
        <asp:Label ID="lbl_pageNumber" runat="server"></asp:Label>&nbsp;&nbsp;
        <asp:LinkButton ID="lbtn_next" runat="server" OnClick="lbtn_next_Click">Next</asp:LinkButton>
    </div>
</div>
</div>
</div>
<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
 <!--Modal -->
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
<ContentTemplate>
    <div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">Quantos moedas queres adicionar?</h5>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="form-group">                          
                            <label for="tb_quantidade" class="col-form-label">Quantidade:</label>
                            <asp:TextBox ID="tb_quantidade" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                            <asp:Label ID="lbl_quantidade" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button class="btn btn-secondary" ID="btn_close" runat="server" Text="Fechar" />
                    <asp:Button class="btn btn-primary" ID="btn_add" runat="server" Text="Adicionar" OnClick="btn_add_Click" />
                </div>
            </div>
        </div>
    </div>
 </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btn_add" />
        <asp:PostBackTrigger ControlID="btn_close" />
    </Triggers>
   </asp:UpdatePanel>
    <script>
        // Re-trigger the modal after postback
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
            if ($('#exampleModal').is(':hidden')) { // Check if the modal is hidden
                $('#exampleModal').modal('show'); // Show the modal
            }
        });

    </script>

     
</asp:Content>
