Vue.component('lista-selecao-funcionarios', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Funcionário selecionado.
     * @type {?object}
     */
    funcionario: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Indica o tipo de funcionário que será recuperado.
     * @type {string}
     */
    tipo: {
      required: false,
      default: 'Todos',
      validator: Mixins.Validacao.validarValores('Todos', 'Vendedores')
    }
  },

  methods: {
    /**
     * Recupera os funcionários para exibição no controle.
     * @return {Promise} Uma Promise com o resultado da busca de lojas.
     */
    buscarFuncionarios: function() {
      return Servicos.Funcionarios.obterParaControle(this.tipo);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o funcionário selecionado e que
     * atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    funcionarioAtual: {
      get: function() {
        return this.funcionario;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.funcionario)) {
          this.$emit('update:funcionario', valor);
        }
      }
    }
  },

  template: '#ListaSelecaoFuncionarios-template'
});
