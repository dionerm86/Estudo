const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar],

  data: {
    dadosOrdenacao_: {
      campo: 'descricao',
      direcao: 'asc'
    },
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false
  },

  methods: {
    /**
     * Busca a lista de processos de etiqueta para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Processos.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de processos.
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

    /**
     * Cria a descrição para os tipos de pedidos que serão exibidos na listagem.
     * @param {Object} processo O objeto com o processo que será exibido.
     * @returns {string} O texto com a descrição dos tipos de pedidos.
     */
    obterDescricaoTiposPedidos: function (processo) {
      if (!processo.tiposPedidos || !processo.tiposPedidos.length) {
        return null;
      }

      var tiposPedidos = processo.tiposPedidos.slice()
        .map(function (item) {
          return item.nome;
        })
        .filter(function (item) {
          return !!item;
        });

      tiposPedidos.sort();
      return tiposPedidos.join(', ');
    },

    editar: function (processo, numeroLinha) {
      this.numeroLinhaEdicao = numeroLinha;
    },

    excluir: function () {

    }
  },

  computed: {
    /**
     * Propriedade computada que indica a ordenação para a lista.
     * @type {string}
     */
    ordenacao: function () {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Processos.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
