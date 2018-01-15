<%@ Page Title="Lista de Deduções Diversas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstDeducaoDiversa.aspx.cs" Inherits="Glass.UI.Web.Listas.LstDeducaoDiversa" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var dataIni = FindContro("txtDataIni", "input").value;
            var dataFim = FindContro("txtDataFim", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=DeducaoDiversa&exportarExcel=" + exportarExcel + "&dataIni=" + dataIni + "&dataFim=" + dataFim);
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
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" style="width: 16px" />
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
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadDeducaoDiversa.aspx">Inserir Dedução Diversa</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdDeducaoDiversa" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsDeducaoDiversa"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" 
                    EmptyDataText="Não há deduções diversas cadastradas." DataKeyNames="IdDeducao">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    PostBackUrl='<%# "~/Cadastros/CadDeducaoDiversa.aspx?idDeducao=" + Eval("IdDeducao") %>' />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="if(!confirm('Deseja realmente excluir esse registro?')) return false;"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdDeducao" HeaderText="Cód." SortExpression="IdDeducao" />
                        <asp:BoundField DataField="DataDeducao" HeaderText="Data" 
                            SortExpression="DataDeducao" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="CnpjDedutora" HeaderText="CNPJ Dedutora" SortExpression="CnpjDedutora" />
                        <asp:BoundField DataField="BcDeducao" HeaderText="BC Dedução" SortExpression="BcDeducao" />
                        <asp:BoundField DataField="ValorPisDeduzir" HeaderText="Valor PIS a Deduzir" 
                            SortExpression="ValorPisDeduzir" DataFormatString="{0:c}" />
                        <asp:BoundField DataField="ValorCofinsDeduzir" 
                            HeaderText="Valor COFINS a Deduzir" SortExpression="ValorCofinsDeduzir" 
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="InformacoesComplementares" HeaderText="Informacões Compl."
                            SortExpression="InformacoesComplementares" />
                        <asp:BoundField DataField="OrigemDeducaoString" HeaderText="Origem Dedução"
                            SortExpression="OrigemDeducao" ReadOnly="True" />
                        <asp:BoundField DataField="NaturezaDeducaoString" HeaderText="Natureza Dedução"
                            SortExpression="NaturezaDeducao" ReadOnly="True" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDeducaoDiversa" runat="server" DataObjectTypeName="Glass.Data.Model.DeducaoDiversa"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.DeducaoDiversaDAO" SelectCountMethod="GetListCount">
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
