<%@ Page Title="Débitos de PIS/Cofins" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" Inherits="Glass.UI.Web.Listas.LstDebitosPisCofins" Codebehind="LstDebitosPisCofins.aspx.cs" %>

<%@ Register src="../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <asp:GridView ID="grdDebitosPisCofins" runat="server" AllowPaging="True" SkinID="gridViewEditable"
        DataKeyNames="IdDetalhamentoPisCofins" DataSourceID="odsDebitosPisCofins" 
        onrowcommand="grdDebitosPisCofins_RowCommand">
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" 
                        ImageUrl="~/Images/Ok.gif" />
                    <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" 
                        CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" Width="16px" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="imgEditar" runat="server" CausesValidation="False" 
                        CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                    <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" 
                        CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" 
                        onclientclick="if (!confirm(&quot;Deseja excluir esse débito?&quot;)) return false;" />
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Data Pagamento" SortExpression="DataPagamento">
                <EditItemTemplate>
                    <uc1:ctrlData ID="ctrlData" runat="server" Data='<%# Bind("DataPagamento") %>' 
                        ValidateEmptyText="True" />
                </EditItemTemplate>
                <FooterTemplate>
                    <uc1:ctrlData ID="ctrlData" runat="server" ValidateEmptyText="True" />
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" 
                        Text='<%# Bind("DataPagamento", "{0:d}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Imposto" SortExpression="TipoImposto">
                <EditItemTemplate>
                    <asp:DropDownList ID="drpTipoImposto" runat="server" 
                        DataSourceID="odsTipoImposto" DataTextField="Translation" DataValueField="Key" 
                        SelectedValue='<%# Bind("TipoImposto") %>' 
                        ondatabound="drpTipoImposto_DataBound">
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="drpTipoImposto" runat="server" 
                        DataSourceID="odsTipoImposto" DataTextField="Translation" DataValueField="Key" 
                        AppendDataBoundItems="True" ondatabound="drpTipoImposto_DataBound">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvTipoImposto" runat="server" 
                        ControlToValidate="drpTipoImposto" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("TipoImposto")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Código Receita" SortExpression="CodigoReceita">
                <EditItemTemplate>
                    <asp:TextBox ID="txtCodReceita" runat="server" 
                        Text='<%# Bind("CodigoReceita") %>' MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodReceita" runat="server" 
                        ControlToValidate="txtCodReceita" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtCodReceita" runat="server" MaxLength="20"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCodReceita" runat="server" 
                        ControlToValidate="txtCodReceita" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodigoReceita") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Contribuição Cumulativa?" SortExpression="Cumulativo">
                <EditItemTemplate>
                    <asp:CheckBox ID="chkCumulativo" runat="server" Checked='<%# Bind("Cumulativo") %>' />
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:CheckBox ID="chkCumulativo" runat="server" />
                </FooterTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chkCumulativo" runat="server" Enabled="False" Checked='<%# Bind("Cumulativo") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Valor Pagamento" SortExpression="ValorPagamento">
                <EditItemTemplate>
                    <asp:TextBox ID="txtValorPagto" runat="server" 
                        Text='<%# Bind("ValorPagamento") %>' Width="100px"
                        onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvValorPagto" runat="server" 
                        ControlToValidate="txtValorPagto" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtValorPagto" runat="server" 
                        onkeypress="return soNumeros(event, false, true)" 
                        Width="100px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvValorPagto" runat="server" 
                        ControlToValidate="txtValorPagto" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label5" runat="server" 
                        Text='<%# Bind("ValorPagamento", "{0:C}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <FooterTemplate>
                    <asp:ImageButton ID="imgInserir" runat="server" ImageUrl="~/Images/Insert.gif" 
                        onclick="imgInserir_Click" />
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle CssClass="pgr" />
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <colo:VirtualObjectDataSource runat="server" ID="odsDebitosPisCofins" Culture="pt-BR"
        DataObjectTypeName="Glass.Fiscal.Negocios.Entidades.DetalhamentoDebitosPisCofins" 
        SelectMethod="ObtemDebitosPisCofins"
        SelectByKeysMethod="ObtemDebitoPisCofins"
        TypeName="Glass.Fiscal.Negocios.IDetalhamentoDebitosPisCofinsFluxo" 
        DeleteMethod="ApagarDebitoPisCofins" 
        DeleteStrategy="GetAndDelete"
        EnablePaging="True"
        MaximumRowsParameterName="pageSize" 
        UpdateMethod="SalvarDebitoPisCofins"
        UpdateStrategy="GetAndUpdate"></colo:VirtualObjectDataSource>
    <colo:virtualobjectdatasource Culture="pt-BR" ID="odsTipoImposto" runat="server"
        SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
        <SelectParameters>
            <asp:Parameter Name="typeName" DefaultValue="Sync.Fiscal.Enumeracao.TipoImposto, Sync.Fiscal.Comum" />
        </SelectParameters>
    </colo:virtualobjectdatasource>
</asp:Content>

