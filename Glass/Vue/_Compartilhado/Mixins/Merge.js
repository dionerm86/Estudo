var Mixins = Mixins || {};

/**
 * Objeto com o mixin para realização de 'merge' entre os objetos.
 */
Mixins.Merge = {
  methods: {
    /**
     * Função que realiza o merge entre dois objetos, copiando os dados preenchidos de um objeto para outro.
     * @param {Object} estrutura O objeto de origem, que será copiado.
     * @param {Object} preenchido O objeto com os dados preenchidos.
     * @returns {Object} O novo objeto, que contém a estrutura informada e com os dados do objeto preenchido.
     */
    merge: function(estrutura, preenchido) {
      if (!estrutura || !preenchido) {
        throw new Error('Ambos os parâmetros são obrigatórios para a função merge.');
      }

      const verificarEVazio = item => item === null || item === undefined || item === '';
      var destino = estrutura;

      for (var campo in destino) {
        if (typeof preenchido[campo] === 'object' && !verificarEVazio(preenchido[campo]) && !(preenchido[campo] instanceof Date)) {
          destino[campo] = this.merge(destino[campo], preenchido[campo]);
        } else {
          destino[campo] = preenchido[campo];
        }
      }

      return destino;
    }
  }
};
