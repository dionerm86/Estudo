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
            return Servicos.Parcelas.ObterListaParcelas(filtro, pagina, numeroRegistros, ordenacao);
        },

        /**
         * Obtém link para a tela de inserção da parcela.
         */
        obterLinkInserirParcela: function () {
            return '../Cadastros/CadParcelas.aspx';
        },

        /**
         * Obtém link para a tela de edição da parcela.
         * @param {Object} item A parcela que será editada.
         */
        obterLinkEditarParcela: function (item) {
            return '../Cadastros/CadParcelas.aspx?idParcela=' + item.id;
        },

    /**
     * Exclui uma parcela.
     * @param {Object} parcela A parcela que será excluída.
     */
        excluir: function (parcela) {
            if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir esta parcela?')) {
                return;
            }

            var vm = this;

            Servicos.Parcelas.excluir(parcela.id)
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
         * Exibe um relatório com a listagem das parcelas.
         * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
         */
        abrirListaParcelas: function (exportarExcel) {
            this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=Parcelas&exportarExcel=' + exportarExcel);
        }
    },

    mounted: function () {
        var vm = this;

        Servicos.Parcelas.obterConfiguracoesListaParcela()
          .then(function (resposta) {
              vm.configuracoes = resposta.data;
          });
    }
});
