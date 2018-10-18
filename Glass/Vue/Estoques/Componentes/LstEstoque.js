const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    estoqueProduto: {},
    estoqueProdutoAtual: {},
    estoqueProdutoOriginal: {},
    insercaoRapidaEstoque: false,
    idProdutoEmAtualizacao: null,
    participanteAtual: null
  },

  methods: {
    /**
     * Busca os estoques de produto para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de estoques de produto.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      if (!this.configuracoes || !Object.keys(this.configuracoes).length) {
        return Promise.reject();
      }

      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Estoques.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Indica que um estoque de produto será editado.
     * @param {Object} estoqueProduto O item que será editado.
     * @param {number} numeroLinha O número da linha que contém o iten que será editado.
     */
    editar: function (estoqueProduto, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(estoqueProduto);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Atualiza os dados do estoque do produto.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var estoqueProdutoAtualizar = this.patch(this.estoqueProduto, this.estoqueProdutoOriginal);
      var vm = this;

      if (this.exibirEstoqueFiscal) {
        Servicos.Estoques.atualizarEstoqueFiscal(this.estoqueProdutoAtual.idProduto, this.estoqueProdutoAtual.idLoja, estoqueProdutoAtualizar)
          .then(function (resposta) {
            vm.atualizarLista();
            vm.cancelar();
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      } else {
        Servicos.Estoques.atualizarEstoqueReal(this.estoqueProdutoAtual.idProduto, this.estoqueProdutoAtual.idLoja, estoqueProdutoAtualizar)
          .then(function (resposta) {
            vm.atualizarLista();
            vm.cancelar();
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      }
    },

    /**
     * Cancela a edição do estoque de produto.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
    },

    /**
     * Exibe o relatório com detalhamento da quantidade em reserva do produto informado.
     * @param {Object} item O pedido que será exibido.
     */
    abrirRelatorioReserva: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=EstoqueProdutos&TipoColunas=1&idProd=' + item.idProduto
        + "&idLoja=" + item.idLoja + "&agrupar=false");
    },

    /**
     * Exibe o relatório com detalhamento da quantidade em liberação do produto informado.
     * @param {Object} item O pedido que será exibido.
     */
    abrirRelatorioLiberacao: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=EstoqueProdutos&TipoColunas=2&idProd=' + item.idProduto
        + "&idLoja=" + item.idLoja + "&agrupar=false");
    },

    /**
     * Exibe um relatório com a listagem de estoques de produto aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaEstoquesProduto: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=Estoque' + (this.exibirEstoqueFiscal ? 'Real' : 'Fiscal') +
        this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Ativa/Desativa a inserção rápida de estoque.
     */
    ativarDesativarInsercaoRapidaEstoque: function () {
      this.cancelar();
      this.insercaoRapidaEstoque = !this.insercaoRapidaEstoque;
    },

    /**
     * Atualiza o estoque do produto ("Inserção rápida de estoque").
     * @param {Object} estoqueProduto O item que será editado.
     */
    atualizarCampoUnico: function (item) {
      var vm = this;

      this.idProdutoEmAtualizacao = item.idProduto;

      if (this.exibirEstoqueFiscal) {
        Servicos.Estoques.atualizarEstoqueFiscalCampoUnico(item.idProduto, item.idLoja, item.quantidadeEstoqueFiscal)
          .then(function (resposta) {
            vm.idProdutoEmAtualizacao = null;
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      } else {
        Servicos.Estoques.atualizarEstoqueRealCampoUnico(item.idProduto, item.idLoja, item.quantidadeEstoque)
          .then(function (resposta) {
            vm.idProdutoEmAtualizacao = null;
          })
          .catch(function (erro) {
            if (erro && erro.mensagem) {
              vm.exibirMensagem('Erro', erro.mensagem);
            }
          });
      }
    },

    /**
     * Função executada para criação dos objetos necessários para edição do estoque do produto.
     * @param {?Object} [estoqueProduto=null] O estoque de produto que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (estoqueProduto) {
      this.estoqueProdutoAtual = estoqueProduto;
      this.participanteAtual = estoqueProduto ? this.clonar(estoqueProduto.participante) : null;

      if (this.exibirEstoqueFiscal) {
        this.estoqueProduto = {
          quantidadeEstoqueFiscal: estoqueProduto ? estoqueProduto.quantidadeEstoqueFiscal : null,
          quantidadePosseTerceiros: estoqueProduto ? estoqueProduto.quantidadePosseTerceiros : null,
          tipoParticipante: estoqueProduto && estoqueProduto.tipoParticipante ? estoqueProduto.tipoParticipante.id : null,
          idParticipante: (this.participanteAtual || {}).id
        };
      } else {
        this.estoqueProduto = {
          estoqueMinimo: estoqueProduto ? estoqueProduto.estoqueMinimo : null,
          estoqueM2: estoqueProduto ? estoqueProduto.estoqueM2 : null,
          quantidadeEstoque: estoqueProduto ? estoqueProduto.quantidadeEstoque : null,
          quantidadeDefeito: estoqueProduto ? estoqueProduto.quantidadeDefeito : null,
        };
      }

      this.estoqueProdutoOriginal = this.clonar(this.estoqueProduto);
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
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'codProd', this.filtro.codigoInternoProduto);
      this.incluirFiltroComLista(filtros, 'descr', this.filtro.descricaoProduto);
      this.incluirFiltroComLista(filtros, 'idGrupo', this.filtro.idGrupoProduto);
      this.incluirFiltroComLista(filtros, 'idsSubgrupoProduto', this.filtro.idsSubgrupoProduto);
      this.incluirFiltroComLista(filtros, 'apenasEstoqueFiscal', this.filtro.apenasComEstoque);
      this.incluirFiltroComLista(filtros, 'apenasPosseTerceiros', this.filtro.apenasPosseTerceiros);
      this.incluirFiltroComLista(filtros, 'apenasProdutosProjeto', this.filtro.apenasProdutosProjeto);
      this.incluirFiltroComLista(filtros, 'idCorVidro', this.filtro.idCorVidro);
      this.incluirFiltroComLista(filtros, 'idCorFerragem', this.filtro.idCorFerragem);
      this.incluirFiltroComLista(filtros, 'idCorAluminio', this.filtro.idCorAluminio);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'aguardSaidaEstoque', this.filtro.aguardandoSaidaEstoque);
      this.incluirFiltroComLista(filtros, 'ordenacao', this.filtro.ordenacaoFiltro);

      return filtros.length
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
     * Propriedade computada que indica se deverão ser exibidos dados fiscais
     */
    exibirEstoqueFiscal: function () {
      return GetQueryString('fiscal') == '1';
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Estoques.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },

  watch: {
    participanteAtual: {
      handler: function (atual) {
        this.estoqueProduto.idParticipante = atual ? atual.id : null;
      },
      deep: true
    },

    tipoParticipanteAtual: {
      handler: function (atual) {
        this.estoqueProduto.tipoParticipante = typeof atual === 'number' ? atual : null;
      },
      deep: true
    }
  }
});
