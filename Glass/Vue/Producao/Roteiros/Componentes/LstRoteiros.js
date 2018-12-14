const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca a lista de roteiros para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Producao.Roteiros.obter(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Redireciona para a tela de cadastro do roteiro.
     */
    obterLinkInserirRoteiro: function () {
      return '../../Cadastros/Producao/CadRoteiroProducao.aspx';
    },

    /**
     * Redireciona para a tela de edição do roteiro.
     * @param {Object} item O roteiro que será editado.
     */
    obterLinkEditarRoteiro: function (item) {
      return this.obterLinkInserirRoteiro() + '?id=' + item.id;
    },

    /**
     * Retorna os nomes dos setores associados ao roteiro informado.
     * @param {Object} roteiro O roteiro que terá os setores listados.
     * @returns {string} Os nomes dos setores associados ao roteiro informado.
     */
    obterNomesSetores: function (roteiro) {
      if (!roteiro || !roteiro.setores) {
        return '';
      }

      return roteiro.setores.slice()
        .join(', ');
    },

    /**
     * Exclui um roteiro.
     * @param {Object} roteiro O roteiro que será excluído.
     */
    excluir: function (roteiro) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este roteiro?')) {
        return;
      }

      var vm = this;

      Servicos.Producao.Roteiros.excluir(roteiro.id)
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
     * Exibe um relatório com a listagem de roteiros aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaRoteiros: function (exportarExcel) {
      var url = '../../Relatorios/Producao/RelBase.aspx?rel=RoteiroProducao' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'grupoProd', this.filtro.idGrupoProduto);
      this.incluirFiltroComLista(filtros, 'subgrupoProd', this.filtro.idSubgrupoProduto);
      this.incluirFiltroComLista(filtros, 'processo', this.filtro.idProcesso);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Força a atualização da lista de roteiros, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
