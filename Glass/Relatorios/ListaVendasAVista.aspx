<%@ Page Title="Vendas à Vista" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaVendasAVista.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaVendasAVista" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt() {
            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var idFormaPagto = FindControl("drpFormaPagto", "select").value;
            var tipoCartao = idFormaPagto == FindControl("hdfFormaPagtoCartao", "input").value ? FindControl("drpTipoCartao", "select").value : 0;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=VendaAVista&idLoja=" + idLoja + "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&idVend=" + idVend + "&idFormaPagto=" + idFormaPagto + "&tipoCartao=" + tipoCartao);

            return false;
        }

        function openRptUnico(idPedido) {
            openWindow(600, 800, "RelPedido.aspx?idPedido=" + idPedido);
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True"/>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left" colspan="3">
                            <table>
                                <tr>
                                    <td align="left">
                                        <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label11" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpFormaPagto" runat="server" DataSourceID="odsFormaPagto"
                                            DataTextField="Descricao" DataValueField="IdFormaPagto" AutoPostBack="True" OnSelectedIndexChanged="drpFormaPagto_SelectedIndexChanged" >
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                            DataTextField="Descricao" DataValueField="IdTipoCartao" Visible="False" AutoPostBack="True">
                                            <asp:ListItem Value="0">Todos</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClick="imgPesq_Click" />
                                         <asp:Image ID="imgConta" runat="server" ImageUrl="~/Images/Help.gif" ToolTip="Considera a Forma de Pagamento do Pedido e a Forma de Pagamento da Liberação."/>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdVendasAVista" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsAVista" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowPaging="True" AllowSorting="True" PageSize="20"
                    EmptyDataText="Nenhum pedido encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("ExibirRelatorio") %>'>
                                    <a href="#" onclick="openRptUnico('<%# Eval("IdPedido") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num. Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeInicialCli" HeaderText="Cliente" SortExpression="NomeInicialCli" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Vendedor" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="DataConf" DataFormatString="{0:d}" HeaderText="Data Conf."
                            SortExpression="DataConf">
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="CustoPedido" DataFormatString="{0:C}" HeaderText="Custo"
                            SortExpression="CustoPedido">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total" />
                        <asp:BoundField DataField="Lucro" DataFormatString="{0:C}" HeaderText="Lucro" SortExpression="Lucro" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfFormaPagtoCartao" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresByLoja"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" DefaultValue="" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAVista" runat="server" SelectMethod="GetListAVista"
                    TypeName="Glass.Data.DAL.PedidoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountAVista" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idVendedor" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoCartao" Name="tipoCartao" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetAVistaForFilter"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
