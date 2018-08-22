Vue.component('campo-busca-cliente', {
  inheritAttrs: false,
  mixins: [Mixins.Comparar],
  props: {
    /**
     * Cliente selecionado.
     * @type {?Object}
     */
    cliente: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Indica se as informações de compra do cliente devem ser exibidas no controle.
     * @type {?boolean}
     */
    exibirInformacoesCompra: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Tipo de validação que será feita para a busca de clientes.
     * @type {?string}
     */
    tipoValidacao: {
      required: false,
      twoWay: false,
      default: '',
      validator: Mixins.Validacao.validarValoresOuVazio('Pedido')
    },

    /**
     * Cor do texto de observações do cliente.
     * @type {?string}
     */
    corTextoObservacoes: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  data: function() {
    return {
      idCliente: (this.cliente || {}).id || 0,
      idInterno: (this.cliente || {}).id || null,
      nomeCliente: (this.cliente || {}).nome || ''
    };
  },

  methods: {
    /**
     * Busca a lista de clientes.
     * @param {?number} id Identificador do cliente para a consulta.
     * @param {string} nome Nome (ou parte dele) usado para a consulta.
     * @returns {Promise} Uma Promise com os dados dos clientes encontrados.
     */
    buscarClientes: function(id, nome) {
      if (!id && !nome) {
        return Promise.reject();
      }

      return Servicos.Clientes.obterParaControle(id, nome, this.tipoValidacao);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o cliente normalizado
     * e que atualiza a propriedade em caso de alteração.
     * @type {Object}
     */
    clienteAtual: {
      get: function () {
        return this.cliente || {};
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.cliente)) {
          this.$emit('update:cliente', valor);
        }
      }
    }
  },

  watch: {
    /**
     * Observador da variável 'cliente.id'.
     * Altera o valor da variável com o ID do cliente exibido no controle.
     */
    'cliente.id': function (atual) {
      this.idCliente = atual > 0 ? atual : null;
      this.idInterno = this.idCliente;
    },

    /**
     * Observador da variável 'cliente.nome'.
     * Altera o valor da variável com o nome do cliente exibido no controle.
     */
    'cliente.nome': function (atual) {
      this.nomeCliente = atual || '';
    }
  },

  template: '#CampoBuscaCliente-template'
});
