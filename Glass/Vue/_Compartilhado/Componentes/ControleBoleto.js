Vue.component('controle-boleto', {
  inheritAttrs: false,
  mixins: [Mixins.Comparar, Mixins.FiltroQueryString],
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
    }
  },

  data: function() {
    return {
      tooltipBoleto: 'Boleto'
    }
  },

  methods: {
    /**
     * Exibe o boleto para impressão
     */
    exibirBoleto: function () {
      if (!this.idNotaFiscal
        && !this.idContaReceber
        && !this.idLiberacao
        && !this.idConhecimentoTransporte) {

        return false;
      }

      var vm = this;

      Servicos.Boletos.validarBoleto(this.idNotaFiscal)
        .then(function (resposta) {
          var filtro = {
            codigoNotaFiscal: vm.idNotaFiscal,
            codigoContaReceber: vm.idContaReceber,
            codigoLiberacao: vm.idLiberacao,
            codigoCte: vm.idConhecimentoTransporte
          };

          const url = '../Relatorios/Boleto/Imprimir.aspx?'
            + vm.formatarFiltro(filtro);

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

      Servicos.Boletos.obterIdNotaFiscalPeloIdContaReceber(this.idContaReceber)
        .then(function (resposta) {
          vm.idNotaFiscal = resposta.idNotaFiscal;
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

      Servicos.Boletos.obterMensagemBoletoImpresso(
        this.idNotaFiscal,
        this.idContaReceber,
        this.idLiberacao,
        this.idConhecimentoTransporte
      )
        .then(function (resposta) {
          vm.tooltipBoleto = 'Boleto ' + (resposta.data.mensagem || '');
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
