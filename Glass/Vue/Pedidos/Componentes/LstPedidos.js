const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    controleTooltipAtual: null
  },

  filters: {
    /**
     * Filtro para formatação do texto de sinal.
     * @param {string} valor O texto que será formatado.
     */
    textoSinal: function(valor) {
      return valor ? 'Sinal (' + valor + ')' : '';
    }
  },

  methods: {
    /**
     * Busca os pedidos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca de pedidos.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     */
    atualizarPedidos: function(filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Pedidos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna o link para a tela de edição de pedidos.
     * @param {Object} item O pedido que será usado para construção do link.
     * @returns {string} O link que redireciona para a edição de pedidos.
     */
    obterLinkEditarPedido: function(item) {
      var byVend = GetQueryString('byVend');
      var link = !item.usarControleReposicao
        ? '../Cadastros/CadPedido.aspx?idPedido=' + item.id + '&ByVend=' + byVend
        : '../Cadastros/CadPedidoReposicao.aspx?idPedido=' + item.id + '&ByVend=' + byVend;

      return link;
    },

    /**
     * Exibe um relatório de pedido, de acordo com o tipo desejado.
     * @param {Object} item O pedido que será exibido.
     * @param {number} tipo O tipo de relatório que será exibido.
     * @param {?boolean} [reposicao=true] Indica se o relatório de reposição será usado, se possível.
     */
    abrirRelatorio: function(item, tipo, reposicao) {
      reposicao = reposicao !== null && reposicao !== undefined
        ? reposicao
        : true;

      this.abrirJanela(
        600,
        800,
        '../Relatorios/RelPedido'
          + (item.usarControleReposicao && reposicao ? 'Repos' : '') + '.aspx'
          + '?idPedido=' + item.id
          + '&tipo=' + tipo
      );
    },

    /**
     * Exibe o relatório de notas promissórias de um pedido.
     * @param {Object} item O pedido que será exibido.
     */
    abrirRelatorioNotasPromissorias: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=NotaPromissoria&idPedido=' + item.id);
    },

    /**
     * Exibe o relatório de projetos de um pedido.
     * @param {Object} item O pedido que será exibido.
     */
    abrirRelatorioProjeto: function(item) {
      const pcp = this.configuracoes.usarImpressaoProjetoPcp && item.permissoes.temAlteracaoPcp
        ? '&pcp=1'
        : '';

      this.abrirJanela(
        600,
        800,
        '../Cadastros/Projeto/ImprimirProjeto.aspx'
        + '?idPedido=' + item.id
        + pcp
      );
    },

    /**
     * Exibe a tela para anexos no pedido.
     * @param {Object} item O pedido que terá itens anexados.
     */
    abrirAnexos: function(item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=pedido');
    },

    /**
     * Retorna o link para a tela de sugestões de clientes.
     * @param {Object} item O pedido que terá sugestões feitas.
     */
    obterLinkSugestoes: function(item) {
      return 'LstSugestaoCliente.aspx?idPedido=' + item.id;
    },

    /**
     * Exibe a tela para realizar o cancelamento de um pedido.
     * @param {Object} item O pedido que será cancelado.
     */
    abrirCancelarPedido: function (item) {
      this.abrirJanela(350, 600, '../Utils/SetMotivoCanc.aspx?idPedido=' + item.id);
    },

    /**
     * Exibe a tela de alteração rápida de dados do pedido.
     * @param {Object} item O pedido que será alterado.
     */
    abrirDesconto: function(item) {
      this.abrirJanela(600, 800, '../Utils/DescontoPedido.aspx?idPedido=' + item.id);
    },

    /**
     * Exibe a tela com as imagens das peças de um pedido.
     * @param {Object} item O pedido que será exibido.
     */
    abrirImagemPeca: function(item) {
      this.abrirJanela(600, 800, '../Utils/SelImagemPeca.aspx?tipo=pedido&idPedido=' + item.id);
    },

    /**
     * Exibe o relatório de itens que ainda não foram liberados de um pedido.
     * @param {Object} item O pedido que será exibido no relatório.
     */
    abrirItensFaltamLiberar: function(item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ProdutosLiberar&idPedido=' + item.id);
    },

    /**
     * Retorna o link para a tela de alteração de processo/aplicação de um pedido.
     * @param {Object} item O pedido que terá os dados alterados.
     */
    obterLinkAlterarProcessoEAplicacao: function(item) {
      return '../Utils/AlterarProcessoAplicacao.aspx?idPedido=' + item.id;
    },

    /**
     * Exibe a tela com os dados sobre a rentabilidade do pedido.
     * @param {Object} item O pedido que terá a rentabilidade exibida.
     */
    abrirRentabilidade: function(item) {
      this.abrirJanela(500, 700, '../Relatorios/Rentabilidade/VisualizacaoItemRentabilidade.aspx?tipo=pedido&id=' + item.id);
    },

    /**
     * Exibe a tela de anexos da liberação de um pedido.
     * @param {Object} item O pedido que será usado para abertura da tela.
     */
    abrirAnexosLiberacao: function(item) {
      if (item.liberacao && item.liberacao.id) {
        this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.liberacao.id + '&tipo=liberacao');
      }
    },

    /**
     * Salva a observação de um produto de pedido.
     * @param {Object} item O produto que será atualizado.
     */
    alterarObservacaoEObservacaoLiberacao: function (item) {
      var vm = this;

      Servicos.Pedidos.salvarObservacao(item.id, item.observacao, item.liberacao.observacao)
        .then(function (resposta) {
          vm.exibirMensagem('Sucesso', resposta.data.mensagem);
          vm.controleTooltipAtual.fechar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        })
    },

    /**
     * Reabre um pedido, se possível.
     * @param {Object} item O pedido que será reaberto.
     */
    reabrirPedido: function(item) {
      var vm = this;

      Servicos.Pedidos.reabrir(item.id)
        .then(function(resposta) {
          vm.atualizarLista();
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Retorna o link para consultar a produção de um pedido.
     * @param {Object} item O pedido que será usado para construção do link.
     */
    obterLinkConsultaProducao: function(item) {
      return '../Cadastros/Producao/LstProducao.aspx?idPedido=' + item.id;
    },

    /**
     * Altera a liberação financeira de um pedido.
     * @param {Object} item O pedido que será alterado.
     * @param {boolean} liberar Indica se o pedido será liberado.
     */
    alterarLiberacaoFinanceira: function(item, liberar) {
      var vm = this;

      Servicos.Pedidos.alterarLiberacaoFinanceira(item.id, liberar)
        .then(function(resposta) {
          vm.atualizarLista();
        })
        .catch(function(erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Exibe a tela de anexar arquivos a vários pedidos.
     */
    anexarArquivosAVariosPedidos: function() {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=0&tipo=pedido');
    },

    /**
     * Abre os relatórios de totais dos pedidos filtrados.
     * @param {boolean} usarValoresQueryString Considerar os valores informados na querystring para o método?
     * @param {number} alturaJanela A altura da janela de popup.
     * @param {number} larguraJanela A largura da janela de popup.
     * @param {string} url A URL do popup que contém o relatório de totais.
     */
    abrirRelatoriosTotais: function (usarValoresQueryString, alturaJanela, larguraJanela, url) {
      var byVend = usarValoresQueryString ? GetQueryString('byVend') : '';
      var maoObra = usarValoresQueryString ? GetQueryString('maoObra') : '';
      var maoObraEspecial = usarValoresQueryString ? GetQueryString('maoObraEspecial') : '';
      var producao = usarValoresQueryString ? GetQueryString('producao') : '';

      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idCli', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCli', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'codCliente', this.filtro.codigoPedidoCliente);
      this.incluirFiltroComLista(filtros, 'idCidade', this.filtro.idCidade);
      this.incluirFiltroComLista(filtros, 'endereco', this.filtro.endereco);
      this.incluirFiltroComLista(filtros, 'bairro', this.filtro.bairro);
      this.incluirFiltroComLista(filtros, 'complemento', this.filtro.complemento);
      this.incluirFiltroComLista(filtros, 'byVend', byVend);
      this.incluirFiltroComLista(filtros, 'altura', this.filtro.altura);
      this.incluirFiltroComLista(filtros, 'largura', this.filtro.largura);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'situacaoProd', this.filtro.situacaoProducao);
      this.incluirFiltroComLista(filtros, 'idOrcamento', this.filtro.idOrcamento);
      this.incluirFiltroComLista(filtros, 'maoObra', maoObra);
      this.incluirFiltroComLista(filtros, 'maoObraEspecial', maoObraEspecial);
      this.incluirFiltroComLista(filtros, 'producao', producao);
      this.incluirFiltroComLista(filtros, 'diasProntoLib', this.filtro.diferencaDiasEntreProntoELiberado);
      this.incluirFiltroComLista(filtros, 'valorDe', this.filtro.valorPedidoMinimo);
      this.incluirFiltroComLista(filtros, 'valorAte', this.filtro.valorPedidoMaximo);
      this.incluirFiltroComLista(filtros, 'dataCadIni', this.filtro.periodoCadastroInicio);
      this.incluirFiltroComLista(filtros, 'dataCadFim', this.filtro.periodoCadastroFim);
      this.incluirFiltroComLista(filtros, 'dataFinIni', this.filtro.periodoFinalizacaoInicio);
      this.incluirFiltroComLista(filtros, 'dataFinFim', this.filtro.periodoFinalizacaoFim);
      this.incluirFiltroComLista(filtros, 'funcFinalizacao', this.filtro.codigoUsuarioFinalizacao);
      this.incluirFiltroComLista(filtros, 'tipo', this.filtro.tipo);
      this.incluirFiltroComLista(filtros, 'fastDelivery', this.filtro.fastDelivery);
      this.incluirFiltroComLista(filtros, 'tipoVenda', this.filtro.tipoVenda);
      this.incluirFiltroComLista(filtros, 'origemPedido', this.filtro.origem);
      this.incluirFiltroComLista(filtros, 'obs', this.filtro.observacao);

      var filtroReal = filtros.length
        ? '?' + filtros.join('&')
        : '';

      this.abrirJanela(alturaJanela, larguraJanela, url + filtroReal);
    },

    /**
     * Exibe a lista de totais dos pedidos na tela.
     */
    abrirListaTotais: function () {
      this.abrirRelatoriosTotais(true, 140, 320, '../Utils/ListaTotalPedido.aspx');
    },

    /**
     * Exibe o gráfico de totais diários na tela.
     */
    abrirGraficoTotaisDiarios: function () {
      this.abrirRelatoriosTotais(false, 600, 800, '../Relatorios/GraficoTotaisDiariosPedido.aspx')
    },

    /**
     * Retorna o link para a tela de cadastro de pedido, de acordo com a listagem sendo exibida.
     * @returns {string} O link para a tela de cadastro de pedido.
     */
    obterLinkInserirPedido: function() {
      var byVend = GetQueryString('byVend');
      var maoObra = GetQueryString('maoObra');
      var maoObraEspecial = GetQueryString('maoObraEspecial');
      var producao = GetQueryString('producao');
      var tipoPedido = '';

      if (maoObra) {
        tipoPedido = '&maoObra=' + maoObra;
      } else if (maoObraEspecial) {
        tipoPedido = '&maoObraEspecial=' + maoObraEspecial;
      } else if (producao) {
        tipoPedido = '&producao=' + producao;
      }

      return '../Cadastros/CadPedido.aspx?byVend=' + byVend + tipoPedido;
    },

    /**
     * Retorna as ordens de carga de um pedido, se existirem.
     * @param {Object} item O pedido que será examinado.
     * @returns {string} Uma string com as ordens de carga do pedido, se existirem.
     */
    exibirOrdensDeCarga: function(item) {
      return item && item.idsOrdensDeCarga
        ? item.idsOrdensDeCarga
            .slice()
            .sort()
            .join(', ')
        : '';
    },

    /**
     * Força a atualização da lista de pedidos, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },

    /**
     * Indica que um controle de tooltip está sendo exibido.
     * @param {!Object} tooltip O controle de tooltip que está sendo exibido.
     */
    mostrarTooltip: function (tooltip) {
      if (this.controleTooltipAtual) {
        this.controleTooltipAtual.fechar();
      }

      this.controleTooltipAtual = tooltip;
    },

    /**
     * Indica que o controle de tooltip atual está fechado.
     */
    esconderTooltip: function () {
      this.controleTooltipAtual = null;
    }
  },

  computed: {
    /**
     * Propriedade computada que indica se o tipo de pedido é fixo ou não.
     * @type {boolean}
     */
    tipoPedidoFixo: function() {
      return !!this.filtro.tipoPedidoFixo;
    },

    /**
     * Propriedade computada que indica se o vendedor é fixo ou não.
     * @type {boolean}
     */
    vendedorFixo: function() {
      return !!this.filtro.vendedorFixo;
    }
  },

  created: function() {
    var byVend = GetQueryString('byVend');
    var maoObra = GetQueryString('maoObra');
    var maoObraEspecial = GetQueryString('maoObraEspecial');
    var producao = GetQueryString('producao');

    if (byVend === '1') {
      this.filtro.vendedorFixo = true;
    }

    if (maoObra) {
      this.filtro.tipoPedidoFixo = 3;
    } else if (maoObraEspecial) {
      this.filtro.tipoPedidoFixo = 5;
    } else if (producao) {
      this.filtro.tipoPedidoFixo = 4;
    }
  },

  mounted: function() {
    var exibirFinanceiro = GetQueryString('financ') === '1';
    var vm = this;

    Servicos.Pedidos.obterConfiguracoesLista(exibirFinanceiro).then(function(resposta) {
      vm.configuracoes = resposta.data;
    });
  }
});
