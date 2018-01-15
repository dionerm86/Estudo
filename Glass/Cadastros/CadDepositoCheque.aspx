<%@ Page Title="Efetuar Depósito" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadDepositoCheque.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDepositoCheque" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function getCheques() {
            var numCheque = FindControl("txtNumCheque", "input").value;

            if (numCheque == "") {
                alert("Informe o número do cheque.");
                return false;
            }

            var response = CadDepositoCheque.GetCheques(numCheque).value;

            if (response == null) {
                alert("Falha ao efetuar Depósito. Erro: AJAX");
                return false;
            }

            response = response.split('\t');

            if (response[0] == "ok") {
                var cheques = response[1].split('|');
                
                for (var i=0; i<cheques.length; i++)
                {
                    var dadosCheque = cheques[i].split(';');
                    setCheque(dadosCheque[0], dadosCheque[1], dadosCheque[2], dadosCheque[3], dadosCheque[4], dadosCheque[5], 
                        dadosCheque[6], dadosCheque[7], null);
                }
            }
            else
                alert(response[1]);

            FindControl("txtNumCheque", "input").value = "";
            FindControl("txtNumCheque", "input").focus();

            return false;
        }

        function setCheque(idCheque, numCheque, titular, banco, agencia, conta, valor, dataVenc, selChequeWin) {

            // Verifica se o cheque já foi adicionado
            var cheques = FindControl("hdfIdsCheque", "input").value.split(',');
            for (i = 0; i < cheques.length; i++) {
                if (idCheque == cheques[i]) {
                    if (selChequeWin != null)
                        selChequeWin.alert("Cheque já incluído.");
                        
                    return false;
                }
            }

            // Adiciona item à tabela
            addItem(new Array(numCheque, titular, banco, agencia, conta, valor, dataVenc),
                new Array('Número', 'Titular', 'Banco', 'Agência', 'Conta', 'Valor', 'Vencimento'),
                'lstCheque', idCheque, 'hdfIdsCheque', valor, 'lblTotal');

            return false;
        }

        function confirmar() {

            if (!confirm('Tem certeza que deseja efetuar este Depósito?'))
                return false;

            var cConfirmar = FindControl("btnConfirmar", "input");
            cConfirmar.disabled = true;

            var idsCheque = FindControl('hdfIdsCheque', 'input').value;
            var dataDeposito = FindControl('ctrlDataDeposito_txtData', 'input').value;
            var taxaAntecip = FindControl("txtTaxaAntecip", "input").value;
            var total = FindControl('lblTotal', 'span').innerHTML;
            var obs = FindControl("txtObs", "textarea").value;

            if (idsCheque == "" || idsCheque == null) {
                alert("Busque pelo menos um cheque antes de efetuar o Depósito.");
                cConfirmar.disabled = false;
                return false;
            }
            
            var idContaBanco = FindControl('drpContaBanco', 'select').value;
            
            if (idContaBanco == "") {
                alert("Informe a Conta Bancária na qual será feito o Depósito.");
                cConfirmar.disabled = false;
                return false;
            }

            if (dataDeposito == "") {
                alert("Informe a Data do Depósito.");
                cConfirmar.disabled = false;
                return false;
            }

            var noCache = new Date();
            var response = CadDepositoCheque.Confirmar(idsCheque, idContaBanco, dataDeposito, taxaAntecip, total, obs, noCache.getMilliseconds()).value;

            if (response == null) {
                alert("Falha ao efetuar Depósito. Erro: AJAX");
                cConfirmar.disabled = false;
                return false;
            }

            response = response.split('\t');

            if (response[0] == "ok") {
                alert("Depósito efetuado. Número: " + response[1] + ".");
                FindControl("hdfIdDeposito", "input").value = response[1];
                FindControl("lnkImprimir", "a").style.visibility = "visible";
                FindControl("btnNovoDeposito", "input").style.visibility = "visible";
                
                openRpt();

                limpar();
            }
            else if (response[0] == "Erro") {
                alert(response[1]);
                cConfirmar.disabled = false;
            }
            else
                alert("Falha ao Efetuar Depósito. Erro: Unknown.");

            return false;
        }

        function limpar() {
            countItem = new Object();
            FindControl("lnkImprimir", "a").style.visibility = "hidden";
            FindControl("btnNovoDeposito", "input").style.visibility = "hidden";
            FindControl("drpContaBanco", "select").selectedIndex = 0;
            FindControl("hdfIdDeposito", "input").value = "";
            FindControl("hdfIdsCheque", "input").value = "";
            FindControl("ctrlDataDeposito_txtData", "input").value = '<%= DateTime.Now.ToString("dd/MM/yyyy") %>';
            FindControl("txtTaxaAntecip", "input").value = "";
            FindControl("lblTotal", "span").innerHTML = "R$ 0,00";
            FindControl("btnConfirmar", "input").disabled = false;
            document.getElementById('lstCheque').innerHTML = "";
        }

        // Abre relatório deste depósito
        function openRpt() {
            var idDeposito = FindControl("hdfIdDeposito", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=Deposito&idDeposito=" + idDeposito);
            return false;
        }

        function getSelChequeUrl() {
            return '../Utils/SelCheque.aspx?tipo=2&situacao=1';
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label21" runat="server" ForeColor="#0066FF" Text="Num. Cheque"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCheque" runat="server" Width="60px" onkeydown="if (isEnter(event)) getCheques();"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgNumCheque" runat="server" ImageUrl="~/Images/Insert.gif"
                                Width="16px" OnClientClick="getCheques(); return false;" ToolTip="Adicionar Cheque" />
                        </td>
                        <td>
                            <a href="#" onclick="return openWindow(500, 850, getSelChequeUrl()); return false;"
                                style="font-size: small;">Buscar Cheques para Depósito</a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="lstCheque" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="Label18" runat="server" Text="Total: " Font-Size="Large"></asp:Label>
                <asp:Label ID="lblTotal" runat="server" Text="R$ 0,00" Font-Size="Large"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table runat="server" id="contaBanco">
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" ForeColor="#0066FF" Text="Conta Bancária"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" DataSourceID="odsContaBanco"
                                DataTextField="Descricao" DataValueField="IdContaBanco">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblDataDeposito" runat="server" ForeColor="#0066FF" Text="Data Depósito"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataDeposito" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" ValidateEmptyText="true"/>
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Taxa Antecip. (R$)"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTaxaAntecip" runat="server" MaxLength="8" Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Observação"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnConfirmar" runat="server" OnClientClick="return confirmar();"
                    Text="Confirmar" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a id="lnkImprimir" href="#" onclick="return openRpt();"
                    style="visibility: hidden">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>
                <br />
                <br />
                <asp:Button ID="btnNovoDeposito" runat="server" OnClientClick="limpar(); return false;"
                    Style="visibility: hidden" Text="Novo Depósito" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdsCheque" runat="server" />
                <asp:HiddenField ID="hdfIdDeposito" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
