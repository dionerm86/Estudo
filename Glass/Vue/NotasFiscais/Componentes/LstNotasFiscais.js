const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista()],

  data: {
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
     * Exibe a tela de anexos da nota fiscal.
     * @param {Object} item A Nota que será usado para abertura da tela.
     */
    abrirAnexos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=notaFiscal');
    },

    /**
     * Retornar uma string com os filtros selecionados na tela
     */
    formatarFiltros_: function () {
      var filtros = [];

      this.incluirFiltroComLista(filtros, 'numeroNfe', this.filtro.numeroNfe);
      this.incluirFiltroComLista(filtros, 'idPedido', this.filtro.idPedido);
      this.incluirFiltroComLista(filtros, 'modelo', this.filtro.modelo);
      this.incluirFiltroComLista(filtros, 'idLoja', this.filtro.idLoja);
      this.incluirFiltroComLista(filtros, 'idCliente', this.filtro.idCliente);
      this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
      this.incluirFiltroComLista(filtros, 'tipoFiscal', this.filtro.tipoFiscal);
      this.incluirFiltroComLista(filtros, 'idFornec', this.filtro.idFornecedor);
      this.incluirFiltroComLista(filtros, 'nomeFornec', this.filtro.nomeFornecedor);
      this.incluirFiltroComLista(filtros, 'codRota', this.filtro.codigoRota);
      this.incluirFiltroComLista(filtros, 'situacao', this.filtro.situacao);
      this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoEmissaoInicio);
      this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoEmissaoFim);
      this.incluirFiltroComLista(filtros, 'idsCfop', this.filtro.idsCfop);
      this.incluirFiltroComLista(filtros, 'tiposCfop', this.filtro.tiposCfop);
      this.incluirFiltroComLista(filtros, 'dataEntSaiIni', this.filtro.periodoEntradaSaidaInicio);
      this.incluirFiltroComLista(filtros, 'dataEntSaiFim', this.filtro.periodoEntradaSaidaFim);
      this.incluirFiltroComLista(filtros, 'formaPagto', this.filtro.tipoVenda);
      this.incluirFiltroComLista(filtros, 'idsFormaPagtoNotaFiscal', this.filtro.idsFormaPagamento);
      this.incluirFiltroComLista(filtros, 'tipoNf', this.filtro.tipoDocumento);
      this.incluirFiltroComLista(filtros, 'finalidade', this.filtro.finalidade);
      this.incluirFiltroComLista(filtros, 'formaEmissao', this.filtro.tipoEmissao);
      this.incluirFiltroComLista(filtros, 'infCompl', this.filtro.informacaoComplementar);
      this.incluirFiltroComLista(filtros, 'codInternoProd', this.filtro.codigoInternoProduto);
      this.incluirFiltroComLista(filtros, 'descrProd', this.filtro.descricaoProduto);
      this.incluirFiltroComLista(filtros, 'lote', this.filtro.lote);
      this.incluirFiltroComLista(filtros, 'apenasNotasFiscaisSemAnexo', this.filtro.apenasNotasFiscaisSemAnexo);
      this.incluirFiltroComLista(filtros, 'valorInicial', this.filtro.valorNotaFiscalInicio);
      this.incluirFiltroComLista(filtros, 'valorFinal', this.filtro.valorNotaFiscalFim);
      this.incluirFiltroComLista(filtros, 'ordenar', this.filtro.ordenacaoFiltro);
      this.incluirFiltroComLista(filtros, 'agrupar', this.filtro.agrupar);

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

  mounted: function() {
    var vm = this;

    Servicos.NotasFiscais.obterConfiguracoesLista()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
