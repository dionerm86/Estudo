<%@ Page Title="Relatório de Entrega por Rota" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaEntregaRota.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaEntregaRota" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function checkAll(checked) {
            var tabela = document.getElementById("<%= grdDados.ClientID %>");
            var inputs = tabela.getElementsByTagName("input");

            for (i = 0; i < inputs.length; i++) {
                if (inputs[i].id.indexOf("chkTodos") > -1 || inputs[i].type != "checkbox")
                    continue;

                inputs[i].checked = checked;
            }
        }

        function getValoresMarcados() {
            var retorno = new Array();
            var tabela = document.getElementById("<%= grdDados.ClientID %>");

            for (i = 0; i < tabela.rows.length; i++) {
                var checkbox = FindControl("chkMarcar", "input", tabela.rows[i]);
                if (checkbox == null || !checkbox.checked)
                    continue;
                else
                    retorno.push(checkbox.value);

            }

            return retorno.join(",");
        }

        function openRpt(excel) {
            var idsLiberacao = getValoresMarcados();
            var rotaId = FindControl("drpRota", "select").value;
            var rota = FindControl("drpRota", "select").options[FindControl("drpRota", "select").selectedIndex].label
            var dataIni = FindControl("txtDataIni", "input").value;
            var dataFim = FindControl("txtDataFim", "input").value;
            var motorista = FindControl("drpFuncionario", "select").options[FindControl("drpFuncionario", "select").selectedIndex].value != "0" ? FindControl("drpFuncionario", "select").options[FindControl("drpFuncionario", "select").selectedIndex].label : "";
            var idVeiculo = FindControl("drpVeiculo", "select").value;
            var agruparPorCidade = FindControl("chkAgruparPorCidade", "input").checked;

            if (idsLiberacao.length > 0)
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=EntregaPorRota&ids=" + idsLiberacao +
                    "&rota=" + rota + "&rotaId=" + rotaId + "&dataIni=" + dataIni + "&dataFim=" + dataFim +
                    "&motorista=" + motorista + "&idVeiculo=" + idVeiculo + "&agruparPorCidade=" + agruparPorCidade +
                    "&ExportaExcel=" + excel);
        }

    
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Selecione uma Rota</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data da Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="txtDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgruparPorCidade" runat="server" Text="Agrupar relatório por cidade" ForeColor="#0066FF"/>
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
                <asp:GridView GridLines="None" ID="grdDados" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum registro encontrado." 
                    DataSourceID="odsEntregaRota" OnDataBound="grdDados_DataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <input type="checkbox" id="chkMarcar" value='<%# Eval("IdLiberacao") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkTodos" runat="server" onclick="checkAll(this.checked)" />
                            </HeaderTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdLiberacao" HeaderText="Liberação" SortExpression="IdLiberacao" />
                        <asp:BoundField DataField="Cliente" HeaderText="Cliente" SortExpression="Cliente" />
                        <asp:BoundField DataField="Cidade" HeaderText="Cidade" SortExpression="Cidade" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:BoundField DataField="Peso" HeaderText="Peso" SortExpression="Peso" 
                            DataFormatString="{0:N2}" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" SortExpression="Valor" DataFormatString="{0:c}" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEntregaRota" runat="server" SelectMethod="ObterLista"
                    TypeName="Glass.Data.RelDAL.EntregaPorRotaDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpRota" Name="idRota" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:Parameter Name="idsLiberacao" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetMotoristas"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:Parameter Name="nome" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVeiculo" runat="server" SelectMethod="GetOrdered" 
                    TypeName="Glass.Data.DAL.VeiculoDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="parametrosRelatorio" runat="server" visible="false">
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Motorista" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Selecione um motorista</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            &nbsp;</td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Veículo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVeiculo" runat="server" DataSourceID="odsVeiculo" 
                                DataTextField="DescricaoCompleta" DataValueField="Placa">
                            </asp:DropDownList>
                        </td>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td align="center" colspan="6">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false); return false"> <img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
