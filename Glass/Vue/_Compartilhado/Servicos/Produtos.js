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
       * Recupera a lista de grupos de produto.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'grupos', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera o objeto com as configurações utilizadas na tela de listagem de grupos de produto.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API + 'grupos/configuracoes');
      },

      /**
       * Remove um grupo de produto.
       * @param {!number} idGrupoProduto O identificador do grupo de produto que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idGrupoProduto) {
        if (!idGrupoProduto) {
          throw new Error('Grupo de produto é obrigatório.');
        }

        return http().delete(API + 'grupos/' + idGrupoProduto);
      },

      /**
       * Insere um grupo de produto.
       * @param {!Object} grupoProduto O objeto com os dados do grupo de produto a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (grupoProduto) {
        return http().post(API + 'grupos', grupoProduto);
      },

      /**
       * Altera os dados de um grupo de produto.
       * @param {!number} idGrupoProduto O identificador do item que será alterado.
       * @param {!Object} grupoProduto O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idGrupoProduto, grupoProduto) {
        if (!idGrupoProduto) {
          throw new Error('Grupo de produto é obrigatório.');
        }

        if (!grupoProduto || grupoProduto === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'grupos/' + idGrupoProduto, grupoProduto);
      },

      /**
       * Recupera a lista de tipos de grupo de produto para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterTipos: function () {
        return http().get(API + 'grupos/tipos');
      },

      /**
       * Recupera a lista de tipos de cálculo para uso no controle de seleção.
       * @param {!boolean} notaFiscal Define se serão buscados tipos de cálculo de nota fiscal, caso false, busca de pedido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterTiposCalculo: function (notaFiscal) {
        if (notaFiscal === undefined || notaFiscal === null) {
          throw new Error('Forma de cálculo é obrigatória.');
        }

        return http().get(API + 'grupos/tiposCalculo', {
            params: {
              notaFiscal: notaFiscal
            }
          });
      },

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
       * Recupera a lista de subgrupos de produto.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'subgrupos', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera o objeto com as configurações utilizadas na tela de listagem de subgrupos de produto.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API + 'subgrupos/configuracoes');
      },

      /**
       * Remove um subgrupo de produto.
       * @param {!number} idSubgrupoProduto O identificador do subgrupo de produto que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idSubgrupoProduto) {
        if (!idSubgrupoProduto) {
          throw new Error('Subgrupo de produto é obrigatório.');
        }

        return http().delete(API + 'subgrupos/' + idSubgrupoProduto);
      },

      /**
       * Insere um grupo de produto.
       * @param {!Object} subgrupoProduto O objeto com os dados do subgrupo de produto a ser inserida.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (subgrupoProduto) {
        return http().post(API + 'subgrupos', subgrupoProduto);
      },

      /**
       * Altera os dados de um subgrupo de produto.
       * @param {!number} idSubgrupoProduto O identificador do item que será alterado.
       * @param {!Object} subgrupoProduto O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idSubgrupoProduto, subgrupoProduto) {
        if (!idSubgrupoProduto) {
          throw new Error('Subgrupo de produto é obrigatório.');
        }

        if (!subgrupoProduto || subgrupoProduto === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'subgrupos/' + idSubgrupoProduto, subgrupoProduto);
      },

      /**
       * Recupera a lista de tipos de subgrupo de produto para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterTipos: function () {
        return http().get(API + 'subgrupos/tipos');
      },

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
      },

      /**
       * Recupera a lista de subgrupos de produto para uso no controle de busca.
       * @param {string} [idGrupoProduto=null] O identificador do grupo de produto que irá filtrar os subgrupos.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterVariosParaControle: function (idsGruposProduto) {
        if (!idsGruposProduto || !idsGruposProduto.length) {
          return Promise.reject();
        }

        return http().get(API + 'subgrupos/filtro/varios', {
          params: {
            idsGruposProduto
          }
        });
      }
    },

    /**
     * Objeto com os serviços para a API de matéria prima.
     */
    MateriaPrima: {
      /**
       * Objeto com os serviços para a API de extrato de matéria prima.
       */
      Extrato: {
        /**
         * Objeto com os serviços para a API de totalizadores para o extrato de movimentação de chapas de vidro.
         */
        Totalizadores: {
          /**
           * Recupera a lista de totalizadores para a tela de listagem de extrato de movimentações de chapas de vidro.
           * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
           * @param {number} pagina O número da página de resultados a ser exibida.
           * @param {number} numeroRegistros O número de registros que serão exibidos na página.
           * @param {string} ordenacao A ordenação para o resultado.
           * @returns {Promise} Uma promise com o resultado da operação.
           */
          obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
            return http().get(API + 'materiaPrima/extrato/totalizadores', {
              params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
            });
          }
        }
      }
    },

    PrecosTabelaCliente: {
      /**
       * Recupera a lista de preços de tabela por cliente.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'precosTabelaCliente', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera o objeto com as configurações utilizadas na tela de listagem de preços de tabela por cliente.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API + 'precosTabelaCliente/configuracoes');
      },

      /**
       * Recupera uma lista com os tipos de valor de tabela para uso no controle.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterTiposValorTabela: function () {
        return http().get(API + 'precosTabelaCliente/tiposValorTabela');
      }
    },

    /**
     * Objeto com os serviços para a API de chapas de vidro.
     */
    ChapasVidro: {
      /**
       * Objeto com os serviços para a API de perdas de chapas de vidro.
       */
      Perdas: {
        /**
         * Recupera a lista de perdas de chapas de vidro.
         * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
         * @param {number} pagina O número da página de resultados a ser exibida.
         * @param {number} numeroRegistros O número de registros que serão exibidos na página.
         * @param {string} ordenacao A ordenação para o resultado.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API + 'chapasVidro/perdas', {
            params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
        },

        /**
         * Cancela uma perda de chapa de vidro.
         * @param {!number} idPerdaChapaVidro.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        cancelar: function (idPerdaChapaVidro) {
          if (!idPerdaChapaVidro) {
            throw new Error('Perda de chapa de vidro é obrigatória.');
          }

          return http().delete(API + 'chapasVidro/perdas/' + idPerdaChapaVidro);
        },
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
        return http().get(API + 'cores/vidro', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
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
        return http().get(API + 'cores/ferragem', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
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
        return http().get(API + 'cores/aluminio', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
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
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
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
     * Altera a situação dos produtos pelo grupo de produto.
     * @param {!number} idGrupoProduto O identificador do grupo de produto.
     * @param {!number} situacao A situação que será alterada nos produtos.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarSituacaoPorGrupoProduto: function (idGrupoProduto, situacao) {
      if (!idGrupoProduto) {
        throw new Error('Grupo de produto é obrigatório.');
      }

      return http().patch(API + 'alterarSituacao/grupo/' + idGrupoProduto, {
        situacao: situacao
      });
    },

    /**
     * Altera a situação dos produtos pelo subgrupo de produto.
     * @param {!number} idSubgrupoProduto O identificador do subgrupo de produto.
     * @param {!number} situacao A situação que será alterada nos produtos.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarSituacaoPorSubgrupoProduto: function (idSubgrupoProduto, situacao) {
      if (!idSubgrupoProduto) {
        throw new Error('Subgrupo de produto é obrigatório.');
      }

      return http().patch(API + 'alterarSituacao/subgrupo/' + idSubgrupoProduto, {
        situacao: situacao
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
