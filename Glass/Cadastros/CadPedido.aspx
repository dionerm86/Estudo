<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadPedido.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadPedido" Title="Cadastrar Pedido" EnableEventValidation="false" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlParcelasSelecionar.ascx" TagName="ctrlParcelasSelecionar" TagPrefix="uc6" %>
<%@ Register Src="../Controls/ctrlDadosDesconto.ascx" TagName="ctrlDadosDesconto" TagPrefix="uc7" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc8" %>
<%@ Register Src="../Controls/ctrlConsultaCadCliSintegra.ascx" TagName="ctrlConsultaCadCliSintegra" TagPrefix="uc9" %>
<%@ Register Src="../Controls/ctrlLimiteTexto.ascx" TagName="ctrlLimiteTexto" TagPrefix="uc10" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc11" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc12" %>
<%@ Register Src="../Controls/ctrlProdComposicao.ascx" TagName="ctrlProdComposicao" TagPrefix="uc13" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        
    var config_UsarBenefTodosGrupos = <%= Glass.Configuracoes.Geral.UsarBeneficiamentosTodosOsGrupos.ToString().ToLower() %>;
    var config_FastDelivery = <%= Glass.Configuracoes.PedidoConfig.Pedido_FastDelivery.FastDelivery.ToString().ToLower() %>;
    var config_GerarPedidoProducaoCorte = <%= Glass.Configuracoes.PedidoConfig.GerarPedidoProducaoCorte.ToString().ToLower() %>;
    var config_BloquearDadosClientePedido = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.BloquearDadosClientePedido.ToString().ToLower() %>;
    var config_UsarControleDescontoFormaPagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;
    var config_ObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
    var config_UtilizarRoteiroProducao = <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
    var config_UsarDescontoEmParcela = <%= Glass.Configuracoes.FinanceiroConfig.UsarDescontoEmParcela.ToString().ToLower() %>;
    var config_NumeroDiasUteisDataEntregaPedido = <%= Glass.Configuracoes.PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido %>;
    var config_ExibirPopupFaltaEstoque = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower() %>;
    var config_LiberarPedido = <%= Glass.Configuracoes.PedidoConfig.LiberarPedido.ToString().ToLower() %>;
    var config_DescontoApenasAVista = <%= Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista.ToString().ToLower() %>;
    var config_NumeroDiasPedidoProntoAtrasado = <%= Glass.Configuracoes.PedidoConfig.NumeroDiasPedidoProntoAtrasado %>;
    var config_UsarControleObraComProduto = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.UsarControleNovoObra.ToString().ToLower() %>;
    var config_UsarComissaoPorPedido = <%= Glass.Configuracoes.PedidoConfig.Comissao.PerComissaoPedido.ToString().ToLower() %>;
    var config_UsarComissionado = <%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente.ToString().ToLower() %>;
    var config_BuscarEnderecoClienteSeEstiverVazio = <%= Glass.Configuracoes.PedidoConfig.TelaCadastro.BuscarEnderecoClienteSeEstiverVazio.ToString().ToLower() %>;
    var config_PermitirDescontoAVistaComUmaParcela = <%= (Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoUmaParcela && Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista).ToString().ToLower() %>;
    var config_BloqEmisPedidoPorPosicaoMateriaPrima = <%= (Glass.Configuracoes.PedidoConfig.BloqEmisPedidoPorPosicaoMateriaPrima != Glass.Data.Helper.DataSources.BloqEmisPedidoPorPosicaoMateriaPrima.Bloquear).ToString().ToLower() %>
    var config_AlterarLojaPedido = <%= Glass.Configuracoes.PedidoConfig.AlterarLojaPedido.ToString().ToLower() %>;
    var config_UsarAltLarg = <%= Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura.ToString().ToLower() %>;
        
    var var_IdPedido = '<%= Request["idPedido"] %>';
    var var_CodCartao = CadPedido.GetCartaoCod().value;
    var var_DataEntregaAntiga = "<%= GetDataEntrega() %>";
    var var_IgnorarBloqueioDataEntrega = "<%= IgnorarBloqueioDataEntrega() %>";
    var var_NomeControleParcelas = "<%= dtvPedido.ClientID %>_ctrlParcelas1";
    var var_PedidoMaoDeObra = '<%= Request["maoObra"] %>' == 1;
    var var_BloquearDataEntrega = <%= GetBloquearDataEntrega().ToString().ToLower() %>;
    var var_ValorDescontoTotalProdutos = <%= GetDescontoProdutos() %>;
    var var_ValorDescontoTotalPedido = <%= GetDescontoPedido() %>;
    var var_QtdProdutosPedido = <%= GetNumeroProdutosPedido() %>;
    var var_TipoEntregaBalcao = <%= GetTipoEntregaBalcao() %>;
    var var_TipoEntregaEntrega = <%= GetTipoEntrega() %>;
    var var_TotalM2Pedido = "<%= GetTotalM2Pedido() %>";
    var var_DataPedido = "<%= GetDataPedido() %>";
    var var_QtdEstoque = 0;
    var var_QtdEstoqueMensagem = 0;
    var var_ExibirMensagemEstoque = false;
    var var_Inserting = false;
    var var_ProdutoAmbiente = false;
    var var_AplAmbiente = false;
    var var_ProcAmbiente = false;
    var var_Loading = true;
    var var_SaveProdClicked = false;

    </script>

    <table id="mainTable" runat="server" clientidmode="Static" style="width: 100%">
        <tr>
            <td>
                <table style="width: 100%">
                    <tr>
                        <td align="center">
                            <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                                DefaultMode="Insert" GridLines="None" Height="50px" Width="125px">
                                <Fields>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cliente
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow">
                                                        <span style="white-space: nowrap">
                                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeydown="if (isEnter(event)) getCli(this);"
                                                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this.value);" ReadOnly='<%# !(bool)Eval("ClienteEnabled") %>'
                                                                Text='<%# Eval("IdCli") %>'></asp:TextBox>
                                                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Text='<%# Eval("NomeCli") %>'
                                                                Width="250px"></asp:TextBox>
                                                            <asp:LinkButton ID="lnkSelCliente" runat="server" Visible='<%# Eval("ClienteEnabled") %>'
                                                                OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>&nbsp;&nbsp;
                                                            <div id="ConsultaCadCliSintegra" style="display: none">
                                                                <uc9:ctrlConsultaCadCliSintegra runat="server" ID="ctrlConsultaCadCliSintegra1" OnLoad="ctrlConsultaCadCliSintegra1_Load" />
                                                            </div>
                                                            <asp:HiddenField ID="hdfPercentualComissao" runat="server" Value='<%# Bind("PercentualComissao") %>' />
                                                        </span>
                                                        <br />
                                                        <asp:Label ID="lblObsCliente" runat="server" ForeColor="<%# GetCorObsCliente() %>"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataPed" runat="server" ReadOnly="True" Text='<%# Eval("DataPedidoString") %>'
                                                            Width="70px"></asp:TextBox>
                                                        <asp:CheckBox ID="chkFastDelivery" runat="server" Checked='<%# Bind("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load" Text="Fast delivery" onclick="alteraFastDelivery(this.checked)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cód. Ped. Cli.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCodPedCli" runat="server" MaxLength="30" Text='<%# Bind("CodCliente") %>'
                                                            onchange="verificaPedCli();" ReadOnly='<%# Importado() %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Orcamento
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("IdOrcamento") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label14" runat="server" Text="Loja"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <uc11:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" MostrarTodas="false"
                                                                        SelectedValue='<%# Bind("IdLoja") %>' OnLoad="Loja_Load" OnChange="AlterouLoja();" />
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="chkDeveTransferir" runat="server" Text="Deve Transferir?" Checked='<%# Bind("DeveTransferir") %>'
                                                                        OnLoad="Loja_Load" ForeColor="Red" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        &nbsp;
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTipoVenda1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Venda
                                                    </td>
                                                    <td id="tdTipoVenda2" align="left" nowrap="nowrap" valign="middle">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoVenda" runat="server" SelectedValue='<%# Bind("TipoVenda") %>'
                                                                        onchange="tipoVendaChange(this, true);" onblur="verificarDescontoFormaPagtoDadosProduto();" Enabled='<%# !(bool)Eval("RecebeuSinal") || (bool)Glass.Configuracoes.PedidoConfig.LiberarPedido %>'
                                                                        DataSourceID="odsTipoVenda" DataTextField="Descr" DataValueField="Id">
                                                                    </asp:DropDownList>
                                                                    <div id="divObra" style="display: none">
                                                                        <asp:TextBox ID="txtObra" runat="server" Enabled="false" Width="200px" Text='<%# Eval("DescrObra") %>'></asp:TextBox>
                                                                        <asp:ImageButton ID="imbObra" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick='<%# "if (podeSelecionarObra()) openWindow(560, 650, \"../Utils/SelObra.aspx?situacao=4&tipo=1&idsPedidosIgnorar=" + Request["idPedido"] + "&idCliente=\" + FindControl(\"txtNumCli\", \"input\").value); return false;" %>' />
                                                                        <br />
                                                                        <asp:Label ID="lblSaldoObra" runat="server" Text='<%# Eval("DescrSaldoObraPedidos") %>'></asp:Label>
                                                                        <asp:HiddenField ID="hdfIdObra" runat="server" Value='<%# Bind("IdObra") %>' />
                                                                    </div>
                                                                </td>
                                                                <td>
                                                                    <div id="divNumParc">
                                                                        <table>
                                                                            <tr>
                                                                                <td nowrap="nowrap" style="font-weight: bold">
                                                                                    Num Parc.:
                                                                                </td>
                                                                                <td nowrap="nowrap">
                                                                                    <uc6:ctrlParcelasSelecionar ID="ctrlParcelasSelecionar1" runat="server" ParcelaPadrao='<%# Bind("IdParcela") %>'
                                                                                        NumeroParcelas='<%# Bind("NumParc") %>' OnLoad="ctrlParcelasSelecionar1_Load"
                                                                                        CallbackSelecaoParcelas="callbackSetParcelas"/>
                                                                                    <asp:HiddenField ID="hdfDataBase" runat="server" OnLoad="hdfDataBase_Load" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding="0" cellspacing="0" id="funcionarioComprador" style='<%# ((bool)Eval("VendidoFuncionario")) ? "": "display: none; " %>padding-top: 2px'>
                                                            <tr>
                                                                <td>
                                                                    Funcionário comp.:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFuncVenda" runat="server" DataSourceID="odsFuncVenda" DataTextField="Nome"
                                                                        DataValueField="IdFunc" AppendDataBoundItems="True" SelectedValue='<%# Bind("IdFuncVenda") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:HiddenField ID="hdfTipoVendaAtual" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                                    </td>
                                                    <td id="tdTipoEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Entrega
                                                    </td>
                                                    <td id="tdTipoEntrega2" align="left" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                <asp:DropDownList ID="ddlTipoEntrega" runat="server" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                                        onchange="setLocalObra(true);" AppendDataBoundItems="True" DataSourceID="odsTipoEntrega"
                                                                        DataTextField="Descr" DataValueField="Id" OnLoad="ddlTipoEntrega_Load" OnDataBound="ddlTipoEntrega_DataBound">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                
                                                                    
                                                                    &nbsp;
                                                                </td>
                                                                <td class="dtvHeader" nowrap="nowrap">
                                                                    Tipo Pedido
                                                                </td>
                                                                <td>
                                                                    <asp:HiddenField ID="hdfPedidoRevenda" runat="server" Value='<%# Bind("IdPedidoRevenda") %>' />
                                                                    <asp:DropDownList ID="drpTipoPedido" runat="server" onchange="alteraDataEntrega(false)" 
                                                                        DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" OnDataBound="drpTipoPedido_DataBound"
                                                                        SelectedValue='<%# Bind("TipoPedido") %>' Enabled='<%# Eval("TipoPedidoEnabled") %>'>
                                                                    </asp:DropDownList>
                                                                    <div id="divGerarPedidoProducaoCorte">
                                                                        <asp:CheckBox ID="chkGerarPedidoProducaoCorte" runat="server"
                                                                           Text="Gerar Pedido de Produção para Corte" Checked='<%# Bind("GerarPedidoProducaoCorte") %>'/>
                                                                    </>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdFormaPagto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Forma Pagto.
                                                    </td>
                                                    <td id="tdFormaPagto2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="true" DataSourceID="odsFormaPagto"
                                                                        DataTextField="Descricao" DataValueField="IdFormaPagto" SelectedValue='<%# Bind("IdFormaPagto") %>'
                                                                        onchange="formaPagtoChanged();" onblur="verificarDescontoFormaPagtoDadosProduto();">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                                        DataTextField="Descricao" DataValueField="IdTipoCartao" SelectedValue='<%# Bind("IdTipoCartao") %>' onblur="verificarDescontoFormaPagtoDadosProduto();">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdDataEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Entrega
                                                    </td>
                                                    <td id="tdDataEntrega2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                                         <table>
                                                            <tr>
                                                                <td>
                                                                    <uc8:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadOnly" DataString='<%# Bind("DataEntregaString") %>'
                                                                        ExibirHoras="False" onchange="verificaDataEntrega(this)" OnLoad="ctrlDataEntrega_Load" />
                                                                    <asp:HiddenField ID="hdfDataEntregaNormal" runat="server" />
                                                                    <asp:HiddenField ID="hdfDataEntregaFD" runat="server" />
                                                                </td>
                                                                <td align="left" class="dtvHeader" nowrap="nowrap">
                                                                    <asp:Label runat="server" ID="lblValorFrete" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label> 
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox runat="server" ID="txtValorFrete" onkeypress="return soNumeros(event, false, true);" Width="80px" Text='<%# Bind("ValorEntrega") %>'
                                                                        OnLoad="txtValorFrete_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdValorEntrada1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Valor Entrada
                                                    </td>
                                                    <td id="tdValorEntrada2" align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValEntrada" runat="server" Value='<%# Bind("ValorEntrada") %>'
                                                            Visible='<%# !(bool)Eval("RecebeuSinal") %>' />
                                                        <asp:Label ID="lblValor" runat="server" Text='<%# Eval("ConfirmouRecebeuSinal") %>'
                                                            Visible='<%# Eval("RecebeuSinal") %>'></asp:Label>
                                                        <asp:HiddenField ID="hdfValorEntrada" runat="server" Value='<%# Eval("ValorEntrada") %>'
                                                            Visible='<%# Eval("RecebeuSinal") %>' />
                                                    </td>
                                                    <td id="tdDesconto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Desconto
                                                    </td>
                                                    <td id="tdDesconto2" align="left" nowrap="nowrap" valign="middle">
                                                        <table class="pos" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                                        Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>' onclick="calcularDesconto(1)">
                                                                        <asp:ListItem Value="2">R$</asp:ListItem>
                                                                        <asp:ListItem Value="1">%</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Enabled='<%# Eval("DescontoEnabled") %>' onchange="calcularDesconto(1)" Text='<%# Bind("Desconto") %>'
                                                                        Width="70px"></asp:TextBox>
                                                                    <asp:Label ID="lblDescontoVista" runat="server" ForeColor="Blue" Text="Desconto só pode ser dado em pedidos à vista"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;
                                                                    <uc7:ctrlDadosDesconto ID="ctrlDadosDesconto" runat="server" TaxaFastDelivery='<%# Glass.Configuracoes.PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery %>'
                                                                        OnLoad="ctrlDadosDesconto_Load" IsPedidoFastDelivery='<%# Eval("FastDelivery") %>' />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:HiddenField ID="hdfDesconto" runat="server" Value='<%# Eval("Desconto") %>' />
                                                        <asp:HiddenField ID="hdfTipoDesconto" runat="server" Value='<%# Eval("TipoDesconto") %>' />
                                                        <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTotal1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Total
                                                    </td>
                                                    <td id="tdTotal2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtTotal" runat="server" ReadOnly="True" Text='<%# Eval("Total", "{0:C}") %>'></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    &nbsp;Acréscimo&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoAcrescimo" runat="server" SelectedValue='<%# Bind("TipoAcrescimo") %>'
                                                                        Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>'>
                                                                        <asp:ListItem Value="2">R$</asp:ListItem>
                                                                        <asp:ListItem Value="1">%</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:TextBox ID="txtAcrescimo" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("Acrescimo") %>' Width="70px" Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>'></asp:TextBox>
                                                                    <asp:HiddenField ID="hdfAcrescimo" runat="server" Value='<%# Eval("Acrescimo") %>' />
                                                                    <asp:HiddenField ID="hdfTipoAcrescimo" runat="server" Value='<%# Eval("TipoAcrescimo") %>' />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdFuncionario1" align="left" nowrap="nowrap" class="dtvHeader">
                                                        Funcionário
                                                    </td>
                                                    <td id="tdFuncionario2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                                        <asp:DropDownList ID="drpVendedorEdit" runat="server" DataSourceID="odsFuncionario" AppendDataBoundItems="true"
                                                            DataTextField="Nome" DataValueField="IdFunc" Enabled='<%# Eval("SelVendEnabled") %>'
                                                            SelectedValue='<%# Bind("IdFunc") %>' OnDataBinding="drpVendedorEdit_DataBinding">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdParcela" align="left" class="dtvHeader" colspan="4" nowrap="nowrap">
                                                        <uc3:ctrlParcelas ID="ctrlParcelas1" runat="server" NumParcelas="4" NumParcelasLinha="6"
                                                            Datas='<%# Bind("DatasParcelas") %>' Valores='<%# Bind("ValoresParcelas") %>'
                                                            OnLoad="ctrlParcelas1_Load" OnDataBinding="ctrlParcelas1_DataBinding" />
                                                        <asp:HiddenField ID="hdfExibirParcela" runat="server" />
                                                        <asp:HiddenField ID="hdfCalcularParcela" runat="server" />
                                                        <asp:HiddenField ID="hdfCliPagaAntecipado" runat="server" Value='<%# Eval("ClientePagaAntecipado") %>' />
                                                        <asp:HiddenField ID="hdfPercSinalMin" runat="server" Value='<%# Eval("PercSinalMinCliente") %>' />
                                                        <asp:HiddenField ID="hdfIdSinal" runat="server" Value='<%# Bind("IdSinal") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td>
                                                        Transportador
                                                        <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                                            SelectedValue='<%# Bind("IdTransportador") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    Local da Obra&nbsp;
                                                                </td>
                                                                <td>
                                                                    <a href="javascript:getEnderecoCli();">
                                                                        <img src="../Images/home.gif" title="Buscar endereço do cliente" border="0"></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        Endereço
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="100" disabled="true" onkeydown="if (isEnter(event)) return false;"
                                                            Text='<%# Bind("EnderecoObra") %>' Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Bairro
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("BairroObra") %>' onkeydown="if (isEnter(event)) return false;"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Cidade
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("CidadeObra") %>' onkeydown="if (isEnter(event)) return false;"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        CEP
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCepObra" runat="server" MaxLength="9" Text='<%# Bind("CepObra") %>' onkeypress="return soCep(event)"
                                                            onkeyup="return maskCep(event, this);"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="iniciaPesquisaCepObra(FindControl('txtCepObra', 'input').value); return false" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbComissionado" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.ComissaoPedido ? "" : "display: none" %>' class="dtvHeader" cellpadding="0"
                                                cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;Comissionado:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblComissionado" runat="server" Text='<%# Eval("NomeComissionado") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                    <asp:ImageButton ID="imbLimparComissionado" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                                        OnClientClick="limparComissionado(); return false;" ToolTip="Limpar comissionado"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td>
                                                                    Percentual:
                                                                </td>
                                                                <td>
                                                                    &nbsp;
                                                                    <asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("PercComissao") %>' Width="50px" OnLoad="txtPercentual_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td>
                                                                    Valor Comissão:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbMedidor" style='<%= Glass.Configuracoes.Geral.ControleMedicao ? "" : "display: none" %>' class="dtvHeader" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;Medidor:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblMedidor" runat="server" Text='<%# Eval("NomeMedidor") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelMedidor" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelMedidor.aspx'); return false;">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr>
                                                    <td class="dtvHeader" align="center" colspan="2">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="txtObs" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc10:ctrlLimiteTexto ID="lmtTxtObs" runat="server" IdControlToValidate="txtObs" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="dtvHeader" align="center" colspan="2" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                        Observação Liberação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                        <asp:TextBox ID="txtObsLiberacao" runat="server" MaxLength="1000" Text='<%# Bind("ObsLiberacao") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfCliente" runat="server" Value='<%# Bind("IdCli") %>' />
                                                        <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                                        <asp:HiddenField ID="hdfIdMedidor" runat="server" Value='<%# Bind("IdMedidor") %>' />
                                                        <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                                        <asp:HiddenField ID="hdfValorComissao" runat="server" Value='<%# Bind("ValorComissao") %>' />
                                                        <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                                                        <asp:HiddenField ID="hdfDataPedido" runat="server" Value='<%# Bind("DataPedidoString") %>' />
                                                        <asp:HiddenField ID="hdfAliquotaIcms" runat="server" Value='<%# Bind("AliquotaIcms") %>' />
                                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsNumParc" runat="server" SelectMethod="GetNumParc"
                                                            TypeName="Glass.Data.Helper.DataSources">
                                                        </colo:VirtualObjectDataSource>
                                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                                                            TypeName="Glass.Data.Helper.DataSources">
                                                        </colo:VirtualObjectDataSource>
                                                    </td>
                                                    <td style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                        <uc10:ctrlLimiteTexto ID="lmtTxtObsLiberacao" runat="server" IdControlToValidate="txtObsLiberacao" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cliente
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow">
                                                        <span style="white-space: nowrap">
                                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                                onkeydown="if (isEnter(event)) getCli(this.value);" onblur="getCli(this.value);" />
                                                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Text='<%# Eval("NomeCliente") %>'
                                                                Width="250px"></asp:TextBox>
                                                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                                            <img alt="" border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                            <div id="ConsultaCadCliSintegra" style="display: none">
                                                                <uc9:ctrlConsultaCadCliSintegra runat="server" ID="ctrlConsultaCadCliSintegra1" OnLoad="ctrlConsultaCadCliSintegra1_Load" />
                                                            </div>
                                                        </span>
                                                        <br />
                                                        <asp:Label ID="lblObsCliente" runat="server" Text="" ForeColor="<%# GetCorObsCliente() %>"></asp:Label>
                                                        <asp:HiddenField ID="hdfPercentualComissao" runat="server" Value='<%# Bind("PercentualComissao") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataPed" runat="server" ReadOnly="True" Width="70px" Text='<%# Eval("DataPedidoString") %>'></asp:TextBox>
                                                        <asp:CheckBox ID="chkFastDelivery" runat="server" Checked='<%# Bind("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load" Text="Fast delivery" onclick="alteraFastDelivery(this.checked)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cód. Ped. Cli.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCodPedCli" runat="server" MaxLength="20" Text='<%# Bind("CodCliente") %>'
                                                            ReadOnly='<%# Importado() %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Orcamento
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("IdOrcamento") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label14" runat="server" Text="Loja"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <uc11:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" OnLoad="Loja_Load"
                                                                        MostrarTodas="false" SelectedValue='<%# Bind("IdLoja") %>' OnChange="AlterouLoja();"/>
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="chkDeveTransferir" runat="server" Text="Deve Transferir?" Checked='<%# Bind("DeveTransferir") %>'
                                                                        OnLoad="Loja_Load" ForeColor="Red" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTipoVenda1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Venda
                                                    </td>
                                                    <td id="tdTipoVenda2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoVenda" runat="server" onchange="tipoVendaChange(this, true);" onblur="verificarDescontoFormaPagtoDadosProduto();"
                                                                        SelectedValue='<%# Bind("TipoVenda") %>' DataSourceID="odsTipoVenda" DataTextField="Descr"
                                                                        DataValueField="Id">
                                                                    </asp:DropDownList>
                                                                    <div id="divObra" style="display: none">
                                                                        <asp:TextBox ID="txtObra" runat="server" Enabled="false" Width="200px" Text='<%# Eval("DescrObra") %>'></asp:TextBox>
                                                                        <asp:ImageButton ID="imbObra" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick='<%# "if (podeSelecionarObra()) openWindow(560, 650, \"../Utils/SelObra.aspx?Situacao=4&idsPedidosIgnorar=" + Request["idPedido"] + "&idCliente=\" + FindControl(\"txtNumCli\", \"input\").value); return false;" %>' />
                                                                        <br />
                                                                        <asp:Label ID="lblSaldoObra" runat="server" Text='<%# Eval("SaldoObra", "{0:C}") %>'></asp:Label>
                                                                        <asp:HiddenField ID="hdfIdObra" runat="server" Value='<%# Bind("IdObra") %>' />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding="0" cellspacing="0" id="funcionarioComprador" style='display: none;
                                                            padding-top: 2px'>
                                                            <tr>
                                                                <td>
                                                                    Funcionário comp.:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFuncVenda" runat="server" DataSourceID="odsFuncVenda" DataTextField="Nome"
                                                                        DataValueField="IdFunc" AppendDataBoundItems="True" SelectedValue='<%# Bind("IdFuncVenda") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdTipoEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Entrega
                                                    </td>
                                                    <td id="tdTipoEntrega2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="ddlTipoEntrega" runat="server" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                                        onchange="setLocalObra(true);" AppendDataBoundItems="True" DataSourceID="odsTipoEntrega"
                                                                        DataTextField="Descr" DataValueField="Id" OnLoad="ddlTipoEntrega_Load" OnDataBound="ddlTipoEntrega_DataBound">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    &nbsp;
                                                                </td>
                                                                <td class="dtvHeader" nowrap="nowrap">
                                                                    Tipo Pedido
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoPedido" runat="server" onchange="alteraDataEntrega(false)"
                                                                        DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" OnDataBound="drpTipoPedido_DataBound"
                                                                        SelectedValue='<%# Bind("TipoPedido") %>' AppendDataBoundItems="True">
                                                                    </asp:DropDownList>
                                                                    <div id="divGerarPedidoProducaoCorte">
                                                                        <asp:CheckBox ID="chkGerarPedidoProducaoCorte" runat="server" 
                                                                            Text="Gerar Pedido de Produção para Corte" Checked='<%# Bind("GerarPedidoProducaoCorte") %>' />
                                                                    </div>
                                                                    <asp:HiddenField ID="hdfPedidoRevenda" runat="server" Value='<%# Bind("IdPedidoRevenda") %>' />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdFormaPagto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Forma Pagto.
                                                    </td>
                                                    <td id="tdFormaPagto2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="True" DataSourceID="odsFormaPagto"
                                                                        DataTextField="Descricao" onchange="formaPagtoChanged();" onblur="verificarDescontoFormaPagtoDadosProduto();" DataValueField="IdFormaPagto"
                                                                        SelectedValue='<%# Bind("IdFormaPagto") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                                        DataTextField="Descricao" DataValueField="IdTipoCartao" SelectedValue='<%# Bind("IdTipoCartao") %>' onblur="verificarDescontoFormaPagtoDadosProduto();">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdDataEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Entrega
                                                    </td>
                                                    <td id="tdDataEntrega2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <uc8:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadOnly" DataString='<%# Bind("DataEntregaString") %>'
                                                                        ExibirHoras="False" onchange="verificaDataEntrega(this)" OnLoad="ctrlDataEntrega_Load" />
                                                                    <asp:HiddenField ID="hdfDataEntregaNormal" runat="server" />
                                                                    <asp:HiddenField ID="hdfDataEntregaFD" runat="server" />
                                                                </td>
                                                                <td align="left" class="dtvHeader" nowrap="nowrap">
                                                                    <asp:Label runat="server" ID="lblValorFrete" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label> 
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox runat="server" ID="txtValorFrete" onkeypress="return soNumeros(event, false, true);" Width="80px" Text='<%# Bind("ValorEntrega") %>'
                                                                        OnLoad="txtValorFrete_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdValorEntrada1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Valor Entrada
                                                    </td>
                                                    <td id="tdValorEntrada2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValEntradaIns" runat="server" Value='<%# Bind("ValorEntrada") %>' />
                                                    </td>
                                                    <td id="tdFuncionario1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Funcionário
                                                    </td>
                                                    <td id="tdFuncionario2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpVendedorIns" runat="server" DataSourceID="odsFuncionario"
                                                            DataTextField="Nome" DataValueField="IdFunc" Enabled="<%# Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuPedido.AlterarVendedorPedido) %>"
                                                            SelectedValue='<%# Bind("IdFunc") %>' onchange="alteraDataPedidoFunc(this)">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td>
                                                        Transportador
                                                        <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                                            SelectedValue='<%# Bind("IdTransportador") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    Local da Obra&nbsp;
                                                                </td>
                                                                <td>
                                                                    <a href="javascript:getEnderecoCli();">
                                                                        <img src="../Images/home.gif" title="Buscar endereço do cliente" border="0"></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        Endereço
                                                    </td>
                                                    <td nowrap="nowrap">
                                                        <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="100" disabled="true"
                                                            Text='<%# Bind("EnderecoObra") %>' Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Bairro
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("BairroObra") %>'
                                                            Width="130px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Cidade
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("CidadeObra") %>'
                                                            Width="130px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Cep
                                                    </td>
                                                    <td>
                                                        <td align="left" nowrap="nowrap">
                                                            <asp:TextBox ID="txtCepObra" runat="server" MaxLength="9" Text='<%# Bind("CepObra") %>' onkeypress="return soCep(event)"
                                                                onkeyup="return maskCep(event, this);"></asp:TextBox>
                                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                                OnClientClick="iniciaPesquisaCepObra(FindControl('txtCepObra', 'input').value); return false" />
                                                        </td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbComissionado" cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.ComissaoPedido ? "" : "display: none" %>'>
                                                <tr class="dtvHeader">
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Comissionado:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblComissionado" runat="server"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                    <asp:ImageButton ID="imbLimparComissionado" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                                        OnClientClick="limparComissionado(); return false;" ToolTip="Limpar comissionado"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Percentual:
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("PercComissao") %>' Width="50px" OnLoad="txtPercentual_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Valor Comissão:
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbMedidor0" cellpadding="0" cellspacing="0" class="dtvHeader" style='<%= Glass.Configuracoes.Geral.ControleMedicao ? "" : "display: none" %>'>
                                                <tr>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;Medidor:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblMedidor" runat="server" Text='<%# Eval("NomeMedidor") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelMedidor" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelMedidor.aspx'); return false;">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr>
                                                    <td class="dtvHeader" align="center" colspan="2">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="txtObs" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                        <td>
                                                            <uc10:ctrlLimiteTexto ID="lmtTxtObs" runat="server" IdControlToValidate="txtObs" />
                                                        </td>
                                                        <tr>
                                                            <td class="dtvHeader" align="center" colspan="2" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                                Observação Liberação
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                                <asp:TextBox ID="txtObsLiberacao" runat="server" MaxLength="1000" Text='<%# Bind("ObsLiberacao") %>'
                                                                    TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                                <asp:HiddenField ID="hdfCliente" runat="server" Value='<%# Bind("IdCli") %>' />
                                                                <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                                                <asp:HiddenField ID="hdfIdMedidor" runat="server" Value='<%# Bind("IdMedidor") %>' />
                                                                <asp:HiddenField ID="hdfAliquotaIcms" runat="server" Value='<%# Eval("AliquotaIcms") %>' />
                                                                <asp:HiddenField ID="hdfDataPedido" runat="server" Value='<%# Bind("DataPedidoString") %>' />
                                                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                                                                    TypeName="Glass.Data.Helper.DataSources">
                                                                </colo:VirtualObjectDataSource>
                                                            </td>
                                                            <td style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                                <uc10:ctrlLimiteTexto ID="lmtTxtObsLiberacao" runat="server" IdControlToValidate="txtObsLiberacao" />
                                                            </td>
                                                        </tr>
                                            </table>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Num. Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNumPedido" runat="server" Text='<%# Eval("IdPedido") %>' Font-Size="Medium"></asp:Label>
                                                        <asp:Label ID="lblDescrTipoPedido" runat="server" Text='<%# "(" + Eval("DescricaoTipoPedido") + ")" %>'
                                                            ForeColor="Green"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Funcionário
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tel. Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTelCliente" runat="server" Text='<%# Eval("RptTelContCli") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Loja
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Endereço Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblEndereco" runat="server" Text='<%# Eval("EnderecoCompletoCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Endereço Obra</td>
                                                    <td align="left" colspan="5" nowrap="nowrap">
                                                        <asp:Label ID="lblLocalObra" runat="server" Text='<%# Eval("LocalizacaoObra") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Valor Entrada
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblValorEnt" runat="server" Text='<%# Eval("ValorEntrada", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Venda
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoVenda" runat="server" Text='<%# Eval("DescrTipoVenda") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoEntrega" runat="server" Text='<%# Eval("DescrTipoEntrega") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Situação
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacaoPedido") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataPedidoString", "{0:d}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDataEntrega" runat="server" Text='<%# Eval("DataEntregaString") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label runat="server" ID="lblValorFrete" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label> 
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="Label18" runat="server" Text='<%# Eval("ValorEntrega", "{0:C}") %>' OnLoad="txtValorFrete_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Desconto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDesconto" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold" runat="server" id="tituloComissao"
                                                        visible='<%# Eval("ComissaoVisible") %>'>
                                                        <asp:Label ID="Label13" runat="server" Text="Comissão"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="comissao" visible='<%# Eval("ComissaoVisible") %>'>
                                                        <asp:Label ID="lblComissao" runat="server" Text='<%# Eval("ValorComissao", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" runat="server" id="tituloIcms" onload="Icms_Load">
                                                        <asp:Label ID="lblTituloIcms" runat="server" Font-Bold="True" Text="Valor ICMS"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="valorIcms" onload="Icms_Load">
                                                        <asp:Label ID="lblValorIcms" runat="server" Text='<%# Eval("ValorIcms", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="tituloIpi" onload="Ipi_Load">
                                                        <asp:Label ID="lblTituloIpi" runat="server" Font-Bold="True" Text="Valor IPI"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="valorIpi" onload="Ipi_Load">
                                                        <asp:Label ID="lblValorIpi" runat="server" Text='<%# Eval("ValorIpi", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="6" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td class="cabecalho">
                                                                    <asp:Label ID="lblTitleTotal" runat="server" Font-Bold="True" OnLoad="lblTotalGeral_Load"
                                                                        Text="Total"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotal" runat="server" ForeColor="#0000CC" OnLoad="lblTotalGeral_Load"
                                                                        Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                                <td class="cabecalho" nowrap="nowrap">
                                                                    <asp:Label ID="lblTitleTotalBruto" runat="server" Font-Bold="True" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text="Total Bruto"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotalBruto" runat="server" OnLoad="lblTotalBrutoLiquido_Load" Text='<%# Eval("TotalBruto", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                                <td class="cabecalho" nowrap="nowrap">
                                                                    <asp:Label ID="lblTitleTotalLiquido" runat="server" Font-Bold="True" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text="Total Líquido"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotalLiquido" runat="server" ForeColor="#0000CC" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Forma Pagto.
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("PagtoParcela") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="lblTituloFastDelivery" runat="server" Text="Fast delivery" OnLoad="FastDelivery_Load"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblFastDelivery" runat="server" Text='<%# Eval("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        
                                                    </td>
                                                    <td colspan="3" align="left">
                                                        
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        
                                                    </td>
                                                    <td align="left">
                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label16" runat="server" Text="Funcionário comp."></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblFuncVenda" runat="server" Text='<%# Eval("NomeFuncVenda") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="lblDeveTransferirTexto" runat="server" Text="Deve Transferir?" OnLoad="Loja_Load"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblDeveTransferirValor" runat="server" Text='<%# Eval("DeveTransferirStr") %>' OnLoad="Loja_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Transportador
                                                    </td>
                                                    <td align="left" colspan="5">
                                                        <asp:Label ID="Label19" runat="server" Text='<%# Eval("NomeTransportador") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Observação
                                                    </td>
                                                    <td align="left" colspan="5">
                                                        <asp:Label ID="lblObs" runat="server" Text='<%# Eval("Obs") %>' ForeColor="Blue"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Obs. do Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblObsCliente" runat="server" OnLoad="lblObsCliente_Load" Text='<%# Eval("ObsCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                            <asp:HiddenField ID="hdfCliRevenda" runat="server" Value='<%# Eval("CliRevenda") %>' />
                                            <asp:HiddenField ID="hdfTipoVenda" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                            <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Eval("PercComissao") %>' />
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                            <asp:HiddenField ID="hdfFastDelivery" runat="server" OnPreRender="FastDelivery_Load"
                                                Value='<%# Eval("FastDelivery") %>' />
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                            <asp:HiddenField ID="hdfTipoPedido" runat="server" Value='<%# Eval("TipoPedido") %>' />
                                            <asp:HiddenField ID="hdfIsReposicao" runat="server" Value='<%# IsReposicao(Eval("TipoVenda")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                                OnClientClick="if (!onUpdate(this)) return false;" />
                                            <asp:Button ID="btnCancelarEdit" CausesValidation="false" runat="server" OnClick="btnCancelarEdit_Click"
                                                Text="Cancelar" />
                                            <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdPedido"
                                                Text='<%# Bind("IdPedido") %>' />
                                            <asp:HiddenField ID="hdfLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                            <asp:HiddenField ID="hdfClienteAtual" runat="server" Value='<%# Eval("IdCli") %>' />
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="if (!onInsert(this)) return false;"
                                                Text="Inserir" />
                                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                                Text="Cancelar" />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="true"><a href="#" onclick='openWindow(500, 700, "../Utils/SelTextoPedido.aspx?idPedido="+&#039;<%# Eval("IdPedido").ToString() %>&#039;); return false;'>
                                                <img border="0" src="../Images/note_add.gif" title="Textos Pedido" /></a> </asp:PlaceHolder>
                                            <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" OnClick="btnEditar_Click" />
                                            <asp:Button ID="btnFinalizar" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                Text="Finalizar" OnClientClick="if (!finalizarPedido()) return false;" OnClick="btnFinalizar_Click" />
                                            <asp:Button ID="btnEmConferencia" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                OnClick="btnEmConferencia_Click" OnClientClick="if (!emConferencia()) return false;"
                                                Text="Em Conferência" Visible='<%# Eval("ConferenciaVisible") %>' Width="110px" />
                                            <asp:Button ID="btnGerarConfEdit" runat="server" OnClick="btnGerarConfEdit_Click"
                                                OnLoad="btnGerarConfEdit_Load" Text="Confirmar editando Conferência " />
                                            <asp:Button ID="btnGerarConfFin" runat="server" OnClick="btnGerarConfFin_Click" OnLoad="btnGerarConfFin_Load"
                                                Text="Confirmar com Conferência Finalizada" />
                                            <asp:Button ID="btnVoltar" runat="server" OnClick="btnCancelar_Click" Visible="false"
                                                Text="Voltar" />
                                            <asp:HiddenField ID="hdfLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                            <asp:HiddenField ID="hdfAlterarProjeto" runat="server" Value="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkProjeto" runat="server" OnClientClick="return openProjeto('', false);">Incluir Projeto</asp:LinkButton>
                <div id="divProduto" runat="server">
                    <table>
                        <tr runat="server" id="inserirMaoObra" visible="false">
                            <td align="center">
                                <asp:LinkButton ID="lbkInserirMaoObra" runat="server">Inserir várias peças de vidro com a mesma mão de obra</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdAmbiente" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdAmbientePedido"
                                    DataSourceID="odsAmbiente" OnRowCommand="grdAmbiente_RowCommand" ShowFooter="True"
                                    OnPreRender="grdAmbiente_PreRender" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnRowDeleted="grdAmbiente_RowDeleted"
                                    OnRowUpdated="grdAmbiente_RowUpdated">
                                    <Columns>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="lnkAddAmbiente" runat="server" OnClientClick="exibirEsconderAmbiente(true); return false;"
                                                    ImageUrl="~/Images/Insert.gif" CausesValidation="False" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="False">
                                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Excluir este ambiente fará com que todos os produtos do mesmo sejam excluídos também, confirma exclusão?&quot;)"
                                                    CausesValidation="False" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" ValidationGroup="ambiente" />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Cancelar" CausesValidation="False" />
                                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                            </EditItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtAmbiente" runat="server" Text='<%# Eval("Ambiente") %>' MaxLength="50"
                                                    Width="150px" OnLoad="txtAmbiente_Load" onchange="FindControl('hdfDescrAmbiente', 'input').value = this.value"></asp:TextBox>
                                                <div runat="server" id="EditAmbMaoObra" onload="ambMaoObra_Load">
                                                    <asp:TextBox ID="txtCodAmb" runat="server" onblur='<%# "var_ProdutoAmbiente=true; loadProduto(this.value, 0);" %>'
                                                        onkeydown='<%# "if (isEnter(event)) loadProduto(this.value, 0);" %>' onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodInterno") %>' Width="50px"></asp:TextBox>
                                                    <asp:Label ID="lblDescrAmb" runat="server" Text='<%# Eval("Ambiente") %>'></asp:Label>
                                                    <a href="#" onclick="var_ProdutoAmbiente=true; getProduto(); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    <asp:HiddenField ID="hdfAmbIdProd" Value='<%# Bind("IdProd") %>' runat="server" />
                                                </div>
                                                <asp:HiddenField ID="hdfDescrAmbiente" Value='<%# Bind("Ambiente") %>' runat="server" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAmbiente" runat="server" MaxLength="50" Width="150px" OnLoad="txtAmbiente_Load"
                                                    onchange="FindControl('hdfDescrAmbiente', 'input').value = this.value"></asp:TextBox>
                                                <div runat="server" id="ambMaoObra" onload="ambMaoObra_Load">
                                                    <asp:TextBox ID="txtCodAmb" runat="server" onblur='<%# "var_ProdutoAmbiente=true; loadProduto(this.value, 0);" %>'
                                                        onkeydown='<%# "if (isEnter(event)) loadProduto(this.value, 0" %>' onkeypress="return !(isEnter(event));"
                                                        Width="50px"></asp:TextBox>
                                                    <asp:Label ID="lblDescrAmb" runat="server"></asp:Label>
                                                    <a href="#" onclick="var_ProdutoAmbiente=true; getProduto(); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    <asp:HiddenField ID="hdfAmbIdProd" runat="server" />
                                                </div>
                                                <asp:HiddenField ID="hdfDescrAmbiente" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkViewProd" runat="server" CausesValidation="False" CommandArgument='<%# Eval("IdAmbientePedido") %>'
                                                    CommandName="ViewProd" Text='<%# Eval("Ambiente") %>' Visible='<%# !(bool)Eval("ProjetoVisible") %>'></asp:LinkButton>
                                                <asp:PlaceHolder ID="PlaceHolder1" Visible='<%# Eval("ProjetoVisible") %>' runat="server">
                                                    <a href="#" onclick='return openProjeto(<%# Eval("IdAmbientePedido") %>)'>
                                                        <%# Eval("Ambiente") %></a> </asp:PlaceHolder>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditDescricao" runat="server" Text='<%# Bind("Descricao") %>'
                                                    MaxLength="1000" Rows="2" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="1000" Rows="2" TextMode="MultiLine"
                                                    Width="300px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                                <asp:Label ID="Label17" runat="server" ForeColor="Red" Text='<%# Eval("DescrObsProj") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditQtdeAmbiente" runat="server" Text='<%# Bind("Qtde") %>' onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtEditQtdeAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtQtdeAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtQtdeAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>                                        
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditLarguraAmbiente" runat="server" Text='<%# Bind("Largura") %>'
                                                    onkeypress="return soNumeros(event, true, true)" Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvLargura" runat="server" ControlToValidate="txtEditLarguraAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtLarguraAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvLargura" runat="server" ControlToValidate="txtLarguraAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditAlturaAmbiente" runat="server" Text='<%# Bind("Altura") %>'
                                                    onkeypress="return soNumeros(event, true, true)" Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvAltura" runat="server" ControlToValidate="txtEditAlturaAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAlturaAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvAltura" runat="server" ControlToValidate="txtAlturaAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>                                        
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso" Visible="False">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbProcIns" runat="server" onblur="var_ProcAmbiente=true; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_ProcAmbiente=true; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_ProcAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbProcIns" runat="server" onblur="var_ProcAmbiente=true; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_ProcAmbiente=true; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_ProcAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao" Visible="False">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbAplIns" runat="server" onblur="var_AplAmbiente=true; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_AplAmbiente=true; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_AplAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbAplIns" runat="server" onblur="var_AplAmbiente=true; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_AplAmbiente=true; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_AplAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Redondo" SortExpression="Redondo" Visible="False">
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Redondo") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:CheckBox ID="chkRedondoAmbiente" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Redondo") %>' Enabled="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor produtos" SortExpression="TotalProdutos">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotalProd" runat="server" Text='<%# Eval("TotalProdutos", "{0:c}") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("TotalProdutos", "{0:c}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Acréscimo" SortExpression="Acrescimo">
                                            <EditItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="drpTipoAcrescimo" runat="server" SelectedValue='<%# Bind("TipoAcrescimo") %>'>
                                                                <asp:ListItem Value="1">%</asp:ListItem>
                                                                <asp:ListItem Selected="True" Value="2">R$</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtAcrescimo" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                                Text='<%# Bind("Acrescimo") %>' Width="50px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desconto" SortExpression="Desconto">
                                            <EditItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                                onclick="calcularDesconto(2)">
                                                                <asp:ListItem Value="1">%</asp:ListItem>
                                                                <asp:ListItem Value="2">R$</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtDesconto" runat="server" onchange="calcularDesconto(2)" onkeypress="return soNumeros(event, false, true)"
                                                                Text='<%# Bind("Desconto") %>' Width="50px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfValorDescontoAtual" runat="server" Value='<%# Eval("ValorDescontoAtual") %>' />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <asp:LinkButton ID="lnkInsAmbiente" runat="server" OnClick="lnkInsAmbiente_Click"
                                                    ValidationGroup="ambiente">
                                            <img border="0" src="../Images/ok.gif" /></asp:LinkButton>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                             <ItemTemplate>
                                                 <uc12:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="AmbientePedido" IdRegistro='<%# Eval("IdAmbientePedido") %>' />
                                             </ItemTemplate>
                                         </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                                <asp:Label ID="lblAmbiente" runat="server" CssClass="subtitle1" Font-Bold="False"></asp:Label>
                                <asp:HiddenField ID="hdfAlturaAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfLarguraAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfQtdeAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfRedondoAmbiente" runat="server" />
                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAmbiente" runat="server" DataObjectTypeName="Glass.Data.Model.AmbientePedido"
                                    DeleteMethod="DeleteComTransacao" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.AmbientePedidoDAO"
                                    UpdateMethod="Update" OnDeleted="odsAmbiente_Deleted" OnUpdating="odsAmbiente_Updating"
                                    OnDeleting="odsAmbiente_Deleting" OnInserting="odsAmbiente_Inserting" >
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                                <asp:HiddenField ID="hdfIdAmbiente" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%# Eval("Ambiente") %>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" CssClass="gridStyle"
                                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                    DataKeyNames="IdProdPed" OnRowDeleted="grdProdutos_RowDeleted" ShowFooter="True"
                                    OnRowCommand="grdProdutos_RowCommand" OnPreRender="grdProdutos_PreRender" PageSize="12"
                                    OnRowUpdated="grdProdutos_RowUpdated" OnRowCreated="grdProdutos_RowCreated">
                                    <FooterStyle Wrap="True" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <select id="drpFooterVisible" style="display: none">
                                                </select>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>'>
                                                    <img border="0" src="../Images/Edit.gif" ></img></asp:LinkButton>
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>' OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(false); return false" : "if (!confirm(\"Deseja remover esse produto do pedido?\")) return false" %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick='<%# "if(!onUpdateProd(" + Eval("IdProdPed") + ")) return false;"%>' />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Cancelar" />
                                                <asp:HiddenField ID="hdfProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                                <asp:HiddenField ID="hdfCodInterno" runat="server" Value='<%# Eval("CodInterno") %>' />
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" Value='<%# Eval("M2Minimo") %>' />
                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                                                <asp:HiddenField ID="hdfIdMaterItemProj" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                                                <asp:HiddenField ID="hdfIdAmbientePedido" runat="server" Value='<%# Bind("IdAmbientePedido") %>' />
                                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" Value='<%# Bind("AliqIcms") %>' />
                                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" Value='<%# Bind("ValorIcms") %>' />
                                                <asp:HiddenField ID="hdfValorTabelaOrcamento" runat="server" Value='<%# Bind("ValorTabelaOrcamento") %>' />
                                                <asp:HiddenField ID="hdfValorTabelaPedido" runat="server" Value='<%# Bind("ValorTabelaPedido") %>' />
                                            </EditItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblCodProdIns" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfCustoProd" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtCodProdIns" runat="server" onblur='<%# "loadProduto(this.value, 0);" %>'
                                                    onkeydown='<%# "if (isEnter(event)) loadProduto(this.value, 0);" %>' onkeypress="return !(isEnter(event));"
                                                    Width="50px" autofocus></asp:TextBox>
                                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                                <a href="#" onclick="getProduto(); return false;">
                                                    <img src="../Images/Pesquisar.gif" border="0" /></a>
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" />
                                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" />
                                                <asp:HiddenField ID="hdfCustoProd" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onblur="calcM2Prod(); return verificaEstoque();"
                                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Text='<%# Bind("Qtde") %>' Width="50px"></asp:TextBox>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd"
                                                    CallbackValorUnit="calcTotalProd" ValidationGroup="produto" PercDescontoQtde='<%# Bind("PercDescontoQtde") %>'
                                                    ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>' OnLoad="ctrlDescontoQtde_Load" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onkeydown="if (isEnter(event)) calcM2Prod();"
                                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    onblur="calcM2Prod(); return verificaEstoque();" Width="50px"></asp:TextBox>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd"
                                                    ValidationGroup="produto" CallbackValorUnit="calcTotalProd" OnLoad="ctrlDescontoQtde_Load" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtLarguraIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, true, true);"
                                                    Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                    onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="calcM2Prod(); return verificaEstoque();"
                                                    Text='<%# Bind("Altura") %>' onchange="FindControl('hdfAlturaReal', 'input').value = this.value"
                                                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"></asp:TextBox>
                                                <asp:HiddenField ID="hdfAlturaReal" runat="server" Value='<%# Bind("AlturaReal") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="calcM2Prod(); return verificaEstoque();"
                                                    Width="50px" onchange="FindControl('hdfAlturaReal', 'input').value = this.value"
                                                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"></asp:TextBox>
                                                <asp:HiddenField ID="hdfAlturaRealIns" runat="server" />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotM2Ins" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfTotM" runat="server" Value='<%# Eval("TotM") %>' />
                                                <asp:HiddenField ID="hdfTamanhoMaximoObra" runat="server" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotM2Ins" runat="server"></asp:Label>
                                                <asp:HiddenField ID="hdfTamanhoMaximoObra" runat="server" />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotM2Calc" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfTotM2Calc" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                                                <asp:HiddenField ID="hdfTotM2CalcSemChapa" runat="server" Value='<%# Eval("TotalM2CalcSemChapaString") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotM2CalcIns" runat="server"></asp:Label>
                                                <asp:HiddenField ID="hdfTotM2Ins" runat="server" />
                                                <asp:HiddenField ID="hdfTotM2CalcIns" runat="server" />
                                                <asp:HiddenField ID="hdfTotM2CalcSemChapaIns" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorVendido", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtValorIns" runat="server" onblur="calcTotalProd();" onkeypress="return soNumeros(event, false, true);"
                                                    Text='<%# Bind("ValorVendido") %>' Width="50px" OnLoad="txtValorIns_Load"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtValorIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProd();"
                                                    onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();"
                                                    Width="50px" OnLoad="txtValorIns_Load"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="var_ProcAmbiente=false; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_ProcAmbiente=false; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkProcesso" href="#" onclick='var_ProcAmbiente=false; buscarProcessos(); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="var_ProcAmbiente=false; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_ProcAmbiente=false; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkProcesso" href="#" onclick='var_ProcAmbiente=false; buscarProcessos(); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="var_AplAmbiente=false; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_AplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_AplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="var_AplAmbiente=false; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_AplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_AplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ped. Cli." SortExpression="PedCli">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPedCli" runat="server" MaxLength="50" Text='<%# Bind("PedCli") %>'
                                                    Width="50px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtPedCli" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("PedCli") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                                                <asp:Label ID="Label43" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>'
                                                    Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotalIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px"></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotalIns" runat="server"></asp:Label>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblValorBenef" runat="server"></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("ValorBenef", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <div id="benefMaoObra" style='<%# !IsPedidoMaoDeObra() ? "display: none;": "" %> white-space: nowrap'>                                                    
                                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Bind("LarguraBenef") %>'>
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Bind("AlturaBenef") %>'>
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    Esp.:
                                                    <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                        Text='<%# Bind("EspessuraBenef") %>'></asp:TextBox>
                                                </div>
                                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick='<%# "exibirBenef(this, " + Eval("IdProdPed") + "); return false;" %>'
                                                    Visible='<%# Eval("BenefVisible") %>'>
                                                    <img border="0" src="../Images/gear_add.gif" />
                                                </asp:LinkButton>
                                                <table id='<%# "tbConfigVidro_" + Eval("IdProdPed") %>' cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtEspessura" runat="server" OnDataBinding="txtEspessura_DataBinding"
                                                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc4:ctrlBenef ID="ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
                                                                ValidationGroup="produto" OnInit="ctrlBenef_Load" Redondo='<%# Bind("Redondo") %>'
                                                                CallbackCalculoValorTotal="setValorTotal" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                        </td>
                                                    </tr>
                                                </table>

                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <div id="benefMaoObra" style='<%# !IsPedidoMaoDeObra() ? "display: none;": "" %> white-space: nowrap'>
                                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()">
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()">
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>                                                    
                                                    Esp.:
                                                    <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                                </div>
                                                <asp:LinkButton ID="lnkBenef" runat="server" Style="display: none;" OnClientClick="exibirBenef(this, 0); return false;">
                                                    <img border="0" src="../Images/gear_add.gif" />
                                                </asp:LinkButton>
                                                <table id="tbConfigVidro_0" cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                            Width="30px"></asp:TextBox>
                                                                        <asp:HiddenField ID="xsds" runat="server" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc4:ctrlBenef ID="ctrlBenefInserir" runat="server" OnInit="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotal"
                                                                ValidationGroup="produto" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <div id='<%# "imgProdsComposto_" + Eval("IdProdPed") %>'>
                                                    <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/box.png" ToolTip="Exibir Produtos da Composição"
                                                        Visible='<%# Eval("IsProdLamComposicao") %>' OnClientClick='<%# "exibirProdsComposicao(this, " + Eval("IdProdPed") + "); return false"%>' />
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif"
                                                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=pedido&idPedido=" + Eval("IdPedido") +"&idProdPed=" +  Eval("IdProdPed") +
                                                            "&pecaAvulsa=" +  ((bool)Eval("IsProdLamComposicao") == false) + "\"); return false" %>'
                                                        ToolTip="Exibir imagem das peças"  Visible='<%# (Eval("IsVidro").ToString() == "true")%>'/>
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate></EditItemTemplate>
                                            <FooterTemplate></FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <a href="#" id="lnkObsCalc" onclick="exibirObs(<%# Eval("IdProdPed") %>, this); return false;" visible='<%# (Eval("IsVidro").ToString() == "true")%>'>
                                                    <img border="0" src="../../Images/blocodenotas.png" title="Observação da peça" /></a>
                                                <table id='tbObsCalc_<%# Eval("IdProdPed") %>' cellspacing="0" style="display: none;">
                                                    <tr>
                                                        <td align="center">
                                                            <asp:TextBox ID="txtObsCalc" runat="server" Width="320" Rows="4" MaxLength="500"
                                                                TextMode="MultiLine" Text='<%# Eval("Obs") %>'></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <input id="btnSalvarObs" onclick='setCalcObs(<%# Eval("IdProdPed") %>, this); return false;'
                                                                type="button" value="Salvar" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                            <EditItemTemplate></EditItemTemplate>
                                            <FooterTemplate></FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                             <ItemTemplate>
                                                </td> </tr>

                                                <tr id="prodPed_<%# Eval("IdProdPed") %>" style="display: none" align="center">
                                                    <td colspan="17">
                                                        <br />
                                                        <uc13:ctrlProdComposicao runat="server" ID="ctrlProdComp" Visible='<%# Eval("IsProdLamComposicao") %>' 
                                                            IdProdPed='<%# Glass.Conversoes.StrParaUint(Eval("IdProdPed").ToString()) %>'/>
                                                        <br />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                </td> </tr>
                                                <tr style='<%= !IsPedidoMaoDeObra() ? "display: none": "" %>'>
                                                    <td colspan="17" style="text-align: right">
                                                        <span style="position: relative; top: -6px">campos usados para definir altura, largura
                                                            e
                                                            <br />
                                                            espessura da lapidação e bisotê </span>
                                                    </td>
                                                </tr>
                                                <tr style='<%= !IsPedidoProducao() ? "display: none": "" %>'>
                                                    <td colspan="4">
                                                    </td>
                                                    <td colspan="13" style="text-align: left">
                                                        <span style="position: relative; top: -6px">altura e largura definidas no produto
                                                            <br />
                                                            e recuperadas automaticamente </span>
                                                    </td>
                                                </tr>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="lnkInsProd" runat="server" OnClick="lnkInsProd_Click" ImageUrl="../Images/ok.gif"
                                                    OnClientClick="if (!onInsertProd()) return false;" />
                                                </td> </tr>
                                                <tr style='<%= !IsPedidoMaoDeObra() ? "display: none": "" %>'>
                                                    <td colspan="15" style="text-align: right">
                                                        <span style="position: relative; top: -6px">campos usados para definir altura, largura
                                                            e
                                                            <br />
                                                            espessura da lapidação e bisotê </span>
                                                    </td>
                                                </tr>
                                                <tr style='<%= !IsPedidoProducao() ? "display: none": "" %>'>
                                                    <td colspan="4">
                                                    </td>
                                                    <td colspan="13" style="text-align: left">
                                                        <span style="position: relative; top: -6px">altura e largura definidas no produto
                                                            <br />
                                                            e recuperadas automaticamente </span>
                                                    </td>
                                                </tr>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
        DeleteMethod="DeleteEAtualizaDataEntrega" EnablePaging="True" MaximumRowsParameterName="pageSize"
        OnDeleted="odsProdXPed_Deleted" SelectCountMethod="GetCount" SelectMethod="GetList"
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
        UpdateMethod="UpdateEAtualizaDataEntrega" OnUpdating="odsProdXPed_Updating" 
        OnDeleting="odsProdXPed_Deleting" OnInserting="odsProdXPed_Inserting" OnUpdated="odsProdXPed_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
            <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value"
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfPedidoMaoDeObra" runat="server" />
    <asp:HiddenField ID="hdfPedidoProducao" runat="server" />
    <asp:HiddenField ID="hdfIdPedido" runat="server" />
    <asp:HiddenField ID="hdfIdProd" runat="server" />
    <asp:HiddenField ID="hdfNaoVendeVidro" runat="server" />
    <asp:HiddenField ID="hdfProdPedComposicaoSelecionado" runat="server" Value="0" />

    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="ObtemListaPorTipo"
        TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
        <SelectParameters>
            <asp:Parameter Name="tipo" Type="Int32" DefaultValue="0" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server"
        SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
        InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoDAO"
        UpdateMethod="Update" OnInserted="odsPedido_Inserted" OnUpdating="odsPedido_Updating" OnUpdated="odsPedido_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
        SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoJato" runat="server" SelectMethod="GetTipoJato"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCanto" runat="server" SelectMethod="GetTipoCanto"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
        TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForPedido"
        TypeName="Glass.Data.DAL.FormaPagtoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncVenda" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedido"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransportador" runat="server"
        SelectMethod="ObtemDescritoresTransportadores" TypeName="Glass.Global.Negocios.ITransportadorFluxo">
    </colo:VirtualObjectDataSource>
    <script type="text/javascript" src="CadPedido.js" />
    <script type="text/javascript">
        inicializarControles();
    </script>
</asp:Content>
