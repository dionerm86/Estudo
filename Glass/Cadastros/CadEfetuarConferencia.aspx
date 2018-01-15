<%@ Page Title="Efetuar Conferência" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadEfetuarConferencia.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadEfetuarConferencia" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
<script type="text/javascript">

    var countPedidos = 1; // Conta a quantidade de pedidos adicionados ao form
    var idsConfirmado = ""; // Guarda quais ids foram confirmados
    var retificar = "false";
    
    function setPedido(idPedido, loja, nomeCliente, telCliente, localObra, dataEntrega, selPedidosWin) {

        // Verifica se a medicao já foi adicionada
        var pedidos = FindControl("hdfIdPedidos", "input").value.split(',');
        for (i = 0; i < pedidos.length; i++) {
            if (idPedido == pedidos[i]) {
                if (selPedidosWin != null)
                    selPedidosWin.alert("Pedido já adicionado.");
                else
                    alert("Pedido já adicionado.");
                    
                return false;
            }
        }

        // Adicionar idPedido selecionado ao hiddenfield que guarda todos os ids ja selecionados
        FindControl("hdfIdPedidos", "input").value += idPedido + ",";

        // Monta tabela dinamicamente
        tabela = document.getElementById('lstPedidos');

        // Cria títulos para a tabela
        if (countPedidos == 1) {
            tabela.innerHTML = "<tr align=\"left\"><td></td>" +
            "<td style=\"font-weight: bold\">Num. Pedido</td>" +
            "<td style=\"font-weight: bold\">Loja</td>" +
            "<td style=\"font-weight: bold\">Cliente</td>" +
            "<td style=\"font-weight: bold\">Tel. Cliente</td>" +
            "<td style=\"font-weight: bold\">Local Obra</td>" +
            "<td style=\"font-weight: bold\">Data Entrega</td></tr>";
        }

        row = tabela.insertRow(countPedidos);
        row.id = "row" + row.rowIndex;
        row.setAttribute("idPedido", idPedido);
        row.innerHTML =
            "<td><a href=\"#\" onclick=\"return excluirItem(" + row.rowIndex + ");\">" +
            "<img src=\"../Images/ExcluirGrid.gif\" border=\"0\" title=\"Excluir\"/></a></td>" +
            "<td>" + idPedido + "</td><td>" + loja + "</td><td>" + nomeCliente + "</td>" +
            "<td>" + telCliente + "</td><td>" + localObra + "</td><td>" + dataEntrega + "</td>";

        countPedidos++;

        return false;
    }

    function excluirItem(linha) {
        // Exclui pedido do vetor de contas
        var pedidos = FindControl("hdfIdPedidos", "input").value.split(',');
        var pedidoAExcluir = document.getElementById("row" + linha).getAttribute("idPedido");
        var newPedidos = ""; // Novo vetor de contas

        // Cria um novo vetor de pedidos, tirando o id do pedido que foi excluido
        for (i = 0; i < pedidos.length; i++) {
            if (pedidoAExcluir != pedidos[i])
                newPedidos += pedidos[i] + ",";
        }

        // Atribui o novo vetor criado ao hidden field que guarda os ids das contas adicionadas
        FindControl("hdfIdPedidos", "input").value = newPedidos.replace(",,", ",");

        // Exclui o pedido da tabela
        document.getElementById("row" + linha).style.display = "none";

        return false;
    }

    function confirma() {

        var result = getConferente(FindControl('txtNumConferente', 'input'));

        if (result == false)
            return false;

        var idsPedido = FindControl('hdfIdPedidos', 'input').value;
        var idConferente = FindControl('txtNumConferente', 'input').value;
        var dataEfetuar = FindControl('txtDataEfetuar', 'input').value;

        if (idsPedido == "") {
            alert('Nenhum pedido selecionado.');
            return false;
        }
        else if (idConferente == "") {
            alert('Informe o conferente responsável pela conferência dos pedidos.');
            return false;
        }
        else if (dataEfetuar == "") {
            alert('Informe a data que as conferências serão efetuadas.');
            return false;
        }

        var cConfirmar = FindControl("btnConfirmar", "input");
        cConfirmar.disabled = true;

        var retorno = CadEfetuarConferencia.Confirmar(idConferente, idsPedido, dataEfetuar, retificar, idsConfirmado).value;

        if (retorno == null) {
            alert("Falha ao efetuar conferências. Erro: Ajax.");
            cConfirmar.disabled = false;
            return false;
        }

        retorno = retorno.split("\t");

        if (retorno[0] == "ok") {

            alert(retorno[1]);

            idsConfirmado = idsPedido;
            FindControl("lnkImprimir", "a").style.visibility = "visible";
            FindControl("btnRetificar", "input").style.visibility = "visible";
            retificar = "false";
            
            openRpt();
        }
        else if (retorno[0] == "Erro") {
            alert(retorno[1]);
            cConfirmar.disabled = false;
            return false;
        }

        return false;
    }

    function retificarConf() {

        if (!confirm("Retificar Conferências?"))
            return false;
    
        FindControl("btnConfirmar", "input").disabled = false;
        FindControl("btnRetificar", "input").style.visibility = "hidden";
        FindControl("lnkImprimir", "a").style.visibility = "hidden";

        retificar = "true";

        return false;
    }

    // Abre popup para selecionar pedidos
    function openPedido() {
        openWindow(500, 750, "../Utils/SelPedidosConferencia.aspx");
        return false;
    }

    // Busca pedido pelo código informado na textBox
    function getPedido() {
        var idPedido = FindControl("txtNumPedido", "input");

        if (idPedido.value == "")
            return false;

        var retorno = CadEfetuarConferencia.GetPedido(idPedido.value).value.split('\t');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idPedido.value = "";
            return false;
        }

        setPedido(retorno[1], retorno[2], retorno[3], retorno[4], retorno[5], retorno[6], null);

        return false;
    }

    // Busca conferente pelo código do mesmo informado na textbox, ou se a mesma estiver em branco,
    // abre tela para selecioná-lo
    function getConferente(idConferente) {
        if (idConferente.value == "") {
            openWindow(500, 700, "../Utils/SelConferente.aspx");
            return false;
        }

        var retorno = MetodosAjax.GetConferente(idConferente.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idConferente.value = "";
            FindControl("txtNomeConferente", "input").value = "";
            return false;
        }

        FindControl("txtNomeConferente", "input").value = retorno[1];
    }

    // Função utilizada após selecionar conferente no popup, para preencher o id e o nome do mesmo
    // Nas respectivas textboxes deste form
    function setConferente(id, nome) {
        FindControl("txtNumConferente", "input").value = id;
        FindControl("txtNomeConferente", "input").value = nome;
        return false;
    }

    // Abre relatório de conferência
    function openRpt() {
        var idConferente = FindControl("txtNumConferente", "input").value;
        var dataEfetuar = FindControl("txtDataEfetuar", "input").value;

        var queryString = "?Rel=ListaConferencia&IdPedido=0&idLoja=0&NomeCliente=&situacao=2&IdConferente=" + idConferente + 
            "&dataConferencia=" + dataEfetuar + "&sitPedido=0";

        openWindow(600, 800, "../Relatorios/RelBase.aspx" + queryString);
        return false;
    }
    
