<%@ Page Title="Produção Instalação" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaProducaoInst.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaProducaoInst" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt() {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idEquipe = FindControl("drpEquipe", "select").value;
            var tipoInstalacao = FindControl("drpTipoInstalacao", "select").value;
            var exibirProdutos = FindControl("chkExibirProdutos", "input").checked;

            openWindow(600, 800, "RelBase.aspx?rel=ProducaoInst&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idEquipe=" + idEquipe +
                "&tipoInstalacao=" + tipoInstalacao + "&exibirProdutos=" + exibirProdutos);

            return false;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Equipe"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe" runat="server" AppendDataBoundItems="True" DataSourceID="odsEquipe"
                                DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Tipo Instalação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoInstalacao" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsTipoInstalacao" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkExibirProdutos" Text="Exibir produtos na impressão" ForeColor="#0066FF" runat="server" />
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
                <asp:GridView GridLines="None" ID="grdProducaoInst" runat="server" DataSourceID="odsProducaoInst"
                    AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:BoundField DataField="NomeEquipe" HeaderText="Equipe" SortExpression="NomeEquipe" />
                        <asp:BoundField DataField="TipoEquipe" HeaderText="Tipo" SortExpression="TipoEquipe" />
                        <asp:BoundField DataField="QtdePecas" HeaderText="Qtde. Peças Colocadas" SortExpression="QtdePecas" />
                        <asp:BoundField DataField="TotalM2" HeaderText="Qtde. m² Colocados" SortExpression="TotalM2" />
                        <asp:BoundField DataField="QtdeGarantia" HeaderText="Qtde. Garantia" SortExpression="QtdeGarantia" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProducaoInst" runat="server" SelectMethod="GetProducaoInst"
                    TypeName="Glass.Data.RelDAL.ProducaoInstDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpEquipe" Name="idEquipe" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoInstalacao" Name="tipoInstalacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoInstalacao" runat="server" SelectMethod="GetTipoInstalacao"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.EquipeDAO"
                    MaximumRowsParameterName="" StartRowIndexParameterName="">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
