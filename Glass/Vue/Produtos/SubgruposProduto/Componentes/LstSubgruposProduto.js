const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.Patch, Mixins.Merge, Mixins.Comparar],

  data: {
    dadosOrdenacao_: {
      campo: 'descricao',
      direcao: 'asc'
    },
    configuracoes: {},
    grupoVidro: false,
    numeroLinhaEdicao: -1,
    inserindo: false,
    idGrupoProduto: GetQueryString('idGrupoProduto'),
    subgrupoProduto: {},
    subgrupoProdutoOriginal: {},
    clienteAtual: {},
    tipoAtual: {},
    tipoCalculoPedidoAtual: {},
    tipoCalculoNotaFiscalAtual: {}
  },

  methods: {
    /**
     * Busca a lista de subgrupos de produto para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      filtro = {
        idGrupoProduto: this.idGrupoProduto
      };

      return Servicos.Produtos.Subgrupos.obter(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de subgrupos de produto.
     * @param {string} campo O nome do campo pelo qual o resultado será ordenado.
     */
    ordenar: function (campo) {
      if (campo !== this.dadosOrdenacao_.campo) {
        this.dadosOrdenacao_.campo = campo;
        this.dadosOrdenacao_.direcao = '';
      } else {
        this.dadosOrdenacao_.direcao = this.dadosOrdenacao_.direcao === '' ? 'desc' : '';
      }
    },

    /**
     * Ativa os produtos do subgrupo informado.
     * @param {!Object} subgrupoProduto O subgrupo de produto que terá os produtos ativados/inativados.
     * @param {!number} situacao A situação que será alterada nos produtos.
     */
    alterarSituacaoProdutos: function (subgrupoProduto, situacao) {
      if (!situacao) {
        this.exibirMensagem('Erro', 'A situação é obrigatória');
        return;
      }

      if (!this.perguntar('Validação', 'Tem certeza que deseja ' + (situacao === this.configuracoes.situacaoAtiva ? 'ativar' : 'inativar') + ' todos os produtos deste subgrupo?')) {
        return;
      }

      var vm = this;

      Servicos.Produtos.alterarSituacaoPorSubgrupoProduto(subgrupoProduto.id, situacao)
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
     * Retorna os itens para o controle de tipos de subgrupo de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipo: function () {
      return Servicos.Produtos.Subgrupos.obterTipos();
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
     * Retorna os itens para o controle de lojas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensLojas: function () {
      return Servicos.Lojas.obterParaControle();
    },

    /**
     * Retorna os nomes das lojas associadas ao subgrupo de produto informado.
     * @param {Object} subgrupoProduto O subgrupo de produto que terá as lojas listadas.
     * @returns {string} Os nomes das lojas associadas do subgrupo de produto.
     */
    obterNomesLojasAssociadas: function(subgrupoProduto) {
      if (!subgrupoProduto || !subgrupoProduto.lojasAssociadas) {
        return '';
      }

      return subgrupoProduto.lojasAssociadas.slice()
        .map(s => s.nome)
        .sort()
        .join(', ');
    },

    /**
     * Inicia o cadastro de subgrupo de produto.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que um subgrupo de produto será editado.
     * @param {Object} subgrupoProduto O subgrupo de produto que será editado.
     * @param {number} numeroLinha O número da linha que contém o subgrupo de produto que será editado.
     */
    editar: function (subgrupoProduto, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(subgrupoProduto);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui um subgrupo de produto.
     * @param {Object} subgrupoProduto O subgrupo de produto que será excluído.
     */
    excluir: function (subgrupoProduto) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este subgrupo de produto?')) {
        return;
      }

      var vm = this;

      Servicos.Produtos.Subgrupos.excluir(subgrupoProduto.id)
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
     * Insere um novo subgrupo de produto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      if (this.subgrupoProduto.permitirItemRevendaNaVenda && this.subgrupoProduto.geraVolume) {
        this.exibirMensagem('Validação', 'Para habilitar a permissão de item de revenda na venda, a opção "Gera Volume" deve estar desmarcada.');
        return;
      }

      var vm = this;

      Servicos.Produtos.Subgrupos.inserir(this.subgrupoProduto)
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
     * Atualiza os dados do subgrupo de produto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var tipoCalculoPedidoAtual = this.subgrupoProdutoOriginal.tipoCalculoPedido;
      var tipoCalculoPedidoNovo = this.subgrupoProduto.tipoCalculoPedido
      var tipoCalculoNotaFiscalAtual = this.subgrupoProdutoOriginal.tipoCalculoPedido;
      var tipoCalculoNotaFiscalNovo = this.subgrupoProduto.tipoCalculoPedido

      if (this.grupoVidro
        && ((tipoCalculoPedidoAtual == this.configuracoes.tipoCalculoQuantidade && tipoCalculoPedidoNovo == this.configuracoes.tipoCalculoMetroQuadrado)
          || (tipoCalculoNotaFiscalAtual == this.configuracoes.tipoCalculoQuantidade && tipoCalculoNotaFiscalNovo == this.configuracoes.tipoCalculoMetroQuadrado))
        && !this.perguntar('Confirmação', 'Ao alterar o cálculo deste subgrupo, os dados Altura, largura, processo e aplicação serão perdidos ao atualizar os produtos.\nDeseja realmente efetuar alteração?')) {
        return;
      }

      if (this.subgrupoProduto.permitirItemRevendaNaVenda && this.subgrupoProduto.geraVolume) {
        this.exibirMensagem('Validação', 'Para habilitar a permissão de item de revenda na venda, a opção "Gera Volume" deve estar desmarcada.');
        return;
      }

      var subgrupoProdutoAtualizar = this.patch(this.subgrupoProduto, this.subgrupoProdutoOriginal);
      var vm = this;

      Servicos.Produtos.Subgrupos.atualizar(this.subgrupoProduto.id, subgrupoProdutoAtualizar)
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
     * Cancela a edição ou cadastro do subgrupo de produto.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de subgrupo de produto.
     * @param {?Object} [subgrupoProduto=null] O subgrupo de produto que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (subgrupoProduto) {
      this.clienteAtual = subgrupoProduto ? this.clonar(subgrupoProduto.cliente) : null;
      this.tipoAtual = subgrupoProduto ? this.clonar(subgrupoProduto.tipo) : null;
      this.tipoCalculoPedidoAtual = subgrupoProduto && subgrupoProduto.tiposCalculo ? this.clonar(subgrupoProduto.tiposCalculo.pedido) : null;
      this.tipoCalculoNotaFiscalAtual = subgrupoProduto && subgrupoProduto.tiposCalculo ? this.clonar(subgrupoProduto.tiposCalculo.notaFiscal) : null;

      this.subgrupoProduto = {
        id: subgrupoProduto ? subgrupoProduto.id : null,
        idCliente: subgrupoProduto && subgrupoProduto.cliente ? subgrupoProduto.cliente.id : null,
        idGrupoProduto: this.idGrupoProduto,
        nome: subgrupoProduto ? subgrupoProduto.nome : null,
        tipo: subgrupoProduto && subgrupoProduto.tipo ? subgrupoProduto.tipo.id : null,
        tipoCalculoPedido: subgrupoProduto && subgrupoProduto.tiposCalculo && subgrupoProduto.tiposCalculo.pedido ? subgrupoProduto.tiposCalculo.pedido.id : null,
        tipoCalculoNotaFiscal: subgrupoProduto && subgrupoProduto.tiposCalculo && subgrupoProduto.tiposCalculo.notaFiscal ? subgrupoProduto.tiposCalculo.notaFiscal.id : null,
        produtoParaEstoque: subgrupoProduto && subgrupoProduto.estoque ? subgrupoProduto.estoque.produtoParaEstoque : null,
        vidroTemperado: subgrupoProduto ? subgrupoProduto.vidroTemperado : null,
        bloquearEstoque: subgrupoProduto && subgrupoProduto.estoque ? subgrupoProduto.estoque.bloquearEstoque : null,
        alterarEstoque: subgrupoProduto && subgrupoProduto.estoque ? subgrupoProduto.estoque.alterarEstoque : null,
        alterarEstoqueFiscal: subgrupoProduto && subgrupoProduto.estoque ? subgrupoProduto.estoque.alterarEstoqueFiscal : null,
        exibirMensagemEstoque: subgrupoProduto && subgrupoProduto.estoque ? subgrupoProduto.estoque.exibirMensagemEstoque : null,
        geraVolume: subgrupoProduto && subgrupoProduto.estoque ? subgrupoProduto.estoque.geraVolume : null,
        bloquearVendaECommerce: subgrupoProduto && subgrupoProduto.estoque ? subgrupoProduto.estoque.bloquearVendaECommerce : null,
        diasMinimoEntrega: subgrupoProduto && subgrupoProduto.entrega && subgrupoProduto.entrega.diaSemana ? subgrupoProduto.entrega.diaSemana.id : null,
        diaSemanaEntrega: subgrupoProduto && subgrupoProduto.entrega && subgrupoProduto.entrega.diaSemana ? subgrupoProduto.entrega.diaSemana.id : null,
        liberarPendenteProducao: subgrupoProduto ? subgrupoProduto.liberarPendenteProducao : null,
        permitirItemRevendaNaVenda: subgrupoProduto ? subgrupoProduto.permitirItemRevendaNaVenda : null,
        idsLojasAssociadas: subgrupoProduto && subgrupoProduto.lojasAssociadas ? subgrupoProduto.lojasAssociadas.map(f => f.id) : null
      };

      this.subgrupoProdutoOriginal = this.clonar(this.subgrupoProduto);
    },

    /**
     * Função que indica se o formulário de subgrupos de produto possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de subgrupos de produto, com base no filtro atual.
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
    ordenacao: function () {
      var direcao = this.dadosOrdenacao_.direcao ? ' ' + this.dadosOrdenacao_.direcao : '';
      return this.dadosOrdenacao_.campo + direcao;
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Produtos.Subgrupos.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
        vm.grupoVidro = vm.idGrupoProduto === vm.configuracoes.idGrupoVidro;
      });
  },

  watch: {
    /**
     * Observador para a variável 'clienteAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    clienteAtual: {
      handler: function (atual) {
        if (this.subgrupoProduto) {
          this.subgrupoProduto.idCliente = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        if (this.subgrupoProduto) {
          this.subgrupoProduto.tipo = atual ? atual.id : null;
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
        if (this.subgrupoProduto) {
          this.subgrupoProduto.tipoCalculoPedido = atual ? atual.id : null;
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
        if (this.subgrupoProduto) {
          this.subgrupoProduto.tipoCalculoNotaFiscal = atual ? atual.id : null;
        }
      },
      deep: true
    }
  }
});
