<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadItensOrdemCarga.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadItensOrdemCarga" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <style type="text/css">
        .boxFlutuante
        {
            margin-left: 5px;
            margin-top: 5px;
            float: left;
            position: relative;
            width: 170px;
            background-color: #fff;
            border: 1px solid #C0C0C0;
            text-align: left;
            -webkit-border-radius: 5px;
            -moz-border-radius: 5px;
            border-radius: 5px;
            -webkit-box-shadow: 7px 6px 5px rgba(50, 50, 50, 0.4);
            -moz-box-shadow: 7px 6px 5px rgba(50, 50, 50, 0.4);
            box-shadow: 7px 6px 5px rgba(50, 50, 50, 0.4);
            padding: 5px;
        }
    </style>

    <script type="text/javascript">

        function finalizarOC() {
            window.opener.atualizar();
            closeWindow();
        }

        function selecionaTodosPedidos(check) {
            var tabela = check;
            while (tabela.nodeName.toLowerCase() != "table")
                tabela = tabela.parentNode;

            var checkBoxProdutos = tabela.getElementsByTagName("input");

            var i = 0;
            for (i = 0; i < checkBoxProdutos.length; i++) {
                if (checkBoxProdutos[i].id.indexOf("chkTodos") > -1 || checkBoxProdutos[i].disabled)
                    continue;

                checkBoxProdutos[i].checked = check.checked;
            }
        }

        function exibirProdutos(botao, idPedido) {

            var linha = document.getElementById("pedido_" + idPedido);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " produtos";
        }

        function exibirObs(botao, idPedido, msg) {

            var boxObs = FindControl("boxObs", "div");
            var lblObs = FindControl("lblObs", "span");

            lblObs.innerHTML = msg;

            TagToTip('boxObs', FADEIN, 300, COPYCONTENT, false, TITLE, 'Observação do pedido: ' + idPedido, CLOSEBTN, true,
                CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('boxObs'), -41 - getTableHeight('boxObs')]);
        }

        $(document).ready(function() {
            $(window).scroll(function() {
                set = ($(document).scrollTop()) + "px";
                $('#boxFloat').animate({ top: set }, { duration: 500, queue: false });
            });
        });
        
        
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView runat="server" ID="grvCliente" DataSourceID="odsCliente" AutoGenerateRows="false"
                    GridLines="None" DataKeyNames="IdCli" DefaultMode="ReadOnly">
                    <Fields>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table cellspacing="1" cellpadding="5" style="min-width: 300px;">
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Cliente
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("IdCli") + " - " +
                                                (Glass.Configuracoes.Liberacao.RelatorioLiberacaoPedido.TipoNomeExibirRelatorioPedido == Glass.Data.Helper.DataSources.TipoNomeExibirRelatorioPedido.NomeFantasia ?
                                                    Eval("NomeFantasia") : Eval("Nome")) %>' />
                                        </td>
                                        <td class="dtvHeader">
                                            CPF / CNPJ
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label3" runat="server" Text='<%# Eval("CpfCnpj") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Rota
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label2" runat="server" Text='<%# Eval("Rota") %>' />
                                        </td>
                                        <td class="dtvHeader">
                                            Telefone
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label4" runat="server" Text='<%# Eval("TelCont") %>' />
                                            <asp:Label ID="Label5" runat="server" Text='<%# Eval("TelCel") %>' />
                                        </td>
                                    </tr>
                                    <tr class="dtvAlternatingRow">
                                        <td class="dtvHeader">
                                            Endereço
                                        </td>
                                        <td nowrap="nowrap" colspan="3">
                                            <asp:Label ID="Label6" runat="server" Text='<%# Eval("EnderecoCompleto") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource Culture="pt-BR" runat="server" ID="odsCliente" SelectMethod="GetElement"
                    DataObjectTypeName="Glass.Data.Model.Cliente" TypeName="Glass.Data.DAL.ClienteDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idCliente" QueryStringField="idCliente" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td style="color: red; font-style:italic; font-weight: bold;">* Pedidos com observação, peças ou volumes pendentes produção</td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:GridView ID="grdPedidos" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                                DataKeyNames="IdPedido" DataSourceID="odsPedidos" GridLines="None" OnRowDataBound="grdPedidos_RowDataBound"
                                style="min-width:550px;">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hdfPeso" runat="server" Value='<%# Bind("PesoOC") %>' />
                                            <asp:HiddenField ID="hdfTotM" runat="server" Value='<%# Bind("TotMOC") %>' />
                                            <asp:HiddenField ID="hdfQtdeItens" runat="server" Value='<%# Bind("QtdePecasVidro") %>' />
                                            <asp:HiddenField ID="hdfQtdeVolumes" runat="server" Value='<%# Bind("QtdeVolume") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdutos(this, " + Eval("IdPedido") + "); return false" %>'
                                                Width="10px" ToolTip="Exibir Produtos" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkSelPed" ToolTip="Selecionar todos os pedidos" runat="server"
                                                onclick="selecionaTodosPedidos(this)" AutoPostBack="true" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelPed" ToolTip='<%# "Selecionar o pedido: " + Eval("idPedido")  %>'
                                                runat="server" AutoPostBack="True" OnCheckedChanged="chkSelPed_CheckedChanged" />
                                            <asp:HiddenField runat="server" ID="hdfIdPedido" Value='<%# Eval("idPedido") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pedido">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# 
                                                (Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados && Convert.ToBoolean(Eval("Importado")) ?
                                                    Eval("IdPedido") + " (" + Eval("IdPedidoExterno") + ")" : Eval("IdPedidoCodCliente")).ToString() + 
                                                    (Convert.ToBoolean(Eval("GerarPedidoProducaoCorte")) ? "(Rev. " + Eval("IdPedidoRevenda") + ")" : "").ToString() %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Clinete Externo">
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
                                    <asp:BoundField HeaderText="Peso" DataField="PesoOC" />
                                    <asp:BoundField HeaderText="Volumes Pendentes" DataField="VolumesPendentes" />
                                    <asp:TemplateField HeaderText="Obs.">
                                        <ItemTemplate>
                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("ObsLiberacao") != null && Eval("ObsLiberacao") != "" %>'>
                                                <a href="#" onclick='exibirObs(this, <%# Eval("IdPedido") %>,&#039;<%# Eval("ObsLiberacao") != null ? ((string)Eval("ObsLiberacao")).Replace("\r\n"," ").Replace("'","") : "" %>&#039;); return false;'>
                                                    <img alt="" border="0" src="../Images/blocodenotas.png" title="Ver Observação." /></a>
                                            </asp:PlaceHolder>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            </td> </tr>
                                            <tr id="pedido_<%# Eval("IdPedido") %>" style="display: none;" class="<%= GetAlternateClass() %>">
                                                <td colspan="10">
                                                    <div style="overflow-y: auto; height: 200px;">
                                                        <asp:GridView ID="grdProdutos" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                                                            DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedido" GridLines="None" Width="100%">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Produto">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label7" runat="server" Text='<%# Eval("CodInternoDescProd") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Altura">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label8" runat="server" Text='<%# Eval("Altura") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Largura">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label9" runat="server" Text='<%# Eval("Largura") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Proc.">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label10" runat="server" Text='<%# Eval("CodProcesso") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Apl.">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label10" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Setores Pendentes">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label11" runat="server" Text='<%# Eval("SetoresPendentes") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosPedido" runat="server"
                                                            SelectMethod="GetProdutosByPedido" 
                                                            TypeName="WebGlass.Business.OrdemCarga.Fluxo.PedidosOCFluxo">
                                                            <SelectParameters>
                                                                <asp:ControlParameter ControlID="hdfIdPedido" PropertyName="Value" Name="idPedido"
                                                                    Type="UInt32" />
                                                                <asp:Parameter Name="produtosEstoque" DefaultValue="False" Type="Boolean" />
                                                            </SelectParameters>
                                                        </colo:VirtualObjectDataSource>
                                  
                                                        <asp:GridView ID="grdProdutosEstoque" runat="server" AutoGenerateColumns="False"
                                                            CssClass="gridStyle" DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedidoEstoque"
                                                            GridLines="None" Width="100%">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Produto Estoque">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label7" runat="server" Text='<%# Eval("CodInternoDescProd") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Altura">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label8" runat="server" Text='<%# Eval("Altura") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Largura">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label9" runat="server" Text='<%# Eval("Largura") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Qtde.">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label10" runat="server" Text='<%# Eval("Qtde") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosPedidoEstoque" runat="server"
                                                            SelectMethod="GetProdutosByPedido" 
                                                            TypeName="WebGlass.Business.OrdemCarga.Fluxo.PedidosOCFluxo">
                                                            <SelectParameters>
                                                                <asp:ControlParameter ControlID="hdfIdPedido" PropertyName="Value" Name="idPedido"
                                                                    Type="UInt32" />
                                                                <asp:Parameter Name="produtosEstoque" DefaultValue="True" Type="Boolean" />
                                                            </SelectParameters>
                                                        </colo:VirtualObjectDataSource>
                                                        
                                                        <asp:GridView ID="grdVolumes" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                                                            DataKeyNames="IdVolume" DataSourceID="odsVolumesPedido" GridLines="None" Width="100%">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Cód. Volume">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label7" runat="server" Text='<%# Eval("IdVolume") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Peso">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Label8" runat="server" Text='<%# Eval("PesoTotal") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle HorizontalAlign="Left" />
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVolumesPedido" runat="server"
                                                            SelectMethod="GetVolumesByPedido" 
                                                            TypeName="WebGlass.Business.OrdemCarga.Fluxo.PedidosOCFluxo">
                                                            <SelectParameters>
                                                                <asp:ControlParameter ControlID="hdfIdPedido" PropertyName="Value" Name="idPedido"
                                                                    Type="UInt32" />
                                                            </SelectParameters>
                                                        </colo:VirtualObjectDataSource>
                                                    </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle HorizontalAlign="Left" />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource Culture="pt-BR" runat="server" ID="odsPedidos" DataObjectTypeName="Glass.Data.Model.Pedido"
                                SelectMethod="GetPedidosForItensOC" TypeName="WebGlass.Business.OrdemCarga.Fluxo.PedidosOCFluxo">
                                <SelectParameters>
                                    <asp:QueryStringParameter Name="tipoOC" QueryStringField="tipoOC" Type="Object" />
                                    <asp:QueryStringParameter Name="idCliente" QueryStringField="idCliente" Type="UInt32" />
                                    <asp:QueryStringParameter Name="idRota" QueryStringField="idRota" Type="UInt32" />
                                    <asp:QueryStringParameter Name="idLoja" QueryStringField="idLoja" Type="UInt32" />
                                    <asp:QueryStringParameter Name="dtEntPedidoIni" QueryStringField="dtEntPedidoIni" Type="String" />
                                    <asp:QueryStringParameter Name="dtEntPedidoFin" QueryStringField="dtEntPedidoFin" Type="String" />
                                    <asp:QueryStringParameter Name="pedidosObs" QueryStringField="pedidosObs" Type="Boolean" />
                                    <asp:QueryStringParameter Name="fastDelivery" QueryStringField="fastDelivery" Type="Boolean" />
                                    <asp:QueryStringParameter Name="codRotasExternas" QueryStringField="codRotasExternas" Type="String" />
                                    <asp:QueryStringParameter Name="idClienteExterno" QueryStringField="idClienteExterno" Type="UInt32" />
                                    <asp:QueryStringParameter Name="nomeClienteExterno" QueryStringField="nomeClienteExterno" Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td valign="top">
                            <div id="boxFloat" class="boxFlutuante">
                                <table>
                                    <tr>
                                        <td>
                                            <b>Peso Total:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPeso" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Total M²:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblTotM" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Qtde. Itens:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblQtdeItens" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Qtde. Volumes:</b>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblQtdeVolumes" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
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
                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar OC" OnClientClick="bloquearPagina();desbloquearPagina(false);"
                    OnClick="btnFinalizar_Click" />
            </td>
        </tr>
    </table>
    <div id="boxObs" style="display: none; width: 350px;">
        <asp:Label ID="lblObs" runat="server" Text="Label"></asp:Label>
    </div>
</asp:Content>
