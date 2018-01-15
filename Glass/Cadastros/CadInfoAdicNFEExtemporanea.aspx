<%@ Page Title="Cadastro de Informações Adicionais de NF-e Extemporânea" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadInfoAdicNFEExtemporanea.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadInfoAdicNFEExtemporanea" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvInfoAdicNFEExtemp" runat="server" 
                    AutoGenerateRows="False" DataSourceID="odsInfoAdicNFEExtemp"
                    DefaultMode="Insert" GridLines="None" Height="50px" Width="125px" CellPadding="4"
                    Style="margin-right: 2px" DataKeyNames="IdInfoAdicNFEExtemporanea" 
                    onitemcommand="dtvInfoAdicNFEExtemp_ItemCommand" 
                    oniteminserting="dtvInfoAdicNFEExtemp_ItemInserting" 
                    onitemupdating="dtvInfoAdicNFEExtemp_ItemUpdating" >
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle CssClass="dtvAlternatingRow" />
                    <FieldHeaderStyle Wrap="False" Font-Bold="False" CssClass="dtvHeader" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Número NF-e" SortExpression="IdNFE">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("IdNFE") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelNotaFiscal" runat="server" DataSourceID="odsNotaFiscal" 
                                    DataTextField="NumeroNFe" DataValueField="IdNF" 
                                    Descricao='<%# Eval("NumeroNFe") %>' PermitirVazio="False" TextWidth="133px" 
                                    TituloTela="Selecione a nota fiscal" Valor='<%# Bind("IdNFE") %>' 
                                    ColunasExibirPopup="IdNf|NumeroNFe|NomeEmitente|NomeDestRem|TotalNota" 
                                    TitulosColunas="IdNf|Número NF-e|Emitente|Destinatário|Total" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Imposto" SortExpression="TipoImposto">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("TipoImposto") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                 <asp:DropDownList ID="drpTipoImposto" runat="server" 
                                    SelectedValue='<%# Bind("TipoImposto") %>' DataSourceID="odsTipoImposto" 
                                    DataTextField="Descr" DataValueField="Id">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" 
                                    DataSourceID="odsTipoImposto" DataTextField="Descr" DataValueField="Id" 
                                    SelectedValue='<%# Bind("TipoImposto") %>'>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Outras Deduções" 
                            SortExpression="ValorOutDeducao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorOutDeducao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ValorOutDeducao") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ValorOutDeducao") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Multa" 
                            SortExpression="ValorMulta">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("ValorMulta") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("ValorMulta") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("ValorMulta") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Juros" 
                            SortExpression="ValorJuro">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("ValorJuro") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("ValorJuro") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("ValorJuro") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CausesValidation="True" CommandName="Update"
                                    Text="Atualizar" ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CausesValidation="True" CommandName="Insert"
                                    Text="Inserir" ValidationGroup="c" />
                                &nbsp;<asp:Button ID="btnCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle HorizontalAlign="Left" BackColor="White" />
                    <AlternatingRowStyle ForeColor="Black" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfoAdicNFEExtemp" runat="server" DataObjectTypeName="Glass.Data.Model.EFD.InfoAdicNFEExtemporanea"
                    DeleteMethod="Excluir" InsertMethod="Inserir" SelectMethod="Obter" TypeName="Glass.Data.DAL.EFD.InfoAdicNFEExtemporaneaDAO"
                    UpdateMethod="Atualizar"  
                    oninserted="odsInfoAdicNFEExtemp_Inserted" onupdated="odsInfoAdicNFEExtemp_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idInfoAdicNFEExtemporanea" QueryStringField="id" Type="Int32" />
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
    </table>
</asp:Content>

