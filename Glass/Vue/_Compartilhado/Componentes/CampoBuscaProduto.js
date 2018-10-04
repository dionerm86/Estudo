Vue.component('campo-busca-produto', {
  inheritAttrs: false,
  mixins: [Mixins.Objetos, Mixins.JsonQuerystring, Mixins.ExecutarTimeout],
  props: {
    /**
     * Produto selecionado no controle.
     * @type {?Object}
     */
    produto: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Tipo de validação do controle.
     * @type {?string}
     */
    tipoValidacao: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarValoresOuVazio('Pedido', 'ProdutoPedido')
    },

    /**
     * Dados adicionais para a validação do controle, de acordo com o tipo.
     * @type {?Object}
     */
    dadosAdicionaisValidacao: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjetoOuVazio
    }
  },

  data: function() {
    return {
      idProduto: (this.produto || {}).id || 0,
      codigoInterno: (this.produto || {}).codigo || '',
      descricao: (this.produto || {}).descricao || ''
    };
  },

  methods: {
    /**
     * Recupera os produtos a partir do código informado.
     * @param {?number} id O identificador do produto a ser buscado.
     * @param {!string} codigoInterno O código do produto a ser buscado.
     * @return {Promise} Uma promise com os produtos encontrados.
     */
    buscarProdutos: function (id, codigoInterno) {
      return Servicos.Produtos.obterParaControle(
        codigoInterno,
        this.tipoValidacao,
        this.codificarJson(this.dadosAdicionaisValidacao)
      )
        .then(function(resposta) {
          if (resposta && resposta.data) {
            resposta.data = [resposta.data];
          }

          return resposta;
        })
        .catch(function(erro) {
          if (erro.codigo === 404) {
            return;
          }

          return Promise.reject(erro);
        });
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o produto selecionado e que atualiza
     * a propriedade em caso de alteração. Também altera a descrição atual do produto.
     * @type {Object}
     */
    produtoAtual: {
      get: function() {
        return this.produto;
      },
      set: function(valor) {
        if (!this.equivalentes(valor, this.produto)) {
          this.$emit('update:produto', valor);
          this.descricaoAtual = valor ? valor.descricao : null;
        }
      }
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'dadosAdicionaisValidacao'.
     * Atualiza os dados do produto atual.
     */
    dadosAdicionaisValidacao: {
      handler: function (atual) {
        if (!this.produtoAtual) {
          return;
        }

        this.executarTimeout('buscarProdutos', function () {
          this.$refs.base.$refs.campoBusca
            .buscar(null, this.produtoAtual.codigo);
        });
      },
      deep: true
    }
  },

  template: '#CampoBuscaProduto-template'
});
