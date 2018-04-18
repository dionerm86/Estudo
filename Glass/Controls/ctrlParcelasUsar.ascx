<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlParcelasUsar.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlParcelasUsar" %>

<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/ctrlParcelasUsar.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

<table cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td nowrap="nowrap">
                        <asp:ImageButton ID="imgTooltip" runat="server" ImageUrl="~/Images/gear_add.gif" />
                        <asp:CheckBoxList ID="cblParcelas" runat="server" DataSourceID="odsParcelas" Style="display: none"
                            DataTextField="Name" DataValueField="Id" OnDataBound="cblParcelas_DataBound"
                            RepeatColumns="3">
                        </asp:CheckBoxList>
                    </td>
                    <td nowrap="nowrap" style="padding-left: 3px">
                        
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td style="padding-top: 3px">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td nowrap="nowrap">
                        Parcela Padrão
                    </td>
                    <td nowrap="nowrap" style="padding-left: 3px">
                        <asp:DropDownList ID="drpTipoPagto" runat="server" DataSourceID="odsParcelas" DataTextField="Name"
                            DataValueField="Id" AppendDataBoundItems="true" OnDataBound="drpTipoPagto_DataBound">
                            <asp:ListItem></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsParcelas" runat="server" SelectMethod="ObtemParcelas"
    TypeName="Glass.Financeiro.Negocios.IParcelasFluxo">
</colo:VirtualObjectDataSource>
<asp:CustomValidator ID="ctvParcelasUsar" runat="server" ClientValidationFunction="validarParcelasUsar"
    Display="None" ErrorMessage="Selecione uma parcela para continuar."></asp:CustomValidator>
<asp:ValidationSummary ID="vsuParcelasUsar" runat="server" ShowMessageBox="True"
    ShowSummary="False" />

<script type="text/javascript">
    
    <%
    for (int i = 0; i < cblParcelas.Items.Count; i++)
        Response.Write("habilitar(document.getElementById('" + cblParcelas.ClientID + "_" + i + "'), " + cblParcelas.Items[i].Value + ", " +
            "document.getElementById('" + drpTipoPagto.ClientID + "'));\n");

    if (FormaPagtoPadrao != null)
        Response.Write("document.getElementById('" + drpTipoPagto.ClientID + "').value = " + FormaPagtoPadrao.Value.ToString() + ";\n");
    %>
    
</script>

