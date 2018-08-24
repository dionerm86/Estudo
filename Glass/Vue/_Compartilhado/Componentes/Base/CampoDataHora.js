var Data = Data || {};

/**
 * Offset utilizado para os inputs tipo 'date' e 'time'.
 * Necessário para corrigir as horas (os controles tipo 'date' e 'time' consideram hora UTC).
 * @type {number}
 */
Data.offset = new Date().getTimezoneOffset() * 60000;

Vue.component('campo-data-hora', {
  inheritAttrs: false,
  props: {
    /**
     * A data/hora selecionada no controle.
     * @type {?Date}
     */
    dataHora: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarDataOuVazio
    },

    /**
     * Indica se o controle exibe um campo para permitir seleção das horas.
     * @type {?boolean}
     */
    exibirHoras: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Data mínima selecionável pelo controle.
     * @type {?Date}
     */
    dataMinima: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarDataOuVazio
    },

    /**
     * Data máxima selecionável pelo controle.
     * @type {?Date}
     */
    dataMaxima: {
      required: false,
      twoWay: false,
      default: null,
      validator: Mixins.Validacao.validarDataOuVazio
    },

    /**
     * Indica se a seleção de datas permite um dia que seja final de semana.
     * @type {?boolean}
     */
    permitirFimDeSemana: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se a seleção de datas permite um dia que seja feriado.
     * @type {?boolean}
     */
    permitirFeriado: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  methods: {
    /**
     * Realiza a atualização da propriedade de data/hora com base nos
     * valores informados.
     * @param {string} data A data selecionada no controle.
     * @param {string} hora A hora selecionada no controle.
     */
    atualizarDataHora: function(data, hora) {
      var dataHoraAtual = new Date(data + ' ' + hora);
      if (isNaN(dataHoraAtual.getTime())) {
        dataHoraAtual = null;
      }

      if (dataHoraAtual !== this.dataHora) {
        this.$emit('update:dataHora', dataHoraAtual);
      }
    },

    /**
     * Retorna a data formatada como string.
     * @param {Date} dataHora A data/hora a ser formatada.
     * @returns {string} A data no formato ISO.
     */
    formataData: function(data) {
      return new Date(data).toISOString().split('T')[0];
    },

    /**
     * Retorna a hora formatada como string.
     * @param {Date} dataHora A data/hora a ser formatada.
     * @returns {string} A hora no formato ISO.
     */
    formataHora: function(dataHora) {
      return new Date(dataHora)
        .toISOString()
        .split('T')[1]
        .split('.')[0];
    },

    /**
     * Valida a data selecionada, de acordo com a propriedade de dias úteis.
     */
    validarDataSelecionada: function (data) {
      if (!data || (this.permitirFimDeSemana && this.permitirFeriado)) {
        return Promise.resolve();
      }

      var vm = this;

      return Servicos.Datas.validar(data, this.permitirFimDeSemana, this.permitirFeriado)
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          return Promise.reject();
        });
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o valor da data atual para o controle e
     * que atualiza a propriedade em caso de alteração.
     * @type {string}
     */
    dataAtual: {
      get: function() {
        return this.dataHora ? this.formataData(this.dataHora - Data.offset) : '';
      },
      set: function (valor) {
        var vm = this;

        this.validarDataSelecionada(valor)
          .then(function () {
            vm.atualizarDataHora(valor, vm.horaAtual);
          })
          .catch(function () {
            vm.atualizarDataHora(null, vm.horaAtual);
          });
      }
    },

    /**
     * Propriedade computada que retorna o valor da hora atual para o controle
     * e que atualiza a propriedade em caso de alteração.
     * @type {string}
     */
    horaAtual: {
      get: function() {
        return this.dataHora ? this.formataHora(this.dataHora - Data.offset) : '';
      },
      set: function(valor) {
        this.atualizarDataHora(this.dataAtual, valor);
      }
    },

    /**
     * Propridade computada com o valor da data mínima selecionável para o controle.
     * @type {string}
     */
    dataMinimaAtual: function() {
      return this.dataMinima ? this.formataData(this.dataMinima - Data.offset) : '';
    },

    /**
     * Propridade computada com o valor da data máxima selecionável para o controle.
     * @type {string}
     */
    dataMaximaAtual: function() {
      return this.dataMaxima ? this.formataData(this.dataMaxima - Data.offset) : '';
    }
  },

  template: '#CampoDataHora-template'
});