<%@ Page Title="Efetuar Acerto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadContaReceberComposto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadContaReceberComposto" %>

<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLimiteTexto.ascx" TagName="ctrlLimiteTexto" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src="https://s3.amazonaws.com/cappta.api/js/cappta-checkout.js"></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/cappta-tef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">

        var countContas = 1; // Conta a quantidade de contas adicionados ao form
        var totalContas = 0; // Calcula o total de todas as contas
        var creditoCliente = 0; // Guarda quanto de crédito o cliente possui
        var selContasWin = null;
        var totalJuros = 0;

        function openRptFinalizar() {
            var abrirRpt = <%= AbrirRptFinalizar().ToString().ToLower() %>;
            if (!abrirRpt)
                return;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Acerto&idAcerto=" + FindControl("hdfIdAcerto", "input").value);
        }

        function setContaReceber(idContaR, idPedido, pedidosLiberacao, cliente, valor, vencimento, juros, multa, obsScript, tipo) {
            FindControl("txtNumCli", "input").disabled = true;
            FindControl("txtNomeCliente", "input").disabled = true;
            FindControl("imgPesq1", "input").disabled = true;

            // Verifica se conta já foi adicionada
            var contas = FindControl("hdfIdContas", "input").value.split(',');
            for (i = 0; i < contas.length; i++) {
                if (idContaR == contas[i]) {
                    if (selContasWin != null)
                        selContasWin.alert("Conta já adicionada.");

                    return false;
                }
            }

            var valorJuros = parseFloat(juros.replace(',', '.')) + (multa != undefined ? parseFloat(multa.replace(',', '.')) : 0);

            addItem([idPedido != "" ? idPedido : pedidosLiberacao, cliente, valor, "R$ " + valorJuros.toFixed(2).replace(".", ","), vencimento, tipo, obsScript],
                ["Num. Pedido", "Cliente", "Valor", "Juros", "Vencimento", "Tipo", "Obs"], "lstContas", idContaR, "hdfIdContas", 0, null, "itemRemovido", null);

            // Incrementa o valor total das contas
            totalContas = parseFloat(totalContas) + parseFloat(valor.replace(".", "").replace(".", "").replace("R$", "").replace(" ", "").replace(",", "."));

            // Atualiza o total de juros
            totalJuros += valorJuros;

            FindControl("hdfTotalJuros", "input").value = totalJuros;

            // Exibe o valor total das contas até então
            FindControl("lblTotalContas", "span").innerHTML = totalContas.toFixed(2).toString().replace(".", ",");
            FindControl("hdfTotalContas", "input").value = totalContas;

            <%= ctrlFormaPagto1.ClientID %>.AdicionarID(idContaR);
            <%= ctrlFormaPagto1.ClientID %>.AlterarJurosMinimos(totalJuros);
            usarCredito('<%= ctrlFormaPagto1.ClientID %>', "callbackUsarCredito");

            return false;
        }

        function chkRenegociarChecked(chk) {
            var controlePagto = <%= ctrlFormaPagto1.ClientID %>;
            controlePagto.ExibirApenasCredito(chk.checked);

            <%= ctrlFormaPagto1.ClientID %>.AlterarJurosMinimos(parseFloat(FindControl("hdfTotalJuros", "input").value.replace(',', '.')));

            document.getElementById("tbPagto").style.display = chk.checked ? "none" : "";

            document.getElementById("tbRenegociar").style.display = !chk.checked ? "none" : "inline";

            var observacao = document.getElementById("observacao");
            var obsReceber = document.getElementById("obsReceber");
            var obsRenegociar = document.getElementById("obsRenegociar");

            obsReceber.innerHTML = "";
            obsRenegociar.innerHTML = "";

            if (chk.checked)
                obsRenegociar.appendChild(observacao);
            else
                obsReceber.appendChild(observacao);

            if (chk.checked) setParcelas();
        }

        function setParcelas() {
            var controleFormaPagto = <%= ctrlFormaPagto1.ClientID %>;
            FindControl("hdfDescontoParc", "input").value = controleFormaPagto.CreditoUtilizado();

            var nomeControleParcelas = "<%= ctrlParcelas.ClientID %>";
            Parc_visibilidadeParcelas(nomeControleParcelas);
        }

        function renegociar(control)
        {
            try {
                if (!validate())
                    return false;
            }
            catch (err) { alert(err); }

            var numParc = parseInt(FindControl("drpNumParc", "select").value);
            var parcelas = "";

            // Salva os valores das parcelas
            for (i=0; i<numParc; i++)
                parcelas += FindControl("ctrlParcelas_Parc" + (i + 1) + "_txtValor", "input").value + ";" +
                    FindControl("ctrlParcelas_Parc" + (i + 1) + "_txtData", "input").value + ";" +
                    FindControl("ctrlParcelas_Parc" + (i + 1) + "_txtJuros", "input").value+ "|";

            var idCliente = FindControl("hdfIdCli", "input").value;

            bloquearPagina();

            var idContas = FindControl("hdfIdContas", "input").value;
            var idFormaPagto = FindControl("drpPagtoReneg", "select").value;

            var multa = FindControl("txtMultaReneg", "input").value;
            var obs = FindControl("txtObs", "textarea").value;

            //control.disabled = true;
            //FindControl("loadGif", "img").style.visibility = "visible";

            var retornoValidaCnab = CadContaReceberComposto.TemCnabGerado(idContas);

            if(retornoValidaCnab.error != null){
                desbloquearPagina(true);
                alert(retornoValidaCnab.error.description);
                return false;
            }

            if(retornoValidaCnab.value.toLowerCase() == "true" && !confirm('Uma ou mais contas a receber possuem arquivo remessa gerado. Deseja continuar?'))
            {
                desbloquearPagina(true);
                return false;
            }

            var creditoUsado = <%= ctrlFormaPagto1.ClientID %>.CreditoUtilizado();
            var retorno = CadContaReceberComposto.Renegociar(idCliente, idContas, idFormaPagto,
                numParc, parcelas, multa, creditoUsado.toString().replace(".", ","), obs).value.split('\t');

            desbloquearPagina(true);

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                //control.disabled = false;
                //FindControl("loadGif", "img").style.visibility = "hidden";
                return false;
            }
            else {
                alert(retorno[1]);
                FindControl("hdfIdAcerto", "input").value = retorno[2];
                openRptFinalizar();
                limpar();
                //control.disabled = false;
                //FindControl("loadGif", "img").style.visibility = "hidden";
            }
        }

        // Busca contas a receber de um pedido
        function getContaRecFromPed()
        {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idCliente = FindControl("hdfIdCli", "input").value;

            if (idPedido == "")
            {
                alert("Informe o número do pedido.");
                return false;
            }

            if (idCliente == "")
                idCliente = 0;

            var retorno = CadContaReceberComposto.GetContasRecFromPedido(idCliente, idPedido).value;

            if (retorno == null)
            {
                alert("Erro de Ajax.");
                return false;
            }

            retorno = retorno.split('|');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }

            // Busca o cliente deste pedido
            if (idCliente == "0") {
                var txtNumCli = FindControl("txtNumCli", "input");
                txtNumCli.value = retorno[0];
                getCli(txtNumCli);
            }

            // Inclui as contas a receber do pedido informado
            var i=1;
            for (i=1; i<retorno.length; i++){
                var conta = retorno[i].split(';');
                setContaReceber(conta[0], conta[1], conta[2], conta[3], conta[4], conta[5], conta[6], conta[7], conta[8]);
            }

            FindControl("txtNumPedido", "input").value = "";
        }

        function callbackUsarCredito(controle, checked)
        {
            // Exibe o valor a ser pago descontado do crédito do cliente
            var totalASerPago = totalContas;

            // se o cliente possuir crédito
            if (checked && creditoCliente > 0) {
                // Se o crédito do cliente for superior ao valor da conta
                if (parseFloat(creditoCliente) > parseFloat(totalContas))
                    totalASerPago = 0;
                else
                    totalASerPago = totalASerPago - creditoCliente;
            }

            if (document.getElementById("<%= chkRenegociar.ClientID %>").checked)
                setParcelas();
        }

        function itemRemovido(linha) {
            // Recupera o total da linha antes de ser excluída
            var totalLinha = new Number(linha.cells[3].innerHTML.replace("R$", "").replace(" ", "").replace(".", "").replace(",", ".")).toFixed(2);
            var jurosLinha = new Number(linha.cells[4].innerHTML.replace("R$", "").replace(" ", "").replace(".", "").replace(",", ".")).toFixed(2);

            var contaAExcluir = linha.getAttribute("idConta");

            // Recalcula o valor total das contas
            totalContas -= totalLinha;
            totalJuros -= jurosLinha;
            FindControl("lblTotalContas", "span").innerHTML = new Number(totalContas).toFixed(2).toString().replace(".", ",");
            FindControl("hdfTotalContas", "input").value = totalContas;

            <%= ctrlFormaPagto1.ClientID %>.AlterarJurosMinimos(totalJuros);
            document.getElementById("<%= ctrlFormaPagto1.ClientID %>_txtJuros").value = totalJuros.toFixed(2).replace(".", ",");
            usarCredito("<%= ctrlFormaPagto1.ClientID %>", "callbackUsarCredito");
            <%= ctrlFormaPagto1.ClientID %>.RemoverID(contaAExcluir);
        }

        // Validações realizadas ao receber conta
        function onReceber() {
            if (!validate())
                return false;

            var control = FindControl("btnReceber", "input");
            //control.disabled = true;
            //FindControl("loadGif", "img").style.visibility = "visible";

            var controle = <%= ctrlFormaPagto1.ClientID %>;
            var idConta = FindControl("hdfIdContas", "input").value;
            var formasPagto = controle.FormasPagamento();
            var tiposCartao = controle.TiposCartao();
            var tiposBoleto = controle.TiposBoleto();
            var parcelasCredito = controle.ParcelasCartao();

            // Se não tiver selecionado pelo menos uma conta
            if (idConta == "" || idConta == null || idConta == "0") {
                alert("Busque uma conta a receber primeiro");
                //control.disabled = false;
                return false;
            }

            bloquearPagina();

            // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
            var chequesPagto = controle.Cheques();

            var cxDiario = FindControl("hdfCxDiario", "input").value;
            var contas = FindControl("hdfIdContas", "input").value;
            var dataRecebido = controle.DataRecebimento();
            var totalASerPago = totalContas;
            var valores = controle.Valores();
            var contasBanco = controle.ContasBanco();
            var juros = controle.Juros();
            var taxasAntecipacao = controle.TaxasAntecipacao();
            var parcial = controle.RecebimentoParcial();
            var creditoUtilizado = controle.CreditoUtilizado();
            var valorGerarCredito = controle.GerarCredito();
            var numAut = controle.NumeroConstrucard();
            var isDescontarComissao = controle.DescontarComissao();
            var obs = FindControl("txtObs", "textarea").value;
            var idCliente = FindControl("hdfIdCli", "input").value;
            var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
            var numAutCartao = controle.NumeroAutCartao();
            var CNI = controle.CartoesNaoIdentificados();

            var idFormaPgtoCartao = <%= (int)Glass.Data.Model.Pagto.FormaPagto.Cartao %>;
            var utilizarTefCappta = <%= Glass.Configuracoes.FinanceiroConfig.UtilizarTefCappta.ToString().ToLower() %>;
            var tipoCartaoCredito = <%= (int)Glass.Data.Model.TipoCartaoEnum.Credito %>;
            var tipoRecebimento = <%= (int)Glass.Data.Helper.UtilsFinanceiro.TipoReceb.Acerto %>;
            var receberCappta = utilizarTefCappta && formasPagto.split(';').indexOf(idFormaPgtoCartao.toString()) > -1;

            retorno = CadContaReceberComposto.Receber(idCliente, contas, dataRecebido, totalASerPago, formasPagto, valores, contasBanco, depositoNaoIdentificado, CNI, tiposCartao, tiposBoleto,
                taxasAntecipacao, juros, parcial, valorGerarCredito, creditoUtilizado, cxDiario, numAut, parcelasCredito, chequesPagto, isDescontarComissao, obs, numAutCartao,
                receberCappta.toString().toLowerCase()).value.split('\t');

            if (retorno[0] == "Erro") {
                desbloquearPagina(true);
                alert(retorno[1]);
                return false;
            }

            //Se utilizar o TEF CAPPTA e tiver selecionado pagamento com cartão à vista
            if (receberCappta) {

                //Busca os dados para autenticar na cappta
                var dadosAutenticacaoCappta = MetodosAjax.ObterDadosAutenticacaoCappta();

                if(dadosAutenticacaoCappta.error) {
                    desbloquearPagina(true);
                    alert(dadosAutenticacaoCappta.error.description);
                    return false;
                }

                //Instancia do canal de recebimento
                CapptaTef.init(dadosAutenticacaoCappta.value, (sucesso, msgErro, codigosAdministrativos, msgRetorno) => callbackCappta(sucesso, msgErro, codigosAdministrativos, msgRetorno));

                //Inicia o recebimento
                CapptaTef.efetuarRecebimento(retorno[2], tipoRecebimento, idFormaPgtoCartao, tipoCartaoCredito, formasPagto, tiposCartao, valores, parcelasCredito);

                return false;
            }

            desbloquearPagina(true);

            alert(retorno[1]);
            FindControl("hdfIdAcerto", "input").value = retorno[2];
            openRptFinalizar();
            limpar();

        }

        //Método chamado ao realizar o pagamento atraves do TEF CAPPTA
        function callbackCappta(sucesso, msgErro, codigosAdministrativos, msgRetorno) {

            desbloquearPagina(true);

            if(!sucesso) {
                alert(msgErro);
                return false;
            }

            var retorno = msgRetorno.split('\t');

            alert(retorno[0]);
            FindControl("hdfIdAcerto", "input").value = retorno[1];
            openRptFinalizar();
            openWindow(600, 800, "../Relatorios/Relbase.aspx?rel=ComprovanteTef&codControle=" + codigosAdministrativos.join(';'));
            limpar();
        }

        // Abre popup para selecionar contas
        function openWindowContas(altura, largura, url) {
            var momentoAtual = new Date();

            getCli(FindControl("txtNumCli", "input"));

            if (FindControl("hdfIdCli", "input").value == "") {
                alert("Escolha um cliente antes de buscar contas a receber");
                return false;
            }

            selContasWin = openWindow(altura, largura, url + "?acerto=1&idCli=" + FindControl("hdfIdCli", "input").value + "&NomeCli=" + FindControl("txtNomeCliente", "input").value);
            return false;
        }

        // Abre popup para cadastrar cheques
        function queryStringCheques(altura, largura, url) {
            /*
            if (FindControl("hdfIdAcerto", "input").value == "") {
                alert("Selecione pelo menos uma conta antes de cadastrar o(s) cheque(s).");
                return false;
            }
            */

            //return "?idAcerto=" + FindControl("hdfIdAcerto", "input").value + "&origem=3";
            return "?idAcerto=" + FindControl("hdfIdAcerto", "input").value + "&origem=3&idCliente=" + FindControl("txtNumCli","input").value + "&tipoPagto=2";
        }

        function getCli(idCli) {
            if (idCli.value == "") {
                openWindow(570, 760, '../Utils/SelCliente.aspx');
                return false;
            }

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
            FindControl("hdfIdCli", "input").value = idCli.value;

            // Busca o crédito que este cliente possui
            creditoCliente = MetodosAjax.GetClienteCredito(idCli.value).value;

            creditoCliente = creditoCliente == null ? 0 : parseFloat(creditoCliente);

            // Guarda quanto de crédito o cliente possui
            FindControl("hdfValorCredito", "input").value = creditoCliente;
            usarCredito('<%= ctrlFormaPagto1.ClientID %>', "callbackUsarCredito");

            return false;
        }

        function limpar() {
            countContas = 1;
            totalContas = 0;
            creditoCliente = 0;

            var btnRenegociar = FindControl("btnRenegociar", "input");
            var chkRenegociar = FindControl("chkRenegociar", "input");
            //var chkGerarCnab = FindControl("chkGerarCnab", "input");

            if (btnRenegociar != null) btnRenegociar.disabled = false;
            if (chkRenegociar != null) chkRenegociar.checked = false;
            //if (chkGerarCnab != null) chkGerarCnab.checked = false;

            //gerarCnab(chkGerarCnab);

            FindControl("hdfIdContas", "input").value = "";
            FindControl("hdfIdCli", "input").value = "";
            FindControl("hdfIdAcerto", "input").value = "";
            FindControl("hdfIdPedido", "input").value = "";
            FindControl("hdfTotalASerPago", "input").value = "";
            FindControl("hdfValorCredito", "input").value = "";

            if(FindControl("txtObs", "textarea") != null)
                FindControl("txtObs", "textarea").value = "";

            FindControl("txtNumCli", "input").disabled = false;
            FindControl("txtNomeCliente", "input").disabled = false;
            FindControl("imgPesq1", "input").disabled = false;
            FindControl("txtNumCli", "input").value = "";
            FindControl("txtNomeCliente", "input").value = "";

            FindControl("lblTotalContas", "span").innerHTML = "0,00";
            FindControl("hdfTotalContas", "input").value = "0";

            document.getElementById('lstContas').innerHTML = "";

            <%= ctrlFormaPagto1.ClientID %>.Limpar();
        }


    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) getCli(this);"
                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td>&nbsp;<asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="return getCli(FindControl('txtNumCli', 'input'));" />
                        </td>
                        <td>&nbsp;
                        </td>
                        <td>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar Contas" OnClientClick="openWindowContas(600, 800, '../Utils/SelContaReceber.aspx'); return false;"
                                OnClick="btnBuscar_Click" />
                        </td>
                    </tr>
                </table>
                <br />
            </td>
        </tr>
        <tr align="center">
            <td align="center">
                <table id="tbGetFromPedido">
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) getContaRecFromPed();"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgGetContaRec" runat="server" ImageUrl="~/Images/Insert.gif"
                                Width="16px" OnClientClick="getContaRecFromPed(); return false;" ToolTip="Adicionar Contas a Receber" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr align="center">
            <td align="center">
                <table id="lstContas" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table align="center">
                    <tr>
                        <td style="font-size: large">Total das Contas: R$
                            <asp:Label ID="lblTotalContas" runat="server">0,00</asp:Label>
                            <asp:HiddenField ID="hdfTotalContas" runat="server" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkRenegociar" runat="server" Text="Renegociar" onclick="chkRenegociarChecked(this);" />
                        </td>
                        <%--<td style='<%= !ExibirCnab() ? "display: none": "" %>'>
                            <asp:CheckBox ID="chkGerarCnab" runat="server" Text="Gerar Arquivo de Remessa" onclick="gerarCnab(this)"
                                ForeColor="Blue" />
                        </td>--%>
                    </tr>
                </table>
                <table align="center" id="tbPagto">
                    <tr>
                        <td>
                            <uc1:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" OnLoad="ctrlFormaPagto1_Load"
                                TipoModel="ContasReceber" CallbackUsarCredito="callbackUsarCredito" ParentID="tbPagto"
                                FuncaoQueryStringCheques="queryStringCheques" />
                        </td>
                    </tr>
                    <tr>
                        <td id="obsReceber" align="center">
                            <table id="observacao" style="margin: 8px">
                                <tr>
                                    <td>Obs.
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="300px"
                                            MaxLength="200" />
                                    </td>
                                    <td>
                                        <uc3:ctrlLimiteTexto ID="lmtTxtObs" runat="server" IdControlToValidate="txtObs" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnReceber" runat="server" OnClientClick="if (!onReceber()) return false;"
                                Text="Receber" />
                            <img id="loadGif" src="../Images/load.gif" border="0px" title="Aguarde..." width="20px"
                                height="20px" style="visibility: hidden;" />
                            <asp:Button ID="btnLimpar" runat="server" OnClientClick="limpar(); return false;"
                                Text="Limpar" />
                        </td>
                    </tr>
                </table>
                <table id="tbRenegociar" style="display: none;">
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label19" runat="server" Text="Número de Parcelas:" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpNumParc" runat="server">
                                            <asp:ListItem>1</asp:ListItem>
                                            <asp:ListItem>2</asp:ListItem>
                                            <asp:ListItem Selected="True">3</asp:ListItem>
                                            <asp:ListItem>4</asp:ListItem>
                                            <asp:ListItem Value="5"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label20" runat="server" Text="Forma Pagto.:" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpPagtoReneg" runat="server" DataSourceID="odsFormaPagtoReneg"
                                            DataTextField="Descricao" DataValueField="IdFormaPagto">
                                        </asp:DropDownList>
                                    </td>
                                    <%--<td>
                                        Juros:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtJurosReneg" runat="server" Width="60px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                    </td>--%>
                                    <td>Multa:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMultaReneg" runat="server" Width="60px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc2:ctrlParcelas ID="ctrlParcelas" runat="server" NumParcelas="5" ParentID="tbRenegociar"
                                OnLoad="ctrlParcelas_Load" CallbackTotal="callbackUsarCredito" ExibirJurosPorParcela="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span id="obsRenegociar"></span>&nbsp;
                            <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                            <asp:HiddenField ID="hdfDescontoParc" runat="server" />
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagtoReneg" runat="server" SelectMethod="GetForRenegociacao"
                                TypeName="Glass.Data.DAL.FormaPagtoDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnRenegociar" runat="server" Text="Renegociar" OnClientClick="return renegociar(this);" />
                        </td>
                    </tr>
                </table>
                <%--<table id="tbCnab" style="display: none">
                    <tr>
                        <td align="center">
                            <br />
                            <br />
                            <asp:Button ID="btnGerarCnab" runat="server" Text="Gerar arquivo de remessa" OnClientClick="abrirGerarCnab(); return false" />
                        </td>
                    </tr>
                </table>--%>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfValorCredito" runat="server" />
    <asp:HiddenField ID="hdfIdContas" runat="server" />
    <asp:HiddenField ID="hdfIdCli" runat="server" />
    <asp:HiddenField ID="hdfIdPedido" runat="server" />
    <asp:HiddenField ID="hdfIdAcerto" runat="server" />
    <asp:HiddenField ID="hdfTotalASerPago" runat="server" />
    <asp:HiddenField ID="hdfTotalJuros" runat="server" />
    <asp:HiddenField ID="hdfCxDiario" runat="server" />

    <script type="text/javascript">
        var valorGerarCredito = FindControl("chkGerarCredito", "input");
        if (valorGerarCredito != null)
            valorGerarCredito.checked = false;

        var chkParcial = FindControl("chkParcial", "input");
        if (chkParcial != null)
            chkParcial.checked = false;

        // Esconde opção de buscar contas pelo idPedido se empresa trabalha com liberação
        if ("<%= Glass.Configuracoes.PedidoConfig.LiberarPedido.ToString().ToLower() %>" == "true")
            document.getElementById("tbGetFromPedido").style.display = "none";

    </script>

</asp:Content>
