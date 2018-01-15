<%@ Page Title="Pendências de Carregamento" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaPendenciaCarregamento.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaPendenciaCarregamento" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function getCli(idCli) {
            if (idCli.value == "") {
                openWindow(570, 760, '../Utils/SelCliente.aspx');
                return false;
            }

            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openRptInd(idCarregamento, idCliente, idClienteExterno, clienteExterno) {
            if (idCarregamento == null || idCarregamento == "") {
                alert("Informe o carregamento.");
                return false;
            }

            if (idCliente == null || idCliente == "") {
                alert("Informe o cliente.");
                return false;
            }

            var dataSaidaIni = FindControl("ctrlDataSaidaIni_txtData", "input").value;
            var dataSaidaFim = FindControl("ctrlDataSaidaFim_txtData", "input").value;
            var rotas = FindControl("cblRota", "select").itens();
            var codRotasExternas = FindControl("cblRotaExterna", "select").itensText();

            openWindow(500, 700, "../Relatorios/RelBase.aspx?rel=PendenciaCarregamento&idCarregamento=" + idCarregamento
                + "&idCliente=" + idCliente + "&dataSaidaIni=" + dataSaidaIni + "&dataSaidaFim=" + dataSaidaFim + "&rotas=" + rotas + "&codRotasExternas=" + codRotasExternas +
                "&idClienteExterno=" + idClienteExterno + "&nomeClienteExterno=" + clienteExterno, null, true, true);

            return false;
        }

        function openRpt(exportarExcel) {

            var idCarregamento = FindControl("txtCodCarregamento", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var dataSaidaIni = FindControl("ctrlDataSaidaIni_txtData", "input").value;
            var dataSaidaFim = FindControl("ctrlDataSaidaFim_txtData", "input").value;
            var rotas = FindControl("cblRota", "select").itens();
            var ignorarPedidoVendaTransferencia = FindControl("chkIgnorarPedidoVendaTransferencia", "input").checked;
            var codRotasExternas = FindControl("cblRotaExterna", "select").itensText();
            var idClienteExterno = FindControl("txtNumCliExterno", "input").value;
            var nomeClienteExterno = FindControl("txtNomeClienteExterno", "input").value;

            var queryString = "../Relatorios/RelBase.aspx?rel=ListaPendenciaCarregamento";
            queryString += "&idCarregamento=" + idCarregamento;
            queryString += "&idLoja=" + idLoja;
            queryString += "&idCliente=" + idCliente;
            queryString += "&nomeCliente=" + nomeCliente;
            queryString += "&dataSaidaIni=" + dataSaidaIni;
            queryString += "&dataSaidaFim=" + dataSaidaFim;
            queryString += "&rotas=" + rotas;
            queryString += "&ignorarPedidoVendaTransferencia=" + ignorarPedidoVendaTransferencia;
            queryString += "&exportarExcel=" + exportarExcel;
            queryString += "&codRotasExternas=" + codRotasExternas;
            queryString += "&idClienteExterno=" + idClienteExterno;
            queryString += "&nomeClienteExterno=" + nomeClienteExterno;
        
            openWindow(500, 700, queryString);

            return false;
        }
        
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Carregamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodCarregamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true"
                                MostrarTodas="false" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Período (Saída)" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataSaidaIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataSaidaFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
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
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table ID="tbClienteExterno" runat="server">
                    <tr>
                         <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente Externo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCliExterno" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeClienteExterno" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Rota Externa" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                             <sync:CheckBoxListDropDown ID="cblRotaExterna" runat="server" Width="200px" CheckAll="False"
                                Title="Selecione a rota" DataSourceID="odsRotasExternas" DataTextField="Descr" DataValueField="Id"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRotasExternas" runat="server" SelectMethod="GetRotasExternas"
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" OnClick="imgPesq_Click"/>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkIgnorarPedidoVendaTransferencia" runat="server" Checked="True"
                                Text="Ignorar pedidos de venda com transferência" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" runat="server" ID="gvrPendencia" AllowPaging="True"
                    AllowSorting="False" DataSourceID="odsPendencia" AutoGenerateColumns="False"
                    EmptyDataText="Nenhuma pendência encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" Style="min-width: 400px;">
                    <Columns>
                        <asp:TemplateField HeaderText="Carregamento">
                            <ItemTemplate>
                                <asp:Label ID="lblCarregamento" runat="server" Text='<%# Eval("IdCarregamento") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente">
                            <ItemTemplate>
                                <asp:Label ID="lblCliente" runat="server" Text='<%# Eval("IdNomeCliente") + (((uint)Eval("IdClienteExterno")) > 0 ? " (" + Eval("IdClienteExterno") + " - " + Eval("ClienteExterno") + ")" : "")  %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Peso Pendente">
                            <ItemTemplate>
                                <asp:Label ID="lblPesoTotal" runat="server" Text='<%# Eval("PesoTotal") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbRelInd" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    ToolTip="Visualizar Pendência" OnClientClick='<%# "openRptInd(" + Eval("IdCarregamento") + ", " + Eval("IdCliente") + "," + Eval("IdClienteExterno") + "); return false;" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsPendencia" runat="server" SelectMethod="GetListagemPendenciaCarregamento"
                    SelectCountMethod="GetListagemPendenciaCarregamentoCount" TypeName="WebGlass.Business.OrdemCarga.Fluxo.PendenciaCarregamentoFluxo"
                    Culture="pt-BR" SortParameterName="sortExpression" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodCarregamento" Name="idCarregamento" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataSaidaIni" Name="dtSaidaIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSaidaFim" Name="dtSaidaFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblRota" Name="rotas" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkIgnorarPedidoVendaTransferencia" Name="ignorarPedidosVendaTransferencia"
                            PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="txtNumCliExterno" Name="idCliExterno" PropertyName="Text" Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNomeClienteExterno" Name="nomeCliExterno" PropertyName="Text" Type="String" />
                                    <asp:ControlParameter ControlID="cblRotaExterna" Name="idsRotasExternas" PropertyName="SelectedItem"
                                        Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
