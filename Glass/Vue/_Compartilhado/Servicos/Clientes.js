var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de clientes.
 */
Servicos.Clientes = (function(http) {
  const API = '/api/v1/clientes/';
  const API_TIPOS = '/api/v1/tiposCliente/';

  return {
    /**
     * Objeto com os serviços para a API de tipos de cliente.
     */
    Tipos: {
      /**
       * Recupera a lista de tipos de cliente para uso no controle de busca.
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

        return http().get(API_TIPOS, {
          params: filtro
        });
      },

      /**
       * Remove um tipo de cliente.
       * @param {!number} idTipoCliente O identificador do tipo de cliente que será excluído.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      excluir: function (idTipoCliente) {
        if (!idTipoCliente) {
          throw new Error('Tipo de cliente é obrigatório.');
        }

        return http().delete(API_TIPOS + idTipoCliente);
      },

      /**
       * Insere um tipo de cliente.
       * @param {!Object} tipoCliente O objeto com os dados do tipo de cliente a ser inserido.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      inserir: function (tipoCliente) {
        return http().post(API_TIPOS, tipoCliente);
      },

      /**
       * Altera os dados de um tipo de cliente.
       * @param {!number} idTipoCliente O identificador do tipo de cliente que será alterado.
       * @param {!Object} tipoCliente O objeto com os dados do tipo de cliente a serem alteradas.
       * @returns {Promise} Uma promise com o resultado da operação.
       */
      atualizar: function (idTipoCliente, tipoCliente) {
        if (!idTipoCliente) {
          throw new Error('Tipo de cliente é obrigatório.');
        }

        if (!tipoCliente || tipoCliente === {}) {
          return Promise.resolve();
        }

        return http().patch(API_TIPOS + idTipoCliente, tipoCliente);
      },

      /**
       * Retorna os itens para o controle de tipos de cliente.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterParaControle: function () {
        return http().get(API_TIPOS + 'filtro');
      },

      /**
       * Retorna os itens para o controle de tipos fiscal de cliente.
       * @returns {Promise} Uma promise com o resultado da busca.
       */
      obterParaControleFiscal: function () {
        return http().get(API_TIPOS + 'fiscal/filtro');
      }
    },

    /**
     * Recupera a lista de clientes.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de clientes.
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
     * Recupera a lista de clientes para uso no controle de busca.
     * @param {?number} [idCliente=null] O identificador do cliente para filtro na busca.
     * @param {?string} [nomeCliente=null] O nome do cliente para filtro na busca.
     * @param {?string} [tipoValidacao=null] O tipo de validação que será feita na busca de clientes.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    obterParaControle: function (idCliente, nomeCliente, tipoValidacao) {
      return http().get(API + 'filtro', {
        params: {
          id: idCliente,
          nome: nomeCliente,
          tipoValidacao
        }
      });
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de clientes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Retorna os itens para o controle de situações de cliente.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return http().get(API + 'situacoes');
    },

    /**
     * Remove um cliente.
     * @param {!number} idCliente O identificador do cliente que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idCliente) {
      if (!idCliente) {
        throw new Error('Cliente é obrigatório.');
      }

      return http().delete(API + idCliente);
    },

    /**
     * Altera a situação do cliente.
     * @param {!number} idCliente O identificador do cliente que será alterada a situação.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarSituacao: function (idCliente) {
      if (!idCliente) {
        throw new Error('Cliente é obrigatório.');
      }

      return http().post(API + idCliente + '/alterarSituacao');
    },

    /**
     * Ativa os clientes com base nos filtros passados.
     * @param {?Object} filtro Objeto com os filtros a serem usados para definir os clientes que serão ativados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    ativar: function (filtro) {
      filtro = filtro || {};

      return http().post(API + 'ativar', filtro);
    },

    /**
     * Altera o vendedor dos clientes com base nos filtros passados.
     * @param {?Object} filtro Objeto com os filtros a serem usados para definir os clientes que serão alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarVendedor: function (filtro, idVendedorNovo) {
      filtro = filtro || {};
      filtro.idVendedorNovo = idVendedorNovo;

      return http().post(API + 'alterarVendedor', filtro);
    },

    /**
     * Altera a rota dos clientes com base nos filtros passados.
     * @param {?Object} filtro Objeto com os filtros a serem usados para definir os clientes que serão alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    alterarRota: function (filtro, idRotaNova) {
      filtro = filtro || {};
      filtro.idRotaNova = idRotaNova;

      return http().post(API + 'alterarRota', filtro);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
