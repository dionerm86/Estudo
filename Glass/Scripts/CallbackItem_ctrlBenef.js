// -------------------------------
// Função executada como callback.
// -------------------------------
function callbackSelecao(beneficiamento, controle, beneficiamentosControle, prefixoControle)
{
    // Verifica se o controle é o da lapidação ou do bisotê
    if ((beneficiamento.TipoControle == 1 || beneficiamento.TipoControle == 2) && controle.id.indexOf("_drpTipo") > -1)
    {
        var prefixoCampos = controle.id.substr(prefixoControle.length);
        prefixoCampos = prefixoCampos.substr(0, prefixoCampos.indexOf("_drpTipo"));

        var altura = parseInt(document.getElementById(prefixoControle + prefixoCampos + "_drpAltura").value, 10);
        var largura = parseInt(document.getElementById(prefixoControle + prefixoCampos + "_drpLargura").value, 10);
        var valor = parseFloat(controle.value) > 0 ? "2" : "0";
        if ((altura > 0 || largura > 0) && !(altura == 2 && largura == 2 && valor == "0"))
          return;

        document.getElementById(prefixoControle + prefixoCampos + "_drpAltura").value = valor;
        document.getElementById(prefixoControle + prefixoCampos + "_drpLargura").value = valor;
    }
}

// ---------------------------------------------------------------------
// Função executada para recuperar valor adicional para a Vintage/Dekor.
// ---------------------------------------------------------------------
function valorAdicionalVintageDekor(beneficiamento, controle, beneficiamentosControle, prefixoControle)
{
    // Variável de valor de retorno
    var valorRetorno = 0;

    // Verifica se o controle é o do molde
    if (controle.id.indexOf("_Molde_") > -1)
    {
        // Variáveis de controle da execução
        var lapidacao = false;
        var bisote = false;

        // Percorre os beneficiamentos
        for (s = 0; s < beneficiamentosControle.length; s++)
        {
            // Verifica se o beneficiamento é 'Lapidação'
            if (beneficiamentosControle[s].Descricao.toLowerCase() == "lapidação")
            {
                // Recupera o valor do beneficiamento (arquivo ctrlBenef.js)
                valorRetorno += recuperaValorBenef(prefixoControle, beneficiamentosControle[s].ID) / 100;

                // Indica que a alteração na lapidação foi feita
                lapidacao = true;
                if (bisote)
                    break;
            }

            // Verifica se o beneficiamento é 'Bisote'
            else if (beneficiamentosControle[s].Descricao.toLowerCase() == "bisote")
            {
                // Recupera o valor do beneficiamento (arquivo ctrlBenef.js)
                valorRetorno += recuperaValorBenef(prefixoControle, beneficiamentosControle[s].ID) / 100;

                // Indica que a alteração no bisotê foi feita
                bisote = true;
                if (lapidacao)
                    break;
            }
        }
    }

    // Retorna o valor
    return valorRetorno;
}

// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Função executada para verificar a obrigatoriedade de preenchimento do valor de um beneficiamento.
// --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
function verificarObrigatoriedadeBeneficiamentos(idProd) {
    var tbConfigVidro = FindControl("tbConfigVidro_", "table");

    //Na tela de CadPedidoEspelho.aspx e LstOrcamentoRapido.aspx o nome é diferente. Por não conseguir precisar, rapidamente, a complexidade do impacto da alteração do nome optei por essa solução.
    if (tbConfigVidro == null)
        tbConfigVidro = FindControl("tbConfigVidro", "table");

    var beneficiamentosSelecaoObrigatoria = MetodosAjax.ObterBeneficiamentosPreenchimentoObrigatorio(idProd).value.split(';');
    var mensagemBeneficiamentosObrigatorios = "";

    for (var i = 0; i < beneficiamentosSelecaoObrigatoria.length; i++) {
        var itens = beneficiamentosSelecaoObrigatoria[i].split('|');

        if (itens[1] == "listaselecao" || itens[1] == "lapidacao") {
            var drop = FindControl(itens[0] + "_drpTipo", "select", tbConfigVidro);
            if (drop.options[drop.selectedIndex].text == "")
                mensagemBeneficiamentosObrigatorios += "\n* " + itens[0].replace("_", " ");
        }
        else if (itens[1] == "quantidade") {
            var textBox = FindControl(itens[0] + "_tblQtd_txtQtd", "input", tbConfigVidro);
            if (textBox.value == "0" || textBox.value == "")
                mensagemBeneficiamentosObrigatorios += "\n* " + itens[0].replace("_", " ");
        }
        else if (itens[1] == "bisote") {
            var dropTipoBisote = FindControl(itens[0] + "_drpTipo", "select", tbConfigVidro);
            var txtEspessuraBisote = FindControl(itens[0] + "_txtEspessura", "input", tbConfigVidro);

            if (dropTipoBisote.options[dropTipoBisote.selectedIndex].text == "" || txtEspessuraBisote.value == "" || txtEspessuraBisote.value == "0")
                mensagemBeneficiamentosObrigatorios += "\n* " + itens[0].replace("_", " ");
        }
        else if (itens[1] == "listaselecaoqtd") {
            var drop = FindControl(itens[0] + "_drpTipo", "select", tbConfigVidro);
            var textBox = FindControl(itens[0] + "_tblQtd_txtQtd", "input", tbConfigVidro);

            if ((drop != null && drop.options[drop.selectedIndex].text == "") || (textBox != null && (textBox.value == "0" || textBox.value == "")))
                mensagemBeneficiamentosObrigatorios += "\n* " + itens[0].replace("_", " ");
        }
    }

    if (mensagemBeneficiamentosObrigatorios != "") {
        alert("O(s) valor(es) do(s) beneficiamento(s) abaixo precisa(m) ser definido(s) " + mensagemBeneficiamentosObrigatorios);
        return false;
    }

    return true;
}
