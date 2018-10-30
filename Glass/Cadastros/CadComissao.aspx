<%@ Page Title="Comissão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadComissao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadComissao" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        // Marca todos os pedidos ao clicar no checkbox do cabeçalho
        // da tabela
        function checkAll(checked) {
            var inputs = document.getElementsByTagName('input');

            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox")
                    inputs[i].checked = !inputs[i].disabled && checked;
            }

            calculaComissao();
        }

        // -------------------------------------------------
        // Função responsável por calcular a comissão total.
        // -------------------------------------------------
        function calculaComissao() {
            if (document.getElementById("<%= btnGerarComissao.ClientID %>") == null)
                return;

            // Recupera alguns dados da tela
            var tipoFunc = FindControl("drpTipo", "select").value;
            var inputs = document.getElementsByTagName('input');
            var numColunas = document.getElementById("<%= grdComissao.ClientID %>").rows[0].cells.length;
            var dataIni = FindControl('ctrlDataIni_txtData', 'input').value;
            var dataFim = FindControl('ctrlDataFim_txtData', 'input').value;

            // Se for funcionário ou instalador, sempre será necessário calcular por Ajax, principalmente porque se estiver usando mais de uma faixa
            // de valor, não tem como calcular por javascript somente.
            var calcularPorAjax = tipoFunc != 1;
            var totalComissao = !calcularPorAjax ? 0 : "";
            var totalPedidos = 0;
            var totalBaseCalcComissao = 0;

            // Salva os dados dos pedidos selecionados:
            // Caso o cálculo seja por Ajax, salva o ID do pedido para o método,
            // senão soma o valor da comissão já calculada
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox" && inputs[i].id.indexOf("chkAll") == -1 && inputs[i].checked) {
                    var somar = !calcularPorAjax ? inputs[i].parentNode.getAttribute("valorComissao") : inputs[i].parentNode.getAttribute("idPedido");
                    totalComissao += !calcularPorAjax ? parseFloat(somar) : "," + somar;
                    totalPedidos += parseFloat(inputs[i].parentNode.getAttribute("ValorPedido"));
                    totalBaseCalcComissao += parseFloat(inputs[i].parentNode.getAttribute("ValorBaseCalcComissao"));
                }
            }

            // Salva o total dos pedidos
            var lblValorPedidos = document.getElementById("<%= lblValorPedidos.ClientID %>");
            lblValorPedidos.innerHTML = totalPedidos.toFixed(2).replace('.', ',');
            var lblValorBaseCalcComissao = document.getElementById("<%= lblValorBaseCalcComissao.ClientID %>");
            lblValorBaseCalcComissao.innerHTML = totalBaseCalcComissao.toFixed(2).replace('.', ',');

            // Invoca a função de cálculo por Ajax, se for necessário
            if (calcularPorAjax) {
                if (totalComissao != "")
                    totalComissao = parseFloat(CadComissao.CalculaComissao(tipoFunc, document.getElementById("<%= drpNome.ClientID %>").value,
                        dataIni, dataFim, totalComissao.substr(1)).value);
                else
                    totalComissao = 0;
            }

            // Mostra o valor da comissão no label na tela
            var lblComissaoTotal = document.getElementById("<%= lblComissaoTotal.ClientID %>");
            lblComissaoTotal.innerHTML = totalComissao.toFixed(2).replace('.', ',');

            // Desconta os débitos da comissão, se existirem
            var lblDebitos = document.getElementById("<%= lblDebitos.ClientID %>");
            if (lblDebitos != null) {
                var valorDebitos = parseFloat(lblDebitos.innerHTML.replace(",", "."));
                totalComissao -= valorDebitos;
                var lblValorPagar = document.getElementById("<%= lblValorPagar.ClientID %>");
                lblValorPagar.innerHTML = totalComissao.toFixed(2).replace(".", ",");
            }

            // Salva o valor da comissão no HiddenField
            document.getElementById("<%= hdfValorComissao.ClientID %>").value = totalComissao.toFixed(2).replace('.', ',');

            // Habilita o botão para geração da comissão se o valor a pagar for maior que 0
            if (document.getElementById("<%= btnGerarComissao.ClientID %>") != null)
                document.getElementById("<%= btnGerarComissao.ClientID %>").disabled = totalComissao <= 0;
        }

        function showLoadGif() {
            bloquearPagina();
            desbloquearPagina(false);
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
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
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Período" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadOnly" ExibirHoras="False" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadOnly" ExibirHoras="False" />
                        </td>
                        <td nowrap="nowrap" align="right">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="showLoadGif()"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpNome" runat="server" DataSourceID="odsFuncionario" OnChange="showLoadGif()"
                                DataTextField="Nome" DataValueField="IdFunc" AutoPostBack="True" OnDataBound="drpNome_DataBound"
                                AppendDataBoundItems="true" OnSelectedIndexChanged="drpNome_SelectedIndexChanged" EnableViewState="true">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                            <asp:HiddenField ID="hdfNome" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="true" MostrarTodas="true" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Com Recebimento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlComRecebimento" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="true">Sim</asp:ListItem>
                                <asp:ListItem Value="false">Não</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                         <td>
                            <asp:Label ID="lblTipoVenda" runat="server" ForeColor="#0066FF" Text="Tipo Venda"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblTipoVenda" runat="server" Width="110px" CheckAll="False" Title="Selecione o tipo">
                                <asp:ListItem Value="1">À Vista</asp:ListItem>
                                <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                <asp:ListItem Value="5">Obra</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdComissao" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsComissao" DataKeyNames="IdPedido" EmptyDataText="Não há comissões para o filtro especificado."
                    OnDataBound="grdComissao_DataBound" PageSize="20" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkAll" runat="server" onclick="checkAll(this.checked);" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSel" runat="server" onclick="calculaComissao()" OnDataBinding="chkSel_DataBinding" />
                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="nfeAssociada" HeaderText="NF-e" SortExpression="nfeAssociada" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="DataConfString" HeaderText="Data Conf. Pedido" SortExpression="DataConf" />
                        <asp:BoundField DataField="DataLiberacao" DataFormatString="{0:d}" HeaderText="Data Liberação"
                            SortExpression="DataLiberacao" />
                        <asp:BoundField DataField="DataFinalizacaoInst" DataFormatString="{0:d}" HeaderText="Data Final. Inst."
                            SortExpression="DataFinalizacaoInst" />
                        <asp:BoundField DataField="TotalReal" HeaderText="Valor Pedido" SortExpression="Total"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="TextoDescontoPerc" HeaderText="Desconto" SortExpression="TextoDescontoPerc" />
                        <asp:BoundField DataField="ValorBaseCalcComissao" DataFormatString="{0:c}" HeaderText="Base Cálc. Comissão"
                            SortExpression="ValorBaseCalcComissao" />
                        <asp:BoundField DataField="ValorComissaoRecebida" DataFormatString="{0:C}" HeaderText="Comissão já paga"
                            SortExpression="ValorComissaoRecebida" />
                        <asp:BoundField  DataField="ValorComissaoPagar" DataFormatString="{0:C}" HeaderText="Valor Comissão"
                            SortExpression="ValorComissaoPagar" />
                        <asp:BoundField DataField="TemRecebimentoString" HeaderText="Tem Recebimento" SortExpression="TemRecebimentoString" />
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <HeaderStyle Wrap="False" />
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="left">
                <asp:Label ID="lblTotalParaComissao" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <span style="font-size: 130%">Valor dos Pedidos: R$
                                <asp:Label ID="lblValorPedidos" runat="server" Text="0,00"></asp:Label>
                            </span>
                        </td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>
                            <span style="font-size: 130%">Valor Base Cálc. Comissão: R$
                                <asp:Label ID="lblValorBaseCalcComissao" runat="server" Text="0,00"></asp:Label>
                            </span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <span style="font-size: 130%">Valor da comissão: R$
                    <asp:Label ID="lblComissaoTotal" runat="server" Text="0,00"></asp:Label>
                    <br />
                    <asp:Panel ID="panDebitos" runat="server">
                        <span style="color: Red">Valor dos débitos: R$
                            <asp:Label ID="lblDebitos" runat="server" Text="0,00"></asp:Label>
                        </span>
                        <br />
                        <span>Valor a pagar: R$
                            <asp:Label ID="lblValorPagar" runat="server" Text="0,00"></asp:Label>
                        </span>
                    </asp:Panel>
                </span>
            </td>
        </tr>
        <tr>
            <td align="center">
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
                    <asp:Button ID="btnGerarComissao" runat="server" OnClick="btnGerarComissao_Click"
                        Text="Gerar Comissão" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfValorComissao" runat="server" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsComissao" runat="server" MaximumRowsParameterName=""
                    SelectMethod="GetPedidosForComissao" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpTipo" Name="tipoFunc" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpNome" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" PropertyName="SelectedValue" Name="idLoja"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="ddlComRecebimento" PropertyName="SelectedValue"
                            Name="comRecebimento" Type="Boolean" />
                        <asp:Parameter Name="isRelatorio" DefaultValue="false" Type="Boolean" />
                           <asp:ControlParameter ControlID="cblTipoVenda" Name="tiposVenda" PropertyName="SelectedValue"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedoresForComissao" TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsComissionado" runat="server"
                    SelectMethod="GetComissionadosForComissao" TypeName="Glass.Data.DAL.ComissionadoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsInstalador" runat="server" SelectMethod="GetColocadoresForComissao"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGerente" runat="server" SelectMethod="GetGerentesForComissao"
                            TypeName="Glass.Data.DAL.FuncionarioDAO">                           
                        </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        calculaComissao();
    </script>

</asp:Content>
