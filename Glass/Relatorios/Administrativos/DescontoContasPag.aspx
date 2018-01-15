<%@ Page Title="Desconto/Acréscimo em Contas a Pagar" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="DescontoContasPag.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.DescontoContasPag" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function limparVenc() {
            FindControl("txtDataIni", "input").value = "";
            FindControl("txtDataFim", "input").value = "";
        }

        function limparDesc() {
            FindControl("txtDataDescIni", "input").value = "";
            FindControl("txtDataDescFim", "input").value = "";
        }

        function openRpt(exportarExcel) {
            var idCompra = FindControl("txtIdCompra", "input").value;
            var numeroNf = FindControl("txtNumeroNf", "input").value;
            var dataIni = FindControl("txtDataIni", "input").value;
            var dataFim = FindControl("txtDataFim", "input").value;
            var dataDescIni = FindControl("txtDataDescIni", "input").value;
            var dataDescFim = FindControl("txtDataDescFim", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=DescParcPag&idCompra=" + idCompra + "&numeroNf=" + numeroNf + "&dataIni=" + dataIni +
                "&dataFim=" + dataFim + "&dataDescIni=" + dataDescIni + "&dataDescFim=" + dataDescFim + "&exportarExcel=" + exportarExcel);

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Compra"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdCompra" runat="server" onkeypress="return soNumeros(event, true, true)"
                                Width="60px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesqPed', 'input')"></asp:TextBox>
                            <asp:ImageButton ID="imgPesqPed" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="NF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumeroNf" runat="server" onkeypress="return soNumeros(event, true, true)"
                                Width="60px" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesqLib', 'input')"></asp:TextBox>
                            <asp:ImageButton ID="imgPesqLib" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Período Venc."></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                            <asp:ImageButton ID="imgLimparVenc" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                OnClientClick="limparVenc(); return false" ToolTip="Limpar período venc." />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Data do desconto/acréscimo"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataDescIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataDesc0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataDescIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataDescFim" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataDesc1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataDescFim', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                            <asp:ImageButton ID="imgLimparDesc" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                OnClientClick="limparDesc(); return false" ToolTip="Limpar data desconto" />
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
                <asp:GridView GridLines="None" ID="grdDescParc" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsDescParc"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" 
                    EmptyDataText="Nenhuma parcela com desconto encontrada.">
                    <Columns>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" 
                            SortExpression="Referencia" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" 
                            SortExpression="NomeFornec" />
                        <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                        <asp:BoundField DataField="ValorVenc" DataFormatString="{0:C}" HeaderText="Valor"
                            SortExpression="ValorVenc">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataVenc" DataFormatString="{0:d}" HeaderText="Data Venc."
                            SortExpression="DataVenc" />
                        <asp:BoundField DataField="ValorPago" DataFormatString="{0:C}" HeaderText="Valor Rec."
                            SortExpression="ValorPago">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataPagto" DataFormatString="{0:d}" HeaderText="Data Rec."
                            SortExpression="DataPagto" />
                        <asp:BoundField DataField="DescontoParc" DataFormatString="{0:C}" HeaderText="Valor Desc."
                            SortExpression="DescontoParc">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
<asp:BoundField DataField="AcrescimoParc" DataFormatString="{0:c}" HeaderText="Valor Acrésc." 
                            SortExpression="AcrescimoParc"></asp:BoundField>
                        <asp:BoundField DataField="DataDescAcresc" DataFormatString="{0:d}" HeaderText="Data Desc./Acrésc."
                            SortExpression="DataDescAcresc" />
                        <asp:BoundField DataField="NomeFuncDesc" HeaderText="Resp. Desc./Acrésc." 
                            SortExpression="NomeFuncDesc" />
                        <asp:BoundField DataField="MotivoDescontoAcresc" 
                            HeaderText="Motivo Desc./Acrésc." SortExpression="MotivoDescontoAcresc" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDescParc" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountContaComDesconto" SelectMethod="GetListContaComDesconto"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ContasPagarDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdCompra" Name="idCompra" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroNf" Name="numeroNf" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDataDescIni" Name="dataDescIni" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataDescFim" Name="dataDescFim" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;">
                    <img alt="" border="0" src="../../Images/printer.png" />  Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
