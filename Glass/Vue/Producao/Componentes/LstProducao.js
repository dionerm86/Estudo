const app = new Vue({
  el: '#app',
  mixins: [Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    filtro: {},
    configuracoes: {},
    agruparImpressao: null
  },

  methods: {

  },

  mounted: function () {
    var vm = this;

    Servicos.Producao.obterConfiguracoesConsulta()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
