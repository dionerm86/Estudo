<%@ Page Title="Cadastro de Conta Bancária" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadContaBanco.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadContaBanco" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">

    function onSave() {
        var banco = FindControl("ddlBanco", "select");

        // Algumas empresas não utilizarão este campo
        /*if (banco.selectedIndex == 0) {
            alert("Selecione um banco.");
            return false;
        }*/
    }

</script>
    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvContaBanco" runat="server" SkinID="defaultDetailsView"
                    DataSourceID="odsContaBanco" DataKeyNames="IdContaBanco">
                    <Fields>
                        <asp:TemplateField HeaderText="Loja" SortExpression="IdLoja">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdLoja") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                    DataTextField="Name" DataValueField="Id" 
                                    SelectedValue='<%# Bind("IdLoja") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                    DataTextField="Name" DataValueField="Id" 
                                    SelectedValue='<%# Bind("IdLoja") %>'>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Nome">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Nome") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlBanco" runat="server" 
                                    AppendDataBoundItems="True" DataSourceID="odsListaBancos" DataTextField="Name" 
                                    DataValueField="Id" 
                                    SelectedValue='<%# Bind("CodBanco") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                    <asp:DropDownList ID="ddlBanco" runat="server"
                                        DataSourceID="odsListaBancos" DataTextField="Name" 
                                        DataValueField="Id" AppendDataBoundItems="True"
                                        SelectedValue='<%# Bind("CodBanco") %>'>
                                        <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Nome">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("CodBanco") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="45"
                                    Text='<%# Bind("Nome") %>' Width="80px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="45" 
                                    Text='<%# Bind("Nome") %>' Width="80px"></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Agência" SortExpression="Agencia">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Agencia") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAgencia" runat="server" Text='<%# Bind("Agencia") %>' 
                                    MaxLength="15" Width="80px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="rqdAgencia" runat="server" 
                                    ControlToValidate="txtAgencia" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtAgencia" runat="server" MaxLength="15" 
                                    Text='<%# Bind("Agencia") %>' Width="80px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="rqdAgencia" runat="server" 
                                    ControlToValidate="txtAgencia" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta" SortExpression="Conta">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Conta") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtConta" runat="server" Text='<%# Bind("Conta") %>' 
                                    MaxLength="15"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="rqdConta" runat="server" 
                                    ControlToValidate="txtConta" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtConta" runat="server" MaxLength="15" 
                                    Text='<%# Bind("Conta") %>'></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="rqdConta" runat="server" 
                                    ControlToValidate="txtConta" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular" SortExpression="Titular">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitular" runat="server" MaxLength="45" Text='<%# Bind("Titular") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rqdTitular" runat="server" 
                                    ControlToValidate="txtTitular" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtTitular" runat="server" MaxLength="45" Text='<%# Bind("Titular") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rqdTitular" runat="server" 
                                    ControlToValidate="txtTitular" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Titular") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Convênio" SortExpression="CodConvenio">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CodConvenio") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" MaxLength="20" 
                                    onkeypress="return soNumeros(event, true, true)" 
                                    Text='<%# Bind("CodConvenio") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" MaxLength="20" 
                                    onkeypress="return soNumeros(event, true, true)" 
                                    Text='<%# Bind("CodConvenio") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <HeaderStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Posto / Variação" SortExpression="Posto">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("Posto") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Posto") %>'
                                    onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. do Cliente" SortExpression="CodCliente">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodCliente" runat="server" Text='<%# Bind("CodCliente") %>' 
                                    MaxLength="15"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtCodCliente" runat="server" MaxLength="15" 
                                    Text='<%# Bind("CodCliente") %>'></asp:TextBox>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativa</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativa</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativa</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativa</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnUpdate" runat="server" CommandName="Update" OnClientClick="return onSave();"  
                                    Text="Atualizar" />
                                <asp:Button ID="Button2" runat="server" onclick="btnCancelar_Click" 
                                    Text="Cancelar" CausesValidation="False" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onSave();" 
                                    ValidationGroup="c" />
                                <asp:Button ID="btnCancelar" runat="server" onclick="btnCancelar_Click" 
                                    Text="Cancelar" CausesValidation="False" />
                            </InsertItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Wrap="False" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" 
                    DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.ContaBanco" 
                    InsertMethod="SalvarContaBanco" 
                    SelectMethod="ObtemContaBanco" 
                    TypeName="Glass.Financeiro.Negocios.IContaBancariaFluxo"
                    UpdateMethod="SalvarContaBanco"
                    UpdateStrategy="GetAndUpdate">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idContaBanco" QueryStringField="idContaBanco" 
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" 
                    SelectMethod="ObtemLojas" 
                    TypeName="Glass.Global.Negocios.ILojaFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsListaBancos" runat="server" 
                    SelectMethod="ObtemBancos" TypeName="Glass.Financeiro.Negocios.IContaBancariaFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