</script>
    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" 
                                Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeydown="if (isEnter(event)) getPedido();"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqPedido" runat="server" ImageUrl="~/Images/Insert.gif" 
                                            OnClientClick="getPedido(); return false;" 
                                            ToolTip="Adicionar Pedido" Width="16px" />
                        </td>
                        <td>
                            <asp:Button ID="btnBuscarPedido" runat="server" Text="Buscar Pedidos" 
                                OnClientClick="return openPedido();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="lstPedidos" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdPedidos" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" 
                                Text="Conferente"></asp:Label>
                        </td>
                        <td>
&nbsp;<asp:TextBox ID="txtNumConferente" runat="server" Width="61px" onblur="getConferente(this);"></asp:TextBox>
                        </td>
                        <td>
&nbsp;<asp:TextBox ID="txtNomeConferente" runat="server" Width="217px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqMedidor" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                            OnClientClick="getConferente(FindControl('txtNumConferente', 'input')); return false;" 
                                            ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" 
                                Text="Data Conferência"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataEfetuar" runat="server" onkeypress="return false;" 
                                            Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataEfetuar" runat="server" ImageAlign="AbsMiddle" 
                                            ImageUrl="~/Images/calendario.gif" 
                                            OnClientClick="return SelecionaData('txtDataEfetuar', this)" 
                                            ToolTip="Alterar" Width="16px" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClientClick="return confirma();" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <a href="#" id="lnkImprimir" onclick="return openRpt();" style="visibility: hidden">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</a>
                
                <br />
                
                <br />
                
                <asp:Button ID="btnRetificar" runat="server" Text="Retificar" style="visibility: hidden;"
                    OnClientClick="retificarConf(); return false;" />
            </td>
        </tr>
    </table>
</asp:Content>

