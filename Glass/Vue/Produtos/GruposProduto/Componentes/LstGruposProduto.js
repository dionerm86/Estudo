const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('descricao', 'asc')],

  data: {
    configuracoes: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    grupoProduto: {},
    grupoProdutoOriginal: {},
    tipoAtual: {},
    tipoCalculoPedidoAtual: {},
    tipoCalculoNotaFiscalAtual: {}
  },

  methods: {
    /**
     * Busca a lista de grupos de produto para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function(filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.Produtos.Grupos.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna o link para a tela de subgrupos deste grupo.
     */
    obterLinkSubgrupos: function (item) {
      return 'CadSubgrupoProduto.aspx?IdGrupoProduto=' + item.id + '&DescrGrupo=' + item.nome;
    },

    /**
     * Ativa os produtos do grupo informado.
     * @param {!Object} grupoProduto O grupo de produto que terá os produtos ativados/inativados.
     * @param {!number} situacao A situação que será alterada nos produtos.
     */
    alterarSituacaoProdutos: function (grupoProduto, situacao) {
      if (!situacao) {
        this.exibirMensagem('Erro', 'A situação é obrigatória');
        return;
      }

      if (!this.perguntar('Validação', 'Tem certeza que deseja ' + (situacao === this.configuracoes.situacaoAtiva ? 'ativar' : 'inativar') + ' todos os produtos deste grupo?')) {
        return;
      }

      var vm = this;

      Servicos.Produtos.alterarSituacaoPorGrupoProduto(grupoProduto.id, situacao)
        .then(function (resposta) {
          vm.exibirMensagem('Concluído', 'Produtos ' + (situacao === vm.configuracoes.situacaoAtiva ? 'ativados.' : 'inativados.'));
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Retorna os itens para o controle de tipos de grupo de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipo: function () {
      return Servicos.Produtos.Grupos.obterTipos();
    },

    /**
     * Retorna os itens para o controle de tipos de cálculo de pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipoCalculoPedido: function () {
      return Servicos.Produtos.Grupos.obterTiposCalculo(false);
    },

    /**
     * Retorna os itens para o controle de tipos de cálculo de nota fiscal.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipoCalculoNotaFiscal: function () {
      return Servicos.Produtos.Grupos.obterTiposCalculo(true);
    },

    /**
     * Exibe um relatório com a listagem de grupos de produto.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaGruposProduto: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ListaSubgrupo' + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Inicia o cadastro de grupo de produto.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um grupo de produto será editado.
     * @param {Object} grupoProduto O grupo de produto que será editado.
     * @param {number} numeroLinha O número da linha que contém o grupo de produto que será editado.
     */
    editar: function (grupoProduto, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(grupoProduto);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um grupo de produto.
     * @param {Object} grupoProduto O grupo de produto que será excluído.
     */
    excluir: function (grupoProduto) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este grupo de produto?')) {
        return;
      }

      var vm = this;

      Servicos.Produtos.Grupos.excluir(grupoProduto.id)
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
     * Insere um novo grupo de produto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Produtos.Grupos.inserir(this.grupoProduto)
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
     * Atualiza os dados do grupo de produto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var grupoProdutoAtualizar = this.patch(this.grupoProduto, this.grupoProdutoOriginal);
      var vm = this;

      Servicos.Produtos.Grupos.atualizar(this.grupoProduto.id, grupoProdutoAtualizar)
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
     * Cancela a edição ou cadastro do grupo de produto.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de grupo de produto.
     * @param {?Object} [grupoProduto=null] O grupo de produto que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (grupoProduto) {
      this.tipoAtual = grupoProduto ? this.clonar(grupoProduto.tipo) : null;
      this.tipoCalculoPedidoAtual = grupoProduto && grupoProduto.tiposCalculo ? this.clonar(grupoProduto.tiposCalculo.pedido) : null;
      this.tipoCalculoNotaFiscalAtual = grupoProduto && grupoProduto.tiposCalculo ? this.clonar(grupoProduto.tiposCalculo.notaFiscal) : null;

      this.grupoProduto = {
        id: grupoProduto ? grupoProduto.id : null,
        nome: grupoProduto ? grupoProduto.nome : null,
        tipo: grupoProduto && grupoProduto.tipo ? grupoProduto.tipo.id : null,
        tipoCalculoPedido: grupoProduto && grupoProduto.tiposCalculo && grupoProduto.tiposCalculo.pedido ? grupoProduto.tiposCalculo.pedido.id : null,
        tipoCalculoNotaFiscal: grupoProduto && grupoProduto.tiposCalculo && grupoProduto.tiposCalculo.notaFiscal ? grupoProduto.tiposCalculo.notaFiscal.id : null,
        bloquearEstoque: grupoProduto && grupoProduto.estoque ? grupoProduto.estoque.bloquearEstoque : null,
        alterarEstoque: grupoProduto && grupoProduto.estoque ? grupoProduto.estoque.alterarEstoque : null,
        alterarEstoqueFiscal: grupoProduto && grupoProduto.estoque ? grupoProduto.estoque.alterarEstoqueFiscal : null,
        exibirMensagemEstoque: grupoProduto && grupoProduto.estoque ? grupoProduto.estoque.exibirMensagemEstoque : null,
        geraVolume: grupoProduto && grupoProduto.estoque ? grupoProduto.estoque.geraVolume : null
      };

      this.grupoProdutoOriginal = this.clonar(this.grupoProduto);
    },

    /**
     * Função que indica se o formulário de grupos de produto possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de grupos de produto, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Produtos.Grupos.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },

  watch: {
    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        if (this.grupoProduto) {
          this.grupoProduto.tipo = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoCalculoPedidoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoCalculoPedidoAtual: {
      handler: function (atual) {
        if (this.grupoProduto) {
          this.grupoProduto.tipoCalculoPedido = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoCalculoNotaFiscalAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoCalculoNotaFiscalAtual: {
      handler: function (atual) {
        if (this.grupoProduto) {
          this.grupoProduto.tipoCalculoNotaFiscal = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
