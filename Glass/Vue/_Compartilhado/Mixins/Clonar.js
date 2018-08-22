var Mixins = Mixins || {};

/**
 * Objeto com o mixin para realização de 'deep clone' entre os objetos.
 */
Mixins.Clonar = {
  methods: {
    /**
     * Função que clona um objeto, usando a estratégia de 'deep clone'.
     * @param {Object} origem O objeto de origem, que será copiado.
     * @param {?boolean} [ignorarVazio=false] Indica se os campos vazios serão ignorados.
     * @param {?string[]} [camposPodemSerZero=null] Lista com os nomes dos campos que podem ter o valor 0 (zero).
     * @returns {Object} O novo objeto, clone do original.
     */
    clonar: function(origem, ignorarVazio, camposPodemSerZero) {
      const verificarEVazio = item => item === null || item === undefined || item === '';

      ignorarVazio = verificarEVazio(ignorarVazio) ? false : ignorarVazio;
      camposPodemSerZero = verificarEVazio(camposPodemSerZero) ? [] : camposPodemSerZero;

      if (!Array.isArray(camposPodemSerZero)) {
        camposPodemSerZero = [camposPodemSerZero];
      }

      var destino = Array.isArray(origem) ? [] : {};

      for (var campo in origem) {
        if (ignorarVazio) {
          if (verificarEVazio(origem[campo])) {
            continue;
          } else if (origem[campo] === 0 && camposPodemSerZero.indexOf(campo) === -1) {
            continue;
          }
        }

        if (typeof origem[campo] === 'object' && !verificarEVazio(origem[campo]) && !(origem[campo] instanceof Date)) {
          destino[campo] = this.clonar(origem[campo], ignorarVazio, camposPodemSerZero);
        } else {
          destino[campo] = origem[campo];
        }
      }

      return destino;
    }
  }
};
