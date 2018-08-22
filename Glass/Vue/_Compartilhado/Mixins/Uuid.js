var Mixins = Mixins || {};

/**
 * Objeto que contém a lógica para geração de UUID para os controles.
 */
Mixins.UUID = {
  methods: {
    /**
     * Função que cria um UUID (identificador único).
     * @returns {string} Uma string com o UUID gerado.
     * @see https://stackoverflow.com/a/2117523
     */
    gerarUuid: function () {
      return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, function (c) {
        return (c ^ (crypto.getRandomValues(new Uint8Array(1))[0] & (15 >> (c / 4)))).toString(16);
      });
    }
  }
}
