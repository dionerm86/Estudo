<%@ Page Title="Retificar Custo Fixo" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCustoFixoRetificar.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCustoFixoRetificar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function retificar() {
        if (!confirm("Tem certeza que deseja Retificar este Custo Fixo?"))
            return false;

        var cIdCustoFixo = FindControl("hdfIdCustoFixo", "input");
        var cMesAno = FindControl("txtData", "input");
        var cNovoValor = FindControl("txtValor", "input");
        var cNovoDiaVenc = FindControl("txtDiaVenc", "input");

        // Verifica se o Custo Fixo foi selecionado
        if (cIdCustoFixo.value == "" || cIdCustoFixo.value == null) {
            alert("Selecione um Custo Fixo.");
            cIdCustoFixo.focus();
            return false;
        }

        // Verifica se o mês/ano foi informado
        if (cMesAno.value == "") {
            alert("Informe o Mês/Ano que este Custo Fixo será retificado.")
            cMesAno.focus();
            return false;
        }

        // Verifica se mês/ano informado é válido
        function validate() {
            // Verifica se mês/ano informado é válido
            if (!validaMesAno(FindControl("txtData", "input")))
                return false;

            return true;
        }

        // Verifica se o Novo Valor foi definido
        if (cNovoValor.value == "" || cNovoValor.value == null){
            alert("Informe o novo Valor para este Custo Fixo no mês especificado.");
            cNovoValor.focus();
            return false;
        }
        
        // Verifica se o Novo Dia Venc. foi definido
        if (cNovoDiaVenc.value == "" || cNovoDiaVenc.value == null){
            alert("Informe o novo Dia de Vencimento para este Custo Fixo no mês especificado.");
            cNovoDiaVenc.focus();
            return false;
        }
        
        // Retifica Custo Fixo via AJAX
        var result = CadCustoFixoRetificar.Retificar(cIdCustoFixo.value, cMesAno.value, cNovoValor.value, cNovoDiaVenc.value);

        // Se o retorno do AJAX não tiver valor, mostra mensagem de erro
        if (result == "" || result == null || result.value == null) {
            alert("Falha ao retificar Custo Fixo.");            
            return false;
        }

        // Se tiver ocorrido algum erro, exibe a mensagem de erro
        if (result.value != "ok") {
            alert(result.value.split('\t')[1]);
            return false;
        }

        // Limpa valores
        cMesAno.value = "";
        setCustoFixoFields("", "", "", "", "", "", "");

        // Esconde tabelas da retificação
        setTablesVisibility("hidden");

        alert("Custo Fixo Retificado com sucesso.");

        return false;
    }

    // Função chamada pela página de seleção de custo fixo
    function setCustoFixo(idCustoFixo, descricao, fornecedor, loja, referenteA, valor, diaVenc, selWindow) {
        // Mostra as tabelas da retificação
        setTablesVisibility("visible");

        // Fecha a janela de seleção do Custo Fixo
        selWindow.close();

        // Atribui valores aos campos do Custo Fixo
        setCustoFixoFields(idCustoFixo, descricao, fornecedor, loja, referenteA, valor, diaVenc);
    }

    // Define a visibilidade das tabelas de retificação do custo fixo
    function setTablesVisibility(visib) {
        find("tbDescrCustoFixo").style.visibility = visib;
        find("tbNovoDiaValor").style.visibility = visib;
        find("tbMesAno").style.visibility = visib;
    }

    // Atribui valores aos campos do custo fixo
    function setCustoFixoFields(idCustoFixo, descricao, fornecedor, loja, referenteA, valor, diaVenc) {
        FindControl("hdfIdCustoFixo", "input").value = idCustoFixo;
        FindControl("lblDescricao", "span").innerHTML = descricao;
        FindControl("lblFornecedor", "span").innerHTML = fornecedor;
        FindControl("lblLoja", "span").innerHTML = loja;
        FindControl("lblReferenteA", "span").innerHTML = referenteA;
        FindControl("lblValor", "span").innerHTML = valor;
        FindControl("lblDiaVenc", "span").innerHTML = diaVenc;
        FindControl("txtValor", "input").value = valor.replace('.', ',');
        FindControl("txtDiaVenc", "input").value = diaVenc;
    }
    
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <a href="#" onclick="return openWindow(500, 700, '../Utils/SelCustoFixo.aspx'); return false;"
                                style="font-size: small;">Buscar Custo Fixo</a>
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
                <table id="tbDescrCustoFixo" style="visibility: hidden">
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label1" runat="server" Text="Descrição" Style="font-weight: bold;"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblDescricao" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" Text="Fornecedor" Style="font-weight: bold;"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblFornecedor" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label3" runat="server" Text="Loja" Style="font-weight: bold;"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblLoja" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="Label4" runat="server" Text="Referente a" Style="font-weight: bold;"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblReferenteA" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label6" runat="server" Text="Valor" Style="font-weight: bold;"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblValor" runat="server" Text=""></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="Label7" runat="server" Text="Dia Venc." Style="font-weight: bold;"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblDiaVenc" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdCustoFixo" runat="server" />
                <br />
                <table id="tbMesAno" style="visibility: hidden">
                    <tr>
                        <td>
                            <asp:Label ID="lblPeriodo" runat="server" Text="Mês/Ano (mm/yyyy) que o Custo Fixo será Retificado:"
                                ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtData" runat="server" Width="60px" MaxLength="7" 
                            onkeypress="mascara_mesAno(event, this); return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table id="tbNovoDiaValor" style="visibility: hidden">
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Novo Dia Venc.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDiaVenc" runat="server" MaxLength="2" onkeypress="return soNumeros(event, true, true);"
                                Width="50px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Novo Valor:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValor" runat="server" MaxLength="12" onkeypress="return soNumeros(event, false, true);"
                                Width="70px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnRetificar" runat="server" OnClientClick="return retificar();"
                                Text="Retificar" />
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
    </table>
</asp:Content>
