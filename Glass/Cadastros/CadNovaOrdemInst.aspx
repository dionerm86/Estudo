<%@ Page Title="Nova Ordem de Instalação" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadNovaOrdemInst.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadNovaOrdemInst" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
    var adicionarAmbientes = <%= Glass.Configuracoes.PedidoConfig.Instalacao.UsarAmbienteInstalacao.ToString().ToLower() %>;
    
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
            
            exibirAmbiente(numeroItem);
        }
    }

    function getInstalacao() {
        var idPedido = FindControl("txtNumPedido", "input");

        if (idPedido.value == "") {
            alert("Informe o Número do Pedido.");
            idPedido.focus();
            return false;
        }

        var noCache = new Date();
        var response = CadNovaOrdemInst.GetInstByPedido(idPedido.value, noCache.getMilliseconds()).value;

        if (response == null) {
            alert("Falha ao buscar Instalação. AJAX Error.");
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            return false;
        }

        var instalacoes = response[1].split('|');

        for (j = 0; j < instalacoes.length; j++) {
            var items = instalacoes[j].split(';');
            setInstalacao(items[0], items[1], items[2], items[3], items[4], items[5], items[6], null);
        }

        return false;
    }

    function setInstalacao(idInstalacao, idPedido, cliente, tipo, loja, localObra, dataConfPedido, selInstWin) {

        // Verifica se a instalação já foi adicionada
        var instalacoes = FindControl("hdfIdInstalacao", "input").value.split(',');
        for (i = 0; i < instalacoes.length; i++) {
            if (idInstalacao == instalacoes[i]) {
                if (selInstWin != null)
                    selInstWin.alert("Instalação já adicionada.");
                    
                return false;
            }
        }
        
        var contadorItem = countItem['lstInst'];
        
        if (countItem['lstInst'] == null)
            contadorItem = 1;
        
        var resposta = adicionarAmbientes ? CadNovaOrdemInst.GetAmbientes(idPedido, contadorItem).value.split("¬") : "";
        var tabelaAmbientes = adicionarAmbientes ? resposta[0] : "";
        var ambientes = !adicionarAmbientes ? "" :
            "<img src='../Images/gear.gif' id='botao_" + contadorItem + "' onclick='exibirAmbiente(" + contadorItem + ");' style='cursor: pointer' />" +
            "<div id='ambiente_" + contadorItem + "' style='display: none'>" + tabelaAmbientes + "</div>";

        // Adiciona item à tabela
        addItem(new Array(idPedido, loja, cliente, localObra, tipo, dataConfPedido, ambientes),
            new Array('Pedido', 'Loja', 'Cliente', 'Local', 'Tipo Colocação', "Data Conf. Ped.", adicionarAmbientes ? "Produtos" : ""),
            'lstInst', idInstalacao, "hdfIdInstalacao", null, null);

        if (resposta != "" && resposta != null)
            if (resposta[1].length > 0)
                eval(resposta[1]);
        
        return false;
    }

    function confirmar() {

        if (!confirm('Ter certeza que deseja confirmar esta Ordem de Instalação?'))
            return false;

        var cConfirmar = FindControl("btnConfirmar", "input");
        cConfirmar.disabled = true;
        
        var idsInst = FindControl('hdfIdInstalacao', 'input').value;
        var dataInst = FindControl('ctrlDataInst_txtData', 'input').value;
        var tipoInstalacao = FindControl('drpTipoInstalacao', 'select').value;
        
        var idsEquipe = new Array();
        for (i = 1; i <= 5; i++)
        {
            if (document.getElementById("equipe" + i).style.display == "none")
                break;
            
            var idEquipe = FindControl('drpEquipe' + i, 'select').value;
            if (idEquipe == "")
            {
                alert("Selecione a " + i + "ª equipe da instalação" + (i > 1 ? " ou remova a linha." : "."));
                cConfirmar.disabled = false;
                return false;
            }
            else
            {
                for (j = 0; j < idsEquipe.length; j++)
                    if (idsEquipe[j] == idEquipe)
                    {
                        alert("A " + (j + 1) + "ª e a " + i + "ª equipes são iguais. Altere ou remova uma delas para continuar.");
                        cConfirmar.disabled = false;
                        return false;
                    }
                
                idsEquipe.push(idEquipe);
            }
        }
        
        idsEquipe = idsEquipe.join(",");

        if (idsInst == "" || idsInst == null) {
            alert("Busque pelo menos uma instalação antes de confirmar.");
            cConfirmar.disabled = false;
            return false;
        }

        if (dataInst == "") {
            alert("Informe a data de instalação.");
            cConfirmar.disabled = false;
            return false;
        }
        
        var produtos = "";
        if (adicionarAmbientes)
        {
            var tabela = document.getElementById("lstInst");
            for (i = 1; i < tabela.rows.length; i++)
            {
                if (tabela.rows[i].style.display == "none")
                    continue;
                
                var idPedido = tabela.rows[i].cells[1].innerHTML;
                var itensAmbientes = document.getElementById("ambiente_" + i).getElementsByTagName("table")[0].getElementsByTagName("input");
                var produtosAmbiente = new Array();
                
                for (j = 0; j < itensAmbientes.length; j++)
                    if (itensAmbientes[j].checked && itensAmbientes[j].getAttribute("isAmbiente") != "true")
                        produtosAmbiente.push(itensAmbientes[j].value);
                
                if (produtosAmbiente.length > 0)
                    produtos += tabela.rows[i].getAttribute("objId") + ";" + idPedido + ";" + produtosAmbiente.join(",") + "|";
                else
                {
                    alert("Selecione pelo menos 1 produto para o pedido " + idPedido + ".");
                    cConfirmar.disabled = false;
                    return false;
                }
            }
        }
        
        var noCache = new Date();
        var obs = FindControl("txtObs", "textarea").value;
        var response = CadNovaOrdemInst.Confirmar(idsInst, idsEquipe, tipoInstalacao, dataInst, produtos, obs, noCache.getMilliseconds()).value;
        
        if (response == null) {
            alert("Falha ao gerar ordem de instalação.");
            cConfirmar.disabled = false;
            return false;
        }
        
        response = response.split('\t');

        if (response[0] == "ok") {
            alert("Ordem de instalação gerada. Número: " + response[1] + ".");
            FindControl("hdfIdOrdemInst", "input").value = response[1];
            FindControl("lnkImprimir", "a").style.visibility = "visible";
            FindControl("btnNova", "input").style.visibility = "visible";
            openRpt();

            // Necessário limpar, pois caso o usuário pressione F5, os pedidos anteriores entrarão na próxima ordem de instalação.
            limpar();
        }
        else if (response[0] == "Erro") {
            alert(response[1]);
            cConfirmar.disabled = false;
        }
        else
            alert("Falha ao gerar Ordem de Instalação. Erro: Unknown.");

        return false;
    }

    function limpar() {
        countItem['lstInst'] = 1;
        FindControl("lnkImprimir", "a").style.visibility = "hidden";
        FindControl("btnNova", "input").style.visibility = "hidden";
        for (i = 1; i <= 5; i++)
            FindControl("drpEquipe" + i, "select").selectedIndex = 0;
        FindControl("hdfIdOrdemInst", "input").value = "";
        FindControl('hdfIdInstalacao', 'input').value = "";
        FindControl("ctrlDataInst_txtData", "input").value = "";
        FindControl("btnConfirmar", "input").disabled = false;
        document.getElementById('lstInst').innerHTML = "";
    }

    // Abre relatório desta ordem de instalação
    function openRpt() {
        var idOrdemInst = FindControl("hdfIdOrdemInst", "input").value;
        var dataInst = FindControl("ctrlDataInst_txtData", "input").value;
        
        var queryString = "?Rel=ListaOrdemInst&IdOrdemInst="+idOrdemInst+"&idEquipe=0";

        openWindow(600, 800, "../Relatorios/RelBase.aspx" + queryString);
        return false;
    }
    
    function addEquipe(numEquipe)
    {
        document.getElementById("equipe" + (numEquipe + 1)).style.display = "";
        FindControl("imbAddEquipe" + numEquipe, "input").style.display = "none";
        if (numEquipe > 1)
            FindControl("imbRemoveEquipe" + numEquipe, "input").style.display = "none";
        
        return false;
    }
    
    function removeEquipe(numEquipe)
    {
        document.getElementById("equipe" + numEquipe).style.display = "none";
        FindControl("drpEquipe" + numEquipe, "select").value = "";
        FindControl("imbAddEquipe" + (numEquipe - 1), "input").style.display = "";
        if (numEquipe > 2)
            FindControl("imbRemoveEquipe" + (numEquipe - 1), "input").style.display = "";
        
        return false;
    }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr id="equipe1">
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Equipes" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe1" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imbAddEquipe1" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(1);" />
                        </td>
                    </tr>
                    <tr id="equipe2" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe2" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddEquipe2" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(2);" />
                            <asp:ImageButton ID="imbRemoveEquipe2" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(2);" />
                        </td>
                    </tr>
                    <tr id="equipe3" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe3" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddEquipe3" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(3);" />
                            <asp:ImageButton ID="imbRemoveEquipe3" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(3);" />
                        </td>
                    </tr>
                    <tr id="equipe4" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe4" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddEquipe4" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(4);" />
                            <asp:ImageButton ID="imbRemoveEquipe4" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(4);" />
                        </td>
                    </tr>
                    <tr id="equipe5" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe5" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imbRemoveEquipe5" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(5);" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeydown="if (isEnter(event)) return getInstalacao();"
                                onkeypress="if (isEnter(event)) return false;"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgAddInstalacao" runat="server" ImageUrl="~/Images/Insert.gif"
                                OnClientClick="return getInstalacao(); return false;" ToolTip="Adicionar Instalação"
                                Width="16px" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;<a href="#" onclick="return openWindow(500, 700, '../Utils/SelInstalacao.aspx'); return false;"
                                style="font-size: small;">Buscar Instalações</a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="lstInst" align="left" cellpadding="4" cellspacing="0" width="100%">
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
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoInstalacao" runat="server" ForeColor="#0066FF" 
                                Text="Alterar tipo das instalações"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoInstalacao" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsTipoInstalacao" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Não alterar</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Data Instalação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataInst" runat="server" ReadOnly="ReadWrite" ValidateEmptyText="true" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Observação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Rows="4" TextMode="MultiLine" Width="350px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClientClick="return confirmar();" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href="#" id="lnkImprimir" onclick="return openRpt();"
                    style="visibility: hidden">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>
                <br />
                <br />
                <asp:Button ID="btnNova" runat="server" Text="Nova Ordem de Instalação" OnClientClick="limpar(); return false;"
                    Style="visibility: hidden" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdInstalacao" runat="server" />
                <asp:HiddenField ID="hdfIdOrdemInst" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" SelectMethod="GetByTipo" TypeName="Glass.Data.DAL.EquipeDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoInstalacao" runat="server" SelectMethod="GetTipoInstalacao"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
