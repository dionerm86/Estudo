<%@ Page Title="Pedidos Internos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ListaProdutoInterno.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaProdutoInterno" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        
        function openRptPedido(idPedido)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=PedidoInterno&idPedido=" + idPedido);
        }

        function openRpt(exportarExcel) {
            var idPedido = FindControl("txtIdPedido", "input").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idFunc = FindControl("drpFunc", "select").value;
            var idFuncAut = FindControl("drpFuncAut", "select").value;
            var idGrupo = FindControl("drpGrupo", "select").value;
            var idSubGrupo = FindControl("drpSubgrupo", "select").value;

            if (idPedido == "") idPedido = 0;
            if(idFunc == "") idFunc = 0;
            if(idFuncAut == "") idFuncAut = 0;
            if(idGrupo == "") idGrupo = 0;
            if(idSubGrupo == "") idSubGrupo = 0;
            
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaProdutoPedidoInterno&idPedido=" + idPedido + "&dataIni=" + dtIni +
                "&dataFim=" + dtFim + "&idFunc=" + idFunc + "&idFuncReceb=" + idFuncAut +
                "&idGrupo=" + idGrupo + "&idSubGrupo=" + idSubGrupo + "&exportarExcel=" + exportarExcel);

            return false;
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFunc" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Funcionário Autorizado" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncAut" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Grupo" ForeColor="#0066FF" />
                        </td>
                        <td>
                            <!-- <sync:CheckBoxListDropDown ID="cbdGrupo" runat="server" CheckAll="False" 
                                DataSourceID="odsGrupo" DataTextField="Descricao" DataValueField="IdGrupoProd" 
                                ImageURL="~/Images/DropDown.png" 
                                JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" 
                                OpenOnStart="False" Title="Selecione o grupo">
                            </sync:CheckBoxListDropDown> -->
                            <asp:DropDownList ID="drpGrupo" runat="server" DataSourceID="odsGrupo" DataTextField="Descricao"
                                DataValueField="IdGrupoProd" AutoPostBack="true" AppendDataBoundItems="true">
                                <asp:ListItem Value="0" Text="Todos" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Subgrupo" ForeColor="#0066FF" />
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" AppendDataBoundItems="true">
                                <asp:ListItem Value="0" Text="Todos" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupos" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirTodos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPedidos" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdPedidoInterno" DataSourceID="odsPedidoInterno" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Não há pedidos internos cadastrados." AllowPaging="True" AllowSorting="True"
                    OnRowCommand="grdPedidos_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="codInterno" HeaderText="Cod. Produto" SortExpression="codInterno" />
                        <asp:BoundField DataField="descrProduto" HeaderText="Produto" SortExpression="descrProduto" />
                        <asp:BoundField DataField="QtdeSomada" HeaderText="Qtde" SortExpression="Qtde"/>
                        <asp:BoundField DataField="TotM2" HeaderText="Total M²" SortExpression="TotM" />
                        <asp:BoundField DataField="Custo" HeaderText="Custo" SortExpression="Custo" DataFormatString="{0:C}" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidoInterno" runat="server" SelectCountMethod="GetForRptProdutoInternoCount"
                    SelectMethod="GetForRptProdutoInterno" TypeName="Glass.Data.DAL.ProdutoPedidoInternoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedidoInterno" PropertyName="Text"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpFunc" Name="idFuncCad" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpFuncAut" Name="idFuncReceb" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataInicio" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpSubGrupo" Name="idSubGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
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
    </table>
</asp:Content>
