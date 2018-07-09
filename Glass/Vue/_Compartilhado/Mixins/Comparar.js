var Mixins = Mixins || {};

/**
 * Objeto com o mixin para realização de comparação entre os objetos.
 */
Mixins.Comparar = {
  methods: {
    /**
     * Função que realiza a comparação entre dois objetos.
     * @param {Object} origem O objeto de origem.
     * @param {Object} destino O objeto de destino.
     * @returns {boolean} Um valor que indica se os objetos são equivalentes.
     */
    equivalentes: function(origem, destino) {
      if (origem === destino) {
        return true;
      }

      if ((!origem && destino) || (origem && !destino) || typeof origem !== typeof destino) {
        return origem !== origem && destino !== destino;
      }

      var listaCampos = Object.keys(origem)
        .concat(Object.keys(destino))
        .filter(function(item, index, array) {
          return array.indexOf(item) === index;
        });

      for (var i in listaCampos) {
        var campo = listaCampos[i];
        var objeto = typeof origem[campo] === 'object' && !(origem[campo] instanceof Date);

        if ((!objeto && origem[campo] !== destino[campo]) || (objeto && !this.equivalentes(origem[campo], destino[campo]))) {
          return false;
        }
      }

      return true;
    }
  }
};
