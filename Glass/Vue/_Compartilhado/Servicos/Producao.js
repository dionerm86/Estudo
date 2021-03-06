﻿var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produção.
 */
Servicos.Producao = (function(http) {
  const API = '/api/v1/producao/';
  const API_TipoPerda = API + 'tiposPerda/';
  const API_TipoPerda_SubtipoPerda = function (idTipoPerda) {
    const complemento = 'subtiposPerda/';

    return API_TipoPerda + idTipoPerda + '/' + complemento;
  };
  const API_Retalhos_Producao = API + 'retalhos/';

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
       * Recupera as configurações para a tela de setores.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API + 'setores/configuracoes');
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

        var posicao = {
          acima
        };

        return http().patch(API + 'setores/' + id + '/posicao', posicao);
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
       * @param {?boolean} [incluirSetorImpressao=false] Indica se o setor de impressão de etiquetas deve ser retornado.
       * @param {?boolean} [incluirEtiquetaNaoImpressa=false] Indica se deve ser retornado um setor de 'etiqueta não impressa'.
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
     * Objeto com os serviços para a API de retalhos.
     */
    Retalhos: {
      /**
         * Recupera a lista de retalhos de produção.
         * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
         * @param {number} pagina O número da página de resultados a ser exibida.
         * @param {number} numeroRegistros O número de registros que serão exibidos na página.
         * @param {string} ordenacao A ordenação para o resultado.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API_Retalhos_Producao, {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera a lista de situações dos retalhos de produção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterSituacoesParaFiltro: function () {
        return http().get(API_Retalhos_Producao + 'situacoes');
      }
    },

    /**
     * Objeto com os serviços para a API de roteiros.
     */
    Roteiros: {
      /**
       * Objeto com os serviços para a API de classificações de roteiro.
       */
      ClassificacoesRoteiro: {
        /**
         * Recupera a lista de classificações de roteiro.
         * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
         * @param {number} pagina O número da página de resultados a ser exibida.
         * @param {number} numeroRegistros O número de registros que serão exibidos na página.
         * @param {string} ordenacao A ordenação para o resultado.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obter: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API + 'roteiros/classificacoes', {
            params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
        },

        /**
         * Remove uma classificação de roteiro.
         * @param {!number} id O identificador da classificação de roteiro que será excluída.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        excluir: function (id) {
          if (!id) {
            throw new Error('Classificação de roteiro é obrigatória.');
          }

          return http().delete(API + 'roteiros/classificacoes/' + id);
        }
      },

      /**
       * Recupera a lista de roteiros.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'roteiros', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Remove um roteiro.
       * @param {!number} id O identificador do roteiro que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Roteiro é obrigatório.');
        }

        return http().delete(API + 'roteiros/' + id);
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
       * Objeto com os serviços para a API de subtipos de perda.
       */
      SubtiposPerda: {
        /**
         * Recupera uma lista de subtipos de perda para um tipo selecionado.
         * @param {!number} idTipoPerda O identificador do tipo de perda.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obterParaFiltro: function (idTipoPerda) {
          return http().get(API_TipoPerda_SubtipoPerda(idTipoPerda) + 'filtro');
        }
      },

      /**
       * Recupera a lista de tipos de perda.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API_TipoPerda.substr(0, API_TipoPerda.length - 1), {
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

        return http().delete(API_TipoPerda + id);
      },

      /**
       * Insere um tipo de perda.
       * @param {!Object} tipoPerda O objeto com os dados do tipo de perda a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (tipoPerda) {
        return http().post(API_TipoPerda.substr(0, API_TipoPerda.length - 1), tipoPerda);
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

        return http().patch(API_TipoPerda + id, tipoPerda);
      },

      /**
       * Recupera a lista de situações para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterSituacoes: function () {
        return http().get(API_TipoPerda + 'situacoes');
      },

      /**
       * Recupera uma lista de tipos de perda para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaFiltro: function () {
        return http().get(API_TipoPerda + 'filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de fornadas.
     */
    Fornadas: {
      /**
       * Recupera a lista de fornadas para a tela de listagem de fornadas.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de peças.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'fornadas', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Obtem a lista de peças de uma dada fornada.
       * @param {!number} idFornada O identificador da fornada.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterPecasFornada: function (idFornada) {
        if (!idFornada) {
          throw new Error('Identificador da fornada é obrigatório.');
        }

        return http().get(API + 'fornadas/' + idFornada + '/pecas');
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
     * Recupera a lista de setores de produção.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSetores: function () {
      return http().get("api/v1/producao/setores");
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
