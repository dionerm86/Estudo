var Busca = Busca || {};

/**
 * Objeto com os dados para funcionamento dos controles de busca por popup.
 */
Busca.Popup = {
  /**
   * Lista de controles exibidos atualmente.
   * @type {Object[]}
   */
  controles: [],

  /**
   * Atualiza o valor selecionado no controle, disparando a busca pelo nome.
   * @param {string} idControle O identificador do controle que foi alterado.
   * @param {?number} id O identificador do item selecionado.
   * @param {string} valor O nome do item selecionado.
   */
  atualizar: function (idControle, id, valor) {
    var controle = Busca.Popup.controles[Busca.Popup.indiceControle(idControle)];
    if (!controle) {
      return;
    }

    if (id) {
      controle.vm.idAtual = id;
    } else if (valor) {
      controle.vm.idAtual = null;
      controle.vm.nomeAtual = valor;
    }

    controle.janela = null;
  },

  /**
   * Retorna o índice de um controle específico na lista de controles.
   * @param {string} idControle O identificador do controle que está sendo buscado.
   * @returns {number} O índice do controle na lista.
   */
  indiceControle: function (idControle) {
    for (var i = 0, l = Busca.Popup.controles.length; i < l; i++) {
      if (Busca.Popup.controles[i].id === idControle) {
        return i;
      }
    }

    return -1;
  }
};

Vue.component('campo-busca-com-popup', {
  inheritAttrs: false,
  mixins: [Mixins.UUID, Mixins.Objetos],
  props: {
    /**
     * O item que está selecionado atualmente no controle.
     * @type {Object}
     */
    itemSelecionado: {
      required: false,
      twoWay: true,
      default: null,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * O ID do item selecionado atualmente no controle.
     * @type {?number}
     */
    id: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * O nome do item selecionado atualmente no controle.
     * @type {?string}
     */
    nome: {
      required: false,
      twoWay: true,
      default: '',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * O nome do campo que corresponde ao 'ID' a ser exibido no controle.
     * @type {?string}
     */
    campoId: {
      required: false,
      twoWay: false,
      default: 'id',
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * O nome do campo que corresponde ao 'nome' a ser exibido no controle.
     * @type {?string}
     */
    campoNome: {
      required: false,
      twoWay: false,
      default: 'nome',
      validator: Mixins.Validacao.validarOuVazio
    },

    /**
     * Função que realiza a busca dos itens a partir do nome.
     * @type {!function}
     * @param {string} nome O nome que será usado para a busca.
     * @returns {Promise}
     */
    funcaoBuscarItens: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarFuncao
    },

    /**
     * Indica se os itens serão pré-carregados ao iniciar o controle ou
     * se a busca será feita sob demanda.
     * @type {?boolean}
     */
    preCarregarItens: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * A URL que será aberta no popup.
     * @type {?string}
     */
    urlPopup: {
      required: true,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * A largura da janela de popup que será aberta.
     * @type {?number}
     */
    larguraPopup: {
      required: false,
      twoWay: false,
      default: 800,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * A altura da janela de popup que será aberta.
     * @type {?number}
     */
    alturaPopup: {
      required: false,
      twoWay: false,
      default: 600,
      validator: Mixins.Validacao.validarNumeroOuVazio
    }
  },

  data: function() {
    return {
      uuid: null
    }
  },

  methods: {
    /**
     * Exibe o popup para busca de itens.
     */
    abrirPopup: function () {
      var controle = Busca.Popup.controles[Busca.Popup.indiceControle(this.uuid)];
      if (!controle) {
        return;
      }

      const url = this.caminhoRelativo(this.urlPopup)
        + (this.urlPopup.indexOf('?') > -1 ? '&' : '?')
        + 'buscaComPopup=true&id-controle=' + this.uuid;

      controle.janela = this.abrirJanela(this.alturaPopup, this.larguraPopup, url);
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o ID selecionado no controle e
     * que atualiza a propriedade se alterada.
     * @type {Object}
     */
    idAtual: {
      get: function () {
        return this.id || 0;
      },
      set: function (valor) {
        if (valor !== this.id) {
          this.$emit('update:id', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o nome selecionado no controle e
     * que atualiza a propriedade se alterada.
     * @type {Object}
     */
    nomeAtual: {
      get: function () {
        return this.nome || '';
      },
      set: function (valor) {
        if (valor !== this.nome) {
          this.$emit('update:nome', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o item selecionado atualmente no controle e
     * que atualiza a propriedade se alterada.
     * @type {Object}
     */
    itemSelecionadoAtual: {
      get: function () {
        return this.itemSelecionado || null;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.itemSelecionado)) {
          this.$emit('update:itemSelecionado', valor);
        }
      }
    }
  },

  mounted: function() {
    this.uuid = this.gerarUuid();

    Busca.Popup.controles.push({
      id: this.uuid,
      vm: this,
      janela: null
    });
  },

  beforeDestroy: function() {
    var indiceControle = Busca.Popup.indiceControle(this.uuid);

    if (indiceControle !== -1) {
      Busca.Popup.controles.splice(indiceControle, 1);
    }
  },

  template: '#CampoBuscaComPopup-template'
});
