<%@ Page Title="Finalizar Conferência de Pedido" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadPedidoEspelhoFinalizarMult.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPedidoEspelhoFinalizarMult" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
    
    function finalizar()
    {
        if (!confirm("Finalizar pedidos?"))
            return false;
        
        return true;
    }
        
    function checkAll(checked)
    {
        var tabela = document.getElementById("<%= grdPedido.ClientID %>");
        var inputs = tabela.getElementsByTagName("input");
        
        for (i = 0; i < inputs.length; i++)
        {
            if (inputs[i].id.indexOf("chkTodos") > -1)
                continue;
            
            inputs[i].checked = checked;
        }
    }

    function getCli(idCli) {
        if (idCli.value == "")
            return;

        var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
        
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNome", "input").value = "";
            return false;
        }
        
        FindControl("txtNome", "input").value = retorno[1];
    }

    function getValoresMarcados(nomeCampo, tipoCampo, sep) {
        var retorno = new Array();
        var tabela = document.getElementById("<%= grdPedido.ClientID %>");

        for (i = 0; i < tabela.rows.length; i++) {
            var checkbox = FindControl("chkMarcar", "input", tabela.rows[i]);
            if (checkbox == null || !checkbox.checked)
                continue;

            var campo = FindControl(nomeCampo, tipoCampo, tabela.rows[i]);
            if (tipoCampo == "input" && campo.value != "")
                retorno.push(campo.value);
            else if (tipoCampo == "span" && campo.innerHTML != "")
                retorno.push(campo.innerHTML);
        }

        return retorno.join(sep);
    }

    function alterarDataEntrega(checked)
    {
        FindControl("ctrlDataEntrega_txtData", "input").disabled = !checked;
        FindControl("ctrlDataEntrega_imgData", "input").disabled = !checked;
        if (FindControl("ctrlDataEntrega_txtData", "input").disable == true)
            FindControl("ctrlDataEntrega_txtData", "input").value = "";
    }

    function openRptPedidos() {
        var idsPedidos = getValoresMarcados("hdfIdPedido", "input", ",");
        if (idsPedidos.length > 0)
            openWindow(600, 800, "../Relatorios/RelPedido.aspx?tipo=0&idPedido=" + idsPedidos);

    }

    function openRptProjetos() {
        var idsItensProjeto = getValoresMarcados("hdfIdItensProjeto", "input", ",");
        if (idsItensProjeto.length > 0)
            openWindow(600, 800, "../Relatorios/Projeto/RelBase.aspx?rel=imagemProjeto&idItemProjeto=" + idsItensProjeto);
        else
            alert("Não há projeto(s) nos pedido(s) selecionado(s).");
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
                            <asp:Label ID="Label2" runat="server" Text="Período Conf. Pedido" ForeColor="#0066FF"></asp:Label>
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
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataSourceID="odsPedido" DataKeyNames="IdPedido"
                    EmptyDataText="Nenhum pedido em produção encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkMarcar" runat="server" />
                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                <asp:HiddenField ID="hdfIdItensProjeto" runat="server" Value='<%# Eval("IdItensProjeto") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkTodos" runat="server" onclick="checkAll(this.checked)" />
                            </HeaderTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeInicialCli" HeaderText="Cliente" SortExpression="NomeInicialCli" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="Conferente" HeaderText="Conferente" SortExpression="Conferente" />
                        <asp:TemplateField HeaderText="Total Pedido" SortExpression="TotalPedido">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("TotalPedido") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTotalPed" runat="server" Text='<%# Bind("TotalPedido", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total Conf." SortExpression="Total">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTotalEsp" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataEspelho" DataFormatString="{0:d}" HeaderText="Data Conf."
                            SortExpression="DataEspelho" />
                        <asp:TemplateField HeaderText="Total m² / Qtde." SortExpression="TotM">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("TotM") + " (" + Eval("QtdePecas") + " pç.)" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Peso" HeaderText="Peso" SortExpression="Peso" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacao") %>' Style='<%# (int)Eval("Situacao") == (int)Glass.Data.Model.PedidoEspelho.SituacaoPedido.Finalizado ? "position: relative; bottom: 3px": "" %>'>
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemStyle VerticalAlign="Middle" Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Entrega" SortExpression="DataEntrega">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DataEntrega") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("DataEntregaExibicao", "{0:d}") + ((bool)Eval("FastDelivery") ? " - Fast Del." : "") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Fábrica" SortExpression="DataFabrica">
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("DataFabrica", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("DataFabrica") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" SelectMethod="GetForFinalizarMult"
                    TypeName="Glass.Data.DAL.PedidoEspelhoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIniConf" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFimConf" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
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
            <td align="center">
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
                <span style="color: Blue">A data de fábrica dos pedidos será recalculada se a data de
                    entrega for alterada. </span>
                <br />
                <br />
                <br />
                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar conferências" OnClick="btnFinalizar_Click"
                    OnClientClick="if (!finalizar()) return false; bloquearPagina(); desbloquearPagina(false)" />
            </td>
        </tr>
    </table>

    <script>
        FindControl("txtNumPedido", "input").focus();
    </script>

</asp:Content>
