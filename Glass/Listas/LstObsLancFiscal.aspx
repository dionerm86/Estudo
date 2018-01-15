<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstObsLancFiscal.aspx.cs" Inherits="Glass.UI.Web.Listas.LstObsLancFiscal" Title="Observações do Lançamento Fiscal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdObsLancFiscal" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdObsLancFiscal" DataSourceID="odsObsLancFiscal" GridLines="None" 
                    ondatabound="grdObsLancFiscal_DataBound" 
                    onrowcommand="grdObsLancFiscal_RowCommand" ShowFooter="True" 
                    onrowdeleted="grdObsLancFiscal_RowDeleted">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/Ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" 
                                    ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir essa observação do lançamento fiscal?&quot;)) return false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="200" Rows="3" 
                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="200" Rows="3" 
                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    onclick="imgAdd_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObsLancFiscal" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.ObsLancFiscal" DeleteMethod="Delete" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ObsLancFiscalDAO" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:Parameter Name="idNfAdd" Type="UInt32" />
                        <asp:Parameter Name="idCteAdd" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

