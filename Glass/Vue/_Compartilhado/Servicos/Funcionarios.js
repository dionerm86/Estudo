var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de funcionários.
 */
Servicos.Funcionarios = (function(http) {
  const API = '/api/v1/funcionarios/';

  return {
    /**
     * Recupera a lista de funcionários vendedores.
     * @param {?number} [idVendedorAtual=null] O identificador do vendedor atual.
     * @param {?boolean} [orcamento=null] Considerar no resultado os emissores de orçamentos?
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterVendedores: function (idVendedorAtual, orcamento) {
      return http().get(API + 'vendedores', {
        params: {
          idVendedorAtual,
          orcamento
        }
      });
    },

    /**
     * Recupera a lista de funcionários compradores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterCompradores: function () {
      return http().get(API + 'compradores');
    },

    /**
     * Recupera a lista de funcionários medidores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterMedidores: function () {
      return http().get(API + 'medidores');
    },

    /**
     * Recupera a lista de funcionários conferentes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConferentes: function () {
      return http().get(API + 'conferentes');
    },

    /**
     * Recupera a lista de funcionários liberadores de pedido.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterLiberadores: function () {
      return http().get(API + 'liberadores');
    },

    /**
     * Recupera a lista de funcionários de caixa diário.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterCaixaDiario: function () {
      return http().get(API + 'caixaDiario');
    },

    /**
     * Recupera a lista de funcionários do financeiro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFinanceiros: function () {
      return http().get(API + 'financeiros');
    },

    /**
     * Recupera a lista de funcionários ativos associados à clientes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterAtivosAssociadosAClientes: function () {
      return http().get(API + 'ativosAssociadosAClientes');
    },

    /**
     * Recupera a lista de funcionários medidores.
     * @param {!number} idVendedor O identificador do vendedor que será feita a busca.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterDataTrabalho: function (idVendedor) {
      if (!idVendedor) {
        throw new Error('Vendedor é obrigatório.');
      }

      return http().get(API + idVendedor + '/dataTrabalho');
    },

    /**
     * Recupera a lista de funcionários que finalizaram pedidos.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFinalizacaoPedidos: function () {
      return http().get(API + 'finalizacao');
    },
    /**
     * Recupera a lista de funcionários.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de funcionários.
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
     * Recupera o objeto com as configurações utilizadas na tela de listagem de funcionários.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesLista: function () {
      return http().get(API + 'configuracoes');
    },

    /**
     * Retorna os itens para o controle de tipos fiscal de cliente.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return Promise.resolve({
        "data": [
          {
            "id": "1",
            "nome": "Ativo"
          },
          {
            "id": "2",
            "nome": "Inativo"
          }
        ]
      });
    },

    /**
     * Retorna os itens para o controle de tipos fiscal de cliente.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTiposFuncionario: function () {
      return http().get(API + 'tiposFuncionario');
    },
        
    /**
     * Remove um cliente.
     * @param {!number} idFuncionario O identificador do funcionário que será excluído.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    excluir: function (idFuncionario) {
      if (!idFuncionario) {
        throw new Error('Funcionário é obrigatório.');
      }

      return http().delete(API + idFuncionario);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
