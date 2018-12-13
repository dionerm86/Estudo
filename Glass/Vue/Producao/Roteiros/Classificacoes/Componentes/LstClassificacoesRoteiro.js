const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca a lista de classificações de roteiro para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Producao.Roteiros.ClassificacoesRoteiro.obter(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Redireciona para a tela de cadastro da classificação de roteiro.
     */
    obterLinkInserirClassificacaoRoteiro: function () {
      return '../Cadastros/CadClassificacaoRoteiroProducao.aspx';
    },

    /**
     * Redireciona para a tela de edição da classificação de roteiro.
     * @param {Object} item A classificação de roteiro que será editada.
     */
    obterLinkEditarClassificacaoRoteiro: function (item) {
      return this.obterLinkInserirClassificacaoRoteiro() + '?idClassificacao=' + item.id;
    },

    /**
     * Exclui uma classificação de roteiro.
     * @param {Object} classificacaoRoteiro A classificação de roteiro que será excluída.
     */
    excluir: function (classificacaoRoteiro) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir esta classificação de roteiro?')) {
        return;
      }

      var vm = this;

      Servicos.Producao.Roteiros.ClassificacoesRoteiro.excluir(classificacaoRoteiro.id)
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
     * Força a atualização da lista de classificações de roteiro, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
