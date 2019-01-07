﻿var Servicos = Servicos || {};

/**
 * Objeto com os serviços para a API de proprietários de veículos.
 */
Servicos.ConhecimentosTransporte = (function (http) {
  const API = '/api/v1/conhecimentosTransporte/';
  const API_Veiculos = API + 'veiculos/';
  const API_Proprietarios = API_Veiculos + 'proprietarios/';

  return {
    /**
     *Objeto com os serviços para a API de veículos.
     */
    Veiculos: {
      /**
       *Objeto com os serviços para a API de proprietários.
       */
      Proprietarios: {
        /**
         * Recupera a lista de proprietários de veículo.
         * @param {?Object} filtro Objeto com os filtros a serem usados para a busca dos itens.
         * @param {number} pagina O número da página de resultados a ser exibida.
         * @param {number} numeroRegistros O número de registros que serão exibidos na página.
         * @param {string} ordenacao A ordenação para o resultado.
         * @returns {Promise} Uma promise com o resultado da busca.
         */
        obter: function (filtro, pagina, numeroRegistros, ordenacao) {
          return http().get(API_Proprietarios, {
            params: Servicos.criarFiltroPaginado(filtro, pagina, numeroRegistros, ordenacao)
          });
        },

        /**
         * Exclui um proprietário de veículo.
         * @returns {Promise} Uma promise com o resultado da busca.
         */
        excluir: function (id) {
          if (!id) {
            throw new Error("Proprietário de veiculo é obrigatório.")
          }

          return http().delete(API_Proprietarios + id);
        }
      }
    }
  };
})(function () {
  return Vue.prototype.$http;
});
