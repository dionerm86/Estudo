const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('CpfCnpj', 'ASC')],

  data: {
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    limiteCheque: {},
    limiteChequeAtual: {}
  },

  methods: {
    /**
     * Busca os limites de cheques para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de cheques.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Cheques.LimitePorCpfCnpj.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Indica que um limite de cheque será editado.
     * @param {Object} limiteCheque O item que será editado.
     * @param {number} numeroLinha O número da linha que contém o item que será editado.
     */
    editar: function (limiteCheque, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(limiteCheque);
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

      var vm = this;

      Servicos.Cheques.LimitePorCpfCnpj.atualizarLimiteCheque(this.limiteChequeAtual.id, this.limiteCheque)
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
     * Cancela a edição do limite de cheque.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
    },

    /**
     * Função executada para criação dos objetos necessários para edição do limite de cheque.
     * @param {?Object} [limiteCheque=null] O limite de cheque que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (limiteCheque) {
      this.limiteChequeAtual = limiteCheque;

      this.limiteCheque = {
        cpfCnpj: limiteCheque ? this.formatarCpfCnpj(limiteCheque.cpfCnpj) : null,
        limite: limiteCheque
          ? limiteCheque.limite 
            ? limiteCheque.limite.total
            : null
          : null,
        observacao: limiteCheque ? limiteCheque.observacao : null
      };
    },

    /**
     * Formata o cpf/cnpj para inserção ou alteração.
     * @returns {string} O cpf/cnpj formatado.
     */
    formatarCpfCnpj(cpfcnpj) {
      while (cpfcnpj.indexOf('.') > -1) {
        cpfcnpj = cpfcnpj.replace('.', '');
      }

      while (cpfcnpj.indexOf('/') > -1) {
        cpfcnpj = cpfcnpj.replace('/', '');
      }

      while (cpfcnpj.indexOf('-') > -1) {
        cpfcnpj = cpfcnpj.replace('-', '');
      }

      return cpfcnpj;
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
     * Exibe o relatório detalhado dos limites de cheque.
     * @param {boolean} exportarExcel indica se o relatório será exportado para um arquivo do excel.
     */
    abrirRelatorio: function (exportarExcel) {
      var cpfCnpj = (this.filtro.cpfCnpj != null ? '&cpfCnpj=' + this.filtro.cpfCnpj : '');
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=LimiteChequeCpfCnpj' + cpfCnpj + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Força a atualização da listagem, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Cheques.LimitePorCpfCnpj.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
