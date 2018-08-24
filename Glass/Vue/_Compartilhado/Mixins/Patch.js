var Mixins = Mixins || {};

/**
 * Objeto de mixin com o método para realização de merge para patch.
 */
Mixins.Patch = {
  methods: {
    /**
     * Função que computa a diferença entre objetos para a realização de evento PATCH.
     * Essa função destaca apenas os itens alterados para envio à API.
     * @param {Object} alterado O objeto com as alterações realizadas.
     * @param {Object} original O objeto com os dados originais.
     * @returns {Object} O objeto com as diferenças entre o objeto alterado e o original.
     */
    patch: function(alterado, original) {
      const verificarEVazio = item => item === null || item === undefined || item === '';
      var destino = Array.isArray(alterado) ? [] : {};

      if (!original) {
        original = {};
      }

      var listaCampos = Object.keys(alterado)
        .concat(Object.keys(original))
        .filter(function(item, index, array) {
          return array.indexOf(item) === index;
        });

      for (var indice in listaCampos) {
        var campo = listaCampos[indice];

        if (alterado[campo] === original[campo]) {
          continue;
        }

        if (typeof alterado[campo] === 'object' && !verificarEVazio(alterado[campo]) && !(alterado[campo] instanceof Date)) {
          var alteracoes = this.patch(alterado[campo], original[campo]);
          if (Object.keys(alteracoes).length > 0) {
            destino[campo] = alteracoes;
          }
        } else {
          destino[campo] = alterado[campo];
        }
      }

      return destino;
    }
  }
};