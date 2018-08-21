/**
 * Filtro para formatação de datas e horas (dd/MM/yyyy HH:mm:sss).
 * @param {date} valor A data a ser formatada.
 * @returns A data formatada, se possível. Senão, retorna um texto vazio.
 */
Vue.filter('dataHora', function (valor) {
  var data = valor
    ? new Date(valor).toLocaleString('pt-BR')
    : null;

  return data && data.toString() !== 'Invalid Date'
    ? data
    : '';
});
