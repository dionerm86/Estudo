<%@ Page Title="Movimentação Interna de Estoque Fiscal" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadMovInternaEstoqueFiscal.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadMovInternaEstoqueFiscal" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        // Carrega dados do produto com base no código do produto passado
        function setProduto(cod, descr, codSelProd) {

            var controleCod = FindControl(cod, "input");
            var controleDescr = FindControl(descr, "input");

            var codInterno = codSelProd != null ? codSelProd : controleCod.value;

            if (codInterno == "")
                return false;

            try {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    controleCod.value = "";
                    controleDescr.value = "";
                    controleCod.focus();
                    return false;
                }

                if (codSelProd != null)
                    controleCod.value = codSelProd;

                controleDescr.value = retorno[2];

            }
            catch (err) {
                alert(err.value);
            }
        }


        function movimentarEstoque() {

            if (!confirm("Deseja realmente realizar a movimentação interna de estoque?"))
                return false;

            var codProdOrigem = FindControl("txtCodProdOrigem", "input").value;
            var codProdDestino = FindControl("txtCodProdDestino", "input").value;
            var qtdeOrigem = FindControl("txtQtdeOrigem", "input").value;
            var qtdeDestino = FindControl("txtQtdeDestino", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;

            var ret = CadMovInternaEstoqueFiscal.MovimentarEstoque(codProdOrigem, codProdDestino, qtdeOrigem, qtdeDestino, idLoja);

            if (ret.error != null) {
                alert(ret.error.description);
                return false;
            }

            alert("Movimentação realizada com sucesso.");
            location.reload();
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <table cellspacing="1" cellpadding="5" style="min-width: 300px;">
                                <tr class="dtvAlternatingRow">
                                    <td align="center" colspan="4"><b>Origem</b></td>
                                </tr>
                                <tr class="dtvAlternatingRow">
                                    <td class="dtvHeader">Cód.</td>
                                    <td valign="middle">
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtCodProdOrigem" runat="server" Width="60px" onblur="setProduto('txtCodProdOrigem', 'txtDescrProdOrigem');"></asp:TextBox></td>
                                                <td>&nbsp;
                                                    <asp:ImageButton ID="imgPesqProd" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                        OnClientClick="openWindow(600, 800, '../Utils/SelProd.aspx?callback=movInterna&cod=txtCodProdOrigem&descr=txtDescrProdOrigem'); return false;" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="dtvHeader">Descrição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescrProdOrigem" runat="server" ReadOnly="true" Width="300px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="dtvAlternatingRow">
                                    <td class="dtvHeader">Qtde.</td>
                                    <td colspan="3">
                                        <asp:TextBox ID="txtQtdeOrigem" runat="server" Width="60px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <asp:Image ID="imbAddPeca" ToolTip="Para" runat="server"
                                ImageUrl="~/Images/arrow_right.gif" /></td>
                        <td>
                            <table cellspacing="1" cellpadding="5" style="min-width: 300px;">
                                <tr class="dtvAlternatingRow">
                                    <td align="center" colspan="4"><b>Destino</b></td>
                                </tr>
                                <tr class="dtvAlternatingRow">
                                    <td class="dtvHeader">Cód.</td>
                                    <td valign="middle">
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtCodProdDestino" runat="server" Width="60px" onblur="setProduto('txtCodProdDestino', 'txtDescrProdDestino');"></asp:TextBox></td>
                                                <td>&nbsp;
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                        OnClientClick="openWindow(600, 800, '../Utils/SelProd.aspx?callback=movInterna&cod=txtCodProdDestino&descr=txtDescrProdDestino'); return false;" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td class="dtvHeader">Descrição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescrProdDestino" runat="server" ReadOnly="true" Width="300px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr class="dtvAlternatingRow">
                                    <td class="dtvHeader">Qtde.</td>
                                    <td colspan="3">
                                        <asp:TextBox ID="txtQtdeDestino" runat="server" Width="60px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <table cellspacing="1" cellpadding="5">
                    <tr class="dtvAlternatingRow">
                        <td class="dtvHeader">Loja</td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="false" MostrarTodas="false" Style="min-width:90px;" />
                        </td>
                    </tr>
                </table>

            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnMovimentarEstoque" runat="server" OnClientClick="return movimentarEstoque();"
                    Text="Movimentar Estoque" Width="150px" /></td>
        </tr>
    </table>

</asp:Content>
