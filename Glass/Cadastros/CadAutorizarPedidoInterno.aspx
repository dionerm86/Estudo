<%@ Page Title="Autorizar Pedido Interno" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadAutorizarPedidoInterno.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadAutorizarPedidoInterno" %>

<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
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
    <asp:DetailsView ID="dtvPedidoInterno" runat="server" 
        DataSourceID="odsPedidoInterno" AutoGenerateRows="False" 
        DataKeyNames="IdPedidoInterno" CssClass="gridStyle detailsViewStyle" 
        GridLines="None">
        <FieldHeaderStyle CssClass="dtvHeader" />
        <Fields>
            <asp:BoundField DataField="IdPedidoInterno" HeaderText="Pedido" 
                SortExpression="IdPedidoInterno" />
            <asp:BoundField DataField="NomeFuncCad" HeaderText="Funcionário" 
                SortExpression="NomeFuncCad" />
            <asp:BoundField DataField="DataPedido" DataFormatString="{0:d}" 
                HeaderText="Data Pedido" SortExpression="DataPedido" />
            <asp:TemplateField HeaderText="Centro de Custo">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlCentroCusto" runat="server" DataSourceID="odsCentroCusto" OnDataBound="ddlCentroCusto_DataBound"
                        DataTextField="Descricao" DataValueField="IdCentroCusto" SelectedValue='<%# Bind("IdCentroCusto") %>'
                        AppendDataBoundItems="true">
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Observacao" HeaderText="Observação" 
                SortExpression="Observacao" />
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:GridView ID="grdProdutos" runat="server" AutoGenerateColumns="False" 
                        CssClass="gridStyle" DataKeyNames="IdProdPedInterno" 
                        DataSourceID="odsProdutosPedidoInterno" GridLines="None" Style="margin: 8px">
                        <Columns>
                            <asp:BoundField DataField="CodInterno" HeaderText="Cód." 
                                SortExpression="CodInterno" />
                            <asp:BoundField DataField="DescrProduto" HeaderText="Descrição" 
                                SortExpression="DescrProduto" />
                            <asp:BoundField DataField="Altura" HeaderText="Altura" 
                                SortExpression="Altura" />
                            <asp:BoundField DataField="Largura" HeaderText="Largura" 
                                SortExpression="Largura" />
                            <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                            <asp:BoundField DataField="TotM" HeaderText="Tot m²" SortExpression="TotM" />
                            <asp:BoundField DataField="Observacao" HeaderText="Observação" 
                                SortExpression="Observacao" />
                        </Columns>
                        <AlternatingRowStyle CssClass="alt" />
                    </asp:GridView>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <br />
                    <asp:Button ID="btnAutorizar" runat="server" Text="Autorizar" 
                        onclick="btnAutorizar_Click" 
                        OnClientClick='if (!confirm("Autorizar pedido?")) return false' 
                        Visible='<%# Eval("DataAut") == null %>' />
                    <asp:Label ID="Label2" runat="server" 
                        Text='<%# "Pedido autorizado em " + Eval("DataAut") + " por " + Eval("NomeFuncAut") + "." %>' 
                        Visible='<%# Eval("DataAut") != null %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
        <AlternatingRowStyle CssClass="alt" />
        <RowStyle HorizontalAlign="Left" />
    </asp:DetailsView>
    <colo:VirtualObjectDataSource runat="server" ID="odsBuscarPedidoInterno" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="pt-BR" MaximumRowsParameterName="" SelectMethod="ObtemParaAutorizacao" 
        StartRowIndexParameterName="" TypeName="Glass.Data.DAL.PedidoInternoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource runat="server" ID="odsPedidoInterno" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="pt-BR" MaximumRowsParameterName="" SelectMethod="GetElement" SkinID="" 
        StartRowIndexParameterName="" TypeName="Glass.Data.DAL.PedidoInternoDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="selPedidoInterno" Name="idPedidoInterno" 
                PropertyName="Valor" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource ID="odsProdutosPedidoInterno" runat="server" 
        CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
        Culture="" MaximumRowsParameterName="" SelectMethod="GetByPedidoInterno" 
        SkinID="" StartRowIndexParameterName="" 
        TypeName="Glass.Data.DAL.ProdutoPedidoInternoDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="selPedidoInterno" Name="idPedidoInterno" 
                PropertyName="Valor" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCentroCusto" runat="server" SelectMethod="ObtemParaSelecao"
                    TypeName="Glass.Data.DAL.CentroCustoDAO" DataObjectTypeName="Glass.Data.Model.CentroCusto">
                    <SelectParameters>
                        <asp:Parameter Name="buscarEstoque" Type="Boolean" DefaultValue="false" />
                        <asp:Parameter Name="idLoja" Type="Int32" DefaultValue="0" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
</asp:Content>

