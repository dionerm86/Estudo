<%@ Page Title="Retificar Depósito" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadRetificarDepositoCheque.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetificarDepositoCheque" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

    // Busca todos os cheques do deposito informado
    function buscarCheques() {

        var idDeposito = FindControl("txtNumDeposito", "input").value;
        
        if (idDeposito == ""){
            alert("Informe o Número do depósito a ser retificado.");
            return false;
        }
        
        var noCache = new Date();        
        var response = CadRetificarDepositoCheque.GetCheques(idDeposito, noCache.getMilliseconds()).value;

        if (response == null) {
            alert("Falha ao buscar cheques do depósito. AJAX Error.");
            
            // Limpa/Esconde campos utilizados na busca de cheques
            buscarFormBehavior(true);
            
            return false;
        }
        
        response = response.split('\t');
        
        if (response[0] == "Erro"){
            alert(response[1]);

            // Limpa/Esconde campos utilizados na busca de cheques
            buscarFormBehavior(true);
            
            return false;
        }
        
        var retorno = response[1].split('\n');
        var dadosDeposito = retorno[0].split(';');

        // Preenche dados do depósito
        FindControl("drpContaBanco", "select").value = dadosDeposito[0];
        FindControl("ctrlDataDeposito_txtData", "input").value = dadosDeposito[1];
        FindControl("hdfIdDeposito", "input").value = idDeposito;

        // Limpa/Esconde campos utilizados na busca de cheques
        buscarFormBehavior(true);

        try {
            var cheques = retorno[1].split('|');
            
            for (j = 0; j < cheques.length; j++) {
                var items = cheques[j].split(';');
                setCheque(items[0], items[1], items[2], items[3], items[4], items[5], items[6], items[7], null);
            }

            // Exibe campos utilizados na busca de cheques
            buscarFormBehavior(false);
        }
        catch (err) {
            alert("Falha ao buscar cheques. Erro: " + err);

            // Limpa/Esconde campos utilizados na busca de cheques
            buscarFormBehavior(true);
            
            return false;
        }

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

    function retificar() {

        if (!confirm('Tem certeza que deseja Retificar este Depósito?'))
            return false;

        var cConfirmar = FindControl("btnRetificar", "input");
        cConfirmar.disabled = true;

        var idDeposito = FindControl('hdfIdDeposito', 'input').value;
        var idsCheque = FindControl('hdfIdsCheque', 'input').value;
        var idContaBanco = FindControl('drpContaBanco', 'select').value;
        var dataDeposito = FindControl('ctrlDataDeposito_txtData', 'input').value;
        var total = FindControl('lblTotal', 'span').innerHTML;
        var taxaAntecip = FindControl('txtTaxaAntecip', 'input').value;

        if (idDeposito == "") {
            alert("Informe o Número do Depósito que será retificado.");
            cConfirmar.disabled = false;
            return false;
        }
    
        if (idsCheque == "" || idsCheque == null) {
            alert("Busque os cheques do Depósito a ser retificado.");
            cConfirmar.disabled = false;
            return false;
        }

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

        FindControl("hdfIdDeposito", "input").value = idDeposito;

        var noCache = new Date();
        var response = CadRetificarDepositoCheque.Retificar(idDeposito, idsCheque, idContaBanco, dataDeposito, taxaAntecip, total, noCache.getMilliseconds()).value;
        
        if (response == null) {
            alert("Falha ao Retificar Depósito. Erro: AJAX");
            cConfirmar.disabled = false;
            return false;
        }
        
        response = response.split('\t');

        if (response[0] == "ok") {
            alert("Depósito retificado.");
            FindControl("lnkImprimir", "a").style.visibility = "visible";
            openRpt();
        }
        else if (response[0] == "Erro") {
            alert(response[1]);
            cConfirmar.disabled = false;
        }
        else
            alert("Falha ao Retificar Depósito. Erro: Unknow.");

        return false;
    }

    // Função acionada quando o formulário buscou com ou sem sucesso o depósito a ser retificado
    function buscarFormBehavior(hide) {
        if (hide) {
            //countItem = 1; // Foi necessário comentar esta linha pois estava causando erros de layout e falha ao excluir mais de um cheque
            FindControl("lnkImprimir", "a").style.visibility = "hidden";
            FindControl("lnkBuscar", "a").style.visibility = "hidden";
            FindControl("hdfIdsCheque", "input").value = "";
            FindControl("lblTotal", "span").innerHTML = "R$ 0,00";
            document.getElementById('lstCheque').innerHTML = "";
            FindControl("btnRetificar", "input").disabled = true;
        }
        else {
            FindControl("lnkBuscar", "a").style.visibility = "visible";
            FindControl("btnRetificar", "input").disabled = false;
        }
    }

    function limpar() {
        countItem = 1;
        FindControl("lnkImprimir", "a").style.visibility = "hidden";
        FindControl("drpContaBanco", "select").selectedIndex = 0;
        FindControl("hdfIdDeposito", "input").value = "";
        FindControl("hdfIdsCheque", "input").value = "";
        FindControl("ctrlDataDeposito_txtData", "input").value = "";
        FindControl("txtNumDeposito", "input").value = "";
        FindControl("lblTotal", "span").innerHTML = "R$ 0,00";
        FindControl("btnRetificar", "input").disabled = false;
        document.getElementById('lstCheque').innerHTML = "";
    }

    // Abre relatório deste depósito
    function openRpt() {
        var idDeposito = FindControl("hdfIdDeposito", "input").value;

        openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=Deposito&idDeposito=" + idDeposito);
            
        return false;
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Num. Depósito"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumDeposito" runat="server" Width="60px" onkeydown="if (isEnter(event)) buscarCheques();"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnBuscarCheques" runat="server" Text="Buscar Cheques" OnClientClick="buscarCheques(); return false;" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <a href="#" id="lnkBuscar" onclick="return openWindow(500, 700, '../Utils/SelCheque.aspx?tipo=2&situacao=1'); return false;"
                    style="font-size: small;">Incluir Cheques</a>
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
                <table>
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
                        <tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="Data Depósito"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataDeposito" runat="server" ReadOnly="ReadWrite" ExibirHoras="false"
                                ValidateEmptyText="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Taxa Antecip. (R$)"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTaxaAntecip" runat="server" MaxLength="8" Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnRetificar" runat="server" OnClientClick="return retificar();"
                    disabled="true" Text="Retificar" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a id="lnkImprimir" href="#" onclick="return openRpt();"
                    style="visibility: hidden">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>
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

    <script type="text/javascript">

        // Limpa form
        buscarFormBehavior(true);
    
    </script>

</asp:Content>
