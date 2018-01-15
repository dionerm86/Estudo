<%@ Page Title="Impressão Individual de Etiqueta" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstEtiquetaIndImp.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaIndImp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function setProdEtiqueta(idProdPed, idProdNf, idAmbientePedido) {
            if (!<%= PodeImprimir() %>)
            {
                alert("Você não tem permissão para reimprimir etiquetas.");
                return false;
            }
            
            var numEtiqueta = FindControl("txtNumEtiqueta", "input").value;
            var id = idProdPed > 0 ? "idProdPed=" + idProdPed : idProdNf > 0 ? "idProdNf=" + idProdNf : "idAmbientePedido=" + idAmbientePedido;
            
            openWindow(500, 700, "../Relatorios/RelEtiquetas.aspx?ind=1&" + id + "&numEtiqueta=" + numEtiqueta + "&tipo=0");
        }

        function leuEtiqueta(txtNumEtiqueta) {
            if (txtNumEtiqueta == null || txtNumEtiqueta == undefined)
                return;

            txtNumEtiqueta.value = corrigeLeituraEtiqueta(txtNumEtiqueta.value);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label1" runat="server" Text="Número NF-e" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumeroNFe" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Descrição Prod." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label5" runat="server" Text="Num. Etiqueta" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumEtiqueta" runat="server" Width="100px" onkeypress="if (isEnter(event)) return leuEtiqueta(this);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td style="color: #0066FF">
                            Processo
                        </td>
                        <td>
                            <asp:DropDownList ID="drpProcesso" runat="server" AppendDataBoundItems="True" DataSourceID="odsProcesso"
                                DataTextField="CodInterno" DataValueField="CodInterno">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td style="color: #0066FF">
                            Aplicação
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAplicacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsAplicacao"
                                DataTextField="CodInterno" DataValueField="CodInterno">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Altura início" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAlturaIni" runat="server" Width="50px" MaxLength="5" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Altura fim" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAlturaFim" runat="server" Width="50px" MaxLength="5" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Largura início" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLarguraIni" runat="server" Width="50px" MaxLength="5" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Largura fim" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLarguraFim" runat="server" Width="50px" MaxLength="5" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:GridView GridLines="None" ID="grdProduto" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProd" DataSourceID="odsProduto"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" 
                    EmptyDataText="Nenhum produto encontrado ou nenhum filtro utilizado." 
                    OnRowDataBound="grdProduto_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setProdEtiqueta('<%# Eval("IdProdPed") %>', '<%# Eval("IdProdNf") %>', '<%# Eval("IdAmbientePedido") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pedido" SortExpression="IdPedido">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" 
                                    Text='<%# (uint)Eval("IdPedido") > 0 ? Eval("IdPedido").ToString() : "" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdPedido") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Número NF-e" SortExpression="NumeroNFe">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" 
                                    Text='<%# (int)Eval("NumeroNFe") > 0 ? Eval("NumeroNFe").ToString() : "" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" 
                            SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                        </asp:BoundField>
                        <asp:BoundField DataField="CodProcesso" HeaderText="Proc." SortExpression="CodProcesso">
                        </asp:BoundField>
                        <asp:BoundField DataField="CodAplicacao" HeaderText="Apl." SortExpression="CodAplicacao">
                        </asp:BoundField>
                        <asp:BoundField DataField="Qtde" HeaderText="Qtd." SortExpression="Qtde" />
                        <asp:BoundField DataField="QtdImpresso" HeaderText="Qtd. Já Impresso" SortExpression="QtdImpresso" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <div style="color: Red">
                    Etiquetas em Vermelho são de pedidos de reposição.
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountImpIndiv" SelectMethod="GetListImpIndiv" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO"
                    >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroNFe" Name="numeroNFe" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumEtiqueta" Name="numEtiqueta" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtAlturaIni" Name="alturaIni" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtAlturaFim" Name="alturaFim" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtLarguraIni" Name="larguraIni" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtLarguraFim" Name="larguraFim" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="drpProcesso" Name="codProcesso" PropertyName="SelectedValue"
                            Type="String" />
                            <asp:ControlParameter ControlID="drpAplicacao" Name="codAplicacao" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProcesso" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.EtiquetaProcessoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAplicacao" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        leuEtiqueta(FindControl("txtNumEtiqueta", "input"));
    </script>

</asp:Content>
