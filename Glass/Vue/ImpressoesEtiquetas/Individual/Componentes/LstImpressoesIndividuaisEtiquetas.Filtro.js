Vue.component('impressoes-individuais-etiquetas-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de impressões individuais de etiquetas.
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
          idPedido: null,
          numeroNotaFiscal: null,
          codigoEtiqueta: null,
          descricaoProduto: null,
          alturaInicio: null,
          alturaFim: null,
          larguraInicio: null,
          larguraFim: null,
          codigoProcesso: null,
          codigoAplicacao: null
        },
        this.filtro
      ),
      processoAtual: null,
      aplicacaoAtual: null
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
     * Retorna os itens para o controle de processos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroProcessos: function () {
      return Servicos.Processos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de aplicações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroAplicacoes: function () {
      return Servicos.Aplicacoes.obterParaControle();
    }
  },

  watch: {
    /**
     * Observador para a variável 'processoAtual'.
     * Atualiza o filtro com o Código do item selecionado.
     */
    processoAtual: {
      handler: function (atual) {
        this.filtroAtual.codigoProcesso = atual ? atual.codigo : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'aplicacaoAtual'.
     * Atualiza o filtro com o Código do item selecionado.
     */
    aplicacaoAtual: {
      handler: function (atual) {
        this.filtroAtual.codigoAplicacao = atual ? atual.codigo : null;
      },
      deep: true
    }
  },

  template: '#LstImpressoesIndividuaisEtiquetas-Filtro-template'
});
