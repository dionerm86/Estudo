<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelPedidoEspelhoImprimir.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelPedidoEspelhoImprimir" Title="Selecione os Pedidos que serão impressos"
    MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
    
        function openRpt() {
        
            var pedidos = "";
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
                alert("Selecione ao menos 1 pedido para ser impresso.");
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

            document.formPost.action = "../Relatorios/RelBase.aspx?rel=PedidoEspelho";
            document.formPost.submit();
            
            //var queryString = window.opener.getRptQueryString() + "&pedidos=";
            //redirectUrl("../Relatorios/RelBase.aspx?rel=PedidoEspelho&" + queryString + (pedidos.length > 0 ? pedidos.substr(1) : ""));
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
                    EmptyDataText="Nenhum pedido em produção encontrado.">
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
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="ResponsavelConferecia" HeaderText="Conferente" SortExpression="Conferente" />
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
                        <asp:TemplateField HeaderText="Total m² / Qtde." SortExpression="TotM">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("TotM") + " (" + Eval("QtdePecas") + " pç.)" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
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
                        <asp:TemplateField HeaderText="Data Entrega" SortExpression="DataEntrega">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DataEntrega") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("DataEntrega", "{0:d}") + ((bool)Eval("FastDelivery") ? " (Fast Del.)" : "") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação Proj. CNC" SortExpression="SituacaoCnc">
                            <ItemTemplate>
                                <asp:Label ID="Label44" runat="server" Text='<%# Bind("DescrSituacaoCnc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidoEspelho" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountSel" SelectMethod="GetListSel" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoEspelhoDAO"
                    >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                        <asp:QueryStringParameter Name="idCli" QueryStringField="idCliente" Type="UInt32" />
                        <asp:QueryStringParameter Name="nomeCli" QueryStringField="nomeCliente" Type="String" />
                        <asp:QueryStringParameter Name="idLoja" QueryStringField="idLoja" Type="UInt32" />
                        <asp:QueryStringParameter Name="idFunc" QueryStringField="idFUnc" Type="UInt32" />
                        <asp:QueryStringParameter Name="idFuncionarioConferente" QueryStringField="idFuncionarioConferente" Type="UInt32" />
                        <asp:QueryStringParameter Name="situacao" QueryStringField="situacao" Type="Int32" />
                        <asp:QueryStringParameter Name="situacaoPedOri" QueryStringField="situacaoPedOri"
                            Type="String" />
                        <asp:QueryStringParameter Name="idsProcesso" QueryStringField="idsProcesso"
                            Type="String" />
                        <asp:QueryStringParameter Name="dataIniEnt" QueryStringField="dataIniEnt" Type="String" />
                        <asp:QueryStringParameter Name="dataFimEnt" QueryStringField="dataFimEnt" Type="String" />
                        <asp:QueryStringParameter Name="dataIniFab" QueryStringField="dataIniFab" Type="String" />
                        <asp:QueryStringParameter Name="dataFimFab" QueryStringField="dataFimFab" Type="String" />
                        <asp:QueryStringParameter Name="dataIniFin" QueryStringField="dataIniFin" Type="String" />
                        <asp:QueryStringParameter Name="dataFimFin" QueryStringField="dataFimFin" Type="String" />
                        <asp:Parameter DefaultValue="false" Name="soFinalizados" Type="Boolean" />
                        <asp:QueryStringParameter Name="pedidosSemAnexo" QueryStringField="pedidosSemAnexos"
                            Type="Boolean" />
                        <asp:QueryStringParameter Name="pedidosAComprar" QueryStringField="pedidosAComprar"
                            Type="Boolean" />
                            <asp:QueryStringParameter Name="situacaoCnc" QueryStringField="situacaoCnc" Type="String" />
                            <asp:QueryStringParameter Name="dataIniSituacaoCnc" QueryStringField="dataIniSituacaoCnc" Type="String" />
                            <asp:QueryStringParameter Name="dataFimSituacaoCnc" QueryStringField="dataFimSituacaoCnc" Type="String" />
                            <asp:QueryStringParameter Name="idsRotas" QueryStringField="idsRotas" Type="String" />
                            <asp:QueryStringParameter Name="origemPedido" QueryStringField="origemPedido" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
