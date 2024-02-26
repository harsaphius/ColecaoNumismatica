<%@ Page Title="My Collection" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiMyCollection.aspx.cs" Inherits="ColecaoNumismatica.NumiMyCollection" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="display:flex;justify-content:center;padding:5px;"> <asp:Label ID="lbl_message" runat="server" Text=""></asp:Label></div>
<div style="display:flex;justify-content:center;padding:20px;">
     <asp:Repeater ID="rpt_mycollection" runat="server" OnItemCommand="rpt_mycollection_ItemCommand">
        <HeaderTemplate>
            <div class="container">
                <div class="row">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="col-xl-2 col-lg-3 col-sm-6 card" style="background-color:powderblue; height:320px; min-width:280px; padding:15px;margin:2px;text-align:center;">
                      <div><b><asp:Label ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:Label></b><br /></div>
                      <div style="padding:5px;"><img src="<%# Eval("imagem") %>" style="width:150px;height:150px;"><br /><br /></div>
                      <div><asp:Label runat="server" ID="lbl_valorAtual"><b>Valor: </b><%# Eval("valorAtual") %></asp:Label>&nbsp;&nbsp;<b>Quantidade: </b><asp:Label runat="server" ID="lbl_quantidade"><%# Eval("quantidade") %></asp:Label><br /></div>
                      <div><b>Estado: </b>
                          <asp:Label ID="lbl_estado" runat="server" Text=""><%# Eval("estado") %></asp:Label>
                      </div>
                    <div style="display:flex; justify-content:center;">
                         <div><asp:LinkButton runat="server" id="lbtn_plus" class="btn btn-mini" CommandName="lbtn_plus" CommandArgument='<%# Eval("cod") %>' AutoPostBack="True"><i id="plus" class="fa fa-plus fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;</div>
                         <div><asp:LinkButton runat="server" id="lbtn_minus" class="btn btn-mini" CommandName="lbtn_minus" CommandArgument='<%# Eval("cod") %>' AutoPostBack="True"><i id="minus" class="fa fa-minus fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;</div>
                         <div><asp:LinkButton runat="server" id="lbt_remove" class="btn btn-mini" CommandName="lbtn_remove" CommandArgument='<%# Eval("cod") %>' AutoPostBack="True"><i id="remove" class="fa fa-trash fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;</div>
                    </div>
            </div>
        </ItemTemplate>

        <AlternatingItemTemplate>
            <div class="col-lg-3 col-sm-6 card" style="background-color:ghostwhite;height:320px; min-width:280px; padding:15px; margin:2px;text-align:center">
                      <div><b><asp:Label ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:Label></b><br /></div>
                      <div style="padding:5px;"><img src="<%# Eval("imagem") %>" style="width:150px;height:150px;"><br /><br /></div>
                      <div><asp:Label runat="server" ID="lbl_valorAtual"><b>Valor: </b><%# Eval("valorAtual") %></asp:Label>&nbsp;&nbsp;<b>Quantidade: </b><asp:Label runat="server" ID="lbl_quantidade"><%# Eval("quantidade") %></asp:Label><br /></div>
                      <div><b>Estado: </b>
                          <asp:Label ID="lbl_estado" runat="server" Text=""><%# Eval("estado") %></asp:Label></div>
                      <div style="display:flex; justify-content:center;">
                         <div><asp:LinkButton runat="server" id="lbtn_plus" class="btn btn-mini" CommandName="lbtn_plus" CommandArgument='<%# Eval("cod") %>' AutoPostBack="True"><i id="plus" class="fa fa-plus fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;</div>
                         <div><asp:LinkButton runat="server" id="lbtn_minus" class="btn btn-mini" CommandName="lbtn_minus" CommandArgument='<%# Eval("cod") %>' AutoPostBack="True"><i id="minus" class="fa fa-minus fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;</div>
                         <div><asp:LinkButton runat="server" id="lbt_remove" class="btn btn-mini" CommandName="lbtn_remove" CommandArgument='<%# Eval("cod") %>' AutoPostBack="True"><i id="remove" class="fa fa-trash fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;</div>
                    </div>
            </div>
        </AlternatingItemTemplate>
        <FooterTemplate>
                        </div>
              </div>
        </FooterTemplate>
    </asp:Repeater>
</div>
    <div style="display:flex;justify-content:center;padding:20px;"><asp:Button ID="btn_export" CssClass="btn btn-primary" style="justify-content:flex-end; height:fit-content;" runat="server" Text="Export Collection" OnClick="btn_export_Click"/><br /><br /></div>
   <div style="display: flex; justify-content: center;">
        <asp:LinkButton ID="lbtn_previous" runat="server" OnClick="lbtn_previous_Click">Previous</asp:LinkButton>&nbsp;&nbsp;
        <asp:Label ID="lbl_pageNumber" runat="server"></asp:Label>&nbsp;&nbsp;
        <asp:LinkButton ID="lbtn_next" runat="server" OnClick="lbtn_next_Click">Next</asp:LinkButton>
    </div>
</asp:Content>
