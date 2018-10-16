var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de produtos.
 */
Servicos.Produtos = (function(http) {
  const API = '/api/v1/produtos/';

  return {
    /**
     * Objeto com os serviços para a API de grupos de produtos.
     */
    Grupos: {
      /**
       * Recupera a lista de grupos de produto para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'grupos/filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de subgrupos de produtos.
     */
    Subgrupos: {
      /**
       * Recupera a lista de subgrupos de produto para uso no controle de busca.
       * @param {?number} [idGrupoProduto=null] O identificador do grupo de produto que irá filtrar os subgrupos.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function (idGrupoProduto) {
        return http().get(API + 'subgrupos/filtro', {
          params: {
            idGrupoProduto
          }
        });
      }
    },

    /**
     * Objeto com os serviços para a API de cores de vidro.
     */
    CoresVidro: {
      /**
       * Recupera a lista de cores de vidro para uso no controle de busca.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        filtro = filtro || {};
        filtro.pagina = pagina;
        filtro.numeroRegistros = numeroRegistros;
        filtro.ordenacao = ordenacao;

        return http().get(API + 'cores/vidro', {
          params: filtro
        });
      },

      /**
       * Remove uma cor de vidro.
       * @param {!number} idCorVidro O identificador da cor de vidro que será excluída.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idCorVidro) {
        if (!idCorVidro) {
          throw new Error('Cor de vidro é obrigatória.');
        }

        return http().delete(API + 'cores/vidro/' + idCorVidro);
      },

      /**
       * Insere uma cor de vidro.
       * @param {!Object} corVidro O objeto com os dados da cor de vidro a ser inserida.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (corVidro) {
        return http().post(API + 'cores/vidro', corVidro);
      },

      /**
       * Altera os dados de uma cor de vidro.
       * @param {!number} idCorVidro O identificador da cor de vidro que será alterada.
       * @param {!Object} corVidro O objeto com os dados da cor de vidro a serem alteradas.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idCorVidro, corVidro) {
        if (!idCorVidro) {
          throw new Error('Cor de vidro é obrigatória.');
        }

        if (!corVidro || corVidro === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'cores/vidro/' + idCorVidro, corVidro);
      },

      /**
       * Recupera a lista de cores de vidro para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'cores/vidro/filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de cores de ferragem.
     */
    CoresFerragem: {
      /**
       * Recupera a lista de cores de ferragem.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        filtro = filtro || {};
        filtro.pagina = pagina;
        filtro.numeroRegistros = numeroRegistros;
        filtro.ordenacao = ordenacao;

        return http().get(API + 'cores/ferragem', {
          params: filtro
        });
      },

      /**
       * Remove uma cor de ferragem.
       * @param {!number} idCorFerragem O identificador da cor de ferragem que será excluída.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idCorFerragem) {
        if (!idCorFerragem) {
          throw new Error('Cor de ferragem é obrigatória.');
        }

        return http().delete(API + 'cores/ferragem/' + idCorFerragem);
      },

      /**
       * Insere uma cor de ferragem.
       * @param {!Object} corFerragem O objeto com os dados da cor de ferragem a ser inserida.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (corFerragem) {
        return http().post(API + 'cores/ferragem', corFerragem);
      },

      /**
       * Altera os dados de uma cor de ferragem.
       * @param {!number} idCorFerragem O identificador da cor de ferragem que será alterada.
       * @param {!Object} corFerragem O objeto com os dados da cor de ferragem a serem alteradas.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idCorFerragem, corFerragem) {
        if (!idCorFerragem) {
          throw new Error('Cor de ferragem é obrigatória.');
        }

        if (!corFerragem || corFerragem === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'cores/ferragem/' + idCorFerragem, corFerragem);
      },

      /**
       * Recupera a lista de cores de ferragem para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'cores/ferragem/filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de cores de alumínio.
     */
    CoresAluminio: {
      /**
       * Recupera a lista de cores de alumínio.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        filtro = filtro || {};
        filtro.pagina = pagina;
        filtro.numeroRegistros = numeroRegistros;
        filtro.ordenacao = ordenacao;

        return http().get(API + 'cores/aluminio', {
          params: filtro
        });
      },

      /**
       * Remove uma cor de alumínio.
       * @param {!number} idCorAluminio O identificador da cor de alumínio que será excluída.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idCorAluminio) {
        if (!idCorAluminio) {
          throw new Error('Cor de alumínio é obrigatória.');
        }

        return http().delete(API + 'cores/aluminio/' + idCorAluminio);
      },

      /**
       * Insere uma cor de alumínio.
       * @param {!Object} corAluminio O objeto com os dados da cor de alumínio a ser inserida.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (corAluminio) {
        return http().post(API + 'cores/aluminio', corAluminio);
      },

      /**
       * Altera os dados de uma cor de alumínio.
       * @param {!number} idCorAluminio O identificador da cor de alumínio que será alterada.
       * @param {!Object} corAluminio O objeto com os dados da cor de alumínio a serem alteradas.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idCorAluminio, corAluminio) {
        if (!idCorAluminio) {
          throw new Error('Cor de alumínio é obrigatória.');
        }

        if (!corAluminio || corAluminio === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'cores/aluminio/' + idCorAluminio, corAluminio);
      },

      /**
       * Recupera a lista de cores de alumínio para uso no controle de busca.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        filtro = filtro || {};
        filtro.pagina = pagina;
        filtro.numeroRegistros = numeroRegistros;
        filtro.ordenacao = ordenacao;

        return http().get(API + 'cores/aluminio', {
          params: filtro
        });
      },

      /**
       * Remove uma cor de alumínio.
       * @param {!number} idCorAluminio O identificador da cor de alumínio que será excluída.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idCorAluminio) {
        if (!idCorAluminio) {
          throw new Error('Cor de alumínio é obrigatória.');
        }

        return http().delete(API + 'cores/aluminio/' + idCorAluminio);
      },

      /**
       * Insere uma cor de alumínio.
       * @param {!Object} corAluminio O objeto com os dados da cor de alumínio a ser inserida.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (corAluminio) {
        return http().post(API + 'cores/aluminio', corAluminio);
      },

      /**
       * Altera os dados de uma cor de alumínio.
       * @param {!number} idCorAluminio O identificador da cor de alumínio que será alterada.
       * @param {!Object} corAluminio O objeto com os dados da cor de alumínio a serem alteradas.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idCorAluminio, corAluminio) {
        if (!idCorAluminio) {
          throw new Error('Cor de alumínio é obrigatória.');
        }

        if (!corAluminio || corAluminio === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'cores/aluminio/' + idCorAluminio, corAluminio);
      },

      /**
       * Recupera a lista de cores de alumínio para uso no controle de busca.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'cores/aluminio/filtro');
      }
    },

    /*
     * Recupera a lista de produtos.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos produtos.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      filtro = filtro || {};
      filtro.pagina = pagina;
      filtro.numeroRegistros = numeroRegistros;
      filtro.ordenacao = ordenacao;

      return http().get(API.substr(0, API.length - 1), {
        params: filtro
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de produtos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Remove um produto.
     * @param {!number} idProduto O identificador do produto que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idProduto) {
      if (!idProduto) {
        throw new Error('Produto é obrigatório.');
      }

      return http().delete(API + idProduto);
    },

    /**
     * Recupera os produtos a partir do código interno.
     * @param {!string} codigoInterno O código interno do produto, para busca.
     * @param {?string} [tipoValidacao=null] O tipo de validação que será feita na busca.
     * @param {?string} [dadosAdicionaisValidacao=null] Os dados adicionais para validação, se houver.
     */
    obterParaControle: function (codigoInterno, tipoValidacao, dadosAdicionaisValidacao) {
      if (!codigoInterno) {
        return Promise.reject();
      }

      return http().get(API + 'filtro', {
        params: {
          codigoInterno,
          tipoValidacao,
          dadosAdicionaisValidacao
        }
      });
    },

    /**
     * Calcula a área, em m², para um produto.
     * @param {!Object} dadosProduto Os dados do produto que serão usados para o cálculo da área.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    calcularAreaM2: function (dadosProduto) {
      if (!dadosProduto) {
        throw new Error('Dados do produto são obrigatórios.');
      }

      return http().post(API + 'calcularAreaM2', dadosProduto);
    },

    /**
     * Calcula o valor total para um produto.
     * @param {!Object} dadosProduto Os dados do produto que serão usados para o cálculo do total.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    calcularTotal: function (dadosProduto) {
      if (!dadosProduto) {
        throw new Error('Dados do produto são obrigatórios.');
      }

      return http().post(API + 'calcularTotal', dadosProduto);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
