<%@ Page Language="C#" Title="Confirmar Pedido Interno" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadConfirmarPedidoInterno.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadConfirmarPedidoInterno" %>
    
<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label1" runat="server" Text="Pedido Interno" AssociatedControlID="selPedidoInterno"></asp:Label>
                <uc1:ctrlSelPopup ID="selPedidoInterno" runat="server" DataSourceID="odsBuscarPedidoInterno"
                    DataTextField="IdPedidoInterno" DataValueField="IdPedidoInterno" ExibirIdPopup="true"
                    FazerPostBackBotaoPesquisar="true" ColunasExibirPopup="IdPedidoInterno|NomeFuncCad|DataPedido|Observacao"
                    TitulosColunas="Pedido|Funcionário|Data pedido|Observação" TituloTela="Selecione o Pedido Interno"
                    TamanhoTela="Tamanho700x525" PermitirVazio="false" TextWidth="70px" />
            </span>
        </div>
    </div>
    <div id="confirmar" runat="server" visible="false">
        <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AutoGenerateColumns="False"
            DataSourceID="odsProdutoPedidoInterno" CssClass="gridStyle" PagerStyle-CssClass="pgr"
            AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
            <Columns>
                <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                    <ItemTemplate>
                        <asp:Label ID="lblQtdePedido" runat="server" Text='<%# Eval("Qtde") %>' Font-Bold='<%# Eval("ConfirmarQtde") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Font-Bold="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                <asp:TemplateField HeaderText="Total m²" SortExpression="TotM">
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("TotM") %>' Font-Bold='<%# !(bool)Eval("ConfirmarQtde") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Qtde Conf.">
                    <ItemTemplate>
                        <asp:TextBox ID="txtQtde" runat="server" Text='<%# Eval("QtdeConfirmar") %>' Width="50px"
                            onkeypress='<%# "return soNumeros(event, CalcProd_IsQtdeInteira(" + Eval("TipoCalc") + "), true)" %>'></asp:TextBox>
                        <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage='<%# Eval("QtdeConfirmar", "Valor deve estar entre 0 e {0}") %>' ValidationGroup="c"
                            ControlToValidate="txtQtde" MinimumValue="0" MaximumValue='<%# Eval("QtdeConfirmar") %>'
                            Display="Dynamic" Type="Double"></asp:RangeValidator>
                        <asp:HiddenField ID="hdfIdProdPedInterno" runat="server" Value='<%# Eval("IdProdPedInterno") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerStyle CssClass="pgr"></PagerStyle>
            <EditRowStyle CssClass="edit"></EditRowStyle>
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
        </asp:GridView>
        <br />
        <asp:Button ID="btnConfirmar" runat="server" OnClientClick="if (!confirm(&quot;Deseja confirmar esse pedido interno?&quot;)) return false"
            Text="Confirmar" OnClick="btnConfirmar_Click" ValidationGroup="c" />
    </div>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="c" />
    <colo:VirtualObjectDataSource runat="server" ID="odsBuscarPedidoInterno" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="pt-BR" MaximumRowsParameterName="" SelectMethod="ObtemParaConfirmacao" 
        StartRowIndexParameterName="" TypeName="Glass.Data.DAL.PedidoInternoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutoPedidoInterno" runat="server" SelectMethod="GetByPedidoInterno"
        TypeName="Glass.Data.DAL.ProdutoPedidoInternoDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="selPedidoInterno" Name="idPedidoInterno" PropertyName="Valor"
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

    <script type="text/javascript">
        FindControl("selPedidoInterno_txtDescr", "input").focus();
    </script>

</asp:Content>
