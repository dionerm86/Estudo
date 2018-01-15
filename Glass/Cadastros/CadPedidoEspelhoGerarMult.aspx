<%@ Page Title="Gerar Conferência de Pedido" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadPedidoEspelhoGerarMult.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPedidoEspelhoGerarMult" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        
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

    function alterarDataEntrega(checked)
    {
        FindControl("ctrlDataEntrega_txtData", "input").disabled = !checked;
        FindControl("ctrlDataEntrega_imgData", "input").disabled = !checked;
        if (FindControl("ctrlDataEntrega_txtData", "input").disable == true)
            FindControl("ctrlDataEntrega_txtData", "input").value = "";
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
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AutoGenerateColumns="False" DataSourceID="odsPedido"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdPedido" EmptyDataText="Nenhum pedido encontrado.">
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
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" SelectMethod="GetForPedidoEspelhoGerar"
                    TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtNumPedCli" Name="codCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:CheckBox ID="chkFinalizarEspelho" runat="server" Text="Conferência dos pedidos já realizada" />
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
                <asp:Button ID="btnGerarEspelho" runat="server" Text="Gerar conferências" OnClick="btnGerarEspelho_Click" OnClientClick="bloquearPagina(); desbloquearPagina(false)" />
            </td>
        </tr>
    </table>

    <script>
        FindControl("txtNumPedido", "input").focus();
    </script>

</asp:Content>
