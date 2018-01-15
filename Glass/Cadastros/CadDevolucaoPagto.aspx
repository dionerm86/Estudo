<%@ Page Title="Devolução de Pagamento" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadDevolucaoPagto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDevolucaoPagto" %>

<%@ Register src="../Controls/ctrlFormaPagto.ascx" tagname="ctrlFormaPagto" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        function queryStringCheques()
        {
            return "situacao=1";
        }

        function devolver()
        {
            if (!validate())
                return false;

            if (!confirm("Devolver pagamento?"))
                return false;

            return true;
        }
        
        function getUrlCheques(tipoPagto, urlPadrao)
        {
            return tipoPagto == 2 ? "CadChequePagto.aspx" : "CadChequePagtoTerc.aspx";
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <uc1:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" ExibirCliente="True" 
                    ExibirGerarCredito="False" ExibirJuros="False" ExibirRecebParcial="False" 
                    ExibirValorAPagar="False" TextoValorReceb="Valor a ser Devolvido" 
                    FuncaoUrlCheques="getUrlCheques" FuncaoQueryStringCheques="queryStringCheques"
                    ExibirValorRestante="false"
                    CobrarJurosCartaoClientes="False" BloquearCamposContaVazia="false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Observação
                        </td>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Rows="4" TextMode="MultiLine" 
                                Width="350px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:Button ID="btnOk" runat="server" Text="Devolver pagamento" 
                    OnClientClick='if (!devolver()) return false' 
                    onclick="btnOk_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Voltar" 
                    onclick="btnCancelar_Click" />
            </td>
        </tr>
    </table>
</asp:Content>

