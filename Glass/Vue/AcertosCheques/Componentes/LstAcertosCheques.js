const app = new Vue({
    el: '#app',
    mixins: [Mixins.Objetos, Mixins.OrdenacaoLista('idAcertoCheque', 'desc')],
    data: {
        filtro: {
            buscarAcertosChequesProprios,
            buscarAcertosCaixaDiario
        }
    },

    methods: {
        /**
         * Busca os acertos de cheques para exibição na lista.
         * @param {!Object} filtro O filtro utilizado para a busca dos itens.
         * @param {!number} pagina O número da página que será exibida na lista.
         * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
         * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
         * @returns {Promise} Uma promise com o resultado da busca de acertos.
         */
        obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
            var filtroUsar = this.clonar(filtro || {});
            return Servicos.AcertosCheques.obterListaAcertosCheques(filtroUsar, pagina, numeroRegistros, ordenacao);
        },

        /**
         * Cancela um acerto de cheque.
         * @param {Object} item O acerto de cheque que será cancelado.
         * @returns {Promise} Uma Promise com o resultado da busca.
         */
        cancelar: function (item) {
            this.abrirJanela(200, 500, '../Utils/SetMotivoCancReceb.aspx?tipo=acertoCheque&id=' + item.id);
        },

        /**
         * Exibe os dados detalhados do acerto de cheques.
         * @param {Object} item O acerto de cheque que será exibido.
         */
        abrirRelatorio: function (id) {
            var url = '../Relatorios/RelBase.aspx?rel=AcertoCheque&idAcertoCheque=' + id;
            this.abrirJanela(600, 800, url);
        },        

        /**
         * Formata os filtros para utilização na url.
         */
        formatarFiltros_: function () {
            var filtros = [];

            this.incluirFiltroComLista(filtros, 'idAcertoCheque', this.filtro.id);
            this.incluirFiltroComLista(filtros, 'idFunc', this.idFuncionario);
            this.incluirFiltroComLista(filtros, 'idCliente', this.idCliente);
            this.incluirFiltroComLista(filtros, 'nomeCliente', this.filtro.nomeCliente);
            this.incluirFiltroComLista(filtros, 'dataIni', this.filtro.periodoCadastroInicio);
            this.incluirFiltroComLista(filtros, 'dataFim', this.filtro.periodoCadastroFim);
            this.incluirFiltroComLista(filtros, 'chequesProprios', this.filtro.buscarAcertosChequesProprios);
            this.incluirFiltroComLista(filtros, 'chequesCaixaDiario', this.filtro.buscarAcertosCaixaDiario);

            return filtros.length
              ? '&' + filtros.join('&')
              : '';
        },

        /**
         * Exibe os dados detalhados da compra em um relatório.
         * @param {Object} item O acerto de cheque que será exibido.
         * @param {Boolean} exportarExcel Define se deverá ser gerada exportação para o excel.
         */
        abrirListaAcertosCheques: function (item, exportaExcel) {
            var url = '../Relatorios/RelBase.aspx?Rel=AcertoChequesDevolvidos' + item.id + this.formatarFiltros_() +'&exportarExcel=' + exportaExcel;
            this.abrirJanela(600, 800, url);
        },
    }
});