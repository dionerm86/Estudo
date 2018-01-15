<%@ Page Title="Cadastro de Dedução Diversa" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadDeducaoDiversa.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadDeducaoDiversa" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvDeducaoDiversa" runat="server" AutoGenerateRows="False" DataSourceID="odsDeducaoDiversa"
                    DefaultMode="Insert" GridLines="None" Height="50px" Width="125px" DataKeyNames="IdDeducao"
                    CellPadding="4" Style="margin-right: 2px" OnItemCommand="dtvDeducaoDiversa_ItemCommand"
                    OnItemInserting="dtvDeducaoDiversa_ItemInserting" OnItemUpdating="dtvDeducaoDiversa_ItemUpdating">
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
                                    AppendDataBoundItems="True">
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
                        <asp:TemplateField HeaderText="Data" SortExpression="DataDeducao">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataDeducao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc2:ctrlData ID="ctrlDataDataDeducao" runat="server" ExibirHoras="False" ReadOnly="ReadWrite"
                                    Data='<%# Bind("DataDeducao") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc2:ctrlData ID="ctrlDataDataDeducao" runat="server" ExibirHoras="False" ReadOnly="ReadWrite"
                                    Data='<%# Bind("DataDeducao") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor PIS a Deduzir" SortExpression="ValorPisDeduzir">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("ValorPisDeduzir") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorPisDeduzir" onkeypress="return soNumeros(event, false, true);" runat="server"
                                    Text='<%# Bind("ValorPisDeduzir") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtValorPisDeduzir" onkeypress="return soNumeros(event, false, true);" runat="server"
                                    Text='<%# Bind("ValorPisDeduzir") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor COFINS a Deduzir" SortExpression="ValorCofinsDeduzir">
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("ValorCofinsDeduzir") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorCofinsDeduzir" onkeypress="return soNumeros(event, false, true);" runat="server"
                                    Text='<%# Bind("ValorCofinsDeduzir") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtValorCofinsDeduzir" onkeypress="return soNumeros(event, false, true);" runat="server"
                                    Text='<%# Bind("ValorCofinsDeduzir") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="BC Dedução" SortExpression="BcDeducao">
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("BcDeducao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtBcDeducao" onkeypress="return soNumeros(event, false, true);" runat="server"
                                    Text='<%# Bind("BcDeducao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtBcDeducao" onkeypress="return soNumeros(event, false, true);" runat="server"
                                    Text='<%# Bind("BcDeducao") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CNPJ da Dedutora" SortExpression="CnpjDedutora">
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("CnpjDedutora") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCnpjDedutora" runat="server" onkeypress="maskCNPJ(event, this);"
                                    Text='<%# Bind("CnpjDedutora") %>' MaxLength="18"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCnpjDedutora" runat="server" onkeypress="maskCNPJ(event, this);"
                                    Text='<%# Bind("CnpjDedutora") %>' MaxLength="18"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Informações Comp." SortExpression="InformacoesComplementares">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("InformacoesComplementares") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtInformacoesComplementares" runat="server" Text='<%# Bind("InformacoesComplementares") %>' TextMode="MultiLine"
                                    Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtInformacoesComplementares" runat="server" Text='<%# Bind("InformacoesComplementares") %>' TextMode="MultiLine"
                                    Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Origem da Dedução" SortExpression="OrigemDeducao">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("OrigemDeducao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupOrigemDeducao" runat="server" DataSourceID="odsOrigemDeducao"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("OrigemDeducaoString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione a origem da dedução" Valor='<%# Bind("OrigemDeducao") %>' TextWidth="200px"
                                    ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupOrigemDeducao" runat="server" DataSourceID="odsOrigemDeducao"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("OrigemDeducaoString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione a origem da dedução" Valor='<%# Bind("OrigemDeducao") %>' TextWidth="200px"
                                    ValidationGroup="c" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Natureza Dedução" SortExpression="NaturezaDeducao">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("NaturezaDeducao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupNaturezaDeducao" runat="server" DataSourceID="odsNaturezaDeducao"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("NaturezaDeducaoString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de natureza da dedução" Valor='<%# Bind("NaturezaDeducao") %>'
                                    TextWidth="200px" ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupNaturezaDeducao" runat="server" DataSourceID="odsNaturezaDeducao"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("NaturezaDeducaoString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de natureza da dedução" Valor='<%# Bind("NaturezaDeducao") %>'
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDeducaoDiversa" runat="server" DataObjectTypeName="Glass.Data.Model.DeducaoDiversa"
                    DeleteMethod="Delete" InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.DeducaoDiversaDAO"
                    UpdateMethod="Update" OnInserted="odsDeducaoDiversa_Inserted" OnUpdated="odsDeducaoDiversa_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idDeducao" QueryStringField="idDeducao" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNaturezaDeducao" runat="server" SelectMethod="GetNaturezaDeducao"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsOrigemDeducao" runat="server" SelectMethod="GetOrigemDeducao"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
