const app = new Vue({
  el: '#app',
  mixins: [Mixins.OrdenacaoLista()],

  data: {
    filtro: {},
    configuracoes: {}
  }
});
