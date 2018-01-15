<%@ Page Title="Pedidos" Language="C#" MasterPageFile="~/WebGlassParceiros/PainelParceiros.master"
    AutoEventWireup="true" CodeBehind="LstPedidos.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.LstPedidos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function openRptUnico(idPedido, tipo) {
        openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + idPedido + "&tipo=" + tipo);
        return false;
    }

    function openRptProj(idPedido, pcp)
    {
        openWindow(600, 800, "ImprimirProjeto.aspx?idPedido=" + idPedido + (pcp ? "&pcp=1" : ""));
        return false;
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Pedido Cli." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPedidoCli" runat="server" Width="80px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPedido" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdPedido" EmptyDataText="Nenhum pedido encontrado."
                    AllowPaging="True" OnRowDataBound="grdPedido_RowDataBound" OnRowCommand="grdPedido_RowCommand">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    OnClientClick='<%# "redirectUrl(\"CadPedido.aspx?idPedido=" + Eval("IdPedido") + "\"); return false" %>'
                                    Visible='<%# (uint)Eval("IdPedido") > 0 && (bool)Eval("EditVisible") %>' />
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# (uint)Eval("IdPedido") > 0 && !(bool)Eval("ExibirImpressaoPcp") %>'>
                                    <a href="#" onclick="openRptUnico('<%# Eval("IdPedido") %>', 0);">
                                        <img border="0" src="../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# (uint)Eval("IdPedido") > 0 && (bool)Eval("ExibirImpressaoPcp") %>'>
                                    <a href="#" onclick="openRptUnico('<%# Eval("IdPedido") %>', 2);">
                                        <img border="0" src="../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchImprProj" runat="server" Visible='<%# (uint)Eval("IdPedido") > 0 && (bool)Eval("ExibirImpressaoProjeto") %>'>
                                    <a href="#" onclick='openRptProj(&#039;<%# Eval("IdPedido") %>&#039;, <%# (UsarImpressaoProjetoPcp() && (bool)Eval("TemAlteracaoPcp")).ToString().ToLower() %>);'>
                                        <img border="0" src="../Images/clipboard.gif" title="Projeto" /></a> </asp:PlaceHolder>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/EditarGrid.gif"
                                    OnClientClick='<%# "redirectUrl(\"" + ResolveUrl("~/WebglassParceiros/") + "CadProjeto.aspx?idProjeto=" + Eval("IdProjeto") + "\"); return false" %>'
                                    Visible='<%# (uint)Eval("IdPedido") == 0 %>' />
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdPedido") %>&tipo=pedido&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="../Images/Nota.gif" Visible='<%# (bool)Eval("EditObsLiberacaoVisible") %>'
                                    OnClientClick='<%# "openWindow(250, 500, \"AlterarObsLiberacaoPedido.aspx?idPedido=" + Eval("IdPedido") + "\"); return false" %>'
                                    ToolTip="Observação Liberação/Faturamento/Entrega"  />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedidoExibir" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="CodCliente" HeaderText="Pedido Cli." SortExpression="CodCliente" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Total", "{0:C}") %>' Visible='<%# !(bool)Eval("ExibirTotalEspelho") %>'></asp:Label>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("TotalEspelho", "{0:C}") %>'
                                    Visible='<%# Eval("ExibirTotalEspelho") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Entrega"
                            SortExpression="DataEntrega" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>'></asp:Label>
                                        </td>
                                        <td style="padding-left: 4px">
                                            <asp:ImageButton ID="imbReabrir" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                CommandName="Reabrir" ImageUrl="~/Images/cadeado.gif" OnClientClick="return confirm(&quot;Deseja reabrir esse pedido?&quot;)"
                                                ToolTip="Reabrir pedido" Visible='<%# Eval("ExibirReabrir") %>' />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrSituacaoProducao" HeaderText="Situação Produção"
                            SortExpression="DescrSituacaoProducao" />
                        <asp:BoundField DataField="TotM" HeaderText="Total m²" SortExpression="TotM" />
                        <asp:BoundField DataField="Peso" HeaderText="Peso total" SortExpression="Peso" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetListAcessoExterno" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoDAO"
                    EnablePaging="True" SelectCountMethod="GetAcessoExternoCount" SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPedidoCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dtIni" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dtFim" PropertyName="Text" Type="String" />
                        <asp:Parameter DefaultValue="false" Name="apenasAbertos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
