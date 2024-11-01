﻿<%@ Page Title="Main Page" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiMainPage.aspx.cs" Inherits="ColecaoNumismatica.NumiMainPage"  ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">  
<script>
    function changeColor(elementId) {
        var element = document.getElementById(elementId);
        if (element) {
            element.style.color = 'red'; // Change color to red
        }
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> 
<div><asp:Label ID="lbl_messageUser" runat="server" Text=""></asp:Label></div>
<div id="messageAR" class="hidden"><asp:Label ID="lbl_message" runat="server" Text=""></asp:Label></div>
<div style="display:flex;justify-content:center;padding:20px;">
     <asp:Repeater ID="rpt_mainpage" runat="server" OnItemCommand="rpt_mainpage_ItemCommand">
        <HeaderTemplate>
              <div class="container">
                        <div class="row">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="col-lg-3 col-sm-6 card" style="background-color:powderblue; height:250px; padding:15px; margin:2px;text-align:center">
                      <div><b><asp:LinkButton href='<%# "NumiMoneyDetail.aspx?id=" + Eval("cod") %>' ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:LinkButton> </b></div><br />
                      <div style="padding:5px;"><img src="<%# Eval("imagem") %>" style="width:150px;height:150px;"><br /></div>
                      <%--<div><%# Eval("valorCunho") %></div>--%>
                      <div>
                          <asp:LinkButton runat="server" id="lbtn_like" class="btn btn-mini" CommandName="like" CommandArgument='<%# Eval("cod") %>'><i id="like" class="fa fa-heart fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                          <asp:LinkButton runat="server" id="lbtn_dislike" class="btn btn-mini" CommandName="dislike" CommandArgument='<%# Eval("cod") %>'><i id="dislike" class="fa fa-thumbs-down fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                      </div>
            </div>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <div class="col-lg-3 col-sm-6 card" style="background-color:ghostwhite; height:250px; padding:15px; margin:2px;text-align:center">
                     <div><b><asp:LinkButton href='<%# "NumiMoneyDetail.aspx?id=" + Eval("cod") %>' ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:LinkButton> </b></div><br />
                      <div style="padding:5px;"><img src="<%# Eval("imagem") %>" style="width:150px;height:150px;"><br /></div>
                      <%--<div><%# Eval("valorCunho") %></div>--%>
                      <div>
                          <asp:LinkButton runat="server" id="lbtn_like" class="btn btn-mini" CommandName="like" CommandArgument='<%# Eval("cod") %>'><i id="like" class="fa fa-heart fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                          <asp:LinkButton runat="server" id="lbtn_dislike" class="btn btn-mini" CommandName="dislike" CommandArgument='<%# Eval("cod") %>'><i id="dislike" class="fa fa-thumbs-down fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                      </div>
            </div>
        </AlternatingItemTemplate>
        <FooterTemplate>
                        </div>
              </div>
        </FooterTemplate>
    </asp:Repeater>
</div>

<!--Modal -->
<div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true" style="justify-content:center;">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="exampleModalLabel">Qual é o estado da moeda?</h5>
        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button>
      </div>
      <div class="modal-body">
        <div>
         <div class="form-group">
             <label for="ddl_estado" class="col-form-label">Estado:</label>
             <asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDStateMN" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList>
             <asp:SqlDataSource ID="SQLDStateMN" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinState] ORDER BY CodEstado"></asp:SqlDataSource>
             <br />
             <label for="tb_quantidade" class="col-form-label">Estado:</label>
             <asp:TextBox ID="tb_quantidade" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
         </div>
        </div>
      </div>
      <div class="modal-footer">
        <asp:Button class="btn btn-secondary" ID="btn_close" runat="server" Text="Fechar" />
        <asp:Button class="btn btn-primary" ID="btn_add" runat="server" Text="Adicionar" OnClick="btn_add_Click"/>
        <asp:Button class="btn btn-primary" ID="Button1" runat="server" Text="Remover" OnClick="btn_remove_Click"/>
      </div>
    </div>
  </div>
</div>

</asp:Content>
