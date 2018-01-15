<%@ Page Title="Declaração de Cheques" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstDeclaracaoCheque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstDeclaracaoCheque" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function setChequeReceb(idCheque, numCheque, titular, banco, agencia, conta, valor, dataVenc, selChequeWin, idCliente) {

            // Verifica se o cheque já foi adicionado
            var cheques = FindControl("hdfIdsCheque", "input").value.split(',');
            for (i = 0; i < cheques.length; i++) {
                if (idCheque == cheques[i]) {
                    selChequeWin.alert("Cheque já incluído.");
                    return false;
                }
            }

            // Adiciona item à tabela
            addItem(new Array(numCheque, titular, banco, agencia, conta, valor, dataVenc),
                new Array('Número', 'Titular', 'Banco', 'Agência', 'Conta', 'Valor', 'Vencimento'),
                'lstCheque', idCheque, 'hdfIdsCheque');

            return false;
        }

        function selCheque() {
            var cliente = FindControl("txtNumCli", "input").value;

            return openWindow(600, 850, '../Utils/SelCheque.aspx?tipo=5&situacao=10&cliente=' + cliente)
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = Lst_LstDeclaracaoCheque.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }
        
        function openRpt(exportarExcel) {

            var cliente = FindControl("txtNumCli", "input").value;
            var texto = FindControl("txtTexto", "textarea").value;
            var cheques = FindControl("hdfIdsCheque", "input").value;

            if (cliente == "") {
                alert("Informe o cliente");
                return false;
            }
            if (texto == "") {
                alert("Informe o texto");
                return false;
            }
            if (cheques == "") {
                alert("Selecione os cheques");
                return false;
            }
            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=DeclaracaoCheque&cliente=" + cliente + "&texto=" + texto + "&cheques=" + cheques + "&exportarExcel=" + exportarExcel);

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnBuscarCheques" runat="server" Text="Buscar cheques" OnClientClick="selCheque(); return false;" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Texto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTexto" runat="server" Width="500px" TextMode="MultiLine" 
                                Rows="3">Como promessa de pagamento dos(s) pedido(s) de número(s) </asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td align="center" colspan="2">
                <table id="lstCheque" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" />Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfIdsCheque" runat="server" />
</asp:Content>
