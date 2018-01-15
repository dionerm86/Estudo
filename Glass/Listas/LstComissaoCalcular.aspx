<%@ Page Title="Relatório de Comissão" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstComissaoCalcular.aspx.cs" Inherits="Glass.UI.Web.Listas.LstComissaoCalcular" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt() {
            var tipo = FindControl("drpTipo", "select").value;
            var idFunc = FindControl("drpNome", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var ordenar = FindControl("drpOrdenar", "select").value;
            var tipoPedido = FindControl("cbdTipoPedido", "select").itens();

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ComissaoCalcular&tipo=" + tipo + "&idFunc=" + idFunc + "&dataIni=" +
                dataIni + "&dataFim=" + dataFim + "&isRelatorio=" + true + "&ordenar=" + ordenar + "&tipoPedido=" + tipoPedido);
        }

        function showLoadGif() {
            var loading = document.getElementById("loading");
            loading.style.display = "inline";
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True" OnChange="showLoadGif()"
                                OnSelectedIndexChanged="drpTipo_SelectedIndexChanged">
                                <asp:ListItem Value="0">Funcionário</asp:ListItem>
                                <asp:ListItem Value="1">Comissionado</asp:ListItem>
                                <asp:ListItem Value="2">Instalador</asp:ListItem>
                                <asp:ListItem Value="3">Gerente</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap" align="right">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="showLoadGif()"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNome" runat="server" DataSourceID="odsFuncionario" OnChange="showLoadGif()"
                                DataTextField="Nome" DataValueField="IdFunc" AutoPostBack="True" AppendDataBoundItems="True"
                                OnDataBound="drpNome_DataBound" OnSelectedIndexChanged="drpNome_SelectedIndexChanged"
                                OnDataBinding="drpNome_DataBinding">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdfNome" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Ordenar relatório por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server">
                                <asp:ListItem Value="1">Nome</asp:ListItem>
                                <asp:ListItem Value="2">Valor da comissão</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Tipos de Pedidos" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdTipoPedido" runat="server" Title="Selecione o tipo de pedido"
                                Width="230px">
                                <asp:ListItem Selected="True" Value="0">Pedidos com comissão não gerada</asp:ListItem>
                                <asp:ListItem Value="1">Pedidos com comissão gerada</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td nowrap="nowrap" align="right">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClientClick="showLoadGif()" ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <img src="../Images/load.gif" id="loading" style="display: none; padding-bottom: 3px" />
                <asp:GridView GridLines="None" ID="grdComissao" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdPedido" DataSourceID="odsComissao" EmptyDataText="Não há comissões para o filtro especificado."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" PageSize="20">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="NomeComissionado" HeaderText="Comissionado" SortExpression="NomeComissionado" />
                        <asp:BoundField DataField="NomeInst" HeaderText="Instalador" SortExpression="NomeInst" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="DataConfString" HeaderText="Data Conf. Pedido" SortExpression="DataConf" />
                        <asp:BoundField DataField="DataFinalizacaoInst" DataFormatString="{0:d}" HeaderText="Data Final. Inst."
                            SortExpression="DataFinalizacaoInst" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Valor Pedido"
                            SortExpression="Total" />
                        <asp:BoundField DataField="TextoDescontoPerc" HeaderText="Desconto" SortExpression="TextoDescontoPerc" />
                        <asp:BoundField DataField="ValorComissaoPagar" DataFormatString="{0:C}" HeaderText="Valor Comissão"
                            SortExpression="ValorComissaoPagar" />
                    </Columns>
                    <PagerStyle />
                    <HeaderStyle Wrap="False" />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissao" runat="server" MaximumRowsParameterName=""
                    SelectMethod="GetPedidosForComissao" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipo" Name="tipoFunc" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpNome" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="cbdTipoPedido" Name="tiposPedidos" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:Parameter Name="isRelatorio" DefaultValue="true" Type="Boolean" />
                        <asp:Parameter Name="tiposVenda" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedoresForComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissionado" runat="server" SelectMethod="GetComissionadosForComissao"
                    TypeName="Glass.Data.DAL.ComissionadoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInstalador" runat="server" SelectMethod="GetColocadoresForComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGerente" runat="server" SelectMethod="GetGerentesForComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">                           
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
                <asp:LinkButton ID="lkbImprimir" runat="server" OnClientClick="openRpt(); return false">
                    <img src="../images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
