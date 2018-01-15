<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaOrcamentos.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaOrcamentos" Title="Orçamentos" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idLoja = FindControl("drpLoja", "select").value;
            var idVend = FindControl("drpVendedor", "select").value;
            var situacao = FindControl("drpSituacao", "select").itens();
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dtIniSit = FindControl("ctrlDataIniSit_txtData", "input") != null ? FindControl("ctrlDataIniSit_txtData", "input").value : "";
            var dtFimSit = FindControl("ctrlDataFimSit_txtData", "input") != null ? FindControl("ctrlDataFimSit_txtData", "input").value : "";

            openWindow(600, 800, "RelBase.aspx?rel=ListaOrcamento&idLoja=" + idLoja + "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&idVend=" + idVend +
                "&situacao=" + situacao + "&dtIniSit=" + dtIniSit + "&dtFimSit=" + dtFimSit + "&exportarExcel=" + exportarExcel);

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
                        <td align="left">
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="left">
                            <asp:Label ID="Label8" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsVendedor" DataTextField="Nome"
                                DataValueField="IdFunc" AutoPostBack="True" OnTextChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label11" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                         <sync:CheckBoxListDropDown runat="server" ID="drpSituacao" DataSourceID="odsSituacaoOrca"
                             DataTextField="Descr" DataValueField="Id" AutoPostBack="true"></sync:CheckBoxListDropDown>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPeriodoSituacao" runat="server" Text="Período (Situação)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataIniSit" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataFimSit" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesqPeriodoSituacao" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdOrcamento" runat="server" AllowPaging="True"
                    AllowSorting="True" DataSourceID="odsOrcamento" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AutoGenerateColumns="False"
                    PageSize="20">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Num" SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFuncAbrv" HeaderText="Vendedor" ReadOnly="True" SortExpression="NomeFuncAbrv" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data Orça."
                            ReadOnly="True" SortExpression="DataCad" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total" />
                        <asp:BoundField DataField="ObservacaoConcatenada" HeaderText="Observação" SortExpression="Obs" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOrcamento" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountRptLista" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    SelectMethod="GetForLista" TypeName="Glass.Data.DAL.OrcamentoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idVendedor" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValues" />
                        <asp:ControlParameter ControlID="ctrlDataIniSit" Name="dataIniSit" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimSit" Name="dataFimSit" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsVendedor" runat="server" SelectMethod="GetVendedoresByLoja"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" DefaultValue="" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoOrca" runat="server" SelectMethod="GetSituacaoOrcamento"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
