const app = new Vue({
  el: '#app',
  data: {
    filtro: {},
    configuracoes: {},
    agruparImpressao: null,
    contagem: null,
    exibirContagem: false
  },

  methods: {
    /**
     * Recupera a lista de peças para a tela de consulta de produção.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de peças.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    buscarPecas: function (filtro, pagina, numeroRegistros, ordenacao) {
      if (!this.configuracoes || !Object.keys(this.configuracoes).length) {
        return Promise.reject();
      }

      return Servicos.Producao.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Busca os dados de contagem de peças com base no filtro atual.
     */
    realizarContagemPecas: function () {
      this.contagem = null;

      if (!this.filtro || !Object.keys(this.filtro).length) {
        return;
      }

      var vm = this;

      Servicos.Producao.obterContagemPecas(this.filtro)
        .then(function (resposta) {
          vm.contagem = resposta.data;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Contagem de peças', erro.mensagem);
          }
        });
    },

    /**
     * Define a exibição da contagem de peças apenas se houver peças em exibição.
     */
    atualizouItens: function (numeroItens) {
      this.exibirContagem = numeroItens > 0;
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Producao.obterConfiguracoesConsulta()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },

  watch: {
    /**
     * Observador para a variável 'filtro'.
     * Realiza a contagem de peças ao alterar o filtro atual.
     */ 
    filtro: {
      handler: function () {
        this.realizarContagemPecas();
      },
      deep: true
    }
  }
});
