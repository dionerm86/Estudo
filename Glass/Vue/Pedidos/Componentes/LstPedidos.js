const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'desc'
    },
    dadosOrdenacaoObservacoesFinanceiro_: {
      campo: '',
      direcao: ''
    },
    configuracoes: {},
    filtro: {}
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
     * Realiza a ordenação da lista de pedidos.
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
     * Exibe uma div como 'popup'.
     * @param {string} nome O nome da DIV que será exibida.
     * @param {Object} event O objeto com o evento JavaScript.
     */
    exibirObsObsLib: function(nome, event, item) {
        var botao = event.target;

        TagToTip(nome, FADEIN, 300, COPYCONTENT, false, TITLE, 'Observação do pedido: ' + item.id, CLOSEBTN, true,
            CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
            FIX, [botao, 320 - getTableWidth(nome), -41 - getTableHeight(nome)]);
    },

    /**
     * Salva a observação de um produto de pedido.
     * @param {Object} item O produto que será atualizado.
     */
    alterarObsObsLib: function (item) {
        var vm = this;

        Servicos.Pedidos.salvarObservacao(item.id, item.obs, item.obsLiberacao)
          .then(function (resposta) {
              vm.exibirMensagem('Sucesso', resposta.data.mensagem);
              UnTip();
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
     * Exibe uma div como 'popup'.
     * @param {string} nome O nome da DIV que será exibida.
     * @param {Object} event O objeto com o evento JavaScript.
     */
    exibirDivComoPopup: function(nome, event) {
      var botao = event.target;

      for (var iTip = 0; iTip < 2; iTip++) {
        TagToTip(
          nome,
          FADEIN,
          300,
          COPYCONTENT,
          false,
          FIX,
          [
            botao,
            9 - getTableWidth(nome),
            -25 - getTableHeight(nome)
          ]);
      }
    },

    /**
     * Realiza a busca de observações do financeiro de um pedido.
     * @param {!Object} filtroOriginal O filtro usado para a busca.
     * @param {!number} pagina O número da página atual na lista.
     * @param {!number} numeroRegistros O número de registros a serem exibidos na lista.
     * @param {string} ordenacao A ordenação a ser usada na busca.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    buscarObservacoesFinanceiro: function(filtroOriginal, pagina, numeroRegistros, ordenacao) {
      return Servicos.Pedidos.obterListaObservacoesFinanceiro(filtroOriginal.id, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de observações do financeiro.
     * @param {!string} campo O campo que será usado na ordenação.
     */
    ordenarObservacoesFinanceiro: function(campo) {
      if (campo !== this.dadosOrdenacaoObservacoesFinanceiro_.campo) {
        this.dadosOrdenacaoObservacoesFinanceiro_.campo = campo;
        this.dadosOrdenacaoObservacoesFinanceiro_.direcao = '';
      } else {
        this.dadosOrdenacaoObservacoesFinanceiro_.direcao =
          this.dadosOrdenacaoObservacoesFinanceiro_.direcao === '' ? 'desc' : '';
      }
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

      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('idPedido', this.filtro.id);
      incluirFiltro('idLoja', this.filtro.idLoja);
      incluirFiltro('idCli', this.filtro.idCliente);
      incluirFiltro('nomeCli', this.filtro.nomeCliente);
      incluirFiltro('codCliente', this.filtro.codigoPedidoCliente);
      incluirFiltro('idCidade', this.filtro.idCidade);
      incluirFiltro('endereco', this.filtro.endereco);
      incluirFiltro('bairro', this.filtro.bairro);
      incluirFiltro('complemento', this.filtro.complemento);
      incluirFiltro('byVend', byVend);
      incluirFiltro('altura', this.filtro.altura);
      incluirFiltro('largura', this.filtro.largura);
      incluirFiltro('situacao', this.filtro.situacao);
      incluirFiltro('situacaoProd', this.filtro.situacaoProducao);
      incluirFiltro('idOrcamento', this.filtro.idOrcamento);
      incluirFiltro('maoObra', maoObra);
      incluirFiltro('maoObraEspecial', maoObraEspecial);
      incluirFiltro('producao', producao);
      incluirFiltro('diasProntoLib', this.filtro.diferencaDiasEntreProntoELiberado);
      incluirFiltro('valorDe', this.filtro.valorPedidoMinimo);
      incluirFiltro('valorAte', this.filtro.valorPedidoMaximo);
      incluirFiltro('dataCadIni', this.filtro.periodoCadastroInicio.toLocaleDateString('pt-BR'));
      incluirFiltro('dataCadFim', this.filtro.periodoCadastroFim.toLocaleDateString('pt-BR'));
      incluirFiltro('dataFinIni', this.filtro.periodoFinalizacaoInicio.toLocaleDateString('pt-BR'));
      incluirFiltro('dataFinFim', this.filtro.periodoFinalizacaoFim.toLocaleDateString('pt-BR'));
      incluirFiltro('funcFinalizacao', this.filtro.codigoUsuarioFinalizacao);
      incluirFiltro('tipo', this.filtro.tipo);
      incluirFiltro('fastDelivery', this.filtro.fastDelivery);
      incluirFiltro('tipoVenda', this.filtro.tipoVenda);
      incluirFiltro('origemPedido', this.filtro.origem);
      incluirFiltro('obs', this.filtro.observacao);

      var filtroReal = filtros.length > 0
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
    },

    /**
     * Propriedade computada que indica a ordenação para as observações do financeiro.
     * @type {string}
     */
    ordenacaoObservacoesFinanceiro: function() {
      var direcao = this.dadosOrdenacaoObservacoesFinanceiro_.direcao
        ? ' ' + this.dadosOrdenacaoObservacoesFinanceiro_.direcao
        : '';
      return this.dadosOrdenacaoObservacoesFinanceiro_.campo + direcao;
    },

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
