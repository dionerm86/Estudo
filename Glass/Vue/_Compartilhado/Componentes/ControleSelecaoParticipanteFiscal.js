var ControleSelecaoParticipanteFiscal = ControleSelecaoParticipanteFiscal || {};

ControleSelecaoParticipanteFiscal.controles = [];
ControleSelecaoParticipanteFiscal.selecionar = function (idControle, id, nome) {
  if (!ControleSelecaoParticipanteFiscal.controles[idControle]) {
    return;
  }

  ControleSelecaoParticipanteFiscal.controles[idControle].participanteAtual = {
    id,
    nome
  };
};

Vue.component('controle-selecao-participante-fiscal', {
  inheritAttrs: false,
  mixins: [Mixins.Objetos, Mixins.UUID],
  props: {
    /**
     * Participante selecionado no controle.
     * @type {?Object}
     */
    participante: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjetoOuVazio
    },

    /**
     * Tipo do participante selecionado no controle.
     * @type {?number}
     */
    tipoParticipante: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se o controle também exibirá uma administradora de cartão como possível participante.
     * @type {?boolean}
     */
    exibirAdministradoraCartao: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function() {
    return {
      uuid: null,
      tiposParticipantes: []
    }
  },

  methods: {
    /**
     * Busca os tipos de participantes válidos para o controle.
     */
    buscarTiposParticipantes: function () {
      var vm = this;

      Servicos.NotasFiscais.obterTiposParticipantesFiscais(this.exibirAdministradoraCartao)
        .then(function (resposta) {
          vm.tiposParticipantes = resposta.data;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre a tela de busca correspondente ao tipo de participante selecionado.
     */
    abrirTelaBusca: function() {
      if (!this.tipoParticipanteAtual) {
        return;
      }

      const complementosUrl = '?' + [
        'callback=participanteFiscal',
        'controle=' + this.uuid
      ].join('&');

      this.abrirJanela(600, 800, this.tipoParticipanteAtual.urlTelaBusca + complementosUrl);
    }
  },

  computed: {
    /**
     * Propriedade computada que recupera o item selecionado atual e
     * atualiza a propriedade em caso de alteração.
     * @type {?Object}
     */
    idTipoParticipanteAtual: {
      get: function () {
        return this.tipoParticipante;
      },
      set: function (valor) {
        valor = typeof valor === 'number'
          ? valor
          : null;

        if (valor !== this.tipoParticipante) {
          this.$emit('update:tipoParticipante', valor);
        }
      }
    },

    /**
     * Propriedade computada que recupera o item selecionado atual e
     * atualiza a propriedade em caso de alteração.
     * @type {?Object}
     */
    tipoParticipanteAtual: function () {
      if (typeof this.tipoParticipante !== 'number') {
        return null;
      }

      return this.tiposParticipantes
        .filter(function (item) {
          return item.id === this.tipoParticipante;
        }, this)[0];
    },

    /**
     * Propriedade computada que recupera o participante selecionado atual e
     * atualiza a propriedade em caso de alteração.
     * @type {?Object}
     */
    participanteAtual: {
      get: function () {
        return this.participante;
      },
      set: function (valor) {
        if (!this.equivalentes(valor, this.participante)) {
          this.$emit('update:participante', valor);
        }
      }
    }
  },

  mounted: function () {
    this.uuid = this.gerarUuid();
    this.buscarTiposParticipantes();

    ControleSelecaoParticipanteFiscal.controles[this.uuid] = this;
  },

  watch: {
    /**
     * Observador para a propriedade 'buscarTiposParticipantes'.
     * Recarrega os tipos de participantes se houver mudança no parâmetro.
     */
    exibirAdministradoraCartao: function () {
      this.buscarTiposParticipantes();
    }
  },

  template: '#ControleSelecaoParticipanteFiscal-template'
});
