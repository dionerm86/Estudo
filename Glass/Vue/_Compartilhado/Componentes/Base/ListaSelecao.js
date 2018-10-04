Vue.component('lista-selecao', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Item selecionado no controle.
     * @type {?object}
     */
    itemSelecionado: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Função utilizada para a recuperação dos itens para o controle.
     * @type {!function}
     * @param {object} filtro O filtro utilizado para a recuperação dos dados.
     * @returns {Promise}
     */
    funcaoRecuperarItens: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncao
    },

    /**
     * Filtro utilizado para a execução da função de recuperação dos itens para o controle.
     * @type {?object}
     */
    filtroRecuperarItens: {
      required: false,
      twoWay: false,
      default: function () {
        return {}
      },
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Texto exibido caso não exista item selecionado.
     * @type {?string}
     */
    textoSelecionar: {
      required: false,
      twoWay: false,
      default: '',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Indica se os itens devem ser ordenados para exibição.
     * @type {?boolean}
     */
    ordenar: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Nome do campo do item selecionado que contém o ID.
     * @type {?string}
     */
    campoId: {
      required: false,
      twoWay: false,
      default: 'id',
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  data: function() {
    return {
      idAtual: (this.itemSelecionado || {})[this.campoId] || null,
      itens: []
    };
  },

  methods: {
    /**
     * Função que busca os itens do controle e os atribui à variável interna.
     */
    buscar: function() {
      var vm = this;

      this.funcaoRecuperarItens(this.filtroRecuperarItens)
        .then(function(resposta) {
          if (vm.ordenar) {
            resposta.data.sort(function(a, b) {
              return a.nome.localeCompare(b.nome);
            });
          }

          vm.itens = resposta.data;
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          vm.itens = [];
        });
    },

    /**
     * Método de tratamento para alteração do item selecionado no controle.
     */
    alterarSelecao: function () {
      var selecionado = this.itens.filter(function (item) {
        return item[this.campoId] === this.idAtual;
      }, this);

      selecionado = selecionado[0] || null;

      if (!this.equivalentes(selecionado, this.itemSelecionado)) {
        this.$emit('update:itemSelecionado', selecionado);
      }
    }
  },

  mounted: function() {
    this.buscar();
  },

  watch: {
    /**
     * Observador para a propriedade 'filtroRecuperarItens'.
     * Reexecuta a recuperação de itens no caso do filtro ser alterado.
     */
    filtroRecuperarItens: {
      handler: function () {
        this.buscar();
      },
      deep: true
    },

    /**
     * Observador para a propriedade 'itemSelecionado'.
     * Altera o ID atual quando houver mudança no item selecionado.
     */
    itemSelecionado: {
      handler: function (atual) {
        this.idAtual = atual ? atual[this.campoId] : null;
      },
      deep: true
    }
  },

  template: '#ListaSelecao-template'
});
