const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('id', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    tabelaDescontoAcrescimoCliente: {},
    tabelaDescontoAcrescimoClienteOriginal: {}
  },

  methods: {
    /**
     * Busca a lista de tabelas de desconto/acréscimo de cliente.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.TabelasDescontoAcrescimoCliente.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Abre um popup para editar dados da tabela de desconto/acréscimo do cliente.
     * @param {Object} item A tabela de desconto/acréscimo do cliente que será usada para alterar os dados.
     */
    abrirDadosTabelaDescontoAcrescimo: function (item) {
      this.abrirJanela(500, 650, '../Cadastros/CadDescontoAcrescimoCliente.aspx?idTabelaDesconto=' + item.id);
    },

    /**
     * Exibe um relatório com a listagem das tabela de desconto/acréscimo do cliente.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaTabelasDescontoAcrescimo: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ListaTabelaCliente&exportarExcel=' + exportarExcel);
    },

    /**
     * Inicia o cadastro da tabela de desconto/acréscimo de cliente.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma tabela de desconto/acréscimo de cliente será editada.
     * @param {Object} tabelaDescontoAcrescimoCliente A tabela de desconto/acréscimo de cliente que será editada.
     * @param {number} numeroLinha O número da linha que contém a tabela de desconto/acréscimo de cliente que será editada.
     */
    editar: function (tabelaDescontoAcrescimoCliente, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(tabelaDescontoAcrescimoCliente);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma tabela de desconto/acréscimo de cliente.
     * @param {Object} tipoCliente A tabela de desconto/acréscimo de cliente que será excluída.
     */
    excluir: function (tabelaDescontoAcrescimoCliente) {
      if (!this.perguntar('Confirmação', 'Deseja excluir essa tabela de desconto/acréscimo?')) {
        return;
      }

      var vm = this;

      Servicos.TabelasDescontoAcrescimoCliente.excluir(tabelaDescontoAcrescimoCliente.id)
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
     * Insere uma nova tabela de desconto/acréscimo de cliente.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.TabelasDescontoAcrescimoCliente.inserir(this.tabelaDescontoAcrescimoCliente)
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
     * Atualiza os dados da tabela de desconto/acréscimo de cliente.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var tabelaDescontoAcrescimoClienteAtualizar = this.patch(this.tabelaDescontoAcrescimoCliente, this.tipoClienteOriginal);
      var vm = this;

      Servicos.TabelasDescontoAcrescimoCliente.atualizar(this.tabelaDescontoAcrescimoCliente.id, tabelaDescontoAcrescimoClienteAtualizar)
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
     * Cancela a edição ou cadastro da tabela de desconto/acréscimo de cliente.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de tabela de desconto/acréscimo de cliente.
     * @param {?Object} [tabelaDescontoAcrescimoCliente=null] A tabela de desconto/acréscimo de cliente que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (tabelaDescontoAcrescimoCliente) {
      this.tabelaDescontoAcrescimoCliente = {
        id: tabelaDescontoAcrescimoCliente ? tabelaDescontoAcrescimoCliente.id : null,
        nome: tabelaDescontoAcrescimoCliente ? tabelaDescontoAcrescimoCliente.nome : null,
      };

      this.tabelaDescontoAcrescimoClienteOriginal = this.clonar(this.tabelaDescontoAcrescimoCliente);
    },

    /**
     * Função que indica se o formulário de tabela de desconto/acréscimo de cliente possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de tabelas de desconto/acréscimo de cliente, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  }
});
