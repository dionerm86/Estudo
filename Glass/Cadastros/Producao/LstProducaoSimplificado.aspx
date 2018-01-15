<%@ Page Title="Consulta Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstProducaoSimplificado.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstProducaoSimplificado" %>


<%@ Register Src="../../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlBenefSetor.ascx" TagName="ctrlBenefSetor" TagPrefix="uc2" %>
<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrllogpopup" TagPrefix="uc4" %>
<%@ Register Src="../../Controls/ctrlImageCadProject.ascx" TagName="ctrlImageCadProject" TagPrefix="uc5" %>
<%@ Register Src="../../Controls/ctrlLstProdProducao.ascx" TagName="ctrlLstProdProducao" TagPrefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function exibirBenef(botao) {
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('benef', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamentos', CLOSEBTN, true,
                CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 0, 0]);
            }
        }

        function abrirDetalhesReposicao(idProdPedProducao) {
            openWindow(600, 800, "../../Utils/DetalhesReposicaoPeca.aspx?idProdPedProducao=" + idProdPedProducao);
        }

        function openRpt(exportarExcel, setorFiltrado, roteiro) {

            var idLiberarPedido = FindControl("txtNumLiberarPedido", "input").value;
            var idPedidoImportado = FindControl("txtNumPedidoImportado", "input").value;
            var dataIniConfPed = FindControl("ctrlDataIniConfPed_txtData", "input").value;
            var dataFimConfPed = FindControl("ctrlDataFimConfPed_txtData", "input").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var pecasProdCanc = FindControl("cbdExibirPecas", "select").itens();
            var idFunc = FindControl("drpFuncionario", "select").value;
            var tipoPedido = FindControl("drpTipoPedido", "select").itens();
            var aguardEntrEstoque = FindControl("chkAguardEntrEstoque", "input").checked;
            var fastDelivery = FindControl("ddlFastDelivery", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;

            var relatorio = roteiro ? "RelBase.aspx?rel=ProducaoPassou" : agrupar == "3" ? "Producao/RelBase.aspx?rel=ProducaoContagem" : "RelBase.aspx?rel=Producao";

            openWindow(600, 800, "../../Relatorios/" + relatorio +
                "&idLiberarPedido=" + idLiberarPedido +
                "&idPedidoImportado=" + idPedidoImportado +
                "&dataIniConfPed=" + dataIniConfPed +
                "&dataFimConfPed=" + dataFimConfPed +
                "&agrupar=" + agrupar +
                "&pecasProdCanc=" + pecasProdCanc +
                "&idFunc=" + idFunc +
                "&tipoPedido=" + tipoPedido +
                "&aguardEntrEstoque=" + aguardEntrEstoque +
                "&fastDelivery=" + fastDelivery +
                "&idLoja=" + idLoja+
                "&produtoComposicao=2");
        }

        function openRptPed(idPedido, isReposicao, tipo, original) {
            var url = original ? "../../Relatorios/RelPedido.aspx?idPedido=" + idPedido :
                "../../Relatorios/RelBase.aspx?rel=PedidoPcp&idPedido=" + idPedido;

            openWindow(600, 800, url);
            return false;
        }

        function getCli(idCli) {
            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function leuEtiqueta(txtNumEtiqueta) {
            if (txtNumEtiqueta == null || txtNumEtiqueta == undefined)
                return;

            txtNumEtiqueta.value = corrigeLeituraEtiqueta(txtNumEtiqueta.value);
        }

        var idCliente = '<%= Request["cliente"]%>';

        function openSetMotivPararPecaProducao(idProdPedProducao, pecaParadaProducao) {

            if (idProdPedProducao == null || idProdPedProducao == 0) {
                alert("Nenhuma peça foi informada.")
                return false;
            }

            var prod = LstProducao.GetDescProd(idProdPedProducao);

            if (prod.error != null) {
                alert(prod.error.description);
                return false;
            }

            if (!confirm("Tem certeza que deseja" + (pecaParadaProducao ? " retornar esta peça para " : " parar esta peça na ") + "produção?\n" + prod.value))
                return false;

            openWindow(200, 405, "../../Utils/SetMotivoPararPecaProducao.aspx?popup=true&idProdPedProducao=" + idProdPedProducao, null, true, false);

            return false;
        }

        function openLogEstornoCarregamento(idProdPedProducao) {
            openWindow(600, 800, '../../Utils/ShowEstornoCarregamento.aspx?idProdPedProducao=' + idProdPedProducao);
        }

        function exibirProdsComposicao(botao, idProdPedProducao) {

            var linha = document.getElementById("ppp_" + idProdPedProducao);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";
        }

        var voltarPecaClicado = false;

        function voltarPeca(idProdPedProducao) {
            if (!voltarPecaClicado)
                voltarPecaClicado = true;
            else
                return false;

            if (!confirm('Confirma remoção desta peça desta situação?')) {
                voltarPecaClicado = false;
                return false;
            }

            var retornoVoltarPeca = LstProducao.VoltarPeca(idProdPedProducao);

            if (retornoVoltarPeca.error != null) {
                alert(retornoVoltarPeca.error.description);
                voltarPecaClicado = false;
                return false;
            }

            if (retornoVoltarPeca.value.split('|')[0] == "Erro") {
                alert(retornoVoltarPeca.value.split('|')[1]);
                voltarPecaClicado = false;
                return false;
            }

            cOnClick("imgPesq", null);
        }

        function limparFiltros() {
            document.location = 'LstProducaoSimplificado.aspx';
            return false;
        }

    </script>

    <style>
        .tbPesquisa {
            vertical-align: middle;
        }

            .tbPesquisa td {
                display: table-cell;
                vertical-align: middle;
                white-space: nowrap;
                width: auto;
                margin: 0;
                padding: 0;
            }

        .tituloCampos {
            text-align: left;
        }
    </style>


    <asp:HiddenField ID="hdfRefresh" runat="server" />

    <table>
        <tr runat="server" id="trFiltros" align="center">
            <td>
                <table>
                    <tr align="center">
                        <td>
                            <asp:Label ID="lblNumLiberarPedido" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="45px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="6" QueryString="idLiberarPedido"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgNumLiberarPedido" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblNumPedidoImportado" runat="server" Text="Ped. importado" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedidoImportado" runat="server" Width="45px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="6" QueryString="idPedidoImportado"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgNumPedidoImportado" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" QueryString="idLoja">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Vendedor"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True" QueryString="idFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Tipo Pedido"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpTipoPedido" runat="server" QueryString="tipoPedido" OnLoad="drpTipoPedido_Load">
                                <asp:ListItem Value="1">Venda</asp:ListItem>
                                <asp:ListItem Value="2">Produção</asp:ListItem>
                                <asp:ListItem Value="3">Mão-de-obra</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Exibir peças" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdExibirPecas" runat="server" CheckAll="False" Title="Tipo de peça"
                                Width="180px" QueryString="pecasProdCanc">
                                <asp:ListItem Value="0" Selected="True">Em produção</asp:ListItem>
                                <asp:ListItem Value="1">Canceladas (mão-de-obra)</asp:ListItem>
                                <asp:ListItem Value="2">Canceladas (venda)</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr align="center">
                        <td>
                            <asp:Label ID="Label42" runat="server" ForeColor="#0066FF" Text="Período (Conf. Ped.)"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataIniConfPed" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" QueryString="dataIniConfPed" />
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataFimConfPed" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" QueryString="dataFimConfPed" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton20" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Fast Delivery"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlFastDelivery" runat="server" QueryString="fastDelivery">
                                <asp:ListItem Selected="True" Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Com Fast Delivery</asp:ListItem>
                                <asp:ListItem Value="2">Sem Fast Delivery</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label30" runat="server" Text="Agrupar impressão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem Text="" Value="0"></asp:ListItem>
                                <asp:ListItem Value="1">Cliente</asp:ListItem>
                                <asp:ListItem Value="2">Pedido</asp:ListItem>
                                <asp:ListItem Value="3">Número de peças</asp:ListItem>
                                <asp:ListItem Value="4">Prev. entrega</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <a href="javascript:limparFiltros();" class="button">
                                <img border="0" src="../../Images/ExcluirGrid.gif" alt="" />Limpar filtros</a>
                            <input type="hidden" runat="server" id="hdfPageIndex" querystring="pageIndex" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr align="center">
                        <td>
                            <asp:CheckBox ID="chkAguardEntrEstoque" runat="server" Text="Peças aguardando entrada no estoque" QueryString="aguardEntrEstoque" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton17" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr align="center">
            <td>

                <asp:GridView GridLines="None" ID="grdPecas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdProdPedProducao" DataSourceID="odsPecas" EmptyDataText="Nenhuma peça encontrada."
                    OnDataBound="grdPecas_DataBound" OnLoad="grdPecas_Load" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnPageIndexChanging="grdPecas_PageIndexChanging"
                    AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/mais.gif" ToolTip="Exibir Produtos da Composição"
                                    Visible='<%# Eval("IsProdutoLaminadoComposicao") %>' OnClientClick='<%# "exibirProdsComposicao(this, " + Eval("IdProdPedProducao") + "); return false"%>' />
                            </ItemTemplate>
                            <EditItemTemplate></EditItemTemplate>
                            <FooterTemplate></FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/arrow_undo.gif" ToolTip="Remover peça desta situação" Visible='<%# Eval("RemoverSituacaoVisible") %>'
                                    OnClientClick='<%# "voltarPeca(" + Eval("IdProdPedProducao") + "); return false;"%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# (bool)Eval("ExibirRelatorioPedido") && Request["Producao"] != "1" %>'>
                                    <a href="#" onclick="openRptPed('<%# Eval("IdPedido") %>', false, 0, true); return false">
                                        <img border="0" src="../../Images/Relatorio.gif" /></a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchPedidoProducao" runat="server">
                                    <a href="#" onclick="openRptPed('<%# Eval("IdPedido") %>', false, 0, false); return false">
                                        <img border="0" src="../../Images/script_go.gif" /></a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../CadFotos.aspx?id=<%# Eval("IdPedido") %>&amp;tipo=pedido&#039;); return false;'>
                                    <img border="0px" src="../../Images/Clipe.gif" /></a></asp:PlaceHolder>
                                <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemPecaUrl") != null ? Eval("ImagemPecaUrl").ToString().Replace("../", "~/") : null %>'
                                    Visible='<%# !(bool)Eval("TemSvgAssociado") %>' />
                                <uc5:ctrlImageCadProject ID="ctrlImageCadProject" runat="server" IdProdPedEsp='<%# Glass.Conversoes.StrParaIntNullable(Eval("IdProdPed").ToString()).GetValueOrDefault(0) %>'
                                    Visible='<%# Eval("TemSvgAssociado") %>' />
                                <asp:ImageButton ID="imgPararPecaProducao" runat="server" ImageUrl='<%# (bool)Eval("PecaParadaProducao") ? "~/Images/stop_red.png" : "~/Images/stop_blue.png" %>'
                                    OnClientClick='<%# "openSetMotivPararPecaProducao(" + Eval("IdProdPedProducao") + ", " + Eval("PecaParadaProducao").ToString().ToLower() + "); return false" %>'
                                    Visible='<%# Eval("ExibirPararPecaProducao") %>' Width="16" Height="16" ToolTip='<%# (bool)Eval("PecaParadaProducao") ? "Retornar peça para produção?" : "Parar peça na produção?" %>' />
                                <uc4:ctrllogpopup ID="ctrlLogPopup1" runat="server" Tabela="ProdPedProducao" IdRegistro='<%# Eval("idProdPedProducao") %>' />
                                <asp:HiddenField ID="hdfIdSetor" runat="server" Value='<%# Eval("IdSetor") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Eval("Situacao") %>' />
                                <asp:HiddenField ID="hdfPedidoCancelado" runat="server" Value='<%# Eval("PedidoCancelado") %>' />
                                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinha") %>' />
                                <asp:ImageButton runat="server" ID="imgPopup" ImageUrl="~/Images/Nota.gif" Visible='<%# Eval("PecaReposta") %>'
                                    OnClientClick='<%# Eval("IdProdPedProducao", "abrirDetalhesReposicao({0}); return false") %>'
                                    ToolTip="Detalhes Reposição Peça" />
                                <asp:ImageButton ID="imgLogEstornoCarregamento" runat="server" OnClientClick='<%# "openLogEstornoCarregamento(" + Eval("IdProdPedProducao") + "); return false" %>'
                                    ImageUrl="~/Images/log_delete.jpg" ToolTip="Exibir log de estorno de carregamento" Visible='<%# Eval("EstornoCarregamentoVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pedido">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdPedidoExibir") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdPedidoExibir") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="False"></asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Ped.">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("SiglaTipoPedido") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("SiglaTipoPedido") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pedido Cli.">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("CodCliente") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("IdNomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("NomeCliente") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrProdLargAlt") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                <asp:Label ID="Label22" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                                <asp:Label ID="Label23" runat="server" Font-Bold="True" Text='<%# Eval("LarguraAltura") %>'></asp:Label>
                                <br />
                                <asp:Label ID="Label24" runat="server" Font-Size="90%" Text='<%# Eval("DescrTipoPerdaLista") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Apl.">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Proc.">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr><asp:HiddenField ID="hdfPpp" runat="server" Value='<%# Eval("IdProdPedProducao") %>' />
                                <tr id="ppp_<%# Eval("IdProdPedProducao") %>" style="display: none;">
                                    <td colspan="37" align="center">
                                        <br />
                                        <uc5:ctrlLstProdProducao runat="server" ID="ctrlProdComposicao" IdProdPedProducao='<%# Eval("IdProdPedProducao") %>' />
                                        <br />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>

            </td>
        </tr>
         <tr>
            <td align="center">
                <asp:DetailsView ID="dtvContagemPecas" runat="server" DataSourceID="odsContagemPecas"
                    AutoGenerateRows="False" GridLines="None">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label13" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Blue"
                                                Text="Peças Prontas:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasProntas" runat="server" Font-Size="Medium" Text='<%# Eval("Prontas") %>'></asp:Label>
                                            <asp:Label ID="Label37" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMProntas"), Eval("TotMProntasCalc")) %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red"
                                                Text="Peças Pendentes:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasPendentes" runat="server" Font-Size="Medium" Text='<%# Eval("Pendentes") %>'></asp:Label>
                                            <asp:Label ID="Label38" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMPendentes"), Eval("TotMPendentesCalc")) %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="Label16" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="#009933"
                                                Text="Peças Entregues:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasEntregues" runat="server" Font-Size="Medium" Text='<%# Eval("Entregues") %>'></asp:Label>
                                            <asp:Label ID="Label39" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMEntregues"), Eval("TotMEntreguesCalc")) %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Gray"
                                                Text="Peças Perdidas:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasPerdidas" runat="server" Font-Size="Medium" Text='<%# Eval("Perdidas") %>'></asp:Label>
                                            <asp:Label ID="Label40" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMPerdidas"), Eval("TotMPerdidasCalc")) %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Black"
                                                Text="Peças Canceladas:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasCanceladas" runat="server" Font-Size="Medium" Text='<%# Eval("Canceladas") %>'></asp:Label>
                                            <asp:Label ID="Label41" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMCanceladas"), Eval("TotMCanceladasCalc")) %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContagemPecas" runat="server"
                    SelectMethod="GetContagemPecasSimplificado" TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter QueryStringField="idLiberarPedido" Name="idLiberarPedido" />
                        <asp:QueryStringParameter QueryStringField="idPedidoImportado" Name="idPedidoImportado" />
                        <asp:QueryStringParameter QueryStringField="dataIniConfPed" Name="dataIniConfPed" />
                        <asp:QueryStringParameter QueryStringField="dataFimConfPed" Name="dataFimConfPed" />
                        <asp:QueryStringParameter QueryStringField="pecasProdCanc" Name="pecasProdCanc" />
                        <asp:QueryStringParameter QueryStringField="idFunc" Name="idFunc" />
                        <asp:QueryStringParameter QueryStringField="tipoPedido" Name="tipoPedido" />
                        <asp:QueryStringParameter QueryStringField="fastDelivery" Name="fastDelivery" />
                        <asp:QueryStringParameter QueryStringField="aguardEntrEstoque" Name="aguardEntrEstoque" />
                        <asp:QueryStringParameter QueryStringField="idLoja" Name="idLoja" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr align="center">
            <td>
                <table>
                    <tr>
                        <td align="right">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false,false,false); return false;"> <img border="0" src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true,false,false); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                        </td>
                    </tr>
                    <tr id="trImpresaoSetorFiltrado" runat="server" visible="false">
                        <td align="right">
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openRpt(false,true,false); return false;"> <img border="0" src="../../Images/Printer.png" /> Imprimir (Setor Selecionado)</asp:LinkButton>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="openRpt(true,true,false); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel (Setor Selecionado)</asp:LinkButton>
                        </td>
                    </tr>
                    <tr id="trImpressaoRoteiro" runat="server" visible="false">
                        <td align="right">
                            <asp:LinkButton ID="LinkButton3" runat="server" OnClientClick="openRpt(false,false,true); return false;"> <img border="0" src="../../Images/Printer.png" /> Imprimir (Roteiro)</asp:LinkButton>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton4" runat="server" OnClientClick="openRpt(true,false,true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel (Roteiro)</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPecas" runat="server" SelectMethod="GetListConsultaSimplificado"
                    TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetListConsultaSimplificadoCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:QueryStringParameter QueryStringField="idLiberarPedido" Name="idLiberarPedido" />
                        <asp:QueryStringParameter QueryStringField="idPedidoImportado" Name="idPedidoImportado" />
                        <asp:QueryStringParameter QueryStringField="dataIniConfPed" Name="dataIniConfPed" />
                        <asp:QueryStringParameter QueryStringField="dataFimConfPed" Name="dataFimConfPed" />
                        <asp:QueryStringParameter QueryStringField="pecasProdCanc" Name="pecasProdCanc" />
                        <asp:QueryStringParameter QueryStringField="idFunc" Name="idFunc" />
                        <asp:QueryStringParameter QueryStringField="tipoPedido" Name="tipoPedido" />
                        <asp:QueryStringParameter QueryStringField="fastDelivery" Name="fastDelivery" />
                        <asp:QueryStringParameter QueryStringField="aguardEntrEstoque" Name="aguardEntrEstoque" />
                        <asp:QueryStringParameter QueryStringField="idLoja" Name="idLoja" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsLoja"
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges"
                    Culture="" MaximumRowsParameterName="" SelectMethod="GetAll" SkinID=""
                    StartRowIndexParameterName="" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        $(document.body).on("keydown", this,
            function (event) {
                if (event.keyCode == 116) {
                    FindControl("hdfRefresh", "input").value = "0";
                }
            });

    </script>

</asp:Content>
