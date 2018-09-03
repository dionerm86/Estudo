const app = new Vue({
  el: '#app',
  mixins: [Mixins.Clonar],

  data: {
    dadosOrdenacao_: {
      campo: '',
      direcao: ''
    },
    configuracoes: {},
    filtro: {},
    notasAutorizadasEFinalizadas: false
  },

  methods: {
    /**
     * Busca as notas fiscais para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    atualizarNotasFiscais: function(filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      this.verificarFiltroAutorizadaFinalizada();
      return Servicos.NotasFiscais.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
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
     * Verifica se está filtrando por notas autorizadas e/ou finalizadas.
     */
    verificarFiltroAutorizadaFinalizada: function () {
      if (this.filtro.situacao == null) {
        return;
      }

      for (var i=0; i< this.filtro.situacao.length; i++) {
        if (this.filtro.situacao[i] == this.configuracoes.situacaoAutorizada || this.filtro.situacao[i] == this.configuracoes.situacaoFinalizada) {
          this.notasAutorizadasEFinalizadas = true;
          return;
        }
      }

      this.notasAutorizadasEFinalizadas = false;
    },

    /**
     * Verifica se ao editar a nota, será feita uma alteração manual de valores (sem cálculos automáticos).
     * @param {Object} item A nota fiscal que será editada.
     */
    verificarAlteracaoManual: function (item) {
      return item.situacao.id == this.configuracoes.situacaoFinalizada || (item.situacao.id == this.configuracoes.situacaoAutorizada && item.permissoes.possuiCartaCorrecaoRegistrada);
    },

    /**
     * Obtém link para a tela de edição de NF-e.
     * @param {Object} item A nota fiscal que será editada.
     */
    obterLinkEditar: function (item) {
      var url = '../Cadastros/CadNotaFiscal.aspx?idNf=' + item.id + '&tipo=' + item.tipoDocumento.id;

      if (this.verificarAlteracaoManual(item)) {
        url += '&manual=1';
      }

      return url;
    },

    /**
     * Exclui a nota fiscal.
     * @param {Object} item A nota fiscal que será excluída.
     */
    excluir: function (item) {
      if (!this.perguntar("Tem certeza que deseja excluir esta nota fiscal?")) {
        return;
      }

      var vm = this;

      Servicos.NotasFiscais.excluir(item.id)
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
     * Exibe os logs de eventos da nota fiscal.
     * @param {Object} item A nota fiscal que será exibida os logs.
     */
    abrirLogEventos: function (item) {
      this.abrirJanela(450, 700, '../Utils/ShowLogNfe.aspx?IdNf=' + item.id);
    },

    /**
     * Exibe o DANFE da nota fiscal.
     * @param {Object} item A nota fiscal a partir da qual será gerado o DANFE.
     */
    abrirImpressaoDanfe: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/NFe/RelBase.aspx?rel=Danfe&idNf=' + item.id);
    },

    /**
     * Exibe a impressão de uma nota fiscal de terceiros.
     * @param {Object} item A nota fiscal a partir da qual será gerado o DANFE.
     */
    abrirImpressaoNotaFiscalTerceiros: function (item) {
      this.abrirJanela(600, 800, '../Relatorios/NFe/RelBase.aspx?rel=NfTerceiros&idNf=' + item.id);
    },

    /**
     * Consulta a situação do lote da nota fiscal na SEFAZ.
     * @param {Object} item A nota fiscal que será consultada.
     */
    consultarSituacaoLote: function (item) {
      var vm = this;

      Servicos.NotasFiscais.consultarSituacaoLote(item.id)
        .then(function (resposta) {
          vm.exibirMensagem('Emissão de nota fiscal', resposta.data.mensagem);
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Consulta a situação da nota fiscal na SEFAZ.
     * @param {Object} item A nota fiscal que será consultada.
     */
    consultarSituacao: function (item) {
      var vm = this;

      Servicos.NotasFiscais.consultarSituacao(item.id)
        .then(function (resposta) {
          vm.exibirMensagem('Emissão de nota fiscal', resposta.data.mensagem);
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Baixa o XML da nota fiscal.
     * @param {Object} item A nota fiscal que será baixado o XML.
     * @param {Boolean} inutilizacao Define se será baixado XML de inutilização.
     */
    baixarXml: function (item, inutilizacao) {
      Servicos.NotasFiscais.baixarXml(item.id, inutilizacao);
    },

    /**
     * Abre uma tela para anexar XML de notas fiscais de terceiros.
     * @param {Object} item A nota fiscal a partir da qual será anexado o XML.
     */
    abrirAnexoXmlTerceiros: function (item) {
      this.abrirJanela(600, 800, '../Utils/AnexarXMLNFeEntradaTerceiros.aspx?idNfTer=' + item.id);
    },

    /**
     * Baixa o XML da nota fiscal de terceiros.
     * @param {Object} item A nota fiscal de terceiros que será baixado o XML.
     */
    baixarXmlTerceiros: function (item) {
      Servicos.NotasFiscais.baixarXmlTerceiros(item.id);
    },

    /**
     * Abre uma tela para gerenciar os Processos/Documentos Referenciados da nota fiscal.
     * @param {Object} item A nota fiscal a partir da qual serão exibidos os processos/documentos referenciados.
     */
    abrirProcessosReferenciados: function (item) {
      this.abrirJanela(600, 800, '../Utils/DocRefNotaFiscal.aspx?idNf=' + item.id);
    },

    /**
     * Abre uma tela para gerenciar as informações adicionais da nota fiscal.
     * @param {Object} item A nota fiscal a partir da qual serão exibidas as informações adicionais.
     */
    abrirInformacoesAdicionais: function (item) {
      this.abrirJanela(600, 800, '../Utils/InfoAdicNotaFiscal.aspx?idNf=' + item.id);
    },

    /**
     * Emite uma nota fiscal gerada em modo de contingência FS-DA.
     * @param {Object} item A nota fiscal que será emitida.
     */
    emitirNotaFiscalFsda: function (item) {
      var vm = this;

      Servicos.NotasFiscais.emitirNotaFiscalFsda(item.id)
        .then(function (resposta) {
          vm.exibirMensagem('Emissão de nota fiscal FS-DA', resposta.data.mensagem);
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre uma tela para gerenciar as observações do lançamento fiscal da nota.
     * @param {Object} item A nota fiscal a partir da qual serão exibidas as observações fiscais.
     */
    abrirObservacoesLancamentoFiscal: function (item) {
      this.abrirJanela(600, 800, '../Utils/SetObsLancFiscal.aspx?idNf=' + item.id);
    },

    /**
     * Abre uma tela para gerenciar os ajustes do documento fiscal da nota.
     * @param {Object} item A nota fiscal a partir da qual serão exibidas os ajustes do documento fiscal.
     */
    abrirAjustesDocumentoFiscal: function (item) {
      this.abrirJanela(600, 950, '../Listas/LstAjusteDocumentoFiscal.aspx?idNf=' + item.id);
    },

    /**
     * Reenvia email para o cliente com a nota fiscal.
     * @param {Object} item A nota fiscal que será enviada por email.
     * @param {?boolean} cancelamento Define se é para reenviar email de cancelamento.
     */
    reenviarEmail: function (item, cancelamento) {
      if (!this.perguntar('Reenviar e-mail', 'Deseja reenviar o e-mail do XML/DANFE?')) {
        return;
      }

      var vm = this;

      Servicos.NotasFiscais.reenviarEmail(item.id, cancelamento)
        .then(function (resposta) {
          vm.exibirMensagem('Reenvio de e-mail', 'E-mail adicionado na fila de envios.');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre uma tela para visualizar dados de rentabilidade.
     * @param {Object} item A nota fiscal que será analisado os dados.
     */
    abrirRentabilidade: function (item) {
      this.abrirJanela(500, 700, '../Relatorios/Rentabilidade/VisualizacaoItemRentabilidade.aspx?tipo=notafiscal&id=' + item.id);
    },

    /**
     * Reabre a nota fiscal para edição.
     * @param {Object} item A nota fiscal que será reaberta.
     */
    reabrir: function (item) {
      if (!this.perguntar('Reabrir nota fiscal', 'Deseja reabrir esta nota fiscal??')) {
        return;
      }

      var vm = this;

      Servicos.NotasFiscais.reabrir(item.id)
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
     * Gera uma nota fiscal complementar a partir de uma nota fiscal.
     * @param {Object} item A nota fiscal usada para gerar uma nota complementar.
     */
    gerarNotaFiscalComplementar: function (item) {
      if (!this.perguntar('Gerar nota fiscal complementar', 'Tem certeza que deseja gerar uma NF-e complementar desta nota?')) {
        return;
      }

      var vm = this;

      Servicos.NotasFiscais.gerarNotaFiscalComplementar(item.id)
        .then(function (resposta) {
          window.location.href = '../Cadastros/CadNotaFiscal.aspx?idNf=' + resposta.data.id + '&tipo=' + item.tipoDocumento.id;
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre uma tela com as cartas de correção da nota.
     * @param {Object} item A nota fiscal que será exibidas as cartas de correção.
     */
    abrirCartaCorrecao: function (item) {
      this.abrirJanela(600, 800, '../Cadastros/CadCartaCorrecao.aspx?popup=true&idNf=' + item.id);
    },

    /**
     * Obtém link para a tela: "Informações Adicionais dos Ajustes da Apuração do ICMS".
     * @param {Object} item A nota fiscal que será editada.
     */
    obterLinkAjusteApuracaoIcms: function (item) {
      return 'LstAjusteApuracaoIdentificacaoDocFiscal.aspx?idNf=' + item.id;
    },

    /**
     * Abre uma tela para gerenciar o centro de custo da nota fiscal.
     * @param {Object} item A nota fiscal que será gerenciado o centro de custo.
     */
    abrirCentroCusto: function (item) {
      this.abrirJanela(365, 700, '../Utils/SelCentroCusto.aspx?idNf=' + item.id);
    },

    /**
     * Separa valores de contas a receber da liberação e da nota fiscal.
     * @param {Object} item A nota fiscal que terá a separação de valores.
     */
    separarValores: function (item) {
      var vm = this;

      Servicos.NotasFiscais.separarValores(item.id)
        .then(function (resposta) {
          vm.exibirMensagem('Operação concluída', 'Vinculação feita com sucesso!');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Cancela a separação de valores de contas a receber da liberação e da nota fiscal.
     * @param {Object} item A nota fiscal que terá a separação de valores cancelada.
     */
    cancelarSeparacaoValores: function (item) {
      var vm = this;

      Servicos.NotasFiscais.cancelarSeparacaoValores(item.id)
        .then(function (resposta) {
          vm.exibirMensagem('Operação concluída', 'Cancelamento feito com sucesso!');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Abre uma tela com um log de movimentações de estoque da nota
     * @param {Object} item A nota fiscal que será exibido o log de movimentação de estoque.
     */
    abrirLogMovimentacaoEstoque: function (item) {
      this.abrirJanela(600, 800, '../Utils/LogMovimentacaoNotaFiscal.aspx?idNf=' + item.id);
    },

    /**
     * Emite uma NFC-e.
     * @param {Object} item A NFC-e que será emitida.
     */
    emitirNfce: function (item) {
      var vm = this;

      Servicos.NotasFiscais.emitirNfce(item.id)
        .then(function (resposta) {
          if (reposta.mensagem.indexOf('Impossível conectar-se ao servidor remoto') > -1 && vm.item.consumidor) {
            var confirmacao = "Houve uma falha de conexão ao tentar emitir a NFC-e.\n\nNesse caso é possível realizar a emissão em Contingência Offline, porém a mesma deverá ser posteriormente autorizada.";
            confirmacao += "\nA não autorização em 24hrs, seja por inconsistência ou persistência do problema, poderá resultar em custos e riscos adicionais.\n\nDeseja prosseguir?";

            if (vm.perguntar("Receita ou internet indisponível", confirmacao)) {
              vm.emitirNfceOffline(vm.item);
            }
          } else if (reposta.mensagem != 'Lote processado.') {
            vm.exibirMensagem('Retorno emissão', resposta.data.mensagem);
          } else if (vm.configuracoes.ufUsuario != "BA" && vm.configuracoes.ufUsuario != "SP") {
            vm.consultarSituacaoLote(vm.item);
          }
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Emite uma NFC-e em modo offline.
     * @param {Object} item A NFC-e que será emitida em modo offline.
     */
    emitirNfceOffline: function (item) {
      var vm = this;

      Servicos.NotasFiscais.emitirNfceOffline(item.id)
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
     * Altera o modo de contingência da nota fiscal.
     * @param {!number} tipoContingencia Define se o modo de contingência a ser alterado.
     */
    alterarContingencia: function (tipoContingencia) {
      var vm = this;

      Servicos.NotasFiscais.alterarContingencia(tipoContingencia)
        .then(function (resposta) {
          vm.exibirMensagem('Processo concluído', 'Modo de contingência habilitado.');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Desabilita o modo de contingência da nota fiscal.
     */
    desabilitarContingencia: function () {
      var vm = this;

      Servicos.NotasFiscais.desabilitarContingencia(fsda || 0)
        .then(function (resposta) {
          vm.exibirMensagem('Processo concluído', 'Modo de contingência desabilitado.');
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Exibe a impressão da lista de notas fiscais.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirImpressaoNotasFiscais: function (exportarExcel) {
      var nomeRelatorio = this.notasAutorizadasEFinalizadas ? "FiscalAutFin" : "Fiscal";

      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=' + nomeRelatorio + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Exibe a impressão da lista de produtos de notas fiscais.
     * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
     */
    abrirImpressaoProdutosNotasFiscais: function (exportarExcel) {
      this.abrirJanela(600, 800, '../Relatorios/RelBase.aspx?rel=FiscalAutFinProd' + this.formatarFiltros_() + '&exportarExcel=' + exportarExcel);
    },

    /**
     * Baixa vários arquivos XML em lote.
     * @param {Boolean} inutilizacao Define se deverão ser baixados xmls de inutilização.
     */
    baixarXmlEmLote: function (inutilizacao) {
      var filtros = this.formatarFiltros_() + (inutilizacao ? '&tipo=inut' : '');
      Servicos.NotasFiscais.baixarXmlEmLote(filtros);
    },

    /**
     * Abre uma tela para consultar a disponibilidade do servidor de emissão de notas fiscais.
     */
    consultarDisponibilidadeNfe: function () {
      this.abrirJanela(600, 800, 'http://www.nfe.fazenda.gov.br/portal/disponibilidade.aspx?versao=0.00&tipoConteudo=Skeuqr8PQBY=');
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

      incluirFiltro('numeroNfe', this.filtro.numeroNfe);
      incluirFiltro('idPedido', this.filtro.idPedido);
      incluirFiltro('modelo', this.filtro.modelo);
      incluirFiltro('idLoja', this.filtro.idLoja);
      incluirFiltro('idCliente', this.filtro.idCliente);
      incluirFiltro('nomeCliente', this.filtro.nomeCliente);
      incluirFiltro('tipoFiscal', this.filtro.tipoFiscal);
      incluirFiltro('idFornec', this.filtro.idFornecedor);
      incluirFiltro('nomeFornec', this.filtro.nomeFornecedor);
      incluirFiltro('codRota', this.filtro.codigoRota);
      incluirFiltro('situacao', this.filtro.situacao);
      incluirFiltro('dataIni', this.filtro.periodoEmissaoInicio ? this.filtro.periodoEmissaoInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataFim', this.filtro.periodoEmissaoFim ? this.filtro.periodoEmissaoFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('idsCfop', this.filtro.idsCfop);
      incluirFiltro('tiposCfop', this.filtro.tiposCfop);
      incluirFiltro('dataEntSaiIni', this.filtro.periodoEntradaSaidaInicio ? this.filtro.periodoEntradaSaidaInicio.toLocaleDateString('pt-BR') : null);
      incluirFiltro('dataEntSaiFim', this.filtro.periodoEntradaSaidaFim ? this.filtro.periodoEntradaSaidaFim.toLocaleDateString('pt-BR') : null);
      incluirFiltro('formaPagto', this.filtro.tipoVenda);
      incluirFiltro('idsFormaPagtoNotaFiscal', this.filtro.idsFormaPagamento);
      incluirFiltro('tipoNf', this.filtro.tipoDocumento);
      incluirFiltro('finalidade', this.filtro.finalidade);
      incluirFiltro('formaEmissao', this.filtro.tipoEmissao);
      incluirFiltro('infCompl', this.filtro.informacaoComplementar);
      incluirFiltro('codInternoProd', this.filtro.codigoInternoProduto);
      incluirFiltro('descrProd', this.filtro.descricaoProduto);
      incluirFiltro('lote', this.filtro.lote);
      incluirFiltro('valorInicial', this.filtro.valorNotaFiscalInicio);
      incluirFiltro('valorFinal', this.filtro.valorNotaFiscalFim);
      incluirFiltro('ordenar', this.filtro.ordenacaoFiltro);
      incluirFiltro('agrupar', this.filtro.agrupar);

      return filtros.length > 0
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

    Servicos.NotasFiscais.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});