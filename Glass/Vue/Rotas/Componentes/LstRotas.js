const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    configuracoes: {}
  },

  methods: {
    /**
     * Busca a lista de rotas para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Rotas.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção de rota.
     */
    obterLinkInserirRota: function () {
      return '../Cadastros/CadRota.aspx';
    },

    /**
     * Obtém link para a tela de edição de rota.
     * @param {Object} item A rota que será editada.
     */
    obterLinkEditarRota: function (item) {
      return '../Cadastros/CadRota.aspx?idRota=' + item.id;
    },

    /**
     * Exclui uma rota.
     * @param {Object} rota A rota que será excluída.
     */
    excluir: function (rota) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir esta rota?')) {
        return;
      }

      var vm = this;

      Servicos.Rotas.excluir(rota.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Exibe um relatório com dados da rota.
     * @param {Object} item A rota que será impressa.
     */
    abrirDadosRota: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=DadosRota&idRota=' + item.id);
    },

    /**
     * Exibe um relatório com a listagem de rotas.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaRotas: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ListaRota&exportarExcel=' + exportarExcel);
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Rotas.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
