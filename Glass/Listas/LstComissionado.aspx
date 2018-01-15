<%@ Page Title="Comissionados" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstComissionado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstComissionado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
<script type="text/javascript">

    function openRpt(exportarExcel) {
        var nome = FindControl("txtNome", "input").value;
        var situacao = FindControl("drpSituacao", "select").value;

        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaComissionado&nome=" + nome + "&situacao=" + situacao + 
                "&exportarExcel=" + exportarExcel);

        return false;
    }

</script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Nome"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="if (isEnter(event)) return false;" Width="170px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                                <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                            </asp:DropDownList>
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Comissionado</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdComissionado" runat="server" SkinID="defaultGridView"
                    DataKeyNames="IdComissionado" DataSourceID="odsComissionado"
                    EmptyDataText="Nenhum Comissionado encontrado.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" NavigateUrl='<%# "../Cadastros/CadComissionado.aspx?idComissionado=" + Eval("IdComissionado") %>'
                                    ToolTip="Editar" Visible='<%# PodeEditar() %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Comissionado?&quot;);"
                                    ToolTip="Excluir" Visible='<%# PodeApagar() %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdComissionado" HeaderText="Cód." SortExpression="IdComissionado" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj" />
                        <asp:BoundField DataField="RgInscEst" HeaderText="RG/Insc. Est." SortExpression="RgInscEst" />
                        <asp:BoundField DataField="TelRes" HeaderText="Tel. Cont." SortExpression="TelRes" />
                        <asp:BoundField DataField="TelCel" HeaderText="Cel." SortExpression="TelCel" />
                        <asp:BoundField DataField="Percentual" HeaderText="Percentual" SortExpression="Percentual" />
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissionado" runat="server" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarComissionados"
                    SelectByKeysMethod="ObtemComissionado"
                    SortParameterName="sortExpression" 
                    TypeName="Glass.Global.Negocios.IComissionadoFluxo"
                    DeleteMethod="ApagarComissionado" 
                    DeleteStrategy="GetAndDelete"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Comissionado">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNome" Name="nome" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
         <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" alt="Exportar para o Excel" /> Exportar para o Excel</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
