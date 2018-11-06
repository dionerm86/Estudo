var Mixins = Mixins || {};

/**
 * Objeto com o mixin para com métodos para manipulação de objetos.
 */
Mixins.Objetos = {
  methods: {
    /**
     * Função que realiza o merge entre dois objetos, copiando os dados preenchidos de um objeto para outro.
     * @param {Object} destino O objeto de destino, que será retornado com os dados preenchidos.
     * @param {Object} origem O objeto com os dados preenchidos.
     * @returns {Object} O novo objeto, que contém a destino informada e com os dados do objeto origem.
     */
    merge: function(destino, origem) {
      if (!origem) {
        throw new Error('Dados de origem são obrigatórios para a função merge.');
      }

      destino = destino || (Array.isArray(origem) ? [] : {});

      const verificarEVazio = function (item) {
        return item === null || item === undefined || item === '';
      };

      for (var campo of Object.keys(origem)) {
        if (typeof origem[campo] === 'object' && !verificarEVazio(origem[campo]) && !(origem[campo] instanceof Date)) {
          destino[campo] = this.merge(destino[campo], origem[campo]);
        } else {
          destino[campo] = origem[campo];
        }
      }

      return destino;
    },

    /**
     * Função que clona um objeto, usando a estratégia de 'deep clone'.
     * @param {Object} objeto O objeto que será copiado.
     * @returns {Object} O novo objeto, clone do original.
     */
    clonar: function (objeto) {
      return this.merge(null, objeto);
    },

    /**
     * Função que computa a diferença entre objetos para a realização de evento PATCH.
     * Essa função destaca apenas os itens alterados para envio à API.
     * @param {Object} alterado O objeto com as alterações realizadas.
     * @param {Object} original O objeto com os dados originais.
     * @returns {Object} O objeto com as diferenças entre o objeto alterado e o original.
     */
    patch: function (alterado, original) {
      const verificarEVazio = function (item) {
        return item === null || item === undefined || item === '';
      };

      var destino = Array.isArray(alterado) ? [] : {};

      if (!original) {
        original = {};
      }

      var listaCampos = Object.keys(alterado)
        .concat(Object.keys(original))
        .filter(function (item, index, array) {
          return array.indexOf(item) === index;
        });

      for (var indice in listaCampos) {
        var campo = listaCampos[indice];

        if ((alterado[campo] === original[campo])
          || (!alterado[campo] && !original[campo] && typeof alterado[campo] === typeof original[campo])) {

          continue;
        }

        if (typeof alterado[campo] === 'object' && !verificarEVazio(alterado[campo]) && !(alterado[campo] instanceof Date)) {
          if (Array.isArray(alterado[campo])) {
            if (!this.equivalentes(original[campo], alterado[campo])) {
              destino[campo] = this.clonar(alterado[campo]);
            }
          } else {
            var alteracoes = this.patch(alterado[campo], original[campo]);
            if (Object.keys(alteracoes).length > 0) {
              destino[campo] = alteracoes;
            }
          }
        } else {
          destino[campo] = alterado[campo];
        }
      }

      return destino;
    },

    /**
     * Função que realiza a comparação entre dois objetos.
     * @param {Object} origem O objeto de origem.
     * @param {Object} destino O objeto de destino.
     * @returns {boolean} Um valor que indica se os objetos são equivalentes.
     */
    equivalentes: function (origem, destino) {
      if (origem === destino) {
        return true;
      }

      if ((!origem && destino) || (origem && !destino) || typeof origem !== typeof destino) {
        return origem !== origem && destino !== destino;
      }

      var listaCampos = Object.keys(origem)
        .concat(Object.keys(destino))
        .filter(function (item, index, array) {
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
