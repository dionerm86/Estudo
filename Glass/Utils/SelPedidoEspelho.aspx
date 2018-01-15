<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelPedidoEspelho.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelPedidoEspelho" Title="Selecione o Pedido" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setPedidoEspelho(idPedido)
        {
            window.opener.setPedidoEspelho(idPedido);
            closeWindow();
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
                <table class="style1">
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesq3" runat="server" OnClick="imgPesq_Click" ImageUrl="~/Images/Pesquisar.gif"
                                Style="width: 16px" />
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
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdPedido" DataSourceID="odsPedidoEspelho"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="setPedidoEspelho('<%# Eval("IdPedido") %>');">
                                    <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                            </ItemTemplate>
                            <ItemStyle Wrap="false" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="IdProjeto" HeaderText="Projeto" SortExpression="IdProjeto" />
                        <asp:BoundField DataField="NomeCli" HeaderText="Cliente" SortExpression="NomeCli" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Vendedor" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="DataEspelho" HeaderText="Data" SortExpression="DataEspelho"
                            DataFormatString="{0:d}"></asp:BoundField>
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidoEspelho" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountSel" SelectMethod="GetListSel" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoEspelhoDAO"
                    >
                    <SelectParameters>
                        <asp:Parameter Name="idPedido" Type="UInt32" DefaultValue="0" />
                        <asp:ControlParameter Name="idCli" ControlID="txtNumCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter Name="nomeCli" ControlID="txtNome" PropertyName="Text" Type="String" />
                        <asp:Parameter Name="idLoja" Type="UInt32" DefaultValue="0" />
                        <asp:Parameter Name="idFunc" Type="UInt32" DefaultValue="0" />
                        <asp:Parameter Name="idFuncionarioConferente" Type="UInt32" DefaultValue="0" />
                        <asp:Parameter Name="situacao" Type="Int32" DefaultValue="0" />
                        <asp:Parameter Name="situacaoPedOri" Type="String" DefaultValue="" />
                        <asp:Parameter Name="idsProcesso" Type="String" />
                        <asp:Parameter Name="dataIniEnt" Type="String" DefaultValue="" />
                        <asp:Parameter Name="dataFimEnt" Type="String" DefaultValue="" />
                        <asp:Parameter Name="dataIniFab" Type="String" />
                        <asp:Parameter Name="dataFimFab" Type="String" />
                        <asp:Parameter Name="dataIniFin" Type="String" DefaultValue="" />
                        <asp:Parameter Name="dataFimFin" Type="String" DefaultValue="" />
                        <asp:ControlParameter Name="soFinalizados" ControlID="hdfFinalizados" PropertyName="Value"
                            Type="Boolean" />
                        <asp:Parameter Name="pedidosSemAnexo" Type="Boolean" DefaultValue="false" />
                        <asp:Parameter Name="pedidosAComprar" Type="Boolean" DefaultValue="false" />
                        <asp:Parameter Name="situacaoCnc" Type="String" />
                        <asp:Parameter Name="dataIniSituacaoCnc" Type="String" />
                        <asp:Parameter Name="dataFimSituacaoCnc" Type="String" />
                        <asp:Parameter Name="idsRotas" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfFinalizados" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
