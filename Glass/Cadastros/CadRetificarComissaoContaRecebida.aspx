<%@ Page Title="Retificar Comissão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadRetificarComissaoContaRecebida.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetificarComissaoContaRecebida" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function checkAll(checked) {
            var inputs = document.getElementsByTagName('input');

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox")
                    inputs[i].checked = !inputs[i].disabled && checked;
            }

            calculaComissao();
        }

        function calculaComissao() {
            if (document.getElementById("<%= btnRetificarComissao.ClientID %>") == null)
                return;

            var totalComissao = 0;
            var inputs = document.getElementsByTagName('input');

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox" && inputs[i].id.indexOf("chkAll") == -1 && inputs[i].checked) {
                    var somar = inputs[i].parentNode.getAttribute("ValorComissao");
                    totalComissao += parseFloat(somar);
                }
            }

            var lblComissaoTotal = document.getElementById("<%= lblComissaoTotal.ClientID %>");
            lblComissaoTotal.innerHTML = totalComissao.toFixed(2).replace('.', ',');
            document.getElementById("<%= hdfValorComissao.ClientID %>").value = totalComissao.toFixed(2).replace('.', ',');

            if (document.getElementById("<%= btnRetificarComissao.ClientID %>") != null) {
                var valorTotal = document.getElementById("<%= drpIdComissao.ClientID %>");
                valorTotal = valorTotal.options[valorTotal.selectedIndex].text;
                valorTotal = valorTotal.substr(valorTotal.indexOf(" - R$ ") + 3);
                valorTotal = parseFloat(valorTotal.replace("R$", "").replace(" ", "").replace(".", "").replace(",", "."));

                document.getElementById("<%= btnRetificarComissao.ClientID %>").disabled = totalComissao == 0 || totalComissao == valorTotal;
            }
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <div>
                    Apenas comissões que ainda não foram pagas podem ser retificadas.
                </div>
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNome" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Comissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpIdComissao" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsComissoesFunc" DataTextField="DescrComissao" DataValueField="IdComissao">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <tr>
                    <td align="center">
                        <br />
                        <div>
                            Selecione as contas recebidas que continuarão no pagamento da comissão
                            <img src="../Images/load.gif" id="loading" style="display: none; padding-bottom: 3px" />
                        </div>
                        <br />
                        <asp:GridView GridLines="None" ID="grdComissao" runat="server" AutoGenerateColumns="False"
                            DataSourceID="odsComissao" DataKeyNames="IdPedido" EmptyDataText="Não há comissões para o filtro especificado."
                            OnDataBound="grdComissao_DataBound" PageSize="20" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                            AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                            <PagerSettings PageButtonCount="20" />
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkAll" runat="server" onclick="checkAll(this.checked);" Checked="True" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSel" runat="server" onclick="calculaComissao()" OnDataBinding="chkSel_DataBinding"
                                            Checked="True" />
                                        <asp:HiddenField ID="hdfIdContaR" runat="server" Value='<%# Eval("IdContaR") %>' />
                                    </ItemTemplate>
                                    <ItemStyle Wrap="False" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Referencia" HeaderText="Referência" SortExpression="Referencia" />
                                <asp:BoundField DataField="NumParcString" HeaderText="Parc." SortExpression="NumParcString" />
                                <asp:BoundField DataField="IdNomeCli" HeaderText="Cliente" SortExpression="IdNomeCli" />

                                <asp:BoundField DataField="ValorVec" DataFormatString="{0:C}" HeaderText="Valor"
                                    SortExpression="ValorVec" />
                                <asp:BoundField DataField="DataVec" DataFormatString="{0:d}" HeaderText="Data Venc."
                                    SortExpression="DataVec" />
                                <asp:BoundField DataField="ValorRec" DataFormatString="{0:C}" HeaderText="Valor Rec."
                                    SortExpression="ValorRec" />
                                <asp:BoundField DataField="DataRec" DataFormatString="{0:d}" HeaderText="Data Rec."
                                    SortExpression="DataRec" />
                                <asp:BoundField DataField="DescricaoContaContabil" HeaderText="Tipo" SortExpression="DescricaoContaContabil" />
                                <asp:BoundField DataField="ValorImpostos" DataFormatString="{0:C}" HeaderText="Valor Imp."
                                    SortExpression="ValorImpostos" />
                                <asp:BoundField DataField="ValorBaseCalcComissao" DataFormatString="{0:C}" HeaderText="Base de Calc."
                                    SortExpression="ValorBaseCalcComissao" />
                                <asp:BoundField DataField="ValorComissao" DataFormatString="{0:C}" HeaderText="Valor da Comissão"
                                    SortExpression="ValorComissao" />
                            </Columns>
                            <PagerStyle CssClass="pgr"></PagerStyle>
                            <HeaderStyle Wrap="False" />
                            <EditRowStyle CssClass="edit"></EditRowStyle>
                            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                        </asp:GridView>
                        <br />
                        <span style="font-size: 130%">Valor da comissão: R$
                            <asp:Label ID="lblComissaoTotal" runat="server">0,00</asp:Label>
                        </span>
                        <asp:HiddenField ID="hdfValorComissao" runat="server" />
                        <div id="gerarComissao" runat="server">
                            <br />
                            <br />
                            <table>
                                <tr>
                                    <td>Data da conta a pagar
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataComissao" runat="server" ReadOnly="ReadWrite" ExibirHoras="False"
                                            ValidateEmptyText="true" />
                                    </td>
                                </tr>
                            </table>
                            <asp:Button ID="btnRetificarComissao" runat="server" OnClick="btnRetificarComissao_Click"
                                OnClientClick="if (!confirm('Deseja retificar essa comissão?')) return false"
                                Text="Retificar Comissão" />
                        </div>
                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsComissoesFunc" runat="server" OnSelecting="odsComissoesFunc_Selecting"
                            SelectMethod="GetForRetificarContasRecebidas" TypeName="Glass.Data.DAL.ComissaoDAO">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="drpNome" Name="idFunc" PropertyName="SelectedValue"
                                    Type="UInt32" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsComissao" runat="server" MaximumRowsParameterName=""
                            SelectMethod="GetContasRecebidasByComissao" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.ContasReceberDAO">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="drpIdComissao" Name="idComissao" PropertyName="SelectedValue"
                                    Type="UInt32" />
                                <asp:ControlParameter ControlID="drpNome" Name="idFunc" PropertyName="SelectedValue"
                                    Type="UInt32" />
                                <asp:Parameter Name="paraRelatorio" Type="Boolean" DefaultValue="false" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                            SelectMethod="GetVendedorForComissaoContasReceber" TypeName="Glass.Data.DAL.FuncionarioDAO">
                        </colo:VirtualObjectDataSource>
                    </td>
                </tr>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        calculaComissao();
    </script>

</asp:Content>
