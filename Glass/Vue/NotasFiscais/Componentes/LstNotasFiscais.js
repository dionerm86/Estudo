const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'desc'
    },
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca as notas fiscais para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    atualizarNotasFiscais: function(filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.NotasFiscais.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista.
     * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
     */
    ordenar: function(campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

    /**
     * Reenvia email para o cliente com a nota fiscal.
     * @param {Object} item A nota fiscal que será enviada por email.
     * @param {?boolean} cancelamento Define se é para reenviar email de cancelamento.
     */
    reenviarEmail: function (item, cancelamento) {
      if (!this.perguntar('Reenviar e-mail', 'Deseja reenviar o e-mail do XML/DANFE?')) {
        return;
      }

      var vm = this;

      Servicos.NotasFiscais.reenviarEmail(item.id, cancelamento)
        .then(function (resposta) {
          vm.exibirMensagem('Reenvio de e-mail', 'E-mail adicionado na fila de envios.');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('numeroNfe', this.filtro.numeroNfe);
      incluirFiltro('idPedido', this.filtro.idPedido);
      incluirFiltro('modelo', this.filtro.modelo);
      incluirFiltro('idLoja', this.filtro.idLoja);
      incluirFiltro('idCliente', this.filtro.idCliente);
      incluirFiltro('nomeCliente', this.filtro.nomeCliente);
      incluirFiltro('tipoFiscal', this.filtro.tipoFiscal);
      incluirFiltro('idFornec', this.filtro.idFornecedor);
      incluirFiltro('nomeFornec', this.filtro.nomeFornecedor);
      incluirFiltro('codRota', this.filtro.codigoRota);
      incluirFiltro('situacao', this.filtro.situacao);
      incluirFiltro('dataIni', this.filtro.periodoEmissaoInicio ? this.filtro.periodoEmissaoInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataFim', this.filtro.periodoEmissaoFim ? this.filtro.periodoEmissaoFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('idsCfop', this.filtro.idsCfop);
      incluirFiltro('tiposCfop', this.filtro.tiposCfop);
      incluirFiltro('dataEntSaiIni', this.filtro.periodoEntradaSaidaInicio ? this.filtro.periodoEntradaSaidaInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataEntSaiFim', this.filtro.periodoEntradaSaidaFim ? this.filtro.periodoEntradaSaidaFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('formaPagto', this.filtro.tipoVenda);
      incluirFiltro('idsFormaPagtoNotaFiscal', this.filtro.idsFormaPagamento);
      incluirFiltro('tipoNf', this.filtro.tipoDocumento);
      incluirFiltro('finalidade', this.filtro.finalidade);
      incluirFiltro('formaEmissao', this.filtro.tipoEmissao);
      incluirFiltro('infCompl', this.filtro.informacaoComplementar);
      incluirFiltro('codInternoProd', this.filtro.codigoInternoProduto);
      incluirFiltro('descrProd', this.filtro.descricaoProduto);
      incluirFiltro('lote', this.filtro.lote);
      incluirFiltro('valorInicial', this.filtro.valorNotaFiscalInicio);
      incluirFiltro('valorFinal', this.filtro.valorNotaFiscalFim);
      incluirFiltro('ordenar', this.filtro.ordenacaoFiltro);
      incluirFiltro('agrupar', this.filtro.agrupar);

      return filtros.length > 0
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  computed: {
    /**
     * Propriedade computada que indica a ordenação para a lista.
     * @type {string}
     */
    ordenacao: function() {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.NotasFiscais.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
