﻿const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.ExecutarTimeout],

  data: {
    inserindo: false,
    editando: false,
    situacaoAtual: {},
    formaPagamentoAtual: {},
    diaAtual: {},
    parcela: {},
    parcelaOriginal: {},
    configuracoes: {},
    exibirDias: false
  },

  methods: {

    /**
     * Retorna os itens para a lista de situação.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return Servicos.Parcelas.situacoes();
    },

    /**
     * Retorna os itens para a lista de Tipo Pagto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterFormaPagto: function () {
      return Servicos.Parcelas.tiposPagamento();
    },

    iniciarCadastroOuAtualizacao_: function (item) {
      this.parcelaOriginal = item ? this.clonar(item) : {};
      this.situacaoAtual = item ? this.clonar(item.situacao) : null;
      this.diaAtual = item ? item.diaAtual : null;

      this.parcela = {
        id: item && item.id ? item.id : null,
        descricao: item ? item.descricao : null,
        parcelaPadrao: item ? item.parcelaPadrao : null,
        situacao: item ? item.situacao : null,
        parcelaAvista: item ? item.parcelaAvista : null,
        dias: item ? item.dias : null,
        numeroParcelas: item ? item.dias.length : null
      }
    },

    /**
     * Insere a parcela, se possível.
     * @param {Object} event O objeto do evento JavaScript.
     */
    inserirParcela: function () {
      if (!this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      this.excluirDiasTipoPagamento();

      Servicos.Parcelas.inserir(this.parcela)
        .then(function (resposta) {
          var url = '../Listas/LstParcelas.aspx';

          window.location.assign(url);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Atualiza a parcela, se possível.
     * @param {Object} event O objeto do evento JavaScript.
     */
    atualizarParcela: function () {
      if (!this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      this.excluirDiasTipoPagamento();

      var parcelaAtualizar = this.patch(this.parcela, this.parcelaOriginal);
      parcelaAtualizar.dias = this.parcela.dias;

      Servicos.Parcelas.atualizar(vm.parcela.id, vm.parcela)
        .then(function (resposta) {
          var url = '../Listas/LstParcelas.aspx';

          window.location.assign(url);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * cancela a edição ou inserção da parcela.
     * @param {Object} event O objeto do evento JavaScript.
     */
    cancelar: function () {
      var url = '../Listas/LstParcelas.aspx';
      window.location.assign(url);
    },

    inserirDia: function () {
      if (this.diaAtual) {
        var dias = this.parcela.dias;

        if (dias == null) {
          dias = [];
        }
        dias.push(this.diaAtual);
        dias = dias
          .map(function (item) {
            return parseInt(item, 10);
          })
          .sort(function (a, b) {
            return a - b;
          });

        this.parcela.dias = dias;
        this.diaAtual = '';
      }
    },

    excluirDia: function (dia) {
      var posicaoDia = this.parcela.dias.indexOf(dia);
      this.parcela.dias.splice(posicaoDia, 1);
    },

    excluirDiasTipoPagamento: function () {
      if (this.formaPagamentoAtual.id == this.configuracoes.tipoPagamentoAVista) {
        this.parcela.numeroParcelas = 0;
        this.parcela.dias = [];
      }
    },

    /**
     * Busca os dados de uma parcela.
     * @param {?number} id O id da parcela.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    buscarParcela: function (id) {
      var vm = this;

      return Servicos.Parcelas.obterParcela(id)
        .then(function (resposta) {
          vm.iniciarCadastroOuAtualizacao_(resposta.data);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Função que indica se o formulário de pedido possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      return form.checkValidity();
    }
  },

  watch: {
    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        if (this.parcela) {
          this.parcela.situacao = atual ? atual.id : null;
        }
        deep: true
      }
    },

    /**
     * Observador para a variável 'formaPagamentoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    formaPagamentoAtual: {
      handler: function (atual) {
        this.exibirDias = atual && atual.id == 0;
        if (this.parcela) {
          this.parcela.parcelaAvista = atual ? atual.id : null;
        }
        deep: true
      },
    }
  },

  mounted: function () {
    var idParcela = GetQueryString('idParcela');
    var vm = this;

    Servicos.Parcelas.obterConfiguracoesCadastroParcelas(idParcela || 0)
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });

    if (!idParcela) {
      vm.inserindo = true;
      vm.iniciarCadastroOuAtualizacao_();
    }

    if (idParcela) {
      vm.editando = true;
      this.buscarParcela(idParcela)
        .then(function (resposta) {
          if (!vm.parcela) {
            var url = '../Listas/LstParcelas.aspx';
            window.location.assign(url);
          }

          vm.formaPagamentoAtual = {
            id: vm.parcela.dias && vm.parcela.dias.length ? vm.configuracoes.tipoPagamentoAPrazo : vm.configuracoes.tipoPagamentoAVista
          };
        });
    }
  }
});
