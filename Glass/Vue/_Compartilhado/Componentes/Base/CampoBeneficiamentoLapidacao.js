Vue.component('campo-beneficiamento-lapidacao', {
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
      altura: ((this.itensSelecionados || [])[0] || {}).altura || 0,
      largura: ((this.itensSelecionados || [])[0] || {}).largura || 0,
      selecionando: false,
      selectsValidar: [],
      itensSelecionadosAtuais: this.itensSelecionados,
      valido: false
    };
  },

  methods: {
    /**
     * Realiza a validação de item selecionado, garantindo que altura ou largura sejam diferentes de 0.
     */
    validar: function () {
      if (!this.selectsValidar.length) {
        this.selectsValidar = [
          this.$el.children[1],
          this.$el.children[2]
        ];
      }

      var mensagem = '';

      if (this.itensSelecionadosAtuais
        && this.itensSelecionadosAtuais.length
        && Object.keys(this.itensSelecionadosAtuais[0]).length
        && (this.altura + this.largura) === 0) {

        mensagem = 'Pelo menos um dos campos deve ter valor diferente de 0.';
      }

      for (var select of this.selectsValidar) {
        select.setCustomValidity(mensagem);
      }

      if (!mensagem !== this.valido) {
        this.valido = !mensagem;
        this.$emit('validation-state-change', this.valido);
      }
    }
  },

  mounted: function() {
    this.validar();
  },

  computed: {
    /**
     * Propriedade que indica se a empresa trabalha com altura x largura (ou se é largura x altura).
     * @type {!boolean}
     */
    empresaTrabalhaComAlturaELargura: function () {
      return typeof Beneficiamentos.configuracoes === 'object'
        && Beneficiamentos.configuracoes.empresaTrabalhaComAlturaELargura;
    }
  },

  watch: {
    /**
     * Observador para a variável 'beneficiamento'.
     * Reinicia os valores selecionados no controle se o beneficiamento for alterado.
     */
    beneficiamento: {
      handler: function () {
        this.altura = 0;
        this.largura = 0;
        this.itensSelecionadosAtuais = null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'itensSelecionados'.
     * Altera os campos internos para o valor do itens selecionados, se houver.
     */
    itensSelecionados: {
      handler: function (atual) {
        if (this.selecionando) {
          return;
        }

        if (atual
          && atual.length
          && (!this.itensSelecionadosAtuais
            || !this.itensSelecionadosAtuais.length
            || this.itensSelecionadosAtuais[0].altura !== (atual[0].altura || 0)
            || this.itensSelecionadosAtuais[0].largura !== (atual[0].largura || 0))) {

          this.altura = atual[0].altura || 0;
          this.largura = atual[0].largura || 0;

          this.itensSelecionadosAtuais = [
            this.criarBeneficiamento({
              id: atual[0].id,
              altura: this.altura,
              largura: this.largura,
              espessura: atual[0].espessura || null
            })
          ];
        } else {
          this.validar();
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'itensSelecionadosAtuais'.
     * Altera a propriedade 'itensSelecionados' com o valor selecionado internamente.
     */
    itensSelecionadosAtuais: {
      handler: function (atual, anterior) {
        this.validar();

        if (this.selecionando) {
          return;
        }

        var inserirValoresPadrao = ((atual && !anterior) || (atual && atual.length && anterior && !anterior.length))
          || ((!atual && anterior) || (atual && !atual.length && anterior && anterior.length))
          || (atual && anterior && atual.length && anterior.length && atual[0].id !== anterior[0].id);

        try {
          this.selecionando = true;
          var item = null;

          if (!atual || !atual.length) {
            if (inserirValoresPadrao && this.altura + this.largura === 4) {
              this.altura = 0;
              this.largura = 0;
            }
          } else {
            if (inserirValoresPadrao && this.altura + this.largura === 0) {
              this.altura = 2;
              this.largura = 2;
            }

            atual[0].altura = this.altura;
            atual[0].largura = this.largura;

            if (atual[0].altura !== this.itensSelecionadosAtuais[0].altura
              || atual[0].largura !== this.itensSelecionadosAtuais[0].largura) {

              this.itensSelecionadosAtuais = atual;
            }

            item = atual;
          }

          if (!this.equivalentes(item, this.itensSelecionados)) {
            this.$emit('update:itensSelecionados', item);
          }
        } finally {
          this.selecionando = false;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'altura'.
     * Altera a altura do item selecionado, se houver.
     */
    altura: function (atual) {
      if (!this.selecionando
        && this.itensSelecionadosAtuais
        && this.itensSelecionadosAtuais.length
        && Object.keys(this.itensSelecionadosAtuais[0]).length) {

        this.itensSelecionadosAtuais[0].altura = atual || 0;
      }
    },

    /**
     * Observador para a variável 'largura'.
     * Altera a largura do item selecionado, se houver.
     */
    largura: function (atual) {
      if (!this.selecionando
        && this.itensSelecionadosAtuais
        && this.itensSelecionadosAtuais.length
        && Object.keys(this.itensSelecionadosAtuais[0]).length) {

        this.itensSelecionadosAtuais[0].largura = atual || 0;
      }
    }
  },

  template: '#CampoBeneficiamentoLapidacao-template'
});
