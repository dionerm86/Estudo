<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlTextBoxFloat.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlTextBoxFloat" %>

    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

<script type="text/javascript">
    function isEmpty(val, args)
    {
        args.IsValid = args.Value != "";
    }
</script>
<asp:TextBox ID="txtNumber" runat="server" MaxLength="20" Width="80px" CssClass="caixatexto" OnKeyPress="return soNumeros(event, false, false);"></asp:TextBox>
<asp:RangeValidator ID="rgvTextBoxFloat" runat="server" ControlToValidate="txtNumber"
    ErrorMessage="Formato inválido." MaximumValue="9999999,99" MinimumValue="0"
    Type="Double" Display="Dynamic"></asp:RangeValidator>
<asp:CustomValidator ID="ctvTextBoxFloat"
    runat="server" ErrorMessage="*" ClientValidationFunction="isEmpty" 
    ControlToValidate="txtNumber" ValidateEmptyText="True" Visible="False" 
    Display="Dynamic"></asp:CustomValidator>