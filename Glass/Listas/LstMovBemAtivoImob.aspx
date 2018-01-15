<%@ Page Title="Movimentação de Bens/Componentes Ativo Imobilizado" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstMovBemAtivoImob.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovBemAtivoImob" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Data início" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server"
                                ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Data término" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server"
                                ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdMovBemAtivoImob" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdProdNf" DataSourceID="odsMovBemAtivoImob" GridLines="None" 
                    EmptyDataText="Não há bens/componentes do ativo imobilizado cadastrados ou não há movimentações desses bens.">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Cancel" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" CausesValidation="False" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" 
                                    ImageUrl="~/Images/EditarGrid.gif" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Bem/Comp. Ativo Imobilizado" 
                            SortExpression="DescrProd">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescrProd") %>'></asp:Label>
                                <asp:HiddenField ID="hdfIdBemAtivoImob" runat="server" 
                                    Value='<%# Bind("IdBemAtivoImobilizado") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescrProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Número NFe" SortExpression="NumeroNFe">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo NFe" SortExpression="TipoNFe">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DescricaoTipoNFe") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescricaoTipoNFe") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Mov." SortExpression="Data">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Data") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Data") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="Tipo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" AppendDataBoundItems="True" 
                                    SelectedValue='<%# Bind("Tipo") %>' DataSourceID="odsTipos" 
                                    DataTextField="Descr" DataValueField="Id">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTipo" runat="server" 
                                    ControlToValidate="drpTipo" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescricaoTipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor ICMS" SortExpression="ValorIcms">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("ValorIcms", "{0:c}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("ValorIcms", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor ICMS ST" SortExpression="ValorIcmsSt">
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" 
                                    Text='<%# Eval("ValorIcmsSt", "{0:c}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" 
                                    Text='<%# Bind("ValorIcmsSt", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor ICMS Frete" 
                            SortExpression="ValorIcmsFrete">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox11" runat="server" Text='<%# Bind("ValorIcmsFreteString") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" 
                                    Text='<%# Bind("ValorIcmsFrete", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor ICMS Dif." SortExpression="ValorIcmsDif">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("ValorIcmsDifString") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" 
                                    Text='<%# Bind("ValorIcmsDif", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Núm. Parc. ICMS" SortExpression="NumParcIcms">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("NumeroParcIcms") %>' Width="70px"
                                    onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("NumeroParcIcms") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Parc. ICMS" SortExpression="ValorParcIcms">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox10" runat="server" Text='<%# Bind("ValorParcIcmsString") %>'
                                    onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" 
                                    Text='<%# Bind("ValorParcIcms", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdProdNf") %>' 
                                    Tabela="MovBemAtivoImobilizado" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMovBemAtivoImob" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.MovimentacaoBemAtivoImob" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.MovimentacaoBemAtivoImobDAO" 
                    UpdateMethod="InsertOrUpdate" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" 
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" 
                            Type="String" />
                        <asp:Parameter DefaultValue="false" Name="apenasCadastrados" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipos" runat="server" SelectMethod="GetTipos" 
                    TypeName="Glass.Data.DAL.MovimentacaoBemAtivoImobDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

