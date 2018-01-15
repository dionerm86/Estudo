<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstRetificarSinalCompra.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstRetificarSinalCompra" Title="Retificar Sinal da Compra" %>

<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function selecionaTodos()
        {
            var selecionar = FindControl("chkSelecionarTodos", "input");
            var inputs = FindControl("grdCompras", "table").getElementsByTagName("input");

            for (i = 0; i < inputs.length; i++)
            {
                if (inputs[i].id != selecionar.id)
                {
                    inputs[i].checked = selecionar.checked;
                    inputs[i].onclick();
                }
            }
        }

        function atualizaCompra(idCompra, adicionar)
        {
            var controle = FindControl("hdfIdsCompras", "input");
            var idsCompras = controle.value.split(',');
            var nova = new Array();

            for (j = 0; j < idsCompras.length; j++)
            {
                if (idsCompras[j] != idCompra && idsCompras[j] != "")
                    nova.push(idsCompras[j]);
            }

            if (adicionar)
                nova.push(idCompra);

            controle.value = nova.join(",");
        }

        function validar()
        {
            var numCompra = FindControl("grdCompras", "table").rows.length - 1;
            var compras = FindControl("hdfIdsCompras", "input").value;

            if (compras == "")
            {
                alert("Selecione pelo menos 1 compra para remover do sinal.");
                return false;
            }
            else if (compras.split(',').length == numCompra)
            {
                alert("Mantenha ao menos 1 compra no Sinal.\nPara que todos as compras sejam removidas, cancele o sinal.");
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
                            <asp:Label ID="lblSinalCompra" runat="server" Text="Sinal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td style="padding-left: 2px">
                            <uc1:ctrlSelPopup ID="selSinal" runat="server" ColunasExibirPopup="IdSinalCompra|NomeFornecedor|TotalSinal|DataCad"
                                DataSourceID="odsSinalCompra" DataTextField="IdSinalCompra" DataValueField="IdSinalCompra" FazerPostBackBotaoPesquisar="True"
                                PermitirVazio="False" TituloTela="Selecione o Sinal" ValidationGroup="id" TitulosColunas="Id|Núm. Sinal|Fornec.|Valor|Data Sinal"
                                TextWidth="80px" />
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
                Selecione as compras que serão removidas do sinal.
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdCompras" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="IdCompra" DataSourceID="odsComprasSinal" GridLines="None" OnDataBound="grdCompras_DataBound">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelecionar" runat="server" onclick='<%# Eval("IdCompra", "atualizaCompra({0}, this.checked)") %>' />
                            </ItemTemplate>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelecionarTodos" runat="server" onclick="selecionaTodos()" />
                            </HeaderTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCompra" HeaderText="Compra" SortExpression="IdCompra" />
                        <asp:BoundField DataField="NomeFornec" HeaderText="Fornec." SortExpression="NomeFornec" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Vendedor" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" SortExpression="Total" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data Sinal" SortExpression="DataCad" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:c}" HeaderText="Valor"
                            SortExpression="ValorEntrada" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <br />
                <asp:Button ID="btnRetificarSinal" runat="server" OnClick="btnRetificarSinal_Click"
                    Text="Retificar Sinal" OnClientClick="if (!validar()) return false" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComprasSinal" runat="server" SelectMethod="GetBySinalCompra"
                    TypeName="Glass.Data.DAL.CompraDAO" >
                    <SelectParameters>
                        <asp:ControlParameter ControlID="selSinal" Name="idSinalCompra" PropertyName="Valor" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSinalCompra" runat="server" SelectMethod="GetForRetificar"
                    TypeName="Glass.Data.DAL.SinalCompraDAO" >
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdSinalCompra" runat="server" />
                <asp:HiddenField ID="hdfIdsCompras" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
