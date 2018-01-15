<%@ Page Title="Tempo Gasto por Etapa na Produção " Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="ProducaoSituacaoData.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ProducaoSituacaoData" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel)
        {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idCliente = FindControl("txtNumCliente", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=ProducaoSituacaoData&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&idPedido=" + idPedido + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente +
                "&exportarExcel=" + exportarExcel);
        }

        function openRptPedido(id)
        {
            openWindow(600, 800, "../RelBase.aspx?rel=Producao&idPedido=" + id + "&situacao=&situacoesPosteriores=false&idSubgrupo=0&tiposSituacoes=0&tipoPedido=0&agrupar=1&pecasProdCanc=0");
        }

        function getCli(id)
        {
            if (id != "")
            {
                var resposta = MetodosAjax.GetCli(id).value.split(";");
                if (resposta[0] == "Erro")
                {
                    alert(resposta[1]);
                    FindControl("txtNomeCliente", "input").value = "";
                }
                else
                    FindControl("txtNomeCliente", "input").value = resposta[1];
            }
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Pedido"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq')" Width="60px"></asp:TextBox>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" ForeColor="#0066FF" Text="Cliente"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:TextBox ID="txtNumCliente" runat="server" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq')" Width="50px" onblur="getCli(this.value)"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq')"></asp:TextBox>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
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
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="Valor" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="Valor" />
                        <asp:BoundField DataField="TotM2" HeaderText="Total m²" SortExpression="TotM2" />
                        <asp:BoundField DataField="Data" DataFormatString="{0:d}" HeaderText="Data Pedido"
                            SortExpression="Data" />
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
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.ProducaoSituacaoDataDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCliente" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNomeCliente" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetores" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.SetorDAO">
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
