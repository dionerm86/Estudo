<%@ Page Title="Finalizar/Confirmar Pedido pelo Financeiro" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstFinalizarPedidoFinanceiro.aspx.cs" Inherits="Glass.UI.Web.Listas.LstFinalizarPedidoFinanceiro" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(idPedido, isReposicao, tipo) {
            if (!isReposicao)
                openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + idPedido + "&tipo=" + tipo);
            else
                openWindow(600, 800, "../Relatorios/RelPedidoRepos.aspx?idPedido=" + idPedido + "&tipo=" + tipo);

            return false;
        }

        function openRptProj(idPedido, pcp) {
            openWindow(600, 800, "../Cadastros/Projeto/ImprimirProjeto.aspx?idPedido=" + idPedido + (pcp ? "&pcp=1" : ""));
            return false;
        }

        function openRptProm(idPedido) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=NotaPromissoria&idPedido=" + idPedido);
        }

        function finalizar(id) {
            abrirFinalizarConfirmar(id, "Finalizar");
        }

        function confirmar(id) {
            abrirFinalizarConfirmar(id, "Confirmar");
        }

        function abrirFinalizarConfirmar(id, tipo) {
            openWindow(250, 500, "../Utils/SetFinalizarFinanceiro.aspx?id=" + id + "&tipo=" + tipo);
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = LstFinalizarPedidoFinanceiro.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openRptLista(exportarExcel) {

            var numPedido = FindControl("txtNumPedido", "input").value;
            var numPedidoCli = FindControl("txtNumPedCli", "input").value;
            var numCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var numOrca = FindControl("txtNumOrca", "input").value;
            var endereco = FindControl("txtEndereco", "input").value;
            var bairro = FindControl("txtBairro", "input").value;
            var dataCadIni = FindControl("ctrlDataCadIni", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim", "input").value;
            var loja = FindControl("drpLoja", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;

            var queryString = "&numPedido=" + numPedido;
            queryString += "&numPedidoCli=" + numPedidoCli;
            queryString += "&numCli=" + numCli;
            queryString += "&nomeCli=" + nomeCli;
            queryString += "&numOrca=" + numOrca;
            queryString += "&endereco=" + endereco;
            queryString += "&bairro=" + bairro;
            queryString += "&dataCadIni=" + dataCadIni;
            queryString += "&dataCadFim=" + dataCadFim;
            queryString += "&loja=" + loja;
            queryString += "&situacao=" + situacao;
            queryString += "&altura=" + altura;
            queryString += "&largura=" + largura;
            queryString += "&exportarExcel=" + exportarExcel;

            openWindow(600, 800, '../Relatorios/RelBase.aspx?rel=ListaFinalizarPedidoFinanceiro' + queryString);
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
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Pedido Cli." ForeColor="#0066FF"></asp:Label>
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
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Num. Orçamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumOrca" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Endereço" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEndereco" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="80"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar3" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Bairro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtBairro" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                MaxLength="50"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesquisar4" runat="server" OnClick="lnkPesquisar_Click"><img border="0" 
                                src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Período Cad.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqValor0" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblTipoPedido" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblTipoPedido" runat="server" CheckAll="False" Title="Selecione o tipo"
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" ImageURL="~/Images/DropDown.png"
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Altura Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Largura Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLargura" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
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
                <asp:GridView ID="grdPedidos" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="Codigo" DataSourceID="odsPedidos"
                    GridLines="None" PageSize="15" OnRowDataBound="grdPedidos_RowDataBound" EmptyDataText="Não há pedidos para serem finalizados/confirmados pelo Financeiro.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbPedido" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("Codigo") + ", " + Eval("UsarControleReposicao").ToString().ToLower() + ", 0); return false" %>'
                                    Visible='<%# Eval("ExibirRelatorio") %>' />
                                <asp:ImageButton ID="imbMemoriaCalculo" runat="server" ImageUrl="~/Images/calculator.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("Codigo") + ", false, 1); return false" %>'
                                    ToolTip="Memória de cálculo" Visible='<%# Eval("ExibirRelatorioCalculo") %>' />
                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Nota.gif" OnClientClick='<%# "openRptProm(" + Eval("Codigo") + "); return false" %>'
                                    ToolTip="Nota promissória" Visible='<%# Eval("ExibirNotaPromissoria") %>' />
                                <asp:PlaceHolder ID="pchImprProj" runat="server" Visible='<%# Eval("ExibirImpressaoProjeto") %>'>
                                    <a href="#" onclick='openRptProj(&#039;<%# Eval("Codigo") %>&#039;, false);'>
                                        <img border="0" src="../Images/clipboard.gif" title="Projeto" /></a> </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("Codigo") %>&amp;tipo=pedido&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchRentabillidade" runat="server" Visible='<%# Glass.Configuracoes.RentabilidadeConfig.ExibirRentabilidadeFinalizarConfirmarPedidoPeloFinanceiro %>'>
                                    <a href="#" onclick='openWindow(500, 700, "../Relatorios/Rentabilidade/VisualizacaoItemRentabilidade.aspx?tipo=pedido&id=<%# Eval("Codigo") %>"); return false;'>
                                        <img border="0" src="../Images/cash_red.png" title="Rentabilidade" /></a>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodigoExibir" HeaderText="Código" ReadOnly="True" SortExpression="Codigo" />
                        <asp:BoundField DataField="PedidoCli" HeaderText="Pedido Cli." ReadOnly="True" SortExpression="PedidoCli" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" ReadOnly="True" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" ReadOnly="True" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFuncionario" HeaderText="Funcionário" ReadOnly="True"
                            SortExpression="NomeFuncionario" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" ReadOnly="True"
                            SortExpression="Total" />
                        <asp:BoundField DataField="TipoVenda" HeaderText="Pagto." ReadOnly="True" SortExpression="TipoVenda" />
                        <asp:BoundField DataField="DataPedido" DataFormatString="{0:d}" HeaderText="Data"
                            ReadOnly="True" SortExpression="DataPedido" />
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Entrega"
                            ReadOnly="True" SortExpression="DataEntrega" />
                        <asp:BoundField DataField="DescricaoSituacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="TipoPedido" HeaderText="Tipo" ReadOnly="True" SortExpression="TipoPedido" />
                        <asp:BoundField DataField="MotivoFinanceiro" HeaderText="Motivo Financeiro" SortExpression="MotivoFinanceiro" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkFinalizar" runat="server" OnClientClick='<%# Eval("Codigo", "finalizar({0}); return false") %>'
                                    Visible='<%# Eval("Finalizar") %>'>Finalizar</asp:LinkButton>
                                <asp:LinkButton ID="lnkConfirmar" runat="server" OnClientClick='<%# Eval("Codigo", "confirmar({0}); return false") %>'
                                    Visible='<%# Eval("Confirmar") %>'>Confirmar</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <table>
                    <tr>
                        <td style="text-align: left; vertical-align: top;">
                            <asp:Label ID="lblCausasFinalizacaoFinanceiro" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                        <td style="text-align: left; vertical-align: top;">
                            <asp:Label ID="lblCausasConfirmacaoFinanceiro" runat="server" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidos" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroItensFinalizarFinanceiro"
                    SelectMethod="ObtemItensFinalizarFinanceiro" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="WebGlass.Business.Pedido.Fluxo.FinalizarFinanceiro">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumOrca" Name="idOrcamento" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtEndereco" Name="endereco" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtBairro" Name="bairro" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataPedidoIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataPedidoFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtAltura" Name="alturaProd" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtLargura" Name="larguraProd" PropertyName="Text"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoProd" runat="server"
                    SelectMethod="GetSituacaoProducao" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedidoFilter"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRptLista(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRptLista(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
