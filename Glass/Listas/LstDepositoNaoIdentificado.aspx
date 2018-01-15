<%@ Page Title="Depósitos não identificados" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstDepositoNaoIdentificado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstDepositoNaoIdentificado" %>

<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlCancLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openMotivoCanc(idDepositoNaoIdentificado) {
            openWindow(150, 450, "../Utils/SetMotivoCancDepositoNaoIdentificado.aspx?idDepositoNaoIdentificado=" + idDepositoNaoIdentificado);
            return false;
        }

        function openRpt(exportarExcel) {
            var idDepositoNaoIdentificado = FindControl("txtIdDepositoNaoIdentificado", "input").value;
            var idContaBanco = FindControl("drpContaBanco", "select").value;
            var valorIni = FindControl("txtValorInicial", "input").value;
            var valorFim = FindControl("txtValorFinal", "input").value;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var dataMovIni = FindControl("ctrlDataMovIni_txtData", "input").value;
            var dataMovFim = FindControl("ctrlDataMovFim_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;

            var query = "../Relatorios/RelBase.aspx?Rel=ListaDepositoNaoIdentificado"
                        + "&idDepositoNaoIdentificado=" + idDepositoNaoIdentificado
                        + "&idContaBanco=" + idContaBanco
                        + "&valorIni=" + valorIni
                        + "&valorFim=" + valorFim
                        + "&dataCadIni=" + dataCadIni
                        + "&dataCadFim=" + dataCadFim
                        + "&dataMovIni=" + dataMovIni
                        + "&dataMovFim=" + dataMovFim
                        + "&situacao=" + situacao
                        + "&exportarExcel=" + exportarExcel;

            openWindow(600, 800, query);

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Depósito não identificado"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdDepositoNaoIdentificado" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label22" runat="server" ForeColor="#0066FF" Text="Conta Bancária"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsContaBanco" DataTextField="Descricao" DataValueField="IdContaBanco">
                                <asp:ListItem Value="0" Text="Todas" Selected="True" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" Style="width: 16px" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label21" runat="server" ForeColor="#0066FF" Text="Valor"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            até
                        </td>
                        <td>
                            <asp:TextBox ID="txtValorFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Text="Todas" Selected="True" />
                                <asp:ListItem Value="1" Text="Ativo" />
                                <asp:ListItem Value="3" Text="Em Uso" />
                                <asp:ListItem Value="2" Text="Cancelado" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Período Cadastro"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Período Movimentação"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataMovIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataMovFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click"> Inserir depósito não identificado</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdDepositoNaoIdentificado" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdDepositoNaoIdentificado"
                    DataSourceID="odsDepositoNaoIdentificado" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum depósito não identificado encontrado.">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" ImageUrl="~/Images/EditarGrid.gif"
                                    NavigateUrl='<%# "../Cadastros/CadDepositoNaoIdentificado.aspx?idDepositoNaoIdentificado=" + Eval("IdDepositoNaoIdentificado") %>'
                                    Visible='<%# Eval("EditarVisible") %>' />
                                <asp:ImageButton ID="imbCancelar" runat="server" ToolTip="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "return openMotivoCanc(" + Eval("IdDepositoNaoIdentificado") + ");" %>'
                                    Visible='<%# Eval("CancelarVisible") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="idDepositoNaoIdentificado" HeaderText="Cód." SortExpression="idDepositoNaoIdentificado" />
                        <asp:BoundField DataField="DescrContaBanco" HeaderText="Conta Bancária" SortExpression="DescrContaBanco" />
                        <asp:BoundField DataField="Referencia" HeaderText="Referência" />
                        <asp:BoundField DataField="ValorMov" HeaderText="Valor da Mov." SortExpression="ValorMov" />
                        <asp:BoundField DataField="DataMovString" HeaderText="Data da Mov." SortExpression="DataMov" />
                        <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="Situacao" />
                        <asp:BoundField DataField="Obs" HeaderText="Obs." SortExpression="Obs" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc2:ctrlLogPopup ID="ctrlLogPopupDni" runat="server" Tabela="DepositoNaoIdentificado" IdRegistro='<%# Eval("IdDepositoNaoIdentificado") %>' />
                                <uc1:ctrlCancLogPopup ID="ctrlLogCancPopup1" runat="server" Tabela="DepositoNaoIdentificado"
                                    IdRegistro='<%# Eval("idDepositoNaoIdentificado") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle />
                </asp:GridView>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton><br />
                <br />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDepositoNaoIdentificado" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetListCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.DepositoNaoIdentificadoDAO"
                    >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdDepositoNaoIdentificado" Name="idDepositoNaoIdentificado"
                            PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtValorInicial" Name="valorIni" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="txtValorFinal" Name="valorFim" PropertyName="Text"
                            Type="Decimal" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataMovIni" Name="dataMovIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataMovFim" Name="dataMovFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
