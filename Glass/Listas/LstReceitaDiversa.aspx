<%@ Page Title="Lista de Receitas Diversas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstReceitaDiversa.aspx.cs" Inherits="Glass.UI.Web.Listas.LstReceitaDiversa" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var dataIni = FindContro("txtDataIni", "input").value;
            var dataFim = FindContro("txtDataFim", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ReceitaDiversa&exportarExcel=" + exportarExcel + "&dataIni=" + dataIni + "&dataFim=" + dataFim);
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
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadReceitaDiversa.aspx">Inserir Receita Diversa</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdReceitaDiversa" runat="server" 
                    AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsReceitaDiversa" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Não há receitas diversas cadastradas." 
                    DataKeyNames="IdReceita">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    PostBackUrl='<%# "~/Cadastros/CadReceitaDiversa.aspx?idReceita=" + Eval("IdReceita") %>' />
                                <asp:ImageButton ID="imbExcluir" runat="server" 
                                    CommandName="Delete" OnClientClick="if(!confirm('Deseja realmente excluir esse registro?')) return false;" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdReceita" HeaderText="Cód." SortExpression="IdReceita" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="DataReceita" HeaderText="Data" 
                            SortExpression="DataReceita" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="ValorReceita" HeaderText="Valor" 
                            SortExpression="ValorReceita" DataFormatString="{0:c}" />
                        <asp:BoundField DataField="BcPis" HeaderText="BC PIS" SortExpression="BcPis" />
                        <asp:BoundField DataField="AliquotaPis" HeaderText="Alíquota PIS" SortExpression="AliquotaPis" />
                        <asp:BoundField DataField="BcCofins" HeaderText="BC COFINS" SortExpression="BcCofins" />
                        <asp:BoundField DataField="AliquotaCofins" HeaderText="Alíquota COFINS" SortExpression="AliquotaCofins" />
                        <asp:BoundField DataField="DescricaoContaContabil" HeaderText="Conta Contábil" SortExpression="DescricaoContaContabil" />
                        <asp:BoundField DataField="DescricaoCentroCusto" HeaderText="Centro de Custo" SortExpression="DescricaoCentroCusto" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="TipoReceitaString" HeaderText="Tipo de Receita" ReadOnly="True"
                            SortExpression="TipoReceita" />
                        <asp:BoundField DataField="TipoOperacaoString" HeaderText="Tipo de Operação" ReadOnly="True"
                            SortExpression="TipoOperacao" />
                        <asp:BoundField DataField="CstPisString" HeaderText="CST PIS" ReadOnly="True"
                            SortExpression="CstPis" />
                        <asp:BoundField DataField="CstCofinsString" HeaderText="CST COFINS" ReadOnly="True"
                            SortExpression="CstCofins" />
                        <asp:BoundField DataField="NaturezaBcCreditoString" HeaderText="Natureza Bc Créd."
                            ReadOnly="True" SortExpression="NaturezaBcCredito" />
                        <asp:BoundField DataField="IndOrigemCredString" HeaderText="Ind. Origem Créd."
                            ReadOnly="True" SortExpression="IndOrigemCredString" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsReceitaDiversa" runat="server" DataObjectTypeName="Glass.Data.Model.ReceitaDiversa"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ReceitaDiversaDAO" 
                    SelectCountMethod="GetListCount">
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
