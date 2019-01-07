Vue.component('preco-tabela-cliente-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de preço de tabela de cliente.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          idCliente: null,
          nomeCliente: null,
          idGrupoProduto: null,
          idsSubgrupoProduto: [],
          codigoProduto: null,
          descricaoProduto: null,
          tipoValorTabela: null,
          valorAlturaInicio: null,
          valorAlturaFim: null,
          valorLarguraInicio: null,
          valorLarguraFim: null,
          ordenacaoManual: null,
          apenasComDesconto: false,
          tipoValorTabela: null,
          incluirBeneficiamentosRelatorio: false
        },
        this.filtro
      ),
      grupoAtual: null,
      subgrupoAtual: null,
      tipoAtual: null,
      ordenacaoManualAtual: null,
      exibirPercentualDescontoAcrescimo: false,
      NaoExibirColunaValorOriginal: false,
      clienteSelecionado: false
    };
  },

  methods: {
    /**
     * Retorna os itens para o controle de grupos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroGrupos: function () {
      return Servicos.Produtos.Grupos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de subgrupos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSubgrupos: function (filtro) {
      var idsGrupoProduto = (filtro || {}).idsGrupoProduto || [];
      return Servicos.Produtos.Subgrupos.obterParaControle(idsGrupoProduto);
    },

    /**
     * Retorna os itens para o controle de tipos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposValorTabela: function () {
      return Servicos.Produtos.PrecosTabelaCliente.obterTiposValorTabela();
    },

    selecionarCliente: function (event) {
      var idCliente = event.target && event.target.value ? event.target.value : null;
      var vm = this;

      if (idCliente != null) {
        return Servicos.Clientes.obterParaControle(idCliente, null, null)
          .then(function (resposta) {
            if (resposta.data && resposta.data.mensagem) {
              this.exibirMensagem(resposta.data.mensagem)
            }
            else {
              var cliente = resposta.data[0];
              vm.filtroAtual.nomeCliente = cliente.nome;
            }
          });
      }
      else {

      }
    },

    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function () {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    }
  },

  template: '#ListaPrecoTabelaCliente-Filtro-template'
});
