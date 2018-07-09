Vue.component('campo-busca-etiqueta-aplicacao', {
  mixins: [Mixins.Comparar],
  props: {
    /**
     * Aplicação selecionada.
     * @type {?Object}
     */
    aplicacao: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    }
  },

  data: function () {
    return {
      idAplicacao: (this.aplicacao || {}).id || 0,
      codigoAplicacao: (this.aplicacao || {}).codigo
    };
  },

  methods: {
    /**
     * Busca as aplicações para o controle.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    buscarAplicacoes: function() {
      return Servicos.Aplicacoes.obterParaControle();
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna a aplicação
     * e que atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    aplicacaoAtual: {
      get: function () {
        return this.aplicacao;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.aplicacao)) {
          this.$emit('update:aplicacao', valor);
        }
      }
    }
  },

  template: '#CampoBuscaEtiquetaAplicacao-template'
});
