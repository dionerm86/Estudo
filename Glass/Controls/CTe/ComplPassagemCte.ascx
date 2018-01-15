<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComplPassagemCte.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.ComplPassagemCte" %>
<div class="dtvRow">
    <%--<div class="dtvHeader">
        <asp:Label ID="lblNumSeqPassagem" runat="server" Text="Número Seq. Passagem *" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox runat="server" ID="txtNumSeqPassagem" Text='<%# Bind("ObjComplCte.ObjComplPassagemCte.NumSeqPassagem") %>'></asp:TextBox>        
        <asp:RequiredFieldValidator ID="rfvtxtNumSeqPassagem" runat="server" ErrorMessage="campo Número Seq. Passagem não pode ser vazio."
            ControlToValidate="txtNumSeqPassagem" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
    </div>--%>
    <div class="dtvHeader">
        <asp:Label ID="lblSiglaPassagem" runat="server" Text="Sigla Passagem" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox runat="server" ID="txtSiglaPassagem"></asp:TextBox>
    </div>
</div>
