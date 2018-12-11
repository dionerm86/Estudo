var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de projetos.
 */
Servicos.Projetos = (function(http) {
  const API = '/api/v1/projetos/';

  return {
    /**
     * Objeto com os serviços para a API de modelos de projeto.
     */
    Modelos: {
      /**
       * Recupera a lista de modelos de projeto.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'modelos', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera a lista de situações de modelo de projeto para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterSituacoes: function () {
        return http().get(API + 'modelos/situacoes');
      }
    },

    /**
     * Objeto com os serviços para a API de grupos de projeto.
     */
    Grupos: {
      /**
       * Recupera a lista de grupos de projeto.
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
       * Remove um grupo de projeto.
       * @param {!number} id O identificador do grupo de projeto que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Grupo de projeto é obrigatório.');
        }

        return http().delete(API + 'grupos/' + id);
      },

      /**
       * Insere um grupo de projeto.
       * @param {!Object} grupoProjeto O objeto com os dados do grupo de projeto a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (grupoProjeto) {
        return http().post(API + 'grupos', grupoProjeto);
      },

      /**
       * Altera os dados de um grupo de projeto.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!Object} grupoProjeto O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (id, grupoProjeto) {
        if (!id) {
          throw new Error('Grupo de projeto é obrigatório.');
        }

        if (!grupoProjeto || grupoProjeto === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'grupos/' + id, grupoProjeto);
      },

      /**
       * Recupera a lista de grupos de projeto para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'grupos/filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de medidas de projeto.
     */
    Medidas: {
      /**
       * Objeto com os serviços para a API de grupos de medida de projeto.
       */
      Grupos: {
        /**
         * Recupera a lista de grupos de medidas de projeto para uso no controle de seleção.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obterParaControle: function () {
          return http().get(API + 'medidas/grupos/filtro');
        },

        /**
         * Recupera a lista de grupos de medidas de projeto.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obter: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API + 'medidas/grupos', {
            params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
        },

        /**
         * Exclui um grupo de medida de projeto.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        excluir: function (id) {
          return http().delete(API + 'medidas/grupos/' + id);
        },

        /**
         * Insere um grupo de medida de projeto.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        inserir: function (grupoMedidaProjeto) {
          return http().post(API + 'medidas/grupos/', grupoMedidaProjeto);
        },

        /**
         * Atualiza um grupo de medida de projeto indicado.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        atualizar: function (id, grupoMedidaProjeto) {
          return http().patch(API + 'medidas/grupos/' + id, grupoMedidaProjeto);
        }
      },

      /**
       * Recupera a lista de medidas de projeto.
       * @param {Object} filtro O filtro que foi informado na tela de pesquisa.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obter: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'medidas', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Remove uma medida de projeto.
       * @param {!number} id O identificador da medida de projeto que será excluída.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Medida de projeto é obrigatória.');
        }

        return http().delete(API + 'medidas/' + id);
      },

      /**
       * Insere uma medida de projeto.
       * @param {!Object} medidaProjeto O objeto com os dados da medida de projeto a ser inserida.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (medidaProjeto) {
        return http().post(API + 'medidas', medidaProjeto);
      },

      /**
       * Altera os dados de uma medida de projeto.
       * @param {!number} id O identificador do item que será alterado.
       * @param {!Object} medida O objeto com os dados do item a serem alterados.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (id, medidaProjeto) {
        if (!id) {
          throw new Error('Medida de projeto é obrigatória.');
        }

        if (!medidaProjeto || medidaProjeto === {}) {
          return Promise.resolve();
        }

        return http().patch(API + 'medidas/' + id, medidaProjeto);
      },

      /**
       * Recupera a lista de medidas de projeto para uso no controle de seleção.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      obterParaControle: function () {
        return http().get(API + 'medidas/filtro');
      }
    },

    /**
     * Objeto com os serviços para a API de ferragens.
     */
    Ferragens: {
      /**
       * Objeto com os serviços para a API de fabricantes de ferragens.
       */
      Fabricantes: {
        /**
         * Recupera a lista de fabricantes de ferragens.
         * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
         * @param {number} pagina O número da página de resultados a ser exibida.
         * @param {number} numeroRegistros O número de registros que serão exibidos na página.
         * @param {string} ordenacao A ordenação para o resultado.
         * @returns {Promise} Uma promise com o resultado da busca.
         */
        obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API + 'ferragens/fabricantes', {
            params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
        },

        /**
         * Remove um fabricante de ferragem.
         * @param {!number} id O identificador do fabricante de ferragem que será excluído.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        excluir: function (id) {
          if (!id) {
            throw new Error('Fabricante de ferragem é obrigatório.');
          }

          return http().delete(API + 'ferragens/fabricantes/' + id);
        },

        /**
         * Insere um fabricante de ferragem.
         * @param {!Object} fabricanteFerragem O objeto com os dados do item a ser inserido.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        inserir: function (fabricanteFerragem) {
          return http().post(API + 'ferragens/fabricantes', fabricanteFerragem);
        },

        /**
         * Altera os dados de um fabricante de ferragem.
         * @param {!number} id O identificador do item que será alterado.
         * @param {!Object} medida O objeto com os dados do item a serem alterados.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        atualizar: function (id, fabricanteFerragem) {
          if (!id) {
            throw new Error('Fabricante de ferragem é obrigatório.');
          }

          if (!fabricanteFerragem || fabricanteFerragem === {}) {
            return Promise.resolve();
          }

          return http().patch(API + 'ferragens/fabricantes/' + id, fabricanteFerragem);
        },

        /**
         * Recupera a lista de grupos de medidas de projeto para uso no controle de seleção.
         * @returns {Promise} Uma promise com o resultado da operação.
         */
        obterParaControle: function () {
          return http().get(API + 'ferragens/fabricantes/filtro');
        }
      },

      /**
       * Recupera a lista de ferragens.
       * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
       * @param {number} pagina O número da página de resultados a ser exibida.
       * @param {number} numeroRegistros O número de registros que serão exibidos na página.
       * @param {string} ordenacao A ordenação para o resultado.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
        return http().get(API + 'ferragens', {
          params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
        });
      },

      /**
       * Recupera as configurações para a tela de ferragens.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterConfiguracoesLista: function () {
        return http().get(API + 'ferragens/configuracoes');
      },

      /**
       * Remove uma ferragem.
       * @param {!number} id O identificador da ferragem que será excluída.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (id) {
        if (!id) {
          throw new Error('Ferragem é obrigatória.');
        }

        return http().delete(API + 'ferragens/' + id);
      },

      /**
       * Altera a situação da ferragem.
       * @param {!number} id O identificador do item que será alterado.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      alterarSituacao: function (id) {
        if (!id) {
          throw new Error('Ferragem é obrigatória.');
        }

        return http().patch(API + 'ferragens/' + id + '/situacao');
      }
    },

    /**
     * Recupera a lista de projetos.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
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
     * Remove um projeto.
     * @param {!number} idProjeto O identificador do projeto que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idProjeto) {
      if (!idProjeto) {
        throw new Error('Projeto é obrigatório.');
      }

      return http().delete(API + idProjeto);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
