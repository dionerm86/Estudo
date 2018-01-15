<%@ Page Title="Retificar Comissão" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadRetificarComissao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetificarComissao" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function checkAll(checked)
        {
            var inputs = document.getElementsByTagName('input');

            for (var i = 0; i < inputs.length; i++)
            {
                if (inputs[i].type == "checkbox")
                    inputs[i].checked = !inputs[i].disabled && checked;
            }

            calculaComissao();
        }

        // -------------------------------------------------
        // Função responsável por calcular a comissão total.
        // -------------------------------------------------
        function calculaComissao()
        {
            if (document.getElementById("<%= btnRetificarComissao.ClientID %>") == null)
                return;

            var totalComissao = 0;
            var inputs = document.getElementsByTagName('input');
            var podeRetificar = false;

            for (var i = 0; i < inputs.length; i++)
            {
                if (inputs[i].type == "checkbox" && inputs[i].id.indexOf("chkAll") == -1)
                {
                    if (inputs[i].checked) {
                        var somar = inputs[i].parentNode.getAttribute("valorPagoComissao");
                        totalComissao += parseFloat(somar);
                    }
                    else
                        podeRetificar = true;
                }
            }

            var lblComissaoTotal = document.getElementById("<%= lblComissaoTotal.ClientID %>");
            lblComissaoTotal.innerHTML = totalComissao.toFixed(2).replace('.', ',');
            document.getElementById("<%= hdfValorComissao.ClientID %>").value = totalComissao.toFixed(2).replace('.', ',');

            if (document.getElementById("<%= btnRetificarComissao.ClientID %>") != null)
            {
                var valorTotal = document.getElementById("<%= drpIdComissao.ClientID %>");
                valorTotal = valorTotal.options[valorTotal.selectedIndex].text;
                valorTotal = valorTotal.substr(valorTotal.indexOf(" - R$ ") + 3);
                valorTotal = parseFloat(valorTotal.replace("R$", "").replace(" ", "").replace(".", "").replace(",", "."));

                document.getElementById("<%= btnRetificarComissao.ClientID %>").disabled = !podeRetificar;
            }
        }

        function showLoadGif()
        {
            var loading = document.getElementById("loading");
            loading.style.display = "inline";
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
                            <asp:Label ID="Label1" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True" OnChange="showLoadGif()"
                                OnSelectedIndexChanged="drpTipo_SelectedIndexChanged">
                                <asp:ListItem Value="0">Funcionário</asp:ListItem>
                                <asp:ListItem Value="1">Comissionado</asp:ListItem>
                                <asp:ListItem Value="2">Instalador</asp:ListItem>
                                <asp:ListItem Value="3">Gerente</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNome" runat="server" DataSourceID="odsFuncionario" OnChange="showLoadGif()"
                                DataTextField="Nome" DataValueField="IdFunc" AutoPostBack="True" OnDataBound="drpNome_DataBound"
                                OnSelectedIndexChanged="drpNome_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdfNome" runat="server" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Comissão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpIdComissao" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsComissoesFunc" DataTextField="DescrComissao" DataValueField="IdComissao"
                                OnSelectedIndexChanged="drpIdComissao_SelectedIndexChanged">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <tr>
                    <td align="center">
                        <br />
                        <div>
                            Selecione os pedidos que continuarão no pagamento da comissão
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
                                        <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                    </ItemTemplate>
                                    <ItemStyle Wrap="False" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                                <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                                <asp:BoundField DataField="DataConfString" HeaderText="Data Conf. Pedido" SortExpression="DataConf" />
                                <asp:BoundField DataField="DataFinalizacaoInst" DataFormatString="{0:d}" HeaderText="Data Final. Inst."
                                    SortExpression="DataFinalizacaoInst" />
                                <asp:BoundField DataField="Total" HeaderText="Valor Pedido" SortExpression="Total"
                                    DataFormatString="{0:C}" />
                                <asp:BoundField DataField="ValorPagoComissao" DataFormatString="{0:c}" HeaderText="Valor Pago Comissão"
                                    SortExpression="ValorPagoComissao" />
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
                                    <td>
                                        Data da conta a pagar
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
                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissoesFunc" runat="server" OnSelecting="odsComissoesFunc_Selecting"
                            SelectMethod="GetForRetificar" TypeName="Glass.Data.DAL.ComissaoDAO">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="drpTipo" Name="tipoFunc" PropertyName="SelectedValue"
                                    Type="UInt32" />
                                <asp:ControlParameter ControlID="drpNome" Name="idFuncComissionado" PropertyName="SelectedValue"
                                    Type="UInt32" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissao" runat="server" MaximumRowsParameterName=""
                            SelectMethod="GetPedidosByComissao" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.PedidoDAO">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="drpIdComissao" Name="idComissao" PropertyName="SelectedValue"
                                    Type="UInt32" />
                                <asp:ControlParameter ControlID="drpTipo" Name="tipoFunc" PropertyName="SelectedValue"
                                    Type="Object" />
                                <asp:ControlParameter ControlID="drpNome" Name="idFunc" PropertyName="SelectedValue"
                                    Type="UInt32" />
                            </SelectParameters>
                        </colo:VirtualObjectDataSource>
                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedoresByComissao"
                            TypeName="Glass.Data.DAL.FuncionarioDAO">
                        </colo:VirtualObjectDataSource>
                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsComissionado" runat="server" SelectMethod="GetComissionadosByComissao"
                            TypeName="Glass.Data.DAL.ComissionadoDAO">
                        </colo:VirtualObjectDataSource>
                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInstalador" runat="server" SelectMethod="GetColocadoresByComissao"
                            TypeName="Glass.Data.DAL.FuncionarioDAO">
                        </colo:VirtualObjectDataSource>
                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGerente" runat="server" SelectMethod="GetGerentesForComissao"
                            TypeName="Glass.Data.DAL.FuncionarioDAO">                           
                        </colo:VirtualObjectDataSource>
                    </td>
                </tr>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        calculaComissao();
        document.getElementById("loading").style.display = "none";
    </script>

</asp:Content>
