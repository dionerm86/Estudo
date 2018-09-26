const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch],

  data: {
    dadosOrdenacao_: {
      campo: 'descricao',
      direcao: 'asc'
    },
    configuracoes: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    aplicacao: {}
  },

  methods: {
    /**
     * 
     * @param {any} filtro
     * @param {any} pagina
     * @param {any} numeroRegistros
     * @param {any} ordenacao
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Aplicacoes.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
    * Realiza a ordenação da lista de aplicacoes.
    * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
    */
    ordenar: function (campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

  },

  computed: {
    ordenacao: function () {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  mounted: function () {
    var vm = this;
  }
})
