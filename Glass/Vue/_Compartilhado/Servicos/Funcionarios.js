var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de funcionários.
 */
Servicos.Funcionarios = (function(http) {
  const API = '/api/v1/funcionarios/';

  return {
    /**
     * Busca os funcionários de um tipo específico para o controle de filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterParaControle: function (tipo) {
      if (tipo === 'Vendedores') {
        return this.obterVendedores();
      }

      return Promise.reject();
    },

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
     * Recupera a lista de funcionários motoristas.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterMotoristas: function () {
      return http().get(API + 'motoristas');
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
     * Recupera a lista de funcionários associados à sugestões de clientes.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSugestoesCliente: function () {
      return http().get(API + 'sugestoesCliente');
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
      return http().get(API.substr(0, API.length - 1), {
        params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
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
    obterTiposFuncionario: function () {
      return http().get(API + 'tiposFuncionario');
    },

    /**
     * Recupera os detalhes de um pedido.
     * @param {!number} idPedido O identificador do pedido que será retornado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterFuncionario: function (idFuncionario) {
      if (!idFuncionario) {
        throw new Error('Funcionário é obrigatório.');
      }

      return http().get(API + idFuncionario);
    },

    /**
     * Recupera o objeto com as configurações utilizadas na tela de listagem de pedidos.
     * @param {number} idPedido O identificador do pedido que está sendo editado (se houver).
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterConfiguracoesDetalhe: function (idFuncionario) {
      return http().get(API + (idFuncionario || 0) + '/configuracoes');
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
    },

    /**
     * Insere um pedido.
     * @param {!number} pedido Os dados do pedido que será inserido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    inserir: function (funcionario) {
      return http().post(API.substr(0, API.length - 1), funcionario);
    },

    /**
     * Altera os dados de um pedido.
     * @param {!number} idPedido O identificador do pedido que será usado para busca do produto.
     * @param {!Object} pedido O objeto com os dados do pedido a serem alterados.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    atualizar: function (idFuncionario, funcionario) {
      if (!idFuncionario) {
        throw new Error('Funcionário é obrigatório.');
      }

      if (!funcionario || funcionario === {}) {
        return Promise.resolve();
      }

      return http().patch(API + idFuncionario, funcionario);
    }
  };
})(function() {
  return Vue.prototype.$http;
});
