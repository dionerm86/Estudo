const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    numeroLinhaEdicao: -1,
    inserindo: false,
    contabilista: {},
    contabilistaOriginal: {},
    tipoPessoaAtual: {},
    situacaoAtual: {}
  },

  methods: {
    /**
     * Busca a lista de contabilistas para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Contabilistas.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Inicia o cadastro de contabilista.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um contabilista será editado.
     * @param {Object} contabilista O contabilista que será editado.
     * @param {number} numeroLinha O número da linha que contém o contabilista que será editado.
     */
    editar: function (contabilista, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(contabilista);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um contabilista.
     * @param {Object} contabilista O contabilista que será excluído.
     */
    excluir: function (contabilista) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este contabilista?')) {
        return;
      }

      var vm = this;

      Servicos.Contabilistas.excluir(contabilista.id)
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
     * Insere um novo contabilista.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Contabilistas.inserir(this.contabilista)
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
     * Atualiza os dados do contabilista.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var contabilistaAtualizar = this.patch(this.contabilista, this.contabilistaOriginal);
      var vm = this;

      Servicos.Contabilistas.atualizar(this.contabilista.id, contabilistaAtualizar)
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
     * Cancela a edição ou cadastro do contabilista.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de contabilista.
     * @param {?Object} [contabilista=null] O contabilista que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (contabilista) {
      this.situacaoAtual = contabilista ? this.clonar(contabilista.situacao) : null;
      this.tipoPessoaAtual = contabilista ? this.clonar(contabilista.tipoPessoa) : null;

      this.contabilista = {
        id: contabilista ? contabilista.id : null,
        nome: contabilista ? contabilista.nome : null,
        tipoPessoa: contabilista && contabilista.tipoPessoa ? contabilista.tipoPessoa.id : null,
        cpfCnpj: contabilista ? contabilista.cpfCnpj : null,
        crc: contabilista ? contabilista.crc : null,
        dadosContato: {
          telefone: contabilista && contabilista.dadosContato ? contabilista.dadosContato.telefone : null,
          fax: contabilista && contabilista.dadosContato ? contabilista.dadosContato.fax : null,
          email: contabilista && contabilista.dadosContato ? contabilista.dadosContato.email : null
        },
        endereco: {
          logradouro: contabilista && contabilista.endereco ? contabilista.endereco.logradouro : null,
          numero: contabilista && contabilista.endereco ? contabilista.endereco.numero : null,
          complemento: contabilista && contabilista.endereco ? contabilista.endereco.complemento : null,
          bairro: contabilista && contabilista.endereco ? contabilista.endereco.bairro : null,
          cidade: {
            id: contabilista && contabilista.endereco && contabilista.endereco.cidade ? contabilista.endereco.cidade.id : null,
            nome: contabilista && contabilista.endereco && contabilista.endereco.cidade ? contabilista.endereco.cidade.nome : null,
            uf: contabilista && contabilista.endereco && contabilista.endereco.cidade ? contabilista.endereco.cidade.uf : null
          },
          cep: contabilista && contabilista.endereco ? contabilista.endereco.cep : null,
        }
      };

      this.contabilistaOriginal = this.clonar(this.contabilista);
    },

    /**
     * Função que indica se o formulário de contabilistas possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de contabilistas, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  watch: {
    /**
     * Observador para a variável 'tipoPessoaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoPessoaAtual: {
      handler: function (atual) {
        if (this.contabilista) {
          this.contabilista.tipoPessoa = atual ? atual.id : null;
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
        if (this.contabilista) {
          this.contabilista.situacao = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
