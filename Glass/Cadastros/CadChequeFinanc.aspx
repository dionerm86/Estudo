<%@ Page Title="Cadastro de Cheques" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadChequeFinanc.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadChequeFinanc" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function validaPlanoConta(val, args)
        {
            args.IsValid = args.Value != "" || FindControl("drpPlanoConta", "select").disabled;
        }

        function validaContaBanco(val, args)
        {
            args.IsValid = args.Value != "" || FindControl("drpContaBanco", "select").disabled;
        }

        function habilitaPlanoContaBanco(tipo, checked)
        {
            FindControl("chkMovCaixaGeral", "input").checked = tipo == 1 && checked;
            FindControl("chkMovBanco", "input").checked = tipo == 2 && checked;
            FindControl("drpPlanoConta", "select").disabled = !checked;
            FindControl("drpContaBanco", "select").disabled = tipo != 2 || !checked;
        }

        function exibirPlanoContaBanco() {
            var situacao = FindControl("drpSituacao", "select").value;

            // Se situa��o Compensado, exibe as op��es de movimentar Caixa ou Banco
            if (situacao == "1") {
                movCaixa.hidden = true;
                movBanco.hidden = true;
                tabPlanoConta.hidden = false;
                tabContaBanco.hidden = true;

                habilitaPlanoContaBanco(1, true);
            }
            else if (situacao == "2") {
                movCaixa.hidden = false;
                movBanco.hidden = false;
                tabPlanoConta.hidden = false;
                tabContaBanco.hidden = false;

                habilitaPlanoContaBanco(0, false);
            }
            else {
                movCaixa.hidden = true;
                movBanco.hidden = true;
                tabPlanoConta.hidden = true;
                tabContaBanco.hidden = true;

                habilitaPlanoContaBanco(0, false);
            }
        }

        function getCli(idCli)
        {
            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                FindControl("hdfIdCliente", "input").value = "";

                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
            FindControl("hdfIdCliente", "input").value = idCli.value;
        }

        function alteraTipoPessoa()
        {
            var tipoPessoa = FindControl("drpTipoPessoa", "select").value;
            var label = FindControl("Label9", "span");
            var controle = FindControl("txtCpfCnpj", "input");

            controle.value = "";

            if (tipoPessoa == "F")
            {
                label.innerHTML = "CPF";
                controle.setAttribute("onkeydown", "return maskCPF(event, this)");
            }
            else
            {
                label.innerHTML = "CNPJ";
                controle.setAttribute("onkeydown", "return maskCNPJ(event, this)");
            }
        }

        function validaCheque() {
            var idCheque = '<%= Request["IdCheque"] %>';
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var banco = FindControl("txtBanco", "input").value;
            var agencia = FindControl("txtAgencia", "input").value;
            var conta = FindControl("txtConta", "input").value;
            var numCheque = FindControl("txtNumero", "input").value;
            var digitoNum = FindControl("txtDigitoNum", "input").value;

            // Verifica se o cheque j� existe
            var validaCheque = CadChequeFinanc.ValidaCheque(idCheque, idCliente, banco, agencia, conta, numCheque, digitoNum).value.split('|');
            if (validaCheque[0] == "false") {
                alert(validaCheque[1]);
                return false;
            }

            var situacao = FindControl("drpSituacao", "select").value;
            var planoConta = FindControl("drpPlanoConta", "select").value;
            var contaBanco = FindControl("drpContaBanco", "select").value;

            if (situacao == "1" && planoConta == "") {
                alert("O plano de contas precisa ser informado")
                return false;
            }

            FindControl("chkMovCaixaGeral", "input").checked;
            FindControl("chkMovBanco", "input").checked;

            if (situacao == "2") {
                if (FindControl("chkMovBanco", "input").checked && (planoConta == "" || contaBanco == "")) {
                    alert("O plano de contas ou a conta banc�ria n�o foi preenchido")
                    return false;
                }
                else if (FindControl("chkMovCaixaGeral", "input").checked && planoConta == "") {
                    alert("O plano de contas deve ser informado")
                    return false;
                }
            }

            var idCli = FindControl("txtNumCli", "input").value;

            if (idCli != null && idCli != "") {
                FindControl("hdfIdCliente", "input").value = idCli;
            }

            return true;
        }

    </script>

    <table style="width: 100%;">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvCheque" runat="server" AutoGenerateRows="False" DataSourceID="odsCheque"
                    DefaultMode="Insert" GridLines="None" DataKeyNames="IdCheque">
                    <Fields>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label39" runat="server" Text="N�mero"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtNumero" runat="server" MaxLength="50" onkeypress="return soNumeros(event, true, true);"
                                                Text='<%# Bind("Num") %>' Width="100px"></asp:TextBox>
                                            <asp:TextBox ID="txtDigitoNum" runat="server" MaxLength="1" OnLoad="txtDigitoNum_Load"
                                                Text='<%# Bind("DigitoNum") %>' Width="15px"></asp:TextBox>
                                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNumero"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                            <asp:RequiredFieldValidator ID="rqdDigitoNum" runat="server" ControlToValidate="txtDigitoNum"
                                                ErrorMessage="*" OnLoad="rqdDigitoNum_Load"></asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label40" runat="server" Text="Banco"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtBanco" runat="server" onkeyup="verificaLeituraCheque(this, event);" Text='<%# Bind("Banco") %>'
                                                Width="150px"></asp:TextBox>
                                            &nbsp;<asp:RequiredFieldValidator ID="rqdBanco" runat="server" ControlToValidate="txtBanco"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label41" runat="server" Text="Titular"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTitular" runat="server" onkeyup="verificaLeituraCheque(this, event);" Text='<%# Bind("Titular") %>'
                                                Width="300px"></asp:TextBox>
                                            &nbsp;<asp:RequiredFieldValidator ID="rqdTitular" runat="server" ControlToValidate="txtTitular"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label42" runat="server" Text="Ag�ncia"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtAgencia" runat="server" onkeyup="verificaLeituraCheque(this, event);" Text='<%# Bind("Agencia") %>'
                                                Width="100px"></asp:TextBox>
                                            &nbsp;<asp:RequiredFieldValidator ID="rqdAgencia" runat="server" ControlToValidate="txtAgencia"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label10" runat="server" Text="Tipo Pessoa"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="drpTipoPessoa" runat="server" onchange="alteraTipoPessoa()"
                                                SelectedValue='<%# ValorTipoPessoa(Eval("CpfCnpj")) %>'>
                                                <asp:ListItem Value="F">F�sica</asp:ListItem>
                                                <asp:ListItem Value="J">Jur�dica</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label9" runat="server" Text="CPF"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtCpfCnpj" runat="server" Columns="20" MaxLength="20"
                                                Text='<%# Bind("CpfCnpjFormatado") %>'></asp:TextBox>
                                            <asp:CustomValidator ID="ctvCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                                ControlToValidate="txtCpfCnpj" Display="Dynamic" ErrorMessage="*" ValidateEmptyText="True"></asp:CustomValidator>

                                            <script type="text/javascript">
                                                alteraTipoPessoa();
                                                FindControl("txtCpfCnpj", "input").value = '<%# Eval("CpfCnpjFormatado") %>';
                                            </script>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label45" runat="server" Text="Valor"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc1:ctrltextboxfloat ID="ctrValor" runat="server" Value='<%# Bind("Valor") %>'
                                                OnPreRender="ctrValor_PreRender" />
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label44" runat="server" Text="Conta"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtConta" runat="server" onkeyup="verificaLeituraCheque(this, event);" Text='<%# Bind("Conta") %>'
                                                Width="100px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label43" runat="server" Text="Data Venc."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <uc3:ctrlData ID="ctrlData" runat="server" Data='<%# Bind("DataVenc") %>' ReadOnly="ReadWrite"
                                                ValidateEmptyText="true" />
                                        </td>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label46" runat="server" Text="Tipo"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="drpTipo" runat="server"
                                                SelectedValue='<%# Bind("Tipo") %>'>
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="2">Cheque de Terceiros</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqdTipo" runat="server" ControlToValidate="drpTipo"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label47" runat="server" Text="Cliente"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumCli" runat="server" onblur="getCli(this);" onkeydown="if (isEnter(event)) getCli(this);"
                                                onkeypress="return soNumeros(event, true, true);" Text='<%# Eval("IdCliente") %>'
                                                Width="50px"></asp:TextBox>
                                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Width="250px"
                                                Text='<%# Eval("NomeCliente") %>'></asp:TextBox>
                                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" Text="Situa��o"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpSituacao" runat="server"
                                                SelectedValue='<%# Bind("Situacao") %>'>
                                                <asp:ListItem Value="1">Em aberto</asp:ListItem>
                                                <asp:ListItem Value="7">Protestado</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label2" runat="server" Text="CMC7"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCMC7" runat="server" Text='<%# Bind("Cmc7") %>' Width="300px"></asp:TextBox>
                                            <img runat="server" src="~/Images/Help.gif" title="Digite as faixas de valor separando a primeira da segunda com (<) e a segunda da terceira com (>) como no exemplo a seguir: 40903151<0013002665>500074931502" />
                                        </td>
                                    </td>
                                    <tr class="alt">
                                        <td align="left" class="dtvHeader">
                                            Obs.
                                        </td>
                                        <td align="left" colspan="3" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtObs" runat="server" Width="100%" TextMode="MultiLine" Rows="4"
                                                Text='<%# Bind("Obs") %>' onkeyup="verificaLeituraCheque(this, event);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="4">
                                            <asp:Label ID="lblMovCaixaGeral" runat="server" Text='<%# Eval("DescrMovCaixaEBanco") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="4">
                                            <asp:Label ID="lblPlanoContas" runat="server" Text='<%# Eval("DescrPlanoConta") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="4">
                                            <asp:Label ID="lblContaBanco" runat="server" Text='<%# Eval("DescrContaBanco") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="4" nowrap="nowrap">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td>
                                                        <asp:HiddenField ID="hdfIdDeposito" runat="server" Value='<%# Bind("IdDeposito") %>' />
                                                    </td>
                                                    <td>
                                                        <asp:HiddenField ID="hdfIdOrigem" runat="server" Value='<%# Bind("Origem") %>' />
                                                    </td>
                                                    <td>
                                                        <asp:HiddenField ID="hdfPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                                    </td>
                                                    <td>
                                                        <asp:HiddenField ID="hdfMovCaixaFinanceiro" runat="server" Value='<%# Bind("MovCaixaFinanceiro") %>' />
                                                    </td>
                                                    <td>
                                                        <asp:HiddenField ID="hdfMovBanco" runat="server" Value='<%# Bind("MovBanco") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label39" runat="server" Text="N�mero"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtNumero" runat="server" MaxLength="50" Text='<%# Bind("Num") %>'
                                                onkeypress="return soNumeros(event, true, true);" Width="100px"></asp:TextBox>
                                            <asp:TextBox ID="txtDigitoNum" runat="server" MaxLength="1" OnLoad="txtDigitoNum_Load"
                                                Text='<%# Bind("DigitoNum") %>' Width="15px"></asp:TextBox>
                                            &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNumero"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                            <asp:RequiredFieldValidator ID="rqdDigitoNum" runat="server" ControlToValidate="txtDigitoNum"
                                                ErrorMessage="*" OnLoad="rqdDigitoNum_Load"></asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label40" runat="server" Text="Banco"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtBanco" runat="server" onkeyup="verificaLeituraCheque(this, event);" Text='<%# Bind("Banco") %>'
                                                Width="150px"></asp:TextBox>
                                            &nbsp;<asp:RequiredFieldValidator ID="rqdBanco" runat="server" ControlToValidate="txtBanco"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label41" runat="server" Text="Titular"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTitular" runat="server" onkeyup="verificaLeituraCheque(this, event);" Text='<%# Bind("Titular") %>'
                                                Width="300px"></asp:TextBox>
                                            &nbsp;<asp:RequiredFieldValidator ID="rqdTitular" runat="server" ControlToValidate="txtTitular"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label42" runat="server" Text="Ag�ncia"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtAgencia" runat="server" onkeyup="verificaLeituraCheque(this, event);" Text='<%# Bind("Agencia") %>'
                                                Width="100px"></asp:TextBox>
                                            &nbsp;<asp:RequiredFieldValidator ID="rqdAgencia" runat="server" ControlToValidate="txtAgencia"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label10" runat="server" Text="Tipo Pessoa"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="drpTipoPessoa" runat="server" onchange="alteraTipoPessoa()"
                                                SelectedValue='<%# ValorTipoPessoa(Eval("CpfCnpj")) %>'>
                                                <asp:ListItem Value="F">F�sica</asp:ListItem>
                                                <asp:ListItem Value="J">Jur�dica</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label9" runat="server" Text="CPF"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtCpfCnpj" runat="server" Columns="20" MaxLength="20"
                                                Text='<%# Bind("CpfCnpj") %>'></asp:TextBox>
                                            <asp:CustomValidator ID="ctvCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                                ControlToValidate="txtCpfCnpj" Display="Dynamic" ErrorMessage="*" ValidateEmptyText="True"></asp:CustomValidator>

                                            <script type="text/javascript">
                                                alteraTipoPessoa();
                                            </script>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label45" runat="server" Text="Valor"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc1:ctrltextboxfloat ID="ctrValor" runat="server" Value='<%# Bind("Valor") %>' />
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="Label44" runat="server" Text="Conta"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtConta" runat="server" onkeyup="verificaLeituraCheque(this, event);" Text='<%# Bind("Conta") %>'
                                                Width="100px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqdConta" runat="server" ControlToValidate="txtConta"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label43" runat="server" Text="Data Venc."></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <uc3:ctrlData ID="ctrlData" runat="server" Data='<%# Bind("DataVenc") %>' ReadOnly="ReadWrite"
                                                ValidateEmptyText="true" />
                                        </td>
                                        <td align="left" nowrap="nowrap" class="dtvHeader">
                                            <asp:Label ID="Label46" runat="server" Text="Tipo"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <asp:DropDownList ID="drpTipo" runat="server"
                                                SelectedValue='<%# Bind("Tipo") %>'>
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem Value="2">Cheque de Terceiros</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;<asp:RequiredFieldValidator ID="rqdTipo" runat="server" ControlToValidate="drpTipo"
                                                ErrorMessage="*"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label47" runat="server" Text="Cliente"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumCli" runat="server" onblur="getCli(this);" onkeydown="if (isEnter(event)) getCli(this);"
                                                onkeypress="return soNumeros(event, true, true);" Text='<%# Eval("IdCliente") %>'
                                                Width="50px"></asp:TextBox>
                                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Width="250px"></asp:TextBox>
                                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" Text="Situa��o"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="drpSituacao" runat="server" onchange="exibirPlanoContaBanco()"
                                                SelectedValue='<%# Bind("Situacao") %>'>
                                                <asp:ListItem Value="1">Em aberto</asp:ListItem>
                                                <asp:ListItem Value="2">Compensado</asp:ListItem>
                                                <asp:ListItem Value="7">Protestado</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label2" runat="server" Text="CMC7"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCMC7" runat="server" Text='<%# Bind("Cmc7") %>' Width="300px"></asp:TextBox>
                                            <img runat="server" src="~/Images/Help.gif" title="Digite as faixas de valor separando a primeira da segunda com (<) e a segunda da terceira com (>) como no exemplo a seguir: 40903151<0013002665>500074931502" />
                                        </td>
                                    </td>
                                    <tr class="alt">
                                        <td class="dtvHeader" align="left">
                                            Obs.
                                        </td>
                                        <td align="left" colspan="3" class="dtvAlternatingRow">
                                            <asp:TextBox ID="txtObs" runat="server" Width="100%" TextMode="MultiLine" Rows="4"
                                                Text='<%# Bind("Obs") %>' onkeyup="verificaLeituraCheque(this, event);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="movCaixa" align="left" colspan="4">
                                            <asp:CheckBox ID="chkMovCaixaGeral" runat="server" Checked='<%# Bind("MovCaixaFinanceiro") %>'
                                                onclick="habilitaPlanoContaBanco(1, this.checked)" Text="Gerar movimenta��o no caixa geral" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="movBanco" align="left" colspan="4">
                                            <asp:CheckBox ID="chkMovBanco" runat="server" Checked='<%# Bind("MovBanco") %>' onclick="habilitaPlanoContaBanco(2, this.checked)"
                                                Text="Gerar movimenta��o na conta banc�ria" />
                                        </td>
                                        </tr>
                                    <tr>
                                        <td align="left" colspan="4">
                                            <table id="tabPlanoConta">
                                                <tr>
                                                    <td align="left">
                                                        Plano de conta
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpPlanoConta" runat="server" DataSourceID="odsPlanoContas"
                                                            DataTextField="DescrPlanoGrupo" DataValueField="IdConta" SelectedValue='<%# Bind("IdConta") %>'
                                                            AppendDataBoundItems="True">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                        &nbsp;
                                                        <asp:CustomValidator ID="ctvPlanoConta" runat="server" ErrorMessage="*" ClientValidationFunction="validaPlanoConta"
                                                            ControlToValidate="drpPlanoConta" Display="Dynamic" ValidateEmptyText="True"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tabContaBanco">
                                                <tr>
                                                    <td align="left">Conta banc�ria
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                                            DataTextField="Descricao" DataValueField="IdContaBanco" SelectedValue='<%# Bind("IdContaBanco") %>'
                                                            AppendDataBoundItems="True">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                        &nbsp;
                                                        <asp:CustomValidator ID="ctvContaBanco" runat="server" ErrorMessage="*" ClientValidationFunction="validaContaBanco"
                                                            ControlToValidate="drpContaBanco" Display="Dynamic" ValidateEmptyText="True"></asp:CustomValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar" OnClientClick="return validaCheque();" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="false" OnClick="btnCancelar_Click"
                                    Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return validaCheque();" />
                                <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" Text="Cancelar"
                                    OnClick="btnCancelar_Click" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCheque" runat="server" DataObjectTypeName="Glass.Data.Model.Cheques"
                    InsertMethod="InsertFinanc" SelectMethod="GetForFinanceiro" TypeName="Glass.Data.DAL.ChequesDAO"
                    UpdateMethod="Update" OnInserted="odsCheque_Inserted" OnUpdated="odsCheque_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idCheque" QueryStringField="idCheque" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContas" runat="server" SelectMethod="GetPlanoContas"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        try
        {
            habilitaPlanoContaBanco(0, false);
            exibirPlanoContaBanco();
        }
        catch (err) { }
    </script>

</asp:Content>
