const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca as impressões de etiquetas para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.ImpressoesEtiquetas.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Abre um popup para exibir os itens impressos.
     * @param {Object} item A impressão de etiqueta a partir da qual serão exibidos os itens impressos.
     */
    abrirItensImpressos: function (item) {
      this.abrirJanela(550, 750, '../Listas/LstEtiquetaProdImp.aspx?IdImpressao=' + item.id);
    },

    /**
     * Abre um popup para exibir a otimização no eCutter.
     * @param {Object} item A impressão de etiqueta que será usada para exibir a otimização no eCutter.
     */
    abrirOtimizacaoECutter: function (item) {
      this.abrirJanela(600, 800, this.configuracoes.enderecoECutter + item.id);
    },

    /**
     * Abre um popup para cancelar a impressão de etiqueta.
     * @param {Object} item A impressão que será cancelada.
     */
    abrirCancelamentoImpressao: function (item) {
      this.abrirJanela(350, 600, '../Utils/SetMotivoCancEtiqueta.aspx?idImpressao=' + item.id + '&tipo=' + item.tipoImpressao.id);
    },

    /**
     * Baixa o arquivo de otimização desta impressão.
     * @param {Object} item A impressão de etiqueta a partir da qual será baixado o arquivo de otimização.
     */
    baixarArquivoOtimizacao: function (item) {
      window.location.assign('../Handlers/ArquivoOtimizacao.ashx?idImpressao=' + item.id);
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.ImpressoesEtiquetas.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
