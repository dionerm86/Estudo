Vue.component('campo-busca-etiqueta-processo', {
  mixins: [Mixins.Comparar],
  props: {
    /**
     * Processo selecionado.
     * @type {?Object}
     */
    processo: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },
  },

  data: function() {
    return {
      idProcesso: (this.processo || {}).id || 0,
      codigoProcesso: (this.processo || {}).codigo
    };
  },

  methods: {
    /**
     * Busca os processos para o controle.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    buscarProcessos: function() {
      return Servicos.Processos.obterParaControle();
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o processo
     * e que atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    processoAtual: {
      get: function () {
        return this.processo;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.processo)) {
          this.$emit('update:processo', valor);
          this.idProcesso = valor ? valor.id : 0;
          this.codigoProcesso = valor ? valor.codigo : null;
        }
      }
    }
  },

  template: '#CampoBuscaEtiquetaProcesso-template'
});
