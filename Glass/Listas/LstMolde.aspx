<%@ Page Title="Execução de Molde" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstMolde.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstMolde" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(idMolde) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Molde&idMolde=" + idMolde);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Molde" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtMolde" runat="server" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPedido" runat="server" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCliente" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HyperLink ID="lnkInserir" runat="server" NavigateUrl="~/Cadastros/CadMolde.aspx">
                    Inserir execução de molde</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMolde" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsMolde" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AllowPaging="True"
                    AllowSorting="True" DataKeyNames="IdMolde">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" NavigateUrl='<%# "~/Cadastros/CadMolde.aspx?idMolde=" + Eval("IdMolde") %>'><img 
                                    border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir essa execução de molde?&quot;)) return false;" />
                                <asp:ImageButton ID="imgRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("IdMolde") + "); return false" %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdMolde" HeaderText="Molde" SortExpression="IdMolde" />
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="EnderecoComplObra" HeaderText="Endereço" SortExpression="EnderecoObra" />
                        <asp:BoundField DataField="FuncVend" HeaderText="Vendedor" 
                            SortExpression="FuncVend" />
                        <asp:BoundField DataField="FuncCad" HeaderText="Cadastrado Por" 
                            SortExpression="FuncCad" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMolde" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetListCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.MoldeDAO" DataObjectTypeName="Glass.Data.Model.Molde"
                    DeleteMethod="Delete">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtMolde" Name="idMolde" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idOrcamento" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
