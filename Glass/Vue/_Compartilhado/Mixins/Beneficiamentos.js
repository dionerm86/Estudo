var Beneficiamentos = Beneficiamentos || {};
Beneficiamentos.configuracoes = null;
Beneficiamentos.controle = {
  itens: null,
  carregando: true
};

var Mixins = Mixins || {};

/**
 * Objeto com o mixin para controles de beneficiamentos.
 */
Mixins.Beneficiamentos = {
  props: {
    /**
     * Tipo de beneficiamentos para exibição no controle.
     * @type {!string}
     */
    tipoBeneficiamentos: {
      required: false,
      twoWay: false,
      default: 'Venda',
      validator: function (valor) {
        return Mixins.Validacao.validarValores('Todos', 'Venda', 'MaoDeObraEspecial')(valor);
      }
    },
  },

  data: function() {
    return {
      pronto: false,
      beneficiamentos: null
    };
  },

  created: function() {
    var vm = this;

    if (Beneficiamentos.configuracoes === null) {
      Beneficiamentos.configuracoes = [this];

      Servicos.Beneficiamentos.obterConfiguracoes()
        .then(function (resposta) {
          var controles = Beneficiamentos.configuracoes;
          Beneficiamentos.configuracoes = resposta.data;

          for (var controle of controles) {
            controle.pronto = true;
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          Beneficiamentos.configuracoes = {};
        });
    } else if (Array.isArray(Beneficiamentos.configuracoes)) {
      Beneficiamentos.configuracoes.push(this);
    } else {
      this.pronto = true;
    }

    if (Beneficiamentos.controle.itens === null) {
      Beneficiamentos.controle.itens = [this];

      Servicos.Beneficiamentos.obterParaControle(this.tipoBeneficiamentos)
        .then(function (resposta) {
          var controles = Beneficiamentos.controle.itens;

          Beneficiamentos.controle.carregando = false;
          Beneficiamentos.controle.itens = resposta.data;

          for (var controle of controles) {
            controle.beneficiamentos = Beneficiamentos.controle.itens;
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }

          Beneficiamentos.controle.itens = {};
        });
    } else if (Beneficiamentos.controle.carregando && Array.isArray(Beneficiamentos.controle.itens)) {
      Beneficiamentos.controle.itens.push(this);
    } else {
      this.beneficiamentos = Beneficiamentos.controle.itens;
    }
  }
}
