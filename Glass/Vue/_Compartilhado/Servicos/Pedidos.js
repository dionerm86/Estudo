var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de pedidos.
 */
Servicos.Pedidos = (function (http) {
  const API = '/api/v1/pedidos/';

  return {
    /**
     * Objeto com os serviços para a API de ambientes de pedidos.
     */
    Ambientes: {
      /**
       * Recupera a lista de ambientes de um pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca dos ambientes.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obter: function (idPedido, pagina, numeroRegistros, ordenacao) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        var filtro = {
          pagina,
          numeroRegistros,
          ordenacao
        };

        return http().get(API + idPedido + '/ambientes', {
          params: filtro
        });
      },

      /**
       * Remove um ambiente do pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca do ambiente.
       * @param {!number} idAmbiente O identificador do ambiente que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idPedido, idAmbiente) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        if (!idAmbiente) {
          throw new Error('Ambiente é obrigatório.');
        }

        return http().delete(API + idPedido + '/ambientes/' + idAmbiente);
      },

      /**
       * Insere um ambiente de pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca do ambiente.
       * @param {!Object} ambiente O objeto com os dados do ambiente a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (idPedido, ambiente) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        return http().post(API + idPedido + '/ambientes', ambiente);
      },

      /**
       * Altera os dados de um ambiente de pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca do ambiente.
       * @param {!number} idAmbiente O identificador do ambiente que será alterado.
       * @param {!Object} ambiente O objeto com os dados do ambiente a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idPedido, idAmbiente, ambiente) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        if (!idAmbiente) {
          throw new Error('Ambiente é obrigatório.');
        }

        if (!ambiente || ambiente === {}) {
          return Promise.resolve();
        }

        return http().patch(API + idPedido + '/ambientes/' + idAmbiente, ambiente);
      }
    },

    /**
     * Objeto com os serviços para a API de produtos de pedidos.
     */
    Produtos: {
      /**
       * Recupera a lista de produtos de um pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca dos produtos.
       * @param {?number} idAmbiente O identificador do ambiente que contém os produtos (se necessário).
       * @param {?number} idProdutoPai O identificador do produto "pai" (para produtos de composição/laminados).
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obter: function (idPedido, idAmbiente, idProdutoPai, pagina, numeroRegistros, ordenacao) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        var filtro = {
          idAmbiente,
          idProdutoPai,
          pagina,
          numeroRegistros,
          ordenacao
        };

        return http().get(API + idPedido + '/produtos', {
          params: filtro
        });
      },

      /**
       * Remove um produto do pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca do produto.
       * @param {!number} idProduto O identificador do produto que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idPedido, idProduto) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        if (!idProduto) {
          throw new Error('Produto é obrigatório.');
        }

        return http().delete(API + idPedido + '/produtos/' + idProduto);
      },

      /**
       * Insere um produto de pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca do produto.
       * @param {!Object} produto O objeto com os dados do ambiente a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (idPedido, produto) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        return http().post(API + idPedido + '/produtos', produto);
      },

      /**
       * Altera os dados de um produto de pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca do produto.
       * @param {!number} idProduto O identificador do produto que será alterado.
       * @param {!Object} produto O objeto com os dados do produto a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idPedido, idProduto, produto) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        if (!idProduto) {
          throw new Error('Produto é obrigatório.');
        }

        if (!produto || produto === {}) {
          return Promise.resolve();
        }

        return http().patch(API + idPedido + '/produtos/' + idProduto, produto);
      },

      /**
       * Altera a observação de um produto de pedido.
       * @param {!number} idPedido O identificador do pedido que será usado para busca do produto.
       * @param {!number} idProduto O identificador do produto que será alterado.
       * @param {string} observacao A nova observação do produto de pedido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      salvarObservacao: function (idPedido, idProduto, observacao) {
        if (!idPedido) {
          throw new Error('Pedido é obrigatório.');
        }

        if (!idProduto) {
          throw new Error('Produto é obrigatório.');
        }

        return http().patch(API + idPedido + '/produtos/' + idProduto + '/observacao', {
          observacao: observacao || ''
        });
      }
    },

    /**
     * Recupera a lista de pedidos.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de pedidos.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Recupera a lista de pedidos para tela de volumes.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de pedidos.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterListaVolumes: function (filtro, pagina, numeroRegistros, ordenacao) {
      return http().get(API + 'volumes', {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
      });
    },

    /**
     * Recupera os detalhes de um pedido.
     * @param {!number} idPedido O identificador do pedido que será retornado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterDetalhe: function (idPedido) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      return http().get(API + idPedido);
    },

    /**
     * Remove um pedido.
     * @param {!number} idPedido O identificador do pedido que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idPedido) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      return http().delete(API + idPedido);
    },

    /**
     * Insere um pedido.
     * @param {!number} pedido Os dados do pedido que será inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (pedido) {
      return http().post(API.substr(0, API.length - 1), pedido);
    },

    /**
     * Altera os dados de um pedido.
     * @param {!number} idPedido O identificador do pedido que será usado para busca do produto.
     * @param {!Object} pedido O objeto com os dados do pedido a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idPedido, pedido) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      if (!pedido || pedido === {}) {
        return Promise.resolve();
      }

      return http().patch(API + idPedido, pedido);
    },

    /**
     * Reabre um pedido, se possível.
     * @param {!number} idPedido O identificador do pedido que será reaberto.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    reabrir: function (idPedido) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      return http().post(API + idPedido + '/reabrir');
    },

    /**
     * Finaliza um pedido, se possível.
     * @param {!number} idPedido O identificador do pedido que será finalizado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    finalizar: function (idPedido) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      return http().post(API + idPedido + '/finalizar');
    },

    /**
     * Finaliza um pedido, confirmando e gerando conferência, se possível.
     * @param {!number} idPedido O identificador do pedido que será finalizado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    confirmarGerandoConferencia: function (idPedido, finalizarConferencia) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      return http().post(API + idPedido + '/confirmarGerandoConferencia', {
        param: {
          finalizarConferencia: finalizarConferencia
        }
      });
    },

    /**
     * Envia pedido para validação no financeiro.
     * @param {!number} idPedido O identificador do pedido que será enviado para validação.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    enviarValidacaoFinanceiro: function (idPedido, mensagem) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      return http().post(API + idPedido + '/enviarValidacaoFinanceiro', {
        mensagem
      });
    },

    /**
     * Altera a liberação financeira de um pedido.
     * @param {!number} idPedido O identificador do pedido que será alterado.
     * @param {boolean} liberar Indica se o pedido será liberado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarLiberacaoFinanceira: function (idPedido, liberar) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      return http().post(API + item.id + '/alterarLiberacaoFinanceira', {
        liberar
      });
    },

    /**
     * Realiza a busca de observações do financeiro de um pedido.
     * @param {!number} idPedido O identificador do pedido que terá recuperadas as observações do financeiro.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterListaObservacoesFinanceiro: function (idPedido, pagina, numeroRegistros, ordenacao) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      var filtro = {
        pagina,
        numeroRegistros,
        ordenacao
      };

      return http().get(API +idPedido + '/observacoesFinanceiro', {
        params: filtro
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de pedidos.
     * @param {boolean} exibirFinanceiro Indica se os dados de financeiro devem ser exibidos na listagem.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function (exibirFinanceiro) {
      return http().get(API + 'configuracoes', {
        params: {
          exibirFinanceiro: exibirFinanceiro || false
        }
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de volumes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesListaVolumes: function () {
      return http().get(API + 'volumes/configuracoes');
    },

    /**
     * Recupera a lista de funcionários vendedores, incluindo o vendedor já selecionado no pedido (se estiver editanto).
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterVendedores: function (idVendedor) {
      return http().get(API + 'vendedores', {
        params: {
          idvendedor: idVendedor
        }
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de pedidos.
     * @param {number} idPedido O identificador do pedido que está sendo editado (se houver).
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesDetalhe: function (idPedido) {
      return http().get(API + (idPedido || 0) + '/configuracoes');
    },

    /**
     * Retorna os itens para o controle de situações de pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera a lista de situações de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoesProducao: function () {
      return http().get(API + 'situacoesProducao');
    },

    /**
     * Retorna os itens para o controle de tipos de pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPedido: function () {
      return http().get(API + 'tipos');
    },

    /**
     * Retorna os itens para o controle do filtro de tipos de venda.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposVenda: function () {
      return http().get(API + 'tiposVenda');
    },

    /**
     * Recupera os tipos de pedido para exibição no cadastro ou edição do pedido.
     * @param {boolean} pedidoMaoDeObra Indica se o pedido é de mão-de-obra.
     * @param {boolean} pedidoProducao Indica se o pedido é de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPedidoPorFuncionario: function (pedidoMaoDeObra, pedidoProducao) {
      return http().get(API + 'tiposPorFuncionario', {
        params: {
          maoDeObra: pedidoMaoDeObra || false,
          producao: pedidoProducao || false
        }
      });
    },

    /**
     * Recupera os tipos de venda do cliente para exibição no cadastro ou edição do pedido.
     * @param {Object} filtro Os dados para o filtro da pesquisa.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposVendaCliente: function (filtro) {
      return http().get(API + 'tiposVendaCliente', {
        params: filtro
      });
    },

    /**
     * Recupera as formas de pagamento para exibição no cadastro ou edição do pedido.
     * @param {Object} filtro Os dados para o filtro da pesquisa.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFormasPagamento: function (filtro) {
      return http().get(API + 'formasPagamento', {
        params: filtro
      });
    },

    /**
     * Recupera os tipos de entrega de pedido para exibição no cadastro ou edição do pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposEntrega: function () {
      return http().get(API + 'tiposEntrega');
    },

    /**
     * Calcula as datas de entrega do pedido (normal e 'fast delivery').
     * @param {number} idPedido O identificador do pedido para cálculo da data de entrega.
     * @param {number} idCliente O identificador do cliente do pedido.
     * @param {number} tipoPedido O tipo de pedido que está sendo calculado.
     * @param {number} tipoEntrega O tipo de entrega do pedido que está sendo calculado.
     * @param {!Date} dataBase A data base para cálculo da data de entrega.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterDatasEntrega: function (idPedido, idCliente, tipoPedido, tipoEntrega, dataBase) {
      return http().get(API + (idPedido || 0) + '/dataEntregaMinima', {
        params: {
          idCliente,
          tipoPedido,
          tipoEntrega,
          dataBase
        }
      });
    },

    /**
     * Retorna os itens para o controle de situações de pedido referente ao volume.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoesVolume: function () {
      return http().get(API + 'volumes/situacoes');
    },

    /**
     * Verifica se o 'fast delivery' pode ser marcado no pedido.
     * @param {number} idPedido O identificador do pedido que será verificado.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    verificarFastDelivery: function (idPedido) {
      if (!idPedido) {
        return Promise.resolve();
      }

      return http().get(API + idPedido + '/verificarFastDelivery');
    },

    /**
     * Realiza a validação do desconto aplicado na tela com os dados do pedido e dos produtos.
     * @param {number} idPedido O identificador do pedido que será usado para recuperação do desconto.
     * @param {number} descontoTela O valor do desconto aplicado na tela.
     * @param {number} tipoDescontoTela O tipo de desconto aplicado na tela.
     * @param {number} tipoVenda O tipo de venda do pedido.
     * @param {number} idFormaPagamento O identificador da forma de pagamento do pedido.
     * @param {number} idTipoCartao O identificador do tipo de cartão do pedido.
     * @param {number} idParcela O identificador da parcela do pedido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    validarDesconto: function (
      idPedido,
      descontoTela,
      tipoDescontoTela,
      tipoVenda,
      idFormaPagamento,
      idTipoCartao,
      idParcela
    ) {
      return http().get(API + (idPedido || 0) + '/validacaoDescontoPedido', {
        params: {
          descontoTela,
          tipoDescontoTela,
          tipoVenda,
          idFormaPagamento,
          idTipoCartao,
          idParcela
        }
      });
    },

    /**
     * Coloca o pedido atual em conferência.
     * @param {number} idPedido O identificador do pedido que terá a conferência gerada.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    colocarEmConferencia: function (idPedido) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }

      return http().post(API + idPedido + '/colocarEmConferencia');
    },

   /**
   * Altera a observação de um produto de pedido.
   * @param {number} idPedido O identificador do pedido que terá a conferência gerada.
   * @param {string} obs a nova observação do pedido.
   * @param {string} obsLiberacao nova observação de liberação do pedido.
   * @returns {Promise} Uma promise com o resultado da operação.
   */
    salvarObservacao: function (idPedido, obs, obsLiberacao) {
      if (!idPedido) {
        throw new Error('Pedido é obrigatório.');
      }
      return http().patch(API + idPedido + '/alterarObservacoes', {
        observacao: obs || '',
        observacaoLiberacao: obsLiberacao || ''
      });
    },

    /**
     * Retorna os itens para o controle de situações do pedido PCP.
     */
    obterSituacoesPedidoPcp: function () {
      return http().get(API + 'situacoesPcp');
    },

    /**
     * Retorna os itens para o controle de tipos do pedido PCP.
     */
    obterTiposPedidoPcp: function () {
      return http().get(API + 'tiposPcp');
    }
  };
})(function () {
  return Vue.prototype.$http;
});
