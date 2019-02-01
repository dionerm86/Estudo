Vue.component('indicador-sistema-ocupado', {
  data: function () {
    const frame = {
      left: '0px',
      top: '0px',
      width: '100%',
      height: '100%',
      position: 'fixed'
    };

    const bloqueio = Object.assign({
      zIndex: 99999,
      backgroundColor: 'white',
      opacity: 0.95,
      display: 'grid',
      alignItems: 'center',
      justifyItems: 'center'
    }, frame);

    const mensagem = {
      textAlign: 'center',
      display: 'inline-block'
    };

    return {
      frame,
      bloqueio,
      mensagem,
      processando: false,
      bloqueado: false
    }
  },

  created: function () {
    var vm = this;

    this.$sistemaOcupado.$on('processando', function (processando) {
      vm.processando = processando;
    });

    this.$sistemaOcupado.$on('bloqueado', function (bloqueado) {
      vm.bloqueado = bloqueado;
    });
  },

  destroyed: function() {
    this.$sistemaOcupado.$off('processando');
    this.$sistemaOcupado.$off('bloqueado');
  },

  template: '#IndicadorSistemaOcupado-template'
});
