<%@ Page Language="C#" MasterPageFile="~/WebGlassParceiros/PainelParceiros.master"
    AutoEventWireup="true" CodeBehind="lstProducao.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.lstProducao"%>

<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlBenefSetor.ascx" TagName="ctrlBenefSetor" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <style>
        .tbPesquisa
        {
            vertical-align: middle;
        }
        .tbPesquisa td
        {
            background-color: #FAFAFA;
            border: solid 1px #F0F0F0;
            display: table-cell;
            vertical-align: middle;
            white-space: nowrap;
            width: auto;
            margin: 0;
            padding: 0;
        }
        .tituloCampos
        {
            text-align: left;
        }
    </style>
    <section>
        <header>
            <h2>
                Pedidos em Produção</h2>
        </header>
        <section id="pesquisa">
            <table class="tbPesquisa">
                <tr>
                    <td class="tituloCampos">
                        <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtNumPedido" runat="server" Width="45px" onkeypress="return soNumeros(event, true, true);"
                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="6"></asp:TextBox>
                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                            ToolTip="Pesquisar" />
                    </td>
                    <td>
                        <asp:Label ID="Label15" runat="server" Text="Pedido Cli." ForeColor="#0066FF"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCodPedCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                            ToolTip="Pesquisar" />
                    </td>
                </tr>
            </table>
            <br />
            <br />
        </section>
        <section id="pecas">
            <asp:GridView GridLines="None" ID="grdPecas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                DataKeyNames="IdProdPedProducao" DataSourceID="odsPecas" EmptyDataText="Nenhuma peça encontrada."
                OnDataBound="grdPecas_DataBound" OnLoad="grdPecas_Load" CssClass="gridStyle"
                PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                AllowSorting="True">
                <Columns>
                    <asp:TemplateField Visible="False">
                    </asp:TemplateField>
                    <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HiddenField ID="hdfIdSetor" runat="server" Value='<%# Eval("IdSetor") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Eval("Situacao") %>' />
                                <asp:HiddenField ID="hdfPedidoCancelado" runat="server" Value='<%# Eval("PedidoCancelado") %>' />
                                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinha") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                    <asp:TemplateField HeaderText="Pedido" SortExpression="IdPedidoExibir">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdPedidoExibir") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdPedidoExibir") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="False"></asp:TemplateField>
                    <asp:TemplateField HeaderText="Tipo Ped." SortExpression="SiglaTipoPedido">
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("SiglaTipoPedido") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("SiglaTipoPedido") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Pedido Cli." SortExpression="CodCliente">
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodCliente") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("CodCliente") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="False"></asp:TemplateField>
                    <asp:TemplateField HeaderText="Produto" SortExpression="DescrProdLargAlt">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrProdLargAlt") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label21" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                            <asp:Label ID="Label22" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                            <asp:Label ID="Label23" runat="server" Font-Bold="True" Text='<%# Eval("LarguraAltura") %>'></asp:Label>
                            <br />
                            <asp:Label ID="Label24" runat="server" Font-Size="90%" Text='<%# Eval("DescrTipoPerdaLista") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Apl." SortExpression="CodAplicacao">
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Proc." SortExpression="CodProcesso">
                        <ItemTemplate>
                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="pgr"></PagerStyle>
                <EditRowStyle CssClass="edit"></EditRowStyle>
                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            </asp:GridView>
            <div>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPecas" runat="server" MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarProdutosProducaoAcessoExterno" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO"
                    EnablePaging="True" SelectCountMethod="PesquisarProdutosProducaoAcessoExternoCount" SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodPedCli" Name="codigoPedidoCliente" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </div>
        </section>
    </section>
</asp:Content>
