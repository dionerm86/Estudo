<%@ Page Title="Cadastro de Beneficiamento" Language="C#" MasterPageFile="~/Painel.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeBehind="CadBenef.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadBenef" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    var idBenefConfig = '<%= Request["IdBenefConfig"] %>';
    var idProcessoItem = new Array();
    var codProcessoItem = new Array();
    var idAplicacaoItem = new Array();
    var codAplicacaoItem = new Array();
    var idProdItem = new Array();
    var codProdItem = new Array();
    var descrProdItem = new Array();
    var acrescimoAlturaItem = new Array();
    var acrescimoLarguraItem = new Array();
    
    var isItem = false;
    
    function carregaItem(lista)
    {
        for (j = 0; j < lista.options.length; j++)
            if (lista.options[j].selected)
            {
                FindControl("txtAplItem", "input").value = codAplicacaoItem[j];
                FindControl("txtProcItem", "input").value = codProcessoItem[j];
                FindControl("hdfIdProd", "input").value = idProdItem[j];
                FindControl("txtCodProd", "input").value = codProdItem[j];
                FindControl("lblDescrProd", "span").innerHTML = descrProdItem[j];
                FindControl("txtAcrescimoAltura", "input").value = acrescimoAlturaItem[j];
                FindControl("txtAcrescimoLargura", "input").value = acrescimoLarguraItem[j];
                break;
            }
    }
    
    function iniciarItens(idProcesso, codProcesso, idAplicacao, codAplicacao,
        idProd, codProd, descrProd, acrescimoAltura, acrescimoLargura)
    {
        idProcessoItem.push(idProcesso);
        codProcessoItem.push(codProcesso);
        idAplicacaoItem.push(idAplicacao);
        codAplicacaoItem.push(codAplicacao);
        
        // Produto associado ao item.
        idProdItem.push(idProd);
        codProdItem.push(codProd);
        descrProdItem.push(descrProd);
        acrescimoAlturaItem.push(acrescimoAltura == "" ? 0 : acrescimoAltura);
        acrescimoLarguraItem.push(acrescimoLargura == "" ? 0 : acrescimoLargura);
        
        // Caso algum item do beneficiamento tenha informações de produto associado então
        // as informações do produto associado são exibidas.
        if ((idProd != "" || codProd != "" || descrProd != "" || acrescimoAltura > 0 ||
            acrescimoLargura > 0) && !FindControl("chkUsarProdutoCompra", "input").checked) {
            FindControl("chkUsarProdutoCompra", "input").checked = true;
            document.getElementById("trProdutoCompra").style.display = "";
            document.getElementById("trProdutoCompra1").style.display = "";
        }
    }

    function addOpt() {
        var newOpt = FindControl("txtOpcao", "input").value;

        if (newOpt == "") {
            alert("Informe a descrição da opção.");
            return false;
        }

        // Verifica se o item já foi adicionado
        var lstOpt = FindControl("lstOpcoes", "select");
        for (j = 0; j < lstOpt.options.length; j++)
            if (lstOpt.options[j].text == newOpt) {
                alert("Item já adicionado na lista.");
                return false;
            }
    
        // Adiciona item na lista
        var opt = document.createElement("OPTION");
        opt.text = newOpt;
        opt.value = newOpt;
        FindControl("lstOpcoes", "select").options.add(opt);

        // Apaga item adicionado da textbox
        FindControl("txtOpcao", "input").value = "";
        
        // Adiciona a opção nos vetores
        idAplicacaoItem.push("");
        codAplicacaoItem.push("");
        idProcessoItem.push("");
        codProcessoItem.push("");
        idProdItem.push("");
        codProdItem.push("");
        descrProdItem.push("");
        acrescimoAlturaItem.push("");
        acrescimoLarguraItem.push("");

        return false;
    }
    
    function removePosicao(vetor, posicao)
    {
        var temp = new Array();
        for (i = 0; i < vetor.length; i++)
            if (i != posicao)
                temp.push(vetor[i]);
        
        vetor = temp;
    }

    function remOpt()
    {
        var idBenefConfig = <%= Request["idBenefConfig"] != null ? Request["idBenefConfig"] : "0" %>;
        var lstOpt = FindControl("lstOpcoes", "select");
        
        for (j = 0; j < lstOpt.options.length; j++) {
            if (lstOpt.options[j].selected == true) {
                if (CadBenef.PodeRemover(idBenefConfig, lstOpt.options[j].value).value == "true") {
                    FindControl("lstOpcoes", "select").remove(j);
                    j--;
                    
                    removePosicao(idAplicacaoItem, j);
                    removePosicao(codAplicacaoItem, j);
                    removePosicao(idProcessoItem, j);
                    removePosicao(codProcessoItem, j);
                    removePosicao(idProdItem, j);
                    removePosicao(codProdItem, j);
                    removePosicao(descrProdItem, j);
                    removePosicao(acrescimoAlturaItem, j);
                    removePosicao(acrescimoLarguraItem, j);
                }
                else {
                    alert("Este item está sendo utlizado, não é possível excluí-lo.");
                }
            }
        }
    }   

    function drpControleChanged(valor) {
        var chkUsarProdCompra = FindControl("chkUsarProdutoCompra", "input");
        // Esconde a opção de lista de seleção e o controle chkUsarProdutoCompra se o tipo do controle for seleção simples ou quantidade.
        if (valor == "SelecaoSimples" || valor == "Quantidade" || valor == "") {
            document.getElementById("tbListaSelecao").style.display = "none";
            chkUsarProdCompra.disabled = true;
            chkUsarProdCompra.checked = false;
            chkUsarProdCompra.onclick();
        }
        else {
            document.getElementById("tbListaSelecao").style.display = "inline";
            FindControl("chkUsarProdutoCompra", "input").disabled = false;
        }
    }

    function onInsert() {
        if (!validaCadastro())
            return false;
        
        var opcoes = "";
        var itens = "";
        
        var lstOpt = FindControl("lstOpcoes", "select");
        for (j = 0; j < lstOpt.options.length; j++)
        {
            opcoes += lstOpt.options[j].text + "|";
            itens += idProcessoItem[j] + ";" + idAplicacaoItem[j] + ";" + idProdItem[j] + ";" + codProdItem[j] + ";" +
                descrProdItem[j] + ";" + acrescimoAlturaItem[j] + ";" + acrescimoLarguraItem[j] + "|";
        }
            
        FindControl("hdfOpcoes", "input").value = opcoes;
        FindControl("hdfItens", "input").value = itens;
        return true;
    }

    function onUpdate() {
        if (!validaCadastro())
            return false;
    
        var opcoes = "";
        var itens = "";

        var lstOpt = FindControl("lstOpcoes", "select");
        for (j = 0; j < lstOpt.options.length; j++)
        {
            opcoes += lstOpt.options[j].text + "|";
            itens += idProcessoItem[j] + ";" + idAplicacaoItem[j] + ";" + idProdItem[j] + ";" + codProdItem[j] + ";" +
                descrProdItem[j] + ";" + acrescimoAlturaItem[j] + ";" + acrescimoLarguraItem[j] + "|";
        }

        FindControl("hdfOpcoes", "input").value = opcoes;
        FindControl("hdfItens", "input").value = itens;
        
        return true;
    }

    function validaCadastro() {
        if (FindControl("txtNome", "input").value == "") {
            alert("Informe o nome do beneficiamento.");
            return false;
        }

        if (FindControl("txtDescricao", "input").value == "") {
            alert("Informe a descrição do beneficiamento.");
            return false;
        }

        var controle = FindControl("drpControle", "select");
        if (!controle.disabled && controle.value == "") {
            alert("Informe o tipo de controle.");
            return false;
        }

        var calculo = FindControl("drpCalculo", "select");
        if (!calculo.disabled && calculo.value == "0") {
            alert("Informe como este beneficiamento será calculado.");
            return false;
        }

        if (document.getElementById("tbListaSelecao").style.display == "inline" &&
            FindControl("lstOpcoes", "select").options.length == 0) {
            alert("Inclua pelo menos um item na lista de opções.");
            return false;
        }

        return true;
    }
    
    // Função chamada pelo popup de escolha da Aplicação do produto
    function setApl(idAplicacao, codInterno) {
        if (!isItem)
        {
            FindControl("txtAplIns", "input").value = codInterno;
            FindControl("hdfIdAplicacao", "input").value = idAplicacao;
        }
        else
        {
            FindControl("txtAplItem", "input").value = codInterno;
            
            var lstOpcoes = FindControl("lstOpcoes", "select");
            for (i = 0; i < lstOpcoes.options.length; i++)
                if (lstOpcoes.options[i].selected)
                {
                    idAplicacaoItem[i] = idAplicacao;
                    codAplicacaoItem[i] = codInterno;
                    break;
                }
        }
        
        isItem = false;
    }

    function loadApl(codInterno) {
        if (codInterno == "") {
            setApl("", "");
            return false;
        }
    
        try {
            var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Aplicação. Ajax Error.");
                setApl("", "");
                return false
            }

            response = response.split("\t");
            
            if (response[0] == "Erro") {
                alert(response[1]);
                setApl("", "");
                return false;
            }

            setApl(response[1], response[2]);
        }
        catch (err) {
            alert(err);
        }
    }

    // Função chamada pelo popup de escolha do Processo do produto
    function setProc(idProcesso, codInterno, codAplicacao) {
        if (!isItem)
        {
            FindControl("txtProcIns", "input").value = codInterno;
            FindControl("hdfIdProcesso", "input").value = idProcesso;
        }
        else
        {
            FindControl("txtProcItem", "input").value = codInterno;
            
            var lstOpcoes = FindControl("lstOpcoes", "select");
            for (i = 0; i < lstOpcoes.options.length; i++)
                if (lstOpcoes.options[i].selected)
                {
                    idProcessoItem[i] = idProcesso;
                    codProcessoItem[i] = codInterno;
                    break;
                }
        }
        
        isItem = false;
    }

    function loadProc(codInterno) {
        if (codInterno == "") {
            setProc("", "", "");
            return false;
        }

        try {
            var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Processo. Ajax Error.");
                setProc("", "");
                return false
            }

            response = response.split("\t");
            
            if (response[0] == "Erro") {
                alert(response[1]);
                setProc("", "", "");
                return false;
            }

            setProc(response[1], response[2], response[3]);
        }
        catch (err) {
            alert(err);
        }
    }
    
    // Função chamada para setar o produto.
    function setProd(idProd, descrProd) {        
        var lstOpcoes = FindControl("lstOpcoes", "select");
        for (i = 0; i < lstOpcoes.options.length; i++)
            if (lstOpcoes.options[i].selected)
            {
                idProdItem[i] = idProd;
                codProdItem[i] = FindControl("txtCodProd", "input").value;
                descrProdItem[i] = descrProd;
                
                break;
            }
    }
    
    function loadProduto(codInterno) {
        if (codInterno == "") {
            limpaCamposProdutoCompra(true, false);
            return false;
        }

        try {            
            var retorno = CadBenef.GetProduto(codInterno).value.split("##");

            if (retorno[0] == "Erro") {
                limpaCamposProdutoCompra(true, false);
                
                alert('Produto não encontrado.');
                
                return false;
            }
            
            FindControl("hdfIdProd", "input").value = retorno[1];
            FindControl("lblDescrProd", "span").innerHTML = retorno[2];
            
            setProd(retorno[1], retorno[2]);
        }
        catch (err) {
            alert(err);
        }
    }
    
    function limpaCamposProdutoCompra(somenteOpcaoSelecionada, limparAcrescimoMedida) {
        FindControl("hdfIdProd", "input").value = "";
        FindControl("txtCodProd", "input").value = "";
        FindControl("lblDescrProd", "span").innerHTML = "";
        
        if (limparAcrescimoMedida) {
            FindControl("txtAcrescimoAltura", "input").value = "";
            FindControl("txtAcrescimoLargura", "input").value = "";
        }            
        
        var lstOpcoes = FindControl("lstOpcoes", "select");
        for (i = 0; i < lstOpcoes.options.length; i++) {
            if (somenteOpcaoSelecionada ? lstOpcoes.options[i].selected : true) {
                idProdItem[i] = "";
                codProdItem[i] = "";
                descrProdItem[i] = "";
                
                if (limparAcrescimoMedida) {
                    acrescimoAlturaItem[i] = "";
                    acrescimoLarguraItem[i] = "";
                }
                
                if (somenteOpcaoSelecionada)
                    break;
            }
        }
    }
    
    function mostrarProdutoCompra(chkMostrarProdutoCompra)
    {
        var trProdutoCompra = document.getElementById("trProdutoCompra");
        var trProdutoCompra1 = document.getElementById("trProdutoCompra1");
            
        if (chkMostrarProdutoCompra.checked) {
            trProdutoCompra.style.display = "";
            trProdutoCompra1.style.display = "";
        }
        else {
            trProdutoCompra.style.display = "none";
            trProdutoCompra1.style.display = "none";
            
            limpaCamposProdutoCompra(false, true);
        }
    }
    
    // Função chamada para setar o acréscimo de altura do produto.
    function setAcrescimoAlt(acrescimoAltura) {
        FindControl("txtAcrescimoAltura", "input").value = acrescimoAltura;
        
        var lstOpcoes = FindControl("lstOpcoes", "select");
        for (i = 0; i < lstOpcoes.options.length; i++)
            if (lstOpcoes.options[i].selected)
            {
                acrescimoAlturaItem[i] = acrescimoAltura;
                break;
            }
    }
    
    // Função chamada para setar o acréscimo de largura do produto.
    function setAcrescimoLarg(acrescimoLargura) {
        FindControl("txtAcrescimoLargura", "input").value = acrescimoLargura;
        
        var lstOpcoes = FindControl("lstOpcoes", "select");
        for (i = 0; i < lstOpcoes.options.length; i++)
            if (lstOpcoes.options[i].selected)
            {
                acrescimoLarguraItem[i] = acrescimoLargura;
                break;
            }
    }
    
    function getProduto()
    {
        openWindow(450, 700, '../Utils/SelProd.aspx');
    }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvBenef" runat="server" DataSourceID="odsBenefConfig"
                    DefaultMode="Insert" Height="50px" Width="125px" DataKeyNames="IdBenefConfig" SkinID="defaultDetailsView">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Nome"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                                Width="150px" OnLoad="txtNome_Load"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Cobrança Opcional"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCobrancaOpcional" runat="server" Checked='<%# Bind("CobrancaOpcional") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Descrição"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="50" Text='<%# Bind("Descricao") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label7" runat="server" Font-Bold="True" Text="Cobrar por Espessura"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCobrarPorEspessura" runat="server" Checked='<%# Bind("CobrarPorEspessura") %>'
                                                Enabled='<%# String.IsNullOrEmpty(Request["idBenefConfig"]) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label9" runat="server" Font-Bold="True" Text="Sub-grupo"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpSubgrupo" runat="server" AppendDataBoundItems="True" DataSourceID="odsSubgrupo"
                                                DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdSubgrupoProd") %>'
                                                Enabled='<%# String.IsNullOrEmpty(Request["idBenefConfig"]) %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label10" runat="server" Font-Bold="True" Text="Cobrar por Cor"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkCobrarPorCor" runat="server" Checked='<%# Bind("CobrarPorCor") %>'
                                                Enabled='<%# String.IsNullOrEmpty(Request["idBenefConfig"]) %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label5" runat="server" Font-Bold="True" Text="Controle"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="drpControle" runat="server" onchange="drpControleChanged(this.value);"
                                                DataSourceID="odsTiposControle" DataTextField="Translation" DataValueField="Key" AppendDataBoundItems="true"
                                                SelectedValue='<%# Bind("TipoControle") %>' Enabled='<%# String.IsNullOrEmpty(Request["idBenefConfig"]) %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Cálculo"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                        <asp:DropDownList ID="drpCalculo" runat="server" DataSourceID="odsTiposCalculo"
                                                            DataTextField="Translation" DataValueField="Key" AppendDataBoundItems="true"
                                                            SelectedValue='<%# Bind("TipoCalculo") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label8" runat="server" Font-Bold="True" Text="Situação"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;&nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                                            <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                                            <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label11" runat="server" Font-Bold="True" Text="Aplicação"></asp:Label>
                                        </td>
                                        <td>
                                            <table class="pos">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtAplIns" runat="server" onblur="aplAmbiente=false; loadApl(this.value);"
                                                            onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                            Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <a href="#" onclick="aplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                            <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                        </td>
                                        <td>
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label12" runat="server" Font-Bold="True" Text="Processo"></asp:Label>
                                                    </td> 
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <table class="pos" style="display: inline-table">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);"
                                                                        onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                                        onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <a href="#" onclick='procAmbiente=false; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                                        <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                                    </td>                                                   
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="left">
                                            <table>
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label14" runat="server" Font-Bold="True" Text="Não exibir descr. na impr. da etiqueta"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chkNaoExibirEtiqueta" runat="server" Checked='<%# Bind("NaoExibirEtiqueta") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label16" runat="server" Font-Bold="True" Text="Tipo"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlTipoBenef" runat="server" DataSourceID="odsTiposBenef"
                                                 DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("TipoBenef") %>'>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label19" runat="server" Font-Bold="True" Text="Cobrar área mínima do produto"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("CobrarAreaMinima") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="4">
                                            <table id="tbListaSelecao">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label6" runat="server" Font-Bold="False" Text="Adicionar opção"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtOpcao" runat="server" MaxLength="50" onkeypress="if (isEnter(event)) return addOpt();"
                                                            Rows="5" Width="293px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:ImageButton ID="imgAddOpt" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addOpt(); return false;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Processo (opção):
                                                        <table class="pos">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtProcItem" runat="server" onblur="isItem=true; loadProc(this.value);"
                                                                        onkeydown="if (isEnter(event)) { isItem=true; loadProc(this.value); }" onkeypress="return !(isEnter(event));"
                                                                        Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <a href="#" onclick='isItem=true; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                                        <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <br />
                                                        Aplicação (opção):
                                                        <table class="pos">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtAplItem" runat="server" onblur="isItem=true; loadApl(this.value);"
                                                                        onkeydown="if (isEnter(event)) { isItem=true; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                        Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <a href="#" onclick="isItem=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                        <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <asp:ListBox ID="lstOpcoes" runat="server" Rows="8" Width="297px" onchange="carregaItem(this)"
                                                            DataSource='<%# Eval("Filhos") %>' DataTextField="Nome" DataValueField="Nome">
                                                        </asp:ListBox>
                                                    </td>
                                                    <td>
                                                        <asp:ImageButton ID="imgRemOpt" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                            OnClientClick="remOpt(); return false;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <asp:CheckBox ID="chkUsarProdutoCompra" runat="server" Text="Associar produto de compra ao item do beneficiamento" ForeColor="Red"
                                                        onclick="mostrarProdutoCompra(this);" Checked="false" />
                                                </tr>
                                                <tr id="trProdutoCompra" style="display: none">
                                                    <td>
                                                        <asp:Label ID="Label13" runat="server" Text="Dados do produto a ser comprado:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label15" runat="server" Text="Cód.:"></asp:Label>
                                                        <asp:TextBox ID="txtCodProd" runat="server" onblur="loadProduto(this.value);" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                                            onkeypress="return !(isEnter(event));" Width="50px"></asp:TextBox>
                                                        <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                                        <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProd") %>' />
                                                        <a href="#" onclick="getProduto(); return false;">
                                                            <img src="../Images/Pesquisar.gif" border="0" alt="Pesquisar" /></a>
                                                    </td>
                                                </tr>
                                                <tr id="trProdutoCompra1" style="display: none">
                                                    <td>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblAcresAlt" runat="server" Text="Acrés. Altura MM: "></asp:Label>
                                                        <asp:TextBox ID="txtAcrescimoAltura" runat="server" onkeypress="return soNumeros(event, true, true);" onblur="setAcrescimoAlt(this.value);"
                                                            Text='<%# Eval("AcrescimoAltura") %>' Width="35px" MaxLength="4" ></asp:TextBox>
                                                        <asp:Label ID="lblAcresLarg" runat="server" Text="Acrés. Largura MM: "></asp:Label>
                                                        <asp:TextBox ID="txtAcrescimoLargura" runat="server" onkeypress="return soNumeros(event, true, true);" onblur="setAcrescimoLarg(this.value);"
                                                            Text='<%# Eval("AcrescimoLargura") %>' Width="35px" MaxLength="4" ></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                </td> </tr> </table>
                                <asp:HiddenField ID="hdfOpcoes" runat="server" Value='<%# Bind("ListaSelecao") %>' />
                                <asp:HiddenField ID="hdfItens" runat="server" Value='<%# Bind("ListaItens") %>'  />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="if (!onUpdate()) return false" />
                                <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar"
                                    CausesValidation="false" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="if (!onInsert()) return false" />
                                <asp:Button ID="btnVoltar" runat="server" OnClick="btnVoltar_Click" Text="Voltar"
                                    CausesValidation="false" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <InsertItemTemplate>
                            </InsertItemTemplate>
                            <EditItemTemplate>
                                <script type="text/javascript">
                                    <%#  ((Glass.Global.UI.Web.Process.Beneficiamentos.BenefConfigWrapper)GetDataItem()).GerarScriptIniciarItens() %>
                                </script>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>

                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsBenefConfig" runat="server"
                    DataObjectTypeName="Glass.Global.UI.Web.Process.Beneficiamentos.BenefConfigWrapper" 
                    InsertMethod="SalvarBenefConfig" 
                    SelectMethod="ObtemBenefConfig"
                    TypeName="Glass.Global.UI.Web.Process.Beneficiamentos.CadastroBenefConfig" 
                    UpdateMethod="SalvarBenefConfig"
                    UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idBenefConfig" QueryStringField="IdBenefConfig" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" 
                    SelectMethod="ObtemSubgruposProduto"
                    TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="idGrupoProd" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsTiposCalculo" SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoCalculoBenef, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsTiposControle" SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoControleBenef, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsTiposBenef" SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoBenef, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>

    <script type="text/javascript">

    var valor = FindControl("drpControle", "select").value;
    
    drpControleChanged(valor);

    </script>

</asp:Content>
