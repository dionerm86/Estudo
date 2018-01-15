<%@ Page Title="Entradas de Estoque" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstEntradaEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEntradaEstoque" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">
        function openRpt(idEntradaEstoque)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=EntradaEstoque" +
                "&idEntradaEstoque=" + idEntradaEstoque);
        }

        function openRptLista()
        {
            var idCompra = FindControl("txtNumCompra", "input").value;
            var numeroNFe = FindControl("txtNumNFe", "input").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var dataIni = FindControl("txtDataIni", "input").value;
            var dataFim = FindControl("txtDataFim", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaEntradaEstoque" +
                "&idCompra=" + idCompra + "&numNFe=" + numeroNFe + "&idFunc=" + idFunc + "&dataIni=" + dataIni + 
                "&dataFim=" + dataFim);
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCompra" runat="server" Width="80px"
                                onkeypress="return soNumeros(event, true, true)" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq');"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                onclick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="NFe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumNFe" runat="server" Width="80px"
                                onkeypress="return soNumeros(event, true, true)" 
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq');"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                onclick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" 
                                AppendDataBoundItems="True" DataSourceID="odsFunc" DataTextField="Nome" 
                                DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                onclick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataIni', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" Width="70px"></asp:TextBox>
                            <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                OnClientClick="return SelecionaData('txtDataFim', this)" ToolTip="Alterar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                onclick="imgPesq_Click" />
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
                <asp:GridView ID="grdEntradas" runat="server" AutoGenerateColumns="False" 
                    CssClass="gridStyle" DataSourceID="odsEntradas" GridLines="None" 
                    DataKeyNames="IdEntradaEstoque" 
                    EmptyDataText="Não há entrada de estoque para esse filtro." AllowPaging="True" 
                    AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" 
                                    ImageUrl="~/Images/Relatorio.gif" 
                                    onclientclick='<%# "openRpt(" + Eval("IdEntradaEstoque") + "); return false" %>' />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" 
                                    Visible='<%# Eval("PodeCancelar") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCompra" HeaderText="Compra" 
                            SortExpression="IdCompra" />
                        <asp:BoundField DataField="NumeroNFe" HeaderText="NFe" 
                            SortExpression="NumeroNFe" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" 
                            SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" 
                            SortExpression="DataCad" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRptLista(); return false"> <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEntradas" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" DeleteMethod="Cancelar"
                    SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.EntradaEstoqueDAO" 
                    ondeleted="odsEntradas_Deleted">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumNFe" Name="numeroNfe" 
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" 
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="Text" 
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" 
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFunc" runat="server" SelectMethod="GetOrdered" 
                    TypeName="Glass.Data.DAL.FuncionarioDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

