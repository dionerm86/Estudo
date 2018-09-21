const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar, Mixins.FiltroQueryString],

  data: {
    dadosOrdenacao_: {
      campo: '',
      direcao: ''
    },
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os pedidos em conferência para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    atualizarPedidosConferencia: function(filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.PedidosConferencia.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista.
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
     * Obtém link para a tela de edição de pedido em conferência.
     * @param {Object} item O pedido em conferência que será editado.
     */
    obterLinkEditarPedido: function (item) {
      return '../Cadastros/CadPedidoEspelho.aspx?idPedido=' + item.id;
    },

    /**
     * Exclui o pedido em conferência.
     * @param {Object} item O pedido em conferência que será excluída.
     */
    excluir: function (item) {
      if (!this.perguntar("Tem certeza que deseja excluir esta conferência?")) {
        return;
      }

      var vm = this;

      Servicos.PedidosConferencia.excluir(item.id)
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
     * Exibe a impressão do pedido em conferência.
     * @param {Object} item O pedido em conferência que será impresso.
     * @param {boolean} exibirReposicao Define se deverá ser impresso o modelo de reposição.
     * @param {number} tipo O tipo de impressão que será feita (0-Pedido comercial, 1-Memória de cálculo, 2-Pedido em conferência, 3-Memória de cálculo de pedido em conferência).
     */
    abrirImpressaoPedidoConferencia: function (item, exibirReposicao, tipo) {
      if (!exibirReposicao) {
        this.abrirJanela(600, 800, '../Relatorios/RelPedido.aspx?idPedido=' + item.id + "&tipo=" + tipo);
      }
      else {
        this.abrirJanela(600, 800, '../Relatorios/RelPedidoRepos.aspx?idPedido=' + item.id + "&tipo=" + tipo);
      }
    },

    /**
     * Abre uma tela para anexar arquivos ao pedido em conferência.
     * @param {Object} item O pedido em conferência a partir do qual os arquivos serão anexados.
     */
    abrirAnexoPedidoConferencia: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=pedido');
    },

    /**
     * Exibe a impressão dos projetos do pedido em conferência.
     * @param {Object} item O pedido em conferência que terá os projetos impressos.
     */
    abrirImpressaoProjetos: function (item) {
      this.abrirJanela(600, 800, '../Cadastros/Projeto/ImprimirProjeto.aspx?idPedido=' + item.id + "&pcp=1");
    },

    /**
     * Abre uma tela para associar imagens às peças de vidro do pedido em conferência.
     * @param {Object} item O pedido em conferência que a partir do qual serão exibidas as peças de vidro.
     */
    abrirAssociacaoImagemAPeca: function (item) {
      this.abrirJanela(600, 800, '../Utils/SelImagemPeca.aspx?tipo=pcp&idPedido=' + item.id);
    },

    /**
     * Exibe a impressão dos produtos que faltam comprar do pedido em conferência.
     * @param {Object} item O pedido em conferência que a partir do qual serão exibidos os produtos a comprar.
     */
    abrirProdutosAComprar: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ProdutosComprar&idPedido=' + item.id);
    },

    /**
     * Abre uma tela para visualizar dados de rentabilidade.
     * @param {Object} item A nota fiscal que será analisado os dados.
     */
    abrirRentabilidade: function (item) {
      this.abrirJanela(500, 700, '../Relatorios/Rentabilidade/VisualizacaoItemRentabilidade.aspx?tipo=pedidoespelho&id=' + item.id);
    },

    /**
     * Reabre o pedido em conferência para edição.
     * @param {Object} item O pedido em conferência que será reaberto.
     */
    reabrir: function (item) {
      var pergunta = item.pedidoImportado
        ? 'Este pedido foi importado, ao reabrí-lo as marcações e etiquetas importadas serão PERDIDAS. Deseja realmente reabrir este pedido?'
        : 'Deseja reabrir este pedido?';

      if (!this.perguntar('Reabrir pedido em conferência', pergunta)) {
        return;
      }

      var vm = this;

      Servicos.PedidosConferencia.reabrir(item.id)
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
     * Altera a situação CNC do pedido em conferencia.
     * @param {Object} item O pedido em conferência que terá a situação CNC alterada.
     */
    alterarSituacaoCnc: function (item) {
      var pergunta = item.permissoes.exibirSituacaoCnc
        ? 'Deseja marcar a situação proj. CNC deste pedido como ' + (item.situacaoCnc.id == this.configuracoes.situacaoCncProjetado ? 'não projetado?' : 'projetado?')
        : 'Deseja marcar a situação proj. CNC deste pedido como ' + (item.situacaoCnc.id == this.configuracoes.situacaoCncSemNecessidadeNaoConferido ? 'sem necessidade (conferido)?' : 'sem necessidade (não conferido)?');

      if (!this.perguntar('Alterar situação CNC', pergunta)) {
        return;
      }

      var vm = this;

      Servicos.PedidosConferencia.alterarSituacaoCnc(item.id)
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
     * Marca um pedido em conferência importado como conferido.
     * @param {Object} item O pedido em conferência importado que será marcado como conferido.
     */
    marcarPedidoImportadoConferido: function (item) {
      if (!this.perguntar('Alterar situação CNC', 'Deseja marcar o pedido importado como ' + (item.pedidoConferido ? 'não conferido?' : 'conferido?'))) {
        return;
      }

      var vm = this;

      Servicos.PedidosConferencia.marcarPedidoImportadoConferido(item.id)
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
     * Exibe o total de peças com e sem marcação, com base nos filtros utilizados.
     */
    exibirTotalPecasComSemMarcacao: function () {
      var filtros = this.formatarFiltros_();

      if (!this.validarFiltrosVazios_(filtros)) {
        return false;
      }

      this.abrirJanela(140, 320, '../Listas/ListaTotalMarcacao.aspx?a=1' + filtros);
    },

    /**
     * Abre uma tela para anexar arquivos à vários pedidos em conferência.
     */
    abrirAnexoVariosPedidosConferencia: function () {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=0&tipo=pedido');
    },

    /**
     * Exibe a impressão dos pedidos em conferência filtrados na tela.
     */
    abrirImpressaoListaPedidoConferencia: function () {
      var filtros = this.formatarFiltros_();

      if (!this.validarFiltrosVazios_(filtros)) {
        return false;
      }

      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=PedidoEspelho' + filtros);
    },

    /**
     * Exibe uma lista dos pedidos em conferência filtrados na tela para impressão seletiva.
     */
    abrirListaSelecaoPedidoConferencia: function () {
      var filtros = this.formatarFiltros_();

      if (!this.validarFiltrosVazios_(filtros)) {
        return false;
      }

      this.abrirJanela(600, 800, '../Utils/SelPedidoEspelhoImprimir.aspx?a=1' + filtros);
    },

    /**
     * Exibe uma lista com os produtos a serem comprados baseados nos pedidos em conferência filtrados na tela.
     */
    abrirImpressaoListaProdutosAComprar: function () {
      var filtros = this.formatarFiltros_();

      if (!this.validarFiltrosVazios_(filtros)) {
        return false;
      }

      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=ProdutosComprar&buscarPedidos=true' + filtros);
    },

    /**
     * Gera arquivos para máquinas de CNC, a partir dos filtros da tela.
     * @param {number} tipoArquivo O tipo do arquivo (1-CNC, 2-DXF, 3-FML, 4-SGlass, 5-Intermac).
     */
    gerarArquivosMaquina: function (tipoArquivo) {
      var filtros = this.formatarFiltros_();

      if (!this.validarFiltrosVazios_(filtros)) {
        return false;
      }

      if ((this.filtro.situacao == 0 || this.filtro.situacao == 1) && this.filtro.idPedido == "") {
        this.exibirMensagem('Validação', 'Estes arquivos só podem ser gerados para pedidos finalizados ou impressos, filtre por alguma destas situações e tente novamente.');
        return false;
      }

      /*
      var idPedido = FindControl("txtNumPedido", "input").value;

      if (LstPedidosEspelho.PodeImprimirPedidoImportado(idPedido).value.toLowerCase() == "false") {
          alert("O pedido importado ainda não foi conferido, confira o mesmo antes de gerar arquivo");
          return false;
      }
      */

      var nomeArquivo = tipoArquivo == 1 ? 'Cnc'
        : tipoArquivo == 2 ? 'Dxf'
        : tipoArquivo == 3 ? 'Fml'
        : tipoArquivo == 3 ? 'SGlass'
        : tipoArquivo == 3 ? 'Intermac'
        : '';

      this.abrirJanela(200, 200, '../Handlers/Arquivo' + nomeArquivo + '.ashx?a=1' + filtros);
    },

    /**
     * Valida se os filtros foram informados.
     * @param {string} filtros Os filtros da tela concatenados.
     */
    validarFiltrosVazios_: function (filtros) {
      if (filtros == '' && !this.perguntar('É recomendável aplicar um filtro. Deseja realmente prosseguir?')) {
        return false;
      }

      return true;
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idFunc', this.filtro.idVendedor);
      this.incluirFiltroComLista(filtros, 'idFuncionarioConferente', this.filtro.idConferente);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'situacaoPedOri', this.filtro.situacaoPedidoComercial);
      this.incluirFiltroComLista(filtros, 'idsProcesso', this.filtro.idsProcesso);
      this.incluirFiltroComLista(filtros, 'dataIniEnt', this.filtro.periodoEntregaInicio);
      this.incluirFiltroComLista(filtros, 'dataFimEnt', this.filtro.periodoEntregaFim);
      this.incluirFiltroComLista(filtros, 'dataIniFab', this.filtro.periodoFabricaInicio);
      this.incluirFiltroComLista(filtros, 'dataFimFab', this.filtro.periodoFabricaFim);
      this.incluirFiltroComLista(filtros, 'dataIniFin', this.filtro.periodoFinalizacaoConferenciaInicio);
      this.incluirFiltroComLista(filtros, 'dataFimFin', this.filtro.periodoFinalizacaoConferenciaFim);
      this.incluirFiltroComLista(filtros, 'dataIniConf', this.filtro.periodoCadastroConferenciaInicio);
      this.incluirFiltroComLista(filtros, 'dataFimConf', this.filtro.periodoCadastroConferenciaFim);
      this.incluirFiltroComLista(filtros, 'dataIniEmis', this.filtro.periodoCadastroPedidoInicio);
      this.incluirFiltroComLista(filtros, 'dataFimEmis', this.filtro.periodoCadastroPedidoFim);
      this.incluirFiltroComLista(filtros, 'pedidosSemAnexos', this.filtro.pedidosSemAnexo);
      this.incluirFiltroComLista(filtros, 'pedidosAComprar', this.filtro.pedidosAComprar);
      this.incluirFiltroComLista(filtros, 'situacaoCnc', this.filtro.situacaoCnc);
      this.incluirFiltroComLista(filtros, 'dataIniSituacaoCnc', this.filtro.periodoProjetoCncInicio);
      this.incluirFiltroComLista(filtros, 'dataFimSituacaoCnc', this.filtro.periodoProjetoCncFim);
      this.incluirFiltroComLista(filtros, 'tipoPedido', this.filtro.tiposPedido);
      this.incluirFiltroComLista(filtros, 'idsRotas', this.filtro.idsRota);
      this.incluirFiltroComLista(filtros, 'origemPedido', this.filtro.origemPedido);
      this.incluirFiltroComLista(filtros, 'pedidosConferidos', this.filtro.pedidoImportacaoConferido);
      this.incluirFiltroComLista(filtros, 'tipoVenda', this.filtro.tipoVenda);

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

    Servicos.PedidosConferencia.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
