<%@ Page Title="Grupos do Plano de Contas e Centro de Custo não considerados pelo Ponto de Equilíbrio"
    Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaNaoConsideradosPE.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaNaoConsideradosPE" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdGruposConta" runat="server" AllowPaging="True"
                    AutoGenerateColumns="False" DataKeyNames="IdGrupo" DataSourceID="odsGruposConta"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="15" AllowSorting="True" ShowFooter="True"
                    Caption="Grupos de Conta">
                    <Columns>
                        <asp:TemplateField HeaderText="Categoria" SortExpression="Categoria">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Categoria") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo" SortExpression="IdGrupo">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdGrupo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGruposConta" runat="server" 
                    EnablePaging="True" 
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarGruposContaIgnoraPE" SortParameterName="sortExpression"
                    TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div style="overflow: auto; overflow-style: marquee,panner; height: 300px">
                    <asp:GridView GridLines="None" ID="grdCustoFixo" runat="server" AllowSorting="True"
                        AutoGenerateColumns="False" DataSourceID="odsCustoFixo" CssClass="gridStyle"
                        PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                        DataKeyNames="IdCustoFixo" EmptyDataText="Nenhum custo fixo encontrado." Caption="Custo Fixo">
                        <PagerSettings PageButtonCount="20" />
                        <Columns>
                            <asp:BoundField DataField="IdCustoFixo" HeaderText="Cód." SortExpression="IdCustoFixo" />
                            <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                            <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                            <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                            <asp:BoundField DataField="DescrPlanoConta" HeaderText="Rerente a" SortExpression="DescrPlanoConta" />
                            <asp:BoundField DataField="ValorVenc" HeaderText="Valor" SortExpression="ValorVenc"
                                DataFormatString="{0:C}">
                                <ItemStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DiaVenc" HeaderText="Dia Venc." SortExpression="DiaVenc">
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DataUltPagto" DataFormatString="{0:d}" HeaderText="Ult. Pagto"
                                SortExpression="DataUltPagto">
                                <HeaderStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                        </Columns>
                        <PagerStyle />
                        <EditRowStyle />
                        <AlternatingRowStyle />
                    </asp:GridView>
                    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCustoFixo" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                        SelectCountMethod="GetCountPE" SelectMethod="GetListPE" SortParameterName="sortExpression"
                        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.CustoFixoDAO"
                        DataObjectTypeName="Glass.Data.Model.CustoFixo" DeleteMethod="Delete">
                        <SelectParameters>
                            <asp:Parameter Name="idCustoFixo" Type="UInt32" />
                            <asp:Parameter Name="idLoja" Type="UInt32" />
                            <asp:Parameter Name="idFornec" Type="UInt32" />
                            <asp:Parameter Name="nomeFornec" Type="String" />
                            <asp:Parameter Name="diaVencIni" Type="Int32" />
                            <asp:Parameter Name="diaVencFim" Type="Int32" />
                            <asp:Parameter Name="descricao" Type="String" />
                            <asp:Parameter Name="situacao" Type="Int32" />
                        </SelectParameters>
                    </colo:VirtualObjectDataSource>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
