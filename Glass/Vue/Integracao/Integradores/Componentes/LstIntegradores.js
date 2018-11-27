const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos],

  data: {
    integradores: []
  },

  methods: {
    /**
     * Busca os integradores para exibição na lista.
     * @returns {Promise} Uma promise com o resultado da busca de integradores.
     */
    obterLista: function () {
      return Servicos.Integracao.Integradores.obterLista();
    },

    /**
     * Carrega os integradores.
     **/
    carregarIntegradores: function() {
      var self = this;
      this.obterLista()
        .then(function(resposta) {
          self.integradores = resposta.data || [];
        });
    }
  },

  mounted: function() {
    var vm = this;

    vm.carregarIntegradores();
  }
});
