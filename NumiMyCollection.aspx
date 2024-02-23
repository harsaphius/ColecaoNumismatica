<%@ Page Title="My Collection" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiMyCollection.aspx.cs" Inherits="ColecaoNumismatica.NumiMyCollection" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
       
<div style="display:flex;justify-content:center;padding:20px;">
     <asp:Repeater ID="rpt_mycollection" runat="server" OnItemCommand="rpt_mycollection_ItemCommand">
        <HeaderTemplate>
            <div class="container">
                <div class="row">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="col-lg-3 col-sm-6 card" style="background-color:powderblue; height:350px; padding:15px;margin:2px;text-align:center;">
                      <div><b><asp:LinkButton href='<%# "NumiMoneyDetail.aspx?id=" + Eval("cod") %>' ID="lbl_titulo" runat="server" Text=""><%# Eval("titulo") %></asp:LinkButton></b><br /></div>
                      <div style="padding:5px;"><img src="<%# Eval("imagem") %>" style="width:150px;height:150px;"><br /><br /></div>
                      <div><asp:Label runat="server" ID="lbl_valorAtual"><b>Valor: </b><%# Eval("valorAtual") %></asp:Label>&nbsp;&nbsp;<b>Quantidade: </b><asp:Label runat="server" ID="lbl_quantidade"><%# Eval("quantidade") %></asp:Label><br /></div>
                      <div><b>Estado: </b>
                          <asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDStateMN" DataTextField="Estado" DataValueField="CodEstado" ></asp:DropDownList>
                          <asp:SqlDataSource ID="SQLDStateMN" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinState] ORDER BY CodEstado"></asp:SqlDataSource>

                      </div>
                      <div>
                          <asp:LinkButton runat="server" id="lbtn_plus" class="btn btn-mini" CommandName="lbtn_plus" CommandArgument='<%# Eval("cod") %>'><i id="plus" class="fa fa-plus fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                          <asp:LinkButton runat="server" id="lbtn_minus" class="btn btn-mini" CommandName="lbtn_minus" CommandArgument='<%# Eval("cod") %>'><i id="minus" class="fa fa-minus fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                      </div>
            </div>
        </ItemTemplate>

        <AlternatingItemTemplate>
            <div class="col-lg-3 col-sm-6 card" style="background-color:ghostwhite; height:350px; padding:15px; margin:2px;text-align:center">
              <div><b><asp:LinkButton href='<%# "NumiMoneyDetail.aspx?id=" + Eval("cod") %>' ID="LinkButton1" runat="server" Text=""><%# Eval("titulo") %></asp:LinkButton></b><br /></div>
                      <div style="padding:5px;"><img src="<%# Eval("imagem") %>" style="width:150px;height:150px;"><br /><br /></div>
                      <div><asp:Label runat="server" ID="lbl_valorAtual"><b>Valor: </b><%# Eval("valorAtual") %></asp:Label>&nbsp;&nbsp;<b>Quantidade: </b><asp:Label runat="server" ID="lbl_quantidade"><%# Eval("quantidade") %></asp:Label><br /></div>
                      <div><b>Estado:</b> <asp:DropDownList ID="ddl_estado" runat="server" DataSourceID="SQLDStateMN" DataTextField="Estado" DataValueField="CodEstado"></asp:DropDownList>
                        <asp:SqlDataSource ID="SQLDStateMN" runat="server" ConnectionString="<%$ ConnectionStrings:NumiCoinConnectionString %>" SelectCommand="SELECT * FROM [NumiCoinState] ORDER BY CodEstado"></asp:SqlDataSource></div>
                      <div>
                          <asp:LinkButton runat="server" id="LinkButton2" class="btn btn-mini" CommandName="lbtn_plus" CommandArgument='<%# Eval("cod") %>'><i id="plus" class="fa fa-plus fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                          <asp:LinkButton runat="server" id="LinkButton3" class="btn btn-mini" CommandName="lbtn_minus" CommandArgument='<%# Eval("cod") %>'><i id="minus" class="fa fa-minus fa-2x" aria-hidden="true" style="color:dodgerblue;"></i></asp:LinkButton>&nbsp;&nbsp;
                      </div>
            </div>
        </AlternatingItemTemplate>
        <FooterTemplate>
                        </div>
              </div>
        </FooterTemplate>
    </asp:Repeater>
     <asp:Button ID="btn_export" CssClass="btn btn-primary" style="justify-content:flex-end; height:fit-content;" runat="server" Text="Export Collection" OnClick="btn_export_Click"/><br /><br />

</div>
</asp:Content>
