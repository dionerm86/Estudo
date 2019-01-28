const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('IdArquivoRemessa', 'desc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca os arquivos de remessa.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.ArquivosRemessa.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exclui um arquivo de remessa.
     * @param {number} id O identificador do arquivo de remessa que será excluido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (id) {
      if (!this.perguntar('Tem certeza que deseja excluir este arquivo remessa?')) {
        return;
      }

      var vm = this;

      return Servicos.ArquivosRemessa.excluir(id)
        .then(function (resposta) {
          if (resposta.data && resposta.data.mensagem) {
            vm.exibirMensagem(resposta.data.mensagem);
          }

          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem(erro.mensagem);
          }
        });
    },

    /**
     * Obtém o link para download do arquivo de remessa.
     * @param {number} id O identificador do arquivo de remessa que será baixado.
     * @returns {string} O link para download do arquivo de remessa.
     */
    obterLinkDownload: function (id) {
      return '../Handlers/ArquivoRemessa.ashx?id=' + id;
    },

    /**
     * Obtém o link para o log de importação do arquivo de remessa.
     * @param {number} id O identificador do arquivo de remessa.
     * @returns {string} O link para o log de importação referente ao arquivo de remessa informado.
     */
    obterLinkLogImportacao: function (id) {
      return '../Handlers/ArquivoRemessa.ashx?logImportacao=true&id=' + id;
    },

    /**
     * Retifica um arquivo de remessa.
     * @param {number} id O identificador do arquivo de remessa que será baixado.
     * @returns {string} O link para cadastro da retificação do arquivo de remessa.
     */
    obterLinkRetificar: function (id) {
      return '../Cadastros/CadRetificarArquivoRemessa.aspx?id=' + id;
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
