<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadPedido.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadPedido" Title="Cadastrar Pedido" EnableEventValidation="false"
    EnableViewState="false" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Pedidos/Templates/CadPedido.Ambientes.html",
            "~/Vue/Pedidos/Templates/CadPedido.Produtos.html")
    %>
    <div id="app">
        <div v-if="editando || inserindo">
            <section class="edicao">
                <span class="cabecalho">
                    <label>
                        Cliente
                    </label>
                </span>
                <campo-busca-cliente :cliente.sync="clienteAtual" :exibir-informacoes-compra="true" tipo-validacao="Pedido"
                    :cor-texto-observacoes="configuracoes.corTextoObservacoesCliente" :disabled="!pedido.podeEditarCliente"
                    v-if="pedido" required></campo-busca-cliente>
                <span class="cabecalho">
                    <label>
                        Data Ped.
                    </label>
                </span>
                <span>
                    {{ pedido.dataPedido | data }}
                    <span v-if="configuracoes.exibirFastDelivery && configuracoes.marcarFastDelivery" style="margin-left: 8px">
                        <input id="fastDelivery" type="checkbox" v-model="pedido.fastDelivery" />
                        <label for="fastDelivery">
                            Fast Delivery
                        </label>
                    </span>
                </span>
                <span class="cabecalho">
                    <label>
                        Cód. Ped. Cli.
                    </label>
                </span>
                <span>
                    <input type="text" v-model="pedido.codigoPedidoCliente" :disabled="pedido && pedido.importado" :required="configuracoes.obrigarInformarPedidoCliente" />
                </span>
                <span class="cabecalho">
                    <label>
                        Orçamento
                    </label>
                </span>
                <span>
                    <input type="number" v-model="pedido.idOrcamento" style="width: 50px" />
                </span>
                <span class="cabecalho">
                    <label>
                        Loja
                    </label>
                </span>
                <span>
                    <lista-selecao-lojas :loja.sync="lojaAtual" :ativas="true" :exibir-todas="false" required
                        :disabled="!configuracoes.usarControleOrdemCarga && !configuracoes.alterarLojaPedido && !clientePermiteAlterarLoja" class="colspan2"></lista-selecao-lojas>
                    <span v-if="configuracoes.exibirDeveTransferir">
                        <input id="deveTransferir" type="checkbox" v-model="pedido.deveTransferir" :disabled="!clientePermiteAlterarLoja" />
                        <label for="deveTransferir">
                            Deve transferir?
                        </label>
                    </span>
                </span>
                <span class="cabecalho">
                    <label>
                        Tipo Pedido
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="tipoPedidoAtual" :funcao-recuperar-itens="obterTiposPedido" :disabled="(editando && configuracoes.bloquearItensTipoPedido) || pedidoMaoDeObra || pedidoProducao" required></lista-selecao-id-valor>
                    <span v-if="configuracoes.gerarPedidoProducaoCorte && pedido.tipo == configuracoes.tipoPedidoRevenda">
                        <input type="checkbox" id="gerarPedidoProducaoCorte" v-model="pedido.gerarPedidoCorte" />
                        <label for="gerarPedidoProducaoCorte">
                            Gerar pedido de produção para corte
                        </label>
                    </span>
                </span>
                <span class="cabecalho">
                    <label>
                        Tipo Venda
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="tipoVendaAtual" :funcao-recuperar-itens="obterTiposVendaCliente" :filtro-recuperar-itens="filtroTiposVendaCliente"
                        :disabled="editando && pedido.sinal && pedido.sinal.id > 0 && !configuracoes.usarLiberacaoPedido" required></lista-selecao-id-valor>
                    <span v-if="vIfTipoVendaObra">
                        <campo-busca-com-popup :id.sync="pedido.idObra" :nome.sync="descricaoObraAtual" campo-nome="descricao" :item-selecionado.sync="obraAtual" :funcao-buscar-itens="obterObras" :disabled="!clienteAtual || clienteAtual.id == 0"
                            :url-popup="'/Utils/SelObra.aspx?situacao=4&tipo=1&idsPedidosIgnorar=' + (pedido.id || '') + '&idCliente=' + (clienteAtual.id || '')" :largura-popup="650" :altura-popup="560" style="width: 90px" required></campo-busca-com-popup>
                        <template v-if="obraAtual">
                            Saldo da Obra: {{ obraAtual.saldo | moeda }}
                            Saldo Pedidos Abertos: {{ obraAtual.totalPedidosAbertosObra | moeda }}
                            Saldo Atual: {{ obraAtual.saldo - obraAtual.totalPedidosAbertosObra | moeda }}
                        </template>
                    </span>
                    <span v-if="vIfNumeroParcelas">
                        <label>
                            Num. Parcelas
                        </label>
                        <lista-selecao-parcelas :parcela.sync="parcelaAtual" :id-cliente="pedido.idCliente" :pode-editar="configuracoes.permitirAlterarDataParcelas" required></lista-selecao-parcelas>
                    </span>
                </span>
                <template v-if="pedido && pedido.tipoVenda == configuracoes.tipoVendaFuncionario">
                    <span class="cabecalho">
                        <label>
                            Funcionário comprador
                        </label>
                    </span>
                    <span>
                        <lista-selecao-id-valor :item-selecionado.sync="funcionarioCompradorAtual" :funcao-recuperar-itens="obterFuncionariosCompradores" required></lista-selecao-id-valor>
                    </span>
                </template>
                <span class="cabecalho">
                    <label>
                        Tipo Entrega
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="tipoEntregaAtual" :funcao-recuperar-itens="obterTiposEntrega" required></lista-selecao-id-valor>
                </span>
                <template v-if="vIfFormaPagamento">
                    <span class="cabecalho">
                        <label>
                            Forma Pagto.
                        </label>
                    </span>
                    <span>
                        <lista-selecao-id-valor :item-selecionado.sync="formaPagamentoAtual" :funcao-recuperar-itens="obterFormasPagamento" :filtro-recuperar-itens="filtroFormasPagamento" required></lista-selecao-id-valor>
                        <lista-selecao-id-valor :item-selecionado.sync="tipoCartaoAtual" :funcao-recuperar-itens="obterTiposCartao" v-if="pedido.formaPagamento.id == configuracoes.idFormaPagamentoCartao" required></lista-selecao-id-valor>
                    </span>
                </template>
                <span class="cabecalho">
                    <label>
                        Data Entrega
                    </label>
                </span>
                <campo-data-hora :data-hora.sync="pedido.entrega.data" :data-minima="dataEntregaMinima"
                    :permitir-feriado="false" :permitir-fim-de-semana="false"
                    :disabled="datasEntrega && datasEntrega.desabilitarCampo" required></campo-data-hora>
                <template v-if="configuracoes.exibirValorFretePedido">
                    <span class="cabecalho">
                        <label>
                            Valor do Frete
                        </label>
                    </span>
                    <span>
                        <input type="number" v-model.number="pedido.valorEntrega" style="width: 80px;" />
                    </span>
                </template>
                <template v-if="vIfValorEntrada">
                    <span class="cabecalho">
                        <label>
                            Valor Entrada
                        </label>
                    </span>
                    <span v-if="vIfCampoEntrada" key="campoEntrada">
                        <input type="number" v-model.number="pedido.valorEntrada" />
                    </span>
                    <span v-else key="campoEntrada">
                        {{ pedido.textoSinal }}
                    </span>
                </template>
                <span class="cabecalho">
                    <label>
                        Desconto
                    </label>
                </span>
                <span v-if="vIfCampoDesconto">
                    <campo-acrescimo-desconto :tipo.sync="pedido.desconto.tipo" :valor.sync="pedido.desconto.valor" :disabled="disabledCampoDesconto"></campo-acrescimo-desconto>
                </span>
                <span style="color: blue" v-else>
                    Desconto só pode ser dado em pedidos à vista
                </span>
                <span class="cabecalho">
                    <label>
                        Total
                    </label>
                </span>
                <span>
                    {{ pedido.total | moeda }}
                </span>
                <span class="cabecalho">
                    <label>
                        Acréscimo
                    </label>
                </span>
                <span>
                    <campo-acrescimo-desconto :tipo.sync="pedido.acrescimo.tipo" :valor.sync="pedido.acrescimo.valor"></campo-acrescimo-desconto>
                </span>
                <span class="cabecalho">
                    <label>
                        Funcionário
                    </label>
                </span>
                <span>
                    <lista-selecao-id-valor :item-selecionado.sync="vendedorAtual" :funcao-recuperar-itens="obterVendedores" :disabled="configuracoes.alterarVendedor" @change.prevent="alterarVendedor"></lista-selecao-id-valor>
                </span>
                <span v-if="vIfAjusteLayoutTransportador" class="colspan2">
                </span>
                <span class="cabecalho">
                    <label>
                        Transportador
                    </label>
                </span>
                <span class="colspan3">
                    <lista-selecao-id-valor :item-selecionado.sync="transportadorAtual" :funcao-recuperar-itens="obterTransportadores"></lista-selecao-id-valor>
                </span>
                <template v-if="pedido && pedido.entrega.tipo && pedido.entrega.tipo.id != configuracoes.tipoEntregaBalcao">
                    <span class="cabecalho">
                        <label>
                            Local da Obra
                        </label>
                    </span>
                    <span class="colspan3">
                        <button @click.prevent="preencherEnderecoObra">
                            <img src="../Images/home.gif" title="Buscar endereço do cliente" />
                        </button>
                        <campo-endereco :endereco="pedido.enderecoObra" required>
                    </span>
                </template>
                <span class="colspan4" style="padding: 8px 0" v-if="vIfControleParcelas">
                    <controle-parcelas :parcelas.sync="parcelaAtual" :data-minima="pedido.dataPedido" :total="totalParaCalculoParcelas"></controle-parcelas>
                </span>
                <template v-if="configuracoes.usarComissaoNoPedido && !configuracoes.usarComissionadoDoCliente">
                    <span class="cabecalho">
                        <label>
                            Comissionado:
                        </label>
                    </span>
                    <span class="colspan3">
                        <campo-busca-com-popup :id.sync="pedido.comissionado.id" campo-nome="descricao" :item-selecionado.sync="comissionadoAtual" :funcao-buscar-itens="obterComissionados"
                            :url-popup="'/Utils/SelComissionado.aspx'" :largura-popup="760" :altura-popup="590" style="width: 90px" required></campo-busca-com-popup>
                        <label>
                            Percentual:
                        </label>
                        <input type="number" v-model="pedido.percentualComissao" :disabled="configuracoes.alterarPercentualComissionado" />
                        <label>
                            Valor comissão:
                        </label>
                        <span>
                            {{ pedido.valorComissao | moeda }}
                        </span>
                    </span>
                </template>
                <template v-if="configuracoes.usarControleMedicao">
                    <span class="cabecalho">
                        <label>
                            Medidor
                        </label>
                    </span>
                    <span class="colspan3">
                        <lista-selecao-id-valor :item-selecionado.sync="medidorAtual" :funcao-recuperar-itens="obterMedidores"></lista-selecao-id-valor>
                    </span>
                </template>
                <span class="cabecalho">
                    <label>
                        Observação
                    </label>
                </span>
                <span class="colspan3">
                    <textarea v-model="pedido.observacao" style="width: 650px"></textarea>
                </span>
                <template v-if="configuracoes.usarLiberacaoPedido">
                    <span class="cabecalho">
                        <label>
                            Observação liberação
                        </label>
                    </span>
                    <span class="colspan3">
                        <textarea v-model="pedido.observacaoLiberacao" style="width: 650px"></textarea>
                    </span>
                </template>
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
                <label>
                    Num. Pedido
                </label>
                <span>
                    <span style="font-size: medium">
                        {{ pedido.id }}
                    </span>
                    <span style="color: green" v-if="pedido && pedido.tipo">
                        ({{ pedido.tipo.nome }})
                    </span>
                </span>
                <label>
                    Cliente
                </label>
                <span class="colspan3" v-if="pedido && pedido.cliente">
                    {{ pedido.cliente.id }}
                    -
                    {{ pedido.cliente.nome }}
                </span>
                <label>
                    Funcionário
                </label>
                <span v-if="pedido && pedido.vendedor">
                    {{ pedido.vendedor.nome }}
                </span>
                <label>
                    Tel. Cliente
                </label>
                <span v-if="pedido && pedido.cliente">
                    {{ pedido.cliente.telefone }}
                </span>
                <label>
                    Loja
                </label>
                <span v-if="pedido && pedido.loja">
                    {{ pedido.loja.nome }}
                </span>
                <label>
                    Endereço Cliente
                </label>
                <span v-if="pedido && pedido.cliente" class="colspan5">
                    {{ pedido.cliente.endereco }}
                </span>
                <template v-if="pedido.obra">
                    <label>
                        Endereço Obra
                    </label>
                    <span class="colspan5">
                        {{ pedido.obra.endereco }}
                    </span>
                </template>
                <template v-if="pedido && pedido.sinal">
                    <label>
                        Valor Entrada
                    </label>
                    <span>
                        {{ pedido.sinal.valor }}
                    </span>
                </template>
                <label>
                    Tipo Venda
                </label>
                <span v-if="pedido && pedido.tipoVenda">
                    {{ pedido.tipoVenda.nome }}
                </span>
                <label>
                    Tipo Entrega
                </label>
                <span v-if="pedido && pedido.entrega && pedido.entrega.tipo">
                    {{ pedido.entrega.tipo.nome }}
                </span>
                <label>
                    Situação
                </label>
                <span v-if="pedido && pedido.situacao">
                    {{ pedido.situacao.nome }}
                </span>
                <label>
                    Data Ped.
                </label>
                <span v-if="pedido">
                    {{ pedido.dataPedido | data }}
                </span>
                <label>
                    Data Entrega
                </label>
                <span v-if="pedido && pedido.entrega">
                    {{ pedido.entrega.data | data }}
                </span>
                <span v-else></span>
                <label>
                    Valor do Frete
                </label>
                <span v-if="pedido && pedido.entrega">
                    {{ pedido.entrega.valor | moeda }}
                </span>
                <span v-else>
                    {{ 0 | moeda }}
                </span>
                <label>
                    Desconto
                </label>
                <span v-if="pedido && pedido.desconto && pedido.desconto.valor">
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
                <label>
                    Comissão
                </label>
                <span v-if="pedido && pedido.comissionado && pedido.comissionado.comissao">
                    {{ pedido.comissionado.comissao.valor | moeda }}
                </span>
                <span v-else>
                    {{ 0 | moeda }}
                </span>
                <template v-if="configuracoes.calcularIcms">
                    <label>
                        Valor ICMS
                    </label>
                    <span v-if="pedido && pedido.icms">
                        {{ pedido.icms.valor | moeda }}
                    </span>
                    <span v-else>
                        {{ 0 | moeda }}
                    </span>
                </template>
                <template v-if="configuracoes.calcularIpi">
                    <label>
                        Valor IPI
                    </label>
                    <span v-if="pedido && pedido.ipi">
                        {{ pedido.ipi.valor | moeda }}
                    </span>
                    <span v-else>
                        {{ 0 | moeda }}
                    </span>
                </template>
                <span class="colspan6" v-if="!configuracoes.empresaNaoVendeVidro">
                    <label>
                        Total
                    </label>
                    <span style="color: #0000CC">
                        {{ pedido.total | moeda }}
                    </span>
                </span>
                <span class="colspan6" v-else>
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
                <label>
                    Forma Pagto.
                </label>
                <span v-if="pedido && pedido.formaPagamento">
                    {{ pedido.formaPagamento.nome }}
                </span>
                <template v-if="configuracoes.exibirFastDelivery">
                    <label>
                        Fast Delivery
                    </label>
                    <span v-if="pedido && pedido.fastDelivery">
                        {{ pedido.fastDelivery.aplicado | simNao }}
                    </span>
                </template>
                <template v-if="configuracoes.exibirDeveTransferir">
                    <label>
                        Deve transferir?
                    </label>
                    <span v-if="pedido">
                        {{ pedido.deveTransferir | simNao }}
                    </span>
                </template>
                <template v-if="pedido && pedido.funcionarioComprador">
                    <label>
                        Funcionário comp.
                    </label>
                    <span>
                        {{ pedido.funcionarioComprador.nome }}
                    </span>
                </template>
                <template v-if="pedido && pedido.transportador">
                    <label>
                        Transportador
                    </label>
                    <span>
                        {{ pedido.transportador.nome }}
                    </span>
                </template>
                <span v-if="pedido && (!configuracoes.exibirDeveTransferir || !pedido.funcionarioComprador || !pedido.transportador)" class="colspan2"></span>
                <label>
                    Observação
                </label>
                <span class="colspan5" v-if="pedido" style="color: blue">
                    {{ pedido.observacao }}
                </span>
                <label>
                    Obs. do Cliente
                </label>
                <span class="colspan5" v-if="pedido && pedido.cliente" style="color: red"
                    v-html="pedido.cliente.observacao">
                </span>
                <template v-if="configuracoes.exibirRentabilidade && pedido && pedido.rentabilidade">
                    <label>
                        Rentabilidade
                    </label>
                    <span>
                        {{ pedido.rentabilidade.percentual | percentual }}
                    </span>
                    <label>
                        Rent. Financeira
                    </label>
                    <span>
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

    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Pedidos/Componentes/CadPedido.Ambientes.js" />
            <asp:ScriptReference Path="~/Vue/Pedidos/Componentes/CadPedido.Produtos.js" />
            <asp:ScriptReference Path="~/Vue/Pedidos/Componentes/CadPedido.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
