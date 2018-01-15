<%@ Page Title="Clientes" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadClienteMulti.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadClienteMulti" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function IgnorarBloqueioPedPronto(valor) {
        
            var rota = FindControl("cbdRota", "select").itens();
            var revenda = FindControl("cdbTipoPedido", "select").itens();

            var ids = CadClienteMulti.GetData(rota, revenda).value;

            var retorno = CadClienteMulti.IgnorarBloqueioPedPronto(ids, valor).value;

            alert(retorno);
        }

        function BloquearPedidoContaVencida(valor) {
            var rota = FindControl("cbdRota", "select").itens();
            var revenda = FindControl("cdbTipoPedido", "select").itens();

            var ids = CadClienteMulti.GetData(rota, revenda).value;

            var retorno = CadClienteMulti.BloquearPedidoContaVencida(ids, valor).value;

            alert(retorno);
        }
    
    </script>

    <table style="width: 100%" align="center">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdRota" runat="server" CheckAll="True">
                                <asp:ListItem Value="0">Sem Rota</asp:ListItem>
                                <asp:ListItem Value="1">Com Rota</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" OnClientClick="return openRota();" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Revenda" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cdbTipoPedido" runat="server" CheckAll="True">
                                <asp:ListItem Value="0">Não Revenda</asp:ListItem>
                                <asp:ListItem Value="1">Revenda</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" OnClientClick="return openRota();" />
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
                <asp:GridView GridLines="None" ID="grdCli" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdCli" EmptyDataText="Nenhum cliente encontrado."
                    DataSourceID="odsCli">
                    <Columns>
                        <asp:BoundField DataField="IdNome" HeaderText="Nome" SortExpression="Nome">
                        </asp:BoundField>
                        <asp:BoundField DataField="CpfCnpj" HeaderText="CPF/CNPJ" SortExpression="CpfCnpj">
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="IgnorarBloqueioPedPronto" SortExpression="IgnorarBloqueioPedPronto"
                            Visible="False">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IgnorarBloqueioPedPronto") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IgnorarBloqueioPedPronto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="BloquearPedidoContaVencida" SortExpression="BloquearPedidoContaVencida"
                            Visible="False">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("BloquearPedidoContaVencida") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("BloquearPedidoContaVencida") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCli" runat="server" SelectMethod="ObterClientesRotaRevenda"
                    TypeName="Glass.Data.DAL.ClienteDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="cbdRota" Name="rota" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cdbTipoPedido" Name="revenda" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center" style="padding: 3px">
                <table class="gridStyle" cellspacing="0" border="0" style="border-collapse:collapse";>
                    <tbody>
                        <tr>
                            <th align="center" style="padding: 3px">
                                Ignorar Bloqueio Pedido Pronto
                            </th>
                            <th align="center" style="padding: 3px">
                                Bloquear Pedido Conta Vencida
                            </th>
                        </tr>
                        </tbody>
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnHabilitarIBPP" runat="server" Text="Habilitar" OnClientClick="if(!confirm('Deseja MARCAR a opção Ignorar Bloqueio Pedido Pronto para os clientes filtrados acima?')) return false; else IgnorarBloqueioPedPronto(true);" />
                               &nbsp;<br />
                                <asp:Button ID="btnDesabilitarIBPP" runat="server" Text="Desabilitar" OnClientClick="if(!confirm('Deseja DESMARCAR a opção Ignorar Bloqueio Pedido Pronto para os clientes filtrados acima?')) return false; else IgnorarBloqueioPedPronto(false);"  />
                            </td>
                             <td>
                                <asp:Button ID="btnHabilitarBPCV" runat="server" Text="Habilitar" OnClientClick="if(!confirm('Deseja MARCAR a opção Bloquear Pedido Conta Vencida para os clientes filtrados acima?')) return false; else BloquearPedidoContaVencida(true);" />
                               &nbsp;<br />
                                 <asp:Button ID="btnDesabilitarBPCV" runat="server" Text="Desabilitar" OnClientClick="if(!confirm('Deseja DESMARCAR a opção Bloquear Pedido Conta Vencida para os clientes filtrados acima?')) return false;else BloquearPedidoContaVencida(false);" />
                            </td>
                        </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
