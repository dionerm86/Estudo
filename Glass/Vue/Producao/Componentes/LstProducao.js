const app = new Vue({
  el: '#app',
  mixins: [Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    filtro: {},
    configuracoes: {}
  },

  methods: {

  }
});
