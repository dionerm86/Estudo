<%@ Page Title="Lista de Valores Retidos na Fonte" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstValorRetidoFonte.aspx.cs" Inherits="Glass.UI.Web.Listas.LstValorRetidoFonte" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var dataIni = FindContro("txtDataIni", "input").value;
            var dataFim = FindContro("txtDataFim", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ValorRetidoFonte&exportarExcel=" + exportarExcel + "&dataIni=" + dataIni + "&dataFim=" + dataFim);
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="txtDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="txtDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
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
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadValorRetidoFonte.aspx">Inserir Valor Retido na Fonte</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdValorRetidoFonte" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsValorRetidoFonte"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Não há valores retidos na fonte cadastradas."
                    DataKeyNames="IdValorRetidoFonte">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    PostBackUrl='<%# "~/Cadastros/CadValorRetidoFonte.aspx?IdValorRetidoFonte=" + Eval("IdValorRetidoFonte") %>' />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="if(!confirm('Deseja realmente excluir esse registro?')) return false;"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdValorRetidoFonte" HeaderText="Cód." SortExpression="IdValorRetidoFonte" />
                        <asp:BoundField DataField="DataRetencao" HeaderText="Data" SortExpression="DataRetencao"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="BcRetencao" HeaderText="BC Retenção" SortExpression="BcRetencao" />
                        <asp:BoundField DataField="ValorRetido" HeaderText="Valor Retido" SortExpression="ValorRetido"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="CnpjRetentora" HeaderText="CNPJ Retentora" SortExpression="CnpjRetentora" />
                        <asp:BoundField DataField="ValorPisRetido" HeaderText="Valor PIS Retido" SortExpression="ValorPisRetido"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="ValorCofinsRetido" HeaderText="Valor COFINS Retido" SortExpression="ValorCofinsRetido"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="CodigoReceita" HeaderText="Cód. da Receita" 
                            SortExpression="CodigoReceita" />
                        <asp:BoundField DataField="NaturezaRetencaoString" HeaderText="Natureza Retenção"
                            SortExpression="NaturezaRetencao" ReadOnly="True" />
                        <asp:BoundField DataField="NaturezaReceitaString" HeaderText="Natureza Receita"
                            SortExpression="NaturezaReceita" ReadOnly="True" />
                        <asp:BoundField DataField="TipoDeclaranteString" HeaderText="Tipo Declarante" SortExpression="TipoDeclarante"
                            ReadOnly="True" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsValorRetidoFonte" runat="server" DataObjectTypeName="Glass.Data.Model.ValorRetidoFonte"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ValorRetidoFonteDAO" SelectCountMethod="GetListCount">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"
                    Visible="False"> <img 
                    border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"
                    Visible="False"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
