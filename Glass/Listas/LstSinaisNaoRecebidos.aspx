<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstSinaisNaoRecebidos.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstSinaisNaoRecebidos" Title="Sinais a Receber" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
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

function openRpt()
{
    var idCli = FindControl("txtNumCli", "input").value;
    var idPedido = FindControl("txtNumPedido", "input").value;

    idCli = idCli == "" ? 0 : idCli;
    idPedido = idPedido == "" ? 0 : idPedido;

    openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=SinaisReceber&IdCli=" + idCli + "&IdPedido=" + idPedido);
    return false;
}

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdSinaisReceber" runat="server" AllowPaging="True" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    AllowSorting="True" DataSourceID="odsSinaisReceber"
                    AutoGenerateColumns="False" PageSize="15">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num. Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" ReadOnly="True" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:C}" HeaderText="Valor Sinal"
                            SortExpression="ValorEntrada" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data Pedido"
                            SortExpression="DataCad" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total Ped."
                            SortExpression="Total" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSinaisReceber" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetSinaisNaoRecebidosCount" SelectMethod="GetSinaisNaoRecebidos"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:Parameter DefaultValue="false" Name="pagtoAntecipado" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
