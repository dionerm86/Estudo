<%@ Page Title="Gerar Arquivo FCI" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadArquivoFCI.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadArquivoFCI" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function baixarArquivo() {

            var tbProd = FindControl("lstProd", "table");

            var dados = "";

            for (var i = 1; i < tbProd.rows.length; i++) {

                var idProd = tbProd.rows[i].getAttribute("objid");
                var percentualImportacao = tbProd.rows[i].cells[2].getElementsByTagName("input")[0].value;
                var saidaInterestadual = tbProd.rows[i].cells[3].getElementsByTagName("input")[0].value;
                var conteudoImportacao = tbProd.rows[i].cells[4].getElementsByTagName("input")[0].value;
                var idProdNf = tbProd.rows[i].cells[5].getElementsByTagName("input")[0].value;

                dados += idProd + ";" + percentualImportacao + ";" + saidaInterestadual + ";" + conteudoImportacao + ";" + idProdNf + "$";
            }

            var retorno = CadArquivoFCI.GeraArquivoFci(dados);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return false;
            }

            window.location.href = "../Handlers/Fiscal.ashx?tipo=FCI" + "&idArquivoFci=" + retorno.value;
        }

        function getProduto(codProd) {
            if (codProd.value == "")
                return;

            var retorno = MetodosAjax.GetProd(codProd.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                codProd.value = "";
                FindControl("txtDescrProd", "input").value = "";
                return false;
            }

            FindControl("txtDescrProd", "input").value = retorno[2];
            FindControl("hdfIdProd", "input").value = retorno[1];
        }

        function addProd(idProduto, idProdNf, alertaProdAdd) {

            if (idProdNf == 0) {
                document.getElementById("<%= ctrlSelNotaFiscal.ClientID %>_txtDescr").readOnly = true;
                document.getElementById("<%= ctrlSelNotaFiscal.ClientID %>_txtDescr").style.background = "#F0F0F0";

                //document.getElementById("<%= ctrlSelNotaFiscal.ClientID %>_imgPesq").style.display = 'none';
                FindControl("btnBuscarProdutosNf", "input").style.display = "none";
            }

            var idsProd = FindControl("hdfIdsProd", "input");

            if (idsProd == null || idProduto == "")
                return false;

            var retorno = CadArquivoFCI.AddProduto(idProdNf, idProduto, idsProd.value, alertaProdAdd);

            if (retorno.error != null) {
                if (retorno.error.description != "")
                    alert(retorno.error.description);

                FindControl("txtCodProd", "input").value = "";
                FindControl("txtDescrProd", "input").value = "";
                FindControl("hdfIdProd", "input").value = "";

                return false;
            }

            var arrRetorno = retorno.value.split(';');

            var idProd = arrRetorno[0];
            var prod = arrRetorno[1];
            var parcelaImportada = arrRetorno[2];
            var saidaInterestadual = arrRetorno[3];
            var conteudoImportacao = arrRetorno[4];

            var inputParcelaImportada = "<input name='txtParcelaImportada_" + idProd + "' type='text' id='txtParcelaImportada_" + idProd + "' " +
                " value='" + parcelaImportada + "' style='width: 55px' onkeypress='return soNumeros(event, true, true);' />";

            var inputSaidaInterestadual = "<input name='txtParcelaImportada_" + idProd + "' type='text' id='txtParcelaImportada_" + idProd + "' " +
                " value='" + saidaInterestadual + "' style='width: 55px' onkeypress='return soNumeros(event, true, true);' />";

            var inputConteudoImportacao = "<input name='txtParcelaImportada_" + idProd + "' type='text' id='txtParcelaImportada_" + idProd + "' " +
                " value='" + conteudoImportacao + "' style='width: 55px' onkeypress='return soNumeros(event, true, true);' />";

            var hdfIdProdNf = "<input type='hidden' name='hdfIdProdNf_" + idProd + "' id='hdfIdProdNf_" + idProd + "' " +
                " value='" + idProdNf + "' />";


            addItem(new Array(prod, inputParcelaImportada, inputSaidaInterestadual, inputConteudoImportacao, hdfIdProdNf),
                new Array('Produto', 'Parcela Importada', 'Saída Interestadual', 'Conteúdo de Importação', ''),
                'lstProd', idProd, 'hdfIdsProd', null, null, 'callbackAddProd', true);

            FindControl("txtCodProd", "input").value = "";
            FindControl("txtDescrProd", "input").value = "";
            FindControl("hdfIdProd", "input").value = "";

            mostrarBotaoGerarArquivo();
        }

        function mostrarBotaoGerarArquivo() {

            var idsProd = FindControl("hdfIdsProd", "input");
            var btnBaixar = FindControl("btnBaixar", "input");


            if (idsProd == null || btnBaixar == null)
                return;

            btnBaixar.style.display = idsProd.value == "" ? "none" : "block";
        }

        function callbackAddProd(control) {
            control.parentNode.removeChild(control);
            mostrarBotaoGerarArquivo();
        }

        function addProdNf() {

            FindControl("txtCodProd", "input").readOnly = true;
            FindControl("txtCodProd", "input").style.background = "#F0F0F0";

            FindControl("txtDescrProd", "input").readOnly = true;
            FindControl("txtDescrProd", "input").style.background = "#F0F0F0";

            FindControl("imgAddProd", "input").style.display = 'none';
            
            //var idNota = document.getElementById(" ctrlSelNotaFiscal.ClientID _hdfValor").value;
            var numeroNFe = document.getElementById("<%= ctrlSelNotaFiscal.ClientID %>_txtDescr").value;

            var retorno = CadArquivoFCI.BuscaProdutosNf(numeroNFe);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return false;
            }

            var prods = retorno.value.split(',');

            for (var i = 0; i < prods.length; i++) {
                addProd(prods[i].split(';')[0], prods[i].split(';')[1], false);
            }
        }
    </script>

    <table style="width: 100%">
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Nota Fiscal: " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                                 <uc1:ctrlSelPopup ID="ctrlSelNotaFiscal" runat="server" DataSourceID="odsNotaFiscal" 
                                    DataTextField="NumeroNFe" DataValueField="IdNf" 
                                    Descricao='<%# Eval("NumeroNFe") %>' PermitirVazio="True" TextWidth="133px" 
                                    TituloTela="Selecione a nota fiscal" Valor='<%# Bind("IdNf") %>' 
                                    ColunasExibirPopup="NumeroNFe|NomeDestRem" TitulosColunas="Número NF-e|Destinatario"
                                 UsarValorRealControle="True" ExibirIdPopup="false"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="btnBuscarProdutosNf" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addProdNf(); return false;"
                                ToolTip="Buscar Produtos" Width="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label21" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="getProduto(this);"></asp:TextBox>
                            <asp:TextBox ID="txtDescrProd" runat="server" Width="200px" ReadOnly="true"></asp:TextBox>
                            <asp:HiddenField ID="hdfIdProd" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgAddProd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addProd(document.getElementById('ctl00_ctl00_Pagina_Conteudo_hdfIdProd').value, 0, true); return false;"
                                ToolTip="Adicionar Produto" Width="16px" />
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
                <table id="lstProd" align="center" cellpadding="4" cellspacing="0">
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnBaixar" runat="server" Text="Gerar arquivo" OnClientClick="baixarArquivo(); return false"
                    Style="display: none;" />
            </td>
        </tr>
    </table>
    <sync:ObjectDataSource ID="odsNotaFiscal" runat="server" SelectMethod="ObterListaNotasEnvioFCI"
        TypeName="Glass.Data.DAL.NotaFiscalDAO" UseDAOInstance="True">
        <SelectParameters>
            <asp:Parameter DefaultValue="0" Name="idNf" Type="UInt32" />
        </SelectParameters>
    </sync:ObjectDataSource>
    <asp:HiddenField ID="hdfIdsProd" runat="server" />
</asp:Content>
