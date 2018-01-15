<%@ Page Title="Gerar Nota Fiscal" Language="C#" MasterPageFile="~/Layout.master"
    AutoEventWireup="true" CodeBehind="SelNaturezaOperacaoGerarNf.aspx.cs" Inherits="Glass.UI.Web.Utils.SelNaturezaOperacaoGerarNf" %>

<%@ Register Src="../Controls/ctrlNaturezaOperacao.ascx" TagName="ctrlNaturezaOperacao"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Menu" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" runat="Server">

    <script type="text/javascript">

        function gerarNf(botao) {

            botao.disabled = true;

            var idCompra = GetQueryString("idCompra");
            var idNaturezaOperacao = FindControl("ctrlNaturezaOperacao_selNaturezaOperacao_hdfValor", "input").value;

            var response = SelNaturezaOperacaoGerarNf.GerarNf(idCompra, idNaturezaOperacao);

            if (response.error != null) {
                alert(response.error.description);
                botao.disabled = false;
                return false;
            }

            window.opener.notaGerada(response.value);
            window.close();
        }
        
    </script>

    <table width="100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Natureza de Operação:
                        </td>
                        <td>
                            <uc1:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server" PermitirVazio="True" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnGerarNf" runat="server" OnClientClick="gerarNf(this, ''); return false;"
                    Text="Gerar NF" />
            </td>
        </tr>
    </table>
</asp:Content>
