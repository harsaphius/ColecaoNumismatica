<%@ Page Title="Main Page" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiMainPage.aspx.cs" Inherits="ColecaoNumismatica.NumiMainPage" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="messageAR" class="hidden">
        <asp:Label ID="lbl_message" runat="server" Text=""></asp:Label></div>

    <div style="display: flex; justify-content: center; padding: 20px;">

        <div class="form-control-sm">
            <asp:DropDownList ID="ddl_tipo" runat="server" DataSourceID="SQLDSTipo" DataTextField="Tipo" DataValueField="CodTipoMN" AutoPostBack="true" AppendDataBoundItems="true"></asp:DropDownList>
            <asp:SqlDataSource ID="SQLDSTipo" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinMNType]"></asp:SqlDataSource>
            <br />
            <br />
            <asp:DropDownList ID="ddl_preco" runat="server" AutoPostBack="True">
                <asp:ListItem Value="Preço Ascendente"></asp:ListItem>
                <asp:ListItem Value="Preço Descendente"></asp:ListItem>
            </asp:DropDownList>
        </div>

        <asp:Repeater ID="rpt_mainpage" runat="server" OnItemCommand="rpt_mainpage_ItemCommand">
            <HeaderTemplate>
                <div class="container">
                    <div class="row">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="col-lg-3 col-md-4 col-sm-6 card" style="background-color: lightblue; height: 300px; padding: 10px; margin: 1px; text-align: center">
                    <div><b>
                        <asp:LinkButton href='<%# "NumiMoneyDetail.aspx?id=" + Eval("cod") + "&estado=" + Eval("estado") %>' ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:LinkButton>
                    </b></div>
                    <br />
                    <div style="padding: 5px;">
                        <img src="<%# Eval("imagem") %>" style="width: 150px; height: 150px;"><br />
                    </div>
                    <div style="color:darkslategrey">
                       <i>Estado: <asp:label runat="server" ID="lbl_estado"><%# Eval("codC") %></asp:label></i>
                    </div>
                    <div>
                        <asp:LinkButton runat="server" ID="lbtn_like" class="btn btn-mini" CommandName="like" CommandArgument='<%# Eval("cod") %>'><i id="like" class="fa fa-heart fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                          <asp:LinkButton runat="server" ID="lbtn_dislike" class="btn btn-mini" CommandName="dislike" CommandArgument='<%# Eval("cod") %>'><i id="dislike" class="fa fa-thumbs-down fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                    </div>
                </div>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <div class="col-lg-3 col-sm-6 card" style="background-color: ghostwhite; height: 300px; padding: 10px; margin: 1px; text-align: center">
                    <div><b>
                        <asp:LinkButton href='<%# "NumiMoneyDetail.aspx?id=" + Eval("cod") + "&estado=" + Eval("estado") %>' ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:LinkButton>
                    </b></div>
                    <br />
                    <div style="padding: 5px;">
                        <img src="<%# Eval("imagem") %>" style="width: 150px; height: 150px;"><br />
                    </div>
                    <div style="color:darkslategrey">
                       <i>Estado: <asp:label runat="server" ID="lbl_estado"><%# Eval("codC") %></asp:label></i>
                    </div>
                    <div>
                        <asp:LinkButton runat="server" ID="lbtn_like" class="btn btn-mini" CommandName="like" CommandArgument='<%# Eval("cod") %>'><i id="like" class="fa fa-heart fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                        <asp:LinkButton runat="server" ID="lbtn_dislike" class="btn btn-mini" CommandName="dislike" CommandArgument='<%# Eval("cod") %>'><i id="dislike" class="fa fa-thumbs-down fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
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
        
        <asp:LinkButton ID="lbtn_next" runat="server" OnClick="lbtn_next_Click">Next</asp:LinkButton>
    </div>


<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
 <!--Modal -->
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
<ContentTemplate>
    <div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">Add or Remove Coins from Collection</h5>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="form-group">
                            <b>Estado:</b>&nbsp; <asp:Label runat="server" ID="lbl_estadoModal"> </asp:Label>
                            &nbsp;<b> Valor Atual:</b>&nbsp;<asp:Label ID="lbl_valorAtual" runat="server" Text=""></asp:Label>
                            <br />
                            <label for="tb_quantidade" class="col-form-label">Quantidade:</label>
                            <asp:TextBox ID="tb_quantidade" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                            <asp:Label ID="lbl_quantidade" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button class="btn btn-secondary" ID="btn_close" runat="server" Text="Fechar" />
                    <asp:Button class="btn btn-primary" ID="btn_add" runat="server" Text="Adicionar" OnClick="btn_add_Click" />
                    <asp:Button class="btn btn-primary" ID="btn_remove" runat="server" Text="Remover" OnClick="btn_remove_Click" />
                </div>
            </div>
        </div>
    </div>
 </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID="btn_add" />
        <asp:PostBackTrigger ControlID="btn_remove" />
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

        function constructURL(linkButton) {
            // Get the values from the attributes of the link button
            var id = linkButton.getAttribute("data-id");
            var estado = linkButton.getAttribute("data-estado");

            // Construct the URL
            var url = "NumiMoneyDetail.aspx?id=" + id + "&estado=" + estado;

            // Navigate to the constructed URL
            window.location.href = url;

            // Return false to prevent the default postback behavior
            return false;
        }

    </script>
     <%--   <script>
            function executeScript() {
                var itemId = '<%= Session["ItemID"] %>'; // ASP.NET syntax for getting the session variable
                var script = `
                document.getElementById('messageAR').classList.remove('hidden');
                document.getElementById('messageAR').classList.add('added');

                var like = document.getElementById('like_' + '${itemId}');
                if (like) {
                    like.style.color = 'red';
                }
            `;
                eval(script); // Execute the JavaScript code
            }
        </script>--%>
</asp:Content>
