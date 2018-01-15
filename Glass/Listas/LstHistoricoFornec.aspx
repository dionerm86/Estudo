<%@ Page Title="Histórico de Fornecedor" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstHistoricoFornec.aspx.cs" Inherits="Glass.UI.Web.Listas.LstHistoricoFornec" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

function getFornec(txtIdFornec)
{
    var retorno = MetodosAjax.GetFornecConsulta(txtIdFornec.value).value.split(';');
    
    if (retorno[0] == "Erro")
    {
        alert(retorno[1]);
        txtIdFornec.value = "";
        FindControl("txtNomeFornec", "input").value = "";
        return false;
    }

    FindControl("txtNomeFornec", "input").value = retorno[1];
}

function openRpt() {
    var idFornec = FindControl("txtNumFornec", "input").value;
    var dtIniVenc = FindControl("ctrlDataIniVenc_txtData", "input").value;
    var dtFimVenc = FindControl("ctrlDataFimVenc_txtData", "input").value;
    var dtIniPag = FindControl("ctrlDataIniPag_txtData", "input").value;
    var dtFimPag = FindControl("ctrlDataFimPag_txtData", "input").value;
    var vIniVenc = FindControl("txtPrecoInicialVenc", "input").value;
    var vFimVenc = FindControl("txtPrecoFinalVenc", "input").value;
    var vIniPag = FindControl("txtPrecoInicialPag", "input").value;
    var vFimPag = FindControl("txtPrecoFinalPag", "input").value;
    var emAberto = FindControl("chkEmAberto", "input").checked;
    var pagEmDia = FindControl("chkPagasEmDia", "input").checked;
    var pagComAtraso = FindControl("chkPagasComAtraso", "input").checked;
    var sort = FindControl("drpSort", "select").value;

    var queryString = (idFornec == "" ? "&idFornec=0" : "&idFornec=" + idFornec) + "&dtIniVenc=" + dtIniVenc +
        "&dtFimVenc=" + dtFimVenc + "&dtIniPag=" + dtIniPag + "&dtFimPag=" + dtFimPag + "&vIniVenc=" + (vIniVenc == "" ? 0 : vIniVenc) +
        "&vFimVenc=" + (vFimVenc == "" ? 0 : vFimVenc) + "&vIniPag=" + (vIniPag == "" ? 0 : vIniPag) + "&sort=" + sort +
        "&vFimPag=" + (vFimPag == "" ? 0 : vFimPag) + "&emAberto=" + emAberto + "&pagEmDia=" + pagEmDia + "&pagComAtraso=" + pagComAtraso;

    openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=HistoricoFornec" + queryString);

    return false;
}

