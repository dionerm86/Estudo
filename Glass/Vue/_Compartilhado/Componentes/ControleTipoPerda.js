Vue.component('controle-tipo-perda', {
  inheritAttrs: false,
  mixins: [Mixins.Objetos],

  props: {
    /**
     * Tipo de perda selecionado no controle.
     * @type {Object}
     */
    tipoPerda: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Subtipo de perda selecionado no controle.
     * @type {Object}
     */
    subtipoPerda: {
      required: false,
      twoWay: true,
      default: null,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Indica se o controle permite a exibição e seleção de subtipos de perda.
     * @type {?boolean}
     */
    exibirSubtipos: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function () {
    return {
      possuiSubtipos: false
    };
  },

  methods: {
    /**
     * Busca os tipos de perda para o controle de seleção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPerda: function () {
      return Servicos.Producao.TiposPerda.obterParaFiltro();
    },

    /**
     * Busca os subtipos de perda para o controle de seleção.
     * @param {Object} filtro O filtro usado para a busca de subtipos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSubtiposPerda: function (filtro) {
      var vm = this;
      const idTipoPerda = filtro.idTipoPerda;

      if (!idTipoPerda) {
        this.possuiSubtipos = false;
        return Promise.reject();
      }

      return Servicos.Producao.TiposPerda.SubtiposPerda.obterParaFiltro(idTipoPerda)
        .then(function (resposta) {
          vm.possuiSubtipos = !!(resposta && resposta.data && repsosta.data.length);
          return resposta;
        })
        .catch(function (erro) {
          vm.possuiSubtipos = false;
          return erro;
        });
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o filtro para a busca de subtipos de perda.
     * @type {Object}
     */
    filtroSubtipoPerda: function () {
      return {
        idTipoPerda: this.tipoPerda ? this.tipoPerda.id : null
      };
    },

    /**
     * Propriedade computada que normaliza o tipo de perda para o controle interno
     * e que atualiza a propriedade principal em caso de alteração.
     * @type {Object}
     */
    tipoPerdaAtual: {
      get: function () {
        return this.tipoPerda;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.tipoPerda)) {
          this.$emit('update:tipoPerda', valor);
        }
      }
    },

    /**
     * Propriedade computada que normaliza o subtipo de perda para o controle interno
     * e que atualiza a propriedade principal em caso de alteração.
     * @type {Object}
     */
    subtipoPerdaAtual: {
      get: function () {
        return this.subtipoPerda;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.subtipoPerda)) {
          this.$emit('update:subtipoPerda', valor);
        }
      }
    }
  },

  template: '#ControleTipoPerda-template'
});
