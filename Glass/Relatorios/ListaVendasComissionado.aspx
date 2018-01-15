<%@ Page Title="Vendas por Comissionado" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaVendasComissionado.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaVendasComissionado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel) {
            if (!validate())
                return;

            var nomeComissionado = FindControl("txtNome", "input").value;
            var mesInicio = FindControl("drpInicio", "select").value;
            var anoInicio = FindControl("txtInicio", "input").value;
            var mesFim = FindControl("drpFim", "select").value;
            var anoFim = FindControl("txtFim", "input").value;
            var ordenar = FindControl("drpOrdenar", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=VendasComissionado&nomeComissionado=" + nomeComissionado +
                "&mesInicio=" + mesInicio + "&anoInicio=" + anoInicio + "&mesFim=" + mesFim + "&anoFim=" + anoFim + "&ordenar=" + ordenar +
                "&tipoVendas=1&exportarExcel=" + exportarExcel);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Comissionado" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpInicio" runat="server">
                                <asp:ListItem Value="1">Janeiro</asp:ListItem>
                                <asp:ListItem Value="2">Fevereiro</asp:ListItem>
                                <asp:ListItem Value="3">Março</asp:ListItem>
                                <asp:ListItem Value="4">Abril</asp:ListItem>
                                <asp:ListItem Value="5">Maio</asp:ListItem>
                                <asp:ListItem Value="6">Junho</asp:ListItem>
                                <asp:ListItem Value="7">Julho</asp:ListItem>
                                <asp:ListItem Value="8">Agosto</asp:ListItem>
                                <asp:ListItem Value="9">Setembro</asp:ListItem>
                                <asp:ListItem Value="10">Outubro</asp:ListItem>
                                <asp:ListItem Value="11">Novembro</asp:ListItem>
                                <asp:ListItem Value="12">Dezembro</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="txtInicio" runat="server" MaxLength="4" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)" Columns="5"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvInicio" runat="server" ErrorMessage="*" ControlToValidate="txtInicio"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="a" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFim" runat="server">
                                <asp:ListItem Value="1">Janeiro</asp:ListItem>
                                <asp:ListItem Value="2">Fevereiro</asp:ListItem>
                                <asp:ListItem Value="3">Março</asp:ListItem>
                                <asp:ListItem Value="4">Abril</asp:ListItem>
                                <asp:ListItem Value="5">Maio</asp:ListItem>
                                <asp:ListItem Value="6">Junho</asp:ListItem>
                                <asp:ListItem Value="7">Julho</asp:ListItem>
                                <asp:ListItem Value="8">Agosto</asp:ListItem>
                                <asp:ListItem Value="9">Setembro</asp:ListItem>
                                <asp:ListItem Value="10">Outubro</asp:ListItem>
                                <asp:ListItem Value="11">Novembro</asp:ListItem>
                                <asp:ListItem Value="12">Dezembro</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="txtFim" runat="server" MaxLength="4" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)" Columns="5"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFim" runat="server" ErrorMessage="*" ControlToValidate="txtFim"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Total</asp:ListItem>
                                <asp:ListItem Value="2">Cliente</asp:ListItem>
                            </asp:DropDownList>
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
                <asp:GridView GridLines="None" ID="grdVendas" runat="server" AutoGenerateColumns="False" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" OnRowDataBound="grdVendas_RowDataBound" EmptyDataText="Não foram encontradas vendas para esse filtro específico."
                    AllowPaging="True" DataSourceID="odsVendas" PageSize="20" 
                    DataKeyNames="IdComissionado">
                    <Columns>
                        <asp:BoundField DataField="IdNomeComissionado" HeaderText="Comissionado" 
                            SortExpression="NomeComissionado" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" SortExpression="Total">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVendas" runat="server" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetListCountComissionado" 
                    SelectMethod="GetListComissionado" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.RelDAL.VendasDAO" OnSelected="odsVendas_Selected" EnablePaging="True"
                    SortParameterName="sortExpression" >
                    <SelectParameters>
                        <asp:Parameter Name="idCliente" Type="UInt32" />
                        <asp:Parameter Name="nomeCliente" Type="String" />
                        <asp:Parameter Name="idComissionado" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeComissionado" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpInicio" Name="mesInicio" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtInicio" Name="anoInicio" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpFim" Name="mesFim" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtFim" Name="anoFim" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:Parameter DefaultValue="1" Name="tipoVendas" Type="Int32" />
                        <asp:Parameter Name="idsFuncionario" Type="String" />
                        <asp:Parameter Name="nomeFuncionario" Type="String" />
                        <asp:Parameter Name="valorMinimo" Type="Decimal" />
                        <asp:Parameter Name="valorMaximo" Type="Decimal" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
