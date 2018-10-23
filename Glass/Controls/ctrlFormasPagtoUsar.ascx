<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlFormasPagtoUsar.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlFormasPagtoUsar" %>

<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/ctrlFormasPagtoUsar.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

<script type="text/javascript">

    var formaPagtoPadrao = <%= FormaPagtoPadrao %>;

</script>

<table cellpadding="0" cellspacing="0">
    <tr>
        <td nowrap="nowrap">
            <asp:ImageButton ID="imgTooltip" runat="server" ImageUrl="~/Images/gear_add.gif" />
            <asp:CheckBoxList ID="cblFormasPagto" runat="server" DataSourceID="odsFormasPagto" Style="display: none"
                DataTextField="Descricao" DataValueField="IdFormaPagto" OnDataBound="CblFormasPagto_DataBound" OnPreRender="CblFormasPagto_PreRender"
                RepeatColumns="3">
            </asp:CheckBoxList>
        </td>
    </tr>
    <tr>
        <td style="padding-top: 3px">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td nowrap="nowrap">
                        Forma Pagto. Padrão
                    </td>
                    <td nowrap="nowrap" style="padding-left: 3px">
                        <asp:DropDownList ID="drpFormaPagto" runat="server"
                            DataSourceID="odsFormasPagto" DataTextField="Descricao"
                            DataValueField="IdFormaPagto" AppendDataBoundItems="True"
                            OnDataBound="DrpFormaPagto_DataBound">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormasPagto" runat="server" SelectMethod="GetForControle"
    TypeName="Glass.Data.DAL.FormaPagtoDAO">
</colo:VirtualObjectDataSource>
<asp:CustomValidator ID="ctvFormasPagtoUsar" runat="server" ClientValidationFunction="validarFormasPagtoUsar"
    Display="None"
    ErrorMessage="Selecione uma forma de pagamento para continuar."></asp:CustomValidator>
<asp:ValidationSummary ID="vsuFormasPagtoUsar" runat="server" ShowMessageBox="True"
    ShowSummary="False" />
