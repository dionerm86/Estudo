<%@ Page Title="Cadastro de Informações Adicionais de Crédito" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadInfoAdicCredito.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadInfoAdicCredito" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvInfoAdicCredito" runat="server" 
                    AutoGenerateRows="False" DataSourceID="odsInfoAdicCredito"
                    DefaultMode="Insert" GridLines="None" Height="50px" Width="125px" CellPadding="4"
                    Style="margin-right: 2px" DataKeyNames="IdInfoAdicCredito" 
                    onitemcommand="dtvInfoAdicCredito_ItemCommand" 
                    oniteminserting="dtvInfoAdicCredito_ItemInserting" 
                    onitemupdating="dtvInfoAdicCredito_ItemUpdating" >
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle CssClass="dtvAlternatingRow" />
                    <FieldHeaderStyle Wrap="False" Font-Bold="False" CssClass="dtvHeader" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Cód. Crédito" SortExpression="CodCred">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodCred") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                  <uc1:ctrlSelPopup ID="ctrlSelCodCred" runat="server" DataSourceID="odsCodCred" 
                                    DataTextField="Descr" DataValueField="Id" 
                                    Descricao='<%# Eval("DescrCodCred") %>' PermitirVazio="False" TextWidth="250px" 
                                    TituloTela="Selecione o Cód. Crédito" Valor='<%# Bind("CodCred") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelCodCred" runat="server" DataSourceID="odsCodCred" 
                                    DataTextField="Descr" DataValueField="Id" 
                                    Descricao='<%# Eval("DescrCodCred") %>' PermitirVazio="False" TextWidth="250px" 
                                    TituloTela="Selecione o Cód. Crédito" Valor='<%# Bind("CodCred") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Período" SortExpression="Periodo">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Periodo") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPeriodoEdit" runat="server" OnKeypress="return mascara_periodo(event, this);" Text='<%# Bind("Periodo") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtPeriodoInsert" runat="server" OnKeyDown="return mascara_periodo(event, this);" Text='<%# Bind("Periodo") %>'></asp:TextBox>
                            </InsertItemTemplate>
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
                        <asp:TemplateField HeaderText="Valor Créd. Per. Res. Ant." 
                            SortExpression="ValorCredPerResAnt">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorCredPerResAnt") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ValorCredPerResAnt") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ValorCredPerResAnt") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Créd. Decl. Comp. Ant." 
                            SortExpression="ValorCredDeclCompAnt">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("ValorCredDeclCompAnt") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("ValorCredDeclCompAnt") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("ValorCredDeclCompAnt") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Créd. Desc. Ant." 
                            SortExpression="ValorCredDescAnt">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("ValorCredDescAnt") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("ValorCredDescAnt") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("ValorCredDescAnt") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Créd. Per. Res." 
                            SortExpression="ValorCredPerRes">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("ValorCredPerRes") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("ValorCredPerRes") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("ValorCredPerRes") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Créd. Decl. Comp." 
                            SortExpression="ValorCredDeclComp">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("ValorCredDeclComp") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("ValorCredDeclComp") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("ValorCredDeclComp") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Créd. Transf." 
                            SortExpression="ValorCredTransf">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("ValorCredTransf") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("ValorCredTransf") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("ValorCredTransf") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Créd. Outros" 
                            SortExpression="ValorCredOutro">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("ValorCredOutro") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("ValorCredOutro") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("ValorCredOutro") %>' OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfoAdicCredito" runat="server" DataObjectTypeName="Glass.Data.Model.EFD.InfoAdicCredito"
                    DeleteMethod="Excluir" InsertMethod="Inserir" SelectMethod="Obter" TypeName="Glass.Data.DAL.InfoAdicCreditoDAO"
                    UpdateMethod="Atualizar"  
                    oninserted="odsInfoAdicCredito_Inserted" onupdated="odsInfoAdicCredito_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idInfoAdicCredito" QueryStringField="id" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                  <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodCred" runat="server" SelectMethod="GetCodCred" 
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" 
                    SelectMethod="GetTipoImposto" TypeName="Glass.Data.EFD.DataSourcesEFD" 
                    >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
