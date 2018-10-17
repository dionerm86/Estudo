var Servicos = Servicos || {};

/**
 * Cria o objeto para envio à API com os dados preenchidos sem que o filtro original seja alterado.
 * @param {?Object} filtro Objeto com os filtros a serem usados para a busca.
 * @param {number} pagina O número da página de resultados a ser exibida.
 * @param {number} numeroRegistros O número de registros que serão exibidos na página.
 * @param {string} ordenacao A ordenação para o resultado.
 * @returns {Promise} Uma promise com o resultado da busca.
 */
Servicos.criarFiltroPaginado = function (filtro, pagina, numeroRegistros, ordenacao) {
  const cloneFiltro = Mixins.Objetos.methods.clonar(filtro || {});
  return Mixins.Objetos.methods.merge(cloneFiltro, {
    pagina,
    numeroRegistros,
    ordenacao
  });
};
