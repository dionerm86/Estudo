<%@ Page Title="Fluxo de Caixa" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="FluxoCaixa.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.FluxoCaixa" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

function openRpt(exportarExcel)
{
    var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
    var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
    var prevCustoFixo = FindControl("chkPrevCustoFixo", "input").checked ? 1 : 0;
    var resumido = FindControl("chkResumido", "input").checked ? 1 : 0;
    var tipoConta = FindControl("cbdTipoConta", "select").itens();

    openWindow(600, 800, "RelBase.aspx?rel=" + (resumido == 0 ? "FluxoCaixa" : "FluxoCaixaSintetico") + "&tipoConta=" + tipoConta + 
        "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&prevCustoFixo=" + prevCustoFixo + "&resumido=" + resumido + 
        "&exportarExcel=" + exportarExcel);
    
    return false;
}

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" ValidateEmptyText="true" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"  ValidateEmptyText="true"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Tipo de Conta"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdTipoConta" runat="server" CheckAll="True" Title="Selecione"
                                Width="150px">
                                <asp:ListItem Value="0">Não contábil</asp:ListItem>
                                <asp:ListItem Value="1">Contábil</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkPrevCustoFixo" runat="server" Text="Previsão de Custos Fixos"
                                AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkResumido" runat="server" Text="Sintético" AutoPostBack="True" />
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
                <asp:GridView ID="grdFluxoCaixa" runat="server" CssClass="gridStyle" GridLines="None"
                    AutoGenerateColumns="False" DataSourceID="odsFluxoCaixa" OnRowDataBound="grdFluxoCaixa_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Data" DataFormatString="{0:d}" HeaderText="Data" SortExpression="Data" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="Parceiro" HeaderText="Parceiro" SortExpression="Parceiro" />
                        <asp:BoundField DataField="Debito" DataFormatString="{0:c}" HeaderText="Débito" SortExpression="Valor" />
                        <asp:BoundField DataField="Credito" DataFormatString="{0:c}" HeaderText="Crédito"
                            SortExpression="Valor" />
                        <asp:BoundField DataField="SaldoGeral" DataFormatString="{0:c}" HeaderText="Total Acumulado"
                            SortExpression="SaldoGeral" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <asp:GridView ID="grdFluxoCaixaSint" runat="server" CssClass="gridStyle" GridLines="None"
                    AutoGenerateColumns="False" DataSourceID="odsFluxoCaixaSint" OnRowDataBound="grdFluxoCaixaSint_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Data" DataFormatString="{0:d}" HeaderText="Data" SortExpression="Data" />
                        <asp:BoundField DataField="ValorSaida" DataFormatString="{0:c}" HeaderText="Débito"
                            SortExpression="ValorSaida" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:c}" HeaderText="Crédito"
                            SortExpression="ValorEntrada" />
                        <asp:BoundField DataField="SaldoGeral" DataFormatString="{0:c}" HeaderText="Total Acumulado"
                            SortExpression="SaldoGeral" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFluxoCaixa" runat="server" SelectMethod="GetList" TypeName="Glass.Data.RelDAL.FluxoCaixaDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkPrevCustoFixo" Name="prevCustoFixo" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="cbdTipoConta" Name="tipoConta" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFluxoCaixaSint" runat="server" SelectMethod="GetList"
                    TypeName="Glass.Data.RelDAL.FluxoCaixaSinteticoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkPrevCustoFixo" Name="prevCustoFixo" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="cbdTipoConta" Name="tipoConta" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"> <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
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
