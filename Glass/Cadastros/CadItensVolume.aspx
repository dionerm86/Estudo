<%@ Page Title="Itens do Volume" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadItensVolume.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadItensVolume" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script src="../scripts/calcProd.js" type="text/javascript"></script>

    <script type="text/javascript">

        function fecharPagina() {
            window.opener.fecharGerarVolume();
            closeWindow();
        }

        function selecionaTodosProdutos(check) {
            var tabela = check;
            while (tabela.nodeName.toLowerCase() != "table")
                tabela = tabela.parentNode;

            var checkBoxProdutos = tabela.getElementsByTagName("input");

            var i = 0;
            for (i = 0; i < checkBoxProdutos.length; i++) {
                if (checkBoxProdutos[i].id.indexOf("chkTodos") > -1 || checkBoxProdutos[i].disabled)
                    continue;

                checkBoxProdutos[i].checked = check.checked;
            }
        }

        function qtdeChanged(control, qtdeTotal) {
            if (control == null)
                return false;

            var qtde = control.value;

            if (parseFloat(qtde.replace(",", ".")) > qtdeTotal) {
                alert("Qtde. maior que o disponível.");
                control.value = qtdeTotal;
                return false;
            }
        }

        function fecharVolume() {

            var idVolume = GetQueryString("idVolume");

            if (idVolume == null || idVolume == "") {
                alert("Nenhum item foi selecionado");
                return false;
            }

            var retorno = CadItensVolume.FecharVolume(idVolume).value.split(";");

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }

            openRptEtiqueta(idVolume);
            fecharPagina();

        }

        function openRptEtiqueta(idVolume) {
            if (idVolume == null || idVolume == "") {
                alert("Informe o volume.");
                return false;
            }

            openWindow(500, 700, "../Relatorios/RelEtiquetaVolume.aspx?rel=EtqVolume&idVolume=" + idVolume, null, true, true);
            return false;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td valign="top">
                            <b>Itens do Pedido</b>
                            <asp:GridView ID="grdProdutosPedido" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedido"
                                GridLines="None" OnDataBound="grdProdutosPedido_DataBound" Style="min-width: 350px;">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkSelProd" ToolTip="Selecionar todos as peças" runat="server"
                                                onclick="selecionaTodosProdutos(this)" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelProd" ToolTip='<%# "Selecionar a peça: " + Eval("CodInternoDescProd")  %>'
                                                runat="server" />
                                            <asp:HiddenField runat="server" ID="hdfIdProdPed" Value='<%# Eval("idProdPed") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Produto" DataField="CodInternoDescProd" />
                                    <asp:TemplateField HeaderText="Qtde.">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQtdePecas" runat="server" Text='<%# (float)Eval("Qtde") - (float)Eval("QtdeVolume")  %>'
                                                Width="30px" onchange='<%# "qtdeChanged(this, " + Eval("Qtde") + ");" %>' onkeypress='<%# Eval("TipoCalc", "return soNumeros(event, CalcProd_IsQtdeInteira({0}), true);") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pgr" />
                                <EditRowStyle CssClass="edit" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosPedido" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
                                EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetForGerarVolumeCount"
                                SelectMethod="GetForGerarVolume" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                                TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                                <SelectParameters>
                                    <asp:QueryStringParameter QueryStringField="idPedido" Name="idPedido" Type="UInt32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td style="padding: 5px;">
                            <asp:ImageButton ID="imbAddPeca" ToolTip="Adicionar peças ao volume" runat="server"
                                ImageUrl="~/Images/arrow_right.gif" OnClick="imbAddPeca_Click" OnClientClick="bloquearPagina(); desbloquearPagina(false);"/>
                            <br />
                            <asp:ImageButton ID="imbRemovePeca" ToolTip="Remover peças do volume" runat="server"
                                ImageUrl="~/Images/arrow_left.gif" OnClick="imbRemovePeca_Click" OnClientClick="bloquearPagina(); desbloquearPagina(false);"/>
                        </td>
                        <td valign="top">
                            <b>Itens do Volume</b>
                            <asp:GridView ID="grdVolumeProdutosPedido" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdVolume, IdProdPed"
                                DataSourceID="odsVolumeProdutosPedido" GridLines="None" OnDataBound="grdVolumeProdutosPedido_DataBound"
                                Style="min-width: 350px;">
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkSelProd" ToolTip="Selecionar todos as peças" runat="server"
                                                onclick="selecionaTodosProdutos(this)" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelProd" ToolTip='<%# "Selecionar a peça: " + Eval("CodInternoDescProd")  %>'
                                                runat="server" />
                                            <asp:HiddenField runat="server" ID="hdfIdProdPed" Value='<%# Eval("idProdPed") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Produto" DataField="CodInternoDescProd" />
                                    <asp:TemplateField HeaderText="Qtde.">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQtdePecas" runat="server" Text='<%# Eval("Qtde") %>' Width="30px"
                                                onchange='<%# "qtdeChanged(this, " + Eval("Qtde") + ");" %>' onkeypress='<%# Eval("TipoCalc", "return soNumeros(event, CalcProd_IsQtdeInteira({0}), true);") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pgr" />
                                <EditRowStyle CssClass="edit" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsVolumeProdutosPedido" runat="server" DataObjectTypeName="Glass.Data.Model.VolumeProdutosPedido"
                                EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetListCount"
                                SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                                TypeName="Glass.Data.DAL.VolumeProdutosPedidoDAO">
                                <SelectParameters>
                                    <asp:QueryStringParameter Name="idVolume" QueryStringField="idVolume" Type="UInt32" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="3">
                            <asp:Button ID="Button1" runat="server" Text="Fechar Volume" 
                                OnClientClick="return fecharVolume();" Width="150px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