function openRptUnico(idCompra) {
    openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Compra&idCompra=" + idCompra);
    return false;
}

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="if (FindControl('txtNumFornec', 'input').value == '') openWindow(570, 760, '../Utils/SelFornec.aspx'); else return true; "
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
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
                <table id="tbHist" width="100%" runat="server">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="right" nowrap="nowrap">
                                        <asp:Label ID="Label19" runat="server" Text="Venc." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <uc1:ctrlData ID="ctrlDataIniVenc" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <uc1:ctrlData ID="ctrlDataFimVenc" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td align="right" nowrap="nowrap">
                                        <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label10" runat="server" Text="Paga entre" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td nowrap="nowrap">
                                        <uc1:ctrlData ID="ctrlDataIniPag" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataFimPag" runat="server" ReadOnly="ReadWrite" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" OnClick="imgPesq_Click" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label18" runat="server" Text="Valor Venc." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrecoInicialVenc" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        até
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrecoFinalVenc" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            Width="16px" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label15" runat="server" Text="Valor pago" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrecoInicialPag" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        até
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPrecoFinalPag" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" Width="16px" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label20" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpSort" runat="server" AutoPostBack="True">
                                            <asp:ListItem Value="1">Vencimento</asp:ListItem>
                                            <asp:ListItem Value="2">Pagamento</asp:ListItem>
                                            <asp:ListItem Value="3">Situação</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="chkEmAberto" runat="server" Checked="True" Text="Em aberto" ForeColor="Red" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkPagasEmDia" runat="server" Checked="True" Text="Pagas em dia"
                                            ForeColor="Green" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkPagasComAtraso" runat="server" Checked="True" Text="Pagas com atraso"
                                            ForeColor="Blue" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                            OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" Width="16px" />
                                    </td>
                                </tr>
                            </table>
                            <asp:Button ID="btnLimparFiltros" runat="server" OnClick="btnLimparFiltros_Click"
                                Text="Limpar Filtros" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdConta" runat="server" AutoGenerateColumns="False"
                                DataKeyNames="IdContaPg" DataSourceID="odsContasPagar" EmptyDataText="Nenhuma conta a receber/recebida encontrada."
                                AllowPaging="True" PageSize="15" AllowSorting="True" OnDataBound="grdConta_DataBound"
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit">
                                <PagerSettings PageButtonCount="20" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Panel ID="Panel1" runat="server" Visible='<%# Eval("IdCompra") != null %>'>
                                                <a href="#" onclick="openRptUnico('<%# Eval("IdCompra") %>');">
                                                    <img border="0" src="../Images/Relatorio.gif" /></a>
                                            </asp:Panel>
                                            <asp:HiddenField ID="hdfColor" runat="server" Value='<%# Eval("Color") %>' />
                                            <asp:HiddenField ID="hdfTotalEmAberto" runat="server" Value='<%# Eval("TotalEmAberto", "{0:C}") %>' />
                                            <asp:HiddenField ID="hdfTotalRecEmDia" runat="server" Value='<%# Bind("TotalRecEmDia", "{0:C}") %>' />
                                            <asp:HiddenField ID="hdfTotalRecComAtraso" runat="server" Value='<%# Eval("TotalRecComAtraso", "{0:C}") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdCompra" HeaderText="Compra" SortExpression="IdCompra">
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                                    <asp:BoundField DataField="DescrPlanoConta" HeaderText="Referente a" SortExpression="DescrPlanoConta" />
                                    <asp:BoundField DataField="ValorVenc" HeaderText="Valor" SortExpression="ValorVenc"
                                        DataFormatString="{0:C}">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DataVenc" DataFormatString="{0:d}" HeaderText="Data. Venc."
                                        SortExpression="DataVenc" />
                                    <asp:BoundField DataField="ValorPago" DataFormatString="{0:C}" HeaderText="Valor Pago"
                                        SortExpression="ValorPago">
                                        <ItemStyle Wrap="False" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="DataPagto" HeaderText="Data Pagto." SortExpression="DataPagto"
                                        DataFormatString="{0:d}">
                                        <HeaderStyle Wrap="False" />
                                    </asp:BoundField>
                                </Columns>
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
                            <table id="tbTotais" runat="server">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label21" runat="server" ForeColor="Red" Text="Total em aberto"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalAberto" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label22" runat="server" ForeColor="Green" Text="Total pagas em dia"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalRecEmDia" runat="server"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label23" runat="server" ForeColor="Blue" Text="Total pagas com atraso"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTotalRecComAtraso" runat="server"></asp:Label>
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
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContasPagar" runat="server" MaximumRowsParameterName="pageSize"
                                SelectMethod="GetListHist" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ContasPagarDAO"
                                EnablePaging="True" SelectCountMethod="GetCountHist" SortParameterName="sortExpression">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text"
                                        Type="UInt32" />
                                    <asp:ControlParameter ControlID="ctrlDataIniVenc" Name="dataIniVenc" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataFimVenc" Name="dataFimVenc" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataIniPag" Name="dataIniPag" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="ctrlDataFimPag" Name="dataFimPag" PropertyName="DataString"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="txtPrecoInicialVenc" Name="vIniVenc" PropertyName="Text"
                                        Type="Single" />
                                    <asp:ControlParameter ControlID="txtPrecoFinalVenc" Name="vFinVenc" PropertyName="Text"
                                        Type="Single" />
                                    <asp:ControlParameter ControlID="txtPrecoInicialPag" Name="vIniPag" PropertyName="Text"
                                        Type="Single" />
                                    <asp:ControlParameter ControlID="txtPrecoFinalPag" Name="vFinPag" PropertyName="Text"
                                        Type="Single" />
                                    <asp:ControlParameter ControlID="chkEmAberto" Name="emAberto" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="chkPagasEmDia" Name="pagEmDia" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="chkPagasComAtraso" Name="pagComAtraso" PropertyName="Checked"
                                        Type="Boolean" />
                                    <asp:ControlParameter ControlID="drpSort" Name="sort" PropertyName="SelectedValue"
                                        Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
