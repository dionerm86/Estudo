<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadPedido.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadPedido" Title="Cadastrar Pedido" EnableEventValidation="false" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>
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
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Pedidos/Templates/CadPedido.Ambientes.html",
            "~/Vue/Pedidos/Templates/CadPedido.Produtos.html")
    %>

    <script type="text/javascript">

        var config_UsarBenefTodosGrupos = <%= Glass.Configuracoes.Geral.UsarBeneficiamentosTodosOsGrupos.ToString().ToLower() %>;
        var config_UsarControleDescontoFormaPagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;
        var config_ObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
        var config_UtilizarRoteiroProducao = <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
        var config_ExibirPopupFaltaEstoque = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower() %>;
        var config_UsarAltLarg = <%= Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura.ToString().ToLower() %>;

        var var_IdPedido = '<%= Request["idPedido"] %>';
        var var_ValorDescontoTotalProdutos = <%= GetDescontoProdutos() %>;
        var var_ValorDescontoTotalPedido = <%= GetDescontoPedido() %>;
        var var_TotalM2Pedido = "<%= GetTotalM2Pedido() %>";
        var var_QtdEstoque = 0;
        var var_QtdEstoqueMensagem = 0;
        var var_ExibirMensagemEstoque = false;
        var var_ProdutoAmbiente = false;
        var var_AplAmbiente = false;
        var var_ProcAmbiente = false;
        var var_Loading = true;
        var var_SaveProdClicked = false;

    </script>

    <div id="app">
        <div v-if="editando || inserindo">
            <section class="detalhes">
                <span class="form-group">
                    <label>
                        Cliente
                    </label>
                    <campo-busca-cliente :cliente.sync="clienteAtual" :exibir-informacoes-compra="true" tipo-validacao="Pedido"
                        :cor-texto-observacoes="configuracoes.corTextoObservacoesCliente" :disabled="pedido.podeEditar" v-if="pedido" required></campo-busca-cliente>
                </span>
                <span class="form-group">
                    <label>
                        Data Ped.
                    </label>
                    {{ pedido.dataPedido | data }}
                </span>
                <span class="form-group" v-if="configuracoes.exibirFastDelivery && configuracoes.marcarFastDelivery">
                    <input id="fastDelivery" type="checkbox" v-model="pedido.fastDelivery" />
                    <label for="fastDelivery">
                        Fast Delivery
                    </label>
                </span>
                <span class="form-group">
                    <label>
                        Cód. Ped. Cli.
                    </label>
                    <input v-model="pedido.codigoPedidoCliente" :disabled="pedido && pedido.importado" />
                </span>
                <span class="form-group">
                    <label>
                        Orçamento
                    </label>
                    <input type="number" v-model="pedido.idOrcamento" style="width: 50px" />
                </span>
                <span class="form-group">
                    <label>
                        Loja
                    </label>
                    <lista-selecao-lojas :loja.sync="lojaAtual" :ativas="true" :disabled="!configuracoes.usarControleOrdemCarga && !configuracoes.alterarLojaPedido && !clientePermiteAlterarLoja"></lista-selecao-lojas>
                </span>
                <span class="form-group" v-if="configuracoes.exibirDeveTransferir">
                    <label for="deveTransferir">
                        Deve transferir?
                    </label>
                    <input id="deveTransferir" type="checkbox" v-model="pedido.deveTransferir" :disabled="!clientePermiteAlterarLoja" />
                </span>
                <span class="form-group">
                    <label>
                        Tipo Pedido
                    </label>
                    <lista-selecao-id-valor :item-selecionado.sync="tipoPedidoAtual" :funcao-recuperar-itens="obterTiposPedido" :disabled="(editando && configuracoes.bloquearItensTipoPedido) || pedidoMaoDeObra || pedidoProducao" required></lista-selecao-id-valor>
                </span>
                <span class="form-group" v-if="configuracoes.gerarPedidoProducaoCorte && pedido.tipo == configuracoes.tipoPedidoRevenda">
                    <input type="checkbox" id="gerarPedidoProducaoCorte" v-model="pedido.gerarPedidoCorte" />
                    <label for="gerarPedidoProducaoCorte">
                        Gerar pedido de produção para corte
                    </label>
                </span>
                <span class="form-group">
                    <label>
                        Tipo Venda
                    </label>
                    <lista-selecao-id-valor :item-selecionado.sync="tipoVendaAtual" :funcao-recuperar-itens="obterTiposVendaCliente" :filtro-recuperar-itens="filtroTiposVendaCliente"
                        :disabled="editando && pedido.sinal && pedido.sinal.id > 0 && !configuracoes.usarLiberacaoPedido" required></lista-selecao-id-valor>
                    <span v-if="pedido && pedido.tipoVenda == configuracoes.tipoVendaObra">
                        <campo-busca-com-popup :id.sync="pedido.idObra" :nome.sync="descricaoObraAtual" campo-nome="descricao" :item-selecionado.sync="obraAtual" :funcao-buscar-itens="obterObras" :disabled="!clienteAtual || clienteAtual.id == 0"
                            :url-popup="'/Utils/SelObra.aspx?situacao=4&tipo=1&idsPedidosIgnorar=' + (pedido.id || '') + '&idCliente=' + (clienteAtual.id || '')" :largura-popup="650" :altura-popup="560" style="width: 90px" required></campo-busca-com-popup>
                        <template v-if="obraAtual">
                            Saldo da Obra: {{ obraAtual.saldo | moeda }}
                            Saldo Pedidos Abertos: {{ obraAtual.totalPedidosAbertosObra | moeda }}
                            Saldo Atual: {{ obraAtual.saldo - obraAtual.totalPedidosAbertosObra | moeda }}
                        </template>
                    </span>
                    <span class="form-group" v-if="vIfNumeroParcelas">
                        <label>
                            Num. Parcelas
                        </label>
                        <lista-selecao-parcelas :parcela.sync="parcelaAtual" :id-cliente="pedido.idCliente" :pode-editar="configuracoes.permitirAlterarDataParcelas" required></lista-selecao-parcelas>
                    </span>
                </span>
                <span class="form-group" v-if="pedido && pedido.tipoVenda == configuracoes.tipoVendaFuncionario">
                    <label>
                        Funcionário comprador
                    </label>
                    <lista-selecao-id-valor :item-selecionado.sync="funcionarioCompradorAtual" :funcao-recuperar-itens="obterFuncionariosCompradores" required></lista-selecao-id-valor>
                </span>
                <span class="form-group">
                    <label>
                        Tipo Entrega
                    </label>
                    <lista-selecao-id-valor :item-selecionado.sync="tipoEntregaAtual" :funcao-recuperar-itens="obterTiposEntrega" required></lista-selecao-id-valor>
                </span>
                <span class="form-group">
                    <label>
                        Forma Pagto.
                    </label>
                    <template v-if="pedido.tipoVenda == configuracoes.tipoVendaAPrazo || pedido.tipoVenda == configuracoes.tipoVendaReposicao || pedido.tipoVenda == configuracoes.tipoVendaGarantia ||
                        (configuracoes.UsarControleDescontoFormaPagamentoDadosProduto && pedido.tipoVenda == configuracoes.tipoVendaAVista)">
                        <lista-selecao-id-valor :item-selecionado.sync="formaPagamentoAtual" :funcao-recuperar-itens="obterFormasPagamento" :filtro-recuperar-itens="filtroFormasPagamento" required></lista-selecao-id-valor>
                        <span class="form-group">
                            <lista-selecao-id-valor :item-selecionado.sync="tipoCartaoAtual" :funcao-recuperar-itens="obterTiposCartao" v-if="pedido.formaPagamento.id == configuracoes.idFormaPagamentoCartao" required></lista-selecao-id-valor>
                        </span>
                    </template>
                </span>
                <span class="form-group">
                    <label>
                        Data Entrega
                    </label>
                    <campo-data-hora :data-hora.sync="pedido.entrega.data" :data-minima="dataEntregaMinima"
                        :permitir-feriado="false" :permitir-fim-de-semana="false"
                        :disabled="datasEntrega && datasEntrega.desabilitarCampo" required></campo-data-hora>
                </span>
                <span class="form-group" v-if="configuracoes.exibirValorFretePedido">
                    <label>
                        Valor do Frete
                    </label>
                    <input type="number" v-model.number="pedido.valorEntrega" style="width: 80px;" />
                </span>
                <span class="form-group" v-if="vIfValorEntrada">
                    <label>
                        Valor Entrada
                    </label>
                    <template v-if="vIfCampoEntrada">
                        <input type="number" v-model="pedido.valorEntrada" />
                    </template>
                    <template v-else>
                        {{ pedido.textoSinal }}
                    </template>
                </span>
                <span class="form-group">
                    <label>
                        Desconto
                    </label>
                    <campo-acrescimo-desconto :tipo.sync="pedido.desconto.tipo" :valor.sync="pedido.desconto.valor" :disabled="disabledCampoDesconto" v-if="vIfCampoDesconto"></campo-acrescimo-desconto>
                    <span style="color: blue" v-else>
                        Desconto só pode ser dado em pedidos à vista
                    </span>
                </span>
                <span class="form-group">
                    <label>
                        Total
                    </label>
                    {{ pedido.total | moeda }}
                </span>
                <span class="form-group">
                    <label>
                        Acréscimo
                    </label>
                    <campo-acrescimo-desconto :tipo.sync="pedido.acrescimo.tipo" :valor.sync="pedido.acrescimo.valor"></campo-acrescimo-desconto>
                </span>
                <span class="form-group">
                    <label>
                        Funcionário
                    </label>
                    <lista-selecao-id-valor :item-selecionado.sync="vendedorAtual" :funcao-recuperar-itens="obterVendedores" :disabled="configuracoes.alterarVendedor" @change.prevent="alterarVendedor"></lista-selecao-id-valor>
                </span>
                <span class="form-group colspan3">
                    <label>
                        Transportador
                    </label>
                    <lista-selecao-id-valor :item-selecionado.sync="transportadorAtual" :funcao-recuperar-itens="obterTransportadores"></lista-selecao-id-valor>
                </span>
                <template v-if="pedido && pedido.entrega.tipo != configuracoes.tipoEntregaBalcao">
                    <span class="form-group">
                        <label>
                            Local da Obra
                        </label>
                        <button @click.prevent="preencherEnderecoObra">
                            <img src="../Images/home.gif" title="Buscar endereço do cliente" />
                        </button>
                    </span>
                    <span class="form-group colspan2">
                        <campo-endereco :endereco="pedido.enderecoObra" required>
                    </span>
                </template>
                <span class="form-group colspan3">
                    <controle-parcelas :parcelas.sync="parcelaAtual" v-if="vIfControleParcelas" :data-minima="pedido.dataPedido" :total="totalParaCalculoParcelas"></controle-parcelas>
                </span>
                <template v-if="configuracoes.usarComissaoNoPedido">
                    <span class="form-group">
                        <label>
                            Comissionado:
                        </label>
                    </span>
                    <template v-if="!configuracoes.usarComissionadoDoCliente">
                        <span class="form-group">
                            <campo-busca-com-popup :id.sync="pedido.comissionado.id" campo-nome="descricao" :item-selecionado.sync="comissionadoAtual" :funcao-buscar-itens="obterComissionados"
                                :url-popup="'/Utils/SelComissionado.aspx'" :largura-popup="760" :altura-popup="590" style="width: 90px" required></campo-busca-com-popup>
                        </span>
                        <span class="form-group">
                            <label>
                                Percentual:
                            </label>
                            <input type="number" v-model="pedido.percentualComissao" :disabled="configuracoes.alterarPercentualComissionado" />
                        </span>
                        <span class="form-group">
                            <label>
                                Valor comissão:
                            </label>
                            {{ pedido.valorComissao | moeda }}
                        </span>
                    </template>
                </template>
                <span class="form-group colspan3" v-if="configuracoes.usarControleMedicao">
                    <label>
                        Medidor
                    </label>
                    <lista-selecao-id-valor :item-selecionado.sync="medidorAtual" :funcao-recuperar-itens="obterMedidores"></lista-selecao-id-valor>
                </span>
                <span class="form-group colspan3">
                    <label>
                        Observação
                    </label>
                    <textarea v-model="pedido.observacao" style="width: 650px"></textarea>
                </span>
                <span class="form-group colspan3" v-if="configuracoes.usarLiberacaoPedido">
                    <label>
                        Observação liberação
                    </label>
                    <textarea v-model="pedido.observacaoLiberacao" style="width: 650px"></textarea>
                </span>
                <span class="botoes">
                    <span>
                        <button @click.prevent="inserirPedido" v-if="inserindo">
                            Inserir
                        </button>
                        <button @click.prevent="atualizarPedido" v-else-if="editando">
                            Atualizar
                        </button>
                        <button @click.prevent="cancelar">
                            Cancelar
                        </button>
                    </span>
                </span>
            </section>
        </div>
        <div v-else>
            <section class="detalhes">
                <span>
                    <label>
                        Num. Pedido
                    </label>
                    <span style="font-size: medium">
                        {{ pedido.id }}
                    </span>
                    <span style="color: green" v-if="pedido && pedido.tipo">
                        ({{ pedido.tipo.nome }})
                    </span>
                </span>
                <span>
                    <label>
                        Cliente
                    </label>
                    <span v-if="pedido && pedido.cliente">
                        {{ pedido.cliente.id }}
                        -
                        {{ pedido.cliente.nome }}
                    </span>
                </span>
                <span>
                    <label>
                        Funcionário
                    </label>
                    <span v-if="pedido && pedido.vendedor">
                        {{ pedido.vendedor.nome }}
                    </span>
                </span>
                <span>
                    <label>
                        Tel. Cliente
                    </label>
                    <span v-if="pedido && pedido.cliente">
                        {{ pedido.cliente.telefone }}
                    </span>
                </span>
                <span>
                    <label>
                        Loja
                    </label>
                    <span v-if="pedido && pedido.loja">
                        {{ pedido.loja.nome }}
                    </span>
                </span>
                <span class="colspan3">
                    <label>
                        Endereço Cliente
                    </label>
                    <span v-if="pedido && pedido.cliente">
                        {{ pedido.cliente.endereco }}
                    </span>
                </span>
                <template v-if="pedido.obra">
                    <span>
                        <label>
                            Endereço Obra
                        </label>
                        <span>
                            {{ pedido.obra.endereco }}
                        </span>
                    </span>
                </template>
                <span v-if="pedido && pedido.sinal">
                    <label>
                        Valor Entrada
                    </label>
                    <span>
                        {{ pedido.sinal.valor }}
                    </span>
                </span>
                <span>
                    <label>
                        Tipo Venda
                    </label>
                    <span v-if="pedido && pedido.tipoVenda">
                        {{ pedido.tipoVenda.nome }}
                    </span>
                </span>
                <span>
                    <label>
                        Tipo Entrega
                    </label>
                    <span v-if="pedido && pedido.entrega && pedido.entrega.tipo">
                        {{ pedido.entrega.tipo.nome }}
                    </span>
                </span>
                <span>
                    <label>
                        Situação
                    </label>
                    <span v-if="pedido && pedido.situacao">
                        {{ pedido.situacao.nome }}
                    </span>
                </span>
                <span>
                    <label>
                        Data Ped.
                    </label>
                    <span v-if="pedido">
                        {{ pedido.dataPedido | data }}
                    </span>
                </span>
                <span>
                    <label>
                        Data Entrega
                    </label>
                    <span v-if="pedido && pedido.entrega">
                        {{ pedido.entrega.data | data }}
                    </span>
                </span>
                <span>
                    <label>
                        Valor do Frete
                    </label>
                    <span v-if="pedido && pedido.entrega">
                        {{ pedido.entrega.valor | moeda }}
                    </span>
                    <span v-else>
                        {{ 0 | moeda }}
                    </span>
                </span>
                <span>
                    <label>
                        Desconto
                    </label>
                    <span v-if="pedido && pedido.desconto">
                        <span v-if="pedido.desconto.tipo == 1">
                            {{ pedido.desconto.valor | percentual }}
                        </span>
                        <span v-else>
                            {{ pedido.desconto.valor | moeda }}
                        </span>
                    </span>
                    <span v-else>
                        {{ 0 | moeda }}
                    </span>
                </span>
                <span>
                    <label>
                        Comissão
                    </label>
                    <span v-if="pedido && pedido.comissionado && pedido.comissionado.comissao">
                        {{ pedido.comissionado.comissao.valor | moeda }}
                    </span>
                    <span v-else>
                        {{ 0 | moeda }}
                    </span>
                </span>
                <span v-if="configuracoes.calcularIcms">
                    <label>
                        Valor ICMS
                    </label>
                    <span v-if="pedido && pedido.icms">
                        {{ pedido.icms.valor | moeda }}
                    </span>
                    <span v-else>
                        {{ 0 | moeda }}
                    </span>
                </span>
                <span v-if="configuracoes.calcularIpi">
                    <label>
                        Valor IPI
                    </label>
                    <span v-if="pedido && pedido.ipi">
                        {{ pedido.ipi.valor | moeda }}
                    </span>
                    <span v-else>
                        {{ 0 | moeda }}
                    </span>
                </span>
                <span class="colspan3" v-if="!configuracoes.empresaNaoVendeVidro">
                    <label>
                        Total
                    </label>
                    <span style="color: #0000CC">
                        {{ pedido.total | moeda }}
                    </span>
                </span>
                <span class="colspan3" v-else>
                    <span>
                        <label>
                            Total Bruto
                        </label>
                        <span>
                            {{ pedido.totalBruto | moeda }}
                        </span>
                    </span>
                    <span>
                        <label>
                            Total Líquido
                        </label>
                        <span style="color: #0000CC">
                            {{ pedido.total | moeda }}
                        </span>
                    </span>
                </span>
                <span>
                    <label>
                        Forma Pagto.
                    </label>
                    <span v-if="pedido && pedido.formaPagamento">
                        {{ pedido.formaPagamento.nome }}
                    </span>
                </span>
                <span v-if="configuracoes.exibirFastDelivery">
                    <label>
                        Fast Delivery
                    </label>
                    <span v-if="pedido && pedido.fastDelivery">
                        {{ pedido.fastDelivery.aplicado | simNao }}
                    </span>
                </span>
                <span v-if="configuracoes.exibirDeveTransferir">
                    <label>
                        Deve transferir?
                    </label>
                    <span v-if="pedido">
                        {{ pedido.deveTransferir | simNao }}
                    </span>
                </span>
                <span v-if="pedido && pedido.funcionarioComprador">
                    <label>
                        Funcionário comp.
                    </label>
                    <span>
                        {{ pedido.funcionarioComprador.nome }}
                    </span>
                </span>
                <span v-if="pedido && pedido.transportador">
                    <label>
                        Transportador
                    </label>
                    <span>
                        {{ pedido.transportador.nome }}
                    </span>
                </span>
                <span>
                    <label>
                        Observação
                    </label>
                    <span v-if="pedido" style="color: blue">
                        {{ pedido.observacao }}
                    </span>
                </span>
                <span class="colspan3">
                    <label>
                        Obs. do Cliente
                    </label>
                    <span v-if="pedido && pedido.cliente" style="color: red" v-html="pedido.cliente.observacao">
                    </span>
                </span>
                <template v-if="configuracoes.exibirRentabilidade && pedido && pedido.rentabilidade">
                    <span>
                        <label>
                            Rentabilidade
                        </label>
                        <span>
                            {{ pedido.rentabilidade.percentual | percentual }}
                        </span>
                    </span>
                    <span>
                        <label>
                            Rent. Financeira
                        </label>
                        <span>
                            {{ pedido.rentabilidade.valor | moeda }}
                        </span>
                        <a href="#" @click.prevent="abrirRentabilidade" title="Rentabilidade">
                            <img src="../Images/cash_red.png">
                        </a>
                    </span>
                </template>
                <span class="botoes" v-if="pedido && pedido.permissoes">
                    <span>
                        <asp:Button ID="btnEmConferencia" runat="server" CommandArgument='<%# Eval("IdPedido") %>' OnClick="btnEmConferencia_Click" OnClientClick="if (!emConferencia()) return false;" Text="Em Conferência" Visible='<%# Eval("ConferenciaVisible") %>' Width="110px" />

                        <a href="#" @click.prevent="abrirTextosPedido" title="Textos Pedido">
                            <img border="0" src="../Images/note_add.gif">
                        </a>
                        <button @click.prevent="editar">
                            Editar
                        </button>
                        <button @click.prevent="finalizar">
                            Finalizar
                        </button>
                        <button @click.prevent="colocarEmConferencia" v-if="pedido.permissoes.colocarEmConferencia">
                            Em conferência
                        </button>
                        <span v-if="configuracoes.exibirBotoesConfirmacao">
                            <button @click.prevent="confirmarGerandoConferencia(false)">
                                Confirmar editando Conferência
                            </button>
                            <button @click.prevent="confirmarGerandoConferencia(true)">
                                Confirmar com Conferência finalizada
                            </button>
                        </span>
                    </span>
                </span>
            </section>
            <section v-if="pedidoMaoDeObra || !pedidoProducao">
                <div>
                    <span v-if="!pedidoMaoDeObra && !pedidoProducao">
                        <a href="#" v-on:click.prevent="incluirProjeto">
                            Incluir Projeto
                        </a>
                    </span>
                    <span v-if="pedidoMaoDeObra">
                        <a href="#" v-on:click.prevent="incluirVariosVidrosMaoDeObra">
                            Inserir várias peças de vidro com a mesma mão de obra
                        </a>
                    </span>
                </div>
            </section>
            <pedido-ambientes :pedido="pedido" :ambiente.sync="ambiente" :configuracoes="configuracoes" :pedido-mao-de-obra="pedidoMaoDeObra"></pedido-ambientes>
            <pedido-produtos :pedido="pedido" :ambiente="ambiente" :configuracoes="configuracoes" :pedido-mao-de-obra="pedidoMaoDeObra"
                :pedido-producao-corte="pedidoProducaoCorte" :pedido-mao-de-obra-especial="pedidoMaoDeObraEspecial" @lista-atualizada="atualizarPedidoEAmbientes"></pedido-produtos>
        </div>
    </div>
    <br /><br /><br />

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
                                                                        Text='<%# Bind("PercComissao") %>' Width="50px"></asp:TextBox>
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
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
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
                                    OnRowUpdated="grdProdutos_RowUpdated">
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
                                        <asp:TemplateField HeaderText="Perc.Comissão Prod." >
                                            <ItemTemplate>
                                                <asp:Label ID="lblPercComissao" runat="server" Text='<%# Eval("PercComissao") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtComissaoProd" runat="server" MaxLength="50" onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("PercComissao") %>' Width="50px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtComissaoProd" runat="server" onkeypress="return soNumeros(event, true, true)" MaxLength="50" Width="50px"></asp:TextBox>
                                            </FooterTemplate>
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
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
        InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoDAO"
        UpdateMethod="Update">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        </SelectParameters>
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
    <script type="text/javascript" src="<%= ResolveUrl("CadPedido.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript">
        inicializarControles();
    </script>

    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Pedidos/Componentes/CadPedido.Ambientes.js" />
            <asp:ScriptReference Path="~/Vue/Pedidos/Componentes/CadPedido.Produtos.js" />
            <asp:ScriptReference Path="~/Vue/Pedidos/Componentes/CadPedido.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
