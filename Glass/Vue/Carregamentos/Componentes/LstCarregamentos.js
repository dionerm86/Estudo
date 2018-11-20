const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    configuracoes: {},
    filtro: {},
    numeroLinhaEdicao: -1,
    inserindo: false,
    ordensCargaEmExibicao: [],
    carregamento: {},
    carregamentoOriginal: {},
    motoristaAtual: null,
    veiculoAtual: null
  },

  methods: {
    /**
     * Busca os carregamentos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Carregamentos.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Retorna os itens para o controle de motoristas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroMotoristas: function () {
      return Servicos.Funcionarios.obterMotoristas();
    },

    /**
     * Retorna os itens para o controle de veículos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroVeiculos: function () {
      return Servicos.Veiculos.obterFiltro();
    },

    /**
     * Retorna as rotas associadas ao carregamento.
     * @param {Object} carregamento O carregamento que terá as rotas listadas.
     * @returns {string} As rotas associadas ao carregamento informado.
     */
    obterRotas: function (carregamento) {
      if (!carregamento || !carregamento.rotas) {
        return '';
      }

      return carregamento.rotas.slice()
        .join(', ');
    },

    /**
     * Indica que um carregamento será editado.
     * @param {Object} item O carregamento que será editado.
     * @param {number} numeroLinha O número da linha que contém o item que será editado.
     */
    editar: function (item, numeroLinha) {
      this.iniciarCadastroOuAtualizacao_(item);
      this.numeroLinhaEdicao = numeroLinha;
    },

    /**
     * Atualiza os dados do carregamento.
     * @param {Object} event O objeto com o evento do JavaScript.
     */
    atualizar: function (event) {
      if (!event || !this.validarFormulario_(event.target)) {
        return;
      }

      var carregamentoAtualizar = this.patch(this.carregamento, this.carregamentoOriginal);
      var vm = this;

      Servicos.Carregamentos.atualizar(this.carregamento.id, carregamentoAtualizar)
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
     * Cancela a edição ou cadastro do carregamento.
     */
    cancelar: function () {
      this.numeroLinhaEdicao = -1;
      this.inserindo = false;
    },

    /**
     * Inicia o cadastro de carregamento.
     */
    iniciarCadastro: function () {
      this.iniciarCadastroOuAtualizacao_();
      this.inserindo = true;
    },

    /**
     * Função executada para criação dos objetos necessários para edição ou cadastro de carregamento.
     * @param {?Object} [carregamento=null] O carregamento que servirá como base para criação do objeto (para edição).
     */
    iniciarCadastroOuAtualizacao_: function (carregamento) {
      this.motoristaAtual = carregamento && carregamento.motorista ? this.clonar(carregamento.motorista) : null;
      this.veiculoAtual = carregamento && carregamento.veiculo ? this.clonar(carregamento.veiculo) : null;

      this.carregamento = {
        id: carregamento ? carregamento.id : null,
        idMotorista: carregamento && carregamento.motorista ? carregamento.motorista.id : null,
        placa: carregamento && carregamento.veiculo ? carregamento.veiculo.codigo : null,
        dataPrevisaoSaida: carregamento ? carregamento.dataPrevisaoSaida : null,
        loja: carregamento ? carregamento.loja : null,
        situacao: carregamento ? carregamento.situacao : null,
        rotas: carregamento ? carregamento.rotas : null,
        peso: carregamento ? carregamento.peso : null,
        valorTotalPedidos: carregamento ? carregamento.valorTotalPedidos : null,
        situacaoFaturamento: carregamento ? carregamento.situacaoFaturamento : null
      };

      this.carregamentoOriginal = this.clonar(this.carregamento);
    },

    /**
     * Obtém o link para geração de novo carregamento.
     * @returns {string} O link para geração de carregamento.
     */
    obterLinkGerarCarregamento: function () {
      return '../Cadastros/CadCarregamento.aspx';
    },

    /**
     * Fatura um carregamento.
     * @param {Object} item O carregamento que será faturado.
     */
    faturar: function (item) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja faturar este carregamento?')) {
        return;
      }

      var vm = this;

      Servicos.Carregamentos.faturar(item.id)
        .then(function (resposta) {
          vm.exibirFaturamento(vm);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.download_('Carregamento(' + item.id + ').txt', erro.mensagem);
          }
        });
    },

    /**
     * Exibe para impressão faturamento do carregamento.
     * @param {Object} vm O contexto atual da tela.
     * @param {Object} item O carregamento que será faturado.
     */
    exibirFaturamento: function (vm, item) {
      if (vm == null) {
        vm = this;
      }

      Servicos.Carregamentos.obterDadosFaturamento(item.id)
        .then(function (resposta) {
          vm.abrirJanela(600, 800, '../Handlers/FaturamentoCarregamento.ashx?idsImprimir=' + resposta.data);
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre um popup para incluir ordens de carga no carregamento.
     * @param {Object} item O carregamento que será usado para incluir ordens de carga.
     */
    abrirInclusaoOrdemCarga: function (item) {
      this.abrirJanela(600, 800, '../Cadastros/CadItensCarregamento.aspx?popup=true&idCarregamento=' + item.id);
    },

    /**
     * Exibe um relatório do carregamento, aplicando os filtros da tela.
     * @param {Object} item O carregamento que será exibido para impressão.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirRelatorioCarregamento: function (item, exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=Carregamento&idCarregamento=' + item.id + "&ordenar=" + this.filtro.ordenacao + "&exportarExcel=" + exportarExcel;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exibe um relatório com a listagem de carregamentos, aplicando os filtros da tela.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaCarregamentos: function (exportarExcel) {
      var url = '../Relatorios/RelBase.aspx?rel=ListaCarregamento' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Exclui um carregamento.
     * @param {Object} carregamento O carregamento que será excluído.
     */
    excluir: function (carregamento) {
      if (!this.perguntar('Confirmação', 'Tem certeza que deseja excluir este carregamento?')) {
        return;
      }

      var vm = this;

      Servicos.Carregamentos.excluir(carregamento.id)
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
     * Retornar uma string com os filtros selecionados na tela.
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'carregamento', this.filtro.id);
      this.incluirFiltroComLista(filtros, 'idRota', this.filtro.idRota);
      this.incluirFiltroComLista(filtros, 'motorista', this.filtro.idMotorista);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idOC', this.filtro.idOrdemCarga);
      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'placa', this.filtro.placa);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacaoCarregamento);
      this.incluirFiltroComLista(filtros, 'dtPrevSaidaIni', this.filtro.periodoPrevisaoSaidaInicio);
      this.incluirFiltroComLista(filtros, 'dtPrevSaidaFim', this.filtro.periodoPrevisaoSaidaFim);

      return filtros.length
        ? '&' + filtros.join('&')
        : '';
    },

    /**
     * Indica se as ordens de carga estão sendo exibidas na lista.
     * @param {!number} indice O número da linha que está sendo verificada.
     * @returns {boolean} Verdadeiro, se os itens estiverem sendo exibidos.
     */
    exibindoOrdensCarga: function (indice) {
      return this.ordensCargaEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Alterna a exibição das ordens de carga.
     */
    alternarExibicaoOrdensCarga: function (indice) {
      var i = this.ordensCargaEmExibicao.indexOf(indice);

      if (i > -1) {
        this.ordensCargaEmExibicao.splice(i, 1);
      } else {
        this.ordensCargaEmExibicao.push(indice);
      }
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @type {number}
     */
    numeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Atualiza a lista de carregamentos.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar();
    },

    /**
     * Função que indica se o formulário de carregamentos possui valores válidos de acordo com os controles.
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
     * Função que realiza o download de uma string em formato txt
     * @param {string} nomeDoArquivo O nome do arquivo txt que o usuário irá receber.
     * @param {string} conteudo O conteúdo do arquivo de texto.
     */
    download_: function (nomeDoArquivo, conteudo) {
      var link = document.createElement('a');
      link.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(conteudo));
      link.setAttribute('download', nomeDoArquivo);

      if (document.createEvent) {
        var event = document.createEvent('MouseEvents');
        event.initEvent('click', true, true);
        link.dispatchEvent(event);
      } else {
        link.click();
      }
    }
  },

  mounted: function() {
    var vm = this;

    Servicos.Carregamentos.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  },

  watch: {
    /**
     * Observador para a variável 'motoristaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    motoristaAtual: {
      handler: function (atual) {
        this.carregamento.idMotorista = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    veiculoAtual: {
      handler: function (atual) {
        this.carregamento.placa = atual ? atual.codigo : null;
      },
      deep: true
    }
  }
});
