<%@ Page Title="Volumes" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadVolume.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadVolume" EnableViewState="false" EnableViewStateMac="false" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Volumes/Templates/LstVolumes.Filtro.html",
            "~/Vue/Volumes/Templates/LstVolumes.Itens.html")
    %>
    <div id="app">
        <volumes-filtros :filtro.sync="filtro" :configuracoes="configuracoes"></volumes-filtros>
        <section>
            <lista-paginada ref="lista" :funcao-recuperar-itens="obterLista" :filtro="filtro" :ordenacao="ordenacao" mensagem-lista-vazia="Nenhum pedido encontrado para o filtro informado."
                v-on:atualizou-itens="atualizouItens">
                <template slot="cabecalho">
                    <th></th>
                    <th>
                        <a href="#" @click.prevent="ordenar('pedido')">Pedido</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('cliente')">Cliente</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('loja')">Loja</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('funcionario')">Funcionário</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('dataEntrega')">Entrega</a>
                    </th>
                    <th>
                        <a href="#" @click.prevent="ordenar('rota')">Rota</a>
                    </th>
                    <th>
                        Total m²
                    </th>
                    <th>
                        Peso total
                    </th>
                    <th>
                        Itens pedido
                    </th>
                    <th>
                        Itens volume
                    </th>
                    <th>
                        Itens pendentes
                    </th>
                    <th>
                        Situação
                    </th>
                    <th></th>
                </template>
                <template slot="item" slot-scope="{ item, index }">
                    <td style="white-space: nowrap">
                        <button v-on:click.prevent="alternarExibicaoVolumes(index)" v-if="!exibindoVolumes(index)">
                            <img src="../../Images/mais.gif" title="Exibir volumes" />
                        </button>
                        <button v-on:click.prevent="alternarExibicaoVolumes(index)" v-if="exibindoVolumes(index)">
                            <img src="../../Images/menos.gif" title="Esconder volumes" />
                        </button>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.id }} 
                        <template v-if="configuracoes.controlarPedidosImportados && item.importado && item.pedidoExterno">
                            ({{ item.pedidoExterno.id }})
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.cliente.id }} - {{ item.cliente.nome }} 
                        <template v-if="configuracoes.controlarPedidosImportados && item.importado && item.pedidoExterno && item.pedidoExterno.cliente">
                            ({{ item.pedidoExterno.cliente.id }} - {{ item.pedidoExterno.cliente.nome }})
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.loja }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.funcionario }}</td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.dataEntrega | data }} 
                        <template v-if="item.dataEntregaOriginal">
                            ({{ item.dataEntregaOriginal | data }})
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">
                        {{ item.rota }}
                        <template v-if="configuracoes.controlarPedidosImportados && item.importado && item.pedidoExterno">
                            ({{ item.pedidoExterno.rota }})
                        </template>
                    </td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.metroQuadrado | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.peso | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.quantidadePecasPedido | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.quantidadePecas | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.quantidadePecasPendentes | decimal }}</td>
                    <td :style="{ color: item.corLinha }">{{ item.dadosVolume.situacao }}</td>
                    <td style="white-space: nowrap">
                        <button @click.prevent="abrirTelaGerarVolume(item)" title="Gerar volume" v-if="item.permissoes.gerarVolume">
                            Gerar volume
                        </button>
                        <button @click.prevent="abrirTelaVisualizarVolume(item)" title="Visualizar volume" v-if="item.permissoes.exibirRelatorioVolume">
                            <img src="../Images/Relatorio.gif">
                        </button>
                    </td>
                </template>
                <template slot="novaLinhaItem" slot-scope="{ item, index, classe }" v-if="exibindoVolumes(index)">
                    <tr v-bind:class="classe" style="border-top: none">
                        <td></td>
                        <td v-bind:colspan="numeroColunasLista() - 1">
                            <volumes-itens v-bind:filtro="{ idPedido: item.id, item }" v-bind:configuracoes="configuracoes"></volumes-itens>
                        </td>
                    </tr>
                </template>
            </lista-paginada>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Volumes/Componentes/LstVolumes.Filtro.js" />
            <asp:ScriptReference Path="~/Vue/Volumes/Componentes/LstVolumes.Itens.js" />
            <asp:ScriptReference Path="~/Vue/Volumes/Componentes/LstVolumes.js" />
        </Scripts>
    </asp:ScriptManager>











    <script type="text/javascript">

        var idTimeout;
        var carregaPagina;

        function getCli(idCli) {
            if (idCli.value == "") {
                openWindow(570, 760, '../Utils/SelCliente.aspx');
                return false;
            }

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

        function exibirProdutos(botao, idPedido) {

            var linha = document.getElementById("produtos_" + idPedido);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " itens";
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

        function gerarVolume(idPedido, idVolume) {
            if (idPedido == null || idPedido == 0) {
                alert("Falha ao gerar volume. Pedido não encontrado.");
                return false;
            }

            var queryString = "popup=true";
            queryString += "&idPedido=" + idPedido;
            queryString += idVolume != null && idVolume > 0 ? "&idVolume=" + idVolume : "";

            var chkCarregaPagina = FindControl("chkCarregaPagina", "input")
            if (chkCarregaPagina.checked) {
                carregaPagina = true;
                chkCarregaPagina.checked = false;
                atualizarAutomaticamente();
            }

            openWindow(600, 800, "../Cadastros/CadItensVolume.aspx?" + queryString, null, true);

            return false;
        }

        function fecharGerarVolume() {
            if (carregaPagina) {
                FindControl("chkCarregaPagina", "input").checked = true;
                atualizarAutomaticamente();
            }
            cOnClick("imgPesq");
        }

        function openRptEtiqueta(idVolume) {
            if (idVolume == null || idVolume == "") {
                alert("Informe o volume.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelEtiquetaVolume.aspx?rel=EtqVolume&idVolume=" + idVolume, null, true, true);
            return false;
        }


        function openRptInd(idPedido) {
            if (idPedido == null || idPedido == "") {
                alert("Informe o pedido.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=Volume&idPedido=" + idPedido, null, true, true);
            return false;
        }

        function atualizarAutomaticamente() {
            var atualizar = FindControl("chkCarregaPagina", "input").checked;

            if (atualizar)
                idTimeout = setTimeout(function() { cOnClick("imgPesq"); }, 1000 * 60);
            else
                clearTimeout(idTimeout);
        }


    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Volume" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumVolume" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Situação Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpSituacaoPedido" runat="server">
                                <asp:ListItem Value="1" Selected="True">Sem volume</asp:ListItem>
                                <asp:ListItem Value="2" Selected="True">Pendente</asp:ListItem>
                                <asp:ListItem Value="3">Finalizado</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Situação Volume" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpSituacaoVolume" runat="server">
                                <asp:ListItem Value="1">Aberto</asp:ListItem>
                                <asp:ListItem Value="2">Fechado</asp:ListItem>
                                <asp:ListItem Value="3">Carregado</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtRota" runat="server" MaxLength="20" Width="80px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Período Entrega Ped.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataEntIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataEntFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblPeriodoLiberacaoPedido" runat="server" Text="Período Liberação Ped.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataLibIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataLibFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPeriodoLiberacaoPedido" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
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
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoPedido0" runat="server" ForeColor="#0066FF" Text="Tipo Entrega"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoEntrega" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server"
                                SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkCarregaPagina" runat="server" Checked="True" Text="Carregar itens automaticamente"
                                onclick="atualizarAutomaticamente();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:GridView GridLines="None" ID="grdPedidos" runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdPedido" DataSourceID="odsPedido"
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Nenhum pedido encontrado para o filtro informado."
                                OnRowDataBound="grdPedidos_RowDataBound">
                                <PagerSettings PageButtonCount="20" />
                                <RowStyle HorizontalAlign="Left" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdutos(this, " + Eval("IdPedido") + "); return false" %>'
                                                Width="10px" ToolTip="Exibir itens" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Num. Pedido" SortExpression="IdPedido">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("idPedido") + 
                                            (Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("Importado")) ? " (" + Eval("IdPedidoExterno") + ")" : "") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCliente">
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("NomeCliente") + 
                                            (Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("Importado")) ? " (" + Eval("IdClienteExterno") + " - " + Eval("ClienteExterno") + ")" : "") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                                    <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                                    <asp:BoundField DataField="DataEntregaExibicao" HeaderText="Entrega" SortExpression="DataEntrega" />
                                    <asp:TemplateField HeaderText="Rota" SortExpression="codRota">
                                        <ItemTemplate>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("codRota") + 
                                            (Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("Importado")) ? " (" + Eval("RotaExterna") + ")" : "") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TotMVolume" HeaderText="Total m²" />
                                    <asp:BoundField DataField="PesoVolume" HeaderText="Peso total" />
                                    <asp:BoundField DataField="QtdePecas" HeaderText="Itens Pedido" />
                                    <asp:BoundField DataField="QtdePecasVolume" HeaderText="Itens Volume" />
                                    <asp:BoundField DataField="QtdePecasPendenteVolume" HeaderText="Itens Pendentes" />
                                    <asp:BoundField DataField="SituacaoVolume" HeaderText="Situação" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkGerarVolume" runat="server" OnClientClick='<%# "return gerarVolume(" + Eval("IdPedido") + ");" %>'
                                                Visible='<%# Eval("GerarVolumeVisible") %>'>Gerar Volume</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbRelInd" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                                ToolTip="Visualizar Volume" OnClientClick='<%# "openRptInd(" + Eval("idPedido") + "); return false;" %>'
                                                Visible='<%# Eval("RelatorioVolumeVisible") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            </td> </tr>
                                <tr id="produtos_<%# Eval("IdPedido") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                    <td colspan="15">
                                        <asp:HiddenField runat="server" ID="hdfIdPedido" Value='<%# Eval("IdPedido") %>' />
                                        <asp:GridView ID="grdVolume" runat="server" AutoGenerateColumns="False" DataKeyNames="IdVolume"
                                            DataSourceID="odsVolume" GridLines="None" Width="100%" class="pos" ShowFooter="true"
                                            CellPadding="0" CellSpacing="0" OnRowDataBound="grdVolume_RowDataBound" EmptyDataText="Nenhum volume encontrado.">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:ImageButton ID="imbEditar" runat="server" ImageUrl="~/Images/Edit.gif" ToolTip="Editar Volume"
                                                                        Visible='<%# Eval("EditarVisible") %>' OnClientClick='<%# "gerarVolume(" + Eval("idPedido") + "," + Eval("idVolume") + "); return false;" %>' />
                                                                </td>
                                                                <td>
                                                                    <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                                        ToolTip="Excluir Volume" OnClientClick="if(!confirm('Deseja realmente excluir este volume?')) return false;" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <div style="margin-top: 10px">
                                                            Total
                                                        </div>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="IdVolume" HeaderText="Num. Volume" SortExpression="IdVolume" />
                                                <asp:TemplateField HeaderText="Qtde. Itens">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("QtdeItens") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Peso Total">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("PesoTotal") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total M²">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="SituacaoStr" HeaderText="Situação" SortExpression="situacao" />
                                                <asp:BoundField DataField="NomeFuncFinalizacao" HeaderText="Func. Finalização" SortExpression="NomeFuncFinalizacao" />
                                                <asp:BoundField DataField="DataFechamento" HeaderText="Data Finalização" SortExpression="DataFechamento" />
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imbImpEtiqueta" runat="server" ImageUrl="~/Images/printer.png"
                                                            ToolTip="Imprimir Etiqueta" Visible="true"
                                                            OnClientClick='<%# "return openRptEtiqueta("+ Eval("IdVolume") + ");" %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle CssClass="pgr"></PagerStyle>
                                            <EditRowStyle CssClass="edit"></EditRowStyle>
                                            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                            <FooterStyle Font-Bold="true" />
                                        </asp:GridView>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVolume" runat="server" DataObjectTypeName="Glass.Data.Model.Volume"
                                            DeleteMethod="Delete" SelectMethod="GetList" TypeName="WebGlass.Business.OrdemCarga.Fluxo.VolumeFluxo"
                                            EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetListCount"
                                            SortParameterName="sortExpression" StartRowIndexParameterName="startRow" OnDeleted="odsVolume_Deleted">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="txtNumVolume" Name="idVolume" PropertyName="Text"
                                                    Type="UInt32" />
                                                <asp:ControlParameter ControlID="hdfIdPedido" Name="idPedido" PropertyName="Value"
                                                    Type="UInt32" />
                                                <asp:ControlParameter ControlID="drpSituacaoVolume" Name="situacao" PropertyName="SelectedValue"
                                                    Type="String" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" EnablePaging="True"
                                MaximumRowsParameterName="pageSize" SelectCountMethod="GetForGeracaoVolumeCount"
                                SelectMethod="GetForGeracaoVolume" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                                TypeName="Glass.Data.DAL.PedidoDAO">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                                        Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCli" PropertyName="text"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataEntIni" Name="dataEntIni" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataEntFim" Name="dataEntFim" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataLibIni" Name="dataLibIni" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataLibFim" Name="dataLibFim" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                                        Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtRota" Name="codRota" PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="drpSituacaoPedido" Name="situacao" PropertyName="SelectedValue"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="drpTipoEntrega" Name="tipoEntrega" PropertyName="SelectedValue"
                                        Type="Int32" />
                                    <asp:ControlParameter ControlID="txtNumCliExterno" Name="idCliExterno" PropertyName="Text" Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNomeClienteExterno" Name="nomeCliExterno" PropertyName="text"
                                        Type="String" />
                                    
                                    <asp:ControlParameter ControlID="cblRotaExterna" Name="idsRotasExternas" PropertyName="SelectedItem"
                            Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblPedidosLiberados" runat="server" Text="* Pedidos liberados não são considerados na geração de volume."
                                Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        atualizarAutomaticamente();
    </script>

</asp:Content>
