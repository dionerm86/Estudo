Vue.component('integradores-itemhistorico', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados do item do esquema do histórico.
     * @type {Object}
     */
    itemEsquema: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Item do histórico.
     * @type {Object}
     **/
    item: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      colunas: [],
      exibirFalha: false,
    }
  },

  methods: {

    /**
     * Carrega as colunas para compor a visualização
     * dos dados do item do histórico.
     **/
    carregarColunas: function () {

      this.colunas = [];
      for (var i=0; i<this.itemEsquema.identificadores.length; i++) {
        this.colunas.push({
          descricao: null,
          valor: this.item.identificadores[i]
        });
      }

      this.colunas.push({
        descricao: 'Tipo',
        valor: this.item.tipo,
      });
      this.colunas.push({
        descricao: 'Mensagem',
        valor: this.item.mensagem,
      });
      this.colunas.push({
        descricao: 'Data',
        valor: this.item.data,
      });
    },

    alterarExibicaoFalha: function () {

      if (this.item.falha != null) {
        this.exibirFalha = !this.exibirFalha;
      }
    },
  },

  mounted: function () {

    this.carregarColunas();
  },

  template: '#LstIntegradores-ItemHistorico-template'
});
