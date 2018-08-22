var Mixins = Mixins || {};

/**
 * Objeto com as funções comuns para envio de objetos via querystring.
 */
Mixins.JsonQuerystring = {
  methods: {
    /**
     * Codifica um objeto em uma string para ser enviada via querystring.
     * @param {?object} objeto
     * @returns {?string} Uma string com o objeto serializado e codificado para envio via querystring.
     */
    codificarJson: function (objeto) {
      if (!objeto || typeof objeto !== 'object' || objeto instanceof Date || objeto instanceof Array) {
        return null;
      }

      var stringJson = JSON.stringify(objeto);
      var stringJsonTratada = encodeURIComponent(stringJson);
      var stringBase64 = btoa(stringJsonTratada);

      return stringBase64;
    }
  }
}
