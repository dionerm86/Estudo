Vue.prototype.$sistemaOcupado = new Vue({
  data: {
    processos: 0,
    bloqueios: 0
  },

  created: function () {
    this.$on('processo', function (indicador) {
      var minimo = Math.max(0, indicador);
      var novo = Math.max(minimo, this.processos + indicador);
      this.processos = novo;

      this.$emit('atualizar-processos', this.processos);
    });

    this.$on('bloqueio', function (indicador) {
      var minimo = Math.max(0, indicador);
      var novo = Math.max(minimo, this.bloqueios + indicador);
      this.bloqueios = novo;

      this.$emit('atualizar-bloqueios', this.bloqueios);
    });
  }
});

/**
 * Objeto com o mixin para indicação de processamento do sistema.
 */
Vue.mixin({
  methods: {
    /**
     * Função para exibição de um indicativo de processamento do sistema (sem bloqueio de tela).
     */
    indicarProcessamento: function () {
      this.$sistemaOcupado.$emit('processo', 1)
    },

    /**
     * Função para remoção do indicativo de processamento do sistema (sem bloqueio de tela).
     */
    finalizarProcessamento: function() {
      this.$sistemaOcupado.$emit('processo', -1)
    },

    /**
     * Função para exibição de um indicativo de processamento do sistema (com bloqueio de tela).
     */
    indicarBloqueio: function () {
      this.$sistemaOcupado.$emit('bloqueio', 1)
    },

    /**
     * Função para remoção do indicativo de processamento do sistema (com bloqueio de tela).
     */
    finalizarBloqueio: function () {
      this.$sistemaOcupado.$emit('bloqueio', -1)
    }
  }
});
