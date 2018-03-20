<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstTabelaDesconto.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstTabelaDesconto" Title="Tabelas de Desconto/Acréscimo Cliente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaTabelaCliente&exportarExcel=" + exportarExcel);
        }
    </script>

    <asp:GridView ID="grdTabelaDesconto" runat="server" SkinID="defaultGridView"
        DataKeyNames="IdTabelaDesconto" DataSourceID="odsTabelaDesconto" OnDataBound="grdTabelaDesconto_DataBound">
        <Columns>
            <asp:TemplateField>
                <EditItemTemplate>
                    <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Update" ImageUrl="~/Images/Ok.gif" />
                    <asp:ImageButton ID="ImageButton2" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                    <asp:ImageButton ID="ImageButton2" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                        OnClientClick="if (!confirm(&quot;Deseja excluir essa tabela de desconto/acréscimo?&quot;)) return false" />
                    <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/money_delete.gif"
                        OnClientClick='<%# "openWindow(500, 650, \"../Cadastros/CadDescontoAcrescimoCliente.aspx?idTabelaDesconto=" + Eval("IdTabelaDesconto") + "\"); return false" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                <EditItemTemplate>
                    <asp:TextBox ID="txtDescricao" runat="server" MaxLength="45" Text='<%# Bind("Descricao") %>'
                        Width="200px"></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtDescricao" runat="server" MaxLength="45" Text='<%# Bind("Descricao") %>'
                        Width="200px"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <FooterTemplate>
                    <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click" />
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerStyle CssClass="pgr" />
        <EditRowStyle CssClass="edit" />
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTabelaDesconto" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.TabelaDescontoAcrescimoCliente"
        DeleteMethod="ApagarTabelaDescontoAcrescimo" 
        DeleteStrategy="GetAndDelete"
        SelectMethod="PesquisarTabelasDescontosAcrescimos" 
        SelectByKeysMethod="ObtemTabelaDescontoAcrescimoCliente"
        TypeName="Glass.Global.Negocios.IClienteFluxo"
        EnablePaging="True" MaximumRowsParameterName="pageSize"
        SortParameterName="sortExpression"
        UpdateStrategy="GetAndUpdate"
        UpdateMethod="SalvarTabelaDescontoAcrescimo">
    </colo:VirtualObjectDataSource>
    <div class="imprimir">
        <span>
            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"> <img 
                border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
        </span>
        <span>
            <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
        </span>
    </div>
</asp:Content>
