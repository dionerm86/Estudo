const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista()],

  data: {
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    regraNatureza: {},
    regraNaturezaOriginal: {},
    lojaAtual: {},
    tipoClienteAtual: {},
    grupoProdutoAtual: {},
    subgrupoProdutoAtual: {},
    corVidroAtual: {},
    corFerragemAtual: {},
    corAluminioAtual: {},
    naturezaOperacaoProducaoIntraestadualAtual: {},
    naturezaOperacaoProducaoInterestadualAtual: {},
    naturezaOperacaoProducaoIntraestadualComStAtual: {},
    naturezaOperacaoProducaoInterestadualComStAtual: {},
    naturezaOperacaoRevendaIntraestadualAtual: {},
    naturezaOperacaoRevendaInterestadualAtual: {},
    naturezaOperacaoRevendaIntraestadualComStAtual: {},
    naturezaOperacaoRevendaInterestadualComStAtual: {},
    ufsDestinoAtuais: null
  },

  methods: {
    /**
     * Busca a lista de regras de natureza de operação para a tela.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Cfops.NaturezasOperacao.Regras.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna as UF's de destino da regra de natureza de operação.
     * @param {Object} item A regra de natureza de operação.
     * @returns {string} As UF's de destino associadas à regra de natureza de operação informada.
     */
    obterUfsDestino: function (item) {
      if (!item || !item.ufsDestino) {
        return '';
      }

      return item.ufsDestino.slice()
        .join(', ');
    },

    /**
     * Retorna os itens para o controle de tipos de cliente.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensTipoCliente: function () {
      return Servicos.Clientes.Tipos.obterParaControle(true);
    },

    /**
     * Retorna os itens para o controle de grupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensGrupoProduto: function () {
      return Servicos.Produtos.Grupos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de subgrupos de produto.
     * @param {?Object} filtro O filtro para a busca de subgrupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensSubgrupoProduto: function (filtro) {
      return Servicos.Produtos.Subgrupos.obterParaControle((filtro || {}).idGrupoProduto);
    },

    /**
     * Retorna os itens para o controle de cor de vidro.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCorVidro: function () {
      return Servicos.Produtos.CoresVidro.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de cor de ferragem.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCorFerragem: function () {
      return Servicos.Produtos.CoresFerragem.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de cor de alumínio.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensCorAluminio: function () {
      return Servicos.Produtos.CoresAluminio.obterParaControle();
    },

    /**
     * Inicia o cadastro de regra de natureza de operação.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Indica que uma regra de natureza de operação será editada.
     * @param {Object} regraNatureza A regra de natureza de operação que será editada.
     * @param {number} numeroLinha O número da linha que contém a regra de natureza de operação que será editada.
     */
    editar: function (regraNatureza, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(regraNatureza);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Exclui ums regra de natureza de operação.
     * @param {Object} regraNatureza A regra de natureza de operação que será excluída.
     */
    excluir: function (regraNatureza) {
      var motivoCancelamento = this.requisitarInformacao('Motivo exclusão', 'Informe o motivo da exclusão:');

      if (!motivoCancelamento) {
        this.exibirMensagem('Aviso', 'O motivo do cancelamento é obrigatório.')
        return;
      }

      var vm = this;

      Servicos.Cfops.NaturezasOperacao.Regras.excluir(regraNatureza.id, motivoCancelamento)
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
     * Insere uma nova regra de natureza de operação.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    inserir: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var vm = this;

      Servicos.Cfops.NaturezasOperacao.Regras.inserir(this.regraNatureza)
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
     * Atualiza os dados da regra de natureza de operação.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function(event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var regraNaturezaAtualizar = this.patch(this.regraNatureza, this.regraNaturezaOriginal);
      var vm = this;

      Servicos.Cfops.NaturezasOperacao.Regras.atualizar(this.regraNatureza.id, regraNaturezaAtualizar)
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
     * Cancela a edição ou cadastro da regra de natureza de operação.
     */
    cancelar: function() {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de regra de natureza de operação.
     * @param {?Object} [regraNatureza=null] A regra de natureza de operação que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (regraNatureza) {
      this.lojaAtual = regraNatureza ? this.clonar(regraNatureza.loja) : null;
      this.tipoClienteAtual = regraNatureza ? this.clonar(regraNatureza.tipoCliente) : null;
      this.grupoProdutoAtual = regraNatureza && regraNatureza.produto ? this.clonar(regraNatureza.produto.grupoProduto) : null;
      this.subgrupoProdutoAtual = regraNatureza && regraNatureza.produto ? this.clonar(regraNatureza.produto.subgrupoProduto) : null;
      this.corVidroAtual = regraNatureza && regraNatureza.produto && regraNatureza.produto.cores ? this.clonar(regraNatureza.produto.cores.vidro) : null;
      this.corFerragemAtual = regraNatureza && regraNatureza.produto && regraNatureza.produto.cores ? this.clonar(regraNatureza.produto.cores.ferragem) : null;
      this.corAluminioAtual = regraNatureza && regraNatureza.produto && regraNatureza.produto.cores ? this.clonar(regraNatureza.produto.cores.aluminio) : null;
      this.naturezaOperacaoProducaoIntraestadualAtual = regraNatureza && regraNatureza.naturezaOperacaoProducao ? this.clonar(regraNatureza.naturezaOperacaoProducao.intraestadual) : null;
      this.naturezaOperacaoProducaoInterestadualAtual = regraNatureza && regraNatureza.naturezaOperacaoProducao ? this.clonar(regraNatureza.naturezaOperacaoProducao.interestadual) : null;
      this.naturezaOperacaoProducaoIntraestadualComStAtual = regraNatureza && regraNatureza.naturezaOperacaoProducao ? this.clonar(regraNatureza.naturezaOperacaoProducao.intraestadualComSt) : null;
      this.naturezaOperacaoProducaoInterestadualComStAtual = regraNatureza && regraNatureza.naturezaOperacaoProducao ? this.clonar(regraNatureza.naturezaOperacaoProducao.interestadualComSt) : null;
      this.naturezaOperacaoRevendaIntraestadualAtual = regraNatureza && regraNatureza.naturezaOperacaoRevenda ? this.clonar(regraNatureza.naturezaOperacaoRevenda.intraestadual) : null;
      this.naturezaOperacaoRevendaInterestadualAtual = regraNatureza && regraNatureza.naturezaOperacaoRevenda ? this.clonar(regraNatureza.naturezaOperacaoRevenda.interestadual) : null;
      this.naturezaOperacaoRevendaIntraestadualComStAtual = regraNatureza && regraNatureza.naturezaOperacaoRevenda ? this.clonar(regraNatureza.naturezaOperacaoRevenda.intraestadualComSt) : null;
      this.naturezaOperacaoRevendaInterestadualComStAtual = regraNatureza && regraNatureza.naturezaOperacaoRevenda ? this.clonar(regraNatureza.naturezaOperacaoRevenda.interestadualComSt) : null;
      this.ufsDestinoAtuais = regraNatureza ? this.clonar(regraNatureza.ufsDestino) : null;
      
      this.regraNatureza = {
        id: regraNatureza ? regraNatureza.id : null,
        idLoja: regraNatureza && regraNatureza.loja ? regraNatureza.loja.id : null,
        idTipoCliente: regraNatureza && regraNatureza.tipoCliente ? regraNatureza.tipoCliente.id : null,
        produto: {
          idGrupoProduto: regraNatureza && regraNatureza.produto && regraNatureza.produto.grupoProduto ? regraNatureza.produto.grupoProduto.id : null,
          idSubgrupoProduto: regraNatureza && regraNatureza.produto && regraNatureza.produto.subgrupoProduto ? regraNatureza.produto.subgrupoProduto.id : null,
          cores: {
            vidro: regraNatureza && regraNatureza.produto && regraNatureza.produto.cores && regraNatureza.produto.cores.vidro ? regraNatureza.produto.cores.vidro.id : null,
            ferragem: regraNatureza && regraNatureza.produto && regraNatureza.produto.cores && regraNatureza.produto.cores.ferragem ? regraNatureza.produto.cores.ferragem.id : null,
            aluminio: regraNatureza && regraNatureza.produto && regraNatureza.produto.cores && regraNatureza.produto.cores.aluminio ? regraNatureza.produto.cores.aluminio.id : null,
          },
          espessura: regraNatureza && regraNatureza.produto && regraNatureza.produto ? regraNatureza.produto.espessura : null
        },
        ufsDestino: regraNatureza ? this.clonar(regraNatureza.ufsDestino) : null,
        naturezaOperacaoProducao: {
          intraestadual: regraNatureza && regraNatureza.naturezaOperacaoProducao && regraNatureza.naturezaOperacaoProducao.intraestadual ? regraNatureza.naturezaOperacaoProducao.intraestadual.id : null,
          interestadual: regraNatureza && regraNatureza.naturezaOperacaoProducao && regraNatureza.naturezaOperacaoProducao.interestadual ? regraNatureza.naturezaOperacaoProducao.interestadual.id : null,
          intraestadualComSt: regraNatureza && regraNatureza.naturezaOperacaoProducao && regraNatureza.naturezaOperacaoProducao.intraestadualComSt ? regraNatureza.naturezaOperacaoProducao.intraestadualComSt.id : null,
          interestadualComSt: regraNatureza && regraNatureza.naturezaOperacaoProducao && regraNatureza.naturezaOperacaoProducao.interestadualComSt ? regraNatureza.naturezaOperacaoProducao.interestadualComSt.id : null,
        },
        naturezaOperacaoRevenda: {
          intraestadual: regraNatureza && regraNatureza.naturezaOperacaoRevenda && regraNatureza.naturezaOperacaoRevenda.intraestadual ? regraNatureza.naturezaOperacaoRevenda.intraestadual.id : null,
          interestadual: regraNatureza && regraNatureza.naturezaOperacaoRevenda && regraNatureza.naturezaOperacaoRevenda.interestadual ? regraNatureza.naturezaOperacaoRevenda.interestadual.id : null,
          intraestadualComSt: regraNatureza && regraNatureza.naturezaOperacaoRevenda && regraNatureza.naturezaOperacaoRevenda.intraestadualComSt ? regraNatureza.naturezaOperacaoRevenda.intraestadualComSt.id : null,
          interestadualComSt: regraNatureza && regraNatureza.naturezaOperacaoRevenda && regraNatureza.naturezaOperacaoRevenda.interestadualComSt ? regraNatureza.naturezaOperacaoRevenda.interestadualComSt.id : null,
        }
      };

      this.regraNaturezaOriginal = this.clonar(this.regraNatureza);
    },

    /**
     * Função que indica se o formulário de regras de natureza de operação possui valores válidos de acordo com os controles.
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
     * Força a atualização da lista de regras de natureza de operação, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Cfops.NaturezasOperacao.Regras.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },

  computed: {
    /**
     * Propriedade computada que retorna o filtro de subgrupos de produto.
     * @type {filtroSubgrupos}
     *
     * @typedef filtroSubgrupos
     * @property {?number} idGrupoProduto O ID do grupo de produto.
     */
    filtroSubgrupos: function () {
      return {
        idGrupoProduto: (this.regraNatureza.produto || {}).idGrupoProduto || 0
      };
    }
  },

  watch: {
    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function (atual) {
        if (this.regraNatureza) {
          this.regraNatureza.idLoja = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoClienteAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoClienteAtual: {
      handler: function (atual) {
        if (this.regraNatureza) {
          this.regraNatureza.idTipoCliente = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'grupoProdutoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoProdutoAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.produto) {
          this.regraNatureza.produto.idGrupoProduto = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'subgrupoProdutoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    subgrupoProdutoAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.produto) {
          this.regraNatureza.produto.idSubgrupoProduto = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'corVidroAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corVidroAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.produto && this.regraNatureza.produto.cores) {
          this.regraNatureza.produto.cores.vidro = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'corFerragemAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corFerragemAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.produto && this.regraNatureza.produto.cores) {
          this.regraNatureza.produto.cores.ferragem = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'corAluminioAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corAluminioAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.produto && this.regraNatureza.produto.cores) {
          this.regraNatureza.produto.cores.aluminio = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'naturezaOperacaoProducaoIntraestadualAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoProducaoIntraestadualAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.naturezaOperacaoProducao) {
          this.regraNatureza.naturezaOperacaoProducao.intraestadual = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'naturezaOperacaoProducaoInterestadualAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoProducaoInterestadualAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.naturezaOperacaoProducao) {
          this.regraNatureza.naturezaOperacaoProducao.interestadual = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'naturezaOperacaoProducaoIntraestadualComStAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoProducaoIntraestadualComStAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.naturezaOperacaoProducao) {
          this.regraNatureza.naturezaOperacaoProducao.intraestadualComSt = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'naturezaOperacaoProducaoInterestadualComStAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoProducaoInterestadualComStAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.naturezaOperacaoProducao) {
          this.regraNatureza.naturezaOperacaoProducao.interestadualComSt = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'naturezaOperacaoRevendaIntraestadualAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoRevendaIntraestadualAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.naturezaOperacaoRevenda) {
          this.regraNatureza.naturezaOperacaoRevenda.intraestadual = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'naturezaOperacaoRevendaInterestadualAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoRevendaInterestadualAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.naturezaOperacaoRevenda) {
          this.regraNatureza.naturezaOperacaoRevenda.interestadual = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'naturezaOperacaoRevendaIntraestadualComStAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoRevendaIntraestadualComStAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.naturezaOperacaoRevenda) {
          this.regraNatureza.naturezaOperacaoRevenda.intraestadualComSt = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'naturezaOperacaoRevendaInterestadualComStAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoRevendaInterestadualComStAtual: {
      handler: function (atual) {
        if (this.regraNatureza && this.regraNatureza.naturezaOperacaoRevenda) {
          this.regraNatureza.naturezaOperacaoRevenda.interestadualComSt = atual ? atual.id : null;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'ufsDestinoAtuais'.
     * Atualiza a variável de natureza de operação atual com as UFs selecionadas no controle.
     */
    ufsDestinoAtuais: {
      handler: function (atual) {
        if (this.regraNatureza) {
          this.regraNatureza.ufsDestino = atual;
        }
      },
      deep: true
    }
  }
});
