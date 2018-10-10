<%@ Page Title="Informações sobre Pedidos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstInfoPedidos.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstInfoPedidos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function voltar()
        {
            var dataIni = '<%= Request["dataIni"] %>';
            var dataFim = '<%= Request["dataFim"] %>';
            redirectUrl("LstInfoPedidosPeriodo.aspx?dataIni=" + dataIni + "&dataFim=" + dataFim);
        }
        
        function getCli(idCli)
        {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
            
            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCli", "input").value = "";
                return false;
            }
            
            FindControl("txtNomeCli", "input").value = retorno[1];
        }

        function openRpt()
        {
            var data = FindControl("lblDataConsulta", "span").innerHTML;
            var fastDelivery = FindControl("lblM2FastDelivery", "span").innerHTML;
            
            var idPedido = FindControl("txtIdPedido", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNomeCli", "input").value;
            var tipo = FindControl("drpTipoPedido", "select").value;
            
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=InfoPedidos&data=" + data + "&fastDelivery=" + fastDelivery + 
               "&idPedido=" + idPedido + "&idCliente=" + idCliente + "&nomeCliente=" +
                nomeCliente + "&tipo=" + tipo);
        }
        
        function openRptPedido(id)
        {
            openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + id + "&tipo=0");
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Panel ID="panFiltro" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Data de consulta"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtData" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                                <asp:ImageButton ID="imgData" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                    OnClientClick="return SelecionaData('txtData', this)" ToolTip="Alterar" />
                            </td>
                            <td>
                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <span runat="server" id="voltar" visible="false">
                        <asp:HyperLink ID="lnkVoltar" runat="server" onclick="voltar(); return false;" NavigateUrl="#">Voltar</asp:HyperLink>
                        <br />
                        <br />
                    </span>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Data de consulta
                        </td>
                        <td style="padding-left: 8px">
                            <asp:Label ID="lblDataConsulta" runat="server" ForeColor="#0066FF"></asp:Label>
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            Quantidade de m² para Fast Delivery
                        </td>
                        <td style="padding-left: 8px">
                            <asp:Label ID="lblM2FastDelivery" runat="server" ForeColor="#0066FF"></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) { cOnClick('imgPesq', 'input'); return false }"
                                Width="70px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" onkeypress="return soNumeros(event, true, true)"
                                onblur="getCli(this)" Width="50px"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCli" runat="server" Width="200px" onkeydown="if (isEnter(event)) { cOnClick('imgPesq', 'input'); return false }"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoPedido" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Venda</asp:ListItem>
                                <asp:ListItem Value="2">Produção</asp:ListItem>
                                <asp:ListItem Value="3">Mão de obra</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:GridView GridLines="None" ID="grdPedidos" runat="server" AutoGenerateColumns="False" DataKeyNames="IdPedido"
                    DataSourceID="odsPedidos" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Não foram encontrados pedidos para esse período.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRptPedido(" + Eval("IdPedido") + "); return false" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" ReadOnly="True" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Vendedor" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DescricaoTipoPedido" HeaderText="Tipo" ReadOnly="True"
                            SortExpression="DescricaoTipoPedido" />
                        <asp:BoundField DataField="DescrSituacaoPedido" HeaderText="Situação" ReadOnly="True"
                            SortExpression="DescrSituacaoPedido" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total" />
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidos" runat="server" SelectMethod="GetForInfoPedidos"
                    TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtData" Name="dataIni" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtData" Name="dataFim" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCli" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoPedido" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
