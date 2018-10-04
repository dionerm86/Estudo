Vue.component('controle-tooltip', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Indica se o botão precisa de um clique (ao invés de só ter o mouse sobre ele) para exibir o tooltip.
     * @type {?boolean}
     */
    precisaClicar: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Título para ser exibido no tooltip.
     * @type {string}
     */
    titulo: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarStringOuVazio
    }
  },

  data: function () {
    const estiloBotaoFecharNormal = {
      backgroundColor: '#cc0000',
      color: 'white'
    };

    const estiloBotaoFecharAtivo = {
      backgroundColor: '#d3e3f6',
      color: '#0000cc'
    };

    const estiloTitulo = {
      backgroundColor: '#003099',
      padding: '2px 4px',
      fontWeight: 'bold',
      display: 'grid',
      gridTemplateColumns: 'auto max-content',
      gridGap: '5px'
    };

    const estiloPopup = {
      border: '1px solid #003099',
      backgroundColor: '#e2e7ff',
      zIndex: 1,
      transition: 'opacity 0.2s ease-in-out',
      opacity: 0,
      position: 'absolute'
    };

    return {
      exibir: false,
      fecharAtivo: false,
      estiloBotaoFecharNormal,
      estiloBotaoFecharAtivo,
      estiloTitulo,
      estiloPopup
    }
  },

  methods: {
    /**
     * Exibe o tooltip ao passar o mouse sobre o botão (se não houver necessidade de clicar).
     */
    exibirMouse: function () {
      if (!this.precisaClicar) {
        this.exibir = true;
      }
    },

    /**
     * Esconde o tooltip ao retirar o mouse do botão (se não houver necessidade de clicar).
     */
    esconderMouse: function () {
      if (!this.precisaClicar) {
        this.exibir = false;
      }
    },

    /**
     * Exibe o tooltip ao clicar sobre o botão (se houver necessidade).
     * Caso não precise do clique, é disparado um evento para informar o clique.
     * @param {Object} event O evento JavaScript.
     */
    exibirClick: function (event) {
      if (this.precisaClicar) {
        this.exibir = true;
      } else {
        this.$emit('click', event)
      }
    },

    /**
     * Fecha o tooltip atual (quando há clique no botão).
     */
    fechar: function () {
      this.exibir = false;
      this.fecharAtivo = false;
    },

    /**
     * Altera a opacidade do tooltip (para efeito de transição), exibindo-o.
     * @param {Object} elemento O elemento HTML que está sendo exibido.
     */
    mostrar__: function (elemento) {
      this.$emit('exibir', this.objetoEventos__);
      elemento.style.opacity = 1;
    },

    /**
     * Altera a opacidade do tooltip (para efeito de transição), escondendo-o.
     * @param {Object} elemento O elemento HTML que está sendo escondido.
     */
    esconder__: function (elemento) {
      this.$emit('esconder', this.objetoEventos__);
      elemento.style.opacity = 0;
    },

    /**
     * Calcula a posição do tooltip para exibição na tela.
     */
    calcularPosicao__: function () {
      var posicoesBotao = this.$refs.botao.getBoundingClientRect();
      var elemento = this.$refs.popup;

      var xElemento = posicoesBotao.left + window.pageXOffset;
      var yElemento = posicoesBotao.top - elemento.offsetHeight + window.pageYOffset;

      if (window.innerWidth <= (xElemento + elemento.offsetWidth)) {
        xElemento = posicoesBotao.right - elemento.offsetWidth + window.pageXOffset;
      }

      if (yElemento <= 10) {
        yElemento = posicoesBotao.bottom + window.pageYOffset;
      }

      elemento.style.left = xElemento + 'px';
      elemento.style.top = yElemento + 'px';
    }
  },

  computed: {
    /**
     * Propriedade computada com o estilo utilizado pelo botão 'Fechar'.
     * @type {Object}
     */
    estiloBotaoFechar: function () {
      var estilo = this.fecharAtivo
        ? this.estiloBotaoFecharAtivo
        : this.estiloBotaoFecharNormal;

      return this.merge(
        {
          position: 'relative',
          left: '3px',
          padding: '0 2px',
          border: 0,
          cursor: 'pointer',
          fontWeight: 'bold',
          fontSize: '8.5pt'
        },
        estilo
      );
    },

    /**
     * Propriedade computada para indicar o argumento dos eventos de exibição e remoção do tooltip.
     * @type {objetoEvento}
     *
     * @typedef {Object} objetoEvento
     * @function fechar
     */
    objetoEventos__: function () {
      var vm = this;

      return {
        fechar: function () {
          vm.fechar();
        }
      }
    }
  },

  mounted: function () {
    var popup = this.$refs.popup;
    new ResizeObserver(this.calcularPosicao__).observe(popup);
  },

  template: '#ControleTooltip-template'
});
