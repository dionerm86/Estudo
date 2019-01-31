Vue.component('arquivos-remessa-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de arquivos de remessa.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          id: null,
          numeroArquivoRemessa: null,
          tipo: null,
          idContaBanco: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null
        },
        this.filtro
      ),
      tipoAtual: null,
      contaBancariaAtual: null
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function () {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Retorna os itens para o controle de tipos de arquivos de remessa.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTipos: function () {
      return Servicos.ArquivosRemessa.obterTipos();
    },

    /**
     * Retorna os itens para o controle de contas bancárias.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroContasBancarias: function () {
      return Servicos.ContasBancarias.obterParaControle();
    },
  },

  watch: {
    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        this.filtroAtual.tipo = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'contaBancariaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    contaBancariaAtual: {
      handler: function (atual) {
        this.filtroAtual.idContaBanco = atual ? atual.id : null;
      },
      deep: true
    },
  },

  template: '#LstArquivosRemessa-Filtro-template'
});
