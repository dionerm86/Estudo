<%@ Page Title="Gerar Arquivo de Remessa" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="GerarArquivoRemessa.aspx.cs" Inherits="Glass.UI.Web.Utils.GerarArquivoRemessa" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlDadosCnab.ascx" TagName="ctrlDadosCnab" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function buscarContas() {
            if (!validate("filtro"))
                return false;

            var tipoPeriodo = FindControl("drpTipoPeriodo", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var tiposConta = FindControl("cbdTipo", "select").itens();
            var formasPagto = FindControl("cbdFormaPagto", "select").itens();
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var loja = FindControl("drpFiltroLoja", "select").value;
            var tipoContaSemSeparacao = FindControl("drpTipoContaSemSeparacao", "select").value;
            var idContaBancoCliente = FindControl("drpContaCliente", "select").value;
            var incluirContasAcertoParcial = FindControl("chkIncluirContasAcertoParcial", "input").checked;
            var incluirContasAntecipacaoBoleto = FindControl("chkIncluirContasAntecipacaoBoleto", "input").checked;

            var contas = GerarArquivoRemessa.GetContas(tipoPeriodo, dataIni, dataFim, tiposConta, tipoContaSemSeparacao, formasPagto,
                idCliente, nomeCliente, loja, idContaBancoCliente, "", incluirContasAcertoParcial, incluirContasAntecipacaoBoleto).value.split("\n");

            if (contas == null || contas.length == 0 || (contas.length == 1 && contas[0] == "")) {
                alert("Não há contas a receber disponíveis para geração do arquivo de remessa.");
                return false;
            }

            for (j = 0; j < contas.length; j++) {
                if (contas[j] == "")
                    continue;

                var dadosConta = contas[j].split("\t");
                addConta(dadosConta[0], dadosConta[1], dadosConta[10], dadosConta[2], dadosConta[3], dadosConta[4], dadosConta[5], dadosConta[6], dadosConta[7],
                dadosConta[8], dadosConta[9], false);
            }

            carregarDadosBanco();
        }

        function addConta(idConta, ref, parc, cliente, dataCad, valorVenc, dataVenc, tipo, formaPagto, loja, obs, load) {
            var ids = getIdsContasReceber();
            for (k = 0; k < ids.length; k++)
                if (ids[k] == idConta && !load)
                    return;

            var titulos = new Array("Ref.", "Parc.", "Cliente", "Data Cad.", "Valor Venc.", "Data Venc.", "Tipo", "Forma Pagto.", "Loja", "Obs.");
            var itens = new Array(ref, parc, cliente, dataCad, valorVenc, dataVenc, tipo, formaPagto, loja, obs);

            addItem(itens, titulos, "tbContasReceber", idConta, "hdfIdsContas", null, null, "permiteGerar", false);

            if (!load)
                permiteGerar();
        }

        function permiteGerar() {

            var qtdeContas = getIdsContasReceber().length;

            document.getElementById("gerarArquivo").style.display = qtdeContas > 0 ? "" : "none";
            document.getElementById("lblQtdeContas").style.display = qtdeContas > 0 ? "" : "none";
            document.getElementById("lblTextoQtdeContas").style.display = qtdeContas > 0 ? "" : "none";
            document.getElementById("lblQtdeContas").innerText = qtdeContas;
        }

        function carregarDadosBanco() {

            var idLojaFiltro = FindControl("drpFiltroLoja", "select").value;

            FindControl("drpLojaGerar", "select", document.getElementById("gerarArquivo")).value = idLojaFiltro;

            var idContaBanco = 0;

            var idContaCliente = FindControl("drpContaCliente", "select").value;

            if (idContaCliente > 0) {
                idContaBanco = idContaCliente;
            }
            else {

                var retorno = GerarArquivoRemessa.GetContaBanco(idLojaFiltro);

                if (retorno.error != null) {
                    alert(retorno.error.description);
                    return;
                }

                idContaBanco = retorno.value;
            }

            if (idContaBanco > 0) {
                $('[id*="drpContaBanco"]').val(idContaBanco);
                $('[id*="drpContaBanco"]').change();
            }
        }

        function getIdsContasReceber() {
            var ids = FindControl("hdfIdsContas", "input").value.split(',');
            if (ids.length > 0 && ids[ids.length - 1] == "")
                ids.pop();

            return ids;
        }

        function validaTipo(val, args) {
            var tiposConta = FindControl("cbdTipo", "select").itens();
            args.IsValid = tiposConta != "";
        }

        function validaFormaPagto(val, args) {
            var formasPagto = FindControl("cbdFormaPagto", "select").itens();
            args.IsValid = formasPagto != "";
        }

        function drpContaBancoChange(controle) {

            var retorno = GerarArquivoRemessa.GetNumArquivoRemessa(controle.value);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }

            FindControl("txtNumRemessa", "input").value = retorno.value;

            retorno = GerarArquivoRemessa.GetCodBanco(controle.value);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }

            FindControl("ctrlDadosCnab_hdfCodBanco", "input").value = retorno.value;
        }

        function drpContaClienteChange(controle) {

            var retorno = GerarArquivoRemessa.GetIdLoja(controle.value);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }

            FindControl("drpFiltroLoja", "select").value = retorno.value;
        }

        function limpar() {

            var tipoPeriodo = FindControl("drpTipoPeriodo", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var tipoContabil = FindControl("cbdTipo", "select").itens();
            var formaPagto = FindControl("cbdFormaPagto", "select").itens();
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idLoja = FindControl("drpFiltroLoja", "select").value;

            var url = window.location.href.indexOf("?") > -1 ? window.location.href.substring(0, window.location.href.indexOf("?")) : window.location.href;

            window.location.href = url + "?tipoPeriodo=" + tipoPeriodo + "&dataIni=" + dataIni.replace("/", "-").replace("/", "-") + "&dataFim=" + dataFim.replace("/", "-").replace("/", "-") + "&tipoContabil=" + tipoContabil +
                "&formaPagto=" + formaPagto + "&idCli=" + idCli + "&nomeCli=" + nomeCli + "&idLoja=" + idLoja;
        }

        //Busca os dados da URL
        $(document).ready(function () {

            FindControl("drpTipoPeriodo", "select").value = GetQueryString("tipoPeriodo") != null ? GetQueryString("tipoPeriodo") : "";
            FindControl("ctrlDataIni_txtData", "input").value = GetQueryString("dataIni") != null ? GetQueryString("dataIni").replace("-", "/").replace("-", "/") : "";
            FindControl("ctrlDataFim_txtData", "input").value = GetQueryString("dataFim") != null ? GetQueryString("dataFim").replace("-", "/").replace("-", "/") : "";

            if (GetQueryString("tipoContabil") != null) {
                var atribuiTipo = function () {
                    var tipo = FindControl("cbdTipo", "select");
                    if (tipo["itens"])
                        tipo.itens(GetQueryString("tipoContabil"));
                    else
                        setTimeout(atribuiTipo, 100);
                };

                atribuiTipo();
            }

            if (GetQueryString("formaPagto") != null) {
                var atribuiFormaPagto = function () {
                    var formaPagto = FindControl("cbdFormaPagto", "select");
                    if (formaPagto["itens"])
                        formaPagto.itens(GetQueryString("formaPagto"));
                    else
                        setTimeout(atribuiFormaPagto, 100);
                };

                atribuiFormaPagto();
            }

            FindControl("txtNumCli", "input").value = GetQueryString("idCli") != null ? GetQueryString("idCli") : "";
            FindControl("txtNome", "input").value = GetQueryString("nomeCli") != null ? GetQueryString("nomeCli").replace(/\%20/g, " ") : "";
            FindControl("drpFiltroLoja", "select").value = GetQueryString("idLoja") != null ? GetQueryString("idLoja") : "";


            var ids = FindControl("hdfIdsContas", "input").value;

            if (ids != "") {

                var retorno = GerarArquivoRemessa.GetContas("", "", "", "", "", "", "", "", "", "", ids, "", "");

                if (retorno.error != null) {
                    alert(retorno.error.description);
                    return false;
                }

                var contas = retorno.value.split("\n");

                FindControl("hdfIdsContas", "input").value = "";

                for (j = 0; j < contas.length; j++) {
                    if (contas[j] == "")
                        continue;   

                    var dadosConta = contas[j].split("\t");
                    addConta(dadosConta[0], dadosConta[1], dadosConta[10], dadosConta[2], dadosConta[3], dadosConta[4], dadosConta[5], dadosConta[6], dadosConta[7],
                    dadosConta[8], dadosConta[9], true);
                }
            }
        });

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center" id="tdFiltro">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Tipo de período"
                                AssociatedControlID="drpTipoPeriodo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoPeriodo" runat="server">
                                <asp:ListItem Value="0">Vencimento</asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">Cadastro</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Período" ForeColor="#0066FF" AssociatedControlID="ctrlDataIni"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ValidateEmptyText="True" ValidationGroup="filtro" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ValidateEmptyText="True" ValidationGroup="filtro" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label24" runat="server" ForeColor="#0066FF" Text="Tipo" AssociatedControlID="cbdTipo"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="cbdTipo" Title="Selecione o tipo de conta"
                                DataSourceID="odsTiposContas" DataValueField="Id" DataTextField="Descr" OnDataBound="cbdTipo_DataBound">
                            </sync:CheckBoxListDropDown>
                            <asp:CustomValidator runat="server" ID="ctvTipo" ClientValidationFunction="validaTipo"
                                Display="Dynamic" ErrorMessage="*" ValidationGroup="filtro"></asp:CustomValidator>
                            <colo:VirtualObjectDataSource ID="odsTiposContas" runat="server" CacheExpirationPolicy="Absolute"
                                ConflictDetection="OverwriteChanges" Culture="" MaximumRowsParameterName="" SelectMethod="ObtemTiposContas"
                                SkinID="" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.ContasReceberDAO">
                            </colo:VirtualObjectDataSource>
                            <script type="text/javascript">
                                ValidatorHookupControl(FindControl("cbdTipo", "select"), FindControl("ctvTipo", "span"));
                            </script>

                        </td>
                        <td>
                            <asp:Label ID="lblTipoContaSemSeparacao" runat="server" ForeColor="#0066FF" Text="Tipo" AssociatedControlID="cbdTipo"></asp:Label>

                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoContaSemSeparacao" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">CN</asp:ListItem>
                                <asp:ListItem Value="2">SN</asp:ListItem>
                            </asp:DropDownList>

                        </td>
                        <td>
                            <asp:Label ID="Label25" runat="server" ForeColor="#0066FF" Text="Forma de Pagto."
                                AssociatedControlID="cbdFormaPagto"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="cbdFormaPagto" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto" Title="Selecione a forma de pagamento" OnDataBound="cbdFormaPagto_DataBound">
                            </sync:CheckBoxListDropDown>
                            <asp:CustomValidator runat="server" ID="ctvFormaPagto" ClientValidationFunction="validaFormaPagto"
                                Display="Dynamic" ErrorMessage="*" ValidationGroup="filtro"></asp:CustomValidator>
                            <colo:VirtualObjectDataSource ID="odsFormaPagto" runat="server" CacheExpirationPolicy="Absolute"
                                ConflictDetection="OverwriteChanges" Culture="" MaximumRowsParameterName="" SelectMethod="GetForCnab"
                                SkinID="" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.FormaPagtoDAO">
                            </colo:VirtualObjectDataSource>
                            <script type="text/javascript">
                                ValidatorHookupControl(FindControl("cbdFormaPagto", "select"), FindControl("ctvFormaPagto", "span"));
                            </script>

                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblIdContaBancoCli" runat="server" ForeColor="#0066FF" Text="Conta Bancária do Cliente: "></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaCliente" runat="server" DataSourceID="odsContaBancoCliente"
                                DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="True"
                                onchange="drpContaClienteChange(this);">
                                <asp:ListItem Text="" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBancoCliente" runat="server" SelectMethod="ObterBancoAgrupado"
                                TypeName="Glass.Data.DAL.ContaBancoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label26" ForeColor="#0066FF" runat="server" Text="Cliente" AssociatedControlID="txtNumCli"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(false);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpFiltroLoja" AutoPostBack="false" MostrarTodas="true" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkIncluirContasAcertoParcial" runat="server" Text="Incluir contas de Acerto Parcial" Checked="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkIncluirContasAntecipacaoBoleto" runat="server" Text="Incluir contas de Antecipação de Boleto" Checked="true" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar contas" OnClientClick="buscarContas(); return false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table id="tbContasReceber">
    </table>
    <br />
    <table id="gerarArquivo">
        <tr>
            <td align="center">
                <table>
                    <tr align="center">
                        <td>
                            <asp:Label ID="lblTextoQtdeContas" runat="server" ClientIDMode="Static" Text="Contas a serem enviadas: " ForeColor="Black" Font-Bold="True" Font-Size="Small"></asp:Label>
                            <asp:Label ID="lblQtdeContas" runat="server" ClientIDMode="Static" Text="0" ForeColor="Black" Font-Size="Small"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Número do arquivo de remessa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNumRemessa" runat="server" onkeypress="return soNumeros(event, true, true)"
                                Width="80px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvNumeroArquivoRemessa" runat="server" ErrorMessage="*"
                                ControlToValidate="txtNumRemessa" Display="Dynamic" ValidationGroup="gerar"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label7" runat="server" Text="Loja associada à conta bancária" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpLojaGerar" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvLoja" runat="server" ErrorMessage="*" ControlToValidate="drpLojaGerar"
                                Display="Dynamic" ValidationGroup="gerar"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label8" runat="server" Text="Conta Bancária de Cobrança" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="True"
                                AutoPostBack="true" OnTextChanged="drpContaBanco_TextChanged">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvContaBanco" runat="server" ErrorMessage="*" ControlToValidate="drpContaBanco"
                                Display="Dynamic" ValidationGroup="gerar"></asp:RequiredFieldValidator>
                            <img id="image" src="../Images/load.gif" style="width: 16px; height: 16px; display: none"
                                alt="" />
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="ObterBancoAgrupado"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdsContas" runat="server" />
            </td>
        </tr>
        <tr id="parametros">
            <td align="center">
                <table>
                    <tr>
                        <td align="center" runat="server" id="tdCtrlDadosCnab">
                            <uc3:ctrlDadosCnab runat="server" ID="ctrlDadosCnab" OnCtrlDadosCnabChange="ctrlDadosCnab_CtrlDadosCnabChange"/>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnGerarCnab" runat="server" Text="Gerar Arquivo" OnClick="btnGerarCnab_Click"/></td>
                                    <td>
                                        <input type="button" onclick="return limpar();" value="Limpar" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        permiteGerar();
    </script>

</asp:Content>
