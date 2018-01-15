<%@ Page Title="Finalizar Inventário de Estoque" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadFinalizarInventario.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadFinalizarInventario" %>

<%@ Register src="../Controls/ctrlNovaLinhaGrid.ascx" tagname="ctrlNovaLinhaGrid" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <%--
    <script type="text/javascript">
        function validaItem(val, args)
        {
            var linha = val;
            while (linha.nodeName.toLowerCase() != "tr")
                linha = linha.parentNode;

            var qtde = FindControl("txtQtde", "input", linha);
            var m2 = FindControl("txtM2", "input", linha);
            
            args.IsValid = (qtde && qtde.value != "") || (m2 && m2.value != "");
        }
    </script>
    --%>
    
    Indique a quantidade/m² de cada produto no estoque:
    <asp:GridView ID="grdProdutos" runat="server" AutoGenerateColumns="False" 
        CssClass="gridStyle" GridLines="None" 
        DataSourceID="odsProdutosInventarioEstoque" 
        DataKeyNames="CodigoInventarioEstoque,CodigoProduto">
        <Columns>
            <asp:TemplateField HeaderText="Produto">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" 
                        Text='<%# Eval("CodigoInternoProduto") + " - " + Eval("DescricaoProduto") %>'></asp:Label>
                    <asp:HiddenField ID="hdfQtdeInicial" runat="server" 
                        Value='<%# Bind("QtdeInicial") %>' />
                    <asp:HiddenField ID="hdfM2Inicial" runat="server" 
                        Value='<%# Bind("M2Inicial") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Qtde">
                <ItemTemplate>
                    <asp:TextBox ID="txtQtde" runat="server" Text='<%# Bind("QtdeFinalizacao") %>'
                        onkeypress="return soNumeros(event, false, true)" Width="70px" 
                        Enabled='<%# Eval("UtilizarCampoQtde") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="M²" Visible="False">
                <ItemTemplate>
                    <asp:TextBox ID="txtM2" runat="server" Text='<%# Bind("M2Finalizacao") %>'
                        onkeypress="return soNumeros(event, false, true)" Width="70px" 
                        Enabled='<%# !(bool)Eval("UtilizarCampoQtde") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <%--
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CustomValidator ID="ctvValidar" runat="server" 
                        ClientValidationFunction="validaItem" Display="Dynamic" ErrorMessage="*" 
                        ValidateEmptyText="True"></asp:CustomValidator>
                    <script type="text/javascript">
                        var linha = document.getElementById("<%= grdProdutos.ClientID %>").rows;
                        linha = linha[linha.length - 1];

                        var validador = FindControl("ctvValidar", "span", linha);
                        var qtde = FindControl("txtQtde", "input", linha);
                        var m2 = FindControl("txtM2", "input", linha);
                        
                        if (qtde && !qtde.disabled) ValidatorHookupControl(qtde, validador);
                        if (m2 && !m2.disabled) ValidatorHookupControl(m2, validador);
                    </script>
                </ItemTemplate>
            </asp:TemplateField>
            --%>
        </Columns>
        <AlternatingRowStyle CssClass="alt" />
    </asp:GridView>
    <asp:Button ID="btnAtualizar" runat="server" Text="Atualizar" 
        onclick="btnAtualizar_Click"></asp:Button>
    <asp:Button ID="btnFechar" runat="server" 
        onclientclick="closeWindow(); return false;" Text="Fechar" />
    <colo:VirtualObjectDataSource ID="odsProdutosInventarioEstoque" runat="server" 
        SelectMethod="ObtemProdutos" Culture="pt-BR"
        TypeName="WebGlass.Business.InventarioEstoque.Fluxo.Finalizar" 
        UpdateMethod="FinalizarProduto" CacheExpirationPolicy="Absolute" 
        ConflictDetection="OverwriteChanges" 
        DataObjectTypeName="WebGlass.Business.InventarioEstoque.Entidade.ProdutoInventarioEstoque" 
        MaximumRowsParameterName="" SkinID="" StartRowIndexParameterName="">
        <SelectParameters>
            <asp:QueryStringParameter Name="codigoInventario" QueryStringField="id" 
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>    
</asp:Content>

