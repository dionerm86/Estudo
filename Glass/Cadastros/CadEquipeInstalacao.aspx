<%@ Page Title="Cadastro de Equipe" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="CadEquipeInstalacao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadEquipeInstalacao" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    function setColocador(idFunc, idTipoFunc, selColocador) {
        var response = CadEquipeInstalacao.InsereColocador(FindControl("hdfIdEquipe", "input").value, FindControl("hdfTipoEquipe", "input").value, idFunc, idTipoFunc).value;

        if (response == "ok") {
            var formObj = getForm();

            if (formObj != null)
                cOnClick('btnReload', null);
        }
        else {
            selColocador.alert(response);
        }
    }
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvEquipe" runat="server" AutoGenerateRows="False" DataKeyNames="IdEquipe"
                    DataSourceID="odsEquipe" DefaultMode="Insert" GridLines="None" Height="50px"
                    Width="125px" CellPadding="4" ForeColor="#333333">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="Black" />
                    <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Nome" SortExpression="Nome">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                    Width="200px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="rqdNome" runat="server" ControlToValidate="txtNome"
                                    ErrorMessage="Informe o Nome da Equipe" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="50" Text='<%# Bind("Nome") %>'
                                    Width="200px"></asp:TextBox>
                                &nbsp;<asp:RequiredFieldValidator ID="rqdNome" runat="server" ControlToValidate="txtNome"
                                    ErrorMessage="Informe o Nome da Equipe" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Nome") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situacao" SortExpression="Situacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="1">Ativa</asp:ListItem>
                                    <asp:ListItem Value="2">Inativa</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="1">Ativa</asp:ListItem>
                                    <asp:ListItem Value="2">Inativa</asp:ListItem>
                                </asp:DropDownList>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="Tipo">
                            <EditItemTemplate>
                                <asp:Label ID="lblDescrTipo" runat="server" Text='<%# Eval("DescrTipo") %>'></asp:Label>
                                <asp:HiddenField ID="hdfTipoEquipe" runat="server" Value='<%# Bind("Tipo") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" SelectedValue='<%# Bind("Tipo") %>'>
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem Value="1">Colocação Comum</asp:ListItem>
                                    <asp:ListItem Value="2">Colocação Temperado</asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="drpTipo"
                                    ErrorMessage="Informe o Tipo da Equipe" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Tipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Veículo" SortExpression="Placa">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpVeiculo" runat="server" DataSourceID="odsVeiculo" DataTextField="DescricaoCompleta"
                                    DataValueField="Placa" SelectedValue='<%# Bind("Placa") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:DropDownList ID="drpVeiculo" runat="server" AppendDataBoundItems="True" DataSourceID="odsVeiculo"
                                    DataTextField="DescricaoCompleta" DataValueField="Placa" SelectedValue='<%# Bind("Placa") %>'>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="drpVeiculo"
                                    ErrorMessage="Informe o Veículo da Equipe" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Placa") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Login" SortExpression="Login">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Login") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Login") %>'></asp:TextBox>
                                <asp:HiddenField ID="hdfSenha" runat="server" Value='<%# Bind("Senha") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="TextBox1" runat="server" MaxLength="20" Text='<%# Bind("Login") %>'></asp:TextBox>
                                        </td>
                                        <td class="dtvHeader">
                                            Senha
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox2" runat="server" MaxLength="20" Text='<%# Bind("Senha") %>'
                                                TextMode="Password"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar" />
                                <asp:Button ID="btnAlterarSenha" runat="server" Text="Alterar Senha" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Voltar"
                                    CausesValidation="false" />
                                <uc2:ctrllinkquerystring ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdLoja"
                                    Text='<%# Bind("IdEquipe") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" />
                                <asp:Button ID="btnCancelar0" runat="server" OnClick="btnCancelar_Click" Text="Cancelar"
                                    CausesValidation="false" />
                            </InsertItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle HorizontalAlign="Left" BackColor="White" />
                    <AlternatingRowStyle BackColor="White" ForeColor="Black" />
                </asp:DetailsView>
                <asp:ValidationSummary ID="vlsMsg" runat="server" DisplayMode="List" ShowMessageBox="True"
                    ShowSummary="False" />
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkIncluirIntegrante" OnClientClick="openWindow(500, 700, '../Utils/SelColocador.aspx'); return false;"
                    runat="server" Visible="False">Incluir Integrante</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdFuncEquipe" runat="server" 
                    AutoGenerateColumns="False" DataKeyNames="Idfunc,IdEquipe"
                    DataSourceID="odsFuncEquipe" EmptyDataText="Inclua os integrantes da Equipe."
                    Visible="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja retirar este Funcionário desta Equipe?&quot;);" />
                            </ItemTemplate>
                            <HeaderStyle Wrap="False" />
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="TipoFunc" HeaderText="Competência" SortExpression="TipoFunc" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" DataObjectTypeName="Glass.Data.Model.Equipe"
                    InsertMethod="Insert" OnInserted="odsEquipe_Inserted" OnUpdated="odsEquipe_Updated"
                    SelectMethod="GetElement" TypeName="Glass.Data.DAL.EquipeDAO" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idEquipe" QueryStringField="idEquipe" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVeiculo" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.VeiculoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncEquipe" runat="server" DataObjectTypeName="Glass.Data.Model.FuncEquipe"
                    DeleteMethod="Delete" SelectMethod="GetByEquipe" TypeName="Glass.Data.DAL.FuncEquipeDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idEquipe" QueryStringField="idEquipe" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdEquipe" runat="server" />
                <asp:Button ID="btnReload" runat="server" OnClick="btnReload_Click" Text="Reload"
                    Style="display: none;" />
            </td>
        </tr>
    </table>
</asp:Content>
