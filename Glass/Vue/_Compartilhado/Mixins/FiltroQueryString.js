var Mixins = Mixins || {};

/**
 * Objeto com o mixin para formatação de filtros para querystring.
 */
Mixins.FiltroQueryString = {
  methods: {
    /**
     * Formata os campos de um objeto de filtro para utilização em uma querystring.
     * @param {Object} filtro O fitro que será formatado.
     * @returns {String} Uma string para utilização em uma querystring.
     */
    formatarFiltro: function (filtro) {
      var filtros = [];

      if (filtro) {
        for (var campo of Object.keys(filtro)) {
          var item = this.incluirFiltro__(campo, filtro[campo]);

          if (item) {
            filtros.push(item);
          }
        }
      }

      return filtros.join('&');
    },

    /**
     * Inclui um filtro na lista para formatação.
     * @param {String} nome O nome do campo para a querystring.
     * @param {Object} valor O valor do filtro.
     * @returns {?String} Uma string com o valor formatado, se o valor for válido.
     */
    incluirFiltro: function (nome, valor) {
      if (valor) {
        valor = this.formatarValor__(valor);
        return nome + '=' + valor;
      }

      return null;
    },

    /**
     * Formata um valor para utilização no filtro, de acordo com seu tipo.
     * @param {Object} valor O valor que será formatado.
     * @returns {?String} O valor formatado.
     */
    formatarValor__: function (valor) {
      if (valor instanceof Date) {
        return valor.toLocaleDateString('pt-BR');
      }

      if (typeof valor === 'number' && valor % 1 !== 0) {
        return Formatacao.decimal.format(valor);
      }

      return valor;
    }
  }
};
