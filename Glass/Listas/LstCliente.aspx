<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCliente.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstCliente" Title="Clientes" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlConsultaCadCliSintegra.ascx" TagName="ctrlConsultaCadCliSintegra"
    TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function openAlterarVendedor() {
            openWindow(200, 400, "../Utils/AlterarVendedorCli.aspx");
        }

        function alteraVendedor(idVendedorNovo) {
            FindControl("hdfIdVendedorNovo", "input").value = idVendedorNovo;
            FindControl("btnAlterarVendedorCliente", "input").click();
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = LstCliente.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openRpt(exportarExcel, ficha, id) {
            var idCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var cpfCnpj = FindControl("txtCnpj", "input").value;
            var telefone = FindControl("txtTelefone", "input").value;
            var endereco = FindControl("txtEndereco", "input").value;
            var bairro = FindControl("txtBairro", "input").value;
            var situacao = FindControl("drpSituacao", "select").itens();
            var codRota = FindControl("txtRota", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var agruparVend = FindControl("chkAgruparVend", "input").checked;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var dataSemCompraIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataSemCompraFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataInativadoIni = FindControl("ctrlDataInIni_txtData", "input").value;
            var dataInativadoFim = FindControl("ctrlDataInFim_txtData", "input").value;
            var idCidade = FindControl("hdfCidade", "input").value;
            var idTipoCliente = FindControl("chkTipoCliente", "select").itens();
            var tipoFiscal = FindControl("cblTipoFiscal", "select").itens();
            var formasPagto = FindControl("cblFormasPagto", "select").itens();
            var controlTabelaDesconto = FindControl("drpTabelaDescontoAcrescimo", "select");
            var idTabelaDesconto = controlTabelaDesconto != null ? controlTabelaDesconto.value : 0;
            var apenasSemRota = FindControl("chkApenasSemRota", "input") == null ? "false" : FindControl("chkApenasSemRota", "input").checked;
            var exibirHistorico = FindControl("chkExibirHistorico", "input") != null ? FindControl("chkExibirHistorico", "input").checked : false;
            var uf = FindControl("drpUf", "select").value;

            if (id === 0)
                if (idCli == "" && nomeCli == "" && cpfCnpj == "" && telefone == "" && endereco == "" && bairro == "" &&
                    situacao == "" && codRota == "" && idLoja == "" && idFunc == "" && dataCadIni == "" &&
                    dataCadFim == "" && dataSemCompraIni == "" && dataSemCompraFim == "" && dataInativadoIni == "" && dataInativadoFim == "" &&
                    idCidade == "" && idTipoCliente == "" && tipoFiscal == "" && formasPagto === "" && idTabelaDesconto == "" && uf == "" && !apenasSemRota) {
                    if (!confirm("É recomendável aplicar um filtro. Deseja realmente prosseguir?"))
                        return false;
                }

            if (idCli == "")
                idCli = 0;

            if (id == 0) {
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=" + (ficha == true ? "Ficha" : "Lista") + "Clientes&dataIni=&dataFim=&Revenda=0&tipoPessoa=0&Compra=0" +
                    "&idCli=" + idCli + "&nome=" + nomeCli + "&cpfCnpj=" + cpfCnpj + "&telefone=" + telefone + "&endereco=" + endereco + "&idCidade=" + idCidade +
                    "&bairro=" + bairro + "&situacao=" + situacao + "&codRota=" + codRota + "&idFunc=" + idFunc + "&idLoja=" + idLoja + "&idTipoCliente=" + idTipoCliente +
                    "&tipoFiscal=" + tipoFiscal + "&formasPagto=" + formasPagto + "&exportarExcel=" + exportarExcel + "&agruparVend=" + agruparVend + "&dataCadIni=" + dataCadIni + "&dataCadFim=" + dataCadFim +
                    "&dataSemCompraIni=" + dataSemCompraIni + "&dataSemCompraFim=" + dataSemCompraFim + "&dataInativadoIni=" + dataInativadoIni + "&dataInativadoFim=" + dataInativadoFim +
                    "&idTabelaDesconto=" + idTabelaDesconto + "&apenasSemRota=" + apenasSemRota + "&exibirHistorico=" + exibirHistorico + "&uf=" + uf);
            }
            else {
                openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=FichaClientes&idCli=" + id);
            }
            return false;
        }

        function openRota() {
            if (FindControl("txtRota", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelRota.aspx");
            return false;
        }

        function setRota(codInterno) {
            FindControl("txtRota", "input").value = codInterno;
        }

        function setCidade(idCidade, nomeCidade) {
            FindControl('hdfCidade', 'input').value = idCidade;
            FindControl('txtCidade', 'input').value = nomeCidade;
        }

        function ativarTodos() {
            return confirm("ATENÇÃO: Essa opção ativará TODOS os clientes inativos que se encaixam nos filtros especificados.\nDeseja continuar?");
        }

        function openPrecoTabelaCliente(idCli) {
            window.location.href = "../Relatorios/ListaPrecoTabCliente.aspx?idCli=" + idCli;
        }
        
    </script>

    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label3" runat="server" Text="Cód. Cliente" AssociatedControlID="txtNumCli"></asp:Label>
                <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                    onblur="getCli(this);"></asp:TextBox>
            </span>
            <span>
                <asp:Label ID="Label12" runat="server" Text="Nome/Apelido" AssociatedControlID="txtNome"></asp:Label>
                <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click"
                    CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label5" runat="server" Text="CPF/CNPJ" AssociatedControlID="txtCnpj"></asp:Label>
                <asp:TextBox ID="txtCnpj" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label22" runat="server" Text="Loja Cliente" AssociatedControlID="drpLoja"></asp:Label>
                <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                    DataTextField="Name" DataValueField="Id" AutoPostBack="True">
                    <asp:ListItem Selected="True"></asp:ListItem>
                </asp:DropDownList>
            </span>
            <span>
                <asp:Label ID="Label8" runat="server" Text="Situação" AssociatedControlID="drpSituacao"></asp:Label>
                <sync:CheckBoxListDropDown ID="drpSituacao" runat="server" DataSourceID="odsSituacaoCliente"
                    DataTextField="Translation" DataValueField="Value" OnDataBound="drpSituacao_DataBound">
                </sync:CheckBoxListDropDown>
                <asp:ImageButton ID="imgPesq9" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label4" runat="server" Text="Telefone" AssociatedControlID="txtTelefone"></asp:Label>
                <asp:TextBox ID="txtTelefone" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label9" runat="server" Text="Endereço" AssociatedControlID="txtEndereco"></asp:Label>
                <asp:TextBox ID="txtEndereco" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label10" runat="server" Text="Bairro" AssociatedControlID="txtBairro"></asp:Label>
                <asp:TextBox ID="txtBairro" runat="server" Width="120px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label17" runat="server" Text="Cidade" AssociatedControlID="txtCidade"></asp:Label>
                <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Width="200px" onkeypress="return false;"></asp:TextBox>
                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar"
                    OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
            </span>
            <span>
                <asp:Label ID="Label24" runat="server" Text="UF" AssociatedControlID="txtCidade"></asp:Label>
                <asp:DropDownList ID="drpUf" runat="server" AppendDataBoundItems="True"
                    DataSourceID="odsUf" DataTextField="Value" DataValueField="Key">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label18" runat="server" Text="Tipo" AssociatedControlID="chkTipoCliente"></asp:Label>
                <sync:CheckBoxListDropDown ID="chkTipoCliente" runat="server" DataSourceID="odsTipoCliente"
                    DataTextField="Name" DataValueField="Id">
                </sync:CheckBoxListDropDown>

                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label20" runat="server" Text="Tipo Fiscal" AssociatedControlID="cblTipoFiscal"></asp:Label>
                <sync:checkboxlistdropdown ID="cblTipoFiscal" runat="server" Title="Selecione o tipo fiscal">
                    <asp:ListItem Value="1">Consumidor Final</asp:ListItem>
                    <asp:ListItem Value="2">Revenda</asp:ListItem>
                </sync:checkboxlistdropdown>
                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label23" runat="server" Text="Forma Pagto." AssociatedControlID="cblFormasPagto"></asp:Label>
                <sync:checkboxlistdropdown ID="cblFormasPagto" runat="server" Title="Selecione a forma pagto." DataSourceID="odsFormasPagto"
                    DataTextField="Descricao" DataValueField="IdFormaPagto" AppendDataBoundItems="True">
                    <asp:ListItem Value="0">Todas</asp:ListItem>
                </sync:checkboxlistdropdown>
                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label13" runat="server" Text="Rota" AssociatedControlID="txtRota"></asp:Label>
                <asp:TextBox ID="txtRota" runat="server" MaxLength="20" Width="80px"></asp:TextBox>
                <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClick="imgPesq_Click" OnClientClick="return openRota();" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label14" runat="server" Text="Vendedor" AssociatedControlID="drpFuncionario"></asp:Label>
                <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                    DataSourceID="odsFuncionario" DataTextField="Name" DataValueField="Id">
                    <asp:ListItem></asp:ListItem>
                </asp:DropDownList>
                <asp:ImageButton ID="imgPesq8" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                    OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:CheckBox ID="chkAgruparVend" runat="server" Text="Agrupar por Vendedor" />
            </span>
        </div>
        <div>
            <span>
                <asp:Label ID="Label21" runat="server" Text="Data Cad." AssociatedControlID="ctrlDataCadIni"></asp:Label>
                <uc2:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <uc2:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label15" runat="server" Text="Período sem comprar" AssociatedControlID="ctrlDataIni"></asp:Label>
                <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
            <span>
                <asp:Label ID="Label16" runat="server" Text="Período em que o cliente foi inativado"
                    AssociatedControlID="ctrlDataInIni"></asp:Label>
                <uc2:ctrlData ID="ctrlDataInIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <uc2:ctrlData ID="ctrlDataInFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                    ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                    OnClick="imgPesq_Click" CssClass="botaoPesquisar" />
            </span>
        </div>
        <div runat="server" id="tbDescAcresc">
            <span>
                <asp:Label ID="Label19" runat="server" Text="Tabela Desconto/Acréscimo" AssociatedControlID="drpTabelaDescontoAcrescimo"></asp:Label>
                <asp:DropDownList ID="drpTabelaDescontoAcrescimo" runat="server" AppendDataBoundItems="true"
                    DataSourceID="odsTabelaDescontoAcrescimo" DataTextField="Name" DataValueField="Id"
                    AutoPostBack="True">
                    <asp:ListItem Selected="True"></asp:ListItem>
                </asp:DropDownList>
            </span>
            <span>
                <asp:CheckBox ID="chkApenasSemRota" runat="server" Text="Apenas clientes sem rota vinculada"
                    AutoPostBack="True" />
            </span>
             <span>
                <asp:CheckBox ID="chkExibirHistorico" runat="server" Text="Exibir Histórico" />
            </span>
        </div>
    </div>
    <div class="inserir">
        <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir 
            Cliente</asp:LinkButton>
    </div>
    <asp:GridView ID="grdCli" runat="server" SkinID="defaultGridView"
        DataSourceID="odsCliente" DataKeyNames="IdCli" EmptyDataText="Nenhum cliente cadastrado"
        OnPageIndexChanged="grdCli_PageIndexChanged" OnRowCommand="grdCli_RowCommand"
        OnDataBound="grdCli_DataBound">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadCliente.aspx?idCli=" + Eval("IdCli") %>'>
                        <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                    <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                        ToolTip="Excluir" Visible='<%# ExcluirVisible() %>' OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Cliente?&quot;);" />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:PlaceHolder ID="pchDesconto" runat="server" Visible='<%# DescontoVisible(Container.DataItem) %>'>
                        <a href="#" onclick='openWindow(500, 650, &#039;../Cadastros/CadDescontoAcrescimoCliente.aspx?IdCliente=<%# Eval("IdCli") %>&#039;); return false;'>
                            <img src="../Images/money_delete.gif" border="0" title="Descontos/Acréscimos" /></a>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="pchFotos" runat="server" Visible='<%# FotosVisible() %>'>
                        <a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdCli") %>&amp;tipo=cliente&#039;); return false;'>
                            <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                    <asp:HyperLink ID="lnkSugestao" runat="server" NavigateUrl='<%# "../Listas/LstSugestaoCliente.aspx?idCliente=" + Eval("IdCli") %>'
                        ToolTip="Sugestões" Visible='<%# SugestoesVisible() %>'>
                        <img border="0" src="../Images/Nota.gif" /></asp:HyperLink>
                    <asp:ImageButton ID="imbInativar" runat="server" CommandArgument='<%# Eval("IdCli") %>'
                        CommandName="Inativar" ImageUrl="~/Images/Inativar.gif" OnClientClick="if (!confirm(&quot;Deseja alterar a situação desse cliente?&quot;)) return false"
                        ToolTip="Alterar situação" Visible='<%# InativarVisible(Container.DataItem) %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
             <asp:TemplateField HeaderText="Nome" SortExpression="Nome">
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Nome(Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
            <asp:BoundField DataField="EnderecoCompleto" HeaderText="Endereço" SortExpression="Endereco, Numero" />
            <asp:BoundField DataField="Telefone" HeaderText="Tel. Cont." SortExpression="TelCont, TelRes, TelCel" />
            <asp:BoundField DataField="TelCel" HeaderText="Celular" SortExpression="TelCel" />
            <asp:BoundField DataField="Situacao" HeaderText="Situação" SortExpression="Situacao" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="DtUltCompra" DataFormatString="{0:d}" HeaderText="Ult. Compra"
                SortExpression="DtUltCompra" />
            <asp:BoundField DataField="TotalComprado" DataFormatString="{0:C}" HeaderText="Total Comprado"
                SortExpression="TotalComprado" />
            <asp:TemplateField>
                <ItemTemplate>
                    <a href="#" onclick='TagToTip("cliente_<%# Eval("IdCli") %>", FADEIN, 300, COPYCONTENT, false, TITLE, "Detalhes", CLOSEBTN, true, CLOSEBTNTEXT, "Fechar", CLOSEBTNCOLORS, ["#cc0000", "#ffffff", "#D3E3F6", "#0000cc"], STICKY, true, FIX, [this, 10, 0]); return false;'>
                        <img src="../Images/user_comment.gif" border="0" /></a>
                    <div id="cliente_<%# Eval("IdCli") %>" style="display: none">
                        <asp:Label ID="Label1" runat="server" Text='<%# DataCadastro(Container.DataItem) %>'></asp:Label><br />
                        <asp:Label ID="Label2" runat="server" Text='<%# NomeUsuarioCadastro(Container.DataItem) %>'></asp:Label><br />
                        <asp:Label ID="Label6" runat="server" Text='<%# DataAlteracao(Container.DataItem) %>'></asp:Label><br />
                        <asp:Label ID="Label7" runat="server" Text='<%# NomeUsuarioAlteracao(Container.DataItem) %>'></asp:Label><br />
                        <asp:Label ID="Label11" runat="server" Style="font-weight: bold" Text='<%# ERevenda(Container.DataItem) %>'></asp:Label><br />
                    </div>
                    <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Cliente" IdRegistro='<%# (uint)(int)Eval("IdCli") %>' />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <uc3:ctrlConsultaCadCliSintegra runat="server" ID="ctrlConsultaCadCliSintegra1" IdCliente='<%# (uint)(int)Eval("IdCli") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <a href="#" onclick='openRpt(false, true, "<%# Eval("IdCli") %>"); return false;'>
                        <img src="../Images/printer.png" border="0" /></a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <a href="#" onclick='openPrecoTabelaCliente("<%# Eval("IdCli") %>"); return false;'>
                        <img src="../Images/cifrao.png" border="0" visible='<%# ExibirPrecoTabelaCliente() %>'
                            title="Preços de Tabela por Cliente" /></a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle />
        <EditRowStyle />
        <AlternatingRowStyle />
    </asp:GridView>
    <div class="inserir">
        <asp:LinkButton ID="lnkAtivarTodos" runat="server" OnClick="lnkAtivarTodos_Click"
            OnClientClick="return ativarTodos();" Visible="False">Ativar Clientes Inativos</asp:LinkButton>
    </div>
    <div style="text-align: center">
        <asp:Label ID="lblStatus" runat="server"></asp:Label>
    </div>
    <div class="imprimir">
        <div>
            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false, false, 0);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true, false, 0); return false;">
                <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkAlterarVendedor" runat="server" OnClientClick="openAlterarVendedor(); return false;">Alterar Vendedor</asp:LinkButton>
        </div>
        <div style="margin-top: 10px">
            <asp:LinkButton ID="lnkImprimirFicha" runat="server" OnClientClick="return openRpt(false, true, 0);">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir ficha</asp:LinkButton>
            &nbsp;&nbsp;&nbsp;
            <asp:LinkButton ID="lnkExportarFicha" runat="server" OnClientClick="openRpt(true, true, 0); return false;"><img border="0" 
                src="../Images/Excel.gif" /> Exportar ficha para o Excel</asp:LinkButton>
        </div>
    </div>
    <asp:HiddenField ID="hdfCidade" runat="server" Value='' />
    <div style="display: none">
        <asp:HiddenField ID="hdfIdVendedorNovo" runat="server" />
        <asp:Button ID="btnAlterarVendedorCliente" runat="server" OnClick="btnAlterarVendedorCliente_Click" />
    </div>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCliente" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Cliente"
        DeleteMethod="ApagarCliente" 
        DeleteStrategy="GetAndDelete"
        SelectMethod="PesquisarClientes" 
        SelectByKeysMethod="ObtemCliente"
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        EnablePaging="True" MaximumRowsParameterName="pageSize"
        SortParameterName="sortExpression">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeOuApelido" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtEndereco" Name="logradouro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="chkTipoCliente" Name="idsTipoCliente" PropertyName="SelectedValues" />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="txtRota" Name="codigoRota" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpFuncionario" Name="idVendedor" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cblTipoFiscal" Name="tiposFiscais" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="cblFormasPagto" Name="formasPagto" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadastroIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadastroFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dataSemCompraIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dataSemCompraFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInIni" Name="dataInativadoIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInFim" Name="dataInativadoFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="drpTabelaDescontoAcrescimo" Name="idTabelaDescontoAcrescimo" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="chkApenasSemRota" Name="apenasSemRota" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
            <asp:Parameter Name="limite" DefaultValue="0" Type="Int32" />
            <asp:Parameter Name="tipoPessoa" DefaultValue="" Type="String" />
            <asp:Parameter Name="comCompra" DefaultValue="False" Type="Boolean" />
        </SelectParameters>
    </colo:virtualobjectdatasource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsClienteAtualizarVendedor" runat="server" 
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        UpdateMethod="AlterarVendedorClientes">
        <UpdateParameters>
            <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeOuApelido" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtEndereco" Name="logradouro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="chkTipoCliente" Name="idsTipoCliente" PropertyName="SelectedValues"  />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="txtRota" Name="codigoRota" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpFuncionario" Name="idVendedor" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cblTipoFiscal" Name="tiposFiscais" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="cblFormasPagto" Name="formasPagto" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadastroIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadastroFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dataSemCompraIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dataSemCompraFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInIni" Name="dataInativadoIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInFim" Name="dataInativadoFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="drpTabelaDescontoAcrescimo" Name="idTabelaDescontoAcrescimo" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="chkApenasSemRota" Name="apenasSemRota" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="hdfIdVendedorNovo" Name="idVendedorNovo" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
        </UpdateParameters>
    </colo:virtualobjectdatasource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAtivarClientesInativos" runat="server" 
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        UpdateMethod="AtivarClientesInativos">
        <UpdateParameters>
            <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text" Type="Int32" />
            <asp:ControlParameter ControlID="txtNome" Name="nomeOuApelido" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtEndereco" Name="logradouro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="hdfCidade" Name="idCidade" PropertyName="Value" Type="Int32" />
            <asp:ControlParameter ControlID="chkTipoCliente" Name="idsTipoCliente" PropertyName="SelectedValues" />
            <asp:ControlParameter ControlID="txtRota" Name="codigoRota" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpFuncionario" Name="idVendedor" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="cblTipoFiscal" Name="tiposFiscais" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="cblFormasPagto" Name="formasPagto" PropertyName="SelectedValues" Type="Object" />
            <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadastroIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadastroFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dataSemCompraIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dataSemCompraFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInIni" Name="dataInativadoIni" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="ctrlDataInFim" Name="dataInativadoFim" PropertyName="DataNullable" Type="DateTime" />
            <asp:ControlParameter ControlID="drpTabelaDescontoAcrescimo" Name="idTabelaDescontoAcrescimo" PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="chkApenasSemRota" Name="apenasSemRota" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="drpUf" Name="uf" PropertyName="SelectedValue" Type="String" />
        </UpdateParameters>
    </colo:virtualobjectdatasource>
    <colo:virtualobjectdatasource Culture="pt-BR" ID="odsFuncionario" runat="server"
        SelectMethod="ObtemFuncionariosAtivosAssociadosAClientes" TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
    </colo:virtualobjectdatasource>
    <colo:virtualobjectdatasource Culture="pt-BR" ID="odsSituacaoCliente" runat="server"
        SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoCliente, Glass.Data" />
        </SelectParameters>
    </colo:virtualobjectdatasource>
    <colo:virtualobjectdatasource Culture="pt-BR" ID="odsTipoCliente" runat="server"
        SelectMethod="ObtemDescritoresTipoCliente" TypeName="Glass.Global.Negocios.IClienteFluxo">
    </colo:virtualobjectdatasource>
    <colo:virtualobjectdatasource Culture="pt-BR" ID="odsTabelaDescontoAcrescimo" runat="server"
        SelectMethod="ObtemDescritoresTabelaDescontoAcrescimo" TypeName="Glass.Global.Negocios.IClienteFluxo">
    </colo:virtualobjectdatasource>
    <colo:virtualobjectdatasource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="ObtemLojas"
        TypeName="Glass.Global.Negocios.ILojaFluxo">
    </colo:virtualobjectdatasource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormasPagto" runat="server" SelectMethod="GetForControle"
        TypeName="Glass.Data.DAL.FormaPagtoDAO">
    </colo:VirtualObjectDataSource>
     <colo:VirtualObjectDataSource ID="odsUf" runat="server" SelectMethod="GetUf" 
                    TypeName="Glass.Data.DAL.CidadeDAO">
                </colo:VirtualObjectDataSource>
</asp:Content>
