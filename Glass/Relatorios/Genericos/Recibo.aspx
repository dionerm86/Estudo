<%@ Page Title="Impressão de Recibo" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="Recibo.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Genericos.Recibo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
<script type="text/javascript">
    var isLiberacao = Recibo.IsLiberacao().value;
    
    function openRpt() {
        var idOrcamento = FindControl("txtNumOrcamento", "input");
        idOrcamento = idOrcamento != null ? idOrcamento.value : "0";

        var idPedido = FindControl("txtNumPedido", "input");
        idPedido = idPedido != null ? idPedido.value : "0";

        var idLiberacao = FindControl("txtNumLiberacao", "input");
        idLiberacao = idLiberacao != null ? idLiberacao.value : "0";
        
        var referente = FindControl("drpReferente", "select").value;
        var motivoRef = FindControl("txtOutro", "textarea").value.replace(/\n/g, "\\n");
        var valorRef = FindControl("txtValor", "textarea").value.replace(/\n/g, "\\n");
        var idPgAntecipado = FindControl("txtNumPgAntecipado", "input").value;
        var tdPgAntecipado = FindControl("tituloPgAntecipado", "td");
        var idAcerto = FindControl("txtNumAcerto", "input").value;
        var tdAcerto = FindControl("tituloAcerto", "td");
        var idContaPagar = FindControl("txtNumContaPagar", "input").value;
        var tdContaPagar = FindControl("tituloContaPagar", "td");

        var validaDados = Recibo.ValidaDados(idOrcamento, idPedido, idLiberacao);
        if (validaDados.value != '')
        {
            alert(validaDados.value);
            return false;
        }

        if (idContaPagar == "" && tdContaPagar.style.display != "none") {
            alert("Informe o número da conta paga que será emitido o recibo.");
            return false;
        }
        
        if(idPgAntecipado == "" && tdPgAntecipado.style.display != "none")
        {
            alert("Informe o número do pagamento antecipado que será emitido o recibo.");
            return false;
        }
        
        if(idAcerto == "" && tdAcerto.style.display != "none")
        {
            alert("Informe o número do acerto que será emitido o recibo.");
            return false;
        }
        
        if (idOrcamento == "0")
        {
            if (idPedido == "0" && FindControl("tituloPedido", "td").style.display != "none")
            {
                alert("Informe o número do pedido que será emitido o recibo.");
                return false;
            }

            if (idLiberacao == "0" && FindControl("tituloLiberacao", "td").style.display != "none")
            {
                alert("Informe o número da liberação que será emitida o recibo.");
                return false;
            }
        }

        if (idContaPagar != "" && tdContaPagar.style.display != "none") {
            var estaPaga = Recibo.ContaEstaPaga(idContaPagar);
            if (estaPaga.value == "false") {
                alert("Não existe conta paga com o número " + idContaPagar);
                return false;
            }
        }

        var orcamento = "idOrcamento=" + idOrcamento;
        var pedido = "idPedido=" + idPedido;
        var liberacao = "idLiberacao=" + idLiberacao;
        var pagtAntecipado = "idPgAntecipado=" + (idPgAntecipado != "" ? idPgAntecipado : "0");
        var acerto = "idAcerto=" + (idAcerto != "" ? idAcerto : "0");
        var contaPagar = "idContaPagar=" + (idContaPagar != "" ? idContaPagar : "0");

        var tipoRel = "recibo";

        if(idPgAntecipado != "")
            tipoRel = "reciboPgAntec";
        if(idAcerto != "")
            tipoRel = "reciboAcerto";
        if (idContaPagar != "")
            tipoRel = "reciboContaPagar";

        openWindow(600, 800, "RelBase.aspx?rel=" + tipoRel + "&" + orcamento + "&" + pedido + "&" + liberacao + "&referente=" + referente +
            "&motivoRef=" + motivoRef + "&valorRef=" + valorRef + "&" + pagtAntecipado + "&" + acerto + "&" + contaPagar);

        if (FindControl("txtNumOrcamento", "input") != null)
            FindControl("txtNumOrcamento", "input").value = "";

        if (FindControl("txtNumPedido", "input") != null)
            FindControl("txtNumPedido", "input").value = "";

        if (FindControl("txtNumLiberacao", "input") != null)
            FindControl("txtNumLiberacao", "input").value = "";

        if (FindControl("txtOutro", "textarea") != null)
            FindControl("txtOutro", "textarea").value = "";

        if (FindControl("txtValor", "textarea") != null)
            FindControl("txtValor", "textarea").value = "";

        if (FindControl("txtNumPgAntecipado", "input") != null)
            FindControl("txtNumPgAntecipado", "input").value = "";

        if (FindControl("txtNumAcerto", "input") != null)
            FindControl("txtNumAcerto", "input").value = "";

        if (FindControl("txtNumContaPagar", "input") != null)
            FindControl("txtNumContaPagar", "input").value = "";
        
        return false;
    }

    function getProd() {
        var idOrcamento = FindControl("txtIdOrcamento", "input").value;

        if (idOrcamento == "") {
            alert("Informe o número do orçamento");
            return false;
        }

        var response = Recibo.GetOrcaProd(idOrcamento).value;

        if (response == null || response == "") {
            alert("AJAX Error.");
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            return false;
        }

        var lstProd = response[1].split('|');

        for (var i = 0; i < lstProd.length; i++)
            addItem(new Array(lstProd[i]), new Array('Produto'), 'lstProd', null, null, null, null);

        return false;
    }

    function getProdPedido() {
        var idPedido = FindControl("txtIdPedido", "input").value;

        if (idPedido == "") {
            alert("Informe o número do pedido.");
            return false;
        }

        var response = Recibo.GetPedidoProd(idPedido).value;

        if (response == null || response == "") {
            alert("AJAX Error.");
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            return false;
        }

        var lstProd = response[1].split('|');

        for (var i = 0; i < lstProd.length; i++)
            addItem(new Array(lstProd[i]), new Array('Produto'), 'lstProd', null, null, null, null);

        return false;
    }

    function addProd() {
        var descr = FindControl("txtProd", "textarea").value;
        // Adiciona item à tabela
        addItem(new Array(descr), new Array('Produto'), 'lstProd', null, null, null, null);

        return false;
    }

    function AlteraVisibilidadeControle(idTdTitulo, idTdTextBox, idTextBox, visivel)
    {
        // Exibe os controles informados por parâmetro.
        if (visivel)
        {
            // Controle que engloba o text box.
            if (FindControl(idTdTextBox, "td") != null)
                FindControl(idTdTextBox, "td").style.display = "";

            // Controle que engloba o título.
            if (FindControl(idTdTitulo, "td") != null)
                FindControl(idTdTitulo, "td").style.display = "";
        }
        // Esconde os controles informados por parâmetro.
        else
        {
            // Controle que engloba o text box.
            if (FindControl(idTdTextBox, "td") != null)
                FindControl(idTdTextBox, "td").style.display = "none";

            // Controle que engloba o título.
            if (FindControl(idTdTitulo, "td") != null)
                FindControl(idTdTitulo, "td").style.display = "none";

            // Controle referente ao text box.
            if (FindControl(idTextBox, "input") != null)
                FindControl(idTextBox, "input").value = "";
        }
    }

    function drpReferenteChanged()
    {
        var valor = FindControl("drpReferente", "select").value;
        document.getElementById("linhaParcelas").style.display = valor == 4 ? "" : "none";
        document.getElementById("tbReferente").style.display = valor == 5 ? "" : "none";
        document.getElementById("trProduto").style.display = valor == 8 ? "none" : "";
        
        // Orçamento.
        if (valor == 9)
        {
            // Exibe os campos do orçamento.
            AlteraVisibilidadeControle("tituloOrcamento", "orcamento", "txtNumOrcamento", true);
            
            // Esconde os demais campos.
            AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", false);
            AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", false);
            AlteraVisibilidadeControle("tituloContaPagar", "contaPagar", "txtNumContaPagar", false);
            AlteraVisibilidadeControle("tituloPgAntecipado", "pgAntecipado", "txtNumPgAntecipado", false);
            AlteraVisibilidadeControle("tituloAcerto", "acerto", "txtNumAcerto", false);
        }
        // Pagto. Antecipado/Sinal.
        else if(valor == 6)
        {
            // Exibe os campos do pagto. antecipado/sinal.
            AlteraVisibilidadeControle("tituloPgAntecipado", "pgAntecipado", "txtNumPgAntecipado", true);

            // Esconde os demais campos.
            AlteraVisibilidadeControle("tituloOrcamento", "orcamento", "txtNumOrcamento", false);
            AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", false);
            AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", false);
            AlteraVisibilidadeControle("tituloAcerto", "acerto", "txtNumAcerto", false);
            AlteraVisibilidadeControle("tituloContaPagar", "contaPagar", "txtNumContaPagar", false);
        }
        // Acerto.
        else if(valor == 7)
        {
            // Exibe os campos do acerto.
            AlteraVisibilidadeControle("tituloAcerto", "acerto", "txtNumAcerto", true);

            // Esconde os demais campos.
            AlteraVisibilidadeControle("tituloOrcamento", "orcamento", "txtNumOrcamento", false);
            AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", false);
            AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", false);
            AlteraVisibilidadeControle("tituloPgAntecipado", "pgAntecipado", "txtNumPgAntecipado", false);
            AlteraVisibilidadeControle("tituloContaPagar", "contaPagar", "txtNumContaPagar", false);
        }
        // Conta a pagar.
        else if (valor == 8)
        {
            // Exibe os campos da conta a pagar.
            AlteraVisibilidadeControle("tituloContaPagar", "contaPagar", "txtNumContaPagar", true);

            // Esconde os demais campos.
            AlteraVisibilidadeControle("tituloOrcamento", "orcamento", "txtNumOrcamento", false);
            AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", false);
            AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", false);
            AlteraVisibilidadeControle("tituloPgAntecipado", "pgAntecipado", "txtNumPgAntecipado", false);
            AlteraVisibilidadeControle("tituloAcerto", "acerto", "txtNumAcerto", false);
        }
        else
        {
            // Esconde os campos do orçamento.
            AlteraVisibilidadeControle("tituloOrcamento", "orcamento", "txtNumOrcamento", false);
            // Esconde os campos do pagto. antecipado/sinal.
            AlteraVisibilidadeControle("tituloPgAntecipado", "pgAntecipado", "txtNumPgAntecipado", false);
            // Esconde os campos do acerto.
            AlteraVisibilidadeControle("tituloAcerto", "acerto", "txtNumAcerto", false);
            // Esconde os campos da conta a pagar.
            AlteraVisibilidadeControle("tituloContaPagar", "contaPagar", "txtNumContaPagar", false);

            if (valor == 5) {
                // Esconde os campos do pedido.
                AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", false);
                // Esconde os campos da liberação.
                AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", false);
            }
            // Se não for orçamento.
            else if (valor != 9) {
                if (isLiberacao) {
                    // Esconde os campos do pedido.
                    AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", false);
                    // Exibe os campos da liberação.
                    AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", true);
                }
                else {
                    // Exibe os campos do pedido.
                    AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", true);
                    // Esconde os campos da liberação.
                    AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", false);
                }
            }
        }

        if (isLiberacao && valor != 5 && valor != 9) {

            if (valor == 1) {
                // Exibe os campos do pedido.
                AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", true);
                // Esconde os campos da liberação.
                AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", false);
            }
            else if (valor != 1 && valor != 6 && valor != 7 && valor != 8) {
                // Esconde os campos do pedido.
                AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", false);
                // Exibe os campos da liberação.
                AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", true);
            }
        }

        loadParcelas(FindControl("txtNumPedido", "input") != null);
    }

    function loadParcelas(isPedido)
    {
        if (FindControl("drpReferente", "select").value == 4)
        {
            var tabela = document.getElementById("parcelas");
            for (i = tabela.rows.length - 1; i >= 0; i--)
                tabela.deleteRow(i);

            var idPedido = FindControl("txtNumPedido", "input");
            var idLiberacao = FindControl("txtNumLiberacao", "input");
            var id = idPedido != null ? idPedido.value : idLiberacao.value;
            if (id == "")
                return;
            
            var response = Recibo.GetParcelas(id, isPedido).value.split(';');

            if (response[0] == "Erro")
            {
                alert(response[1]);
                return false;
            }

            var lstParc = response[1].split('|');

            var colunas = 1;
            var numColuna = colunas;
            var linha;

            for (i = 0; i < lstParc.length; i++)
            {
                var id = lstParc[i].split(':')[0];
                var descricao = "Parcela " + lstParc[i].split(':')[1];

                if (numColuna == colunas)
                {
                    linha = tabela.insertRow(tabela.rows.length);
                    numColuna = 0;
                }

                var celula = linha.insertCell(linha.cells.length);
                celula.innerHTML = "<input id='chkParcelas_" + id + "' type='checkbox' checked='checked' value='" + id + "' /><label for='chkParcelas_" + id + "'>" + descricao + "</label>";
                numColuna++;
            }
        }
    } 
    
