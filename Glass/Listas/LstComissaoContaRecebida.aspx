<%@ Page Title="Comissões de Contas Recebidas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstComissaoContaRecebida.aspx.cs" Inherits="Glass.UI.Web.Listas.LstComissaoContaRecebida" %>


<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function openRptIndividual(idComissao, idFunc) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ComissaoContasRecebidas&idComissao=" + idComissao + "&idFunc=" + idFunc);
            return false;
        }

        function openRptGeral() {
            var idComissao = FindControl("txtIdComissao", "input").value;
            var idFunc = FindControl("drpNome", "select").value;
            var idLoja = FindControl("drpNome", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Comissao&contasRecebidas=true&idFunc=" + idFunc + "&idLoja=" + idLoja +
                "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idComissao=" + idComissao);

            return false;
        }

        function openRptRecibos() {
            var idComissao = FindControl("txtIdComissao", "input").value;
            var tipoFunc = FindControl("drpTipo", "select").value;
            var idFunc = FindControl("drpNome", "select").value;
            var idPedido = FindControl("txtPedido", "input").value;
            idPedido = idPedido != "" ? idPedido : "0";
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=RecibosComissao&tipoFunc=" + tipoFunc + "&idFunc=" + idFunc +
                "&idPedido=" + idPedido + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&idComissao=" + idComissao);

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Cód" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtIdComissao" runat="server" Width="60px"></asp:TextBox></td>

                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" /></td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="drpNome" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="true">
                                <asp:ListItem Selected="True" Value="0" Text="Todos"></asp:ListItem>
                            </asp:DropDownList>

                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" /></td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" OnChange="dataChange();" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" /></td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" MostrarTodas="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdComissao" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdComissao" DataSourceID="odsComissao"
                    EmptyDataText="Nenhuma comissão encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="openRptIndividual('<%# Eval("IdComissao") %>','<%# Eval("IdFunc") %>');">
                                    <img border="0" src="../Images/Relatorio.gif" title="Pedidos desta Comissão" /></a>
                                <asp:LinkButton ID="lnkExcluir" runat="server" OnClientClick="return confirm('Tem certeza que deseja excluir esta comissão?')"
                                    CommandName="Delete">
                                     <img border="0" src="../Images/ExcluirGrid.gif" title="Excluir comissão" /></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdComissao" HeaderText="Cód" ReadOnly="True" SortExpression="IdComissao" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" ReadOnly="True" SortExpression="Nome" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Comissão"
                            SortExpression="Total" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRptGeral();">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton></td>
        </tr>
    </table>


    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
        SelectMethod="GetVendedorForComissaoContasReceber" TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>

    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsComissao" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetListContasRecebidasCount" SelectMethod="GetListContasRecebidas" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ComissaoDAO" DataObjectTypeName="Glass.Data.Model.Comissao"
        DeleteMethod="RemoveComissaoContasRecebidas" OnDeleted="odsComissao_Deleted">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtIdComissao" Name="idComissao" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="drpNome" Name="idFunc" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

</asp:Content>
