const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('nome', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    contaBancaria: {},
    contaBancariaOriginal: {},
    bancoAtual: {},
    lojaAtual: {},
    situacaoAtual: {}
  },

  methods: {
    /**
     * Busca a lista de contas bancárias para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.ContasBancarias.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de bancos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensBanco: function () {
      return Servicos.ContasBancarias.obterBancos();
    },

    /**
     * Retorna os itens para o controle de situações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensSituacao: function () {
      return Servicos.ContasBancarias.obterSituacoes();
    },

    /**
     * Inicia o cadastro de conta bancária.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma conta bancária será editada.
     * @param {Object} contaBancaria A conta bancária que será editada.
     * @param {number} numeroLinha O número da linha que contém a conta bancária que será editada.
     */
    editar: function (contaBancaria, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(contaBancaria);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui uma conta bancária.
     * @param {Object} contaBancaria A conta bancária que será excluída.
     */
    excluir: function (contaBancaria) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir esta conta bancária?')) {
        return;
      }

      var vm = this;

      Servicos.ContasBancarias.excluir(contaBancaria.id)
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
     * Insere uma nova conta bancária.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.ContasBancarias.inserir(this.contaBancaria)
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
     * Atualiza os dados da conta bancária.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var contaBancariaAtualizar = this.patch(this.contaBancaria, this.contaBancariaOriginal);
      var vm = this;

      Servicos.ContasBancarias.atualizar(this.contaBancaria.id, contaBancariaAtualizar)
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
     * Cancela a edição ou cadastro da conta bancária.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro da conta bancária.
     * @param {?Object} [contaBancaria=null] A conta bancária que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (contaBancaria) {
      this.bancoAtual = contaBancaria && contaBancaria.dadosBanco ? this.clonar(contaBancaria.dadosBanco.banco) : null;
      this.lojaAtual = contaBancaria ? this.clonar(contaBancaria.loja) : null;
      this.situacaoAtual = contaBancaria ? this.clonar(contaBancaria.situacao) : null;

      this.contaBancaria = {
        id: contaBancaria ? contaBancaria.id : null,
        nome: contaBancaria ? contaBancaria.nome : null,
        idLoja: contaBancaria && contaBancaria.loja ? contaBancaria.loja.id : null,
        situacao: contaBancaria && contaBancaria.situacao ? contaBancaria.situacao.id : null,
        dadosBanco: {
          banco: contaBancaria && contaBancaria.dadosBanco && contaBancaria.dadosBanco.banco ? contaBancaria.dadosBanco.banco.id : null,
          titular: contaBancaria && contaBancaria.dadosBanco ? contaBancaria.dadosBanco.titular : null,
          agencia: contaBancaria && contaBancaria.dadosBanco ? contaBancaria.dadosBanco.agencia : null,
          conta: contaBancaria && contaBancaria.dadosBanco ? contaBancaria.dadosBanco.conta : null,
          codigoConvenio: contaBancaria && contaBancaria.dadosBanco ? contaBancaria.dadosBanco.codigoConvenio : null
        },
        cnab: {
          codigoCliente: contaBancaria && contaBancaria.cnab ? contaBancaria.cnab.codigoCliente : null,
          posto: contaBancaria && contaBancaria.cnab ? contaBancaria.cnab.posto : null
        }
      };

      this.contaBancariaOriginal = this.clonar(this.contaBancaria);
    },

    /**
     * Função que indica se o formulário de contas bancárias possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de contas bancárias, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  watch: {
    /**
     * Observador para a variável 'bancoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    bancoAtual: {
      handler: function (atual) {
        if (this.contaBancaria) {
          this.contaBancaria.dadosBanco.banco = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function (atual) {
        if (this.contaBancaria) {
          this.contaBancaria.idLoja = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        if (this.contaBancaria) {
          this.contaBancaria.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
