<%@ Page Title="Pedidos em Conferência" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstPedidosConferencia.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPedidosConferencia" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        // Abre relatório de conferência
        function openRpt() {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;

            if (idPedido == "")
                idPedido = 0;

            if (idLoja == "")
                idLoja = 0;

            var queryString = "?Rel=ListaConferencia&IdPedido=" + idPedido + "&idLoja=" + idLoja + "&NomeCliente=" + nomeCliente +
                "&situacao=" + situacao + "&IdConferente=0&dataConferencia=&sitPedido=0";

            openWindow(600, 800, "../Relatorios/RelBase.aspx" + queryString);
            return false;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Num. Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Sit. Confer."></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="2">Em Andamento</asp:ListItem>
                                <asp:ListItem Value="3">Finalizada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True" AutoPostBack="True" OnSelectedIndexChanged="drpSituacao_SelectedIndexChanged">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdPedidosConferencia" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataSourceID="odsPedidosConferencia"
                    EmptyDataText="Nenhum pedido em conferência encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="Vendedor" HeaderText="Vendedor" SortExpression="Vendedor" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="TelCli" HeaderText="Tel. Cliente" SortExpression="TelCli" />
                        <asp:BoundField DataField="LocalObra" HeaderText="Local Obra" SortExpression="LocalObra" />
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Data Entrega"
                            SortExpression="DataEntrega" />
                        <asp:BoundField DataField="DataIni" DataFormatString="{0:d}" HeaderText="Início Confer."
                            SortExpression="DataIni" />
                        <asp:BoundField DataField="DataFim" DataFormatString="{0:d}" HeaderText="Fim Confer."
                            SortExpression="DataFim" />
                        <asp:BoundField DataField="Conferente" HeaderText="Conferente" SortExpression="Conferente" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Sit. Confer." ReadOnly="True"
                            SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <a id="lnkImprimir" href="#" onclick="return openRpt();">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a><br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidosConferencia" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoConferenciaDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:Parameter Name="idConferente" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" DefaultValue="0" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:Parameter Name="sitPedido" Type="Int32" />
                        <asp:Parameter Name="dataConferencia" Type="String" />
                        <asp:Parameter Name="dataFinalIni" Type="String" />
                        <asp:Parameter Name="dataFinalFim" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
