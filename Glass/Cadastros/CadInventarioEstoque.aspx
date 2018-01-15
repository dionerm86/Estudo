<%@ Page Title="Inventário de Estoque" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadInventarioEstoque.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadInventarioEstoque" %>

<%@ Register src="../Controls/ctrlLoja.ascx" tagname="ctrlLoja" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlSelGrupoSubgrupoProd.ascx" tagname="ctrlSelGrupoSubgrupoProd" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
        function validar()
        {
            var idLoja = FindControl("ctrlLoja", "select").value;
            if (idLoja == "" || idLoja == "0")
            {
                alert("Selecione a loja.");
                return false;
            }

            return true;
        }
    </script>
    <asp:DetailsView ID="dtvInventarioEstoque" runat="server" 
        AutoGenerateRows="False" CssClass="gridStyle detailsViewStyle" 
        DataKeyNames="Codigo" DataSourceID="odsInventarioEstoque" DefaultMode="Insert" 
        GridLines="None">
        <FieldHeaderStyle CssClass="dtvHeader" />
        <Fields>
            <asp:TemplateField HeaderText="Loja" SortExpression="CodigoLoja">
                <EditItemTemplate>
                    <uc1:ctrlLoja ID="ctrlLoja" runat="server" MostrarTodas="False" 
                        MostrarVazia="True" SelectedValue='<%# Bind("CodigoLoja") %>' 
                        SomenteAtivas="True" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodigoLoja") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Grupo / Subgrupo" 
                SortExpression="CodigoGrupoProduto">
                <EditItemTemplate>
                    <uc2:ctrlSelGrupoSubgrupoProd ID="ctrlSelGrupoSubgrupoProd" runat="server" 
                        ApenasVidros="False" CodigoGrupoProduto='<%# Bind("CodigoGrupoProduto") %>' 
                        CodigoSubgrupoProduto='<%# Bind("CodigoSubgrupoProduto") %>' 
                        ExibirGrupoProdutoVazio="False" ExibirSubgrupoProdutoVazio="True" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodigoGrupoProduto") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>
                    <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" 
                        Text="Atualizar" OnClientClick="if (!validar()) return false;" />
                    <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                        onclick="btnCancelar_Click" Text="Cancelar" />
                    <asp:HiddenField ID="hdfSituacao" runat="server" 
                        Value='<%# Bind("Situacao") %>' />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:Button ID="btnInserir" runat="server" CommandName="Insert" 
                        Text="Inserir" OnClientClick="if (!validar()) return false;" />
                    <asp:Button ID="btnCancelar" runat="server" CausesValidation="False" 
                        onclick="btnCancelar_Click" Text="Cancelar" />
                </InsertItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
        </Fields>
        <AlternatingRowStyle CssClass="alt" />
    </asp:DetailsView>
    <colo:VirtualObjectDataSource ID="odsInventarioEstoque" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        DataObjectTypeName="WebGlass.Business.InventarioEstoque.Entidade.InventarioEstoque" 
        InsertMethod="NovoInventario" MaximumRowsParameterName="" 
        oninserted="odsInventarioEstoque_Inserted" 
        onupdated="odsInventarioEstoque_Updated" SelectMethod="ObtemItem" 
        StartRowIndexParameterName="" 
        TypeName="WebGlass.Business.InventarioEstoque.Fluxo.CRUD" 
        UpdateMethod="Atualizar">
        <SelectParameters>
            <asp:QueryStringParameter Name="codigoInventario" QueryStringField="id" 
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>

