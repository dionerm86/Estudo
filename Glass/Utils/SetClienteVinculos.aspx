<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetClienteVinculos.aspx.cs"
    Title="" Inherits="Glass.UI.Web.Utils.SetClienteVinculos" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <style type="text/css">
        .style1
        {
            width: 50%;
        }
    </style>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td class="subtitle1">
                            Clientes Disponíveis
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" nowrap="nowrap">
                            <table>
                                <tr>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                            onblur="getCli(this);"></asp:TextBox>
                                        <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click"
                                            Style="width: 16px" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Telefone" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTelefone" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            Width="70px"></asp:TextBox>
                                        <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="CPF/CNPJ" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox ID="txtCnpj" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                        <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
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
                            <asp:GridView GridLines="None" ID="grdCli" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" DataSourceID="odsDisponiveis" CssClass="gridStyle"
                                PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                DataKeyNames="Idcli" EmptyDataText="Nenhum cliente encontrado." OnRowCommand="grdCli_RowCommand">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkCriarVinc" runat="server" CommandName="CriarVinculo" CommandArgument='<%# Eval("IdCli") %>'>
                                                <img src="../Images/insert.gif" border="0" title="Vincular" /></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdNome" HeaderText="Nome" SortExpression="Nome" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDisponiveis" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                SelectCountMethod="GetCountVinculo" SelectMethod="GetForVinculo" SortParameterName="sortExpression"
                                StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ClienteDAO">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNumCli" Name="codCliente" PropertyName="Text"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtNome" Name="nome" PropertyName="Text" Type="String" />
                                    <asp:Parameter Name="codRota" Type="String" />
                                    <asp:Parameter Name="idFunc" Type="UInt32" />
                                    <asp:Parameter Name="endereco" Type="String" />
                                    <asp:Parameter Name="bairro" Type="String" />
                                    <asp:ControlParameter ControlID="txtTelefone" Name="telefone" PropertyName="Text"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtCnpj" Name="cpfCnpj" PropertyName="Text" Type="String" />
                                    <asp:Parameter Name="situacao" Type="Int32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
            <td valign="top">
                <table>
                    <tr>
                        <td class="subtitle1">
                            Clientes Vinculados
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdVinculados" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsVinculados" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" DataKeyNames="Idcli" EmptyDataText="Nenhum cliente vinculado."
                                OnRowCommand="grdVinculados_RowCommand">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkRemVinc" runat="server" CommandName="RemoverVinculo" CommandArgument='<%# Eval("IdCli") %>'>
                                            <img src="../Images/removergrid.gif" border="0" title="Vincular" /></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdNome" HeaderText="Nome" SortExpression="Nome" />
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVinculados" runat="server" MaximumRowsParameterName=""
                                SelectMethod="GetVinculados" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.ClienteDAO">
                                <SelectParameters>
                                    <asp:QueryStringParameter Name="idCli" QueryStringField="idCliente" Type="UInt32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
