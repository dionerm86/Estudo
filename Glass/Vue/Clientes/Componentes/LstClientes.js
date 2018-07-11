const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar],

  data: {
    dadosOrdenacao_: {
      campo: 'id',
      direcao: 'desc'
    },
    configuracoes: {},
    filtro: {}
  },

  methods: {
    /**
     * Busca os clientes para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca de clientes.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca de clientes.
     */
    atualizarClientes: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Clientes.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Realiza a ordenação da lista de clientes.
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
     * Retorna o link para a tela de edição de clientes.
     * @param {Object} item O cliente que será usado para construção do link.
     * @returns {string} O link que redireciona para a edição de clientes.
     */
    obterLinkEditarCliente: function(item) {
      return '../Cadastros/CadCliente.aspx?idCli=' + item.id;
    },

    /**
     * Retorna o link para a tela de sugestões de clientes.
     * @param {Object} item O cliente que terá sugestões feitas.
     * @returns {string} O link que redireciona para a sugestão de clientes.
     */
    obterLinkSugestoes: function(item) {
      return '../Listas/LstSugestaoCliente.aspx?idCliente=' + item.id;
    },

    /**
     * Retorna o link para a tela de cadastro de cliente, de acordo com a listagem sendo exibida.
     * @returns {string} O link para a tela de cadastro de cliente.
     */
    obterLinkInserirCliente: function() {
      return '../Cadastros/CadCliente.aspx';
    },

    /**
     * Retorna o link para exibir os preços de tabela do cliente.
     * @param {Object} item O cliente que será usado para consultar os preços de tabela.
     */
    obterLinkPrecosTabela: function (item) {
      return '../Relatorios/ListaPrecoTabCliente.aspx?idCli=' + item.id;
    },

    /**
     * Abre um popup para editar a tabela de desconto/acréscimo do cliente.
     * @param {Object} item O cliente que será usado para alterar a tabela de desconto/acréscimo.
     */
    abrirTabelaDescontoAcrescimo: function (item) {
      this.abrirJanela(500, 650, '../Cadastros/CadDescontoAcrescimoCliente.aspx?IdCliente=' + item.id);
    },

    /**
     * Abre um popup para exibir/editar os anexos do cliente.
     * @param {Object} item O cliente que será usado para exibir/alterar os anexos.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=cliente');
    },

    /**
     * Exibe um relatório com os dados do cliente.
     * @param {Object} item O cliente que será exibido.
     */
    abrirFichaCliente: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?Rel=FichaClientes&idCli=' + item.id);
    },

    /**
     * Exibe um relatório com a listagem de clientes aplicando os filtros da tela.
     * @param {Boolean} ficha Define se os clientes serão impressos no formato de ficha e não de listagem.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirListaClientes: function (ficha, exportarExcel) {
      var filtroReal = formatarFiltros_();

      if (filtro.nomeCliente && filtro.nomeCliente.indexOf('&') >= 0) {
        this.exibirMensagem("O filtro Nome/Apelido do cliente não deve conter o caractere '&', pois ele é utilizado como chave para geração do relatório. Tente filtrar sem o nome do cliente ou apenas com a primeira parte do nome antes do '&'.");
        return false;
      }

      if (filtroReal == '' && !this.perguntar("É recomendável aplicar um filtro. Deseja realmente prosseguir?")) {
          return false;
      }

      var url = '../Relatorios/RelBase.aspx?Rel=' + (ficha ? "Ficha" : "Lista") + 'Clientes' + filtroReal + '&exportarExcel=' + exportarExcel;

      this.abrirJanela(600, 800, url);
    },

    /**
     * Ativa os clientes, com base nos filtros da tela.
     */
    ativarClientes: function () {
      if (!this.perguntar("ATENÇÃO: Essa opção ativará TODOS os clientes inativos que se encaixam nos filtros especificados.\nDeseja continuar?")) {
        return;
      }

      var vm = this;

      var filtroUsar = this.clonar(filtro || {});

      Servicos.Clientes.ativar(filtroUsar)
        .then(function (resposta) {
          vm.exibirMensagem('Clientes ativados com sucesso!');
          vm.filtro.refresh_++;
          vm.filtrar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre uma tela para alterar os vendedores dos clientes, com base nos filtros da tela.
     */
    abrirAlteracaoVendedor: function () {
      this.abrirJanela(200, 400, '../Utils/AlterarVendedorCli.aspx?vue=true');
    },

    /**
     * Altera o vendedor dos clientes definidos nos filtros.
     */
    alterarVendedor: function (idVendedorNovo) {
      var vm = this;

      var filtroUsar = this.clonar(filtro || {});

      Servicos.Clientes.alterarVendedor(filtroUsar, idVendedorNovo)
        .then(function (resposta) {
          vm.exibirMensagem('Vendedor alterado com sucesso!');
          vm.filtro.refresh_++;
          vm.filtrar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre uma tela para alterar as rotas dos clientes, com base nos filtros da tela.
     */
    abrirAlteracaoRota: function () {
      this.abrirJanela(200, 400, '../Utils/AlterarRotaClientes.aspx?vue=true');
    },

    /**
     * Altera a rota dos clientes definidos nos filtros.
     */
    alterarRota: function (idRotaNova) {
      var vm = this;

      var filtroUsar = this.clonar(filtro || {});

      Servicos.Clientes.alterarRota(filtroUsar, idRotaNova)
        .then(function (resposta) {
          vm.exibirMensagem('Rota alterada com sucesso!');
          vm.filtro.refresh_++;
          vm.filtrar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Exclui o cliente.
     * @param {Object} item O cliente que será excluído.
     */
    excluir: function (item) {
      if (!this.perguntar("Tem certeza que deseja excluir este cliente?")) {
        return;
      }

      var vm = this;

      Servicos.Clientes.excluir(item.id)
        .then(function (resposta) {
          vm.filtro.refresh_++;
          vm.filtrar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Ativa/inativa o cliente.
     * @param {Object} item O cliente que será ativado/inativado.
     */
    alterarSituacao: function (item) {
      if (!this.perguntar("Tem certeza que deseja alterar a situação deste cliente?")) {
        return;
      }

      var vm = this;

      Servicos.Clientes.alterarSituacao(item.id)
        .then(function (resposta) {
          vm.filtro.refresh_++;
          vm.filtrar();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = []
      const incluirFiltro = function (campo, valor) {
        if (valor) {
          filtros.push(campo + '=' + valor);
        }
      }

      incluirFiltro('idCli', filtro.id);
      incluirFiltro('nome', filtro.nomeCliente);
      incluirFiltro('cpfCnpj', filtro.cpfCnpj);
      incluirFiltro('idLoja', filtro.idLoja);
      incluirFiltro('telefone', filtro.telefone);
      incluirFiltro('endereco', filtro.endereco);
      incluirFiltro('bairro', filtro.bairro);
      incluirFiltro('idCidade', filtro.idCidade);
      incluirFiltro('idTipoCliente', filtro.tipo);
      incluirFiltro('situacao', filtro.situacao);
      incluirFiltro('codRota', filtro.codRota);
      incluirFiltro('idFunc', filtro.idVendedor);
      incluirFiltro('tipoFiscal', filtro.tipoFiscal);
      incluirFiltro('formasPagto', filtro.formasPagamento);
      incluirFiltro('dataCadIni', filtro.periodoCadastroInicio);
      incluirFiltro('dataCadFim', filtro.periodoCadastroFim);
      incluirFiltro('dataSemCompraIni', filtro.periodoSemCompraInicio);
      incluirFiltro('dataSemCompraFim', filtro.periodoSemCompraFim);
      incluirFiltro('dataInativadoIni', filtro.periodoInativadoInicio);
      incluirFiltro('dataInativadoFim', filtro.periodoInativadoFim);
      incluirFiltro('idTabelaDesconto', filtro.idTabelaDescontoAcrescimo);
      incluirFiltro('apenasSemRota', filtro.apenasSemRota);
      incluirFiltro('agruparVend', filtro.agruparVendedor);
      incluirFiltro('exibirHistorico', filtro.exibirHistorico);
      incluirFiltro('uf', filtro.uf);

      return filtros.length > 0
        ? '&' + filtros.join('&')
        : '';
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

    Servicos.Clientes.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
