<%@ Page Title="Débitos" Language="C#" MasterPageFile="~/WebGlassParceiros/PainelParceiros.master"
    AutoEventWireup="true" CodeBehind="LstDebitos.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.LstDebitos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt()
        {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLiberarPedido = FindControl("txtIdLiberarPedido", "input");

            var queryString = idPedido == "" ? "&idPedido=0" : "&idPedido=" + idPedido;
            queryString += idLiberarPedido == null || idLiberarPedido.value == "" ? "&idLiberarPedido=0" : "&idLiberarPedido=" + idLiberarPedido;

            openWindow(600, 800, "RelBase.aspx?Rel=Debitos" + queryString);
            return false;
        }

        function openRptPedido(idPedido, tipo) {
            openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + idPedido + "&tipo=" + tipo);
            return false;
        }

        function openRptLiberacao(idLiberarPedido, relatorioCompleto, cliente) {
            openWindow(600, 800, "../Relatorios/RelLiberacao.aspx?idLiberarPedido=" + idLiberarPedido + "&relatorioCompleto=" + relatorioCompleto + "&EnvioEmail=" + cliente + "&ApenasViaCliente=true");
            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label7" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label2" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdLiberarPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdContaR" DataSourceID="odsContasReceber" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum débito encontrado." AllowPaging="True" PageSize="30">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbPedido" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRptPedido(" + Eval("IdPedido") + ", 0);" %>'
                                    ToolTip="Imprimir pedido" Visible='<%# Eval("IdPedido") != null %>' />
                                <asp:ImageButton ID="imbLiberacao" runat="server" ImageUrl="~/Images/RelatorioCliente.png"
                                    OnClientClick='<%# "openRptLiberacao(" + Eval("IdLiberarPedido") + ", false, true);" %>'
                                    ToolTip="Imprimir Liberação" Visible='<%# Eval("IdLiberarPedido") != null %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                        <asp:BoundField DataField="IdNomeCli" HeaderText="Cliente" SortExpression="NomeCli" />
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ValorVec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotal" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ValorVec", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVec">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DataVec") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DataVec", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ForeColor="Red" Font-Bold="true" Font-Size="Large" ID="lblCreditoCliente" OnLoad="lblCreditoCliente_Load" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasReceber" runat="server" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetDebitosListParceiros" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ContasReceberDAO" EnablePaging="True" SelectCountMethod="GetDebitosCountParceiros"
                    SortParameterName="sortExpression" >
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="idCliente" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtIdLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:Parameter Name="buscarChequesEmAberto" Type="Boolean" />
                        <asp:Parameter Name="ordenar" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
