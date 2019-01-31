Vue.component('acertos-cheques-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de acertos de cheques.
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
          idFuncionario: null,
          idCliente: null,
          nomeCliente: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
        },
        this.filtro
      ),
      funcionarioAtual: null
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
     * Retorna os funcionários para controle de funcionários da tela de acertos de cheques.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFuncionario: function () {
      return Servicos.Funcionarios.obterFinanceiros();
    }
  },

  watch: {
    /**
     * Observador para a variável 'funcionarioAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    funcionarioAtual: {
      handler: function (atual) {
        this.filtroAtual.idFuncionario = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstAcertosCheques-Filtro-template'
});