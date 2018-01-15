<%@ Page Title="Ferragens" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstFerragem.aspx.cs" Inherits="Glass.UI.Web.Listas.LstFerragem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Nome Ferragem:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeFerragem" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Fabricante:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFabricanteFerragem" runat="server" AppendDataBoundItems="true"
                                DataSourceID="odsFabricantesFerragem" DataTextField="Name" DataValueField="Id">
                                <asp:ListItem Value="0" Text="Todos"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Código:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodigo" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserirFerragem" runat="server" OnLoad="lnkInserirFerragem_Load" Text="Inserir Ferragem" OnClick="lnkInserirFerragem_Click" ></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFerragem" runat="server" AllowPaging="true" AllowSorting="true"
                    CssClass="gridStyle" AutoGenerateColumns="false" GridLines="None" ShowFooter="true"
                    DataSourceID="odsFerragem" DataKeyNames="IdFerragem">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ImageUrl="~/Images/Edit.gif" ToolTip="Editar"
                                    NavigateUrl='<%# ResolveUrl("~/Cadastros/Projeto/CadFerragem.aspx?IdFerragem=" + Eval("IdFerragem")) %>' />
                                <asp:ImageButton ID="imbInativar" runat="server" CommandName="Update" ImageUrl="~/Images/Inativar.gif"
                                    Visible='<%# Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync %>'
                                    ToolTip="Ativar/Inativar" />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    Visible='<%# Glass.Data.Helper.UserInfo.GetUserInfo.IsAdminSync %>'
                                    ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir esta Ferragem?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:BoundField DataField="IdFerragem" HeaderText="IdFerragem" SortExpression="IdFerragem" />--%>
                        <asp:BoundField DataField="Nome" HeaderText="Ferragem" SortExpression="Nome" />
                        <asp:BoundField DataField="NomeFabricante" HeaderText="Fabricante" SortExpression="NomeFabricante" />
                        <%--<asp:BoundField DataField="EstiloAncoragem" HeaderText="Estilo Ancoragem" SortExpression="EstiloAncoragem" />--%>
                        <asp:BoundField DataField="Situacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="DataAlteracao" HeaderText="Data Alteração" SortExpression="DataAlteracao" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                    <EditRowStyle CssClass="edit" />
                    <PagerStyle CssClass="pgr" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsFerragem" runat="server" Culture="pt-BR"
                    EnablePaging="true" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression"
                    TypeName="Glass.Projeto.Negocios.IFerragemFluxo"
                    DataObjectTypeName="Glass.Projeto.Negocios.Entidades.Ferragem"
                    SelectMethod="PesquisarFerragem" SelectByKeysMethod="ObterFerragem"
                    UpdateStrategy="GetAndUpdate" UpdateMethod="AtivarInativarFerragem"
                    DeleteStrategy="GetAndDelete" DeleteMethod="ApagarFerragem">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNomeFerragem" Name="nomeFerragem" PropertyName="Text" />
                        <asp:ControlParameter ControlID="drpFabricanteFerragem" Name="idFabricanteFerragem" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtCodigo" Name="codigo" PropertyName="Text" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsFabricantesFerragem" runat="server" Culture="pt-BR"
                    TypeName="Glass.Projeto.Negocios.IFerragemFluxo" SelectMethod="ObterFabricantesFerragem">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
