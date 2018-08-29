<%@ Page Title="Confirmar Pedido" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadConfirmarPedidoLiberacao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadConfirmarPedidoLiberacao" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function checkAll(checked) {
            var tabela = document.getElementById("<%= grdPedido.ClientID %>");
            var inputs = tabela.getElementsByTagName("input");
            var calcularTotM = false;

            for (i = 0; i < inputs.length; i++) {
                if (inputs[i].id.indexOf("chkTodos") > -1 || inputs[i].type != "checkbox")
                    continue;

                // Verifica se a marcação do check box está sendo alterada.
                calcularTotM = inputs[i].checked != checked;

                inputs[i].checked = checked;

                // Calcula o total de metro quadrado somente se o método checkAll estiver alterando a situação do check box.
                if (calcularTotM)
                    calculaTotM(inputs[i]);
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

        function alterarDataEntrega(checked) {
            FindControl("ctrlDataEntrega_txtData", "input").disabled = !checked;
            FindControl("ctrlDataEntrega_imgData", "input").disabled = !checked;
            if (FindControl("ctrlDataEntrega_txtData", "input").disable == true)
                FindControl("ctrlDataEntrega_txtData", "input").value = "";
        }

        function getValoresMarcados(nomeCampo) {
            var retorno = new Array();
            var tabela = document.getElementById("<%= grdPedido.ClientID %>");

            for (i = 0; i < tabela.rows.length; i++) {
                var checkbox = FindControl("chkMarcar", "input", tabela.rows[i]);
                if (checkbox == null || !checkbox.checked)
                    continue;

                var campo = FindControl(nomeCampo, "input", tabela.rows[i]);
                if (campo.value != "")
                    retorno.push(campo.value);
            }

            return retorno.join(",");
        }

        function openRptPedidos() {
            var pedidos = getValoresMarcados("hdfIdPedido");

            // Imprime todos os pedidos em apenas um PDF
            if (pedidos != "")
                openWindow(600, 800, "../Relatorios/RelPedido.aspx?tipo=0&idPedido=" + pedidos);

            // Imprime os pedidos em PDFs separados
            //for (var i = 0; i < idsPedido.split(',').length; i++)
            //    openWindow(600, 800, "../Relatorios/RelPedido.aspx?tipo=0&idPedido=" + idsPedido.split(',')[i]);
        }

        function openRptProjetos() {
            var idsItensProjeto = getValoresMarcados("hdfIdItensProjeto");
            if (idsItensProjeto.length > 0)
                openWindow(600, 800, "../Relatorios/Projeto/RelBase.aspx?rel=imagemProjeto&idItemProjeto=" + idsItensProjeto);
        }

        function openRptCorEspessura() {
            var idsPedido = getValoresMarcados("hdfIdPedido");

            if (idsPedido.length > 0)
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=CorEspessura&idsPedido=" + idsPedido);
        }

        var data;

        function openRptConf(idsPedidos) {
            var rel = openWindow(600, 800, "../Relatorios/RelBase.aspx?postData=getPostData()");
            data = new Object();
            data["rel"] = "ListaPedidos";
            data["idsPedidos"] = idsPedidos;
        }

        function getPostData() {
            return data;
        }

        function calculaTotM(controle) {
            var totM2Selecionado = FindControl("lblTotalM2", "span").innerHTML;
            totM2Selecionado = totM2Selecionado != "" ? parseFloat(totM2Selecionado.toString().replace(",", ".")) : parseFloat(0);
            var totM2Pedido = controle.parentNode.parentNode.childNodes[9].innerHTML;
            totM2Pedido = totM2Pedido != "" ? parseFloat(totM2Pedido.toString().replace(",", ".")) : parseFloat(0);

            if (controle.checked)
                FindControl("lblTotalM2", "span").innerHTML = (totM2Selecionado + totM2Pedido).toFixed(2);
            else if (!controle.checked)
                FindControl("lblTotalM2", "span").innerHTML = (totM2Selecionado - totM2Pedido).toFixed(2);
        }
    
    </script>

    <table style="width: 100%">
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
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" MostrarTodas="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label28" runat="server" ForeColor="#0066FF" Text="Origem Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrigemPedido" runat="server" AutoPostBack="true">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Normal</asp:ListItem>
                                <asp:ListItem Value="2">E-commerce</asp:ListItem>
                                <asp:ListItem Value="3">Importado</asp:ListItem>
                            </asp:DropDownList>
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
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Tipo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoPedido" runat="server" 
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" >
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AutoGenerateColumns="False"
                    OnRowDataBound="grdPedido_RowDataBound" DataSourceID="odsPedido" CssClass="gridStyle" AllowSorting="True"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    DataKeyNames="IdPedido" EmptyDataText="Nenhum pedido encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkMarcar" runat="server" onclick="calculaTotM(this);" />
                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                <asp:HiddenField ID="hdfIdItensProjeto" runat="server" Value='<%# Eval("IdItensProjeto") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkTodos" runat="server" onclick="checkAll(this.checked)" />
                            </HeaderTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedidoExibir" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="IdProjeto" HeaderText="Proj." SortExpression="IdProjeto" />
                        <asp:BoundField DataField="IdOrcamento" HeaderText="Orça." SortExpression="IdOrcamento" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente"/>
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="TotM" HeaderText="Tot. M2" />
                        <asp:BoundField DataField="DescrTipoVenda" HeaderText="Pagto">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataPedido" DataFormatString="{0:d}" HeaderText="Data"
                            SortExpression="DataPedido" />
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Entrega" SortExpression="DataEntrega" />
                        <asp:TemplateField HeaderText="Situação">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacaoPedido") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="True" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescricaoTipoPedido" HeaderText="Tipo" >
                            <ItemStyle Wrap="True" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" 
                    SelectMethod="GetForConfirmation"
                     SortParameterName="sortExpression"
                    TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:QueryStringParameter Name="revenda" QueryStringField="revenda" Type="Boolean" />
                        <asp:QueryStringParameter Name="liberarPedido" QueryStringField="liberarPedido" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpLoja" PropertyName="SelectedValue" Name="idLoja"
                            Type="UInt32" />
                         <asp:ControlParameter ControlID="drpOrigemPedido" PropertyName="SelectedValue" Name="origemPedido"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpTipoPedido" PropertyName="SelectedValue" Name="tipoPedido"/>
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedido"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="Label5" runat="server" Text="Total M2 selecionado: " Font-Size="12pt"
                    ForeColor="Blue"></asp:Label>
                <asp:Label ID="lblTotalM2" runat="server" Font-Bold="True" Font-Size="12pt" ForeColor="Blue">0.00</asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimirPedidos" runat="server" OnClientClick="openRptPedidos(); return false"> <img src="../Images/Printer.png" border="0" /> Imprimir Pedidos Selecionados</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkImprimirProjetos" runat="server" OnClientClick="openRptProjetos(); return false"> <img src="../Images/Clipboard.gif" border="0" /> Imprimir Projetos dos Pedidos Selecionados</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimirCorEspessura" runat="server" OnClientClick="openRptCorEspessura(); return false"> <img src="../Images/Printer.png" border="0" /> Imprimir Cor/Espessura dos Pedidos Selecionados</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:CheckBox ID="chkGerarEspelho" runat="server" Text="Conferência dos pedidos já realizada" />
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkAlterarDataEntrega" runat="server" Text="Alterar data de entrega dos pedidos"
                                onclick="alterarDataEntrega(this.checked)" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />

                            <script type="text/javascript">
                                alterarDataEntrega(FindControl('chkAlterarDataEntrega', 'input').checked);
                            </script>

                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar Pedidos" OnClick="btnConfirmar_Click"
                    OnClientClick="bloquearPagina(); desbloquearPagina(false);" />
            </td>
        </tr>
    </table>
</asp:Content>
