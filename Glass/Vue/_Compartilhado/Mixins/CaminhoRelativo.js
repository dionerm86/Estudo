/**
 * Objeto com o mixin para definir o caminho para uma URL.
 */
Vue.mixin({
  methods: {
    /**
     * Função para calcular a URL relativa a partir de um caminho informado.
     * @param {!string} url A URL que será alterada.
     * @returns {string} A URL com o caminho relativo.
     */
    caminhoRelativo: function (url) {
      if (!url) {
        throw new Error('URL é obrigatória para o cálculo do caminho relativo.');
      }

      // O caminho absoluto do site está definido no arquivo Layout.master,
      // como variável global (solução temporária para o ASP.net)
      const caminhoSite = caminhoAbsolutoSite.trim();
      url = url.trim();

      if (url.startsWith(caminhoSite)) {
        return url;
      }

      const urlFinal = url;

      if (caminhoSite.endsWith("/")) {
        urlFinal = url.replace(/^\/*/, '');
      }

      return url !== urlFinal.replace(/^\/*/, '')
        ? caminhoSite + urlFinal
        : url;
    }
  }
});
