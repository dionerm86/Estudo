<%@ Page Title="Retificar Sinal" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstRetificarSinal.aspx.cs" Inherits="Glass.UI.Web.Listas.LstRetificarSinal" %>

<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">
        function selecionaTodos()
        {
            var selecionar = FindControl("chkSelecionarTodos", "input");
            var inputs = FindControl("grdPedidos", "table").getElementsByTagName("input");

            for (i = 0; i < inputs.length; i++)
            {
                if (inputs[i].id != selecionar.id)
                {
                    inputs[i].checked = selecionar.checked;
                    inputs[i].onclick();
                }
            }
        }

        function atualizaPedidos(idPedido, adicionar)
        {
            var controle = FindControl("hdfIdsPedidos", "input");
            var idsPedidos = controle.value.split(',');
            var nova = new Array();

            for (j = 0; j < idsPedidos.length; j++)
            {
                if (idsPedidos[j] != idPedido && idsPedidos[j] != "")
                    nova.push(idsPedidos[j]);
            }

            if (adicionar)
                nova.push(idPedido);

            controle.value = nova.join(",");
        }

        function validar()
        {
            var numPed = FindControl("grdPedidos", "table").rows.length - 1;
            var pedidos = FindControl("hdfIdsPedidos", "input").value;
            var tipo = FindControl("lblSinal", "span").innerHTML.toLowerCase();

            if (pedidos == "")
            {
                alert("Selecione pelo menos 1 pedido para remover do " + tipo + ".");
                return false;
            }
            else if (pedidos.split(',').length == numPed)
            {
                alert("Mantenha ao menos 1 pedido no " + tipo + ".\nPara que todos os pedidos sejam removidos, cancele o " + tipo + ".");
                return false;
            }

            bloquearPagina();
            desbloquearPagina(false);
            return true;
        }
    </script>
    
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblSinal" runat="server" Text="Sinal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td style="padding-left: 2px">
                            <uc1:ctrlSelPopup ID="selSinal" runat="server" ColunasExibirPopup="IdSinal|NomeCliente|TotalSinal|DataCad" 
                                DataSourceID="odsSinal" DataTextField="IdSinal" DataValueField="IdSinal" 
                                FazerPostBackBotaoPesquisar="True" PermitirVazio="False" 
                                TituloTela="Selecione o Sinal" ValidationGroup="id" 
                                TitulosColunas="Núm. Sinal|Cliente|Valor|Data Sinal" TextWidth="80px" />
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
        <tr id="caption" runat="server">
            <td align="center">
                Selecione os pedidos que serão removidos do <%= !IsPagtoAntecipado() ? "sinal" : "pagamento antecipado" %>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdPedidos" runat="server" AutoGenerateColumns="False" 
                    CssClass="gridStyle" DataKeyNames="IdPedido" DataSourceID="odsPedidosSinal" 
                    GridLines="None" ondatabound="grdPedidos_DataBound" 
                    EmptyDataText="A retificação de sinal deve ser utilizada somente se o mesmo possuir mais de um pedido associado, caso contrário deverá ser utilizada a opção de cancelar sinal.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelecionar" runat="server" onclick='<%# Eval("IdPedido", "atualizaPedidos({0}, this.checked)") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelecionarTodos" runat="server" onclick="selecionaTodos()" />
                            </HeaderTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Vendedor" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" SortExpression="Total" />
                        <asp:BoundField DataField="DataEntrada" HeaderText="Data Sinal" SortExpression="DataEntrada" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="ValorEntrada" />
                        <asp:BoundField DataField="ValorPagamentoAntecipado" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="ValorPagamentoAntecipado" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <asp:Label runat="server" ForeColor="Red">Só é possível alterar as datas das movimentações bancárias nessa tela se todas as formas de pagamento do pagto.antecipado selecionado gerarem movimentações bancárias.</asp:Label> 
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblAlterarData" runat="server" ForeColor="#0066FF" Text="Alterar data da movimentação bancária do pagamento antecipado"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlData" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"/>
                        </td>
                        <td>
                            <asp:Button ID="btnAterarDataMov" runat="server" Text="Retificar Data" ToolTip="Retificar data das movimentações bancárias do Pagto. Antecipado" 
                                OnClick="btnAterarDataMov_Click" />
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnRetificarSinal" runat="server" 
                    onclick="btnRetificarSinal_Click" Text="Retificar Sinal" 
                    onclientclick="if (!validar()) return false" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedidosSinal" runat="server" 
                    SelectMethod="GetBySinal" TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="selSinal" Name="idSinal" PropertyName="Valor" Type="UInt32" />
                        <asp:QueryStringParameter DefaultValue="" Name="pagtoAntecipado" QueryStringField="antecipado" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSinal" runat="server" 
                    SelectMethod="GetForRetificar" TypeName="Glass.Data.DAL.SinalDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="pagamentoAntecipado" QueryStringField="antecipado" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfPagtoAntecipado" runat="server" />
                <asp:HiddenField ID="hdfIdSinal" runat="server" />
                <asp:HiddenField ID="hdfIdsPedidos" runat="server" />
            </td>            
        </tr>
    </table>
</asp:Content>