</script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <table cellpadding="2" cellspacing="0">
                                <tr>
                                    <td id="tituloOrcamento" runat="server" align="right">
                                        <asp:Label ID="Label3" runat="server" Text="Num. Orçamento" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td id="orcamento" runat="server">
                                        <asp:TextBox ID="txtNumOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px" onchange="loadParcelas(true)"
                                            onkeydown="if (isEnter(event)) loadParcelas(true)"></asp:TextBox>
                                    </td>
                                    <td id="tituloPedido" runat="server" align="right">
                                        <asp:Label ID="Label10" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td id="pedido" runat="server">
                                        <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px" onchange="loadParcelas(true)"
                                            onkeydown="if (isEnter(event)) loadParcelas(true)"></asp:TextBox>
                                    </td>
                                    <td id="tituloLiberacao" runat="server" align="right">
                                        <asp:Label ID="Label1" runat="server" Text="Num. Liberação" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td id="liberacao" runat="server">
                                        <asp:TextBox ID="txtNumLiberacao" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px" onchange="loadParcelas(false)"
                                            onkeydown="if (isEnter(event)) loadParcelas(false)"></asp:TextBox>
                                    </td>
                                    <td id="tituloPgAntecipado" runat="server" style="display: none" align="right">
                                        <asp:Label ID="lblNumPgAntecipado" runat="server" Text="Num. Pgto. Antecipado" ForeColor="#0066FF" />
                                    </td>
                                    <td id="pgAntecipado" runat="server" style="display: none">
                                        <asp:TextBox ID="txtNumPgAntecipado" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px" />
                                    </td>  
                                     <td id="tituloAcerto" runat="server" style="display: none" align="right">
                                        <asp:Label ID="lblNumAcerto" runat="server" Text="Num. do Acerto" ForeColor="#0066FF" />
                                    </td>
                                    <td id="acerto" runat="server" style="display: none">
                                        <asp:TextBox ID="txtNumAcerto" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px" />
                                    </td>     
                                     <td id="tituloContaPagar" runat="server" style="display: none" align="right">
                                        <asp:Label ID="Label9" runat="server" Text="Num. da Conta Paga" ForeColor="#0066FF" />
                                    </td>
                                    <td id="contaPagar" runat="server" style="display: none">
                                        <asp:TextBox ID="txtNumContaPagar" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px" />
                                    </td>                                    
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Referente a" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpReferente" runat="server" onchange="drpReferenteChanged()">
                                            <asp:ListItem Value="9">Total do Orçamento</asp:ListItem>
                                            <asp:ListItem Value="2">Total do Pedido</asp:ListItem>
                                            <asp:ListItem Value="1">Sinal do Pedido</asp:ListItem>
                                            <asp:ListItem Value="3">Restante</asp:ListItem>
                                            <asp:ListItem Value="4">Parcelas</asp:ListItem>
                                            <asp:ListItem Value="6">Pagamento Antecipado</asp:ListItem>
                                            <asp:ListItem Value="7">Acerto</asp:ListItem>
                                            <asp:ListItem Value="8">Conta Paga</asp:ListItem>
                                            <asp:ListItem Value="5">Outro</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <table id="tbReferente" style="display: none">
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="Label2" runat="server" Text="Motivo ref." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtOutro" runat="server" Rows="5" TextMode="MultiLine" 
                                            Width="500px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="Label7" runat="server" Text="Valor" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="left">
                                        <asp:TextBox ID="txtValor" runat="server" Width="500px" Rows="3" 
                                            TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="linhaParcelas" style="display: none">
                        <td align="center">
                            <table id="parcelas">
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr id="trProduto">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Buscar produto de Orçamento" 
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdOrcamento" runat="server" 
                                onkeypress="return soNumeros(event, true, true);" Width="60px"
                                onkeydown="if (isEnter(event)) cOnClick('btnBuscarOrcamento', null);"></asp:TextBox>
                            </td>
                        <td>
                            <asp:Button ID="btnBuscarOrcamento" runat="server" Text="Buscar" 
                                onclientclick="return getProd();" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Buscar produto de Pedido" 
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedidoBuscar" runat="server" 
                                onkeypress="return soNumeros(event, true, true);" Width="60px"
                                onkeydown="if (isEnter(event)) cOnClick('btnBuscarPedido');"></asp:TextBox>
                            </td>
                        <td>
                            <asp:Button ID="btnBuscarPedido" runat="server" Text="Buscar" 
                                onclientclick="return getProdPedido();" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Inserir Produto" 
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtProd" runat="server" Rows="2" TextMode="MultiLine" 
                                Width="500px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnInserir" runat="server" onclientclick="return addProd();" 
                                Text="Inserir" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <table id="lstProd" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                    <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
                </td>
        </tr>
    </table>
    <script type="text/javascript">

        // Exibe os campos do orçamento, pois, é o item que aparece pré-selecionado.
        AlteraVisibilidadeControle("tituloOrcamento", "orcamento", "txtNumOrcamento", true);

        // Esconde os demais campos.
        AlteraVisibilidadeControle("tituloPedido", "pedido", "txtNumPedido", false);
        AlteraVisibilidadeControle("tituloLiberacao", "liberacao", "txtNumLiberacao", false);
        AlteraVisibilidadeControle("tituloContaPagar", "contaPagar", "txtNumContaPagar", false);
        AlteraVisibilidadeControle("tituloPgAntecipado", "pgAntecipado", "txtNumPgAntecipado", false);
        AlteraVisibilidadeControle("tituloAcerto", "acerto", "txtNumAcerto", false);

    </script>
</asp:Content>

