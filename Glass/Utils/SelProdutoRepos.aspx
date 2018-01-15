<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelProdutoRepos.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelProdutoRepos" Title="Produtos para Reposição" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function selecionar(idProduto, etiqueta)
        {
            if (!confirm("Deseja adicionar esse produto à lista de reposição?"))
                return false;

            window.opener.adicionarProdutoCallback(idProduto, etiqueta);
            setTimeout(atualizarPagina, 500);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsProdutosRepos" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdProdPed"
                    PageSize="12" EmptyDataText="Não há mais produtos para reposição neste pedido.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbSelecionar" runat="server" Height="16px" ImageUrl="~/Images/ok.gif"
                                    OnClientClick='<%# "selecionar(" + Eval("IdProdPed") + ", \"" + Eval("NumEtiquetaConsulta") + "\"); return false;" %>'
                                    ToolTip="Selecionar" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescrProduto") + ((bool)Eval("Redondo") ? " REDONDO" : "") + (Eval("DescrBeneficiamentos").ToString().Length > 0 ? " " + Eval("DescrBeneficiamentos") : "") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") + ((bool)Eval("Redondo") ? " REDONDO" : "") %>'></asp:Label>
                                &nbsp;
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodProdIns" runat="server" Width="50px" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                    onkeypress="return !(isEnter(event));" onblur="loadProduto(this.value);"></asp:TextBox>
                                <asp:Label ID="lblDescrProd0" runat="server"></asp:Label>
                                <a href="#" onclick="openWindow(450, 700, '../Utils/SelProd.aspx?IdPedido=<%= Request["IdPedido"] %>'); return false;">
                                    <img src="../Images/Pesquisar.gif" border="0" /></a>
                                <asp:HiddenField ID="hdfValMin0" runat="server" />
                                <asp:HiddenField ID="hdfIsVidro0" runat="server" />
                                <asp:HiddenField ID="hdfIsAluminio0" runat="server" />
                                <asp:HiddenField ID="hdfIsMoldura0" runat="server" />
                                <asp:HiddenField ID="hdfM2Minimo0" runat="server" />
                            </FooterTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumEtiquetaConsulta" HeaderText="Etiqueta" SortExpression="NumEtiquetaConsulta" />
                        <asp:BoundField DataField="DescrPerdaRepos" HeaderText="Tipo Perda" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="calcM2Prod();" Text='<%# Bind("AlturaString") %>'
                                    onkeypress="return soNumeros(event, !(FindControl('hdfIsAluminio', 'input').value == 'true'), true);"
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAlturaIns0" runat="server" onkeypress="return soNumeros(event, !prodInsAluminio, true);"
                                    onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# (bool)Eval("Redondo") ? "0" : Eval("Largura") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLarguraIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, true, true);"
                                    Text='<%# Bind("Largura") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLarguraIns0" runat="server" onkeypress="return soNumeros(event, true, true);"
                                    onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total M2" SortExpression="TotM">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Math.Round(float.Parse(Eval("TotM").ToString()) / float.Parse(Eval("Qtde").ToString()), 2) %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblTotM2Ins" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotM2Ins0" runat="server"></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorVendido", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorIns" runat="server" onblur="calcTotalProd();" onkeypress="return soNumeros(event, false, true);"
                                    Text='<%# Bind("ValorString") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorIns0" runat="server" onkeydown="if (isEnter(event)) calcTotalProd();"
                                    onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();"
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                onkeypress="return !(isEnter(event));" Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns0" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao0" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="loadProc(this.value);" onkeydown="if (isEnter(event)) loadProc(this.value);"
                                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick='openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns0" runat="server" onblur="loadProc(this.value);" onkeydown="if (isEnter(event)) loadProc(this.value);"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick='openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProcesso0" runat="server" Value='<%# Bind("IdProcesso") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("IdProcesso") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosRepos" runat="server" MaximumRowsParameterName=""
                    SelectMethod="GetForRepos" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
                    >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedidoRepos" QueryStringField="IdPedido" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
