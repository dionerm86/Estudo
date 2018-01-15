<%@ Page Title="Ordens de Carga" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstOrdemCarga.aspx.cs" Inherits="Glass.UI.Web.Listas.lstOrdemCarga" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">

        function getCli(idCli) {
            if (idCli.value == "") {
                openWindow(570, 760, '../Utils/SelCliente.aspx');
                return false;
            }

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openRptInd(idOrdemCarga) {
            if (idOrdemCarga == null || idOrdemCarga == "") {
                alert("Informe a OC.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=OrdemCarga&idOrdemCarga=" + idOrdemCarga);
            return false;
        }

        function openRpt(exportarExcel) {

            var codOC = FindControl("txtCodOC", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var tipoOC = FindControl("drpTipoOC", "select").itens();
            var situacaoOC = FindControl("drpSituacaoOC", "select").itens();
            var idRota = FindControl("drpRota", "select").value;
            var dtEntPedIni = FindControl("txtDataIni_txtData", "input").value;
            var dtEntPedFin = FindControl("txtDataFin_txtData", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idCarregamento = FindControl("txtIdCarregamento", "input").value;
            var idPedido = FindControl("txtIdPedido", "input").value;
            var codRotasExternas = FindControl("cblRotaExterna", "select").itensText();
            var idClienteExterno = FindControl("txtNumCliExterno", "input").value;
            var nomeClienteExterno = FindControl("txtNomeClienteExterno", "input").value;

            var queryString = "&idOC=" + codOC;
            queryString += "&idCli=" + idCliente;
            queryString += "&nomeCli=" + nomeCliente;
            queryString += "&tipo=" + tipoOC;
            queryString += "&situacao=" + situacaoOC;
            queryString += "&idRota=" + idRota;
            queryString += "&dtEntPedIni=" + dtEntPedIni;
            queryString += "&dtEntPedFin=" + dtEntPedFin;
            queryString += "&idLoja=" + idLoja;
            queryString += "&idCarregamento=" + idCarregamento;
            queryString += "&idPedido=" + idPedido;
            queryString += "&codRotasExternas=" + codRotasExternas;
            queryString += "&idClienteExterno=" + idClienteExterno;
            queryString += "&nomeClienteExterno=" + nomeClienteExterno;

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=ListaOrdemCarga" + queryString);
            return false;
        }

        function exibirPedidos(botao, idOC) {

            var linha = document.getElementById("oc_" + idOC);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Pedidos";
        }

        function adicionarPedido(idOC) {

            var podeAddPedido = lstOrdemCarga.PodeAdicionarPedidoOC(idOC);

            if (podeAddPedido.error != null) {
                alert(podeAddPedido.error.description);
                return false;
            }

            var retorno = lstOrdemCarga.BuscarDadosOC(idOC);

            if (retorno.error != null) {
                alert("Falha ao buscar dados OC." + retorno.error.description);
                return false;
            }

            var dados = retorno.value.split(";");

            var queryString = "popup=true";
            queryString += "&idCliente=" + dados[0];
            queryString += "&idRota=" + dados[1];
            queryString += "&dtEntPedidoIni=" + dados[2];
            queryString += "&dtEntPedidoFin=" + dados[3];
            queryString += "&idLoja=" + dados[4];
            queryString += "&tipoOC=" + dados[5];
            queryString += "&situacao=" + dados[6];
            queryString += "&idOC=" + idOC;

            openWindow(550, 1000, "../Cadastros/CadItensOrdemCarga.aspx?" + queryString);

            return false;
        }

        function exibirObs(botao, idPedido, msg) {

            var boxObs = FindControl("boxObs", "div");
            var lblObs = FindControl("lblObs", "span");

            lblObs.innerHTML = msg;

            TagToTip('boxObs', FADEIN, 300, COPYCONTENT, false, TITLE, 'Observação: ' + idPedido, CLOSEBTN, true,
                CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('boxObs'), -41 - getTableHeight('boxObs')]);
        }

        function atualizar() {
            cOnClick("imgPesq");
        }

    </script>
    
    <style>
    
    .destaque span
    {
        font-size:1.2em;
        margin: 0px 8px;
        display: inline-block;
        font-weight: bold;
    }
    
    </style>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Carregamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdCarregamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód. OC" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodOC" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Selecione uma Rota</asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período Entrega Ped.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataFin" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" MostrarTodas="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Tipo da OC"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpTipoOC" runat="server">
                                <asp:ListItem Value="1">Venda</asp:ListItem>
                                <asp:ListItem Value="2">Transferência</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpSituacaoOC" runat="server">
                                <asp:ListItem Value="1">Finalizado</asp:ListItem>
                                <asp:ListItem Value="2">Pedente de Carregamento</asp:ListItem>
                                <asp:ListItem Value="3">Carregado</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table ID="tbClienteExterno" runat="server">
                    <tr>
                         <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente Externo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCliExterno" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeClienteExterno" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Rota Externa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                             <sync:CheckBoxListDropDown ID="cblRotaExterna" runat="server" Width="200px" CheckAll="False"
                                Title="Selecione a rota" DataSourceID="odsRotasExternas" DataTextField="Descr" DataValueField="Id"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRotasExternas" runat="server" SelectMethod="GetRotasExternas"
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click"/>
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click"> Gerar Ordem de Carga</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdOrdemCarga" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdOrdemCarga" DataSourceID="odsOrdemCarga"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhuma OC encontrada para o filtro informado." OnRowDataBound="grdOrdemCarga_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir OC" OnClientClick="if(!confirm('Deseja realmente excluir esta OC?')) return false;" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirPedidos(this, " + Eval("IdOrdemCarga") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir Pedidos" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. OC">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("IdOrdemCarga") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdNomeCliente") %>'></asp:Label>
                                <asp:HiddenField runat="server" ID="hdfIdCliente" Value='<%# Bind("IdCliente") %>' />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rota">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodRota") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Peso">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Peso") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle CssClass="destaque" HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Peso Pendente">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("PesoPendenteProducao") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total M²">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("TotalM2") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Itens">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("QtdePecasVidro") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total">
                            <ItemTemplate>
                                <asp:Label ID="lblValorTotalPedidos" runat="server" Text='<%# ((decimal?)Eval("ValorTotalPedidos")).GetValueOrDefault().ToString("C") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total M² Pendente">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotalM2PendenteProducao") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Itens Pendentes">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("QtdePecaPendenteProducao") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Volumes">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("QtdeVolumes") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo OC">
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("TipoOrdemCargaStr") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("SituacaoStr") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbRelInd" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    ToolTip="Visualizar OC" OnClientClick='<%# "openRptInd(" + Eval("IdOrdemCarga") + "); return false;" %>' />
                                <asp:HiddenField ID="hdfTipoOC" runat="server" Value='<%# Bind("TipoOrdemCarga") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc4:ctrlLogPopup ID="ctrlLogPopup1"
                                        runat="server" Tabela="OrdemCarga" IdRegistro='<%# Eval("IdOrdemCarga") %>' />
                                <uc3:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdOrdemCarga") %>'
                                    Tabela="PedidoOC" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr><asp:HiddenField ID="hdfIdOC" runat="server" Value='<%# Eval("IdOrdemCarga") %>' />
                                <tr id="oc_<%# Eval("IdOrdemCarga") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <td colspan="18">
                                        <br />
                                        &nbsp;
                                        <asp:LinkButton ID="lnkAdicionarPedido" runat="server" OnClientClick='<%# "return adicionarPedido(" + Eval("IdOrdemCarga") + ");" %>'>Adicionar Pedido</asp:LinkButton>
                                        <br />
                                        <br />
                                        <asp:GridView ID="grdPedidos" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                                            DataKeyNames="IdPedido" DataSource='<%# Eval("Pedidos") %>' GridLines="None" OnRowDataBound="grdPedidos_RowDataBound"
                                            Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                                        <asp:ImageButton ID="imgExcluir" runat="server" CommandName="RemoverPedido" ImageUrl="~/Images/ExcluirGrid.gif"
                                                            OnPreRender="imgExcluir_PreRender" OnCommand="imgExcluir_Command" ToolTip="Remover Pedido"
                                                            OnClientClick="if(!confirm('Deseja realmente remover este pedido da OC?')) return false;" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Pedido">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" Text='<%# 
                                                            (Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("Importado")) ?
                                                                Eval("IdPedido") + " ("+Eval("IdPedidoExterno")+")" : Eval("IdPedidoCodCliente")).ToString()  %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cliente Externo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("Importado")) ?
                                            Eval("IdClienteExterno") + " - " + Eval("ClienteExterno") : "" %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Rota Externa">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%# Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("Importado")) ?
                                            Eval("RotaExterna") : "" %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField HeaderText="Tipo de Pedido" DataField="DescricaoTipoPedido" />
                                                <asp:BoundField HeaderText="Dt. Entrega" DataField="DataEntregaString" />
                                                <asp:BoundField HeaderText="Peso" DataField="PesoOc" />
                                                <asp:BoundField HeaderText="Volumes Pendentes" DataField="VolumesPendentes" />
                                                <asp:TemplateField HeaderText="Obs.">
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("ObsLiberacao") != null && Eval("ObsLiberacao") != "" %>'>
                                                            <a href="#" onclick='exibirObs(this, <%# Eval("IdPedido") %>,&#039;<%# Eval("ObsLiberacao") %>&#039;); return false;'>
                                                                <img alt="" border="0" src="../Images/blocodenotas.png" title="Ver Observação." /></a>
                                                        </asp:PlaceHolder>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:GridView>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOrdemCarga" runat="server" DataObjectTypeName="Glass.Data.Model.OrdemCarga"
                    DeleteMethod="Delete" SelectMethod="GetListWithExpression" TypeName="WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetListWithExpressionCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" OnDeleted="odsOrdemCarga_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodOC" Name="idOC" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" PropertyName="SelectedValue" Name="idLoja"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpRota" Name="idRota" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dtEntPedidoIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFin" Name="dtEntPedidoFin" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacaoOC" Name="situacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoOC" Name="tipo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtIdCarregamento" Name="idCarregamento" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCliExterno" Name="idCliExterno" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeClienteExterno" Name="nomeCliExterno" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="cblRotaExterna" Name="idsRotasExternas" PropertyName="SelectedItem"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
    <div id="boxObs" style="display: none; width: 350px;">
        <asp:Label ID="lblObs" runat="server" Text="Label"></asp:Label>
    </div>
</asp:Content>
