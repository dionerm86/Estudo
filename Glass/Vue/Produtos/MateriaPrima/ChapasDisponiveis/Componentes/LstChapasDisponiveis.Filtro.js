Vue.component('chapas-disponiveis-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de preço de tabela por cliente.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    data: function () {
      return {
        filtroAtual: this.merge(
          {
            cor: null,
            espessura: null,
            idFornecedor: null,
            nomeFornecedor: null,
            numeroNotaFiscal: null,
            lote: null,
            altura: null,
            largura: null,
            codigoProduto: null,
            descricaoProduto: null,
            codigoEtiqueta: null,
            idLoja: null
          },
          this.filtro
        ),
        corAtual: null,
        lojaAtual: null
      };
    },

    methods: {
      /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
      filtrar: function () {
        var novoFiltro = this.clonar(this.filtroAtual);
        if (!this.equivalentes(this.filtro, novoFiltro)) {
          this.$emit('update:filtro', novoFiltro);
        }
      },

      /**
       * Retorna os itens para o controle de cores de chapas disponíveis.
       * @returns {Promise} Uma Promise com o resultado da busca.
       */
      obterItensFiltroCor: function () {
        return Servicos.Produtos.CoresVidro();
      },

      /**
       * Retorna os itens para o controle de lojas.
       * @returns {Promise} Uma Promise com o resultado da busca.
       */
      obterItensFiltroLoja: function () {
        return Servicos.Lojas.obterParaControle();
      }
    },

    watch: {
      /**
     * Observador para a variável 'corAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
      corAtual: {
        handler: function (atual) {
          this.filtroAtual.cor = atual ? atual.id : null;
        }
      },

      /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
      lojaAtual: {
        handler: function (atual) {
          this.filtroAtual.idLoja = atual ? atual.id : null;
        }
      }
    }
  },

  template: '#ListaChapasDisponiveis-Filtro-template'
});
