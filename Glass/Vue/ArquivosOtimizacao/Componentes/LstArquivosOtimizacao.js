const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('ao.dataCad', 'desc')],

  data: {
    filtro: {},
    configuracoes: {}
  },

  methods: {
    /**
     * Busca os arquivos de otimização.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.ArquivosOtimizacao.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Formata a data para ser utlizada no link de download.
     * @param {Date} data A data que será utilizada.
     */
    formatarData: function (data) {
      var date = data.toISOString();
      date = date.substr(0, 19);
      date = date.replace('%20', ' ');
      
      while (date.indexOf(':') > -1) {
        date = date.replace(':', '_');
      }

      while (date.indexOf(' ') > - 1) {
        date = date.replace(' ', '%20');
      }

      return date;
    },

    /**
     * Formata os filtros para a utilização no link de download.
     */
    formatarFiltros_: function (caminhoArquivo, nomeArquivo) {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'filePath', caminhoArquivo);
      this.incluirFiltroComLista(filtros, 'fileName', nomeArquivo);

      return filtros.length
        ? '?' + filtros.join('&')
        : '';
    },

    /**
     * Função que realiza o download do arquivo.
     * @param {string} arquivoOtimizacao O arquivo que o usuário irá receber.
     */
    abrirLinkDownload: function (arquivoOtimizacao) {
      var nomeArquivo = arquivoOtimizacao.funcionario + ' ' + this.formatarData(arquivoOtimizacao.dataCadastro) + arquivoOtimizacao.arquivo.extensao;
      var link = ('../../Handlers/Download.ashx' + this.formatarFiltros_(arquivoOtimizacao.arquivo.caminho, nomeArquivo));

      redirectUrl(link);
    },

    /**
     * Função que realiza o download do arquivo pelo ECutter.
     * @param {string} arquivoOtimizacao O arquivo que o usuário irá receber.
     */
    abrirLinkDownloadECutter: function (idArquivoOtimizacao) {
      var link = this.configuracoes.enderecoECutter + idArquivoOtimizacao;

      redirectUrl(link);
    },
  },

  mounted: function () {
    var vm = this;

    Servicos.ArquivosOtimizacao.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },
});
