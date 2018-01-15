<%@ Page Title="Impressões Pendentes" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstImpressoesPendentes.aspx.cs" Inherits="Glass.UI.Web.Listas.LstImpressoesPendentes" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(exportarExcel) {
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var buscarPecasDeBox = FindControl("chkPecaBox", "input").checked;
            var buscarPecasRepostas = FindControl("chkPecaReposta", "input").checked;
            var agruparCliente = FindControl("chkAgruparCliente", "input").checked;
            var rota = FindControl("ddlRota", "select").value;
            var codProcesso = FindControl("drpProcesso", "select").value;
            var codAplicacao = FindControl("drpAplicacao", "select").value;
            var idCorVidro = !!idCorVidro ? idCorVidro : FindControl("drpCorVidro", "select").value;
            var espessura = !!espessura ? espessura : FindControl("txtEspessura", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;

            if (idPedido == "")
                idPedido = 0;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ImpressoesPendentes&dataIni=" + dtIni + "&dataFim=" + dtFim +
            "&IdPedido=" + idPedido + "&buscarPecasDeBox=" + buscarPecasDeBox + "&buscaPecaReposta=" + buscarPecasRepostas +
            "&agruparCliente=" + agruparCliente + "&rota=" + rota + "&codProcesso=" + codProcesso + "&codAplicacao=" + codAplicacao +
             "&exportarExcel=" + exportarExcel + "&idCorVidro=" + idCorVidro + "&espessura=" + espessura + "&idLoja=" + idLoja);

            return false;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Período Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td align="right">
                            <asp:Label ID="Label1" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:DropDownList ID="ddlRota" runat="server" AppendDataBoundItems="True" DataSourceID="odsRota"
                                DataTextField="Descricao" DataValueField="IdRota" Width="200px">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" Height="16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style="color: #0066FF">
                            Cor
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCorVidro" runat="server" AppendDataBoundItems="True" DataSourceID="odsCorVidro"
                                DataTextField="Descricao" DataValueField="IdCorVidro">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="color: #0066FF">
                            Espessura
                        </td>
                        <td>
                            <asp:TextBox ID="txtEspessura" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td style="color: #0066FF">
                            Processo
                        </td>
                        <td>
                            <asp:DropDownList ID="drpProcesso" runat="server" AppendDataBoundItems="True" DataSourceID="odsProcesso"
                                DataTextField="CodInterno" DataValueField="CodInterno">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td style="color: #0066FF">
                            Aplicação
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAplicacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsAplicacao"
                                DataTextField="CodInterno" DataValueField="CodInterno">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                         <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                         <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true"   />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLoja" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkPecaBox" runat="server" Text="Buscar peças de estoque" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkPecaReposta" runat="server" Text="Buscar peças repostas" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Height="16px" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparCliente" runat="server" Text="Agrupar por cliente" />
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
                <asp:GridView GridLines="None" ID="grdProduto" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProd" DataSourceID="odsProduto"
                    EmptyDataText="Nenhum produto encontrado." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente">
                        </asp:BoundField>
                        <asp:BoundField DataField="RotaCliente" HeaderText="Rota" SortExpression="RotaCliente" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="TotM" HeaderText="m²" SortExpression="TotM" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                        </asp:BoundField>
                        <asp:BoundField DataField="CodProcesso" HeaderText="Proc." SortExpression="CodProcesso" />
                        <asp:BoundField DataField="CodAplicacao" HeaderText="Apl." SortExpression="CodAplicacao" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtd." SortExpression="Qtde" />
                        <asp:BoundField DataField="QtdImpresso" HeaderText="Qtd. Já Impresso" SortExpression="QtdImpresso" />
                        <asp:BoundField DataField="DataPedido" DataFormatString="{0:d}" HeaderText="Data Pedido"
                            SortExpression="DataPedido" />
                        <asp:BoundField DataField="DataEntrega" HeaderText="Data Entrega" SortExpression="DataEntrega"
                            DataFormatString="{0:d}"></asp:BoundField>
                        <asp:BoundField DataField="DataFabrica" DataFormatString="{0:d}" 
                            HeaderText="Data Fábrica" SortExpression="DataFabrica" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProduto" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountImprPend" SelectMethod="GetListImprPend"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkPecaBox" Name="buscarPecasDeBox" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkPecaReposta" Name="buscarPecasRepostas" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="ddlRota" Name="idRota" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpProcesso" Name="codProcesso" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpAplicacao" Name="codAplicacao" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpCorVidro" name="idCorVidro" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtEspessura" Name="espessura" PropertyName="Text" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProcesso" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.EtiquetaProcessoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAplicacao" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
