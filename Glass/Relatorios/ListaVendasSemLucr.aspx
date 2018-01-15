<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaVendasSemLucr.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaVendasSemLucr" Title="Vendas sem Lucratividade" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var idPedido = FindControl("txtIdPedido", "input").value;
            var idCliente = FindControl("txtIdCliente", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var tipoVenda = FindControl("drpTipoVenda", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var orderBy = FindControl("drpOrdenar", "select").value;
            var agrupar = FindControl("chkAgrupar", "input").checked ? "1" : "0";

            openWindow(600, 800, "RelBase.aspx?rel=semLucr&idLoja=" + idLoja + "&dtIni=" + dtIni + "&dtFim=" + dtFim +
                "&idVend=" + idVend + "&idPedido=" + idPedido + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente +
                "&tipoVenda=" + tipoVenda + "&agrupar=" + agrupar + "&orderBy=" + orderBy + "&exportarExcel=" + exportarExcel);

            return false;
        }

        function openRptUnico(idPedido) {
            openWindow(600, 800, "RelPedido.aspx?idPedido=" + idPedido);
            return false;
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = ListaVendasSemLucr.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
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
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdCliente" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
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
                                        <asp:Label ID="Label11" runat="server" Text="Tipo Venda" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpTipoVenda" runat="server">
                                            <asp:ListItem Value="0">Todos</asp:ListItem>
                                            <asp:ListItem Value="1">À Vista</asp:ListItem>
                                            <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                            <asp:ListItem Value="3">Reposição</asp:ListItem>
                                            <asp:ListItem Value="4">Garantia</asp:ListItem>
                                            <asp:ListItem Value="5">Obra</asp:ListItem>
                                            <asp:ListItem Value="6">À vista/à prazo</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label12" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                            <asp:ListItem Value="1">Data Conf.</asp:ListItem>
                                            <asp:ListItem Value="2">Cliente</asp:ListItem>
                                            <asp:ListItem Value="3">Pedido</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkAgrupar" runat="server" Text="Agrupar relatório por vendedor" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdVendasLucr" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsLucr" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowPaging="True" AllowSorting="True" PageSize="20">
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
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Vendedor" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="DataConf" DataFormatString="{0:d}" HeaderText="Data Conf."
                            SortExpression="DataConf">
                            <HeaderStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt();">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresByLoja"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" DefaultValue="" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLucr" runat="server" SelectMethod="GetListLucr" TypeName="Glass.Data.DAL.PedidoDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountLucr"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idVendedor" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtIdCliente" Name="idCliente" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpTipoVenda" Name="tipoVenda" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="orderBy" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
