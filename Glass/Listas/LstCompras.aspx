<%@ Page Title="Compras" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstCompras.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCompras" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function cancelar(idCompra) {
            openWindow(150, 420, "../Utils/SetMotivoCancCompra.aspx?idCompra=" + idCompra);
        }

        function openRpt(idCompra, exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Compra&idCompra=" + idCompra +
                "&exportarexcel=" + exportarExcel);
            return false;
        }

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openFornec() {
            if (FindControl("txtFornecedor", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelFornec.aspx");

            return false;
        }

        function openRptLista(exportarExcel) {
            var numCompra = FindControl("txtNumCompra", "input").value;
            var numPedido = FindControl("txtNumPedido", "input").value;
            var nfPedido = FindControl("txtNF", "input").value;
            var idFornec = FindControl("txtFornecedor", "input").value;
            var nomeFornec = FindControl("txtNome", "input").value;
            var obs = FindControl("txtObservacao", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var emAtraso = FindControl("chkEmAtraso", "input");
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataFabIni = FindControl("ctrlDataFabIni_txtData", "input").value;
            var dataFabFim = FindControl("ctrlDataFabFim_txtData", "input").value;
            var dataSaidaIni = FindControl("ctrlDataSaidaIni_txtData", "input").value;
            var dataSaidaFim = FindControl("ctrlDataSaidaFim_txtData", "input").value;
            var dataFinIni = FindControl("ctrlDataFinIni_txtData", "input").value;
            var dataFinFim = FindControl("ctrlDataFinFim_txtData", "input").value;
            var dataEntIni = FindControl("ctrlDataEntIni_txtData", "input").value;
            var dataEntFim = FindControl("ctrlDataEntFim_txtData", "input").value;
            var idsGrupos = FindControl("cbdGrupo", "select").itens();
            var idSubgrupo = FindControl("drpSubgrupo", "select").value;
            var codProd = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescrProd", "input").value;
            var centroCustoDivergente = FindControl("chkCentroCustoDivergente", "input") != null ?
                FindControl("chkCentroCustoDivergente", "input").checked : false;
            var agruparPorFornecedor = FindControl("chkAgruparFornecedor", "input") != null ?
                FindControl("chkAgruparFornecedor", "input").checked : false;
            var idLoja = FindControl("drpLoja", "select").value;

            var queryString = "&numCompra=" + numCompra +
                "&numPedido=" + numPedido +
                "&nfPedido=" + nfPedido +
                "&idFornec=" + idFornec +
                "&nomeFornec=" + nomeFornec +
                "&obs=" + obs +
                "&situacao=" + situacao +
                "&emAtraso=" + (emAtraso != null ? emAtraso.checked : "") +
                "&dataIni=" + dataIni +
                "&dataFim=" + dataFim +
                "&dataFabIni=" + dataFabIni +
                "&dataFabFim=" + dataFabFim +
                "&dataSaidaIni=" + dataSaidaIni +
                "&dataSaidaFim=" + dataSaidaFim +
                "&dataFinIni=" + dataFinIni +
                "&dataFinFim=" + dataFinFim +
                "&dataEntIni=" + dataEntIni +
                "&dataEntFim=" + dataEntFim +
                "&idsGrupoProd=" + idsGrupos +
                "&idSubgrupoProd=" + idSubgrupo +
                "&codProd=" + codProd +
                "&descrProd=" + descrProd +
                "&centroCustoDivergente=" + centroCustoDivergente +
                "&agruparPorFornecedor=" + agruparPorFornecedor +
                "&exportarexcel=" + exportarExcel+
            "&idLoja=" + idLoja;

            openWindow(600, 800, '../Relatorios/RelBase.aspx?rel=ListaCompras' + queryString);
            return false;
        }

        function produtoChegou(idCompra) {
            openWindow(600, 800, '../Utils/ProdutoCompraChegou.aspx?idCompra=' + idCompra);
        }

        function gerarNfe(idCompra) {

            if (!confirm('Deseja gerar a nota fiscal para essa compra?'))
                return false;

            redirectUrl('../Cadastros/CadNotaFiscalGerarCompra.aspx?idCompra=' + idCompra);
        }

        function notaGerada(idNf) {
            alert("Nota fiscal gerada com sucesso!");
            redirectUrl("../Cadastros/CadNotaFiscal.aspx?idNf=" + idNf + "&tipo=3");
        }

        function getProduto(codProd) {
            if (codProd.value == "")
                return;

            var retorno = MetodosAjax.GetProd(codProd.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                codProd.value = "";
                FindControl("txtDescrProd", "input").value = "";
                return false;
            }

            FindControl("txtDescrProd", "input").value = retorno[2];
        }

        function exibirCentroCusto(idCompra) {

            var idLoja = LstCompras.ObtemIdLoja(idCompra).value;
            openWindow(365, 700, '../Utils/SelCentroCusto.aspx?idCompra=' + idCompra + "&compra=true" + "&idLoja=" + idLoja);
            return false;
        }

        
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCompra" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label245" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Cotação de Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCotacaoCompra" runat="server" Width="60px" onkeyup="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label17" runat="server" ForeColor="#0066FF" Text="NF/Pedido"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNF" runat="server" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True" OnLoad="drpSituacao_Load">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Ativa</asp:ListItem>
                                <asp:ListItem Value="4">Em andamento</asp:ListItem>
                                <asp:ListItem Value="5">Aguardando Entrega</asp:ListItem>
                                <asp:ListItem Value="2">Finalizada</asp:ListItem>
                                <asp:ListItem Value="3">Cancelada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Observação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtObservacao" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="300"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                             <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                    DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true">
                                 <asp:ListItem Selected="True" Value="0">Todas</asp:ListItem>
                                </asp:DropDownList>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" 
                    SelectMethod="ObtemLojas" 
                    TypeName="Glass.Global.Negocios.ILojaFluxo">
                </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdGrupo" runat="server" CheckAll="False" DataSourceID="odsGrupo"
                                DataTextField="Descricao" DataValueField="IdGrupoProd" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False"
                                Title="Selecione o grupo">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" OnDataBound="drpSubgrupo_DataBound">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label21" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="getProduto(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescrProd" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getProduto(FindControl('txtCodProd', 'input'));"
                                OnClick="imgPesq_Click" Height="16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Período Ent. Fábrica" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFabIni" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFabFim" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Período Saída" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataSaidaIni" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataSaidaFim" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Período Finalização" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFinIni" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFinFim" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Período Entrega Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataEntIni" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataEntFim" runat="server" ExibirHoras="False" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkEmAtraso" runat="server" Text="Compras em atraso" AutoPostBack="true" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:CheckBox ID="chkCentroCustoDivergente" runat="server" Text="Compras com valor do centro custo divergente" AutoPostBack="true" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparFornecedor" runat="server" Text="Agrupar relatório por fornecedor" />
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Nova Compra</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCompra" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsCompra" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdCompra" EnableViewState="false"
                    EmptyDataText="Não há compras cadastradas." OnRowCommand="grdCompra_RowCommand"> 
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadCompra.aspx?idCompra=" + Eval("IdCompra") %>'
                                    Visible='<%# Eval("EditVisible") %>'><img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <a href="#" onclick="return openRpt(<%# Eval("IdCompra") %>, false);">
                                    <img src="../Images/relatorio.gif" border="0" title="Visualizar dados da Compra"></a>
                                <a href="#" onclick="return openRpt(<%# Eval("IdCompra") %>, true);">
                                    <img src="../Images/Excel.gif" border="0" title="Visualizar dados da Compra (Excel)"></a>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                    Visible='<%# Eval("CancelVisible") %>' OnClientClick='<%# "cancelar(" + Eval("IdCompra") + "); return false;" %>'
                                    ToolTip="Cancelar" />
                                <asp:ImageButton ID="imbExcluirPagto" runat="server" CommandName="CancelarPagto"
                                    ImageUrl="~/Images/CancelarPagto.gif" Visible='<%# Eval("CancelPagtoVisible") %>'
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja cancelar o pagamento desta compra?&quot;);"
                                    ToolTip="Cancelar Pagto." CommandArgument='<%# Eval("IdCompra") %>' />
                                <asp:PlaceHolder ID="pchFotos" runat="server" Visible='<%# Eval("FotosVisible") %>'>
                                    <a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdCompra") %>&tipo=compra&#039;); return false;'>
                                        <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                                <asp:ImageButton ID="imgNfe" runat="server" ImageUrl="~/Images/script_go.gif" OnClientClick='<%# "gerarNfe(" + Eval("IdCompra") + "); return false" %>'
                                    ToolTip="Gerar NF de entrada" Visible='<%# Eval("GerarNFeVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCompra" HeaderText="Num" SortExpression="IdCompra" />
                        <asp:BoundField DataField="IdCotacaoCompra" HeaderText="Cotação" SortExpression="IdCotacaoCompra" />
                        <asp:TemplateField HeaderText="Pedido" SortExpression="IdsPedido">
                            <ItemTemplate>
                                <asp:Label ID="Label101" runat="server" Text='<%# Bind("IdsPedido") %>'></asp:Label>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("IdPedidoEspelho") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fornecedor" SortExpression="NomeFornec">
                            <ItemTemplate>
                                <asp:Label ID="Label199" runat="server" Text='<%# Bind("IdNomeFornec") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdNomeFornec") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataFabrica" DataFormatString="{0:d}" HeaderText="Data Ent. Fábrica"
                            SortExpression="DataFabrica" />
                        <asp:BoundField DataField="DescrTipoCompra" HeaderText="Pagto" SortExpression="DescrTipoCompra">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                        <asp:CheckBoxField DataField="Contabil" HeaderText="Contábil" SortExpression="Contabil" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Image ToolTip='<%# "Nota fiscal gerada: " + Eval("NumeroNfe") %>' runat="server"
                                    Visible='<%# Eval("ExibirNfeGerada") %>' ImageUrl="../Images/blocodenotas.png"
                                    ID="imgNfeGerada" Style="cursor: pointer" />
                                <asp:ImageButton ID="imgReabrir" runat="server" CommandArgument='<%# Eval("IdCompra") %>'
                                    CommandName="Reabrir" ImageUrl="~/Images/cadeado.gif" OnClientClick="if (!confirm(&quot;Deseja reabrir essa compra?&quot;)) return false;"
                                    Visible='<%# Eval("ReabrirVisible") %>' />
                                <asp:Image ID="imgEstoque" runat="server" ImageUrl="~/Images/basket_add.gif" ToolTip="Estoque Creditado"
                                    Visible='<%# Eval("EstoqueBaixado") %>' />
                                <asp:LinkButton ID="lnkProdutoChegou" runat="server" OnClientClick='<%# "produtoChegou(" + Eval("IdCompra") + "); return false" %>'
                                    Visible='<%# Eval("ProdutoChegouVisible") %>'>Produto chegou</asp:LinkButton>
                                <asp:LinkButton ID="lnkFinalizar" runat="server" CommandArgument='<%# Eval("IdCompra") %>'
                                    CommandName="Finalizar" OnClientClick="if (!confirm(&quot;Deseja finalizar essa compra?&quot;)) return false;"
                                    Visible='<%# Eval("FinalizarAguardandoEntregaVisible") %>'>Finalizar</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                 <asp:ImageButton ID="imbCentroCusto" runat="server" ImageUrl='<%# "~/Images/" + ((bool)Eval("CentroCustoCompleto") ? "cash_blue.png" : "cash_red.png") %>' Visible='<%# Eval("ExibirCentroCusto") %>' 
                                    ToolTip="Exibir Centro de Custos" OnClientClick='<%# "exibirCentroCusto(" + Eval("IdCompra") + "); return false" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Compra" IdRegistro='<%# Eval("IdCompra") %>'  EnableViewState="false"/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCompra" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.CompraDAO"
                    OnDeleted="odsCompra_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCotacaoCompra" Name="idCotacaoCompra" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNF" Name="nf" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtFornecedor" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtObservacao" Name="obs" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkEmAtraso" Name="emAtraso" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFabIni" Name="dataFabIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFabFim" Name="dataFabFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSaidaIni" Name="dataSaidaIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSaidaFim" Name="dataSaidaFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFinIni" Name="dataFinIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFinFim" Name="dataFinFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataEntIni" Name="dataEntIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataEntFim" Name="dataEntFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idsGrupoProd" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupoProd" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDescrProd" Name="descrProd" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkCentroCustoDivergente" Name="centroCustoDivergente" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpLoja" PropertyName="SelectedValue" Name="idLoja"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="cbdGrupo" Name="idGrupos" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRptLista(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRptLista(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
