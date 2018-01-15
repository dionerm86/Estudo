<%@ Page Title="Cadastro de Receita Diversa" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadReceitaDiversa.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadReceitaDiversa" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvReceitaDiversa" runat="server" AutoGenerateRows="False" DataSourceID="odsReceitaDiversa"
                    DefaultMode="Insert" GridLines="None" Height="50px" Width="125px" DataKeyNames="IdReceita"
                    CellPadding="4" Style="margin-right: 2px" 
                    onitemcommand="dtvReceitaDiversa_ItemCommand" 
                    oniteminserting="dtvReceitaDiversa_ItemInserting" 
                    onitemupdating="dtvReceitaDiversa_ItemUpdating">
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
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                    ControlToValidate="ddlLoja" ErrorMessage="Selecione uma loja" 
                                    SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlLoja" Width="200px" runat="server" DataSourceID="odsLoja"
                                    DataTextField="NomeFantasia" DataValueField="IdLoja" SelectedValue='<%# Bind("IdLoja") %>'
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                    ControlToValidate="ddlLoja" ErrorMessage="Selecione uma loja" 
                                    SetFocusOnError="True" ValidationGroup="c">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="DataReceita">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataReceita") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc2:ctrlData ID="ctrlDataDataReceita" runat="server" ExibirHoras="False" ReadOnly="ReadWrite"
                                    Data='<%# Bind("DataReceita") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc2:ctrlData ID="ctrlDataDataReceita" runat="server" ExibirHoras="False" ReadOnly="ReadWrite"
                                    Data='<%# Bind("DataReceita") %>' />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorReceita">
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("ValorReceita") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("ValorReceita") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox1" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("ValorReceita") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="BC PIS" SortExpression="BcPis">
                            <ItemTemplate>
                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("BcPis") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("BcPis") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox2" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("BcPis") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alíquota PIS" SortExpression="AliquotaPis">
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("AliquotaPis") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("AliquotaPis") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox3" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("AliquotaPis") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="BC COFINS" SortExpression="BcCofins">
                            <ItemTemplate>
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("BcCofins") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("BcCofins") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox4" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("BcCofins") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alíquota COFINS" SortExpression="AliquotaCofins">
                            <ItemTemplate>
                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("AliquotaCofins") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("AliquotaCofins") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox5" onkeypress="return soNumeros(event, false, true);" runat="server" Text='<%# Bind("AliquotaCofins") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta Contábil" SortExpression="IdContaContabil">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("IdContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlContaContabil" runat="server" DataSourceID="odsContaContabil"
                                    DataTextField="Descricao" DataValueField="IdContaContabil" SelectedValue='<%# Bind("IdContaContabil") %>'
                                    Width="200px" AppendDataBoundItems="True">
                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlContaContabil" runat="server" DataSourceID="odsContaContabil"
                                    DataTextField="Descricao" DataValueField="IdContaContabil" SelectedValue='<%# Bind("IdContaContabil") %>'
                                    Width="200px" AppendDataBoundItems="True">
                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Centro de Custo" SortExpression="IdCentroCusto">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("IdCentroCusto") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlCentroCusto" runat="server" DataSourceID="odsCentroCusto"
                                    DataTextField="Descricao" DataValueField="IdCentroCusto" SelectedValue='<%# Bind("IdCentroCusto") %>'
                                    Width="200px" AppendDataBoundItems="True">
                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="ddlCentroCusto" runat="server" DataSourceID="odsCentroCusto"
                                    DataTextField="Descricao" DataValueField="IdCentroCusto" SelectedValue='<%# Bind("IdCentroCusto") %>'
                                    Width="200px" AppendDataBoundItems="True">
                                    <asp:ListItem Value="">Selecione</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>' 
                                    TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>' 
                                    TextMode="MultiLine" Width="300px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Receita" SortExpression="TipoReceita">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("TipoReceita") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopup1" runat="server" DataSourceID="odsTipoReceita"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("TipoReceitaString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de receita" 
                                    Valor='<%# Bind("TipoReceita") %>' TextWidth="200px" ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopup1" runat="server" DataSourceID="odsTipoReceita"
                                    PermitirVazio="False" DataTextField="Descr" FazerPostBackBotaoPesquisar="false"
                                    DataValueField="Id" Descricao='<%# Eval("TipoReceitaString") %>' ExibirIdPopup="False"
                                    TituloTela="Selecione o tipo de receita" 
                                    Valor='<%# Bind("TipoReceita") %>' TextWidth="200px" ValidationGroup="c" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Operação" SortExpression="TipoOperacao">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("TipoOperacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupTipoOperacao" runat="server" DataSourceID="odsTipoOperacao"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("TipoOperacaoString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de operação" Valor='<%# Bind("TipoOperacao") %>'
                                    ValidationGroup="c"  TextWidth="200px" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupTipoOperacao" runat="server" DataSourceID="odsTipoOperacao"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("TipoOperacaoString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de operação" Valor='<%# Bind("TipoOperacao") %>' TextWidth="200px" ValidationGroup="c"/>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CST PIS" SortExpression="CstPis">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("CstPis") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupCstPis" runat="server" DataSourceID="odsCstPis"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("CstPiSString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de Cst Pis" Valor='<%# Bind("CstPis") %>' TextWidth="200px" ValidationGroup="c"/>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupCstPis" runat="server" DataSourceID="odsCstPis"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("CstPiSString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de Cst Pis" Valor='<%# Bind("CstPis") %>'  TextWidth="200px" ValidationGroup="c" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CST COFINS" SortExpression="CstCofins">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CstCofins") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupCstCofins" runat="server" DataSourceID="odsCstCofins"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("CstCofinsString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de Cst Cofins" Valor='<%# Bind("CstCofins") %>' TextWidth="200px" ValidationGroup="c"/>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupCstCofins" runat="server" DataSourceID="odsCstCofins"
                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("CstCofinsString") %>'
                                    ExibirIdPopup="False" FazerPostBackBotaoPesquisar="false" PermitirVazio="False"
                                    TituloTela="Selecione o tipo de Cst Cofins" Valor='<%# Bind("CstCofins") %>' TextWidth="200px" ValidationGroup="c"/>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Natureza BC Créd." SortExpression="NaturezaBcCredito">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("NaturezaBcCredito") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupNaturezaBcCredito" runat="server" 
                                    DataSourceID="odsNaturezaBcCredito" DataTextField="Descr" DataValueField="Id" 
                                    Descricao='<%# Eval("NaturezaBcCreditoString") %>' ExibirIdPopup="False" 
                                    FazerPostBackBotaoPesquisar="false" PermitirVazio="False" 
                                    TituloTela="Selecione o tipo de natureza bc crédito" Valor='<%# Bind("NaturezaBcCredito") %>' TextWidth="200px" ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                  <uc1:ctrlSelPopup ID="ctrlSelPopupNaturezaBcCredito" runat="server" 
                                    DataSourceID="odsNaturezaBcCredito" DataTextField="Descr" DataValueField="Id" 
                                    Descricao='<%# Eval("NaturezaBcCreditoString") %>' ExibirIdPopup="False" 
                                    FazerPostBackBotaoPesquisar="false" PermitirVazio="False" 
                                    TituloTela="Selecione o tipo de natureza bc crédito" Valor='<%# Bind("NaturezaBcCredito") %>' TextWidth="200px" ValidationGroup="c" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ind. Origem Créd." SortExpression="IndOrigemCred">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("IndOrigemCred") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                  <uc1:ctrlSelPopup ID="ctrlSelPopupIndOrigemCred" runat="server" 
                                    DataSourceID="odsIndOrigemCred" DataTextField="Descr" DataValueField="Id" 
                                    Descricao='<%# Eval("IndOrigemCredString") %>' ExibirIdPopup="False" 
                                    FazerPostBackBotaoPesquisar="false" PermitirVazio="False" 
                                    TituloTela="Selecione o tipo de ind. origem crédito" Valor='<%# Bind("IndOrigemCred") %>'  TextWidth="200px" ValidationGroup="c" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <uc1:ctrlSelPopup ID="ctrlSelPopupIndOrigemCred" runat="server" 
                                    DataSourceID="odsIndOrigemCred" DataTextField="Descr" DataValueField="Id" 
                                    Descricao='<%# Eval("IndOrigemCredString") %>' ExibirIdPopup="False" 
                                    FazerPostBackBotaoPesquisar="false" PermitirVazio="False" 
                                    TituloTela="Selecione o tipo de ind. origem crédito" Valor='<%# Bind("IndOrigemCred") %>' TextWidth="200px" ValidationGroup="c" />
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CausesValidation="True" 
                                    CommandName="Update" Text="Atualizar" ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CausesValidation="True" 
                                    CommandName="Insert" Text="Inserir" ValidationGroup="c" />
                                &nbsp;<asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle HorizontalAlign="Left" BackColor="White" />
                    <AlternatingRowStyle ForeColor="Black" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsReceitaDiversa" runat="server" DataObjectTypeName="Glass.Data.Model.ReceitaDiversa"
                    DeleteMethod="Delete" InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.ReceitaDiversaDAO"
                    UpdateMethod="Update" oninserted="odsReceitaDiversa_Inserted" 
                    onupdated="odsReceitaDiversa_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idReceita" QueryStringField="idReceita" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoReceita" runat="server" SelectMethod="GetTipoReceita"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoOperacao" runat="server" SelectMethod="GetTipoOperacao"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstPis" runat="server" SelectMethod="GetCstPisCofins"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstCofins" runat="server" SelectMethod="GetCstPisCofins"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaContabil" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.PlanoContaContabilDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCentroCusto" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CentroCustoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNaturezaBcCredito" runat="server" SelectMethod="GetNaturezaBcCredito"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
                    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsIndOrigemCred" runat="server" SelectMethod="GetIndOrigemCred"
                    TypeName="Glass.Data.EFD.DataSourcesEFD"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
