const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca os projetos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Projetos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtém link para a tela de inserção de projeto.
     */
    obterLinkInserirProjeto: function () {
      return 'CadProjeto.aspx';
    },

    /**
     * Obtém link para a tela de edição de projeto.
     * @param {Object} item O projeto que será editado.
     */
    obterLinkEditarProjeto: function (item) {
      return this.obterLinkInserirProjeto() + '?idProjeto=' + item.id;
    },

    /**
     * Exclui um projeto.
     * @param {Object} projeto O projeto que será excluído.
     */
    excluir: function (projeto) {
      if (!this.perguntar('Tem certeza que deseja excluir este projeto?')) {
        return;
      }

      if (projeto.numeroItensProjeto >= 10 && !this.perguntar("ESTE PROJETO TEM MAIS DE 10 CÁLCULOS. DESEJA EXCLUÍ-LO ASSIM MESMO?")) {
        return;
      }

      if (projeto.numeroItensProjeto >= 20 && !this.perguntar("ESTE PROJETO TEM MAIS DE 20 CÁLCULOS. DESEJA EXCLUÍ-LO ASSIM MESMO?")) {
        return;
      }

      if (projeto.numeroItensProjeto >= 30 && !this.perguntar("ESTE PROJETO TEM MAIS DE 30 CÁLCULOS. DESEJA EXCLUÍ-LO ASSIM MESMO?")) {
        return;
      }

      var vm = this;

      Servicos.Projetos.excluir(projeto.id)
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
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  }
});
