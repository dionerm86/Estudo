<%@ Page Title="Controle de Pagamentos Antecipados de Obra" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstObra.aspx.cs" Inherits="Glass.UI.Web.Listas.LstObra" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(idObra, obraDetalhada, exportarExcel) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Obra&idObra=" + idObra + "&obraDetalhada=" + obraDetalhada + "&exportarExcel=" + exportarExcel);
            return false;
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = LstObra.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

        function openRptGerarCredito(exportarExcel) {
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var idFunc = FindControl("drpFuncionarioObra", "select").value;
            var idFuncCad = FindControl("drpFuncionarioCad", "select").value;
            var idFormaPagto = FindControl("drpFormaPagto", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataFinIni = FindControl("ctrlDataFinIni_txtData", "input").value;
            var dataFinFim = FindControl("ctrlDataFinFim_txtData", "input").value;
            var idObra = FindControl("txtNumPagto", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var descricao = FindControl("txtDescricao", "input").value;
            var agrupar = FindControl("drpAgrupar", "select").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ObraGerarCredito&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente +
                "&situacao=" + situacao + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&dataFinIni=" + dataFinIni + "&dataFinFim=" + dataFinFim +
                "&idFunc=" + idFunc + "&idFuncCad=" + idFuncCad + "&idFormaPagto=" + idFormaPagto + "&idObra=" + idObra + "&idPedido=" + idPedido + "&descricao=" + descricao +
                "&agrupar=" + agrupar + "&exportarExcel=" + exportarExcel);

            return false;
        }

        function openRptPagtoAnt(exportarExcel) {
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var idFunc = FindControl("drpFuncionarioObra", "select").value;
            var idFuncCad = FindControl("drpFuncionarioCad", "select").value;
            var idFormaPagto = FindControl("drpFormaPagto", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataFinIni = FindControl("ctrlDataFinIni_txtData", "input").value;
            var dataFinFim = FindControl("ctrlDataFinFim_txtData", "input").value;
            var idObra = FindControl("txtNumPagto", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var descricao = FindControl("txtDescricao", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ObraPagAntecipado&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente +
                "&situacao=" + situacao + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&dataFinIni=" + dataFinIni + "&dataFinFim=" + dataFinFim +
                "&idFunc=" + idFunc + "&idFuncCad=" + idFuncCad + "&idFormaPagto=" + idFormaPagto + "&idObra=" + idObra + "&idPedido=" + idPedido + "&descricao=" + descricao +
                "&exportarExcel=" + exportarExcel);

            return false;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblNumPagto" runat="server" Text="Num. Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPagto" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onblur="getCli(this);" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" Style="margin-bottom: 0px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Funcionário Obra"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionarioObra" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsFuncionarioObra" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" ForeColor="#0066FF" Text="Funcionário Cad."></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionarioCad" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsFuncionarioCad" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberta</asp:ListItem>
                                <asp:ListItem Value="5">Aguardando Financeiro</asp:ListItem>
                                <asp:ListItem Value="4">Confirmada</asp:ListItem>
                                <asp:ListItem Value="3">Finalizada</asp:ListItem>
                                <asp:ListItem Value="2">Cancelada</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <table id="tbNumPedido" runat="server">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label7" runat="server" Text="Num. pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumPedido" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" Width="250px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Forma Pagto."></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="True" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto">
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Período cadastro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc2:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                            <uc2:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período finalização" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc2:ctrlData ID="ctrlDataFinIni" runat="server" ReadOnly="ReadWrite" />
                            <uc2:ctrlData ID="ctrlDataFinFim" runat="server" ReadOnly="ReadWrite" />
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="lblAgrupar" runat="server" ForeColor="#0066FF" Text="Agrupar relatório por"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="1">Cliente</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imbAgrupar" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:LinkButton ID="lbkInserir" runat="server" PostBackUrl="~/Cadastros/CadObra.aspx">Inserir pagamento antecipado</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdObra" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsObra" DataKeyNames="IdObra" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Não há obras cadastradas." OnRowCommand="grdObra_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" Visible='<%# Eval("EditVisible") %>'
                                    ImageUrl="~/Images/EditarGrid.gif" NavigateUrl='<%# "~/Cadastros/CadObra" + (!GerarCreditoObra() && UsarControleNovoObra() ? "Novo" : "") + ".aspx?idObra=" + Eval("IdObra") + (Request["cxDiario"] == "1" ? "&cxDiario=1" : "") %>'></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar Obra" OnClientClick='<%# "openWindow(200, 500, \"../Utils/SetMotivoCancReceb.aspx?tipo=obra&id=" + Eval("IdObra") + "\"); return false" %>'
                                    Visible='<%# Eval("CancelVisible") %>' />
                                <a href="#" onclick='openRpt(&#039;<%# Eval("IdObra") %>&#039;, false, false);'>
                                    <img border="0" src="../Images/Relatorio.gif" title="Impressão Obra" /></a>
                                <a href="#" onclick='openRpt(&#039;<%# Eval("IdObra") %>&#039;, true, false);'>
                                    <img border="0" src="../Images/detalhes.gif" title="Impressão Obra Detalhada" /></a>
                                <a href="#" onclick='openRpt(&#039;<%# Eval("IdObra") %>&#039;, true, true);'>
                                    <img border="0" src="../Images/excel.gif" title="Exportar para o Excel Obra Detalhada" /></a>
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdObra") %>&tipo=obra&gerarCredito=<%# Request["gerarCredito"] %>&#039;); return false;'>
                                    <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdObra" HeaderText="Num. pag." SortExpression="IdObra" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" SortExpression="IdNomeCliente" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <table class="pos" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgReabrir" runat="server" ImageUrl="~/Images/Cadeado.gif" Visible='<%# Eval("ReabrirVisible") %>'
                                                CommandArgument='<%# Eval("IdObra") %>' CommandName="Reabrir" />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ValorObra" DataFormatString="{0:c}" HeaderText="Valor Obra"
                            SortExpression="ValorObra" />
                        <asp:BoundField DataField="Saldo" DataFormatString="{0:C}" HeaderText="Saldo" SortExpression="Saldo">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário Obra" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="NomeFuncCad" HeaderText="Funcionário Cad." SortExpression="NomeFuncCad" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data de cadastro"
                            SortExpression="DataCad" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkFinalizar" runat="server" CommandArgument='<%# Eval("IdObra") %>'
                                    CommandName="Finalizar" OnClientClick="return confirm(&quot;Tem certeza que deseja finalizar esta obra?&quot;);"
                                    Visible='<%# Eval("FinalizarVisible") %>'>Finalizar</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc3:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="Obra" IdRegistro='<%# Eval("IdObra") %>' />
                                <uc1:ctrlLogCancPopup ID="ctrlLogCancPopup1" runat="server" IdRegistro='<%# Eval("IdObra") %>' Tabela="Obra" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsObra" runat="server" DataObjectTypeName="Glass.Data.Model.Obra"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ObraDAO" SelectCountMethod="GetListCount" OnDeleted="odsObra_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionarioObra" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionarioCad" Name="idFuncCad" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataFinIni" Name="dataFinIni" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="ctrlDataFinFim" Name="dataFinFim" PropertyName="DataString"
                            Type="DateTime" />
                        <asp:ControlParameter ControlID="hdfGerarCredito" Name="gerarCredito" PropertyName="Value"
                            Type="Boolean" />
                        <asp:Parameter Name="idsPedidosIgnorar" Type="String" />
                        <asp:ControlParameter ControlID="txtNumPagto" Name="idObra" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionarioCad" runat="server"
                    SelectMethod="GetFinanceiros" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionarioObra" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForConsultaContasReceber"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfGerarCredito" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimirGerarCred" runat="server" OnClientClick="openRptGerarCredito(); return false;"
                    Visible="False"> <img 
                    border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcelGerarCred" runat="server" OnClientClick="openRptGerarCredito(true); return false;"
                    Visible="False"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimirPagtoAnt" runat="server" OnClientClick="openRptPagtoAnt(); return false;"> <img 
                    border="0" src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;
                <asp:LinkButton ID="lnkExportarExcelPagtoAnt" runat="server" OnClientClick="openRptPagtoAnt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
