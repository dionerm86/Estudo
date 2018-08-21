<%@ Page Title="Importação de Nota Fiscal Eletrônica de Entrada de Terceiros" Language="C#"
    MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadImportarNFEEntrada.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadImportarNFEEntrada" %>

<%@ Register Src="../Controls/ctrlNaturezaOperacao.ascx" TagName="ctrlNaturezaOperacao" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagtoNotaFiscal.ascx" TagName="ctrlFormaPagtoNotaFiscal" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        var prodInFocus;

        function setProductFocus(codHdf) {
            prodInFocus = codHdf;
        }

        function associarProduto(codFornec) {
            var labelName = "lblProdAssoc" + prodInFocus.toString();
            var lblcodName = "lblCodProdAssoc" + prodInFocus.toString();
            var hdfCodName = "hdfCodProd" + prodInFocus.toString();
            var idFornec = FindControl("hdfIdFornec", "input").value;

            var tbxName = ("tbxProdCodAssoc" + prodInFocus.toString());
            var prodCodDigitado = FindControl(tbxName, "input").value;

            if (prodCodDigitado == "") {
                return false;
            }

            var prodAssociado = CadImportarNFEEntrada.AssociaProduto(prodCodDigitado, codFornec, idFornec).value;

            if (prodAssociado == "#ERRO#") {
                FindControl(hdfCodName, "input").value = "";
                FindControl(lblcodName, "span").innerHTML = "";

                FindControl(labelName, "span").innerHTML = "<br/>ERRO: Código de produto não encontrado. Tente novamente.";
                FindControl(labelName, "span").style.color = "red";

                return false;
            }
            else {
                var retorno = prodAssociado.split('#');

                FindControl(hdfCodName, "input").value = retorno[0];

                FindControl(labelName, "span").innerHTML = "<br/>Associado ao item:&nbsp;";
                FindControl(labelName, "span").style.color = "blue";

                FindControl(lblcodName, "span").innerHTML = retorno[0] + " - " + retorno[1];
            }
        }

        function setProdNatOp(ctrlNatOp) {
            var codNaturezaOperacao = FindControl("hdfValor", "input", ctrlNatOp.parentNode.parentNode.parentNode.parentNode.parentNode).value;

            var hdfNatOpProd = ("hdfNatOpProd" + prodInFocus.toString());
            FindControl(hdfNatOpProd, "input").value = codNaturezaOperacao.toString();

            var cfopProdAssoc = (CadImportarNFEEntrada.SetNaturezaOperacaoProduto(codNaturezaOperacao, prodInFocus.toString())).value;
        }

        function setProduto(codInterno) {
            FindControl("tbxProdCodAssoc" + prodInFocus.toString(), "input").value = codInterno;

            associarProduto(FindControl("hdfCodFornecProd" + prodInFocus.toString(), "input").value);
        }

        function validarAssociacoes(numProd) {

            if (!validate())
                return false;

            for (var i = 0; i < numProd; i++) {
                var hdfName = ("hdfCodProd" + i.toString());

                if (FindControl(hdfName, "input").value === "") {
                    alert("Há produtos sem associação. Confira acima e associe todos os produtos da NFe.");
                    return false;
                }
            }

            // Valida as formas de pagamento da nota importada
            var valoresRecebidos = FindControl("ctrlFormaPagtoNotaFiscal_hdfValoreReceb", "input").value.split(';');
            for (var i = 0; i < valoresRecebidos.length; i++) {
                if (valoresRecebidos[i] == "") {
                    alert("Informe os valores da forma de pagamento.");
                    return false;
                }
            }

            var tipoPlanoConta = FindControl("drpPlanoConta", "select").value;

            if (tipoPlanoConta == "") {
                alert("Selecione o plano de conta.");
                return false;
            }

            return true;
        }

        function exibeParcelas() {
            FindControl("hdfNumParc", "input").value = FindControl("txtNumParc", "input").value;
            Parc_visibilidadeParcelas("ctl00_ctl00_Pagina_Conteudo_ctrlParcelas1");
        }
        
    </script>

    <table>
        <tr>
            <td colspan="2">
                Selecione o arquivo da Nota Fiscal Eletrônica<br />
                <asp:FileUpload ID="fupNFEUpload" runat="server" Height="24px" />
                &nbsp;<asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Enviar"
                    Width="100px" Height="24px" />
                <br />
                <asp:RegularExpressionValidator ID="revUploadFileTypeCheck" runat="server" ErrorMessage="Apenas arquivos do tipo '.xml' são permitidos."
                    ValidationExpression="^.*\.(xml|XML|Xml)$" ControlToValidate="fupNFEUpload"></asp:RegularExpressionValidator>
                <br />
                <asp:PlaceHolder ID="plcResultados" runat="server"></asp:PlaceHolder>
                <br />
                <br />
                <asp:PlaceHolder ID="plcInformacao" runat="server"></asp:PlaceHolder>
                <br />
                <br />
                <asp:PlaceHolder ID="plcProdutos" runat="server"></asp:PlaceHolder>
                <br />
                <br />
                <asp:PlaceHolder ID="plcValores" runat="server"></asp:PlaceHolder>
                <br />
                <br />
                <asp:PlaceHolder ID="plcParcelas" runat="server"></asp:PlaceHolder>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblNumParc" runat="server" Text="Parc." Visible="false"></asp:Label>
                <asp:TextBox ID="txtNumParc" runat="server" Width="40px" Visible="false"
                    onkeypress="exibeParcelas(); return soNumeros(event, true, true);" onblur="exibeParcelas();"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td runat="server" id="parc2">
                <uc3:ctrlParcelas ID="ctrlParcelas1" runat="server" ExibirCampoAdicional="True" NumParcelas="10"
                    NumParcelasLinha="5" TituloCampoAdicional="Boleto:" OnLoad="ctrlParcelas1_Load" />
                <asp:HiddenField runat="server" ID="hdfTotalParcelas" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <uc2:ctrlFormaPagtoNotaFiscal ID="ctrlFormaPagtoNotaFiscal" runat="server" EnableViewState="true" />
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblPlanoConta" runat="server" Text="Plano de Conta:" Enabled="False"
                    Visible="False"></asp:Label>
            </td>
            <td align="left">
                <asp:DropDownList ID="drpPlanoConta" runat="server" DataSourceID="odsPlanoConta"
                    DataTextField="DescrPlanoGrupo" DataValueField="IdConta" Height="20px" Width="400px"
                    AppendDataBoundItems="True" OnDataBound="drpPlanoConta_DataBound">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblNaturezaOperacao" runat="server" Text="Natureza da Operação:" Enabled="False"
                    Visible="False"></asp:Label>
            </td>
            <td align="left">
                <uc1:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server" PermitirVazio="false" 
                ErrorMessage="A Natureza da Operação da Nf-e não foi informada."/>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblNumCompra" runat="server" Text="Número Compra:" Visible="false" />
            </td>
            <td align="left">
                <asp:TextBox ID="txtNumCompra" runat="server" Width="125px" Visible="false" onKeyPress="return soNumeros(event, true, true);" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center" style="height: 49px">
                <asp:Label ID="lblImportar" runat="server" Enabled="False" Text="Importar Nota Fiscal Eletrônica: "
                    Visible="False"></asp:Label>
                &nbsp;<asp:Button ID="btnImportar" runat="server" Enabled="False" Height="24px" OnClick="btnImportar_Click"
                    Text="Importar" Visible="False" OnClientClick="return importar();" />
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="left" style="height: 75px">
                <asp:Label ID="lblInfoNFE" runat="server"></asp:Label>
                <br />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContas"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfNumProdutos" runat="server" />
                <asp:HiddenField ID="hdfChaveAcesso" runat="server" />
                <asp:HiddenField ID="hdfExibirParcelas" runat="server" Value="true" />
                <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="false" />
                <asp:HiddenField ID="hdfNumParc" runat="server"/>
                <asp:HiddenField ID="hdfTotal" runat="server" />
                <asp:ValidationSummary runat="server" ID="vsNatOp"  ShowMessageBox="true" ShowSummary="false"/>
            </td>
        </tr>
    </table>

</asp:Content>
