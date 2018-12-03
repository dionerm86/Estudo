Vue.component('notafiscal-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de notas fiscais.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function() {
    return {
      filtroAtual: this.merge(
        {
          numeroNfe: null,
          idPedido: null,
          modelo: null,
          idLoja: null,
          idCliente: null,
          nomeCliente: null,
          tipoFiscal: null,
          idFornecedor: null,
          nomeFornecedor: null,
          codigoRota: null,
          situacao: null,
          periodoEmissaoInicio: null,
          periodoEmissaoFim: null,
          idsCfop: null,
          tiposCfop: null,
          periodoEntradaSaidaInicio: null,
          periodoEntradaSaidaFim: null,
          tipoVenda: null,
          idsFormaPagamento: null,
          tipoDocumento: null,
          finalidade: null,
          tipoEmissao: null,
          informacaoComplementar: null,
          codigoInternoProduto: null,
          descricaoProduto: null,
          lote: null,
          valorNotaFiscalInicio: null,
          valorNotaFiscalFim: null,
          apenasNotasFiscaisSemAnexo: false,
          ordenacaoFiltro: null,
          agrupar: 0
        },
        this.filtro
      ),
      rotaAtual: null,
      lojaAtual: null,
      vencimentoCertificado: null
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function() {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Retorna os itens para o controle de cfop.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroCfops: function() {
      return Servicos.Cfops.obterFiltro();
    },

    /**
     * Retorna os itens para o controle de tipos de cfop.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposCfop: function () {
      return Servicos.Cfops.obterTiposCfop();
    },

    /**
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotas: function (id, codigo) {
      return Servicos.Rotas.obterFiltro(id, codigo);
    },

    /**
     * Retorna os itens para o controle de formas de pagamento de nota fiscal.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFormasPagamentoNotaFiscal: function () {
      return Servicos.FormasPagamento.obterFiltroNotaFiscal();
    },

    /**
     * Retorna os itens para o controle de situações de nota fiscal.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoes: function () {
      return Servicos.NotasFiscais.obterSituacoes();
    },

    /**
     * Retorna os itens para o .
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterVencimentoCertificado: function () {
      var vm = this;

      Servicos.Lojas.obterDataVencimentoCertificado(vm.lojaAtual.id)
        .then(function (resposta) {
          if (resposta.dataVencimento != null){
            if (resposta.vencido){
              vm.vencimentoCertificado = 'Falta(m) ' + resposta.diasParaVencimento + ' dia(s) para a data de vencimento do Certificado';
            } else {
              vm.vencimentoCertificado = 'ATENÇÃO: Certificado vencido desde o dia ' + resposta.dataVencimento;
            }
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    }
  },

  mounted: function () {
    this.filtroAtual.agrupar = 0;
    this.filtroAtual.ordenacaoFiltro = 3;
  },

  watch: {
    /**
     * Observador para a variável 'rotaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    rotaAtual: {
      handler: function (atual) {
        this.filtroAtual.codigoRota = atual ? atual.nome : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function (atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
        this.obterVencimentoCertificado();
      },
      deep: true
    }
  },

  template: '#LstNotasFiscais-Filtro-template'
});
