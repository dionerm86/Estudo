<%@ Page Title="Lista de Informações Adicionais de NF-e Extemporânea" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstInfoAdicNFEExtemporanea.aspx.cs" Inherits="Glass.UI.Web.Listas.LstInfoAdicNFEExtemporanea" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                          <td>
                            <asp:Label ID="Label1" runat="server" Text="Tipo Imposto:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                              <asp:DropDownList ID="drpTipoImposto" runat="server" 
                                    DataSourceID="odsTipoImposto" 
                                    DataTextField="Descr" DataValueField="Id" AppendDataBoundItems="True">
                                  <asp:ListItem Value="0">Selecione</asp:ListItem>
                                </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" style="width: 16px" />
                        </td>
                          <td>
                            <asp:Label ID="Label3" runat="server" Text="Número NF-e:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                             <uc1:ctrlSelPopup ID="ctrlSelNotaFiscal" runat="server" DataSourceID="odsNotaFiscal" 
                                    DataTextField="NumeroNFe" DataValueField="IdNF" 
                                    Descricao='<%# Eval("NumeroNFe") %>' PermitirVazio="True" TextWidth="133px" 
                                    TituloTela="Selecione a nota fiscal" Valor='<%# Bind("IdNFE") %>' 
                                    ColunasExibirPopup="IdNf|NumeroNFe|NomeEmitente|NomeDestRem|TotalNota" 
                                    TitulosColunas="IdNf|Número NF-e|Emitente|Destinatário|Total" 
                                 FazerPostBackBotaoPesquisar="True" />
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
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadInfoAdicNFEExtemporanea.aspx">Inserir Informação Adicional de NF-e Extemporânea</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdInfoAdicNFEExtemporanea" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsInfoAdicNFEExtemporanea"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" 
                    EmptyDataText="Não há informações adicionais de NF-e extemporâneas." 
                    DataKeyNames="IdInfoAdicNFEExtemporanea">
                    <Columns>
                     <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    PostBackUrl='<%# "~/Cadastros/CadInfoAdicNFEExtemporanea.aspx?id=" + Eval("IdInfoAdicNFEExtemporanea") %>' />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="if(!confirm('Deseja realmente excluir esse registro?')) return false;"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroNFe" HeaderText="Número NF-e" 
                            SortExpression="NumeroNFe" />
                        <asp:BoundField DataField="DescrTipoImposto" HeaderText="Tipo Imposto" 
                            SortExpression="DescrTipoImposto" ReadOnly="True" />
                        <asp:BoundField DataField="ValorOutDeducao" HeaderText="Valor Out. Ded." 
                            SortExpression="ValorOutDeducao" />
                        <asp:BoundField DataField="ValorMulta" 
                            HeaderText="Valor Multa" SortExpression="ValorMulta" />
                        <asp:BoundField DataField="ValorJuro" 
                            HeaderText="Valor Juros" 
                            SortExpression="ValorJuro" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfoAdicNFEExtemporanea" runat="server" DataObjectTypeName="Glass.Data.Model.EFD.InfoAdicNFEExtemporanea"
                    DeleteMethod="Excluir" SelectMethod="ObterLista" 
                    TypeName="Glass.Data.DAL.EFD.InfoAdicNFEExtemporaneaDAO"  
                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlSelNotaFiscal" Name="idNfe" PropertyName="Valor" 
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpTipoImposto" Name="tipoImposto" 
                            PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" 
                    SelectMethod="GetTipoImposto" TypeName="Glass.Data.EFD.DataSourcesEFD" 
                    >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNotaFiscal" runat="server" 
                    SelectMethod="ObtemAutorizadasFinalizadas" TypeName="Glass.Data.DAL.NotaFiscalDAO" 
                    >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"
                    Visible="False"> <img border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"
                    Visible="False"><img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
