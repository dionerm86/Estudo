<%@ Page Title="Informações de Diferimento" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstInfoDiferimento.aspx.cs" Inherits="Glass.UI.Web.Listas.LstInfoDiferimento" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdInfoDifer" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsInfoDifer"
                    GridLines="None" DataKeyNames="IdInfoDiferimento" EmptyDataText="Não há informações de diferimento cadastradas"
                    ShowFooter="True" OnDataBound="grdInfoDifer_DataBound" OnRowCommand="grdInfoDifer_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="lnkEditar" runat="server" CommandName="Update" ImageUrl="~/Images/Ok.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="lnkEditar" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    CommandName="Edit" CausesValidation="False"></asp:ImageButton>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir essa informação de diferimento?&quot;)) return false"
                                    CausesValidation="False" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CNPJ" SortExpression="Cnpj">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCnpj" runat="server" Text='<%# Bind("Cnpj") %>' MaxLength="18"
                                    onkeypress="return soNumeros(event, true, true)" onkeydown="return maskCNPJ(event, this)"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCnpj" runat="server" ControlToValidate="txtCnpj"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="ctvCnpj" runat="server" ClientValidationFunction="validarCnpj"
                                    ControlToValidate="txtCnpj" Display="Dynamic" ErrorMessage="CNPJ inválido"></asp:CustomValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCnpj" runat="server" onkeydown="return maskCNPJ(event, this)"
                                    MaxLength="18" onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("Cnpj") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCnpj" runat="server" ControlToValidate="txtCnpj"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="ctvCnpj" runat="server" ClientValidationFunction="validarCnpj"
                                    ControlToValidate="txtCnpj" Display="Dynamic" ErrorMessage="CNPJ inválido"></asp:CustomValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("Cnpj") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Diferimento" SortExpression="Data">
                            <EditItemTemplate>
                                <uc2:ctrlData ID="ctrlData" runat="server" Data='<%# Bind("Data") %>' ValidateEmptyText="True"
                                    ReadOnly="ReadWrite" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc2:ctrlData ID="ctrlData" runat="server" Data='<%# Bind("Data") %>' ValidateEmptyText="True"
                                    ReadOnly="ReadWrite" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("Data", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Imposto" SortExpression="TipoImposto">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" SelectedValue='<%# Bind("TipoImposto") %>'
                                    DataSourceID="odsTipoImposto" DataTextField="Descr" DataValueField="Id">
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipoImposto" runat="server" DataSourceID="odsTipoImposto"
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoImposto") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DescrTipoImposto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Alíq. Imposto" SortExpression="AliqImpostoString">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAliqImposto" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("AliqImpostoString") %>' Width="70px"></asp:TextBox>
                                %
                                <asp:RequiredFieldValidator ID="rfvAliqImposto" runat="server" ControlToValidate="txtAliqImposto"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAliqImposto" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("AliqImpostoString") %>' Width="70px"></asp:TextBox>
                                %
                                <asp:RequiredFieldValidator ID="rfvAliqImposto" runat="server" ControlToValidate="txtAliqImposto"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("AliqImposto", "{0:0.##}") %>'></asp:Label>
                                %
                            </ItemTemplate>
                            <FooterStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Recebido" SortExpression="ValorRecebido">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorRecebido" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorRecebidoString") %>' Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValorRecebido" runat="server" ControlToValidate="txtValorRecebido"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorRecebido" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorRecebidoString") %>' Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValorRecebido" runat="server" ControlToValidate="txtValorRecebido"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("ValorRecebido", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor não Recebido" SortExpression="ValorNaoRecebido">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorNaoRecebido" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorNaoRecebidoString") %>' Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValorNaoRecebido" runat="server" ControlToValidate="txtValorNaoRecebido"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorNaoRecebido" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorNaoRecebidoString") %>' Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValorNaoRecebido" runat="server" ControlToValidate="txtValorNaoRecebido"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("ValorNaoRecebido", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Crédito" SortExpression="CodCred">
                            <EditItemTemplate>
                                <span style="position: relative; left: 2px">
                                    <uc1:ctrlSelPopup ID="ctrlSelCodCred" runat="server" DataSourceID="odsCodCred" DataTextField="Descr"
                                        DataValueField="Id" Descricao='<%# Eval("DescrCodCred") %>' PermitirVazio="False"
                                        TextWidth="250px" TituloTela="Selecione o Cód. Crédito" Valor='<%# Bind("CodCred") %>' />
                                </span>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <span style="position: relative; left: 2px">
                                    <uc1:ctrlSelPopup ID="ctrlSelCodCred" runat="server" DataSourceID="odsCodCred" DataTextField="Descr"
                                        DataValueField="Id" Descricao='<%# Eval("DescrCodCred") %>' PermitirVazio="False"
                                        TextWidth="250px" TituloTela="Selecione o Cód. Crédito" Valor='<%# Bind("CodCred") %>' />
                                </span>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrCodCred") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Crédito" SortExpression="ValorCreditoString">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorCredito" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorCreditoString") %>' Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValorCredito" runat="server" ControlToValidate="txtValorCredito"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorCredito" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorCreditoString") %>' Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValorCredito" runat="server" ControlToValidate="txtValorCredito"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorCredito", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Contribuição" SortExpression="DescrCodCont">
                            <EditItemTemplate>
                                <span style="position: relative; left: 2px">
                                    <uc1:ctrlSelPopup ID="ctrlSelCodCont" runat="server" DataSourceID="odsCodCont" DataTextField="Descr"
                                        DataValueField="Id" Descricao='<%# Eval("DescrCodCont") %>' PermitirVazio="False"
                                        TextWidth="250px" TituloTela="Selecione o Cód. Contribuição" Valor='<%# Bind("CodCont") %>' />
                                </span>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <span style="position: relative; left: 2px">
                                    <uc1:ctrlSelPopup ID="ctrlSelCodCont" runat="server" DataSourceID="odsCodCont" DataTextField="Descr"
                                        DataValueField="Id" Descricao='<%# Eval("DescrCodCont") %>' PermitirVazio="False"
                                        TextWidth="250px" TituloTela="Selecione o Cód. Contribuição" Valor='<%# Bind("CodCont") %>' />
                                </span>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrCodCont") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Contribuição" SortExpression="ValorContribuicaoString">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorCont" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorContribuicaoString") %>' Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValorContribuicao" runat="server" ControlToValidate="txtValorCont"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorCont" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorContribuicaoString") %>' Width="100px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValorContribuicao" runat="server" ControlToValidate="txtValorCont"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ValorContribuicao", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgInserir" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgInserir_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfoDifer" runat="server" DataObjectTypeName="Glass.Data.Model.InfoDiferimento"
                    DeleteMethod="Delete" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.InfoDiferimentoDAO" EnablePaging="True" 
                    UpdateMethod="Update">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodCred" runat="server" SelectMethod="GetCodCred" TypeName="Glass.Data.EFD.DataSourcesEFD"
                    >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodCont" runat="server" SelectMethod="GetCodCont" TypeName="Glass.Data.EFD.DataSourcesEFD"
                    >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoImposto" runat="server" SelectMethod="GetTipoImposto"
                    TypeName="Glass.Data.EFD.DataSourcesEFD" >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
