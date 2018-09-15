const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'desc'
    },
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    cheque: {},
    chequeAtual: {},
    chequeOriginal: {},
    caixaDiario: false
  },

  methods: {
    /**
     * Busca os cheques para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de cheques.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Cheques.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de cheques.
     * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
     */
    ordenar: function(campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

    /**
     * Indica que um cheque será editado.
     * @param {Object} cheque O item que será editado.
     * @param {number} numeroLinha O número da linha que contém o item que será editado.
     */
    editar: function (cheque, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(cheque);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Atualiza os dados do cheque.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var chequeAtualizar = this.patch(this.cheque, this.chequeOriginal);
      var vm = this;

      Servicos.Cheques.atualizarCheque(this.chequeAtual.id, chequeAtualizar)
        .then(function (resposta) {
          vm.atualizarLista();
          vm.cancelar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela a edição do cheque.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
    },

    /**
     * Exibe o a localização do cheque.
     * @param {Object} item O cheque que terá a localização exibida.
     */
    abrirLocalizacaoCheque: function (item) {
      this.abrirJanela(150, 500, '../Utils/LocalizacaoCheque.aspx?idCheque=' + item.id);
    },

    /**
     * Exibe os anexos do cheque.
     * @param {Object} item O cheque que terá os anexos exibidos.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(150, 500, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=cheque');
    },

    /**
     * Exibe um relatório com a listagem de cheques aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaCheques: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?Rel=ListaCheque' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Cancela a reapresentação do cheque.
     * @param {Object} item O cheque que terá a reapresentação cancelada.
     */
    cancelarReapresentacao: function (item) {
      if (!this.perguntar("Tem certeza que deseja cancelar a reapresentação deste cheque?")) {
        return;
      }

      var vm = app;

      Servicos.Cheques.cancelarReapresentacao(item.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela a devolução do cheque.
     * @param {Object} item O cheque que terá a devolução cancelada.
     */
    cancelarDevolucao: function (item) {
      if (!this.perguntar("Tem certeza que deseja cancelar a devolução deste cheque?")) {
        return;
      }

      var vm = app;

      Servicos.Cheques.cancelarDevolucao(item.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela o protesto do cheque.
     * @param {Object} item O cheque que terá o protesto cancelada.
     */
    cancelarProtesto: function (item) {
      if (!this.perguntar("Tem certeza que deseja cancelar o protesto deste cheque?")) {
        return;
      }

      var vm = app;

      Servicos.Cheques.cancelarProtesto(item.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Função executada para criação dos objetos necessários para edição do cheque.
     * @param {?Object} [cheque=null] O cheque que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (cheque) {
      this.chequeAtual = cheque;

      this.cheque = {
        numeroCheque: cheque ? cheque.numeroCheque : null,
        digitoNumeroCheque: cheque ? cheque.digitoNumeroCheque : null,
        banco: cheque ? cheque.banco : null,
        agencia: cheque ? cheque.agencia : null,
        conta: cheque ? cheque.conta : null,
        titular: cheque ? cheque.titular : null,
        dataVencimento: cheque ? cheque.dataVencimento : null,
        observacao: cheque ? cheque.observacao : null,
      };

      this.chequeOriginal = this.clonar(this.cheque);
    },

    /**
     * Função que indica se o formulário possui valores válidos de acordo com os controles.
     * @param {Object} botao O botão que foi disparado no controle.
     * @returns {boolean} Um valor que indica se o formulário está válido.
     */
    validarFormulario_: function (botao) {
      var form = botao.form || botao;
      while (form.tagName.toLowerCase() !== 'form') {
        form = form.parentNode;
      }

      return form.checkValidity();
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('idLoja', this.filtro.idLoja);
      incluirFiltro('idPedido', this.filtro.idPedido);
      incluirFiltro('idLiberarPedido', this.filtro.idLiberacao);
      incluirFiltro('idAcerto', this.filtro.idAcerto);
      incluirFiltro('numeroNfe', this.filtro.numeroNfe);
      incluirFiltro('tipo', this.filtro.tipo);
      incluirFiltro('numCheque', this.filtro.numeroCheque);
      incluirFiltro('situacao', this.filtro.situacao);
      incluirFiltro('reapresentado', this.filtro.reapresentado);
      incluirFiltro('advogado', this.filtro.advogado);
      incluirFiltro('titular', this.filtro.titular);
      incluirFiltro('agencia', this.filtro.agencia);
      incluirFiltro('conta', this.filtro.conta);
      incluirFiltro('dataIni', this.filtro.periodoVencimentoInicio ? this.filtro.periodoVencimentoInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataFim', this.filtro.periodoVencimentoFim ? this.filtro.periodoVencimentoFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataCadIni', this.filtro.periodoCadastroInicio ? this.filtro.periodoCadastroInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataCadFim', this.filtro.periodoCadastroFim ? this.filtro.periodoCadastroFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('cpfCnpj', this.filtro.cpfCnpj);
      incluirFiltro('idCli', this.filtro.idCliente);
      incluirFiltro('nomeCli', this.filtro.nomeCliente);
      incluirFiltro('idFornec', this.filtro.idFornecedor);
      incluirFiltro('nomeFornec', this.filtro.nomeFornecedor);
      incluirFiltro('valorInicial', this.filtro.valorChequeInicio);
      incluirFiltro('valorFinal', this.filtro.valorChequeFim);
      incluirFiltro('nomeUsuCad', this.filtro.usuarioCadastro);
      incluirFiltro('chequesCaixaDiario', this.caixaDiario);
      incluirFiltro('idsRotas', this.filtro.idsRota);
      incluirFiltro('obs', this.filtro.observacao);
      incluirFiltro('ordenacao', this.filtro.ordenacaoFiltro);
      incluirFiltro('agrupar', this.filtro.agruparCliente);

      return filtros.length > 0
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  computed: {
    /**
     * Propriedade computada que indica a ordenação para a lista.
     * @type {string}
     */
    ordenacao: function() {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  mounted: function() {
    var vm = this;

    this.caixaDiario = GetQueryString('caixaDiario') == 'true';

    Servicos.Cheques.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
