Vue.component('checkbox-todos', {
  mixins: [Mixins.UUID],
  props: {
    /**
     * Identificador dos checkbox do controle pai que serão considerados para a busca.
     * @type {String}
     */
    idConsiderar: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarString
    },

    /**
     * Título a ser exibido para o checkbox.
     * @type {String}
     */
    titulo: {
      required: false,
      twoWay: false,
      default: 'Todos',
      validator: Mixins.Validacao.validarString
    }
  },

  data: function () {
    return {
      uuid: null,
      controles: null,
      todos: false
    };
  },

  methods: {
    /**
     * Recupera todos os checkbox que serão considerados para a indicação do controle 'Todos'.
     * @returns {Object[]}
     */
    obterTodosOsCheckboxControlePai: function (controlePai) {
      return controlePai.querySelectorAll('input[type=checkbox][id*=' + this.idConsiderar + ']');
    },

    /**
     * Prepara o controle para disparar um evento indicando que foi alterado.
     * @param {Object} controle O controle checkbox que será preparado para disparar o evento de troca de valor.
     */
    prepararControle: function (controle) {
      var vm = this;
      var original = controle.onchange;

      controle.onchange = function ($event) {
        vm.alterarTodos(original, $event);
      }
    },

    /**
     * Altera o valor do checkbox 'Todos' se todos os itens estiverem marcados.
     * @param {Object} original O evento original do controle.
     * @param {Object} event Objeto com os dados do evento JavaScript.
     */
    alterarTodos: function (original, event) {
      this.todos = this.selecionarTodos();

      if (original) {
        original(event);
      }
    },

    /**
     * Altera o valor dos itens selecionados de acordo com o checkbox 'Todos'.
     */
    selecionarItens: function () {
      var selecionar = this.todos;

      this.controles.forEach(function (item) {
        item.checked = selecionar;
      });
    },

    /**
     * Retorna um valor indicando se o checkbox 'Todos' deve estar selecionado.
     * @returns {Boolean} Verdadeiro, se o checkbox deve estar selecionado.
     */
    selecionarTodos: function () {
      return this.controles
        .every(function (item) {
          return item.checked;
        });
    }
  },

  mounted: function () {
    this.uuid = this.gerarUuid();
    this.controles = [...this.obterTodosOsCheckboxControlePai(this.$parent.$el)];

    this.controles.forEach(function (item) {
      this.prepararControle(item);
    }, this);

    this.todos = this.selecionarTodos();
  },

  template: '#CheckboxTodos-template'
});
