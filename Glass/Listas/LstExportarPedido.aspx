<%@ Page Title="Exportar Pedido" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstExportarPedido.aspx.cs" Inherits="Glass.UI.Web.Listas.LstExportarPedido" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function selecionaTodosProdutos(check)
        {
            var tabela = check;
            while (tabela.nodeName.toLowerCase() != "table")
            tabela = tabela.parentNode;

            var checkBoxProdutos = tabela.getElementsByTagName("input");

            var i = 0;
            for (i = 0; i < checkBoxProdutos.length; i++)
            {
                if (checkBoxProdutos[i].id.indexOf("chkTodos") > -1 || checkBoxProdutos[i].disabled)
                continue;

                checkBoxProdutos[i].checked = check.checked;
            }    
        }
        function exibirProdutos(botao, idPedido)
        {
            var liberarProdutos = <%= Glass.Configuracoes.Liberacao.DadosLiberacao.LiberarPedidoProdutos.ToString().ToLower() %>;
            var exibirProdutosPedidoAoLiberar = <%= Glass.Configuracoes.PedidoConfig.ExibirProdutosPedidoAoLiberar.ToString().ToLower() %>;

            if (!liberarProdutos && !exibirProdutosPedidoAoLiberar)
                return;

            var linha = document.getElementById("produtos_" + idPedido);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " produtos";
        }

        function checkAll(checked) {
            var tabela = document.getElementById("<%= grdPedido.ClientID %>");
            var inputs = tabela.getElementsByTagName("input");

            for (i = 0; i < inputs.length; i++) {
                if (inputs[i].id.indexOf("chkTodos") > -1 && inputs[i].id.indexOf("Benef") > -1)
                    continue;

                if (inputs[i].id.indexOf("Benef") == -1)
                    inputs[i].checked = checked;
            }
        }

        function checkAllBenef(checked) {
            var tabela = document.getElementById("<%= grdPedido.ClientID %>");
            var inputs = tabela.getElementsByTagName("input");

            for (i = 0; i < inputs.length; i++) {
                if (inputs[i].id.indexOf("chkTodos") > -1 && inputs[i].id.indexOf("Benef") > -1)
                    continue;

                if (inputs[i].id.indexOf("Benef") > -1)
                    inputs[i].checked = checked;
            }
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function Validar() {

            var fornecedor = FindControl("ddlFornecedor", "select").value;

            if (fornecedor == "0") {
                alert("Selecione o fornecedor.");
                return false;
            }
            else {
                bloquearPagina();
                desbloquearPagina(false);
            }
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Num. Ped. Cli." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                    OnRowDataBound="grdPedido_RowDataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkMarcar" runat="server" />
                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkTodos" runat="server" onclick="checkAll(this.checked)" />
                            </HeaderTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdutos(this, " + Eval("IdPedido") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir produtos" Visible="<%# Glass.Configuracoes.Liberacao.DadosLiberacao.LiberarPedidoProdutos || Glass.Configuracoes.PedidoConfig.ExibirProdutosPedidoAoLiberar %>" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="IdProjeto" HeaderText="Proj." SortExpression="IdProjeto" />
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Orça." SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DescrTipoVenda" HeaderText="Pagto" SortExpression="DescrTipoVenda">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataPedido" DataFormatString="{0:d}" HeaderText="Data"
                            SortExpression="DataPedido" />
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Entrega"
                            SortExpression="DataEntrega" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacaoPedido">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescricaoTipoPedido" HeaderText="Tipo" SortExpression="DescricaoTipoPedido">
                            <ItemStyle Wrap="True" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkMarcarBenef" runat="server" Checked="true" ToolTip="Define exportação de beneficiamentos"
                                    Text="Exportar Beneficiamentos?" />
                                <asp:HiddenField ID="hdfIdPedidoBenef" runat="server" Value='<%# Eval("IdPedido") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkTodosBenef" runat="server" Checked="true" ToolTip="Marcar/Desmarcar todos - Exportação de beneficiamentos"
                                    onclick="checkAllBenef(this.checked)" Text="Exportar Beneficiamentos (todos)?" />
                            </HeaderTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr>
                                <tr id="produtos_<%# Eval("IdPedido") %>" style="display: none">
                                    <td>
                                    </td>
                                    <td colspan="13" style="padding: 0px">
                                        <asp:GridView ID="grdProdutosPedido" runat="server" AutoGenerateColumns="False" CellPadding="3"
                                            DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedido" GridLines="None" Width="100%"
                                            OnRowDataBound="grdProdutosPedido_RowDataBound" ShowFooter="True" OnDataBound="grdProdutosPedido_DataBound"
                                            
                                            EmptyDataText="Não foram encontrados produtos do grupo Vidro que não sejam de estoque ou que não foram exportados.">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <FooterTemplate>
                                                        Total
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkTodos" runat="server" onclick="selecionaTodosProdutos(this)"
                                                            Checked="True" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder ID="pchInicio" runat="server"></asp:PlaceHolder>
                                                        <asp:CheckBox ID="chkSelProdPed" runat="server" Checked="True"
                                                            Style="margin-left: -1px; margin-right: 1px" />
                                                        <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProd") %>' />
                                                        <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                                        <asp:HiddenField ID="hdfIdProdPedProducao" runat="server" Value='<%# Eval("IdProdPedProducaoConsulta") %>' />
                                                        <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# ((System.Drawing.Color)Eval("CorLinha")).Name %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="1%" />
                                                    <HeaderStyle Width="1%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cod." SortExpression="CodInterno">
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox10" runat="server" Text='<%# Bind("CodInterno") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCodInterno" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDescricao" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                                                        <asp:Label ID="lblBenef" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Qtde" SortExpression="QtdeOriginal">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Qtde") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Altura" SortExpression="AlturaLista">
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAltura" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("Largura") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLargura" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotM" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total" SortExpression="TotalCalc">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotal" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:GridView>
                                        <asp:HiddenField ID="hdfIdPedidoProdutos" runat="server" Value='<%# Eval("IdPedido") %>' />
                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosPedido" runat="server" SelectMethod="ObterProdutosNaoExportados"
                                            TypeName="Glass.Data.DAL.ProdutosPedidoDAO" >
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdPedidoProdutos" Name="idPedido" PropertyName="Value"
                                                    Type="UInt32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" SelectMethod="GetForPedidoExportar"
                    TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" Type="String" />
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
                                DataTextField="Nomefantasia" DataValueField="IdFornec" AppendDataBoundItems="True"
                                Width="200px">
                                <asp:ListItem Value="0">Selecione</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnExportar" runat="server" Text="Exportar Pedidos Selecionados"
                                OnClientClick="return Validar();" OnClick="btnExportar_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" SelectMethod="ObterFornecedoresComUrlSistema"
        TypeName="Glass.Data.DAL.FornecedorDAO" >
    </colo:VirtualObjectDataSource>

    <script>
        FindControl("txtNumPedido", "input").focus();
    </script>

</asp:Content>
