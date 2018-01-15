<%@ Page Title="Cheques para Pagamento" Language="C#" AutoEventWireup="true" CodeBehind="CadChequePagto.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadChequePagto" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
        var nomeTabelaChequesOpener = <%= !String.IsNullOrEmpty(Request["tabelaCheque"]) ? Request["tabelaCheque"] : "'tbChequePagto'" %>;

        function insertCheque() {
            var contaBanco = FindControl("drpContaBanco", "select");
            contaBanco = contaBanco != null ? contaBanco.value : null;
            var numCheque = FindControl("txtNumero", "input").value;
            var titular = FindControl("txtTitular", "input").value;
            var valor = FindControl("txtValor", "input").value;
            var dataVenc = FindControl("txtDataVenc", "input").value;
            var banco = FindControl("txtBanco", "input").value;
            var agencia = FindControl("txtAgencia", "input").value;
            var conta = FindControl("txtConta", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var linha = FindControl("hdfLinha", "input").value;

            if (contaBanco == "")
            {
                alert("Selecione a conta bancária do cheque.");
                return false;
            }
            else if (contaBanco == null)
                contaBanco = "";

            if (numCheque == "") {
                alert("Informe o número do cheque.");
                return false;
            }

            if (titular == "") {
                alert("Informe o titular do cheque.");
                return false;
            }

            if (valor == "") {
                alert("Informe o valor do cheque.");
                return false;
            }

            if (dataVenc == "") {
                alert("Informe a data de vencimento do cheque.");
                return false;
            }

            var callbackIncluir = <%= Request["callbackIncluir"] %>;
        var callbackExcluir = <%= Request["callbackExcluir"] %>;
        var nomeControleFormaPagto = <%= Request["nomeControleFormaPagto"] != null ? Request["nomeControleFormaPagto"] : "''" %>;
                
        // Verifica se o cheque já existe
        var validaCheque = CadChequePagto.ValidaCheque(banco, agencia, conta, numCheque).value.split('|');
        if (validaCheque[0] == "false")
        {
            alert(validaCheque[1]);
            return false;
        }
        
        window.opener.setCheque(nomeTabelaChequesOpener, null, contaBanco, numCheque, null, titular, valor, dataVenc, banco, agencia, conta, situacao, '', window, "proprio",
                4, null, null, null, null, null, null, null, null, '', '', <%= Request["controlPagto"] %>, linha, callbackIncluir, callbackExcluir, nomeControleFormaPagto);
            
            window.opener.loadDropCheque();
            
            var tabela = document.getElementById("tbChequePagto");
            var tabelaOpener = window.opener.document.getElementById(nomeTabelaChequesOpener);
            duplicarTabela(tabela, tabelaOpener);
            
            return false;
        }

        function limpar() {
            FindControl("txtNumero", "input").value = "";
            FindControl("txtValor", "input").value = "";
            FindControl("txtDataVenc", "input").value = "";
        }
    </script>

    <table style="width: 100%;">
        <tr id="contasBanco" runat="server">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label46" runat="server" ForeColor="#0066CC" Text="Selecione a conta bancária"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco"
                                OnSelectedIndexChanged="drpContaBanco_SelectedIndexChanged">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
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
                <br />
                <table cellspacing="0" border="0" id="dtvCheque" style="border-collapse: collapse;">
                    <tr>
                        <td>
                            <table align="left" cellpadding="2" cellspacing="0" style="width: 100%">
                                <tr>
                                    <td align="left" class="dtvHeader">
                                        <span id="dtvCheque_Label39">Número</span>
                                    </td>
                                    <td align="left" class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtNumero" MaxLength="50" onkeypress="return soNumeros(event, true, true);"
                                            Style="width: 100px;" runat="server"></asp:TextBox>
                                    </td>
                                    <td align="left" class="dtvHeader">
                                        <span id="dtvCheque_Label40">Banco</span>
                                    </td>
                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                        <asp:TextBox ID="txtBanco" runat="server" MaxLength="25" Enabled="false" Width="150px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                        <span id="dtvCheque_Label41">Titular</span>
                                    </td>
                                    <td align="left" nowrap="nowrap">
                                        <asp:TextBox ID="txtTitular" MaxLength="45" Style="width: 300px;" runat="server"></asp:TextBox>
                                    </td>
                                    <td align="left" class="dtvHeader">
                                        <span id="dtvCheque_Label42">Agência</span>
                                    </td>
                                    <td align="left" nowrap="nowrap">
                                        <asp:TextBox ID="txtAgencia" MaxLength="25" Style="width: 100px;" runat="server"
                                            Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                        <span id="dtvCheque_Label45">Valor</span>
                                    </td>
                                    <td align="left" class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtValor" MaxLength="20" runat="server" OnKeyPress="return soNumeros(event, false, false);"
                                            Style="width: 80px;"></asp:TextBox>
                                    </td>
                                    <td align="left" class="dtvHeader">
                                        <span id="dtvCheque_Label44">Conta</span>
                                    </td>
                                    <td align="left" class="dtvAlternatingRow">
                                        <asp:TextBox ID="txtConta" MaxLength="20" Style="width: 100px;" runat="server" Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                        <span id="dtvCheque_Label43">Data Venc.</span>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtDataVenc" runat="server" onkeypress="return false;"></asp:TextBox>
                                        <asp:ImageButton ID="imgDataVencGrid" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                            ToolTip="Alterar" OnClientClick="return SelecionaData('txtDataVenc', this)" />
                                    </td>
                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                        <span id="Span1">Situação</span>
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList ID="drpSituacao" runat="server">
                                            <asp:ListItem Value="1">Aberto</asp:ListItem>
                                            <asp:ListItem Value="2">Compensado</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <br />
                            <asp:Button ID="btnInserir" runat="server" Text="Inserir" OnClientClick="return insertCheque();" />
                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" Text="Fechar"
                                OnClientClick="closeWindow();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <table id="tbChequePagto">
                </table>
                <asp:HiddenField ID="hdfPopupCheque" runat="server" />
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfTotal" runat="server" />
    <asp:HiddenField ID="hdfLinha" runat="server" />
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.ContaBancoDAO"></colo:VirtualObjectDataSource>
    </form>
    <script type="text/javascript">
        var tabela = document.getElementById("tbChequePagto");
        var tabelaOpener = window.opener.document.getElementById(nomeTabelaChequesOpener);
        duplicarTabela(tabela, tabelaOpener);
        
        if (<%= (!String.IsNullOrEmpty(Request["editar"])).ToString().ToLower() %>)
            editarItemCheque("tbChequePagto", '<%= Request["editar"] %>', <%= Request["controlPagto"] %>, '', '');
    </script>
</asp:Content>