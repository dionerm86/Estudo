Vue.component('campo-busca-natureza-operacao', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Natureza de operação selecionada.
     * @type {?Object}
     */
    naturezaOperacao: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    }
  },

  data: function () {
    return {
      idNaturezaOperacao: (this.naturezaOperacao || {}).id || 0,
      nomeNaturezaOperacao: (this.naturezaOperacao || {}).nome
    };
  },

  methods: {
    /**
     * Busca as naturezas de operação para o controle.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    buscarNaturezasOperacao: function() {
      return Servicos.Cfops.NaturezasOperacao.obterParaControle();
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna a natureza de operação
     * e que atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    naturezaOperacaoAtual: {
      get: function () {
        return this.naturezaOperacao;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.naturezaOperacao)) {
          this.$emit('update:naturezaOperacao', valor);
          this.idNaturezaOperacao = valor ? valor.id : 0;
          this.nomeNaturezaOperacao = valor ? valor.nome : null;
        }
      }
    }
  },

  template: '#CampoBuscaNaturezaOperacao-template'
});
