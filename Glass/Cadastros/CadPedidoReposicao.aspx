<%@ Page Title="Cadastrar Reposição de Pedido" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadPedidoReposicao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPedidoReposicao"
    EnableEventValidation="false" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function adicionarProduto()
        {
            openWindow(590, 760, '../Utils/SelProdutoRepos.aspx?idPedido=<%= Request["IdPedido"] %>');
        }

        function adicionarProdutoCallback(idProduto, etiqueta)
        {
            // Adiciona o produto
            var resposta = CadPedidoReposicao.AddProduto(idProduto, '<%= Request["IdPedido"] %>', '<%= hdfIdAmbiente.Value %>', etiqueta).value;

            // Verifica se houve erro
            if (resposta != "")
            {
                alert(resposta);
                return;
            }

            // Atualiza a página
            redirectUrl(window.location.href);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Panel ID="panFiltro" runat="server">
                    <table cellpadding="0" cellspacing="0">
                        <tr valign="middle">
                            <td>Número do Pedido:&nbsp;
                            </td>
                            <td align="center">
                                <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                    runat="server" onkeydown="if (isEnter(event)) cOnClick('imbPesq', null);"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="valNumPedido" runat="server" ControlToValidate="txtNumPedido"
                                    ErrorMessage="*"></asp:RequiredFieldValidator>&nbsp;
                                <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imbPesq_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <table style="width: 100%">
                    <tr>
                        <td align="center">
                            <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                                GridLines="None" Visible="False" EmptyDataText="&lt;br&gt;Pedido não encontrado."
                                OnDataBound="dtvPedido_DataBound">
                                <Fields>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Num. Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNumPedido" runat="server" Text='<%# Eval("IdPedido") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Funcionário
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Tipo Venda
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoVenda" runat="server" Text='<%# Eval("DescrTipoVenda") %>'
                                                            ForeColor="Red"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Loja
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Valor Entrada
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblValorEnt" runat="server" Text='<%# Eval("ValorEntrada") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Tipo Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoEntrega" runat="server" Text='<%# Eval("DescrTipoEntrega") %>'></asp:Label>
                                                    </td>
                                                    <td id="dataEntregaTitulo" align="left" nowrap="nowrap" style="font-weight: bold">Data Entrega
                                                    </td>
                                                    <td id="dataEntrega" align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDataEntrega" runat="server" Visible='<%# panFiltro.Visible %>'
                                                            Text='<%# Eval("DataEntrega", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                                        <asp:TextBox ID="txtDataEntrega" runat="server" Width="80px" Visible='<%# !panFiltro.Visible %>'
                                                            onkeypress="return false;" Text='<%# Eval("DataEntrega", "{0:dd/MM/yyyy}") %>'></asp:TextBox>
                                                        <asp:ImageButton ID="imgDataEntrega" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                                            Visible='<%# !panFiltro.Visible %>' OnClientClick="return SelecionaData('txtDataEntrega', this)"
                                                            ToolTip="Alterar" />

                                                        <script type="text/javascript">
                                                            if (<%= (EsconderCamposDataEntrega() && !panFiltro.Visible).ToString().ToLower() %>)
                                                            {
                                                                document.getElementById("dataEntregaTitulo").style.display = "none";
                                                            document.getElementById("dataEntrega").style.display = "none";
                                                            }
                                                        </script>

                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Situação
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacaoPedido") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Data Ped.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                                                    </td>
                                                    <td id="dataClienteTitulo" align="left" nowrap="nowrap" style="font-weight: bold">Data Cliente Inf.
                                                    </td>
                                                    <td id="dataCliente" align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataCliente" runat="server" Width="80px" Text='' onkeypress="return false;"
                                                            OnLoad="txtDataCliente_Load"></asp:TextBox>
                                                        <asp:ImageButton ID="imgDataCliente" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                                            OnClientClick="return SelecionaData('txtDataCliente', this)" ToolTip="Alterar" />

                                                        <script type="text/javascript">
                                                            if (<%= panFiltro.Visible.ToString().ToLower() %>)
                                                            {
                                                                document.getElementById("dataClienteTitulo").style.display = "none";
                                                            document.getElementById("dataCliente").style.display = "none";
                                                            }
                                                        </script>

                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Desconto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDesconto" runat="server" Text='<%# Eval("Desconto", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label13" runat="server" Text="Comissão" Visible='<%# Eval("ComissaoVisible") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblComissao" runat="server" Text='<%# Eval("ValorComissao", "{0:C}") %>'
                                                            Visible='<%# Eval("ComissaoVisible") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="Label14" runat="server" Font-Bold="True" Text="Total"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Forma Pagto.
                                                    </td>
                                                    <td align="left" colspan="3" nowrap="nowrap">
                                                        <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("PagtoParcela") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Reposição Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblPedAnterior" runat="server" Text='<%# Eval("IdPedidoAnterior") %>'
                                                            ForeColor="Red"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                            <asp:HiddenField ID="hdfCliRevenda" runat="server" Value='<%# Eval("CliRevenda") %>' />
                                            <asp:HiddenField ID="hdfTipoVenda" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Button ID="btnSalvar" runat="server" Text="Salvar datas" OnDataBinding="ReposicaoButtons_DataBinding"
                                                OnClick="btnSalvar_Click" />
                                            <asp:Button ID="btnFinalizar" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                Text="Finalizar" OnClientClick="return confirm(&quot;Finalizar pedido?&quot;);"
                                                OnClick="btnFinalizar_Click" OnDataBinding="ReposicaoButtons_DataBinding" />
                                            <asp:Button ID="btnRepor" runat="server" OnClick="btnRepor_Click" OnClientClick="return confirm(&quot;Escolher esse pedido para reposição?&quot;)"
                                                Text="Iniciar reposição" OnDataBinding="ReposicaoButtons_DataBinding" />
                                            <br />
                                            <br />
                                            <asp:Label ID="lblErro" runat="server" ForeColor="Red"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td align="center">
                <div id="divProduto" runat="server">
                    <table>
                        <tr>
                            <td align="center">
                                <asp:LinkButton ID="lkbAdicionarProduto" runat="server" OnClientClick="adicionarProduto(); return false;"
                                    Visible="False">Adicionar produto</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" CssClass="gridStyle"
                                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                    DataKeyNames="IdProdPed" OnRowDeleted="grdProdutos_RowDeleted" OnPreRender="grdProdutos_PreRender"
                                    PageSize="12" Visible="False" OnRowDeleting="grdProdutos_RowDeleting" OnRowUpdated="grdProdutos_RowUpdated">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Visible='<%# !Glass.Configuracoes.ProducaoConfig.ReporApenasProduzidos %>'>
                                                    <img border="0" src="../Images/Edit.gif" /></asp:LinkButton>
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Deseja excluir este produto da lista de reposição?&quot;)" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Cancelar" CausesValidation="False" />
                                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                                <asp:HiddenField ID="hdfIdProdPedAnterior" runat="server" Value='<%# Bind("IdProdPedAnterior") %>' />
                                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                                <asp:HiddenField ID="hdfCodInterno" runat="server" Value='<%# Eval("CodInterno") %>' />
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" Value='<%# Eval("M2Minimo") %>' />
                                                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                                                <asp:HiddenField ID="hdfIdMaterItemProj" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                                                <asp:HiddenField ID="hdfIdAmbientePedido" runat="server" Value='<%# Bind("IdAmbientePedido") %>' />
                                            </EditItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescrProduto") + ((bool)Eval("Redondo") ? " REDONDO" : "") + (Eval("DescrBeneficiamentos").ToString().Length > 0 ? " " + Eval("DescrBeneficiamentos") : "") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                                &nbsp;
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtCodProdIns" runat="server" Width="50px" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                                    onkeypress="return !(isEnter(event));" onblur="loadProduto(this.value);"></asp:TextBox>
                                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                                <a href="#" onclick="openWindow(450, 700, '../Utils/SelProd.aspx?IdPedido=<%= Request["IdPedido"] %>'); return false;">
                                                    <img src="../Images/Pesquisar.gif" border="0" /></a>
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                                <asp:HiddenField ID="hdfIsMoldura" runat="server" />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="True" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Etiqueta Orig." SortExpression="NumEtiquetaRepos">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("NumEtiquetaRepos") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("NumEtiquetaRepos") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tipo Perda Orig.">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("DescrPerdaRepos") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("DescrPerdaRepos") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                            <ItemTemplate>
                                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtQtde" runat="server" Text='<%# Bind("Qtde") %>' Width="50px"
                                                    onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                                <asp:RangeValidator ID="rgvQtde" runat="server" ControlToValidate="txtQtde" Display="Dynamic"
                                                    MinimumValue="1" OnDataBinding="rgvQtde_DataBinding" Type="Integer"></asp:RangeValidator>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAlturaIns" runat="server" onkeypress="return soNumeros(event, !prodInsAluminio, true);"
                                                    onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# (bool)Eval("Redondo") ? "0" : Eval("Largura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                    onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total M2" SortExpression="TotM">
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotM2Ins" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotM2Ins" runat="server"></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorVendido", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("ValorVendido", "{0:C}") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfValorVendido" runat="server" Value='<%# Bind("ValorVendido") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtValorIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProd();"
                                                    onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();"
                                                    Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("IdAplicacao") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label100" runat="server" Text='<%# Bind("IdProcesso") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="loadProc(this.value);" onkeydown="if (isEnter(event)) loadProc(this.value);"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick='openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label1000" runat="server" Text='<%# Bind("IdProcesso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotalIns" runat="server" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Eval("Total") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotalIns" runat="server"></asp:Label>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfValorBenef" runat="server" Value='<%# Eval("ValorBenef") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblValorBenef" runat="server"></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("ValorBenef", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvPedidoRepos" runat="server" AutoGenerateRows="False" DataSourceID="odsPedidoRepos"
                    DefaultMode="Edit" DataKeyNames="IdPedRepos" GridLines="None" Visible="False">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table class="gridStyle">
                                    <tr>
                                        <th colspan="2">Dados da reposição
                                        </th>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblLoja" runat="server" Text="Loja: "></asp:Label>
                                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True" DataSourceID="odsLoja"
                                                DataTextField="NomeFantasia" DataValueField="IdLoja"
                                                OnLoad="Loja_Load" Width="250px">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Assunto
                                            <br />
                                            <asp:TextBox ID="TextBox1" runat="server" MaxLength="1000" Rows="5" TextMode="MultiLine"
                                                Width="300px" Text='<%# Bind("Assunto") %>'></asp:TextBox>
                                        </td>
                                        <td>Solução
                                            <br />
                                            <asp:TextBox ID="TextBox2" runat="server" MaxLength="1000" Rows="5" TextMode="MultiLine"
                                                Width="300px" Text='<%# Bind("Solucao") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="<%# ExibirTroca() %>" class="alt">
                                        <td colspan="2" align="center">
                                            <asp:CheckBox ID="chkPermitirTroca" runat="server" Checked='<%# Bind("PodeUtilizarTroca") %>'
                                                Text="Cobrar pedido de reposição se não houver troca?" Enabled='<%# HabilitarPodeTrocar() %>' />
                                        </td>
                                    </tr>
                                </table>
                                <div style="text-align: center">
                                    <asp:HiddenField ID="hdfDataCli" runat="server" Value='<%# Bind("DataClienteInformado") %>' />
                                    <br />
                                    <asp:Button ID="btnAtualizar" runat="server" Text="Atualizar dados da reposição"
                                        CommandName="Update" />
                                </div>
                                <asp:HiddenField ID="IdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidoRepos" runat="server"
                    SelectMethod="GetByPedido" TypeName="Glass.Data.DAL.PedidoReposicaoDAO" DataObjectTypeName="Glass.Data.Model.PedidoReposicao"
                    UpdateMethod="Update" OnInserted="odsPedidoRepos_Inserted"
                    OnUpdated="odsPedidoRepos_Updated" OnUpdating="odsPedidoRepos_Updating">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="IdPedido" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdAmbiente" runat="server" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
                    InsertMethod="Insert" UpdateMethod="Update">
                    <UpdateParameters>
                        <asp:Parameter DefaultValue="true" Name="isReposicao" />
                    </UpdateParameters>
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdPedido0" runat="server" />
                <asp:HiddenField ID="hdfIdProd0" runat="server" />
                <asp:HiddenField ID="hdfComissaoVisible" runat="server" Value='<%# Eval("ComissaoVisible") %>' />
                <asp:HiddenField ID="hdfCurrPage" runat="server" Value="0" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoDAO"
                    UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <sync:ObjectDataSource ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </sync:ObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
