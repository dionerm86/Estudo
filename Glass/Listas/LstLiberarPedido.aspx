<%@ Page Title="Consultar Liberações de Pedidos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstLiberarPedido.aspx.cs" Inherits="Glass.UI.Web.Listas.LstLiberarPedido" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlBoleto.ascx" TagName="ctrlBoleto" TagPrefix="uc7" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function cancelarLiberacao(idLiberarPedido) {
            openWindow(200, 500, "../Utils/SetMotivoCancLib.aspx?idLiberarPedido=" + idLiberarPedido);
        }

        function validaHora(val, args) {
            var partes = args.Value.split(':');
            if (partes.length < 2) {
                args.IsValid = false;
                return;
            }

            for (i = 0; i < partes.length; i++)
                if (partes[i].length != 2) {
                args.IsValid = false;
                return;
            }

            args.IsValid = true;
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

        function openRpt(idLiberarPedido, relatorioCompleto, cliente) {
            openWindow(600, 800, "../Relatorios/RelLiberacao.aspx?idLiberarPedido=" + idLiberarPedido + "&relatorioCompleto=" + relatorioCompleto + "&EnvioEmail=" + cliente);
            return false;
        }

        function openRptList(exportarExcel) {
            var idLiberacao = FindControl("txtNumLiberacao", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var numeroNfe = FindControl("txtNumeroNfe", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var horaIni = FindControl("ctrlDataIni_txtHora", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var horaFim = FindControl("ctrlDataFim_txtHora", "input").value;
            var idFunc = FindControl("drpFunc", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var dataIniCanc = FindControl("ctrlDataIniCanc_txtData", "input").value;
            var dataFimCanc = FindControl("ctrlDataFimCanc_txtData", "input").value;
            var horaIniCanc = FindControl("ctrlDataIniCanc_txtHora", "input").value;
            var horaFimCanc = FindControl("ctrlDataFimCanc_txtHora", "input").value;
            var liberacaoNf = FindControl("cbLiberarcaoNfe", "select").value;

            if (horaIni != "") horaIni = " " + horaIni;
            if (horaFim != "") horaFim = " " + horaFim;
            if (idLiberacao == "") idLiberacao = 0;
            if (idPedido == "") idPedido = 0;
            if (idCliente == "") idCliente = 0;
            if (idFunc == "") idFunc = 0;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaLiberacao&idLiberarPedido=" + idLiberacao + "&idPedido=" + idPedido +
            "&numeroNfe=" + numeroNfe + "&situacao=" + situacao + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente +
            "&liberacaoNf=" + liberacaoNf + "&dataIni=" + dataIni + horaIni + "&dataFim=" + dataFim + horaFim +
            "&idFunc=" + idFunc + "&idLoja=" + idLoja + "&dataIniCanc=" + dataIniCanc + horaIniCanc + "&dataFimCanc=" + dataFimCanc + horaFimCanc + "&exportarExcel=" + exportarExcel);

            return false;
        }

        function openRptProm(idLiberarPedido) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=NotaPromissoria&idLiberarPedido=" + idLiberarPedido);
        }

        function openGerarNfe(idLiberarPedido) {
            redirectUrl("../Cadastros/CadNotaFiscalGerar.aspx?idLiberarPedido=" + idLiberarPedido);
        }

        function openListaTotal() {
            var idLiberacao = FindControl("txtNumLiberacao", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var numeroNfe = FindControl("txtNumeroNfe", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var horaIni = FindControl("ctrlDataIni_txtHora", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var horaFim = FindControl("ctrlDataFim_txtHora", "input").value;
            var idFunc = FindControl("drpFunc", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var liberacaoNf = FindControl("cbLiberarcaoNfe", "select").value;

            if (horaIni != "") horaIni = " " + horaIni;
            if (horaFim != "") horaFim = " " + horaFim;
            if (idLiberacao == "") idLiberacao = 0;
            if (idPedido == "") idPedido = 0;
            if (idCliente == "") idCliente = 0;
            if (idFunc == "") idFunc = 0;

            openWindow(60, 160, "../Utils/ListaTotalLiberacao.aspx?idLiberarPedido=" + idLiberacao + "&idPedido=" + idPedido +
            "&numeroNfe=" + numeroNfe + "&situacao=" + situacao + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente +
            "&liberacaoNf=" + liberacaoNf + "&dataIni=" + dataIni + horaIni + "&dataFim=" + dataFim + horaFim +
            "&idFunc=" + idFunc + "&idLoja=" + idLoja);
        }

        function confirmarReenvioEmail() {
            return confirm("Deseja realmente reenviar o e-mail da liberação?");
        }
    </script>

    <table>
        <tr>
            <td align="center" style="height: 106px">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Num. Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumLiberacao" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label13" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumeroNfe" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" runat="server" Width="60px"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label12" runat="server" Text="Liberado por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpFunc" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsFunc" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label1" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">Liberado</asp:ListItem>
                                <asp:ListItem Value="2">Cancelado</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:DropDownList ID="cbLiberarcaoNfe" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Selected="True">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Apenas liberações com nota fiscal</asp:ListItem>
                                <asp:ListItem Value="2">Apenas liberações sem nota fiscal</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Período Cancelamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIniCanc" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFimCanc" runat="server" ReadOnly="ReadWrite" ExibirHoras="true" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdLiberarPedido" runat="server" AllowPaging="True"
                    AllowSorting="True" DataSourceID="odsLiberarPedido" AutoGenerateColumns="False"
                    DataKeyNames="IdLiberarPedido" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhuma liberação de pedido encontrada."
                    OnRowCommand="grdLiberarPedido_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("IdLiberarPedido") + ", false, false" + "); return false" %>'
                                    ToolTip="Relatório"  Visible='<%# (bool)Eval("ImprimirRelatorioLiberacaoVisible") %>' />
                                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Report.png"
                                    OnClientClick='<%# "openRpt(" + Eval("IdLiberarPedido") + ", true, false" + "); return false" %>'
                                    ToolTip="Relatório Completo" Visible='<%# ExibirRelatorioCompleto() %>' />
                                <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/RelatorioCliente.png"
                                    OnClientClick='<%# "openRpt(" + Eval("IdLiberarPedido") + ", false, true" + "); return false" %>'
                                    ToolTip="Relatório do Cliente" Visible='<%# ExibirRelatorioCliente() %>' />
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Nota.gif" OnClientClick='<%# "openRptProm(" + Eval("IdLiberarPedido") + "); return false" %>'
                                    ToolTip="Nota promissória" Visible='<%# (bool)Eval("ExibirNotaPromissoria") && !SomenteConsulta() %>' />
                                <asp:ImageButton ID="imbGerarNfe" runat="server" ImageUrl="~/Images/script_go.gif"
                                    ToolTip="Gerar NF-e" Visible='<%# (bool)Eval("ExibirGerarNf") && !SomenteConsulta() %>' OnClientClick='<%# "openGerarNfe(" + Eval("IdLiberarPedido") + "); return false" %>' />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandArgument='<%# Eval("IdLiberarPedido") %>'
                                    CommandName="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick='<%# "cancelarLiberacao(" + Eval("IdLiberarPedido") + "); return false" %>'
                                    ToolTip="Cancelar liberação" Visible='<%# (bool)Eval("CancelarVisible") && !SomenteConsulta() %>' />
                                <asp:PlaceHolder ID="pchAnexos" runat="server" Visible='<%# !SomenteConsulta() %>'><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdLiberarPedido") %>&tipo=liberacao&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                                <uc7:ctrlBoleto ID="ctrlBoleto1" runat="server" CodigoLiberacao='<%# Eval("IdLiberarPedido") != null ? Glass.Conversoes.StrParaInt(Eval("IdLiberarPedido").ToString()) : (int?)null %>'
                                    Visible='<%# Eval("BoletoVisivel") %>' />
                                <asp:ImageButton ID="imbReenviarEmail" runat="server" CommandArgument='<%# Eval("IdLiberarPedido") %>'
                                    CommandName="ReenviarEmail" ImageUrl="~/Images/email.png" ToolTip="Reenviar e-mail da liberação" 
                                    OnClientClick='<%# "return confirmarReenvioEmail();" %>'
                                    Visible='<%# (bool)Eval("ExibirReenvioEmail") && !SomenteConsulta() %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdLiberarPedido" HeaderText="Num. Liberação" SortExpression="IdLiberarPedido" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" SortExpression="IdCliente" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DescrPagto" HeaderText="Pagto." SortExpression="DescrPagto" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total" />
                        <asp:BoundField DataField="ValorIcms" DataFormatString="{0:c}" HeaderText="Valor ICMS"
                            SortExpression="ValorIcms" />
                        <asp:BoundField DataField="TotalSemIcms" DataFormatString="{0:c}" HeaderText="Total s/ ICMS"
                            SortExpression="TotalSemIcms" />
                        <asp:BoundField DataField="DataLiberacao" HeaderText="Data" SortExpression="DataLiberacao" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Image ToolTip='<%# "Notas fiscais geradas: " + Eval("NotasFiscaisGeradas") %>'
                                    runat="server" Visible='<%# Eval("ExibirNfeGerada") %>' ImageUrl="../Images/blocodenotas.png"
                                    ID="imgNfeGerada" Style="cursor: pointer" />
                                    <uc3:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="LiberacaoReenvioEmail" IdRegistro='<%# Eval("IdLiberarPedido") %>' />
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
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Image ID="Image1" runat="server" ImageUrl="../Images/Clipe.gif" />
                <asp:LinkButton ID="lnkAnexos" runat="server" OnClientClick="openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=0&tipo=liberacao&#039;); return false;"
                    ToolTip="Anexar arquivos à várias liberações">Anexar arquivos à várias liberações</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRptList();return false;"> 
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir
                </asp:LinkButton>
                 &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRptList(true); return false;">
                    <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
               &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkImprimirTotais" runat="server" OnClientClick="return openListaTotal(); return false;"
                    ToolTip="Exibe os valores de preço, peso e m² totais das liberações listadas."> <img 
                    alt="" border="0" src="../Images/detalhes.gif" /> Total</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLiberarPedido" runat="server"
                    DataObjectTypeName="Glass.Data.Model.LiberarPedido" DeleteMethod="Delete" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.LiberarPedidoDAO"
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" SkinID="">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumLiberacao" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumeroNfe" Name="numeroNfe" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpFunc" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="cbLiberarcaoNfe" Name="liberacaoNf" PropertyName="SelectedValue"
                            Type="Int32" />
                           <asp:ControlParameter ControlID="ctrlDataIniCanc" Name="dataIniCanc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimCanc" Name="dataFimCanc" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFunc" runat="server" SelectMethod="GetFuncLiberacao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
