const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('IdImpressao', 'DESC')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca as impressões individuais de etiquetas para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.ImpressoesEtiquetas.Individual.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Seleciona uma etiqueta para impressão.
     */
    selecionarEtiquetaProduto: function (EtiquetaProduto) {
      if (!this.configuracoes.possuiPermissaoImprimirEtiquetas) {
        this.exibirMensagem('Você não tem permissão para reimprimir etiquetas.');
        return;
      }

      var id = '';
      var codigoEtiqueta = this.filtro.codigoEtiqueta;
      if (EtiquetaProduto.idProdutoPedido > 0) {
        id = 'idProdPed=' + EtiquetaProduto.idProdutoPedido;
      }
      else if (EtiquetaProduto.numeroNotaFiscal > 0) {
        id = 'idProdNf=' + EtiquetaProduto.idProdutoNotaFiscal;
      }
      else {
        id = 'idAmbientePedido=' + EtiquetaProduto.idAmbientePedido;
      }

      this.abrirJanela(500, 700, '../Relatorios/RelEtiquetas.aspx?ind=1&' + id + '&tipo=0' + (codigoEtiqueta ? '&numEtiqueta=' + codigoEtiqueta : ''));
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

    Servicos.ImpressoesEtiquetas.Individual.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
