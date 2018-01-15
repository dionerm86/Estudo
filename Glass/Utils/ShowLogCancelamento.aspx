<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowLogCancelamento.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ShowLogCancelamento" Title="Log de Cancelamento: " MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center" class="subtitle1">
                Referência:
                <asp:Label ID="lblSubtitle" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblIdRegistroCanc" runat="server" ForeColor="#0066FF" Text="IdRegistroCanc"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdRegistroCanc" runat="server" onchange="FindControl('hdfIdRegistroCanc', 'input').value = this.value"
                                Width="50px" onkeydown="if (isEnter(event)) { FindControl('hdfIdRegistroCanc', 'input').value = this.value; cOnClick('imgPesq', 'input'); }"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Funcionário"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsFunc" DataTextField="Value" DataValueField="Key" AutoPostBack="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" ForeColor="#0066FF" Text="Período do cancelamento"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Campo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCampo" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsCampos" DataTextField="Value" DataValueField="Key">
                                <asp:ListItem Value="">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" ForeColor="#0066FF" Text="Valor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValor" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', 'input')"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLog" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsLog" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Não há itens para esse filtro."
                    PageSize="15" OnRowDataBound="grdLog_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="IdRegistroCanc" HeaderText="IdRegistroCanc" SortExpression="IdRegistroCanc" />
                        <asp:BoundField DataField="DataCanc" HeaderText="Data" SortExpression="DataCanc" />
                        <asp:BoundField DataField="NomeFuncCanc" HeaderText="Funcionário" SortExpression="NomeFuncCanc" />
                        <asp:TemplateField HeaderText="Manual?" SortExpression="CancelamentoManual">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# (bool)Eval("CancelamentoManual") ? "Sim" : "Não" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("CancelamentoManual") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Motivo" HeaderText="Motivo" SortExpression="Motivo" />
                        <asp:BoundField DataField="Campo" HeaderText="Campo" SortExpression="Campo" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="Valor" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLog" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.LogCancelamentoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="tabela" QueryStringField="tabela" Type="Int32" />
            <asp:ControlParameter ControlID="hdfIdRegistroCanc" Name="idRegistroCanc" PropertyName="Value"
                Type="UInt32" />
            <asp:ControlParameter ControlID="hdfExibirAdmin" Name="exibirAdmin" PropertyName="Value"
                Type="Boolean" />
            <asp:ControlParameter ControlID="drpCampo" Name="campo" PropertyName="SelectedValue"
                Type="String" />
            <asp:ControlParameter ControlID="drpFuncionario" Name="idFuncCanc" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                Type="String" />
            <asp:ControlParameter ControlID="txtValor" Name="valor" PropertyName="Text" Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCampos" runat="server" SelectMethod="GetCampos" TypeName="Glass.Data.DAL.LogCancelamentoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="tabela" QueryStringField="tabela" Type="Int32" />
            <asp:QueryStringParameter Name="idRegistroCanc" QueryStringField="id" Type="UInt32" />
            <asp:ControlParameter ControlID="hdfExibirAdmin" Name="exibirAdmin" PropertyName="Value"
                Type="Boolean" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFunc" runat="server" SelectMethod="GetFuncionarios"
        TypeName="Glass.Data.DAL.LogCancelamentoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="tabela" QueryStringField="tabela" Type="Int32" />
            <asp:QueryStringParameter DefaultValue="" Name="idRegistroCanc" QueryStringField="id"
                Type="UInt32" />
            <asp:ControlParameter ControlID="hdfExibirAdmin" Name="exibirAdmin" PropertyName="Value"
                Type="Boolean" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfIdRegistroCanc" runat="server" />
    <asp:HiddenField ID="hdfExibirAdmin" runat="server" Value="False" />
</asp:Content>
