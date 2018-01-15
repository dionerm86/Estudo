<%@ Page Title="Peças não levadas pelo Cliente" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstPecasSemSaida.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPecasSemSaida" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function escondeColuna(numColuna)
        {
            var tabela = FindControl("grdProdutosPedido", "table");
            if (tabela == null)
                return;

            for (i = 0; i < tabela.rows.length; i++)
            {
                if (tabela.rows[i].cells.length == 1 || numColuna > tabela.rows[i].cells.length)
                    continue;

                tabela.rows[i].cells[numColuna].style.display = "none";
            }
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
        
        function openRpt(exportarExcel)
        {
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;

            idCliente = idCliente != "" ? idCliente : "0";
            idPedido = idPedido != "" ? idPedido : "0";

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=PecasSemSaida&idCliente=" + idCliente +
                "&nomeCliente=" + nomeCliente + "&idPedido=" + idPedido + "&dataIni=" + dataIni + "&dataFim=" + dataFim +"&idLoja=" + idLoja + "&exportarExcel=" + exportarExcel);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr runat="server" id="filtros">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false"/>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr runat="server" id="separador">
            <td>
                &nbsp;
            </td>
        </tr>
        <tr runat="server" id="lista">
            <td align="center">
                <asp:GridView ID="grdProdutosPedido" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedido"
                    GridLines="None" EmptyDataText="Não há peças que esse cliente ainda não levou.">
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCliente">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("IdCliente") + " - " + Eval("NomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:BoundField DataField="AlturaLista" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Tot. M²" SortExpression="TotM" />
                        <asp:BoundField DataField="ValorVendido" HeaderText="Valor Vendido" SortExpression="ValorVendido"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:c}" />
                        <asp:BoundField DataField="ValorBenef" HeaderText="Valor Benef." SortExpression="ValorBenef"
                            DataFormatString="{0:c}" />
                        <asp:TemplateField HeaderText="Qtde não levada">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# float.Parse(Eval("Qtde").ToString()) - float.Parse(Eval("QtdSaida").ToString()) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>               
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
               &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosPedido" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountPecasSemSaida" SelectMethod="GetListPecasSemSaida"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        FindControl("txtNumCli", "input").focus();
        
        var esconderColunas = <%= EsconderColunas().ToString().ToLower() %>;
        if (esconderColunas)
        {
            escondeColuna(2); // Ambiente
            escondeColuna(6); // Altura
            escondeColuna(7); // Largura
            escondeColuna(8); // Tot. M²
            escondeColuna(11); // Valor Benef.
        }
    </script>

</asp:Content>
