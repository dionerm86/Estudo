var Log = Log || {};
Log.Cancelamento = {
  Tabelas: null,
  Controles: []
};

Vue.component('log-cancelamento', {
  props: {
    /**
     * Tabela vinculada ao log de cancelamento.
     * @type {!string}
     */
    tabela: {
      required: true,
      validator: Mixins.Validacao.validarString
    },

    /**
     * ID do item para recuperação do log de cancelamento.
     * @type {!number}
     */
    idItem: {
      required: true,
      validator: Mixins.Validacao.validarNumero
    },

    /**
     * Nome do campo para busca dos logs.
     * @type {?string}
     */
    campo: {
      required: false,
      default: '',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Indica se o controle é responsável pela verificação de logs ao
     * alterar as propriedades para busca (tabela ou ID do item).
     * @type {?boolean}
     */
    atualizarAoAlterar: {
      required: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function() {
    return {
      itemTabela: null,
      dados: Log.Cancelamento
    };
  },

  methods: {
    /**
     * Atualiza a variável 'itemTabela' com os dados da tabela atual.
     */
    atualizaTabelas: function() {
      if (!this.dados || this.dados.Tabelas === null) {
        return;
      }

      this.itemTabela = this.dados.Tabelas.filter(function(item) {
        return item.nome === this.tabela;
      }, this)[0];
    },

    /**
     * Verifica se o item atual (tabela e ID) possui log de cancelamento.
     * Apenas é executado se as tabelas já tiverem sido carregadas e se o controle for responsável
     * pela exibição ou não do botão.
     */
    verificarPossuiLog: function() {
      if (!this.atualizarAoAlterar || !this.dados || this.dados.Tabelas === null || this.dados.Tabelas.length === 0) {
        return;
      }

      if (!this.itemTabela) {
        this.visivel = false;
      } else {
        var vm = this;

        Servicos.Log.Cancelamento.verificarLogItem(this.itemTabela.id, this.idItem, this.campo)
          .then(function(resposta) {
            vm.visivel = true;
          })
          .catch(function(resposta) {
            vm.visivel = false;
          });
      }
    },

    /**
     * Exibe os logs de cancelamento em um popup.
     */
    abrirLogCancelamento: function() {
      const url = '/Utils/ShowLogCancelamento.aspx?tabela=' + this.itemTabela.id + '&id=' + this.idItem;
      this.abrirJanela(500, 700, this.caminhoReltivo(url));
    }
  },

  mounted: function() {
    if (this.dados.Tabelas === null) {
      this.dados.Tabelas = [];
      this.dados.Controles.push(this);

      var vm = this;

      Servicos.Log.Cancelamento.obterTabelas().then(function(resposta) {
        vm.dados.Tabelas = resposta.data;

        for (var controle of vm.dados.Controles) {
          controle.atualizaTabelas();
        }
      });
    } else if (this.dados.Tabelas.length === 0) {
      this.dados.Controles.push(this);
    } else {
      this.atualizaTabelas();
    }
  },

  computed: {
    /**
     * Propriedade computada que indica se o item está visível.
     */
    visivel: function() {
      return !this.atualizarAoAlterar;
    }
  },

  watch: {
    /**
     * Observador para a variável 'dados'.
     * Atualiza a exibição do controle, se necessário.
     */
    'dados.Tabelas': {
      handler: function() {
        this.verificarPossuiLog();
      },
      deep: true
    },

    /**
     * Observador para a variável 'idItem'.
     * Atualiza a exibição do controle, se necessário.
     */
    idItem: function() {
      this.verificarPossuiLog();
    },

    /**
     * Observador para a variável 'tabela'.
     * Atualiza a exibição do controle, se necessário.
     */
    tabela: function() {
      this.atualizaTabelas();
      this.verificarPossuiLog();
    }
  },

  template: '#LogCancelamento-template'
});
