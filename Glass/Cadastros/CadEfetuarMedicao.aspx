<%@ Page Title="Medições para efetuar" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadEfetuarMedicao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadEfetuarMedicao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

<script type="text/javascript">

    var countMedicoes = 1; // Conta a quantidade de medicoes adicionados ao form
    
    function setMedicao(idMedicao, nomeCliente, turno, situacao, dataMedicao, selMedicoesWin) {

        // Verifica se a medicao já foi adicionada
        var medicoes = FindControl("hdfIdMedicoes", "input").value.split(',');
        for (i = 0; i < medicoes.length; i++) {
            if (idMedicao == medicoes[i]) {
                if (selMedicoesWin != null)
                    selMedicoesWin.alert("Medição já adicionada.");
                else
                    alert("Medição já adicionada.");
                    
                return false;
            }
        }

        // Adicionar id Conta selecionada ao hiddenfield que guarda todos os ids ja selecionados
        FindControl("hdfIdMedicoes", "input").value += idMedicao + ",";

        // Monta tabela dinamicamente
        tabela = document.getElementById('lstMedicoes');

        // Cria títulos para a tabela
        if (countMedicoes == 1) {
            tabela.innerHTML = "<tr align=\"left\"><td></td>" +
            "<td style=\"font-weight: bold\">Num. Medição</td>" +
            "<td style=\"font-weight: bold\">Cliente</td>" +
            "<td style=\"font-weight: bold\">Turno</td>" +
            "<td style=\"font-weight: bold\">Situação</td>" +
            "<td style=\"font-weight: bold\">Data Medição</td></tr>";
        }

        row = tabela.insertRow(countMedicoes);
        row.id = "row" + row.rowIndex;
        row.setAttribute("idMedicao", idMedicao);
        row.innerHTML =
            "<td><a href=\"#\" onclick=\"return excluirItem(" + row.rowIndex + ");\">" +
            "<img src=\"../Images/ExcluirGrid.gif\" border=\"0\" title=\"Excluir\"/></a></td>" +
            "<td>" + idMedicao + "</td><td>" + nomeCliente + "</td><td>" + turno + "</td>" + 
            "<td>" + situacao + "</td><td>" + dataMedicao + "</td>";

        countMedicoes++;

        return false;
    }

    function excluirItem(linha) {
        // Exclui conta do vetor de contas
        var medicoes = FindControl("hdfIdMedicoes", "input").value.split(',');
        var medicaoAExcluir = document.getElementById("row" + linha).getAttribute("idMedicao");
        var newMedicoes = ""; // Novo vetor de contas

        // Cria um novo vetor de contas, tirando o id da conta que foi excluido
        for (i = 0; i < medicoes.length; i++) {
            if (medicaoAExcluir != medicoes[i])
                newMedicoes += medicoes[i] + ",";
        }

        // Atribui o novo vetor criado ao hidden field que guarda os ids das contas adicionadas
        FindControl("hdfIdMedicoes", "input").value = newMedicoes.replace(",,", ",");

        // Exclui o produto da tabela
        document.getElementById("row" + linha).style.display = "none";

        return false;
    }

    function confirma() {
        var result = getMedidor(FindControl('txtNumMedidor', 'input'));

        if (result == false)
            return false;

        var idsMedicao = FindControl('hdfIdMedicoes', 'input').value;
        var idMedidor = FindControl('txtNumMedidor', 'input').value;
        var dataEfetuar = FindControl('txtDataEfetuar', 'input').value;

        if (idsMedicao == "") {
            alert('Nenhuma medição selecionada.');
            return false;
        }
        else if (idMedidor == "") {
            alert('Informe o medidor responsável pelas medições.');
            return false;
        }
        else if (dataEfetuar == "") {
            alert('Informe a data que as medições serão efetuadas.');
            return false;
        }

        var retorno = CadEfetuarMedicao.Confirmar(idsMedicao, idMedidor, dataEfetuar).value.split("\t");

        if (retorno[0] == "ok") {
            alert(retorno[1]);

            openRpt();
            
            countMedicoes = 1;
            document.getElementById('lstMedicoes').innerHTML = "";
            FindControl("hdfIdMedicoes", "input").value = "";
            FindControl("txtNumMedidor", "input").value = "";
            FindControl("txtNomeMedidor", "input").value = "";
        }
        else if (retorno[0] == "Erro") {
            alert(retorno[1]);
            return false;
        }
    }

    function getCli(idCli) {
        if (idCli.value == "")
            return;

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNomeCli", "input").value = "";
            return false;
        }

        FindControl("txtNomeCli", "input").value = retorno[1];
    }

    // Abre popup para selecionar a medição
    function openMedicao() {
        openWindow(500, 750, "../Utils/SelMedicao.aspx");
        return false;
    }

    // Busca medição pelo código informado na textBox
    function getMedicao() {
        var idMedicao = FindControl("txtNumMedicao", "input");

        if (idMedicao.value == "")
            return false;

        var retorno = CadEfetuarMedicao.GetMedicao(idMedicao.value).value.split('\t');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idMedicao.value = "";
            return false;
        }

        setMedicao(retorno[1], retorno[2], retorno[3], retorno[4], retorno[5], null);

        FindControl("txtNumMedicao", "input").value = "";
        FindControl("txtNumMedicao", "input").focus();

        return false;
    }

    // Busca medidor pelo código do mesmo informado na textbox, ou se a mesma estiver em branco,
    // abre tela para selecioná-lo
    function getMedidor(idMedidor) {
        if (idMedidor.value == "") {
            openWindow(500, 700, "../Utils/SelMedidor.aspx");
            return false;
        }

        var retorno = MetodosAjax.GetMedidor(idMedidor.value).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idMedidor.value = "";
            FindControl("txtNomeMedidor", "input").value = "";
            return false;
        }

        FindControl("txtNomeMedidor", "input").value = retorno[1];
    }

    // Função utilizada após selecionar medidor no popup, para preencher o id e o nome do mesmo
    // Nas respectivas textboxes deste form
    function setMedidor(id, nome) {
        FindControl("txtNumMedidor", "input").value = id;
        FindControl("txtNomeMedidor", "input").value = nome;
        return false;
    }

    // Abre relatório de medição
    function openRpt() {
        var idMedidor = FindControl("txtNumMedidor", "input").value;
        var nomeMedidor = FindControl("txtNomeMedidor", "input").value;
        var dataEfetuar = FindControl("txtDataEfetuar", "input").value;

        var queryString = "?Rel=ListaMedicao&IdMedicao=0&IdMedidor=" + idMedidor + "&NomeMedidor=" + nomeMedidor + "&NomeCliente=&situacao=2&dataIni=&dataFim=&dataEfetuar="+dataEfetuar;

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
                                Text="Medição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumMedicao" runat="server" Width="70px" onkeydown="if (isEnter(event)) getMedicao();"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                                        <asp:ImageButton ID="imgPesqMedicao" runat="server" ImageUrl="~/Images/Insert.gif" 
                                            OnClientClick="getMedicao(); return false;" 
                                            ToolTip="Adicionar Medição" Width="16px" />
                                    </td>
                        <td>
                <asp:Button ID="btnBuscarMedicao" runat="server" Text="Buscar Medições" OnClientClick="return openMedicao();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="lstMedicoes" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdMedicoes" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                                        <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" 
                                Text="Medidor"></asp:Label>
                        </td>
                        <td>
&nbsp;<asp:TextBox ID="txtNumMedidor" runat="server" Width="61px" onblur="getMedidor(this);"></asp:TextBox>
                        </td>
                        <td>
&nbsp;<asp:TextBox ID="txtNomeMedidor" runat="server" Width="217px"></asp:TextBox>
                        </td>
                        <td>
                                        <asp:ImageButton ID="imgPesqMedidor" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                            OnClientClick="getMedidor(FindControl('txtNumMedidor', 'input')); return false;" 
                                            ToolTip="Pesquisar" />
                                    </td>
                        <td>
                                        <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" 
                                Text="Data"></asp:Label>
                                    </td>
                        <td>
                                        <asp:TextBox ID="txtDataEfetuar" runat="server" onkeypress="return false;" 
                                            Width="80px"></asp:TextBox>
                                        <asp:ImageButton ID="imgDataEfetuar" runat="server" ImageAlign="AbsMiddle" 
                                            ImageUrl="~/Images/calendario.gif" 
                                            OnClientClick="return SelecionaData('txtDataEfetuar', this)" 
                                            ToolTip="Alterar" />
                                    </td>
                    </tr>
                </table>
                <br />
                            <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" 
                                OnClientClick="return confirma();" />
            </td>
        </tr>
    </table>
</asp:Content>

