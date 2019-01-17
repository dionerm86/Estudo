Vue.component('arquivos-otimizacao-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de arquivos de otimização.
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
          idFuncionario: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          direcao: null,
          idPedido: null,
          codigoEtiqueta: null
        },
        this.filtro
      ),
      funcionarioAtual: null,
      direcaoAtual: null
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
     * Retorna os itens para o controle de funcionários.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroIdFuncionarios: function () {
      return Servicos.Funcionarios.obterParaControleArquivosOtimizacao();
    },

    /**
     * Retorna os itens para o controle de direções.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroDirecoes: function () {
      return Servicos.ArquivosOtimizacao.obterDirecoes();
    },
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
    },

    /**
     * Observador para a variável 'direcaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    direcaoAtual: {
      handler: function (atual) {
        this.filtroAtual.direcao = atual ? atual.id : null;
      },
      deep: true
    },
  },

  template: '#LstArquivosOtimizacao-Filtro-template'  
});
