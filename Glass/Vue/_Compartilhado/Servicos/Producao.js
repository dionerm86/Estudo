var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produção.
 */
Servicos.Producao = (function(http) {
  const API = '/api/v1/producao/';

  return {
    /**
     * Objeto com os serviços para a API de setores de produção.
     */
    Setores: {
      /**
       * Recupera a lista de setores.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'setores', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Remove um setor.
       * @param {!number} id O identificador do setor que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Setor é obrigatório.');
        }

        return http().delete(API + 'setores/' + id);
      },

      /**
       * Insere um setor.
       * @param {!Object} setor O objeto com os dados do setor a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (setor) {
        return http().post(API + 'setores', setor);
      },

      /**
       * Altera os dados de um setor.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!Object} setor O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (id, setor) {
        if (!id) {
          throw new Error('Setor é obrigatório.');
        }

        if (!setor || setor === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'setores/' + id, setor);
      },

      /**
       * Altera a posição de um setor.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!boolean} acima Define se o setor será movimentado para cima.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      alterarPosicao: function (id, acima) {
        if (!id) {
          throw new Error('Setor é obrigatório.');
        }

        return http().patch(API + 'setores/' + id, {
            acima: acima || false
          });
      },

      /**
       * Recupera a lista de tipos de setor para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterTipos: function () {
        return http().get(API + 'setores/tipos');
      },

      /**
       * Recupera a lista de cores de setor para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterCores: function () {
        return http().get(API + 'setores/cores');
      },

      /**
       * Recupera a lista de cores de tela de setor para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterCoresTela: function () {
        return http().get(API + 'setores/coresTela');
      },

      /**
       * Recupera a lista de situações setor para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterSituacoes: function () {
        return http().get(API + 'setores/situacoes');
      },

      /**
       * Recupera a lista de setores de produção para uso no controle de busca.
       * @param {?boolean} [incluirSetorImpressao=null] Indica se o setor de impressão de etiquetas deve ser retornado.
       * @param {?boolean} [incluirEtiquetaNaoImpressa=null] Indica se deve ser retornado um setor de 'etiqueta não impressa'.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function (incluirSetorImpressao, incluirEtiquetaNaoImpressa) {
        return http().get(API + 'setores/filtro', {
          params: {
            incluirSetorImpressao: incluirSetorImpressao || false,
            incluirEtiquetaNaoImpressa: incluirEtiquetaNaoImpressa || false
          }
        });
      }
    },

    /**
     * Objeto com os serviços para a API de turnos.
     */
    Turnos: {
      /**
       * Recupera a lista de turnos.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'turnos', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Remove um turno.
       * @param {!number} id O identificador do turno que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Turno é obrigatório.');
        }

        return http().delete(API + 'turnos/' + id);
      },

      /**
       * Insere um turno.
       * @param {!Object} turno O objeto com os dados do turno a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (turno) {
        return http().post(API + 'turnos', turno);
      },

      /**
       * Altera os dados de um turno.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!Object} turno O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (id, turno) {
        if (!id) {
          throw new Error('Turno é obrigatório.');
        }

        if (!turno || turno === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'turnos/' + id, turno);
      },

      /**
       * Recupera a lista de sequências de turno para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterSequencias: function () {
        return http().get(API + 'turnos/sequencias');
      }
    },

    /**
     * Objeto com os serviços para a API de tipos de perda.
     */
    TiposPerda: {
      /**
       * Recupera a lista de tipos de perda.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'tiposPerda', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Remove um tipos de perda.
       * @param {!number} id O identificador do tipos de perda que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Tipo de perda é obrigatório.');
        }

        return http().delete(API + 'tiposPerda/' + id);
      },

      /**
       * Insere um tipo de perda.
       * @param {!Object} tipoPerda O objeto com os dados do tipo de perda a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (tipoPerda) {
        return http().post(API + 'tiposPerda', tipoPerda);
      },

      /**
       * Altera os dados de um tipo de perda.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!Object} tipoPerda O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (id, tipoPerda) {
        if (!id) {
          throw new Error('Tipo de perda é obrigatório.');
        }

        if (!tipoPerda || tipoPerda === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'tiposPerda/' + id, tipoPerda);
      },

      /**
       * Recupera a lista de situações para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterSituacoes: function () {
        return http().get(API + 'tiposPerda/situacoes');
      }
    },

    /**
     * Recupera a lista de peças para a tela de consulta de produção.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de peças.
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
     * Recupera a contagem de peças por situação para a tela de consulta de produção.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a contagem de peças.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterContagemPecas: function (filtro) {
      return http().get(API + 'contagemPecas', {
        params: filtro
      });
    },

    /**
     * Recupera a lista de produtos de composição para a tela de consulta de produção.
     * @param {!number} id O identificador da peça 'pai' para a busca dos produtos de composição.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterProdutosComposicao: function (id, pagina, numeroRegistros, ordenacao) {
      if (!id) {
        throw new Error('Produto pai é obrigatório.');
      }

      var filtro =  {
        pagina,
        numeroRegistros,
        ordenacao
      };

      return http().get(API + id + '/composicao', {
        params: filtro
      });
    },

    /**
     * Recupera as configurações para a tela de consulta de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesConsulta: function () {
      return http().get(API + 'configuracoes/consulta');
    },

    /**
     * Recupera a lista de situações de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Recupera a lista de tipos de situações de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposSituacoes: function () {
      return http().get(API + 'tiposSituacoes');
    },

    /**
     * Recupera a lista de tipos de pedidos para o filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPedido: function () {
      return http().get(API + 'tiposPedidos');
    },

    /**
     * Recupera a lista de tipos de peças que poderão ser exibidas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposPecasExibir: function () {
      return http().get(API + 'tiposPecasExibir');
    },

    /**
     * Recupera a lista de tipos de produtos de composição.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposProdutosComposicao: function () {
      return http().get(API + 'tiposProdutosComposicao');
    },

    /**
     * Recupera a lista de tipos de 'fast delivery'.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposFastDelivery: function () {
      return http().get(API + 'tiposFastDelivery');
    },

    /**
     * Desfaz a última leitura de produção da peça informada.
     * @param {!number} id O identificador da peça que terá a leitura desfeita.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    desfazerUltimaLeituraPeca: function (id) {
      if (!id) {
        throw new Error('Produto pai é obrigatório.');
      }

      return http().delete(API + id + '/leituras');
    }
  };
})(function() {
  return Vue.prototype.$http;
});
