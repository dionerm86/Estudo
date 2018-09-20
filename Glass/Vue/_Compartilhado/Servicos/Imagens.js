var Servicos = Servicos || {};

Servicos.Imagens = (function (http) {
  const API = '/api/v1/imagens/';

  return {
    /**
     * Recupera os dados para exibição de imagem no controle de exibição.
     * @param {?number} idItem O identificador do item que contém a imagem.
     * @param {?string} tipoItem O tipo do item que contém a imagem.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterDados: function (idItem, tipoItem) {
      if (!idItem || !tipoItem) {
        return Promise.reject();
      }

      return http().get(API + 'exibicao/' + idItem + '/' + tipoItem);
    }
  }
})(function () {
  return Vue.prototype.$http;
});
