function exibirFormasPagto(botao, nomeTabela)
{
    for (iTip = 0; iTip < 2; iTip++)
    {
        TagToTip(nomeTabela, FADEIN, 300, COPYCONTENT, false, TITLE, 'Formas Pagto.', CLOSEBTN, true,
            CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
            FIX, [botao, 9-getTableWidth(nomeTabela), -41-getTableHeight(nomeTabela)]);
    }
}

function validarFormasPagtoUsar(val, args)
{
    var nomeControle = val.id.substr(0, val.id.indexOf("ctvFormasPagtoUsar"));
    var formasPagto = nomeControle + "cblFormasPagto";

    args.IsValid = false;
    eval(val.id).errormessage = "Selecione uma forma de pagamento para continuar.";

    var inputs = document.getElementById(formasPagto).getElementsByTagName("input");
    for (i = 0; i < inputs.length; i++)
        if (inputs[i].checked)
        {
            args.IsValid = true;
            break;
        }

    /*
    if (args.IsValid)
    {
        eval(val.id).errormessage = "Selecione a forma de pagamento padrão para continuar.";
        args.IsValid = document.getElementById(nomeControle + "drpTipoPagto").selectedIndex > 0;
    }
    */
}

/**
 * Altera os dados do drop de formas de pagamento.
 * @param {?Object} selecionado Indicador se o checkbox está ou não marcado.
 * @param {?number} idFormaPagto Identificador da forma de pagamento.
 * @param {?string} formaPagto Descritivo da forma de pagamento.
 * @param {?number} idFormaPagtoPadrao Identificador da forma de pagamento padrão.
 */
function alterarFormasPagtoPadrao(selecionado, idFormaPagto, formaPagto, idFormaPagtoPadrao) {

  if (idFormaPagtoPadrao == idFormaPagto) {
    return;
  }

  var drpFormaPagto = FindControl("drpFormaPagto", "select");

  if (selecionado) {
    var option = document.createElement('option');
    option.text = formaPagto;
    option.value = idFormaPagto;
    drpFormaPagto.add(option, drpFormaPagto.options.length);
  } else {
    for (i = 0; i < drpFormaPagto.options.length; i++) {
      if (drpFormaPagto.options[i].value == idFormaPagto) {
        drpFormaPagto.remove(drpFormaPagto.options[i].index);
        break;
      }
    }
  }
}

/**
 * Altera os dados do checkbox com a forma de pagamento padrão caso desmarcado.
 * @param {?Object} controle checkbox para verificação.
 * @param {?number} idFormaPagto Identificador da forma de pagamento.
 * @param {?number} idFormaPagtoPadrao Identificador da forma de pagamento padrão.
 */
function alterarFormaPagtoPadraoDesmarcada(controle, idFormaPagto, idFormaPagtoPadrao) {
  if (!controle.checked && idFormaPagtoPadrao == idFormaPagto) {
    controle.checked = true;
    alert("A forma de pagamento padrão não pode ser retirada.\nAltere a forma de pagamento padrão e atualize o cliente para que seja possível retirar esta forma de pagamento.");
  }
}
