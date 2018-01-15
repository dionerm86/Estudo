<%@ Page Title="Cadastro de Valor Retido na Fonte" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadValorRetidoFonte.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadValorRetidoFonte" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvValorRetidoFonte" runat="server" AutoGenerateRows="False"
                    DataSourceID="odsValorRetidoFonte" DefaultMode="Insert" GridLines="None" Height="50px"
                    Width="125px" DataKeyNames="IdValorRetidoFonte" CellPadding="4" Style="margin-right: 2px"
                    OnItemCommand="dtvValorRetidoFonte_ItemCommand" OnItemInserting="dtvValorRetidoFonte_ItemInserting"
                    OnItemUpdating="dtvValorRetidoFonte_ItemUpdating">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle CssClass="dtvAlternatingRow" />
                    <FieldHeaderStyle Wrap="False" Font-Bold="False" CssClass="dtvHeader" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdLoja") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlLoja" Width="200px" runat="server" DataSourceID="odsLoja"
                                    DataTextField="NomeFantasia" DataValueField="IdLoja" SelectedValue='<%# Bind("IdLoja") %>'
                                    AppendDataBoundItems="True" Height="16px">
                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlLoja"
                                    ErrorMessage="Selecione uma loja" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlLoja" Width="200px" runat="server" DataSourceID="odsLoja"
                                    DataTextField="NomeFantasia" DataValueField="IdLoja" SelectedValue='<%# Bind("IdLoja") %>'
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlLoja"
                                    ErrorMessage="Selecione uma loja" SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="DataRetencao">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataRetencao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc2:ctrlData ID="ctrlDataDataRetencao" runat="server" ExibirHoras="False" ReadOnly="ReadWrite"
                                    Data='<%# Bind("DataRetencao") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc2:ctrlData ID="ctrlDataDataRetencao" runat="server" ExibirHoras="False" ReadOnly="ReadWrite"
                                    Data='<%# Bind("DataRetencao") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Retido" SortExpression="ValorRetido">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("ValorRetido") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorRetido" onkeypress="return soNumeros(event, false, true);"
                                    runat="server" Text='<%# Bind("ValorRetido") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtValorRetido" onkeypress="return soNumeros(event, false, true);"
                                    runat="server" Text='<%# Bind("ValorRetido") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor PIS Retido" SortExpression="ValorPisRetido">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("ValorPisRetido") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorPisRetido" onkeypress="return soNumeros(event, false, true);"
                                    runat="server" Text='<%# Bind("ValorPisRetido") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtValorPisRetido" onkeypress="return soNumeros(event, false, true);"
                                    runat="server" Text='<%# Bind("ValorPisRetido") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor COFINS Retido" SortExpression="ValorCofinsRetido">
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("ValorCofinsRetido") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorCofinsRetido" onkeypress="return soNumeros(event, false, true);"
                                    runat="server" Text='<%# Bind("ValorCofinsRetido") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtValorCofinsRetido" onkeypress="return soNumeros(event, false, true);"
                                    runat="server" Text='<%# Bind("ValorCofinsRetido") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="BC Retenção" SortExpression="BcRetencao">
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("BcRetencao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtBcRetencao" onkeypress="return soNumeros(event, false, true);"
                                    runat="server" Text='<%# Bind("BcRetencao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtBcRetencao" onkeypress="return soNumeros(event, false, true);"
                                    runat="server" Text='<%# Bind("BcRetencao") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CNPJ da Retentora" SortExpression="CnpjRetentora">
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("CnpjRetentora") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCnpjRetentora" runat="server" onkeypress="maskCNPJ(event, this);"
                                    Text='<%# Bind("CnpjRetentora") %>' MaxLength="18"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCnpjRetentora" runat="server" onkeypress="maskCNPJ(event, this);"
                                    Text='<%# Bind("CnpjRetentora") %>' MaxLength="18"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Receita" SortExpression="CodigoReceita">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("CodigoReceita") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodigoReceita" MaxLength="4" runat="server" Text='<%# Bind("CodigoReceita") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCodigoReceita" MaxLength="4" runat="server" Text='<%# Bind("CodigoReceita") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Natureza da Retenção" SortExpression="NaturezaRetencao">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("NaturezaRetencao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupNaturezaRetencao" runat="server" DataSourceID="odsNaturezaRetencao"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("NaturezaRetencaoString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione a natureza da retenção" Valor='<%# Bind("NaturezaRetencao") %>'
                                    TextWidth="200px" ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupNaturezaRetencao" runat="server" DataSourceID="odsNaturezaRetencao"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("NaturezaRetencaoString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione a natureza da retenção" Valor='<%# Bind("NaturezaRetencao") %>'
                                    TextWidth="200px" ValidationGroup="c" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Natureza Receita" SortExpression="NaturezaReceita">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("NaturezaReceita") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupNaturezaReceita" runat="server" DataSourceID="odsNaturezaReceita"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("NaturezaReceitaString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione a natureza da receita" Valor='<%# Bind("NaturezaReceita") %>'
                                    TextWidth="200px" ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupNaturezaReceita" runat="server" DataSourceID="odsNaturezaReceita"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("NaturezaReceitaString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione a natureza da receita" Valor='<%# Bind("NaturezaReceita") %>'
                                    TextWidth="200px" ValidationGroup="c" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Declarante" SortExpression="TipoDeclarante">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("TipoDeclarante") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupTipoDeclarante" runat="server" DataSourceID="odsTipoDeclarante"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("TipoDeclaranteString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione o tipo de declarante" Valor='<%# Bind("TipoDeclarante") %>'
                                    TextWidth="200px" ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupTipoDeclarante" runat="server" DataSourceID="odsTipoDeclarante"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("TipoDeclaranteString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione o tipo de declarante" Valor='<%# Bind("TipoDeclarante") %>'
                                    TextWidth="200px" ValidationGroup="c" />
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsValorRetidoFonte" runat="server" DataObjectTypeName="Glass.Data.Model.ValorRetidoFonte"
                    DeleteMethod="Delete" InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.ValorRetidoFonteDAO"
                    UpdateMethod="Update" OnInserted="odsValorRetidoFonte_Inserted" OnUpdated="odsValorRetidoFonte_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="IdValorRetidoFonte" QueryStringField="IdValorRetidoFonte"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNaturezaRetencao" runat="server" SelectMethod="GetNaturezaRetencao"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNaturezaReceita" runat="server" SelectMethod="GetNaturezaReceita"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoDeclarante" runat="server" SelectMethod="GetTipoDeclarante"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
