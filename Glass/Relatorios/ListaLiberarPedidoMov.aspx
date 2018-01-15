<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaLiberarPedidoMov.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaLiberarPedidoMov" Title="Movimentações de Liberações de Pedidos" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(semValor)
        {
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            
            openWindow(600, 800, "RelBase.aspx?rel=LiberarPedidoMov" + (semValor ? "SemValor" : "") + 
                "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&dataIni=" + dataIni + 
                "&dataFim=" + dataFim + "&situacao=" + situacao + "&idFunc=" + idFunc);
        }
        
        function getCli(idCli)
        {
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
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label1" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1" Selected="True">Liberado</asp:ListItem>
                                <asp:ListItem Value="2">Cancelado</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True" OnDataBound="drpFuncionario_DataBound">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
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
                <asp:GridView GridLines="None" ID="grdLiberacoes" runat="server" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    AutoGenerateColumns="False" DataSourceID="odsLiberarPedidoMov" AllowPaging="True"
                    AllowSorting="True" DataKeyNames="IdLiberarPedido" PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="IdLiberarPedido" HeaderText="Liberação" SortExpression="IdLiberarPedido" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="Situacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total" />
                        <asp:BoundField DataField="Desconto" HeaderText="Desconto" SortExpression="Desconto"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="Dinheiro" DataFormatString="{0:C}" HeaderText="Dinheiro"
                            SortExpression="Dinheiro" />
                        <asp:BoundField DataField="Cheque" DataFormatString="{0:C}" HeaderText="Cheque" SortExpression="Cheque" />
                        <asp:BoundField DataField="Prazo" DataFormatString="{0:C}" HeaderText="Prazo" SortExpression="Prazo" />
                        <asp:BoundField DataField="Boleto" DataFormatString="{0:C}" HeaderText="Boleto" SortExpression="Boleto" />
                        <asp:BoundField DataField="Deposito" DataFormatString="{0:C}" HeaderText="Depósito"
                            SortExpression="Deposito" />
                        <asp:BoundField DataField="Cartao" DataFormatString="{0:c}" HeaderText="Cartão" SortExpression="Cartao" />
                        <asp:BoundField DataField="Outros" DataFormatString="{0:C}" HeaderText="Outros" SortExpression="Outros" />
                        <asp:BoundField DataField="Debito" DataFormatString="{0:c}" HeaderText="Débito" SortExpression="Debito" />
                        <asp:BoundField DataField="Credito" DataFormatString="{0:C}" HeaderText="Crédito"
                            SortExpression="Credito" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLiberarPedidoMov" runat="server" SelectMethod="GetList"
                    TypeName="Glass.Data.RelDAL.LiberarPedidoMovDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetListCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetFinanceiros"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkImprimirSemValor" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir (sem valores)</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
