Vue.component('campo-beneficiamento-bisote', {
  inheritAttrs: false,
  mixins: [Mixins.Comparar, Mixins.Clonar, Mixins.CampoBeneficiamento],
  props: {
    /**
     * Beneficiamento que o controle representa.
     * @type {!Object}
     */
    beneficiamento: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Itens selecionados no controle.
     * @type {?Object[]}
     */
    itensSelecionados: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarArrayOuVazio
    }
  },

  data: function () {
    return {
      espessura: ((this.itensSelecionados || [])[0] || {}).espessura || 0,
      selecionando: false,
      itensSelecionadosAtuais: this.itensSelecionados,
      inputValidar: null,
      lapidacaoValida: false
    };
  },

  methods: {
    /**
     * Realiza a validação de item selecionado, garantindo que altura ou largura sejam diferentes de 0.
     * @param {?boolean} [lapidacaoValida=null] Informa se o controle de lapidação (interno) está válido. Só é informado pelo controle interno.
     */
    validar: function (lapidacaoValida) {
      if (!this.inputValidar) {
        this.inputValidar = this.$el.children[2];
      }

      if (lapidacaoValida !== null && lapidacaoValida !== undefined) {
        this.lapidacaoValida = lapidacaoValida;
      }

      var mensagem = '';

      if (this.itensSelecionadosAtuais
        && this.itensSelecionadosAtuais.length
        && (!this.lapidacaoValida || this.espessura === 0)) {

        mensagem = 'A espessura deve ser diferente de 0.';
      }

      this.inputValidar.setCustomValidity(mensagem);
      this.inputValidar.reportValidity();
    }
  },

  watch: {
    /**
     * Observador para a variável 'beneficiamento'.
     * Reinicia os valores selecionados no controle se o beneficiamento for alterado.
     */
    beneficiamento: {
      handler: function () {
        this.itensSelecionadosAtuais = null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'itensSelecionados'.
     * Altera os campos internos para o valor dos itens selecionados, se houver.
     */
    itensSelecionados: {
      handler: function (atual) {
        if (this.selecionando) {
          return;
        }

        if (atual && atual.length) {
          var item = this.criarBeneficiamento(atual[0]);

          if (!this.itensSelecionadosAtuais
            || !this.itensSelecionadosAtuais.length
            || this.itensSelecionadosAtuais[0].altura !== item.altura
            || this.itensSelecionadosAtuais[0].largura !== item.largura
            || this.itensSelecionadosAtuais[0].espessura !== item.espessura) {

            this.itensSelecionadosAtuais = [item];
            this.espessura = atual[0].espessura || 0;
          } else {
            this.validar();
          }
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'itensSelecionadosAtuais'.
     * Altera a propriedade 'itensSelecionados' com o valor selecionado internamente.
     */
    itensSelecionadosAtuais: {
      handler: function (atual) {
        this.validar();

        if (this.selecionando) {
          return;
        }

        try {
          this.selecionando = true;
          var valor = null;

          if (atual && atual.length && this.espessura > 0) {
            atual[0].espessura = this.espessura || 0;

            if (atual[0].espessura !== this.itensSelecionadosAtuais[0].espessura) {
              this.itensSelecionadosAtuais = atual;
            }

            valor = atual;
          }

          if (!this.equivalentes(valor, this.itensSelecionados)) {
            this.$emit('update:itensSelecionados', valor);
          }
        } finally {
          this.selecionando = false;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'espessura'.
     * Altera a espessura dos itens selecionados, se houver.
     */
    espessura: function (atual) {
      if (!this.selecionando
        && this.itensSelecionadosAtuais
        && this.itensSelecionadosAtuais.length
        && Object.keys(this.itensSelecionadosAtuais[0]).length) {

        this.itensSelecionadosAtuais[0].espessura = atual || 0;
      }
    }
  },

  template: '#CampoBeneficiamentoBisote-template'
});
