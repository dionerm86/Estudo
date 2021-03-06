﻿<%@ Page Title="Cadastro de Pagamento Antecipado" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadObra.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadObra" %>

<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src="https://s3.amazonaws.com/cappta.api/js/cappta-checkout.js"></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/cappta-tef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        var totalASerPago = 0;

        function getProduto()
        {
            openWindow(600, 800, "../Utils/SelProd.aspx?obra=true");
        }

        function setProduto(codInterno)
        {
            loadProduto(codInterno);
        }

        function loadProduto(codInterno)
        {
            var idObra = <%= Request["idObra"] != null ? Request["idObra"] : "0" %>;
            var idCliente = FindControl("lblIdCliente", "span").innerHTML;
            if (codInterno == "")
            {
                FindControl("txtCodProd", "input").value = "";
                FindControl("lblDescrProd", "span").innerHTML = "";
                FindControl("hdfIdProd", "input").value = "";
                FindControl("txtValorUnit", "input").value = "";
                return;
            }
            else if (CadObraNovo.IsVidro(codInterno).value != "true" || CadObraNovo.ProdutoJaExiste(idObra, codInterno).value == "true")
            {
                FindControl("txtCodProd", "input").value = "";
                FindControl("lblDescrProd", "span").innerHTML = "";
                FindControl("hdfIdProd", "input").value = "";
                FindControl("txtValorUnit", "input").value = "";

                alert("Apenas produtos do grupo Vidro podem ser incluídos nesse <%= DescrTipoObra() %>.");
                return;
            }

            var resposta = CadObraNovo.GetProd(codInterno, idCliente).value.split(";");
            if (resposta[0] == "Erro")
            {
                FindControl("txtCodProd", "input").value = "";
                FindControl("lblDescrProd", "span").innerHTML = "";
                FindControl("hdfIdProd", "input").value = "";
                FindControl("txtValorUnit", "input").value = "";

                alert(resposta[1]);
                return;
            }

            FindControl("txtCodProd", "input").value = codInterno;
            FindControl("lblDescrProd", "span").innerHTML = resposta[2];
            FindControl("hdfIdProd", "input").value = resposta[1];
            FindControl("txtValorUnit", "input").value = resposta[3];
        }

        var clicked = false;

        // Validações realizadas ao receber conta
        function onInsertUpdate() {
            if (!validate())
                return false;

            if (clicked)
                return false;

            clicked = true;

            var idCliente = FindControl("txtNumCli", "input").value;
            var descricao = FindControl("txtDescricao", "textarea").value;
            var idFunc = FindControl("drpFuncionario", "select").value;

            if (descricao == "")
            {
                alert("Informe a descrição.");
                clicked = false;
                return false;
            }

            if (idCliente == "")
            {
                alert("Informe o Cliente.");
                clicked = false;
                return false;
            }

            if (idFunc == "")
            {
                alert("Informe o Funcionário.");
                clicked = false;
                return false;
            }

            return true;
        }

        function limpar() {
            try
            {
                <%= dtvObra.ClientID %>_ctrlFormaPagto1.Limpar();
            }
            catch (err) { }
        }

        function getCli(idCli)
        {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        // Abre popup para cadastrar cheques
        function queryStringCheques() {
            var ctrTipoPagto = FindControl("drpTipoPagto", "select");
            return "?origem=3&IdCli=" + FindControl("txtNumCli","input").value +"&tipoPagto=" + ctrTipoPagto.value;
        }

        function tipoPagtoChanged(calcParcelas)
        {
            var tipoPagto = FindControl("drpTipoPagto", "select");

            if (tipoPagto == null)
                return;
            else
                tipoPagto = tipoPagto.value;

            document.getElementById("a_vista").style.display = (tipoPagto == 1) ? "" : "none";
            document.getElementById("a_prazo").style.display = (tipoPagto == 2) ? "" : "none";

            FindControl("hdfCalcularParcelas", "input").value = calcParcelas;
            var nomeControle = "<%= dtvObra.ClientID %>_ctrlParcelas1";
            if (typeof <%= dtvObra.ClientID %>_ctrlParcelas1 != "undefined")
                Parc_visibilidadeParcelas(nomeControle);
        }

        function finalizar(exibir)
        {
            FindControl("btnEditar", "input").style.display = exibir ? "none" : "";
            FindControl("btnFinalizar", "input").style.display = exibir ? "none" : "";
            FindControl("btnVoltar", "input").style.display = exibir ? "none" : "";

            var tabProdutos = FindControl("grdProdutoObra", "table");
            var numLinha = tabProdutos.rows.length - 1;
            if (tabProdutos.rows[numLinha].cells.length == 1)
                numLinha--;

            tabProdutos.rows[numLinha].style.display = exibir ? "none" : "";

            document.getElementById("receber").style.display = exibir ? "" : "none";
            FindControl("btnReceber", "input").style.display = exibir ? "" : "none";
            FindControl("btnCancelar", "input").style.display = exibir ? "" : "none";
        }

        function validaFormaPagtoPrazo(val, args)
        {
            var totalPrazo = parseFloat(document.getElementById("<%= dtvObra.ClientID %>_ctrlParcelas1_txtValorParcelas").value);
            if (isNaN(totalPrazo))
                totalPrazo = 0;

            args.IsValid = document.getElementById("a_prazo").style.display == "none" ||
                args.Value != "" || totalPrazo == 0;
        }

        function onReceber() {
            if (!validate())
                return false;

            bloquearPagina();

            var idObra = GetQueryString("idObra");
            var tipoPagto = FindControl("drpTipoPagto", "select");
            var cxDiario = GetQueryString("cxDiario") == "1";

            var retornoReceber = "";

            // À vista
            if(tipoPagto && tipoPagto.value == "1") {

                var controle = <%= dtvObra.FindControl("ctrlFormaPagto1") != null ? dtvObra.FindControl("ctrlFormaPagto1").ClientID : "''" %>;

                if(controle == null) {
                    desbloquearPagina(true);
                    alert("O controle de pagto. não foi encontrado.");
                    return false;
                }

                var valores = controle.Valores();
                var formasPagto = controle.FormasPagamento();
                var tiposCartao = controle.TiposCartao();
                var parcelasCredito = controle.ParcelasCartao();
                var contas = controle.ContasBanco();
                var chequesPagto = controle.Cheques();
                var creditoUtilizado = controle.CreditoUtilizado();
                var dataRecebido = controle.DataRecebimento();
                var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
                var numAutCartao = controle.NumeroAutCartao();
                var CNI = controle.CartoesNaoIdentificados();
                var isGerarCredito = controle.GerarCredito();

                var idFormaPgtoCartao = <%= (int)Glass.Data.Model.Pagto.FormaPagto.Cartao %>;
                var utilizarTefCappta = <%= Glass.Configuracoes.FinanceiroConfig.UtilizarTefCappta.ToString().ToLower() %>;
                var tipoCartaoCredito = <%= (int)Glass.Data.Model.TipoCartaoEnum.Credito %>;
                var tipoRecebimento = <%= (int)Glass.Data.Helper.UtilsFinanceiro.TipoReceb.Obra %>;
                var receberCappta = utilizarTefCappta && formasPagto.split(';').indexOf(idFormaPgtoCartao.toString()) > -1;

                var retornoReceber = CadObra.ReceberAVista(idObra, valores, formasPagto, tiposCartao, parcelasCredito, contas, chequesPagto, creditoUtilizado, dataRecebido, depositoNaoIdentificado,
                    numAutCartao, CNI, isGerarCredito, cxDiario, receberCappta.toString().toLowerCase());

                if(retornoReceber.error != null){
                    desbloquearPagina(true);
                    alert(retornoReceber.error.description);
                    return false;
                }

                //Se utilizar o TEF CAPPTA e tiver selecionado pagamento com cartão à vista
                if(receberCappta) {

                    //Busca os dados para autenticar na cappta
                    var dadosAutenticacaoCappta = MetodosAjax.ObterDadosAutenticacaoCappta();

                    if(dadosAutenticacaoCappta.error) {
                        desbloquearPagina(true);
                        alert(dadosAutenticacaoCappta.error.description);
                        return false;
                    }

                    //Inicia o canal de recebimento
                    CapptaTef.init(dadosAutenticacaoCappta.value, (sucesso, msg, codigosAdministrativos) => callbackCappta(sucesso, msg, codigosAdministrativos, retornoReceber, cxDiario));

                    //Faz o recebimento
                    CapptaTef.efetuarRecebimento(idObra, tipoRecebimento, idFormaPgtoCartao, tipoCartaoCredito, formasPagto, tiposCartao, valores, parcelasCredito);

                    return false;
                }
            } else if(tipoPagto && tipoPagto.value == "2") { // À prazo

                var numParcelas = FindControl("drpNumParcelas", "select");
                var controle = <%= dtvObra.FindControl("ctrlParcelas1") != null ? dtvObra.FindControl("ctrlParcelas1").ClientID : "''" %>;
                var formasPagto = FindControl("drpFormaPagtoPrazo", "select");
                var valores = controle.Valores();
                var datas = controle.Datas();

                var retornoReceber = CadObra.ReceberAPrazo(idObra, numParcelas.value, formasPagto.value, valores, datas, cxDiario);

                if(retornoReceber.error != null) {
                    desbloquearPagina(true);
                    alert(retornoReceber.error.description);
                    return false;
                }
            }

            desbloquearPagina(true);
            alert(retornoReceber.value);
            var gerarCreditoObra = "<%= GerarCreditoObra().ToString().ToLower() %>";
            redirectUrl('../Listas/LstObra.aspx' + (gerarCreditoObra ? "?gerarCredito=1" : "") + (cxDiario ? (gerarCreditoObra? "&": "?") + "cxDiario=1" : ""));
            return false;
        }

        function callbackCappta(sucesso, msg, codigosAdministrativos, retorno, cxDiario) {

            desbloquearPagina(true);

            if(!sucesso){
                alert(msg);
                return false;
            }

            alert(retorno.value);
            openWindow(600, 800, "../Relatorios/Relbase.aspx?rel=ComprovanteTef&codControle=" + codigosAdministrativos.join(';'));
            var gerarCreditoObra = "<%= GerarCreditoObra().ToString().ToLower() %>";
            redirectUrl('../Listas/LstObra.aspx' + (gerarCreditoObra ? "?gerarCredito=1" : "") + (cxDiario ? (gerarCreditoObra? "&": "?") + "cxDiario=1" : ""));
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvObra" runat="server" AutoGenerateRows="False" DataKeyNames="IdObra"
                    DataSourceID="odsObra" DefaultMode="Insert" GridLines="None">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table cellspacing="0">
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Descrição
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="200" Rows="3" Width="400px"
                                                Text='<%# Bind("Descricao") %>' TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Funcionário
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario" DataTextField="Nome"
                                                DataValueField="IdFunc" AppendDataBoundItems="True" SelectedValue='<%# Bind("IdFunc") %>'
                                                OnDataBinding="drpFuncionario_DataBinding" OnDataBound="drpFuncionario_DataBound">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                                                SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Cliente
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getCli(this);" Text='<%# Bind("IdCliente") %>'></asp:TextBox>
                                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px"
                                                Text='<%# Bind("NomeCliente") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                                OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=obra'); return false;" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Valor do <%= DescrTipoObra() %>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtTotal" runat="server" Text='<%# Bind("ValorObra") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfGerarCredito" runat="server"
                                    Value='<%# Bind("GerarCredito") %>' onload="hdfGerarCredito_Load" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <table cellspacing="0" cellpadding="4">
                                    <tr>
                                        <td class="dtvHeader">
                                            Descrição
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Funcionário
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblNomeFuncionario" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("IdCliente") %>'></asp:Label>
                                            &nbsp;-
                                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Valor do <%= DescrTipoObra() %>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorObra", "{0:c}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:Panel ID="panReceber" runat="server" Visible="false">
                                    <div style="padding: 12px">
                                        Tipo de pagamento
                                        <asp:DropDownList ID="drpTipoPagto" runat="server" onchange="tipoPagtoChanged(true)">
                                            <asp:ListItem Value="1">À vista</asp:ListItem>
                                            <asp:ListItem Value="2">À prazo</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div id="a_vista">
                                        <uc2:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" OnInit="ctrlFormaPagto1_Init"
                                            CobrarJurosCartaoClientes="False" ExibirComissaoComissionado="false" ExibirCredito="True"
                                            ExibirDataRecebimento="true" ExibirGerarCredito="False" ExibirJuros="false"
                                            ExibirRecebParcial="false" ExibirUsarCredito="True" ExibirValorAPagar="True"
                                            FuncaoQueryStringCheques="queryStringCheques" UsarCreditoMarcado="false"
                                            OnLoad="ctrlFormaPagto1_Load" ParentID="a_vista" EfetuarBindContaBanco="false" />
                                    </div>
                                    <div id="a_prazo">
                                        <br />
                                        Número de parcelas:
                                        <asp:DropDownList ID="drpNumParcelas" runat="server" DataSourceID="odsParcelas" DataTextField="Descr"
                                            DataValueField="Id">
                                        </asp:DropDownList>
                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsParcelas" runat="server" SelectMethod="GetNumParc"
                                            TypeName="Glass.Data.Helper.DataSources">
                                        </colo:VirtualObjectDataSource>
                                        <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                                        <br />
                                        Forma de pagamento:
                                        <asp:DropDownList ID="drpFormaPagtoPrazo" runat="server" DataSourceID="odsFormaPagto"
                                            AppendDataBoundItems="true" DataTextField="Descricao"
                                            DataValueField="IdFormaPagto" EnableViewState="false">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CustomValidator ID="ctvPrazo" runat="server" ClientValidationFunction="validaFormaPagtoPrazo"
                                            ControlToValidate="drpFormaPagtoPrazo" Display="Dynamic" ErrorMessage="Selecione uma forma de pagamento"
                                            ValidateEmptyText="True"></asp:CustomValidator>

                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForPedido"
                                            TypeName="Glass.Data.DAL.FormaPagtoDAO">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdCliente" PropertyName="Value" Name="idCliente" Type="Int32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>

                                        <uc1:ctrlParcelas ID="ctrlParcelas1" runat="server" NumParcelas="25" NumParcelasLinha="3"
                                            OnLoad="ctrlParcelas1_Load" ParentID="a_prazo"  />
                                    </div>
                                    <asp:HiddenField ID="hdfTotalObra" runat="server" />
                                </asp:Panel>
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCliente") %>' />
                                <asp:HiddenField ID="hdfCreditoCliente" runat="server" Value='<%# Eval("CreditoCliente") %>' />
                                <asp:HiddenField ID="hdfGerarCredito" runat="server"
                                    onload="hdfGerarCredito_Load" Value='<%# Bind("GerarCredito") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onInsertUpdate()) return false;" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False"
                                    Text="Cancelar"
                                    onclientclick="redirectUrl(window.location.href); return false" />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <br />
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsertUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelar_Click" />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <br />
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar"
                                    onclick="btnFinalizar_Click" OnLoad="btnFinalizar_Load" />
                                <asp:Button ID="btnVoltar" runat="server" Text="Voltar"
                                    onclick="btnCancelar_Click" />
                                <asp:Button ID="btnReceber" runat="server" Text="Receber" Visible="False" OnClientClick="return onReceber();"/>
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar"
                                    onclick="btnCancelarReceb_Click" Visible="False"
                                    onclientclick="if (!confirm(&quot;Deseja cancelar o recebimento?&quot;)) return false"
                                    CausesValidation="False" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObra" runat="server" DataObjectTypeName="Glass.Data.Model.Obra"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.ObraDAO"
                    UpdateMethod="Update" OnInserted="odsObra_Inserted"
                    onupdated="odsObra_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="IdObra" QueryStringField="IdObra" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        tipoPagtoChanged(false);
    </script>

</asp:Content>
