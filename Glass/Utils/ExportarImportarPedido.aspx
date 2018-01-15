<%@ Page Title="Exportar/Importar Pedido" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ExportarImportarPedido.aspx.cs" Inherits="Glass.UI.Web.Utils.ExportarImportarPedido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript"  src="../Scripts/wz_tooltip.js"></script>
    
    <script type="text/javascript">
        var tipo = 1;
        function alteraTipo()
        {
            tipo = FindControl("rblTipo", "table").getElementsByTagName("input");
            for (i = 0; i < tipo.length; i++)
                if (tipo[i].type == "radio" && tipo[i].checked)
                {
                    tipo = tipo[i].value;
                    break;
                }
            
            document.getElementById("exportar").style.display = tipo == 1 ? "" : "none";
            document.getElementById("importar").style.display = tipo == 2 ? "" : "none";
        }

        function adicionar()
        {
            if (tipo == 2)
                return;

            var compNome = "Exp";
            var codigo = FindControl("txtCodigo" + compNome, "input");
            
            setPedido(codigo.value);
            
            codigo.value = "";
            codigo.focus();
        }
        
        function setPedido(idPedido)
        {
            var resposta = ExportarImportarPedido.GetDadosPedido(idPedido).value.split('#');

            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return;
            }

            setPedidoFinal(idPedido, resposta[1], resposta[2], resposta[3], resposta[4], resposta[5]);
        }

        function setPedidoFinal(idPedido, nomeCliente, codCliente, nomeFunc, dataEntrega, total)
        {
            if (tipo == 2)
                return;
            
            var compNome = "Exp";
            var campoCodigos = FindControl("hdfIdsPedidos" + compNome, "input");
            var nomeTabela = "tbPedidos" + compNome;
            
            var existentes = campoCodigos.value.split(',');
            for (i = 0; i < existentes.length; i++)
                if (existentes[i] == idPedido)
                    return;

            if (tipo == 1 && ExportarImportarPedido.PodeAdicionar(idPedido).value != "true")
                return;
                
            var resposta = ExportarImportarPedido.GetAmbientes(idPedido, countItem).value.split("¬");
            var ambientes = "<img src='../Images/gear.gif' id='botao_" + countItem + "' onclick='exibirAmbiente(" + countItem + ");' style='cursor: pointer' />" +
                "<div id='ambiente_" + countItem + "' style='display: none'>" + resposta[0] + "</div>";
            
            addItem(new Array(idPedido, nomeCliente, codCliente, nomeFunc, dataEntrega, total, ambientes), 
                new Array("Pedido", "Cliente", "Cod. Ped. Cli.", "Funcionário", "Data Entrega", "Total", "Produtos"), 
                nomeTabela, idPedido, campoCodigos.id, null, null, null, false);
            
            if (resposta[1].length > 0)
                eval(resposta[1]);
        }
        
        function limpar()
        {
            var compNome = "Exp";
            var nomeTabela = "tbPedidos" + compNome;
            var tabela = document.getElementById(nomeTabela);
            
            tabela.innerHTML = "";
            FindControl("txtCodigo" + compNome, "input").value = "";
            FindControl("hdfIdsPedidos" + compNome, "input").value = "";
            FindControl("hdfProdutos" + compNome, "input").value = "";
        }
        
        function exportar(idsPedidos, idsProdutos)
        {
            document.getElementById("loading").style.display = "";
            redirectUrl("../Handlers/ExportarPedido.ashx?idsPedido=" + idsPedidos + "&idsProdutos=" + idsProdutos);
            document.getElementById("loading").style.display = "none";
        }

        function validaExportar()
        {
            var compNome = "Exp";
        
            if (FindControl("hdfIdsPedidos" + compNome, "input").value.length == 0)
            {
                alert("Selecione pelo menos 1 pedido para exportar.");
                return false;
            }
            
            var produtos = new Array();
            var compNome = "Exp";
            var nomeTabela = "tbPedidos" + compNome;
            var tabela = document.getElementById(nomeTabela);
            
            for (var i = 1; i < tabela.rows.length; i++)
            {
                if (tabela.rows[i].style.display == "none")
                    continue;
                
                var inputs = tabela.rows[i].cells[7].getElementsByTagName("input");
                for (var j = 0; j < inputs.length; j++)
                {
                    if (inputs[j].type != "checkbox" || !inputs[j].checked || inputs[j].getAttribute("isAmbiente") == "true")
                        continue;
                    
                    produtos.push(inputs[j].value);
                }
            }
            
            if (produtos.length == 0)
            {
                alert("Não há nenhum produto selecionado para a exportação.");
                return false;
            }
            
            FindControl("hdfProdutos" + compNome, "input").value = produtos.join(",");
            
            exportar(FindControl("hdfIdsPedidos" + compNome, "input").value,
                FindControl("hdfProdutos" + compNome, "input").value);
            
            limpar();
            return false;
        }

        function validaImportar()
        {
            if (FindControl("fluArquivo", "input").value == "")
            {
                alert("Indique o arquivo que será importado.");
                return false;
            }

            document.getElementById("loading").style.display = "";
            return true;
        }
        
        function exibirAmbiente(numeroItem)
        {
            var titulo = "Produtos";
            var botao = document.getElementById("botao_" + numeroItem);
            
            for (iTip = 0; iTip < 2; iTip++)
            {
                TagToTip('ambiente_' + numeroItem, FADEIN, 300, COPYCONTENT, false, TITLE, titulo, CLOSEBTN, true, CLOSEBTNTEXT, 'Aplicar', 
                    CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true, FIX, [botao, 8-getTableWidth("ambiente_" + numeroItem), 1]);
            }
        }
        
        function exibirProdutosAmbiente(exibir, ambiente, numeroItem)
        {
            var tabelaProdutos = document.getElementById("produtosAmbiente_" + ambiente + "_" + numeroItem);
            
            if (tabelaProdutos != null)
            {
                tabelaProdutos.style.display = exibir ? "" : "none";
                
                var inputs = tabelaProdutos.getElementsByTagName("input");
                for (i = 0; i < inputs.length; i++)
                {
                    if (inputs[i].type != "checkbox")
                        continue;
                    
                    inputs[i].checked = exibir;
                }
                
                var chkApenasVidros = document.getElementById("apenasVidros_" + ambiente);
                if (chkApenasVidros != null)
                    apenasVidros(exibir && chkApenasVidros.checked, ambiente, numeroItem);
                
                exibirAmbiente(numeroItem);
            }
        }
        
        function apenasVidros(marcar, ambiente, numeroItem)
        {
            if (document.getElementById("apenasVidros_" + ambiente) == null)
                return;
        
            var tabelaProdutos = document.getElementById("produtosAmbiente_" + ambiente + "_" + numeroItem);
            
            if (tabelaProdutos != null)
            {
                var inputs = tabelaProdutos.getElementsByTagName("input");
                for (i = 0; i < inputs.length; i++)
                {
                    if (inputs[i].type != "checkbox")
                        continue;
                    
                    var isVidro = inputs[i].getAttribute("isVidro") == "true";
                    inputs[i].checked = isVidro || !marcar;
                }
            }
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <asp:RadioButtonList ID="rblTipo" runat="server" RepeatDirection="Horizontal" OnClick="alteraTipo()">
                    <asp:ListItem Selected="True" Value="1">Exportar</asp:ListItem>
                    <asp:ListItem Value="2">Importar</asp:ListItem>
                </asp:RadioButtonList>
                <br />
            </td>
        </tr>
        <tr id="exportar">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigoExp" runat="server" Width="50px"
                                onkeydown="if (isEnter(event)) { adicionar(); return false }"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddExp" runat="server" OnClientClick="adicionar(); return false"
                                ImageUrl="~/Images/Insert.gif" Width="16px" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" align="center">
                            <asp:Button ID="btnBuscarExp" runat="server" Text="Buscar pedidos"
                                OnClientClick="openWindow(600, 800, 'SelPedido.aspx?tipo=4&varios=true'); return false" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdsPedidosExp" runat="server" />
                <asp:HiddenField ID="hdfProdutosExp" runat="server" />
                <br />
                <br />
                <table id="tbPedidosExp"></table>
                <br />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar"
                    OnClientClick="if (!validaExportar()) return false" />
            </td>
        </tr>
        <tr id="importar" style="display: none">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Arquivo para importação
                        </td>
                        <td>
                            <asp:FileUpload ID="fluArquivo" runat="server" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblTamanhoMaximo" runat="server" Text="Tamanho máximo do arquivo: "></asp:Label>
                <br /><br />
                <asp:Button ID="btnImportar" runat="server" Text="Importar" 
                    onclick="btnImportar_Click" OnClientClick="if (!validaImportar()) return false" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <img id="loading" src="../Images/Load.gif" style="display: none" />
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        alteraTipo();

        switch (tipo)
        {
            case 1:
                FindControl("txtCodigoExp", "input").focus();
                break;

            case 2:
                FindControl("fluArquivo", "input").focus();
                break;
        }
    </script>
</asp:Content>

