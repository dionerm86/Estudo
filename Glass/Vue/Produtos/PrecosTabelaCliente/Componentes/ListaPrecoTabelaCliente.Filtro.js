Vue.component('preco-tabela-cliente-filtros', {
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

    /**
     * Configurações da lista de preço de tabela por cliente.
     * @type {Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
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
          incluirBeneficiamentosRelatorio: false,
          exibirPercentualDescontoAcrescimo: false,
          naoExibirColunaValorOriginal: false
        },
        this.filtro
      ),
      grupoAtual: null,
      tipoAtual: null,
      clienteSelecionado: false,
      clienteAtual: null
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
      var idGrupoProduto = (filtro || {}).idGrupoProduto || 0;
      return Servicos.Produtos.Subgrupos.obterParaControle(idGrupoProduto);
    },

    /**
     * Retorna os itens para o controle de tipos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposValorTabela: function () {
      return Servicos.Produtos.PrecosTabelaCliente.obterTiposValorTabela();
    },

    /**
     * Limpa as variáveis que armazenam as informações referentes ao cliente selecionado.
     * Permite que um novo cliente possa ser selecionado.
     */
    selecionarNovoCliente: function () {
      this.clienteSelecionado = false;
      this.clienteAtual = null;
      this.filtroAtual = {};
      this.filtrar();
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

  computed: {
    /**
     * Propriedade computada que retorna o filtro de subgrupos de produto.
     * @type {filtroSubgrupos}
     *
     * @typedef filtroSubgrupos
     * @property {?number} idGrupoProduto O ID do grupo de produto.
     */
    filtroSubgrupos: function () {
      return {
        idGrupoProduto: (this.filtroAtual || {}).idGrupoProduto || null
      };
    }
  },

  watch: {
    /**
     * Observador para a variável 'clienteAtual'.
     * Atualiza o filtro com o ID e Nome do cliente selecionado e indica que foi selecionado um cliente.
     */
    clienteAtual: {
      handler: function () {
        this.filtroAtual.idCliente = this.clienteAtual && this.clienteAtual.id ? this.clienteAtual.id : null;
        this.filtroAtual.nomeCliente = this.clienteAtual && this.clienteAtual.nome ? this.clienteAtual.nome : null;

        this.clienteSelecionado = this.clienteAtual ? true : false;
      }
    },

    /**
     * Observador para a variável 'grupoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoAtual: {
      handler: function (atual) {
        this.filtroAtual.idGrupoProduto = atual ? atual.id : null;
      }
    },

    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoValorTabela = atual ? atual.id : null;
      }
    },

    /**
     * Observador com as configurações e filtros personalizados para a tela.
     */
    configuracoes: {
      handler: function (atual) {
        (this.$refs.filtroSubgrupos || {}).itens = this.configuracoes.subgruposPadraoParaFiltro;
        this.filtroAtual.idsSubgrupoProduto.push(
          this.configuracoes.alterarSubgruposSelecionados &&
          this.configuracoes.alterarSubgruposSelecionados.id ?
          this.configuracoes.alterarSubgruposSelecionados.id : 0);

        var vm = this;

        this.$nextTick(function () {
          vm.filtrar();
        });
      },
      deep: true
    }
  },

  template: '#ListaPrecoTabelaCliente-Filtro-template'
});
