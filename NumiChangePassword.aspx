<%@ Page Title="Alterar Password" Language="C#" MasterPageFile="~/Numismatic.Master" AutoEventWireup="true" CodeBehind="NumiChangePassword.aspx.cs" Inherits="ColecaoNumismatica.NumiChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="vh-100" style="background-color: #eee;">
  <div class="container flex h-100">
    <div class="row justify-content-center align-items-center h-100">
      <div class="col-lg-12 col-xl-11 flex">
        <div class="card text-black" style="border-radius: 25px;">
          <div class="card-body p-md-5">
            <div class="row justify-content-center">
              <div class="col-md-10 col-lg-6 col-xl-5 order-2 order-lg-1">
                <p class="text-center h1 fw-bold mb-5 mx-1 mx-md-4 mt-4">Alterar a Palavra-Passe</p>
                <div class="mx-1 mx-md-4">

                  <div class="d-flex flex-row align-items-center mb-4">
                    <i class="fa fa-lock fa-md me-3 fa-fw"></i>
                    <div class="form-outline flex-fill mb-0">
                      <asp:TextBox ID="tb_pw" runat="server" class="form-control" TextMode="Password" placeholder="Password Atual"></asp:TextBox>
                      <label class="form-label" for="tb_pw">Introduza a password atual</label>
                    </div>
                  </div>

                  <div class="d-flex flex-row align-items-center mb-4">
                    <i class="fa fa-key fa-md me-3 fa-fw"></i>
                    <div class="form-outline flex-fill mb-0">
                      <asp:TextBox ID="tb_pwn" runat="server" class="form-control" TextMode="Password" placeholder="Nova Password"></asp:TextBox>
                      <label class="form-label" for="tb_pwn">Introduza a nova password</label>
                    </div>
                  </div>

                  <div class="d-flex flex-row align-items-center mb-4">
                    <i class="fa fa-key fa-md me-3 fa-fw"></i>
                    <div class="form-outline flex-fill mb-0">
                      <asp:TextBox ID="tb_pwnr" runat="server" class="form-control" TextMode="Password" placeholder="Repita a Nova Password"></asp:TextBox>
                      <label class="form-label" for="tb_pwnr">Repita a nova password</label>
                    </div>
                  </div>


                  <div class="d-flex justify-content-center mx-4 mb-3 mb-lg-4">
                      <asp:Button ID="btn_alterarPw" runat="server" Text="Alterar PW" class="btn btn-primary btn-lg" OnClick="btn_alterarPw_Click"/>
                  </div>
                    <asp:Label ID="lbl_message" runat="server" Text=""></asp:Label>
                </div>

                </div>
              <div class="col-md-10 col-lg-6 col-xl-7 d-flex align-items-center order-1 order-lg-2">
                <img src="..\Images\NumiCoin.png" class="img-fluid" alt="Cinel Logo">
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</section>

</asp:Content>
