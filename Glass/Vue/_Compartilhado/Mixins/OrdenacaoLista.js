var Mixins = Mixins || {};

/**
 * Objeto com o mixin para ordenação de listas paginadas.
 */
Mixins.OrdenacaoLista = function (campo, direcao) {
  return {
    data: function () {
      return {
        dadosOrdenacao__: {
          campo: campo || '',
          direcao: direcao || ''
        }
      }
    },

    methods: {
      /**
       * Realiza a ordenação da lista de pedidos.
       * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
       */
      ordenar: function (campo) {
        if (campo !== this.dadosOrdenacao__.campo) {
          this.dadosOrdenacao__.campo = campo;
          this.dadosOrdenacao__.direcao = '';
        } else {
          this.dadosOrdenacao__.direcao = this.dadosOrdenacao__.direcao === '' ? 'desc' : '';
        }
      }
    },

    computed: {
      /**
       * Propriedade computada que indica a ordenação para a lista.
       * @type {string}
       */
      ordenacao: function () {
        var direcao = this.dadosOrdenacao__.direcao ? ' ' + this.dadosOrdenacao__.direcao : '';
        return this.dadosOrdenacao__.campo + direcao;
      }
    }
  }
};
