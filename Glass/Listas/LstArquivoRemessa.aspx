<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstArquivoRemessa.aspx.cs" Inherits="Glass.UI.Web.Listas.LstArquivoRemessa" Title="Arquivos de Remessa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdArquivoRemessa" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdArquivoRemessa" DataSourceID="odsArquivoRemessa" 
                    EmptyDataText="Não há arquivos de remessa." GridLines="None" PageSize="30" 
                    onrowdatabound="grdArquivoRemessa_RowDataBound">
                    <Columns>
                    <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir este arquivo remessa?');"
                                    Visible='<%# Eval("DeletarVisivel") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/disk.gif" 
                                    onclientclick='<%# "redirectUrl(\"../Handlers/ArquivoRemessa.ashx?id=" + Eval("IdArquivoRemessa") + "\"); return false" %>' 
                                    ToolTip="Download do Arquivo" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRetificar" runat="server" ImageUrl="~/Images/retificar.png" Height="16" Width="16"
                                    onclientclick='<%# "openWindow(700, 1000,\"../Cadastros/CadRetificarArquivoRemessa.aspx?id=" + Eval("IdArquivoRemessa") + "\"); return false" %>' 
                                    ToolTip="Retificar Arquivo Remessa" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdArquivoRemessa" HeaderText="Cód." 
                            SortExpression="IdArquivoRemessa" />
                        <asp:BoundField DataField="NumRemessa" HeaderText="Núm. Remessa" 
                            SortExpression="NumRemessa" />
                        <asp:BoundField DataField="Tipo" HeaderText="Tipo" SortExpression="Tipo" />
                        <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" 
                            SortExpression="DescrUsuCad" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data Cad." 
                            SortExpression="DataCad" />
                             <asp:TemplateField>
                            <ItemTemplate>
                               <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/blocodenotas.png" 
                                    onclientclick='<%# "redirectUrl(\"../Handlers/ArquivoRemessa.ashx?logImportacao=true&id=" + Eval("IdArquivoRemessa") + "\"); return false" %>' 
                                    ToolTip="Log de importação" Visible='<%# Eval("LogVisivel") %>'/>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsArquivoRemessa" 
                    runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
                    SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" DataObjectTypeName="Glass.Data.Model.ArquivoRemessa"
                    TypeName="Glass.Data.DAL.ArquivoRemessaDAO" DeleteMethod="Delete"
                    ondeleted="odsArquivoRemessa_Deleted"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

