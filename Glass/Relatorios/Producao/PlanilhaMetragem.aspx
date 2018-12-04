<%@ Page Title="Planilha de Metragem" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="PlanilhaMetragem.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.PlanilhaMetragem" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idImpressao = FindControl("txtNumImpressao", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var dataIniEnt = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dataFimEnt = FindControl("ctrlDataFimEnt_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var setor = FindControl("drpSetor", "select").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var exibirAgrupadas = FindControl("chkAgruparPecas", "input").checked;
            var idsRotas = FindControl("drpRota", "select").itens();
            var idTurno = FindControl("ddlTurno", "select").value;

            if (idImpressao == "") idImpressao = 0;

            openWindow(600, 800, "RelBase.aspx?rel=PlanilhaMetragem&idPedido=" + idPedido + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&dataIni=" + dataIni +
             "&idSetor=" + setor + "&dataFim=" + dataFim + "&dataIniEnt=" + dataIniEnt + "&dataFimEnt=" + dataFimEnt + "&situacao=" + situacao +
              "&setoresPosteriores=true" + "&idImpressao=" + idImpressao + "&exportarExcel=" + exportarExcel + "&idFunc=" + idFunc +
               "&agruparPecas=" + exibirAgrupadas + "&idsRotas=" + idsRotas + "&idTurno=" + idTurno);
        }

        function getCli(idCli) {
            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }
    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Pedido Cli." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodPedCli" runat="server" Width="70px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq10" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="180px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpSetor_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Produção</asp:ListItem>
                                <asp:ListItem Value="2">Perda</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Setor"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSetor" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsSetor" DataTextField="Descricao" DataValueField="IdSetor" OnSelectedIndexChanged="drpSetor_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Turno" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlTurno" runat="server" DataSourceID="odsTurno" DataTextField="Descricao"
                                DataValueField="IdTurno" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPeriodoSit" runat="server" ForeColor="#0066FF" Text="Período" Visible="False"></asp:Label>&nbsp;
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" Visible="false" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Período (Entrega)"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="drpRota" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota" Title="Selecione a rota">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Funcionário Setor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                AutoPostBack="True" DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Num. Impressão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumImpressao" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparPecas" runat="server" Text="Exibir total de peças agrupadas" />
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
                <asp:GridView GridLines="None" ID="grdMetragem" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsMetragem" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NumPeca" HeaderText="Peça N.º" SortExpression="NumPeca" />
                        <asp:BoundField DataField="Cor" HeaderText="Cor" SortExpression="Cor" />
                        <asp:BoundField DataField="Espessura" HeaderText="Espessura" SortExpression="Espessura" />
                        <asp:BoundField DataField="Medidas" HeaderText="Medidas" SortExpression="Medidas" />
                        <asp:BoundField DataField="TotM2" HeaderText="Área M2" SortExpression="TotM2" />
                        <asp:BoundField DataField="Obs" HeaderText="Observação" SortExpression="Obs" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsMetragem" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetMetragensCount" SelectMethod="GetMetragens"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.MetragemDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumImpressao" Name="idImpressao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodPedCli" Name="codPedCli" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dataIniEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dataFimEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpRota" Name="idsRotas" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:Parameter DefaultValue="true" Name="setoresPosteriores" Type="Boolean" />
                        <asp:ControlParameter ControlID="ddlTurno" Name="idTurno" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetProducao" TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpSetor" Name="idSetor" PropertyName="SelectedValue"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.RotaDAO" CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges"
                    MaximumRowsParameterName="" SkinID="" StartRowIndexParameterName="">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTurno" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.TurnoDAO">
                </colo:VirtualObjectDataSource>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false;"><img border="0" 
                    src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
