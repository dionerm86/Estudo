<%@ Page Title="Consulta de Motivos de Finalização/Confirmação pelo Financeiro" Language="C#"
    MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstConsultarMotivoFinalizacaoFinanceiro.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstConsultarMotivoFinalizacaoFinanceiro" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt() {
            var idPedido = FindControl("txtIdPedido", "input").value;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var dataIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var motivo = FindControl("drpMotivo", "select").itens();
            var numCli = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;

            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=MotivoFinalizacaoFinanceiro&idPedido=" + idPedido +
                "&idFunc=" + idFunc + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&motivo=" + motivo + "&numCli=" + numCli + "&nomeCli=" + nomeCli);
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return false;

            var idCliente = idCli.value;

            var retorno = MetodosAjax.GetCli(idCliente);

            if (retorno.error != null) {
                alert(result.error.description);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return
            }

            var dados = retorno.value.split(';');

            if (dados[0] == "Erro") {
                alert(dados[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = dados[1];
            FindControl("txtNumCli", "input").value = idCliente;

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Período Cadastro" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataCadIni" runat="server" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataCadFim" runat="server" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="drpMotivo" Title="Selecione o motivo"
                                Width="140px">
                                <asp:ListItem Value="1">Finalização</asp:ListItem>
                                <asp:ListItem Value="2" Style="color: Red">Negação ao Finalizar</asp:ListItem>
                                <asp:ListItem Value="3">Confirmação</asp:ListItem>
                                <asp:ListItem Value="4" Style="color: Red">Negação ao Confirmar</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView ID="grdObservacaoFinanceiro" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsObservacaoFinanceiro"
                    GridLines="None" OnRowDataBound="grdObservacaoFinanceiro_RowDataBound" EmptyDataText="Não há registros para o filtro especificado.">
                    <Columns>
                        <asp:BoundField DataField="CodigoPedido" HeaderText="Pedido" ReadOnly="True" SortExpression="CodigoPedido" />
                        <asp:BoundField DataField="IdNomeCliente" HeaderText="Cliente" ReadOnly="True" SortExpression="IdNomeCliente" />
                        <asp:BoundField DataField="Motivo" HeaderText="Ação" ReadOnly="True" SortExpression="Motivo" />
                        <asp:BoundField DataField="Observacao" HeaderText="Observação" SortExpression="Observacao" />
                        <asp:BoundField DataField="NomeFuncionarioCadastro" HeaderText="Funcionário" ReadOnly="True"
                            SortExpression="NomeFuncionarioCadastro" />
                        <asp:BoundField DataField="DataCadastro" HeaderText="Data Cadastro" ReadOnly="True"
                            SortExpression="DataCadastro" />
                        <asp:BoundField DataField="MotivoFinanceiro" HeaderText="Motivo Financeiro" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsObservacaoFinanceiro" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="ObtemNumeroObservacoesFinanceiro"
                    SelectMethod="ObtemObservacoesFinanceiro" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="WebGlass.Business.Pedido.Fluxo.MotivoFinalizacaoFinanceiro">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFuncCad" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpMotivo" Name="motivo" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCliente" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCliente" PropertyName="Text"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetFinanceiros" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false">
                    <img src="../images/Printer.png" border="0" /> Imprimir
                </asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
