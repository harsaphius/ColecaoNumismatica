<%@ Page Title="Main Page" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiMainPage.aspx.cs" Inherits="ColecaoNumismatica.NumiMainPage" EnableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
</asp:Content>
