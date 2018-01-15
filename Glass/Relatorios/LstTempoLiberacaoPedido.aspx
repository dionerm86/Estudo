<%@ Page Title="Tempo gasto para liberação" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstTempoLiberacaoPedido.aspx.cs" Inherits="Glass.UI.Web.Relatorios.LstTempoLiberacaoPedido" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idCliente = FindControl("txtNumCliente", "input").value;
            var nomeCliente = FindControl("txtNomeCliente", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idRota = FindControl("drpRota", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=TempoGastoParaLiberacao&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&idPedido=" + idPedido + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&idLoja=" + idLoja + "&idRota=" + idRota +
                "&exportarExcel=" + exportarExcel);
        }

        function openRptPedido(id) {
            openWindow(600, 800, "RelBase.aspx?rel=Producao&idPedido=" + id + "&situacoesPosteriores=false&pecasProdCanc=0");
        }

        function getCli(id) {
            if (id != "") {
                var resposta = MetodosAjax.GetCli(id).value.split(";");
                if (resposta[0] == "Erro") {
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
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" MostrarTodas="true" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Selecione uma Rota</asp:ListItem>
                            </asp:DropDownList>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
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
                            <uc1:ctrldata id="ctrlDataIni" runat="server" readonly="ReadWrite" exibirhoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrldata id="ctrlDataFim" runat="server" readonly="ReadWrite" exibirhoras="False" />
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
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProducaoSituacao" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsProducaoSituacao" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AllowPaging="True"
                    PageSize="15" >
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="Valor" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="Valor" />
                        <asp:BoundField DataField="TotM2" HeaderText="Total m²" SortExpression="TotM2" />
                        <asp:BoundField DataField="Data" DataFormatString="{0:d}" HeaderText="Data Pedido"
                            SortExpression="Data" />
                        <asp:BoundField DataField="DataLiberacao" DataFormatString="{0:d}" HeaderText="Data Lib."
                            SortExpression="DataLiberacao" />
                        <asp:BoundField DataField="DiferencaDataLiberacao"  HeaderText="Diferença"
                            SortExpression="DiferencaDataLiberacao" >
                        <ItemStyle HorizontalAlign="Center" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProducaoSituacao" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCountTempoLiberacao" SelectMethod="GetListTempoLiberacao"
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
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpRota" Name="idRota" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSetores" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
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
