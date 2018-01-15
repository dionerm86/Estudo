<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelFerragemExportar.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.SelFerragemExportar" Title="Selecione a Ferragem" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblNomeFerragem" runat="server" Text="Nome da Ferragem" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeFerragem" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblFabricanteFerragem" runat="server" Text="Fabricante" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFabricanteFerragem" runat="server" AppendDataBoundItems="true" DataSourceID="odsFabricantesFerragem" DataTextField="Name" DataValueField="Id">
                                <asp:ListItem Value="0" Text="Todos"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFerragens" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsFerragem" GridLines="None">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbSelecionarFerragem" runat="server" ImageUrl="~/Images/Insert.gif"
                                    OnClientClick='<%# string.Format("window.opener.setarFerragem({0}, \"{1}\", \"{2}\", \"{3}\", \"{4}\"); return false;",
                                        Eval("IdFerragem"), Eval("Nome"), Eval("NomeFabricante"), Eval("Situacao"), Eval("DataAlteracao")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Ferragem" SortExpression="Nome" />
                        <asp:BoundField DataField="NomeFabricante" HeaderText="Fabricante" SortExpression="NomeFabricante" />
                        <asp:BoundField DataField="Situacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="DataAlteracao" HeaderText="Data Alteração" SortExpression="DataAlteracao" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClick="lnkAddAll_Click">
                    <img src="../../Images/addMany.gif" border="0"> Adicionar Todas</asp:LinkButton>
            </td>
        </tr>
    </table>
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
            <asp:Parameter DefaultValue="" Type="String" Name="codigo" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsFabricantesFerragem" runat="server" Culture="pt-BR" TypeName="Glass.Projeto.Negocios.IFerragemFluxo" SelectMethod="ObterFabricantesFerragem">
    </colo:VirtualObjectDataSource>
</asp:Content>
