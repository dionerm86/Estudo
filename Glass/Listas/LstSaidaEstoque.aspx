<%@ Page Title="Saídas de Estoque" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstSaidaEstoque.aspx.cs" Inherits="Glass.UI.Web.Listas.LstSaidaEstoque" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt(idSaidaEstoque)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=SaidaEstoque" +
                "&idSaidaEstoque=" + idSaidaEstoque);
        }

        function openRptLista()
        {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLiberarPedido = FindControl("txtNumLiberacao", "input");
            var idVolume = FindControl("txtNumVolume", "input");
            var idFunc = FindControl("drpFuncionario", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaSaidaEstoque" +
                "&idPedido=" + idPedido + "&idFunc=" + idFunc + "&dataIni=" + dataIni +
                "&dataFim=" + dataFim + "&idLiberarPedido=" + (idLiberarPedido != null ? idLiberarPedido.value : "") + 
                "&idVolume=" + idVolume);
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
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="80px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq');"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumLiberacao" runat="server" Width="80px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq');"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqLiberacao" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblVolume" runat="server" Text="Volume" ForeColor="#0066FF"></asp:Label>
                        </td>
                         <td>
                            <asp:TextBox ID="txtNumVolume" runat="server" Width="80px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq');"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbPesqVolume" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsFunc" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false"/>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
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
                <asp:GridView ID="grdSaidas" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataSourceID="odsSaidas" GridLines="None" DataKeyNames="IdSaidaEstoque" EmptyDataText="Não há saída de estoque para esse filtro."
                    AllowPaging="True" AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("IdSaidaEstoque") + "); return false" %>' />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" 
                                    Visible='<%# Eval("PodeCancelar") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="concat(coalesce(IdPedido,0), coalesce(IdLiberarPedido,0))" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRptLista(); return false"> <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSaidas" runat="server" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.SaidaEstoqueDAO" DeleteMethod="Cancelar">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumLiberacao" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                            <asp:ControlParameter ControlID="txtNumVolume" Name="idVolume" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFunc" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
