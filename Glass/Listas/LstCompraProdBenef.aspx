<%@ Page Title="Gerar compra de caixa" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstCompraProdBenef.aspx.cs" Inherits="Glass.UI.Web.Listas.LstCompraProdBenef" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getRptQueryString() {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var situacaoPedOri = FindControl("cbdSituacaoPedOri", "select").itens();
            var dataIniFin = FindControl("ctrlDataIniFin_txtData", "input").value;
            var dataFimFin = FindControl("ctrlDataFimFin_txtData", "input").value;
            var idsRota = FindControl("cblRota", "select").itens();
            var compraGerada = FindControl("drpCompraGerada", "select").value;
            var dtCompraIni = FindControl("ctrlDataCompraIni_txtData", "input").value;
            var dtCompraFim = FindControl("ctrlDataCompraFim_txtData", "input").value;
            var idCompra = FindControl("txtNumCompra", "input").value;
            var dtEntIni = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dtEntFim = FindControl("ctrlDataFimEnt_txtData", "input").value;
            var ordenarPor = FindControl("drpOrdenarPor", "select").value;

            idPedido = idPedido == "" ? 0 : idPedido;
            idCliente = idCliente == "" ? 0 : idCliente;

            return "idPedido=" + idPedido + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCli + "&idLoja=" + idLoja +
                "&situacao=" + situacao + "&situacaoPedOri=" + situacaoPedOri + "&dataIniFin=" + dataIniFin + "&dataFimFin=" + dataFimFin +
                "&idsRota=" + idsRota + "&compraGerada=" + compraGerada + "&dtCompraIni=" + dtCompraIni + "&dtCompraFim=" + dtCompraFim +
                "&idCompra=" + idCompra + "&dtEntIni=" + dtEntIni + "&dtEntFim=" + dtEntFim + "&ordenarPor=" + ordenarPor;
        }

        function getCli(idCli) {
            var retorno = Listas_LstCompraProdBenef.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function validaFiltro() {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var situacaoPedOri = FindControl("cbdSituacaoPedOri", "select").itens();
            var dataIniFin = FindControl("ctrlDataIniFin_txtData", "input").value;
            var dataFimFin = FindControl("ctrlDataFimFin_txtData", "input").value;
            var idsRotas = FindControl("cblRota", "select").itens();
            var compraGerada = FindControl("drpCompraGerada", "select").value;
            var dtCompraIni = FindControl("ctrlDataCompraIni_txtData", "input").value;
            var dtCompraFim = FindControl("ctrlDataCompraFim_txtData", "input").value;
            var idCompra = FindControl("txtNumCompra", "input").value;
            var dtEntIni = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dtEntFim = FindControl("ctrlDataFimEnt_txtData", "input").value;

            if (idPedido == "" && idCliente == "" && nomeCli == "" && situacao == 0 && situacaoPedOri == 0 &&
            dataIniFin == "" && dataFimFin == "" && idsRotas == "" && compraGerada == "0" && dtCompraIni == "" && dtCompraFim == "" &&
                idCompra == "" && dtEntIni == "" && dtEntFim == "") {
                if (!confirm("É recomendável aplicar um filtro! Deseja realmente prosseguir?")) return false;
                else return true;
            }
            else return true;
        }

        function openRptProdutosCaixa(exportarExcel) {
            if (validaFiltro())
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ProdutosCaixa&" + getRptQueryString() + "&exportarExcel=" + exportarExcel);
        }

        function gerarCompraProdBenef() {
            openWindow(600, 800, "../Utils/SelPedidoEspelhoCompraProdBenef.aspx?" + getRptQueryString());
        }

        function exibirProdBenef(botao, idPedido) {
            var linha = document.getElementById("pedido_" + idPedido);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " chapas";
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
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblRota" runat="server" Width="110px" CheckAll="False"
                                Title="Selecione a rota" DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacaoPedOri" runat="server"
                                DataTextField="Descr" CheckAll="False" DataSourceID="odsSituacao"
                                DataValueField="Id" Title="Selecione a Situação">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Situação PCP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberto</asp:ListItem>
                                <asp:ListItem Value="2">Finalizado</asp:ListItem>
                                <asp:ListItem Value="3">Impresso</asp:ListItem>
                                <asp:ListItem Value="4">Impresso Comum</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Período Finalização PCP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniFin" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimFin" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Compra Gerada" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCompraGerada" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Selecione uma opção</asp:ListItem>
                                <asp:ListItem Value="1">Com compra gerada</asp:ListItem>
                                <asp:ListItem Value="2">Sem compra gerada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Num. Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCompra" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>

                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Data da Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCompraIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCompraFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Ordenar Por:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenarPor" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Nenhum</asp:ListItem>
                                <asp:ListItem Value="1">Data Entrega</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPedidoEspelho" runat="server" AllowPaging="True" AllowSorting="True"
                    DataKeyNames="IdPedido" DataSourceID="odsPedidoEspelho"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum pedido encontrado com o filtro aplicado." AutoGenerateColumns="False">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdBenef(this, " + Eval("IdPedido") + "); return false;" %>'
                                    Width="10px" ToolTip="Exibir produtos" Visible="true" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="CompraGerada" HeaderText="Compras" SortExpression="CompraGerada" />
                        <asp:BoundField DataField="NomeInicialCli" HeaderText="Cliente" SortExpression="NomeInicialCli" />
                        <asp:BoundField DataField="Total" HeaderText="Total Conf." SortExpression="Total"
                            DataFormatString="{0:C}"></asp:BoundField>
                        <asp:BoundField DataField="DataEspelho" DataFormatString="{0:d}" HeaderText="Data Conf."
                            SortExpression="DataEspelho" />
                        <asp:BoundField DataField="DataConf" DataFormatString="{0:d}" HeaderText="Finalização"
                            SortExpression="DataConf" />
                        <asp:TemplateField HeaderText="Total m² / Qtde." SortExpression="TotM">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
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
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr>
                                <tr align="center" id="pedido_<%# Eval("IdPedido").ToString() %>" style="display: none" class="<%= GetAlternateClass() %>">
                                    <td colspan="11">
                                        <asp:GridView ID="grdProdBenef" runat="server"
                                            DataSource='<%# Eval("ProdutosBenefCompra") %>'
                                            Width="100%" ShowFooter="False" AutoGenerateColumns="False"
                                            CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                            EditRowStyle-CssClass="edit">
                                            <PagerSettings PageButtonCount="20" />
                                            <Columns>
                                                <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                                                <asp:BoundField DataField="RotaCliente" HeaderText="Rota" SortExpression="RotaCliente" />
                                                <asp:BoundField DataField="Qtde" HeaderText="Quantidade" SortExpression="Qtde" />
                                                <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                                                <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                                                <asp:TemplateField HeaderText="Prof." SortExpression="">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("ProfundidadeCaixa") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Espessura" HeaderText="Espessura" SortExpression="Espessura" />
                                                <asp:TemplateField HeaderText="Obs Caixa" SortExpression="">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("DescrProdutoBenef") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="DescrProduto" HeaderText="Descrição Produto" SortExpression="DescrProduto" />
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidoEspelho" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountCompraProdBenefSel" SelectMethod="GetListCompraProdBenefSel"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoEspelhoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="cbdSituacaoPedOri" Name="situacaoPedOri" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniFin" Name="dataIniFin" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimFin" Name="dataFimFin" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="cblRota" Name="idsRota" PropertyName="SelectedValue" Type="String" />
                        <asp:ControlParameter ControlID="drpCompraGerada" Name="compraGerada" PropertyName="SelectedValue" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataCompraIni" Name="dtCompraIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCompraFim" Name="dtCompraFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dtEntregaPedIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dtEntregaPedFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpOrdenarPor" Name="ordenarPor" PropertyName="SelectedValue" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedidoPCP"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkGerarCompraCaixa" runat="server" OnClientClick="gerarCompraProdBenef(); return false;">
                                <img alt="" border="0" src="../Images/basket_go.gif" /> Gerar compra de Caixas</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkProdutosCaixa" runat="server" OnClientClick="openRptProdutosCaixa(false); return false;">
                                <img alt="" border="0" src="../Images/printer.png" /> Relatório de Caixas</asp:LinkButton>
                        </td>
                        <td></td>
                        <td align="center">
                            <asp:LinkButton ID="lnkProdutosCaixaExcel" runat="server" OnClientClick="openRptProdutosCaixa(true); return false;">
                                <img alt="" border="0" src="../Images/Excel.gif" /> Exportar para o Excel Caixas</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
