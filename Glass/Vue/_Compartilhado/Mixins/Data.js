var Mixins = Mixins || {};

Mixins.Data = {
  methods: {
    adicionarDias: function (dataBase, dias) {
      var data = new Date(dataBase);
      data.setDate(data.getDate() + dias);
      return data;
    }
  }
}