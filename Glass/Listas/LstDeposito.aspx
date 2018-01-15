<%@ Page Title="Depósitos de Cheques" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstDeposito.aspx.cs" Inherits="Glass.UI.Web.Listas.LstDeposito" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRptInd(idDeposito, exportarExcel) {
            var ordemCheque = FindControl("drpOrdemCheque", "select").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=Deposito&idDeposito=" + idDeposito + "&ordemCheque=" + ordemCheque + 
                "&exportarExcel=" + exportarExcel);
            return false;
        }

        function openRpt(exportarExcel) {
            var idDeposito = FindControl("txtNumDeposito", "input").value;
            var idContaBanco = FindControl("drpContaBanco", "select").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ListaDeposito&idDeposito=" + idDeposito + "&idContaBanco=" + idContaBanco +
                "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&exportarExcel=" + exportarExcel);

            return false;
        }

        function cancelarDeposito(idDeposito) {

            openWindow(370, 600, "../Utils/SetMotivoCancDepositoCheque.aspx?idDeposito=" + idDeposito);

            return false;
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Num. Depósito"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumDeposito" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Conta Bancária"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" DataSourceID="odsContaBanco"
                                DataTextField="Descricao" DataValueField="IdContaBanco" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Período Depósito"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label21" runat="server" ForeColor="#0066FF" Text="Ordenar cheques por"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdemCheque" runat="server">
                                <asp:ListItem Value="1">Data Venc.</asp:ListItem>
                                <asp:ListItem Value="2">Num. Cheque</asp:ListItem>
                                <asp:ListItem Value="3">Titular</asp:ListItem>
                            </asp:DropDownList>
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
                <asp:GridView GridLines="None" ID="grdDepositos" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataSourceID="odsDeposito"
                    PageSize="15" EmptyDataText="Nenhum Depósito encontrado." DataKeyNames="IdDeposito">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a id="lnkImprimir" href="#" onclick="return openRptInd(<%# Eval("IdDeposito") %>);">
                                    <img border="0" src="../Images/Relatorio.gif" title="Visualizar Depósito" /></a>
                                <a id="lnkImprimir1" href="#" onclick='return openRptInd(<%# Eval("IdDeposito") %>, true);'>
                                    <img border="0" src="../Images/Excel.gif" title="Visualizar Depósito (Excel)" /></a>
                                <asp:ImageButton ID="imgCancelar" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "return cancelarDeposito(" + Eval("IdDeposito") + ");"%>'
                                    Visible='<%# Eval("BotaoVisible") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdDeposito" HeaderText="Num. Depósito" SortExpression="IdDeposito" />
                        <asp:BoundField DataField="DescrContaBanco" HeaderText="Conta Bancária" SortExpression="DescrContaBanco" />
                        <asp:BoundField DataField="NomeFuncDeposito" HeaderText="Funcionário" SortExpression="NomeFuncDeposito" />
                        <asp:BoundField DataField="TaxaAntecip" HeaderText="Taxa Antecip." SortExpression="TaxaAntecip" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs" SortExpression="Obs" />
                        <asp:BoundField DataField="Valor" DataFormatString="{0:C}" HeaderText="Valor" SortExpression="Valor" />
                        <asp:BoundField DataField="DataDeposito" DataFormatString="{0:d}" HeaderText="Data Depósito"
                            SortExpression="DataDeposito" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                 <uc2:ctrlLogCancPopup ID="ctrlLogCancPopup2" runat="server" Tabela="DepositoCheque"
                                    IdRegistro='<%# Eval("IdDeposito") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDeposito" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.DepositoChequeDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" DeleteMethod="CancelarDeposito">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumDeposito" Name="idDeposito" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir0" runat="server" CausesValidation="False" OnClientClick="openRpt(false); return false;"> <img alt="" border="0" 
                    src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
