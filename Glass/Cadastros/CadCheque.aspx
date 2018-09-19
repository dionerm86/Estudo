<%@ Page Title="Cadastro de Cheques" Language="C#" AutoEventWireup="true" CodeBehind="CadCheque.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadCheque" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        var nomeTabelaChequesOpener = <%= !String.IsNullOrEmpty(Request["tabelaCheque"]) ? Request["tabelaCheque"] : "'tbChequePagto'" %>;
        var tipoPagto = <%= Request["tipoPagto"] %>;

        function validar()
        {
            var dataVenc = FindControl("ctrlData_txtData", "input");

            // Verifica se a data dos cheques deve ser maior que a data de hoje
            var bloquearRetroativo = <%= Glass.Configuracoes.FinanceiroConfig.FormaPagamento.BloquearChequesDataRetroativa.ToString().ToLower() %>;
            if (bloquearRetroativo && dataVenc.value != "")
            {
                var dataMin = "<%= GetDataVencMin() %>";

                if (firstGreaterThenSec(dataMin, dataVenc.value))
                {
                    alert("A data de vencimento do cheque deve ser, no mínimo, " + dataMin + ".");
                    return false;
                }
            }

            //var origem = FindControl("ctrOrigem_hdfLink", "input").value;
            //var idLiberarPedido = FindControl("ctrlLinkQueryString3_hdfLink", "input").value;

            // Verifica se o cheque é para pagamento à vista de liberação
            // Bloqueia se a data de vencimento for superior à de amanhã e se a liberação bloquear dados do pedido
            var bloquearVista = <%= (Glass.Configuracoes.Liberacao.FormaPagamento.NumeroDiasChequeVistaLiberarPedido > 0).ToString().ToLower() %>;

            // Verifica se o cheque é para pagamento à prazo de liberação
            // Bloqueia se a data de vencimento for superior ao prazo máximo do cliente e se a liberação bloquear dados do pedido
            var bloquearPrazo = <%= Glass.Configuracoes.Liberacao.DadosLiberacao.BloquearLiberacaoDadosPedido.ToString().ToLower() %>;

            var dataMax = null;

            if (bloquearVista && tipoPagto == "1")
                dataMax = "<%= GetDataVencMaxVista() %>";
            else if (bloquearPrazo && tipoPagto == "2")
                dataMax = "<%= GetDataVencMaxPrazo() %>";

            if (dataMax != null)
            {
                if (firstGreaterThenSec(dataVenc.value, dataMax))
                {
                    alert("A data de vencimento do cheque deve ser, no máximo, " + dataMax + ".");
                    return false;
                }
            }

            return validate(null);
        }

        function setOpenerCheque()
        {
            if (!validar())
                return false;

            var numCheque = FindControl("txtNumero", "input").value;
            var digitoNum = FindControl("txtDigitoNum", "input").value;
            var titular = FindControl("txtTitular", "input").value;
            var valor = FindControl("ctrValor_txtNumber", "input").value;
            var dataVenc = FindControl("ctrlData_txtData", "input").value;
            var banco = FindControl("txtBanco", "input").value;
            var agencia = FindControl("txtAgencia", "input").value;
            var conta = FindControl("txtConta", "input").value;
            var origem = FindControl("ctrOrigem_hdfLink", "input").value;
            var idAcertoCheque = FindControl("ctrIdAcertoCheque_hdfLink", "input").value;
            var idContaR = FindControl("ctrIdContaR_hdfLink", "input").value;
            var idPedido = FindControl("ctrlLinkQueryString1_hdfLink", "input").value;
            var idAcerto = FindControl("ctrlLinkQueryString2_hdfLink", "input").value;
            var idLiberarPedido = FindControl("ctrlLinkQueryString3_hdfLink", "input").value;
            var idTrocaDevolucao = FindControl("ctrlLinkQueryString4_hdfLink", "input").value;
            var idSinal = FindControl("ctrlLinkQueryString5_hdfLink", "input").value;
            var linha = FindControl("hdfLinha", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var obs = FindControl("txtObs", "textarea").value;
            var loja = FindControl("drpLoja", "select");
            var nomeLoja = loja.options[loja.selectedIndex].text;
            var cpfCnpj = FindControl("txtCpfCnpj", "input");
            cpfCnpj = cpfCnpj ? cpfCnpj.value : "";

            // Verifica se a data é válida
            if (!verifica_data(dataVenc))
                return false;

            // Verificar se o titular possui menos de 45 caracteres
            if (titular != "" && titular.toString().length > 45)
            {
                alert("O Titular deve ter no máximo 45 caracteres.");
                return false;
            }

            // Verifica se o cheque já existe
            var validaCheque = CadCheque.ValidaCheque(idCliente, banco, agencia, conta, numCheque, digitoNum).value.split('|');
            if (validaCheque[0] == "false")
            {
                alert(validaCheque[1]);
                return false;
            }

            // Verifica se o número do cheque foi digitado com 6 caracteres
            var numeroDigitosCheque = <%= NumeroDigitosCheque() %>;
            if (numeroDigitosCheque > 0 && FindControl("txtNumero", "input").value.toString().length != numeroDigitosCheque)
            {
                alert("O número do cheque deve ser informado com " + numeroDigitosCheque + " caracteres.");
                return false;
            }

            var callbackIncluir = <%= !String.IsNullOrEmpty(Request["callbackIncluir"]) ? Request["callbackIncluir"] : "''" %>;
            var callbackExcluir = <%= !String.IsNullOrEmpty(Request["callbackExcluir"]) ? Request["callbackExcluir"] : "''" %>;
            var nomeControleFormaPagto = <%= !String.IsNullOrEmpty(Request["nomeControleFormaPagto"]) ? Request["nomeControleFormaPagto"] : "''" %>;
            var controlePagto = <%= !String.IsNullOrEmpty(Request["controlPagto"]) ? Request["controlPagto"] : "''" %>;

            var exibirCpfCnpj = "<%= ExibirDadosLimiteCheque().ToString().ToLower() %>";

            window.opener.setCheque(nomeTabelaChequesOpener, null, null, numCheque, digitoNum, titular, valor, dataVenc, banco, agencia,
                conta, 1, obs, window, "terceiro", origem, idAcertoCheque, idContaR, idPedido, idSinal, idAcerto, idLiberarPedido, idTrocaDevolucao,
                cpfCnpj, loja.value, nomeLoja, controlePagto, linha, callbackIncluir, callbackExcluir, nomeControleFormaPagto, exibirCpfCnpj);

            var tabela = document.getElementById("tbChequePagto");
            var tabelaOpener = window.opener.document.getElementById(nomeTabelaChequesOpener);
            duplicarTabela(tabela, tabelaOpener);

            atualizaTotal();

            FindControl("drpLoja", "select").focus();
        }

        function atualizaTotal()
        {
            var controleTotal = FindControl("lblTotal", "span");
            if (controleTotal != null)
            {
                var total = parseFloat(window.opener.document.getElementById(<%= Request["controlPagto"] %>).value.replace(',', '.'));
                if (isNaN(total))
                    total = 0;

                controleTotal.innerHTML = "<br />Total: R$ " + total.toFixed(2).replace('.', ',') + "<br /><br />";
            }
        }

        function limpar()
        {
            var control = GetQueryString("controlForma");

            FindControl("txtNumero", "input").value = "";
            FindControl("txtDigitoNum", "input").value = "";
            FindControl("ctrValor_txtNumber", "input").value = "";
            FindControl("ctrlData_txtData", "input").value = "";
            FindControl("txtBanco", "input").value = "";
            FindControl("txtAgencia", "input").value = "";
            FindControl("txtConta", "input").value = "";
            // Só limpa os campos de CPF/CNPJ se não for cheque próprio
            if(control != 2){
                if (FindControl("drpTipoPessoa", "select")){
                    FindControl("drpTipoPessoa", "select").value = "F";
                }
                if (FindControl("txtCpfCnpj", "input")){
                    FindControl("txtCpfCnpj", "input").value = "";
                }

                alteraTipoPessoa();
            }
            FindControl("drpLoja", "select").value = "";
            FindControl("hdfLinha", "input").value = "";
            FindControl("btnInserir", "input").value = "Inserir";
        }

        function fechar()
        {
            if (FindControl("hdfLinha", "input").value != "")
                window.opener.document.getElementById(nomeTabelaChequesOpener).rows[FindControl("hdfLinha", "input").value].style.backgroundColor = "White";

            closeWindow();
        }

        function alteraTipoPessoa()
        {
            if (!FindControl("drpTipoPessoa", "select"))
                return;

            var tipoPessoa = FindControl("drpTipoPessoa", "select").value;
            var label = FindControl("Label9", "span");
            var controle = FindControl("txtCpfCnpj", "input");
            var validador = eval(FindControl("ctvCpfCnpj", "span").id);

            if (controle)
                controle.value = "";

            if (tipoPessoa == "F")
            {
                label.innerHTML = "CPF";

                if (controle)
                    controle.setAttribute("onkeydown","return maskCPF(event, this)");
            }
            else
            {
                label.innerHTML = "CNPJ";

                if (controle)
                    controle.setAttribute("onkeydown","return maskCNPJ(event, this)");
            }
        }

        window.onload=function(){

            var idCli = GetQueryString("idCliente");

            if(idCli == null || idCli == "")
                idCli = GetQueryString("IdCli");

            var control = GetQueryString("controlForma");


            if(idCli != null && idCli != "" && control != null && control != 9){

                var retorno = CadCheque.DadosCliente(idCli);

                if(retorno.error != null){
                    alert(retorno.error.description);
                    return false;
                }

                var dados = retorno.value.split(";");

                FindControl("txtTitular", "input").value = dados[0];

                if (FindControl("drpTipoPessoa", "select")) FindControl("drpTipoPessoa", "select").value = dados[1];
                if (FindControl("txtCpfCnpj", "input")) FindControl("txtCpfCnpj", "input").value = dados[2];

                if(FindControl("drpTipoPessoa", "select").value == "J")
                    FindControl("Label9", "span").innerHTML = "CNPJ"
            }
        };

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvCheque" runat="server" AutoGenerateRows="False" DataSourceID="odsCadCheque"
                    DefaultMode="Insert" GridLines="None" DataKeyNames="IdCheque">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <InsertItemTemplate>
                                <table class="gridStyle detailsViewStyle" cellpadding="1" cellspacing="0">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label41" runat="server" Text="Titular"></asp:Label>
                                        </td>
                                        <td align="left" colspan="3" nowrap="nowrap">
                                            <asp:TextBox ID="txtTitular" runat="server" Text='<%# Bind("Titular") %>' onkeyup="verificaLeituraCheque(this, event);"
                                                Width="350px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqdTitular" runat="server" ControlToValidate="txtTitular"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label11" runat="server" Text="Loja"></asp:Label>
                                        </td>
                                        <td align="left" colspan="3" nowrap="nowrap">
                                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="Name"
                                                DataValueField="Id" AppendDataBoundItems="true" SelectedValue='<%# Bind("IdLoja") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvLoja" runat="server" ControlToValidate="drpLoja"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr runat="server" visible='<%# ExibirDadosLimiteCheque() %>'>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label10" runat="server" Text="Tipo Pessoa"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTipoPessoa" runat="server" onchange="alteraTipoPessoa()">
                                                <asp:ListItem Value="F">Física</asp:ListItem>
                                                <asp:ListItem Value="J">Jurídica</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label9" runat="server" Text="CPF"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCpfCnpj" runat="server" MaxLength="20" Text='<%# Bind("CpfCnpj") %>'
                                                Columns="20"></asp:TextBox>
                                            <asp:CustomValidator ID="ctvCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                                ControlToValidate="txtCpfCnpj" Display="Dynamic" ErrorMessage="*" ValidateEmptyText="True"></asp:CustomValidator>

                                            <script type="text/javascript">
                                                alteraTipoPessoa();
                                            </script>

                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label40" runat="server" Text="Banco"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtBanco" runat="server" Text='<%# Bind("Banco") %>' Width="150px"
                                                onkeyup="verificaLeituraCheque(this, event);"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqdBanco" runat="server" ControlToValidate="txtBanco"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label39" runat="server" Text="Núm. Cheque"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtNumero" runat="server" MaxLength="6" onkeypress="return soNumeros(event, true, true);"
                                                Text='<%# Bind("Num") %>' Width="100px"></asp:TextBox>
                                            <asp:TextBox ID="txtDigitoNum" runat="server" MaxLength="1" OnLoad="txtDigitoNum_Load"
                                                Text='<%# Bind("DigitoNum") %>' Width="15px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqdNumeroCheque" runat="server" ControlToValidate="txtNumero"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label42" runat="server" Text="Agência"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtAgencia" runat="server" Text='<%# Bind("Agencia") %>' Width="100px"
                                                onkeyup="verificaLeituraCheque(this, event);"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqdAgencia" runat="server" ControlToValidate="txtAgencia"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label43" runat="server" Text="Data Venc."></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc3:ctrlData ID="ctrlData" runat="server" Data='<%# Bind("DataVenc") %>' ExibirHoras="False"
                                                ValidateEmptyText="True" ReadOnly="ReadWrite" />
                                        </td>
                                    </tr>
                                    <tr class="alt">
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label44" runat="server" Text="Conta"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtConta" runat="server" Text='<%# Bind("Conta") %>' Width="100px"
                                                onkeyup="verificaLeituraCheque(this, event);"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label46" runat="server" Text="Valor"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc1:ctrltextboxfloat ID="ctrValor" runat="server" AcceptsEmptyValue="False" Value='<%# Bind("Valor") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrOrigem" runat="server" NameQueryString="origem" Text='<%# Bind("Origem") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrIdAcertoCheque" runat="server" NameQueryString="idAcertoCheque"
                                                Text='<%# Bind("IdAcertoCheque") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrIdContaR" runat="server" NameQueryString="idContaR"
                                                Text='<%# Bind("IdContaR") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdPedido"
                                                Text='<%# Bind("IdPedido") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrlLinkQueryString2" runat="server" NameQueryString="IdAcerto"
                                                Text='<%# Bind("IdAcerto") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrlLinkQueryString3" runat="server" NameQueryString="IdLiberarPedido"
                                                Text='<%# Bind("IdLiberarPedido") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrlLinkQueryString4" runat="server" NameQueryString="IdTrocaDevolucao"
                                                Text='<%# Bind("IdTrocaDevolucao") %>' />
                                            <uc2:ctrllinkquerystring ID="ctrlLinkQueryString5" runat="server" NameQueryString="IdSinal"
                                                Text='<%# Bind("IdSinal") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label2" runat="server" Text="CMC7"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCMC7" runat="server" onkeypress="return soNumeros(event, true, true);" Text='<%# Bind("Cmc7") %>' Width="300px"></asp:TextBox>
                                            <img runat="server" src="~/Images/Help.gif" title="Digite as faixas de valor separando a primeira da segunda com (<) e a segunda da terceira com (>) como no exemplo a seguir: 40903151<0013002665>500074931502" />
                                        </td>
                                    </td>
                                    <tr class="alt">
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label8" runat="server" Text="Obs."></asp:Label>
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="4" Text='<%# Bind("Obs") %>'
                                                TextMode="MultiLine" Width="100%" onkeyup="verificaLeituraCheque(this, event);"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnLoad="btnInserir_Load" />
                                <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" Text="Fechar"
                                    OnClientClick="fechar();" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <asp:Label ID="lblTotal" runat="server" Font-Bold="False" Font-Size="Large"></asp:Label>
                <asp:GridView GridLines="None" ID="grdCheque" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdCheque" DataSourceID="odsCheques"
                    EmptyDataText="Nenhum cheque cadastrado." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkAtualizar" runat="server" CommandName="Update"><img
                                    border="0" src="../Images/Ok.gif" /></asp:LinkButton>
                                <asp:LinkButton ID="lnkCancelar" runat="server" CommandName="Cancel"><img
                                    border="0" src="../Images/ExcluirGrid.gif" /></asp:LinkButton>
                                <asp:HiddenField ID="hdfPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    CausesValidation="false" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Cheque?&quot;);"
                                    ToolTip="Excluir" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num." SortExpression="Num">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" MaxLength="50" Text='<%# Bind("Num") %>'
                                    Width="60px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Num") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtBanco" runat="server" MaxLength="25" Text='<%# Bind("Banco") %>'
                                    Width="90px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Banco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agência" SortExpression="Agencia">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAgencia" runat="server" MaxLength="25" Text='<%# Bind("Agencia") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Agencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta" SortExpression="Conta">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtConta" runat="server" MaxLength="20" Text='<%# Bind("Conta") %>'
                                    Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Conta") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular" SortExpression="Titular">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitular" runat="server" MaxLength="45" Text='<%# Bind("Titular") %>'
                                    Width="170px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Titular") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <uc1:ctrltextboxfloat ID="ctrlTextBoxFloat6" runat="server" Value='<%# Bind("Valor") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Valor", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Venc." SortExpression="DataVenc">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDataVencGrid" runat="server" onkeypress="return false;" Text='<%# Bind("DataVencString") %>'
                                    Width="70px"></asp:TextBox>
                                <asp:ImageButton ID="imgDataVencGrid" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                    ToolTip="Alterar" OnClientClick="return SelecionaData('txtDataVencGrid', this)" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DataVenc", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <table id="tbChequePagto">
                </table>
                <asp:HiddenField ID="hdfPopupCheque" runat="server" />
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server"
        SelectMethod="ObtemLojasAtivas" TypeName="Glass.Global.Negocios.ILojaFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCheques" runat="server" EnablePaging="True"
        MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ChequesDAO"
        DataObjectTypeName="Glass.Data.Model.Cheques" DeleteMethod="Delete" UpdateMethod="Update"
        OnDeleted="odsCheques_Deleted">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
            <asp:QueryStringParameter Name="idAcerto" QueryStringField="idAcerto" Type="UInt32" />
            <asp:QueryStringParameter Name="idContaR" QueryStringField="idContaR" Type="UInt32"
                DefaultValue="" />
            <asp:Parameter Name="idChequeDevolvido" Type="UInt32" />
            <asp:QueryStringParameter DefaultValue="" Name="idLiberarPedido" QueryStringField="idLiberarPedido"
                Type="UInt32" />
            <asp:Parameter DefaultValue="0" Name="situacao" Type="Int32" />
            <asp:QueryStringParameter DefaultValue="" Name="origem" QueryStringField="origem"
                Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCadCheque" runat="server" DataObjectTypeName="Glass.Data.Model.Cheques"
        InsertMethod="Insert" SelectMethod="GetElementByPrimaryKey" TypeName="Glass.Data.DAL.ChequesDAO"
        UpdateMethod="Update" OnInserted="odsCadCheque_Inserted">
        <SelectParameters>
            <asp:QueryStringParameter Name="key" QueryStringField="idCheque" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfTotal" runat="server" />
    <asp:HiddenField ID="hdfLinha" runat="server" />
    <asp:HiddenField ID="hdfIdCliente" runat="server" />

    <script type="text/javascript">
    <% if (!Cadastrar()) %>
    <% { %>

    var tabela = document.getElementById("tbChequePagto");
    var tabelaOpener = window.opener.document.getElementById(nomeTabelaChequesOpener);
    duplicarTabela(tabela, tabelaOpener);
    atualizaTotal();
    alteraTipoPessoa();

    if (<%= (!String.IsNullOrEmpty(Request["editar"])).ToString().ToLower() %>)
        editarItemCheque("tbChequePagto", '<%= Request["editar"] %>', <%= Request["controlPagto"] %>, '', '');

    <% } %>
    <% else %>
    <% { %>

    try
    {
        window.opener.document.getElementById(<%= Request["controlPagto"] %>).value = parseFloat(FindControl("hdfTotal", "input").value).toFixed(2).toString().replace(".", ",");
    }
    catch (err) {}

    <% } %>

    </script>

</asp:Content>
