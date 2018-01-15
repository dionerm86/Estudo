<%@ Page Title="Pedidos em Produção" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstPedidosProducao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPedidosProducao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

function openRpt(idPedido)
{
    openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + idPedido);
    return false;
}

function getCli(idCli)
{
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

function openMotivoCanc(idPedido) {
    openWindow(150, 400, "../Utils/SetMotivoCanc.aspx?idPedido=" + idPedido);
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
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AllowPaging="True" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsPedido" 
                    DataKeyNames="IdPedido" EmptyDataText="Nenhum pedido encontrado.">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="openRpt('<%# Eval("IdPedido") %>');">
                                    <img border="0" src="../Images/Relatorio.gif" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeInicialCli" HeaderText="Cliente" SortExpression="NomeInicialCli" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="DescrSitProducao" HeaderText="Situação" SortExpression="DescrSitProducao" />
                        <asp:BoundField DataField="DataProducao" HeaderText="Produção" SortExpression="DataProducao" />
                        <asp:BoundField DataField="DataProntoCorte" HeaderText="Pronto" 
                            SortExpression="DataProntoCorte" />
                        <asp:BoundField DataField="DataEntregue" HeaderText="Entregue" SortExpression="DataEntregue" />
                        <asp:BoundField DataField="FuncProd" HeaderText="Func. Produção" SortExpression="FuncProd" />
                        <asp:BoundField DataField="FuncEntregue" HeaderText="Func. Entregue" SortExpression="FuncEntregue" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountListCorte" SelectMethod="GetListCorte" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoDAO" OnDeleted="odsPedido_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
