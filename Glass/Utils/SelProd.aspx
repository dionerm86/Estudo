<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelProd.aspx.cs" Inherits="Glass.UI.Web.Utils.SelProd"
    Title="Produtos" MasterPageFile="~/Layout.master" %>
    
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
    
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">

        function setProduto(codInterno, idProd) {
            if (FindControl("hdfCallback", "input").value == "setVidro")
                window.opener.setVidro(codInterno);
            else if (FindControl("hdfCallback", "input").value == "setParent")
                window.opener.setParent(codInterno);
            else if (FindControl("hdfCallback", "input").value == "setBaixaEstFiscal")
                window.opener.setBaixaEstFiscal(codInterno);
            else if (FindControl("hdfCallback", "input").value == "setForPopup")
                eval("window.opener." + '<%= Request["controle"] %>').AlteraValor(idProd, codInterno);
            else if (FindControl("hdfCallback", "input").value == "movInterna")
                window.opener.setProduto(GetQueryString('cod'), GetQueryString('descr'), codInterno);
            else if (FindControl("hdfCallback", "input").value == "prodComposicao") {
                window.opener.setProdutoComposicao(codInterno, GetQueryString('idPedido'), GetQueryString('idProdPed'));
            }
            else {
                if (FindControl("hdfCallback", "input").value == "chapa") {
                    var ignorar = '<%= Request["idProd"] != null ? Request["idProd"] : "" %>';

                    var resposta = SelProd.IsProdChapa(codInterno, ignorar).value;
                    if (resposta == "true") {
                        alert("Esse produto já está associado a uma chapa de vidro.");
                        return false;
                    }
                }

                window.opener.setProduto(codInterno);
            }

            closeWindow();
        }

    </script>
    <table>
        <tr>
            <td align="center">
                <table id="tbProd" runat="server">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:TextBox ID="txtDescr" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupo"
                                            DataTextField="Descricao" DataValueField="IdGrupoProd" OnDataBound="ddlGrupo_DataBound">
                                        </asp:DropDownList>
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo" OnDataBound="ddlSubgrupo_DataBound"
                                            DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                            Height="16px" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblAltura" runat="server" Text="Altura" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="center">
                                        <asp:TextBox ID="txtAltura" runat="server" Width="40px" onkeypress="return soNumeros(event, true, true)"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="lblLargura" runat="server" Text="Largura" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="center">
                                        <asp:TextBox ID="txtLargura" runat="server" Width="40px" onkeypress="return soNumeros(event, true, true)"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdProduto" runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProd" DataSourceID="odsProduto" 
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Nenhum produto encontrado.">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <a href="#" onclick="return setProduto('<%# Eval("CodInterno") %>', <%# Eval("IdProd") %>);">
                                                <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CodInterno" HeaderText="Código" SortExpression="CodInterno" />
                                    <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                                    <asp:BoundField DataField="Largura" HeaderText="Largura" 
                                        SortExpression="Largura" />
                                    <asp:BoundField DataField="Altura" HeaderText="Altura" 
                                        SortExpression="Altura" />
                                    <asp:BoundField DataField="DescrTipoProduto" HeaderText="Tipo" SortExpression="DescrTipoProduto" />
                                    <asp:BoundField DataField="CustoCompra" DataFormatString="{0:C}" HeaderText="Custo"
                                        SortExpression="CustoCompra">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ValorAtacado" HeaderText="Atacado" SortExpression="ValorAtacado"
                                        DataFormatString="{0:C}">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ValorBalcao" DataFormatString="{0:C}" HeaderText="Balcão"
                                        SortExpression="ValorBalcao">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ValorObra" DataFormatString="{0:C}" HeaderText="Obra"
                                        SortExpression="ValorObra">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="ValorReposicao" DataFormatString="{0:C}" HeaderText="Reposição"
                                        SortExpression="ValorReposicao" />
                                    <asp:BoundField DataField="EstoqueDisponivel" HeaderText="Disponível" 
                                        SortExpression="EstoqueDisponivel" />
                                    <asp:BoundField DataField="DescrEstoqueFiscal" HeaderText="Estoque Fiscal" SortExpression="DescrEstoqueFiscal" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemUrl") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfPedidoProducao" runat="server" />
    <asp:HiddenField ID="hdfPedidoInterno" runat="server" />
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" DataObjectTypeName="Glass.Data.Model.Produto"
        DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetProdutosCount" SelectMethod="GetProdutos" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlGrupo" Name="idGrupo" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="ddlSubgrupo" Name="idSubgrupo" PropertyName="SelectedValue"  />
            <asp:ControlParameter ControlID="txtDescr" Name="descr" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtAltura" Name="altura" PropertyName="Text" />
            <asp:ControlParameter ControlID="txtLargura" Name="largura" PropertyName="Text" />
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" DefaultValue="0" />
            <asp:QueryStringParameter DefaultValue="0" Name="idLoja" QueryStringField="idLoja" />
            <asp:QueryStringParameter DefaultValue="0" Name="idCompra" QueryStringField="idCompra" />
            <asp:QueryStringParameter Name="pedidoInterno" QueryStringField="PedidoInterno" DefaultValue="0" />
            <asp:QueryStringParameter Name="idItemProjeto" QueryStringField="idItemProjeto" />
            <asp:QueryStringParameter Name="parceiro" QueryStringField="Parceiro" DefaultValue="0" />
            <asp:QueryStringParameter Name="idCliente" QueryStringField="idCliente" DefaultValue="0" />
            <asp:QueryStringParameter DefaultValue="0" Name="idOrcamento" QueryStringField="idOrcamento" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
        <SelectParameters>
            <asp:Parameter DefaultValue="true" Name="incluirTodos" Type="Boolean" />
            <asp:ControlParameter ControlID="hdfPedidoInterno" DefaultValue="" Name="paraPedidoInterno"
                PropertyName="Value" Type="Boolean" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
        TypeName="Glass.Data.DAL.SubgrupoProdDAO">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlGrupo" Name="idGrupo" PropertyName="SelectedValue"
                Type="String" />
            <asp:ControlParameter ControlID="hdfPedidoProducao" Name="paraPedidoProducao" PropertyName="Value"
                Type="Boolean" />
            <asp:ControlParameter ControlID="hdfPedidoInterno" Name="paraPedidoInterno" PropertyName="Value"
                Type="Boolean" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfCallback" runat="server" />
</asp:Content>