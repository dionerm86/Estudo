<%@ Page Title="M² por etapa na produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ProducaoSituacao.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ProducaoSituacao" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel)
        {
            var idFunc = FindControl("drpFuncionario", "select").value;
            var idPedido = FindControl("txtPedido", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var agrupar = FindControl("chkAgrupar", "input").checked;

            openWindow(600, 800, "RelBase.aspx?rel=ProducaoSituacao&idFunc=" + idFunc + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&exportarExcel=" + exportarExcel + "&agrupar=" + agrupar + "&idPedido=" + idPedido + "&agrupar=0");
        }

        function openRptPedido(id)
        {
            openWindow(600, 800, "../RelBase.aspx?rel=Producao&idPedido=" + id + "&situacoesPosteriores=false&pecasProdCanc=0&agrupar=0");
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Funcionário (setor)"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="true"
                                DataSourceID="odsFuncionarios" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:CheckBox ID="chkAgrupar" runat="server" Text="Agrupar impressão por funcionário" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:TextBox ID="txtPedido" runat="server" Width="60px" onkeyup="if (isEnter(event)) cOnClick('imgPesq1');"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPeriodo" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
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
                <asp:GridView GridLines="None" ID="grdProducaoSituacao" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsProducaoSituacao" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AllowPaging="True"
                    PageSize="15" OnLoad="grdProducaoSituacao_Load">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRptPedido(" + Eval("IdPedido") + "); return false" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Data" DataFormatString="{0:d}" HeaderText="Data" SortExpression="Data" />
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="Valor" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="Valor" />
                        <asp:BoundField DataField="TotM2" HeaderText="Total m²" SortExpression="TotM2" />
                        <asp:BoundField DataField="DataConf" DataFormatString="{0:d}" HeaderText="Data Conf."
                            SortExpression="DataConf" />
                        <asp:BoundField DataField="DataLiberacao" DataFormatString="{0:d}" HeaderText="Data Lib."
                            SortExpression="DataLiberacao" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProducaoSituacao" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.ProducaoSituacaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFuncSetor" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idPedido" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionarios" runat="server" SelectMethod="GetProducao"
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
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"><img border="0" 
                    src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
