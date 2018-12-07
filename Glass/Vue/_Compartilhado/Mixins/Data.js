var Mixins = Mixins || {};

Mixins.Data = {
  methods: {
    adicionarDias: function (dataBase, dias) {
      var data = new Date(dataBase);
      data.setDate(data.getDate() + dias);
      return data;
    },

    /**
     * Adiciona a quantidade de meses informada à data base informada.
     * @param {!date} dataBase A data que será alterada.
     * @param {!number} meses A quantidade de meses que serão adicionados à data base.
     * @returns {Date} A database acrescida da quantidade de meses informados.
     */
    adicionarMeses: function (dataBase, meses) {
      var data = new Date(dataBase);
      data.setMonth(data.getMonth() + meses);
      return data;
    }
  }
}