<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelPedidoEspelhoCompraProdBenef.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelPedidoEspelhoCompraProdBenef" Title="Selecione os Pedidos que irão gerar compra de caixa"
    MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
    
        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openFornec() {
            if (FindControl("txtFornecedor", "input").value != "")
                return true;

            openWindow(500, 700, "../Utils/SelFornec.aspx");

            return false;
        }
        
        var pedidos = "";
        var fornecedor = "";
        
        function validaGerarCompra() {
            var inputs = document.getElementById("<%= grdPedido.ClientID %>").getElementsByTagName("input");
            
            for (i = 0; i < inputs.length; i++)
            {
                if (inputs[i].id.indexOf("chkTodos") > -1)
                    continue;
                
                if (inputs[i].checked)
                    pedidos += "," + inputs[i].parentNode.getAttribute("idPedido");
            }
            
            if (pedidos.length == 0)
            {
                alert("Selecione ao menos 1 pedido.");
                return false;
            }
            
            fornecedor = FindControl("ddlFornecedor", "select");
            
            if (fornecedor.value == null || fornecedor.value == 0) {
                alert("Selecione o fornecedor da compra.");
                return false;                
            }
            
            return validaLoja();
        }
        
        function validaLoja() {
            var retorno = SelPedidoEspelhoCompraProdBenef.ValidaLojaPedidos(pedidos.substr(1)).value;
            
            if (retorno.split(';')[0] == "Erro"){
                alert(retorno.split(';')[1]);
                return false;
            }
            
            return true;
        }
    
        function gerarCompraCaixa() {
            if (!validaGerarCompra())
            {
                pedidos = "";
                fornecedor = "";
                return false;
            }
            
            var campoPedidos = FindControl("campoPedidos", "input");

            if (campoPedidos == null) {
                campoPedidos = document.createElement("input");
                campoPedidos.id = "campoPedidos";
                campoPedidos.name = "pedidos";
                document.formPost.appendChild(campoPedidos);
            }

            campoPedidos.value = pedidos.substr(1);

            var retorno = SelPedidoEspelhoCompraProdBenef.GerarCompraProdBenef(campoPedidos.value, fornecedor.value).value;
            
            var mensagemRetorno = "";
            
            if (retorno.split(';')[0] == null || retorno.split(';')[0] == "") {
                alert("Falha ao gerar compra." +
                    "\nPedidos que não geraram compra: " + retorno.split(';')[2]);
                    
                pedidos = "";
                fornecedor = "";
                return false;
            }
            else {
                alert("Compra gerada com sucesso." +
                    "\nCódigo da compra: " + retorno.split(';')[0] +
                    "\nPedidos que geraram compra: " + retorno.split(';')[1] +
                    (retorno.split(';')[2] != null && retorno.split(';')[2] != "" ?
                    "\nPedidos que não geraram compra: " + retorno.split(';')[2] : ""));
            }
            
            window.close();
        }
        
        function checkAll(checkbox)
        {
            var inputs = document.getElementById("<%= grdPedido.ClientID %>").getElementsByTagName("input");
            for (i = 0; i < inputs.length; i++)
                inputs[i].checked = checkbox.checked;
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPedidoEspelho" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdPedido"
                    EmptyDataText="Nenhum pedido em produção encontrado." AllowPaging="True" PageSize="50">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkImprimir" runat="server" Checked="True" OnDataBinding="chkImprimir_DataBinding" />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkTodos" runat="server" Checked="True" OnDataBinding="chkImprimir_DataBinding"
                                    onclick="checkAll(this)" />
                            </HeaderTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeInicialCli" HeaderText="Cliente" SortExpression="NomeInicialCli" />
                        <asp:BoundField DataField="TotalPedido" HeaderText="Total Pedido" SortExpression="TotalPedido"
                            DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Total" HeaderText="Total Conf." SortExpression="Total"
                            DataFormatString="{0:C}"></asp:BoundField>
                        <asp:BoundField DataField="DataEspelho" DataFormatString="{0:d}" HeaderText="Data Conf."
                            SortExpression="DataEspelho" />
                        <asp:BoundField DataField="DataConf" DataFormatString="{0:d}" HeaderText="Finalização"
                            SortExpression="DataConf" />
                        <asp:BoundField DataField="Peso" HeaderText="Peso" SortExpression="Peso" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacao") %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemStyle VerticalAlign="Middle" Wrap="False" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidoEspelho" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountCompraProdBenefSel" SelectMethod="GetListCompraProdBenefSel"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoEspelhoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                        <asp:QueryStringParameter Name="idCli" QueryStringField="idCliente" Type="UInt32" />
                        <asp:QueryStringParameter Name="nomeCli" QueryStringField="nomeCliente" Type="String" />
                        <asp:QueryStringParameter Name="idLoja" QueryStringField="idLoja" Type="UInt32" />
                        <asp:QueryStringParameter Name="situacao" QueryStringField="situacao" Type="Int32" />
                        <asp:QueryStringParameter Name="situacaoPedOri" QueryStringField="situacaoPedOri"
                            Type="String" />
                        <asp:QueryStringParameter Name="dataIniFin" QueryStringField="dataIniFin" Type="String" />
                        <asp:QueryStringParameter Name="dataFimFin" QueryStringField="dataFimFin" Type="String" />
                        <asp:QueryStringParameter Name="idsRota" QueryStringField="idsRota" Type="String" />
                        <asp:QueryStringParameter Name="compraGerada" QueryStringField="compraGerada" Type="Int32" />
                        <asp:QueryStringParameter Name="dtCompraIni" QueryStringField="dtCompraIni" Type="String" />
                        <asp:QueryStringParameter Name="dtCompraFim" QueryStringField="dtCompraFim" Type="String" />
                        <asp:QueryStringParameter Name="idCompra" QueryStringField="idCompra" Type="Int32" />
                        <asp:QueryStringParameter Name="dtEntregaPedIni" QueryStringField="dtEntIni" Type="String" />
                        <asp:QueryStringParameter Name="dtEntregaPedFim" QueryStringField="dtEntFim" Type="String" />
                        <asp:QueryStringParameter Name="ordenarPor" QueryStringField="ordenarPor" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="ddlFornecedor" runat="server" DataSourceID="odsFornecedor"
                                DataTextField="Nome" DataValueField="IdFornec" AppendDataBoundItems="True"
                                Width="200px">
                                <asp:ListItem Value="0">Selecione</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnGerarCompra" runat="server" Text="Gerar compra dos pedidos selecionados"
                                OnClientClick="gerarCompraCaixa(); return false;" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" SelectMethod="GetForCompraProdBenef"
            TypeName="Glass.Data.DAL.FornecedorDAO" >
        </colo:VirtualObjectDataSource>
    </table>
</asp:Content>
