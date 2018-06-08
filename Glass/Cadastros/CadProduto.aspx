<%@ Page Title="Cadastro de Produto" Language="C#" MasterPageFile="~/Painel.master"
    EnableEventValidation="false" AutoEventWireup="true" EnableViewState="false" CodeBehind="CadProduto.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadProduto" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlSelCorProd.ascx" TagName="ctrlSelCorProd" TagPrefix="uc5" %>
<%@ Register src="../Controls/ctrlProdutoBaixaEstoque.ascx" tagname="ctrlProdutoBaixaEstoque" tagprefix="uc6" %>
<%@ Register src="../Controls/ctrlMvaProdutoPorUf.ascx" tagname="ctrlMvaProdutoPorUf" tagprefix="uc7" %>
<%@ Register src="../Controls/ctrlIcmsProdutoPorUf.ascx" tagname="ctrlIcmsProdutoPorUf" tagprefix="uc8" %>
<%@ Register src="../Controls/ctrlSelProduto.ascx" tagname="ctrlSelProduto" tagprefix="uc9" %>
<%@ Register src="../Controls/ctrlLojaNCM.ascx" tagname="ctrlLojaNCM" tagprefix="uc10" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery.utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function exibirBenef(botao) {
            var tabela = document.getElementById("tbConfigVidro");
            tabela.style.display = tabela.style.display == "" ? "none" : "";
        }
        
        function alteraItemGenerico()
        {
            var compra = FindControl('chkCompra', 'input');
            var itemGenerico = FindControl('chkItemGenerico', 'input');
            
            itemGenerico.parentNode.style.display = compra.checked ? '' : 'none';
            if (!compra.checked)
                itemGenerico.checked = false;
        }

        function loadSubgrupos(idGrupo) {
            try {
                var drpSubgrupo = FindControl("drpSubgrupo", "select");

                // Remove as opções do DropDownList dos subgrupos
                while (drpSubgrupo.options.length > 0)
                    drpSubgrupo.remove(0);

                // Recupera e insere as opções
                var subgrupos = CadProduto.GetSubgrupos(idGrupo).value.split('|');
                for (i = 0; i < subgrupos.length; i++) {
                    var dados = subgrupos[i].split(';');
                    var opcao = document.createElement("option");
                    opcao.value = dados[0];
                    opcao.text = dados[1];

                    drpSubgrupo.options.add(opcao);
                }

                drpSubgrupo.selectedIndex = 0;
                if (FindControl("hdfIdSubgrupo", "input") != null)
                    FindControl("hdfIdSubgrupo", "input").value = "";
            }
            catch (err) {
            }
        }

        function atualizaTipoMercadoria(tipoMercadoria){
            FindControl("hdfTipoMercadoria", "input").value = tipoMercadoria;
        }

        function alteraVisibilidade(idSubgrupo) {
            var tabela = document.getElementById("<%= dtvProduto.ClientID %>");
            var idGrupo = FindControl("drpGrupoProd", "select").value;
            var exibirProducao = CadProduto.ExibirProducao(idGrupo, idSubgrupo).value == "true";
            var exibirBeneficiamento = CadProduto.ExibirBenef(idGrupo, idSubgrupo).value == "true";
            FindControl("hdfSalvarBenef", "input").value = "true";
            var exibirAlturaLargura = CadProduto.ExibirAlturaLargura(idGrupo, idSubgrupo).value == "true";

            // Esconde campos referentes ao controle de produção: "Altura, Largura, Aplicação, Processo e Arquivo de mesa".
            for (i = 28; i < 33; i++)
                tabela.rows[i].style.display = exibirProducao ? "" : "none";
            
            if (!exibirProducao && !exibirAlturaLargura) {
                for (i = 28; i < 33; i++)
                    for (j = 0; j < tabela.rows[i].cells.length; j++) {
                        var inputs = tabela.rows[i].cells[j].getElementsByTagName("input");
                        for (k = 0; k < inputs.length; k++)
                            inputs[k].value = "";
                    }
            }

            if (exibirAlturaLargura && !exibirProducao) {
                for (i = 30; i < 33; i++)
                    for (j = 0; j < tabela.rows[i].cells.length; j++) {
                        var inputs = tabela.rows[i].cells[j].getElementsByTagName("input");
                        for (k = 0; k < inputs.length; k++)
                            inputs[k].value = "";
                    }
            }

            //Exibe os campos de altura e largura.
            if (exibirAlturaLargura) {

                tabela.rows[28].style.display = "";
                tabela.rows[29].style.display = "";
            }

            var indexBenef = 0;

            // Esconde campos referentes ao controle de beneficiamento: "Beneficiamento".
            for (i = 0; i < tabela.rows.length; i++) {
                if (tabela.rows[i].cells[0].innerText == "Beneficiamentos") {
                    indexBenef = i;
                    break;
                }
            }

            if (indexBenef > 0) {
                tabela.rows[indexBenef].style.display = exibirBeneficiamento ? "" : "none";

                if (!exibirBeneficiamento) {
                    for (j = 0; j < tabela.rows[indexBenef].cells.length; j++) {
                        var inputs = tabela.rows[indexBenef].cells[j].getElementsByTagName("input");
                        for (k = 0; k < inputs.length; k++)
                            inputs[k].value = "";
                    }
                }
            }

            FindControl("hdfIdSubgrupo", "input").value = idSubgrupo;

            ctrlProdutoBaixaEst.AtualizaVisibilidadeProcApl(idSubgrupo);

            // Exibe e esconde os campos de produto base e materia prima
            var tipoSubgrupo = CadProduto.ObterTipoSubgrupoPeloSubgrupo(idSubgrupo).value.split(';');
            if (tipoSubgrupo[0] == "Erro") {
                alert(tipoSubgrupo[1]);
                return false;
            }

            switch (tipoSubgrupo[1]) {
                case "ChapasVidro":
                    // Produto Base
                    tabela.rows[25].style.display = "";
                    // Materia Prima
                    tabela.rows[27].style.display = "none";
                    FindControl("ctrlProdutoBaixaEstoque1_ctrlSelProduto_ctrlSelProdBuscar_txtDescr", "input").value = "";
                    FindControl("ctrlProdutoBaixaEstoque1_ctrlSelProduto_ctrlSelProdBuscar_txtDescr", "input").onblur();
                    // Tipo Mercadoria
                    //MateriaPrima
                    FindControl("drpTipoMercadoria", "select").selectedIndex = 2
                    atualizaTipoMercadoria("MateriaPrima");
                    FindControl("drpTipoMercadoria", "select").disabled = "disabled";
                    break;
                case "ChapasVidroLaminado":
                    tabela.rows[25].style.display = "";
                    tabela.rows[27].style.display = "";
                    //MateriaPrima
                    FindControl("drpTipoMercadoria", "select").selectedIndex = 2
                    atualizaTipoMercadoria("MateriaPrima");
                    FindControl("drpTipoMercadoria", "select").disabled = "disabled";
                    break;
                case "VidroLaminado":
                    tabela.rows[25].style.display = "none";
                    FindControl("ctrlSelProd_ctrlSelProdBuscar_txtDescr", "input").value = "";
                    tabela.rows[27].style.display = "";
                    FindControl("drpTipoMercadoria", "select").disabled = "";
                    break;
                default:
                    tabela.rows[25].style.display = "none";
                    FindControl("ctrlSelProd_ctrlSelProdBuscar_txtDescr", "input").value = "";
                    tabela.rows[27].style.display = "";
                    FindControl("drpTipoMercadoria", "select").disabled = "";
                    break;
            }
        }

        function setBaixaEstFiscal(codInterno) {
            loadBaixaEstFiscal(codInterno);
        }

        function onSave() {
            if (!validate("produto"))
                return false;

            var fornec = FindControl("hdfFornec", "input").value;

            // Verifica se o Fornecedor foi selecionado
            if (fornec == "" || fornec == null) {
                alert("Informe o Fornecedor.");
                return false;
            }

            if (FindControl("txtCodInterno", "input").value == "") {
                alert("Informe o código interno.");
                return false;
            }

            if (FindControl("txtDescr", "input").value == "") {
                alert("Informe a descrição do produto.");
                return false;
            }

            for (i = 0; i < FindControl("drpGrupoProd", "select").options.length; i++) {
                if (FindControl("drpGrupoProd", "select").options[i].selected == true &&
                    FindControl("drpGrupoProd", "select").options[i].label.toUpperCase() == "VIDRO" &&
                    (FindControl("txtEspessura", "input").value == "" || FindControl("txtEspessura", "input").value == "0")) {
                    alert("Informe a espessura do produto.");
                    return false;
                }
            }

            if (FindControl("drpSubgrupo", "select").value == "" || FindControl("drpSubgrupo", "select").value == "0")
            {
                alert("Informe o subgrupo do produto.");
                return false;
            }
            else
                FindControl("hdfIdSubgrupo", "input").value = FindControl("drpSubgrupo", "select").value;

            var ncm = FindControl("txtNcm", "input").value;

            if (ncm == "") {
                alert("Informe NCM do produto.");
                return false;
            }

            // Se o NCM tiver sido informado, deve possuir exatos 8 dígitos
            if (ncm != "" && ncm.length != 8)
            {
                alert("O NCM deve possuir exatamente 8 dígitos.");
                return false;
            }
            
            // Se o produto não for vidro, exige que seja preenchido o peso do mesmo
            if (FindControl("drpGrupoProd", "select").value != "1")
            {
                var peso = FindControl("txtPeso", "input").value;

                if (peso == "" || peso == 0) {
                    alert("Informe o peso do produto.");
                    return false;
                }
            }

            var tipoCor = FindControl("ctrlSelCorProd1_drpTipoCor", "select") != null ? FindControl("ctrlSelCorProd1_drpTipoCor", "select").value : "";
            var corAluminio = FindControl("ctrlSelCorProd1_drpCorAluminio", "select") != null ? FindControl("ctrlSelCorProd1_drpCorAluminio", "select").value : "";
            var corFerragem = FindControl("ctrlSelCorProd1_drpCorFerragem", "select") != null ? FindControl("ctrlSelCorProd1_drpCorFerragem", "select").value : "";
            var corVidro = FindControl("ctrlSelCorProd1_drpCorVidro", "select") != null ? FindControl("ctrlSelCorProd1_drpCorVidro", "select").value : "";

            if (tipoCor != "0" && corAluminio == "" && corFerragem == "" && corVidro == "")
            {
                alert("Informe a cor do produto.");
                return false;
            }

            // Valida os campos de produto base e matéria prima
            var idSubgrupo = FindControl("hdfIdSubgrupo", "input").value;
            var tipoSubgrupo = CadProduto.ObterTipoSubgrupoPeloSubgrupo(idSubgrupo).value.split(';');
            if (tipoSubgrupo[0] == "Erro") {
                alert(tipoSubgrupo[1]);
                return false;
            }

            // Produto Base
            var produtoBase = FindControl("ctrlSelProd_ctrlSelProdBuscar_txtDescr", "input").value;
            // Materia Prima
            var materiaPrima = FindControl("ctrlProdutoBaixaEstoque1_ctrlSelProduto_ctrlSelProdBuscar_txtDescr", "input").value;
            var tipoMercadoria = FindControl("drpTipoMercadoria", "select").value;
            switch (tipoSubgrupo[1]) {
                case "ChapasVidro":
                    if (materiaPrima != "") {
                        alert("Para produtos de subgrupo do tipo chapa de vidro não deve ser informado matéria prima.");
                        return false;
                    }
                    if (tipoMercadoria != "MateriaPrima") {
                        alert("Para produtos de subgrupo do tipo chapa de vidro o tipo do produto deve ser matéria prima.");
                        return false;
                    }
                    break;
                case "ChapasVidroLaminado":
                    if (tipoMercadoria != "MateriaPrima") {
                        alert("Para produtos de subgrupo do tipo chapa de vidro laminado o tipo do produto deve ser matéria prima.");
                        return false;
                    }
                    break;
                case "VidroLaminado":
                    if (produtoBase != "") {
                        alert("Para produtos de subgrupo do tipo vidro laminado não deve ser informado produto base.");
                        return false;
                    }
                    if (materiaPrima == "") {
                        alert("Para produtos de subgrupo do tipo vidro laminado deve ser informado matéria prima.");
                        return false;
                    }
                    break;
                default:
                    if (produtoBase != "") {
                        alert("Para salvar as alterações nesse produto não deve ser informado produto base.");
                        return false;
                    }
                    break;
            }
        }

        function getFornec(idFornec) {
            var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                FindControl("hdfFornec", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornec", "input").value = retorno[1];
            FindControl("hdfFornec", "input").value = idFornec.value;
        }

        // Faz com que todos os textBoxes aceitem somente letras maiúsculas
        function txtToUpper() {
            var inputs = document.getElementsByTagName("input");

            var i = 0;
            for (i = 0; i < inputs.length; i++)
                if (inputs[i].getAttribute("type") == "text") {
                if (inputs[i].getAttribute("style") != null)
                    inputs[i].setAttribute("style", inputs[i].getAttribute("style") + ";text-transform:uppercase;");
                else
                    inputs[i].setAttribute("style", "text-transform:uppercase;");
            }
        }

        // Função chamada pelo popup de escolha da Aplicação do produto
        function setApl(idAplicacao, codInterno, idControle) {

            if (idControle != undefined && idControle != "") {
                ctrlProdutoBaixaEst.setApl(idAplicacao, codInterno, idControle);
                return;
            }

            if (!aplAmbiente) {
                FindControl("txtAplIns", "input").value = codInterno;
                FindControl("hdfIdAplicacao", "input").value = idAplicacao;
            }
            else {
                FindControl("txtAmbAplIns", "input").value = codInterno;
                FindControl("hdfAmbIdAplicacao", "input").value = idAplicacao;
            }

            aplAmbiente = false;
        }

        function loadApl(codInterno) {
            if (codInterno == undefined || codInterno == "") {
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
        function setProc(idProcesso, codInterno, codAplicacao, idControle) {

            if (idControle != undefined && idControle != "") {
                ctrlProdutoBaixaEst.setProc(idProcesso, codInterno, codAplicacao, idControle);
                return;
            }

            if (!procAmbiente) {
                FindControl("txtProcIns", "input").value = codInterno;
                FindControl("hdfIdProcesso", "input").value = idProcesso;
            }
            else {
                FindControl("txtAmbProcIns", "input").value = codInterno;
                FindControl("hdfAmbIdProcesso", "input").value = idProcesso;
            }

            if (codAplicacao != "") {
                aplAmbiente = procAmbiente;
                loadApl(codAplicacao);
            }

            procAmbiente = false;
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

        function openCest(){
                openWindow(300, 400, "../Utils/SelCest.aspx?");
        }

        function setCest(idCest, Codigo){            
            FindControl("txtCest", "input").value = Codigo;
            FindControl("hdfIdCest", "input").value = idCest;
        }

        function bloquearEspeciais(e) {
            if (e.currentTarget.value.includes(';')) {
                alert("O caractere ( ; ) não é permitido nesse campo.");
                e.currentTarget.value = "";
            }
        }

        function openProdMateriaPrima() {
            openWindow(600, 1000, "../Utils/SelProdMateriaPrima.aspx?idprod=" + GetQueryString("idProd"));
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvProduto" runat="server" AutoGenerateRows="False" DataKeyNames="IdProd"
                    DataSourceID="odsProduto" DefaultMode="Insert" GridLines="None" CssClass="gridStyle detailsViewStyle">
                    <RowStyle Wrap="True" />
                    <FieldHeaderStyle Wrap="True" CssClass="dtvHeader" />
                    <Fields>
                        <asp:TemplateField HeaderText="Código" SortExpression="CodInterno">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" onChange='bloquearEspeciais(event)' Text='<%# Bind("CodInterno") %>'
                                    MaxLength='50' ></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" onChange='bloquearEspeciais(event)' Text='<%# Bind("CodInterno") %>'
                                    MaxLength='50' ></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="IdFornec">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                    onblur="getFornec(this);" Text='<%# Eval("IdFornec") %>'></asp:TextBox>                                
                                <asp:TextBox ID="txtNomeFornec" runat="server" onkeypress="return false" 
                                             Text='<%# Eval("Fornecedor") == null ? null : ((Glass.Global.Negocios.Entidades.Fornecedor)Eval("Fornecedor")).Nome %>' Width="250px"></asp:TextBox>
                                <asp:Label ID="Label106" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                <asp:LinkButton ID="lnkSelFornec" runat="server" OnClientClick="openWindow(570, 760, '../Utils/SelFornec.aspx'); return false;">
                                    <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" />
                                </asp:LinkButton>
                                <asp:HiddenField ID="hdfFornec" runat="server" Value='<%# Bind("IdFornec") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("IdFornec") %>'></asp:Label>
                            </ItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                    onblur="getFornec(this);" Text='<%# Eval("IdFornec") %>'></asp:TextBox>                                
                                <asp:TextBox ID="txtNomeFornec" runat="server" Width="250px" onkeypress="return false"></asp:TextBox>
                                <asp:Label ID="Label106" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                <asp:LinkButton ID="lnkSelFornec" runat="server" OnClientClick="openWindow(570, 760, '../Utils/SelFornec.aspx'); return false;">
                                    <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" />
                                </asp:LinkButton>
                                <asp:HiddenField ID="hdfFornec" runat="server" Value='<%# Bind("IdFornec") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescr" runat="server" onChange='bloquearEspeciais(event)' Text='<%# Bind("Descricao") %>' Width="420px"
                                    MaxLength='80'></asp:TextBox>
                                <asp:Label ID="Label107" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDescr" runat="server" onChange='bloquearEspeciais(event)' Text='<%# Bind("Descricao") %>' Width="420px"
                                    MaxLength='80' ></asp:TextBox>
                                <asp:Label ID="Label107" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Preço de Custo">
                            <EditItemTemplate>
                                <table align="left">
                                    <tr>
                                        <td nowrap="nowrap">
                                            Custo Forn.:
                                            <uc1:ctrlTextBoxFloat ID="ctrFabBase" runat="server" Value='<%# Bind("CustoFabBase") %>' />
                                            &nbsp;
                                        </td>
                                        <td nowrap="nowrap">
                                            Custo Imp.:
                                            <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" Value='<%# Bind("CustoCompra") %>' />
                                        </td>
                                        <td nowrap="nowrap" runat="server" id="tbMarkup" onload="tbMarkup_Load">
                                            MarkUp:
                                            <uc1:ctrlTextBoxFloat ID="ctrlMarkUp" runat="server" Value='<%# Bind("MarkUp") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table align="left">
                                    <tr>
                                        <td nowrap="nowrap">
                                            Custo Forn.:
                                            <uc1:ctrlTextBoxFloat ID="ctrFabBase" runat="server" Value='<%# Bind("CustoFabBase") %>' />
                                            &nbsp;
                                        </td>
                                        <td nowrap="nowrap">
                                            Custo Imp.:
                                            <uc1:ctrlTextBoxFloat ID="ctrlTextBoxFloat1" runat="server" Value='<%# Bind("CustoCompra") %>' />
                                        </td>
                                        <td nowrap="nowrap" runat="server" id="tbMarkup" onload="tbMarkup_Load">
                                            MarkUp:
                                            <uc1:ctrlTextBoxFloat ID="ctrlMarkUp" runat="server" Value='<%# Bind("MarkUp") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo" SortExpression="IdTipoProd">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdTipoProd") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="drpGrupoProd" runat="server" onchange="loadSubgrupos(this.value)"
                                                DataSourceID="odsGrupoProd" DataTextField="Name" DataValueField="Id"
                                                SelectedValue='<%# Bind("IdGrupoProd") %>' OnDataBound="drpGrupoProdEdit_DataBound">
                                            </asp:DropDownList>
                                        </td>
                                        <td class="dtvHeader">
                                            Subgrupo:
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpSubgrupo" runat="server" onchange="alteraVisibilidade(this.value)"
                                                SelectedValue='<%# Eval("IdSubgrupoProd") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="Label108" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdSubgrupo" runat="server" Value='<%# Bind("IdSubgrupoProd") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="drpGrupoProd" runat="server" DataSourceID="odsGrupoProd"
                                                onchange="loadSubgrupos(this.value)" DataTextField="Name" DataValueField="Id"
                                                SelectedValue='<%# Bind("IdGrupoProd") %>' OnDataBound="drpGrupoProdEdit_DataBound">
                                            </asp:DropDownList>
                                        </td>
                                        <td class="dtvHeader">
                                            Subgrupo:
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpSubgrupo" runat="server" onchange="alteraVisibilidade(this.value)">
                                            </asp:DropDownList>
                                            <asp:Label ID="Label108" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdSubgrupo" runat="server" Value='<%# Bind("IdSubgrupoProd") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Preço de Venda">
                            <EditItemTemplate>
                                <table align="left">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lbl1" runat="server" Text="Atacado"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctrAtacado" runat="server" Value='<%# Bind("ValorAtacado") %>' />
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lbl11" runat="server" Text="Obra"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctvObraPequena" runat="server" Value='<%# Bind("ValorObra") %>' />
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label31" runat="server" Text="Valor Fiscal"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctvValorFiscal" runat="server" Value='<%# Bind("ValorFiscal") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lbl23" runat="server" Text="Balcão"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctrBalcao" runat="server" Value='<%# Bind("ValorBalcao") %>' />
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lbl24" runat="server" Text="Reposição"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctrRepos" runat="server" Value='<%# Bind("ValorReposicao") %>' />
                                        </td>
                                        <td align="left" nowrap="nowrap" style='<%= Glass.Configuracoes.OrdemCargaConfig.UsarControleOrdemCarga ? "": "display: none" %>'>
                                            <asp:Label ID="Label21" runat="server" Text="Valor Transferência"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style='<%= Glass.Configuracoes.OrdemCargaConfig.UsarControleOrdemCarga ? "": "display: none" %>'>
                                            <uc1:ctrlTextBoxFloat ID="CtrlTextBoxFloat2" runat="server" Value='<%# Bind("ValorTransferencia") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table align="left">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lbl2" runat="server" Text="Atacado"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctrAtacado" runat="server" Value='<%# Bind("ValorAtacado") %>' />
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lbl12" runat="server" Text="Obra"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctvObraPequena" runat="server" Value='<%# Bind("ValorObra") %>' />
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label31" runat="server" Text="Valor Fiscal"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctvValorFiscal" runat="server" Value='<%# Bind("ValorFiscal") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lbl30" runat="server" Text="Balcão"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctrBalcao" runat="server" Value='<%# Bind("ValorBalcao") %>' />
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lbl24" runat="server" Text="Reposição"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc1:ctrlTextBoxFloat ID="ctrRepos" runat="server" Value='<%# Bind("ValorReposicao") %>' />
                                        </td>
                                        <td align="left" nowrap="nowrap" style='<%= Glass.Configuracoes.OrdemCargaConfig.UsarControleOrdemCarga ? "": "display: none" %>'>
                                            <asp:Label ID="Label21" runat="server" Text="Valor Transferência"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style='<%= Glass.Configuracoes.OrdemCargaConfig.UsarControleOrdemCarga ? "": "display: none" %>'>
                                            <uc1:ctrlTextBoxFloat ID="CtrlTextBoxFloat2" runat="server" Value='<%# Bind("ValorTransferencia") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alíquotas">
                            <EditItemTemplate>
                                <table align="left">
                                    <tr>
                                        <td colspan="2" style="padding: 0px; position: relative; left: -1px">
                                            <table class="pos">
                                                <tr>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lbl13" runat="server" Text="Aliq. IPI"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctvAliqIPI" runat="server" 
                                                            Value='<%# Bind("AliqIPI") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lbl3" runat="server" Text="Aliq. ICMS"></asp:Label>
                                            &nbsp;
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc8:ctrlIcmsProdutoPorUf ID="ctrlIcmsProdutoPorUf1" runat="server" EnableViewState="false" 
                                                    AliquotasIcms='<%# Bind("AliquotasIcms") %>' ValidationGroup="produto" />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CST" SortExpression="Cst">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCst" runat="server" SelectedValue='<%# Bind("Cst") %>'>
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem>60</asp:ListItem>
                                    <asp:ListItem>00</asp:ListItem>
                                    <asp:ListItem>10</asp:ListItem>
                                    <asp:ListItem>20</asp:ListItem>
                                    <asp:ListItem>30</asp:ListItem>
                                    <asp:ListItem>40</asp:ListItem>
                                    <asp:ListItem>41</asp:ListItem>
                                    <asp:ListItem>50</asp:ListItem>
                                    <asp:ListItem>51</asp:ListItem>
                                    <asp:ListItem>70</asp:ListItem>
                                    <asp:ListItem>90</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpCst" runat="server" SelectedValue='<%# Bind("Cst") %>'>
                                    <asp:ListItem>60</asp:ListItem>
                                    <asp:ListItem>00</asp:ListItem>
                                    <asp:ListItem>10</asp:ListItem>
                                    <asp:ListItem>20</asp:ListItem>
                                    <asp:ListItem>30</asp:ListItem>
                                    <asp:ListItem>40</asp:ListItem>
                                    <asp:ListItem>41</asp:ListItem>
                                    <asp:ListItem>50</asp:ListItem>
                                    <asp:ListItem>51</asp:ListItem>
                                    <asp:ListItem>70</asp:ListItem>
                                    <asp:ListItem>90</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Cst") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CSOSN" SortExpression="Csosn">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpCsosn" runat="server" AppendDataBoundItems="True" DataSourceID="odsCsosn"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("Csosn") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpCsosn" runat="server" AppendDataBoundItems="True" DataSourceID="odsCsosn"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("Csosn") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label20" runat="server" Text='<%# Bind("Csosn") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="NCM" SortExpression="Ncm">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td> <asp:TextBox ID="txtNcm" runat="server" MaxLength="20" Text='<%# Bind("Ncm") %>'></asp:TextBox>
                                            <asp:Label ID="Label109" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                        </td>                                        
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc10:ctrlLojaNCM runat="server" ID="ctrlLojaNCM" NCMs='<%# Bind("NCMs") %>'/>                                            
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNcm" runat="server" MaxLength="20" Text='<%# Bind("Ncm") %>'></asp:TextBox>
                                <asp:Label ID="Label109" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("Ncm") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código EX" SortExpression="CodigoEX">
                            <ItemTemplate>
                                <asp:Label ID="Label24" runat="server" Text='<%# Bind("CodigoEX") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodigoEx" runat="server" Text='<%# Bind("CodigoEX") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCodigoEx" runat="server" Text='<%# Bind("CodigoEX") %>' Width="50px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gênero Produto" SortExpression="IdGeneroProduto">
                            <ItemTemplate>
                                <asp:Label ID="Label23" runat="server" Text='<%# Bind("IdGeneroProduto") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpGeneroProduto" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsGeneroProduto" DataTextField="CodigoDescricao" DataValueField="IdGeneroProduto"
                                    SelectedValue='<%# Bind("IdGeneroProduto") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpGeneroProduto" runat="server" AppendDataBoundItems="True"
                                    DataSourceID="odsGeneroProduto" DataTextField="CodigoDescricao" DataValueField="IdGeneroProduto"
                                    SelectedValue='<%# Bind("IdGeneroProduto") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="GTIN Produto" SortExpression="GTINProduto">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtGtinProduto" runat="server" MaxLength="14" Text='<%# Bind("GTINProduto") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtGtinProduto" runat="server" MaxLength="14" Text='<%# Bind("GTINProduto") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label28" runat="server" Text='<%# Bind("GTINProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="GTIN Unid. Trib." SortExpression="GTINUnidTrib">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtGtinUnidTrib" runat="server" MaxLength="14" Text='<%# Bind("GTINUnidTrib") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtGtinUnidTrib" runat="server" MaxLength="14" Text='<%# Bind("GTINUnidTrib") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label29" runat="server" Text='<%# Bind("GTINUnidTrib") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="MVA" SortExpression="Mva">
                            <EditItemTemplate>
                                <uc7:ctrlMvaProdutoPorUf ID="ctrlMvaProdutoPorUf" runat="server" 
                                    ValidationGroup="produto"
                                    Mva='<%# Bind("Mva") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("DescrMva") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Área Mínima (m²)" SortExpression="AreaMinima">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("AreaMinima") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkAtivAreaMin" runat="server" Checked='<%# Bind("AtivarAreaMinima") %>'
                                    Text="Ativar" />
                                &nbsp;<uc1:ctrlTextBoxFloat ID="ctrAreaMin" runat="server" Value='<%# Bind("AreaMinima") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:CheckBox ID="chkAtivAreaMin" runat="server" Checked='<%# Bind("AtivarAreaMinima") %>'
                                    Text="Ativar" />
                                &nbsp;<uc1:ctrlTextBoxFloat ID="ctrAreaMin" runat="server" Value='<%# Bind("AreaMinima") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Unidade" SortExpression="Unidade">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Unidade") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpUnidade" runat="server" DataSourceID="odsUnidadeMedida"
                                    DataTextField="Name" DataValueField="Id" 
                                    SelectedValue='<%# Bind("IdUnidadeMedida") %>' AppendDataBoundItems="True">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpUnidade" runat="server" DataSourceID="odsUnidadeMedida"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdUnidadeMedida") %>'>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Unidade Trib." SortExpression="UnidadeTrib">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("UnidadeTrib") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpUnidadeTrib" runat="server" DataSourceID="odsUnidadeMedida"
                                    DataTextField="Name" DataValueField="Id" 
                                    SelectedValue='<%# Bind("IdUnidadeMedidaTrib") %>' AppendDataBoundItems="True">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpUnidadeTrib" runat="server" DataSourceID="odsUnidadeMedida"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdUnidadeMedidaTrib") %>'>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cod. Otimização" SortExpression="CodOtimizacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodOtimizacao" runat="server" Text='<%# Bind("CodOtimizacao") %>' MaxLength="13"  Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCodOtimizacao" runat="server" Text='<%# Bind("CodOtimizacao") %>' MaxLength="13" Width="70px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label17" runat="server" Text='<%# Bind("CodOtimizacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cor" SortExpression="IdCorVidro, IdCorAluminio, IdCorFerragem">
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("IdCorVidro") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc5:ctrlSelCorProd ID="ctrlSelCorProd1" runat="server" IdCorAluminioInt32='<%# Bind("IdCorAluminio") %>'
                                    IdCorFerragemInt32='<%# Bind("IdCorFerragem") %>' IdCorVidroInt32='<%# Bind("IdCorVidro") %>'
                                    OnLoad="ctrlSelCorProd1_Load" />
                                <asp:Label ID="Label110" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Espessura" SortExpression="Espessura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEspessura" runat="server" MaxLength="4" Text='<%# Bind("Espessura") %>'
                                    onkeypress="return soNumeros(event, false, true);" Width="50px"></asp:TextBox>
                                &nbsp;mm
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtEspessura" runat="server" MaxLength="4" Text='<%# Bind("Espessura") %>'
                                    onkeypress="return soNumeros(event, false, true);" Width="50px"></asp:TextBox>
                                &nbsp;mm
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Espessura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Peso" SortExpression="Peso">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("Peso") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPeso" runat="server" Text='<%# Bind("Peso") %>' Width="50px"
                                    onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                <asp:Label ID="Label111" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                &nbsp;kg
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtPeso" runat="server" Text='<%# Bind("Peso") %>' Width="50px"
                                    onkeypress="return soNumeros(event, false, true);" Height="22px"></asp:TextBox>
                                <asp:Label ID="Label111" runat="server" Text="&nbsp;*" ForeColor="Red"></asp:Label>
                                &nbsp;kg
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Compra" SortExpression="Compra">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("Compra") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkCompra" runat="server" Checked='<%# Bind("Compra") %>' 
                                    onclick="alteraItemGenerico()" />
                                <span>
                                    <asp:CheckBox ID="chkItemGenerico" runat="server" Checked='<%# Bind("ItemGenerico") %>' Text="Item genérico"
                                        style="margin-left: 16px" />
                                </span>
                                <script type="text/javascript">
                                    alteraItemGenerico();
                                </script>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forma" SortExpression="Forma">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtForma" runat="server" MaxLength="8" Text='<%# Bind("Forma") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtForma" runat="server" MaxLength="8" Text='<%# Bind("Forma") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label22" runat="server" Text='<%# Bind("Forma") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto Base" SortExpression="IdProdBase">
                            <EditItemTemplate>
                               <uc9:ctrlSelProduto runat="server" ID="ctrlSelProd" IdProdInt32='<%# Bind("IdProdBase") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estoque fiscal - Produto para baixa" SortExpression="IdProdBaixaEstoqueFiscal">
                            <EditItemTemplate>
                                <uc6:ctrlProdutoBaixaEstoque ID="ctrlProdutoBaixaEstoque2" runat="server" 
                                    BaixasEstoqueFiscal='<%# Bind("BaixasEstoqueFiscal") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Matéria Prima">
                            <EditItemTemplate>                                
                                <asp:ImageButton ID="imbOpenProdMateriaPrima" Style="float: left" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="openProdMateriaPrima(); return false;" />
                                <asp:Label Text="Adicionar Matéria Prima" runat="server" />
<%--                            <div runat="server" id="divProdBaixaEstoque" clientidmode="Static">
                                <uc6:ctrlProdutoBaixaEstoque ID="ctrlProdutoBaixaEstoque1" runat="server"
                                    BaixasEstoque='<%# Bind("BaixasEstoque") %>' />
                            </div>--%>
                            </EditItemTemplate>
                         </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <ItemTemplate>
                                <asp:Label ID="Label18" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" Text='<%# Bind("Altura") %>' onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" Text='<%# Bind("Altura") %>' onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                            <ItemTemplate>
                                <asp:Label ID="Label19" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" Text='<%# Bind("Largura") %>' onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" Text='<%# Bind("Largura") %>' onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                            <EditItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="aplAmbiente=false; loadApl(this.value);"
                                                onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                Text='<%# Eval("Aplicacao") != null ? ((Glass.Global.Negocios.Entidades.EtiquetaAplicacao)Eval("Aplicacao")).CodInterno : null %>' Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="aplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="aplAmbiente=false; loadApl(this.value);"
                                                onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="aplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("Aplicacao") != null ? ((Glass.Global.Negocios.Entidades.EtiquetaAplicacao)Eval("Aplicacao")).CodInterno : null %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                            <EditItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);"
                                                onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                onkeypress="return !(isEnter(event));" Width="30px" 
                                                Text='<%# Eval("Processo") != null ? ((Glass.Global.Negocios.Entidades.EtiquetaProcesso)Eval("Processo")).CodInterno : null %>'></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick='procAmbiente=false; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);"
                                                onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick='procAmbiente=false; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("Processo") != null ? ((Glass.Global.Negocios.Entidades.EtiquetaProcesso)Eval("Processo")).CodInterno : null %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Arq. Mesa Corte">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpArquivo" runat="server" AppendDataBoundItems="True" DataSourceID="odsArquivoMesaCorte"
                                    DataTextField="Codigo" DataValueField="IdArquivoMesaCorte" SelectedValue='<%# Bind("IdArquivoMesaCorte") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="drpTipoArquivo" runat="server" AppendDataBoundItems="true" DataSourceID="odsTipoArquivoMesaCorte"
                                    DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("TipoArquivo") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <sync:CheckBoxListDropDown ID="drpFlagArqMesa" runat="server" DataSourceID="odsFlagArqMesa" AppendDataBoundItems="true"
                                    DataTextField="Name" DataValueField="Id" Title="" SelectedValues='<%# Bind("FlagsArqMesa") %>'>
                                </sync:CheckBoxListDropDown>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpArquivo" runat="server" AppendDataBoundItems="True" DataSourceID="odsArquivoMesaCorte"
                                    DataTextField="Codigo" DataValueField="IdArquivoMesaCorte" SelectedValue='<%# Bind("IdArquivoMesaCorte") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="drpTipoArquivo" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoArquivoMesaCorte"
                                    DataTextField="Translation" DataValueField="Key">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <sync:CheckBoxListDropDown ID="drpFlagArqMesa" runat="server" DataSourceID="odsFlagArqMesa" AppendDataBoundItems="true"
                                    DataTextField="Name" DataValueField="Id" Title="" SelectedValues='<%# Bind("FlagsArqMesa") %>'>
                                </sync:CheckBoxListDropDown>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Beneficiamentos">
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;">
                                    <img border="0" src="../Images/gear_add.gif" alt="Adicionar" />
                                </asp:LinkButton>
                                <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                    <tr>
                                        <td>
                                            <uc3:ctrlBenef ID="ctrlBenef1" runat="server" OnLoad="ctrlBenef1_Load" Redondo='<%# Bind("Redondo") %>'
                                                ValidationGroup="produto" 
                                                Beneficiamentos2='<%# Bind("ProdutoBeneficiamentos") %>' 
                                                TipoBeneficiamento="Glass.Global.Negocios.Entidades.ProdutoBenef"
                                                CarregarBenefPadrao="false"
                                                CalcularBeneficiamentoPadrao="true" />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Mercadoria" SortExpression="TipoMercadoria">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoMercadoria" runat="server" AppendDataBoundItems="True" onchange="atualizaTipoMercadoria(this.value)"
                                    DataSourceID="odsTipoMercadoria" DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Eval("TipoMercadoria") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdfTipoMercadoria" runat="server" Value='<%# Bind("TipoMercadoria") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label25" runat="server" Text='<%# Bind("TipoMercadoria") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Imagem" SortExpression="Imagem">
                            <ItemTemplate>
                                <asp:Label ID="Label22" runat="server" Text='<%# Bind("Imagem") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:FileUpload ID="filImagem" runat="server" accept="image/*"/>
                                <uc4:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.ProdutoRepositorioImagens.Instance.ObtemUrl((int)Eval("IdProd")) %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:FileUpload ID="filImagem" runat="server"  accept="image/*"/>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                    TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                    TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Local de Armazenagem" SortExpression="LocalArmazenagem">
                            <ItemTemplate>
                                <asp:Label ID="lblLocalArmazenagem" runat="server" Text='<%# Bind("LocalArmazenagem") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLocalArmazenagem" runat="server" Text='<%# Bind("LocalArmazenagem") %>'
                                    Width="240px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtLocalArmazenagem" runat="server" Text='<%# Bind("LocalArmazenagem") %>'
                                    Width="241px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Plano de Conta Contábil" SortExpression="IdContaContabil">
                            <EditItemTemplate>
                                <asp:DropDownList ID="DropDownList1" runat="server" AppendDataBoundItems="True" DataSourceID="odsPlanoContaContabil"
                                    DataTextField="Descricao" DataValueField="IdContaContabil" SelectedValue='<%# Bind("IdContaContabil") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label27" runat="server" Text='<%# Bind("IdContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cest" SortExpression="Cest">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCest" runat="server" MaxLength="7" style="float:left" Text='<%# Eval ("Cest.Codigo") %>' Enabled="false"></asp:TextBox>
                                <asp:HiddenField ID="hdfIdCest" runat="server" Value='<%# Bind("IdCest") %>'/>
                                <asp:ImageButton ID="imbOpenCest" style="float:left" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="openCest(); return false;" />
                            </EditItemTemplate>
                            <InsertItemTemplate> 
                                <asp:TextBox ID="txtCest" runat="server" MaxLength="7" style="float:left" Text='<%# Eval ("Cest.Codigo") %>' Enabled="false"></asp:TextBox>
                                <asp:HiddenField ID="hdfIdCest" runat="server" Value='<%# Bind("IdCest") %>' />
                                <asp:ImageButton ID="imbOpenCest" style="float:left" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                    OnClientClick="openCest(); return false;" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCest" runat="server" Text='<%# Bind("Cest") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="return onSave();" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="False" />
                                <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdProd"
                                    Text='<%# Bind("IdProd") %>' />
                                <asp:HiddenField ID="hdfSalvarBenef" runat="server" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onSave();" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="False" />
                                <asp:HiddenField ID="hdfSalvarBenef" runat="server" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                    <InsertRowStyle HorizontalAlign="Left" Wrap="False" />
                    <EditRowStyle HorizontalAlign="Left" Wrap="False" />
                    <AlternatingRowStyle Wrap="True" CssClass="alt" />
                </asp:DetailsView>

                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFlagArqMesa" runat="server"
                    SelectMethod="ObtemFlagsArqMesa"
                    TypeName="Glass.Projeto.Negocios.IFlagArqMesaFluxo">
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Produto"
                    TypeName="Glass.Global.Negocios.IProdutoFluxo"
                    CreateDataObjectMethod="CriarProduto"
                    InsertMethod="SalvarProduto" 
                    SelectMethod="ObtemProduto"  
                    UpdateMethod="SalvarProduto" 
                    UpdateStrategy="GetAndUpdate"
                    OnInserted="odsProduto_Inserted" OnUpdated="odsProduto_Updated"
                    OnInserting="odsProduto_Inserting" >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProd" QueryStringField="idProd" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoProd" runat="server" 
                    SelectMethod="ObtemGruposProduto" TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCsosn" runat="server" 
                    SelectMethod="ObtemCSOSNs" TypeName="Glass.Fiscal.Negocios.INotaFiscalFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsUnidadeMedida" runat="server" 
                    SelectMethod="ObtemUnidadesMedida" TypeName="Glass.Global.Negocios.IUnidadesFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGeneroProduto" runat="server" 
                    SelectMethod="ObtemGenerosProduto" TypeName="Glass.Global.Negocios.IProdutoFluxo"></colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdPrimeiroProduto" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoMercadoria" runat="server" 
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoMercadoria, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstIpi" runat="server" 
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.ProdutoCstIpi, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContaContabil" runat="server" SelectMethod="GetSorted"
                    TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="natureza" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsArquivoMesaCorte" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ArquivoMesaCorteDAO"></colo:VirtualObjectDataSource>
                <colo:virtualobjectdatasource culture="pt-BR" ID="odsTipoArquivoMesaCorte" runat="server" 
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoArquivoMesaCorte, Glass.Data" />
                    </SelectParameters>
                </colo:virtualobjectdatasource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        alteraVisibilidade(FindControl("drpSubgrupo", "select").value);
        txtToUpper();
    </script>

</asp:Content>
