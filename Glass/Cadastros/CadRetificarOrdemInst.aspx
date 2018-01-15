<%@ Page Title="Retificar Ordem de Instalação" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadRetificarOrdemInst.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetificarOrdemInst" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

    // Busca instalações a partir de uma Ordem de Instalação
    function getOrdemInst() {
        var idOrdemInst = FindControl("txtIdOrdemInst", "input");

        if (idOrdemInst.value == "") {
            alert("Informe o Número da Ordem de Instalação a ser Retificada.");
            idOrdemInst.focus();
            return false;
        }

        buscarFormBehavior(true);

        var noCache = new Date();
        var response = CadRetificarOrdemInst.GetOrdemInst(idOrdemInst.value, noCache.getMilliseconds()).value;

        if (response == null) {
            alert("Falha ao buscar Ordem de Instalação. AJAX Error.");
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            return false;
        }

        var dadosOrdemInst = response[1].split(';');
        
        for (k = 5; k > 1; k--)
        {
            if (document.getElementById("equipe" + k).style.display != "none")
                removeEquipe(k);
        }
        
        var idsEquipes = dadosOrdemInst[0].split(',');
        for (k = 1; k <= idsEquipes.length; k++)
        {
            if (k > 1)
                addEquipe(k - 1);            
            
            var equipe = FindControl("drpEquipe" + k, "select");
            if (equipe == null)
                continue;
            
            equipe.value = idsEquipes[k - 1];
        }

        FindControl("hdfIdOrdemInst", "input").value = idOrdemInst.value;
        FindControl("ctrlDataInst_txtData", "input").value = dadosOrdemInst[1];

        var instalacoes = response[2].split('|');

        try {
            for (k = 0; k < instalacoes.length; k++) {
                var items = instalacoes[k].split(';');
                setInstalacao(items[0], items[1], items[2], items[3], items[4], items[5], items[6], null);
            }
        }
        catch (err) {
            alert("Falha ao preencher instalações. Erro: " + err);
            buscarFormBehavior(true);
            return false;
        }

        buscarFormBehavior(false);

        return false;
    }

    // Busca instalação pelo idPedido informado diretamente
    function getInstalacao() {
        var idPedido = FindControl("txtNumPedido", "input");

        if (idPedido.value == "") {
            alert("Informe o Número do Pedido.");
            idPedido.focus();
            return false;
        }

        var noCache = new Date();
        var response = CadRetificarOrdemInst.GetInstByPedido(idPedido.value, noCache.getMilliseconds()).value;

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

        // Adiciona item à tabela
        addItem(new Array(idPedido, loja, cliente, localObra, tipo, dataConfPedido),
            new Array('Pedido', 'Loja', 'Cliente', 'Local', 'Tipo Colocação', "Data Conf. Ped."), 
            'lstInst', idInstalacao, "hdfIdInstalacao");

        return false;
    }

    function retificar() {

        if (!confirm('Ter certeza que deseja Retificar esta Ordem de Instalação?'))
            return false;

        var cConfirmar = FindControl("btnRetificar", "input");
        cConfirmar.disabled = true;
        
        var idsInst = FindControl('hdfIdInstalacao', 'input').value;
        var dataInst = FindControl('ctrlDataInst_txtData', 'input').value;
        var idOrdemInst = FindControl('hdfIdOrdemInst', 'input').value;
        
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

        if (idOrdemInst == "") {
            alert("Busque as Instalações que serão Retificadas.");
            cConfirmar.disabled = false;
            return false;
        }

        if (idsInst == "" || idsInst == null) {
            alert("Busque pelo menos uma instalação antes de Retificar esta Ordem de Instalação.");
            cConfirmar.disabled = false;
            return false;
        }

        if (dataInst == "") {
            alert("Informe a data de instalação.");
            cConfirmar.disabled = false;
            return false;
        }
        
        var noCache = new Date();
        var response = CadRetificarOrdemInst.Retificar(idOrdemInst, idsInst, idsEquipe, dataInst, noCache.getMilliseconds()).value;
        
        if (response == null) {
            alert("Falha ao Retificar Ordem de Instalação.");
            cConfirmar.disabled = false;
            return false;
        }
        
        response = response.split('\t');

        if (response[0] == "ok") {
            alert("Ordem de Instalação Retificada.");
            FindControl("lnkImprimir", "a").style.visibility = "visible";
            openRpt();
        }
        else if (response[0] == "Erro") {
            alert(response[1]);
            cConfirmar.disabled = false;
        }
        else
            alert("Falha ao Retificar Ordem de Instalação. Erro: Unknow.");

        return false;
    }

    // Limpa/Esconde campos utilizados na busca de instalações
    function buscarFormBehavior(hide) {
        if (hide) {
            //countItem = 1;
            FindControl("tbBuscar", "table").style.visibility = "hidden";
            FindControl("btnRetificar", "input").disabled = true;
            FindControl("lnkImprimir", "a").style.visibility = "hidden";
            FindControl("drpEquipe", "select").selectedIndex = 0;
            FindControl("hdfIdOrdemInst", "input").value = "";
            FindControl('hdfIdInstalacao', 'input').value = "";
            FindControl("ctrlDataInst_txtData", "input").value = "";
            document.getElementById('lstInst').innerHTML = "";
        }
        else {
            FindControl("tbBuscar", "table").style.visibility = "visible";
            FindControl("btnRetificar", "input").disabled = false;
        }
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
        var equipe = document.getElementById("equipe" + (numEquipe + 1));

        if (equipe == null)
            return;

        equipe.style.display = "";
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

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Num. Ordem de Instalação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdOrdemInst" runat="server" Width="60px" onkeydown="if (isEnter(event)) getOrdemInst();"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnBuscarOrdemInst" runat="server" Text="Buscar Ordem de Instalação"
                                OnClientClick="getOrdemInst(); return false;" />
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
                <table id="tbBuscar" style="visibility: hidden;">
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeydown="if (isEnter(event)) getInstalacao();"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgAddInstalacao" runat="server" ImageUrl="~/Images/Insert.gif"
                                OnClientClick="getInstalacao(); return false;" ToolTip="Adicionar Instalação"
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
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Data Instalação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataInst" runat="server" ReadOnly="ReadWrite" />
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
                        <td>
                            <asp:ImageButton ID="imbAddEquipe5" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(5);" />
                            <asp:ImageButton ID="imbRemoveEquipe5" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(5);" />
                        </td>
                    </tr>
                     <tr id="equipe6" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe6" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddEquipe6" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(6);" />
                            <asp:ImageButton ID="imbRemoveEquipe6" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(6);" />
                        </td>
                    </tr>
                     <tr id="equipe7" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe7" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddEquipe7" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(7);" />
                            <asp:ImageButton ID="imbRemoveEquipe7" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(7);" />
                        </td>
                    </tr>
                     <tr id="equipe8" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe8" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddEquipe8" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(8);" />
                            <asp:ImageButton ID="imbRemoveEquipe8" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(8);" />
                        </td>
                    </tr>
                     <tr id="equipe9" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe9" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddEquipe9" runat="server" ImageUrl="../Images/Insert.gif"
                                OnClientClick="return addEquipe(9);" />
                            <asp:ImageButton ID="imbRemoveEquipe9" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(9);" />
                        </td>
                    </tr>
                     <tr id="equipe10" style="display: none">
                        <td>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe10" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imbRemoveEquipe10" runat="server" ImageUrl="../Images/ExcluirGrid.gif"
                                OnClientClick="return removeEquipe(10);" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnRetificar" runat="server" Text="Retificar" OnClientClick="return retificar();" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href="#" id="lnkImprimir" onclick="return openRpt();"
                    style="visibility: hidden">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdInstalacao" runat="server" />
                <asp:HiddenField ID="hdfIdOrdemInst" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" SelectMethod="GetByTipo" TypeName="Glass.Data.DAL.EquipeDAO"
                    MaximumRowsParameterName="" StartRowIndexParameterName="">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        buscarFormBehavior(true);
    
    </script>

</asp:Content>
