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
      opacity: 0.75,
      display: 'grid',
      alignItems: 'center',
      justifyItems: 'center'
    }, frame);

    const mensagem = {
      textAlign: 'center',
      display: 'inline-block'
    };

    const processamento = {
      position: 'fixed',
      backgroundColor: 'white',
      border: '1px solid #E2E2E4',
      padding: '3px',
      display: 'inline-grid',
      gridTemplateColumns: 'repeat(2, max-content)',
      alignItems: 'center',
      gridGap: '0 4px',
      bottom: '0px',
      left: '0px'
    };

    return {
      frame,
      bloqueio,
      processamento,
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
