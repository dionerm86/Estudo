<%@ Page Title="Agendamento de Instalações" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstAgendamentoInstalacao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstAgendamentoInstalacao" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        // Abre relatório das ordens de instalação
        function openRpt(exportarExcel) {
            var dataIni = FindControl("ctrlDataInicial_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFinal_txtData", "input").value;
            var cliente = FindControl("ckbCliente", "input").checked;
            var equipe = FindControl("ckbEquipe", "input").checked;
            if (cliente) {
                tipo = "cliente";
                id = FindControl("txtNumCli", "input").value == "" ? "0" : FindControl("txtNumCli", "input").value;
            }
            if (equipe) {
                tipo = "equipe";
                id = FindControl("drpEquipe", "select").value;
            }
            

            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=AgendamentoInstalacao&id=" + id +
                "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&exportarExcel=" + exportarExcel + "&tipo=" + tipo);

            return false;
        }

        function difDate(data1, data2) {
            var d1 = new Date(data1.substr(6, 4), data1.substr(3, 2) - 1, data1.substr(0, 2));
            var d2 = new Date(data2.substr(6, 4), data2.substr(3, 2) - 1, data2.substr(0, 2));
            return Math.ceil((d2.getTime() - d1.getTime()) / 1000 / 60 / 60 / 24);

        }
        function RangeData(source, args) {

            var dataIni = FindControl("ctrlDataInicial_txtData", "input").value;
            var dataFim = args.Value;

            if (difDate(dataIni, dataFim) > 30) {
                args.IsValid = false;
                alert("O período deve corresponder ao intervalo de um mês.");
            }
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            var retorno = LstAgendamentoInstalacao.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtCliente", "input").value = "";
                return false;
            }

            FindControl("txtCliente", "input").value = retorno[1];
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:RadioButton ID="ckbEquipe" GroupName="filtro" runat="server" Checked="true"
                                ForeColor="#0066FF" Text="Equipe" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:RadioButton ID="ckbCliente" GroupName="filtro" runat="server" ForeColor="#0066FF"
                                Text="Cliente" AutoPostBack="true" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" id="tdEquipe" runat="server">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Equipe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpEquipe" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsEquipe" DataTextField="NomeEstendido" DataValueField="IdEquipe">
                                <asp:ListItem Value="0">TODAS</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" ValidationGroup="c" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" id="tdCliente" runat="server">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('pesqCli', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="pesqCli" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" id="tdPeriodo" runat="server">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataInicial" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFinal" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" ValidationGroup="c" />
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
            <td align="center" id="tdGridEquipe" runat="server">
                <asp:GridView GridLines="None" ID="grdAgendamento" runat="server" AllowPaging="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AutoGenerateColumns="False" DataSourceID="odsAgendamento"
                    EmptyDataText="Nenhum agendamento encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdEquipeInstalacao" HeaderText="Cód. Equipe" SortExpression="IdEquipeInstalacao">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="100px" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NomeEquipe" HeaderText="Equipe" SortExpression="NomeEquipe">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="200px" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataInstalacao" HeaderText="Data da Instalação" SortExpression="DataInstalacao"
                            DataFormatString="{0:d}">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="150px" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="QuantidadeInstalacao" HeaderText="Quantidade" SortExpression="QuantidadeInstalacao">
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle Width="100px" HorizontalAlign="Right" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAgendamento" runat="server" SelectMethod="ObterListaPorEquipe"
                    TypeName="Glass.Data.DAL.AgendamentoInstalacaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpEquipe" Name="idEquipe" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataInicial" Name="dataInicial" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFinal" Name="dataFinal" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEquipe" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.EquipeDAO"
                    MaximumRowsParameterName="" StartRowIndexParameterName="">
                </colo:VirtualObjectDataSource>
                <br />
                <a id="lnkImprimir" href="#" onclick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center" id="tdGridCliente" runat="server">
                <asp:GridView GridLines="None" ID="grdCliente" runat="server" AllowPaging="True"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AutoGenerateColumns="False" DataSourceID="odsCliente"
                    EmptyDataText="Nenhum agendamento encontrado.">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:BoundField DataField="IdEquipeInstalacao" HeaderText="Cód. Equipe" SortExpression="IdEquipeInstalacao">
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle Width="100px" HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="NomeEquipe" HeaderText="Equipe" SortExpression="NomeEquipe">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="200px" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Cliente" HeaderText="Cliente" SortExpression="Cliente">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="200px" HorizontalAlign="Center" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataInstalacao" HeaderText="Data da Instalação" SortExpression="DataInstalacao"
                            DataFormatString="{0:d}">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="150px" HorizontalAlign="Center" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCliente" runat="server" SelectMethod="ObterListaPorCliente"
                    TypeName="Glass.Data.DAL.AgendamentoInstalacaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataInicial" Name="dataInicial" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFinal" Name="dataFinal" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <br />
                <a id="A1" href="#" onclick="return openRpt(false);">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openRpt(true); return false;"> <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
            </td>
        </tr>
    </table>
</asp:Content>
