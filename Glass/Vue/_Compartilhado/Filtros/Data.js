/**
 * Filtro para formatação de datas (dd/MM/yyyy).
 * @param {date} valor A data a ser formatada.
 * @returns A data formatada, se possível. Senão, retorna um texto vazio.
 */
Vue.filter('data', function (valor) {
  var data = valor
    ? new Date(valor).toLocaleDateString('pt-BR')
    : null;

  return data && data.toString() !== 'Invalid Date'
    ? data
    : '';
});
