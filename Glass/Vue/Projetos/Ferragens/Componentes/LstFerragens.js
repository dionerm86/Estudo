const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'asc')],

  data: {
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca as ferragens para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Projetos.Ferragens.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção de ferragem.
     */
    obterLinkInserirFerragem: function () {
      return '../Cadastros/Projeto/CadFerragem.aspx';
    },

    /**
     * Obtém link para a tela de edição de ferragem.
     * @param {Object} ferragem A ferragem que será editada.
     */
    obterLinkEditarFerragem: function (item) {
      return this.obterLinkInserirFerragem() + '?IdFerragem=' + item.id;
    },

    /**
     * Exclui uma ferragem.
     * @param {Object} ferragem A ferragem que será excluída.
     */
    excluir: function (ferragem) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir esta ferragem?')) {
        return;
      }

      var vm = this;

      Servicos.Projetos.Ferragens.excluir(ferragem.id)
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
     * Altera a situação da ferragem.
     * @param {Object} item O item que terá a situação alterada.
     */
    alterarSituacao: function (item) {
      var vm = this;

      Servicos.Projetos.Ferragens.alterarSituacao(item.id)
        .then(function (resposta) {
          vm.atualizarLista();
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Projetos.Ferragens.obterConfiguracoesLista().then(function (resposta) {
      vm.configuracoes = resposta.data;
    });
  }
});
