<%@ Page Title="Movimentações Bancárias" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstMovConta.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMovConta" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idContaBanco = FindControl("drpContaBanco", "select").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var chkApenasDinheiro = FindControl("chkApenasDinheiro", "input");
            var chkApenasManual = FindControl("chkApenasManual", "input").checked;
            var tipoMov = FindControl("drpTipoMov", "select").value;
            var valorInicial = FindControl("txtValorInicial", "input").value;
            var valorFinal = FindControl("txtValorFinal", "input").value;

            var apenasDinheiro = chkApenasDinheiro ? chkApenasDinheiro.checked : false;

            if (valorInicial == "") valorInicial = 0;
            if (valorFinal == "") valorFinal = 0;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ExtratoBancario&idContaBanco=" + idContaBanco + "&dataIni=" + dtIni +
                "&dataFim=" + dtFim + "&valorInicial=" + valorInicial + "&valorFinal=" + valorFinal + "&apenasDinheiro=" + apenasDinheiro +
                "&tipoMov=" + tipoMov + "&lancManual=" + chkApenasManual + "&exportarExcel=" + exportarExcel);

            return false;
        }

        function openLog()
        {
            openWindow(600, 800, '../Utils/ShowLogCancelamento.aspx?tabela=<%= GetCodigoTabela() %>');
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblPeriodo0" runat="server" Text="Conta Bancária" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco"
                                OnDataBound="drpContaBanco_DataBound" OnSelectedIndexChanged="drpContaBanco_DataBound">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="lblPeriodo" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:Label ID="Label18" runat="server" Text="Valor Mov." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtValorInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            até
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtValorFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:Label ID="lblTipoMov" runat="server" Text="Movimentações" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:DropDownList ID="drpTipoMov" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Entradas</asp:ListItem>
                                <asp:ListItem Value="2">Saídas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkApenasDinheiro" runat="server" Text="Apenas movimentações em dinheiro" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:CheckBox ID="chkApenasManual" runat="server" Text="Apenas lançamentos manuais" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdMovBanco" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsMovBanco" OnDataBound="grdMovBanco_DataBound" DataKeyNames="IdMovBanco"
                    OnRowCommand="grdMovBanco_RowCommand" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Edit" Visible='<%# Eval("EditVisible") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "openWindow(200, 500, \"../Utils/SetMotivoCancReceb.aspx?tipo=movBanco&id=" + Eval("IdMovBanco") + "\"); return false;" %>'
                                    ToolTip="Excluir" Visible='<%# Eval("ExcluirVisible") %>' />
                                <asp:HiddenField ID="hdfIdContaBancoDest" runat="server" Value='<%# Eval("IdContaBancoDest") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" OnClientClick="return onSave(false);" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cod. Mov." SortExpression="IdCaixaGeral">
                            <ItemTemplate>
                                <asp:Label ID="lblCodMov" runat="server" Text='<%# Eval("CodMov") %>'></asp:Label>
                                <asp:HiddenField ID="hdfTipoMov" runat="server" Value='<%# Eval("TipoMov") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Eval("Referencia") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente / Fornecedor" SortExpression="NomeClienteFornecedor">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("NomeClienteFornecedor") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("NomeClienteFornecedor") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conta Bancária" SortExpression="DescrContaBanco">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("DescrContaBanco") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("DescrContaBanco") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="DataMov">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DataMovString", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                            
                            
                            <uc1:ctrlData ID="ctrlData" runat="server"  Data='<%# Bind("DataMov") %>'
                                ValidateEmptyText="true" />
                            
                            
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referente a" SortExpression="DescrPlanoConta">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DescrPlanoConta") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("DescrPlanoConta") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>' Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorMov">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("ValorString", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("ValorMov", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Saldo" SortExpression="Saldo">
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("Saldo", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("Saldo", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="D/C" SortExpression="DescrTipoMov">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("DescrTipoMov") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescrTipoMov") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Cad." SortExpression="DataCad">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("DataCadString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cadastrado por" SortExpression="DescrUsuCad">
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("DescrUsuCad") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgUp" runat="server" CommandArgument='<%# Eval("IdMovBanco") %>'
                                    CommandName="Up" ImageUrl="~/Images/up.gif" Visible='<%# Eval("UpDownVisible") %>' />
                                &nbsp;<asp:ImageButton ID="imgDown" runat="server" CommandArgument='<%# Eval("IdMovBanco") %>'
                                    CommandName="Down" ImageUrl="~/Images/down.gif" Visible='<%# Eval("UpDownVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
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
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"
                    Visible="False">
                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;">
                    <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkCanceladas" runat="server" OnClientClick="openLog(); return false">
                    <img alt="" border="0" src="../Images/ExcluirGrid.gif" /> Movimentações Excluídas</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMovBanco" runat="server" SelectMethod="GetMovimentacoes"
                    TypeName="Glass.Data.DAL.MovBancoDAO" DataObjectTypeName="Glass.Data.Model.MovBanco"
                    UpdateMethod="AtualizaDataObs"  OnUpdated="odsMovBanco_Updated">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="txtValorInicial" Name="valorInicial" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFinal" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="drpTipoMov" Name="tipoMov" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkApenasManual" Name="lancManual" PropertyName="Checked"
                            Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
