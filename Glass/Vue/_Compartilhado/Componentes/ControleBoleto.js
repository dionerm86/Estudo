Vue.component('controle-boleto', {
  inheritAttrs: false,
  mixins: [Mixins.Comparar],
  props: {
    /**
     * Identificador da nota fiscal.
     * @type {?number}
     */
    idNotaFiscal: {
      required: false,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Identificador da liberação de pedidos.
     * @type {?number}
     */
    idLiberacao: {
      required: false,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Identificador da conta a receber.
     * @type {?number}
     */
    idContaReceber: {
      required: false,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Identificador do conhecimento de transporte.
     * @type {?number}
     */
    idConhecimentoTransporte: {
      required: false,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Define uma tooltip para o boleto
     * @type {?string}
     */
    tooltipBoleto: {
      required: false,
      twoWay: true,
      default: 'Boleto',
      validator: Mixins.Validacao.validarTextoOuVazio
    }
  },

  methods: {
    /**
     * Exibe o boleto para impressão
     */
    exibirBoleto: function () {
      var vm = this;

      Servicos.NotasFiscais.validarBoleto(this.idNotaFiscal)
        .then(function (resposta) {
          var url = '../Relatorios/Boleto/Imprimir.aspx?codigoNotaFiscal=' + (vm.idNotaFiscal || 0) + '&codigoContaReceber=' + (vm.idContaReceber || 0) + '&codigoLiberacao=' + (vm.idLiberacao || 0);
          vm.abrirJanela(400, 600, url);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Recupera o id da nota fiscal
     */
    buscarIdNotaFiscal: function () {
      if (this.idNotaFiscal > 0 || !this.idContaReceber) {
        return;
      }

      var vm = this;

      Servicos.NotasFiscais.obterIdNotaFiscalPeloIdContaReceber(this.idContaReceber || 0)
        .then(function (resposta) {
          vm.idNotaFiscal = resposta.idNotaFiscal || null;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Verifica se o boleto já foi impresso
     */
    obterMensagemBoletoImpresso: function () {
      var vm = this;

      Servicos.NotasFiscais.obterMensagemBoletoImpresso(this.idNotaFiscal, this.idContaReceber, this.idLiberacao, this.idConhecimentoTransporte)
        .then(function (resposta) {
          vm.tooltipBoleto = 'Boleto' + (resposta.mensagem || '');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    }
  },

  mounted: function () {
    this.buscarIdNotaFiscal();
    this.obterMensagemBoletoImpresso();
  },

  template: '#ControleBoleto-template'
});
