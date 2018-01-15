
function carregaCidadeCargaInicial(nomeControle, cidadeCarga) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    if (cidadeCarga.indexOf(';') > -1) {
        var _cidadeCarga = cidadeCarga.split(';');
        var primeiroItem = true;
        for (var i = 0; i < _cidadeCarga.length; i++) {
            if (_cidadeCarga[i] != '') {
                if (primeiroItem) {
                    FindControl('drpCidadeCarga', 'select', tabela.rows[tabela.rows.length - 1]).value = _cidadeCarga[i];
                    primeiroItem = false;
                }
                else {
                    tabela.insertRow(tabela.rows.length);
                    tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                    FindControl('drpCidadeCarga', 'select', tabela.rows[i - 1]).value = _cidadeCarga[i];

                    atualizaBotoesCidadeCarga(nomeControle);
                }
            }
        }
    }
    else {
        FindControl('drpCidadeCarga', 'select', tabela.rows[tabela.rows.length - 1]).value = cidadeCarga;
    }

    pegarValorCidadeCarga(nomeControle);
}

function atualizaBotoesCidadeCarga(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
    }
}

function removerLinhaCidadeCarga(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoesCidadeCarga(nomeControle);

    FindControl('drpCidadeCarga', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;

    pegarValorCidadeCarga(nomeControle);
}

function adicionarLinhaCidadeCarga(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    var cidadeCargaAtual = FindControl('drpCidadeCarga', 'select', tabela.rows[tabela.rows.length - 1]).value;
    if (cidadeCargaAtual == '' || cidadeCargaAtual == null) {
        alert('para criar nova linha, a atual deve estar preenchida');
    }
    else {
        tabela.insertRow(tabela.rows.length);
        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;

        FindControl('drpCidadeCarga', 'select', tabela.rows[tabela.rows.length - 2]).disabled = true;
        FindControl('drpCidadeCarga', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;

        atualizaBotoesCidadeCarga(nomeControle);
    }
}

function pegarValorCidadeCarga(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    var cidadeCarga = '';

    for (var i = 0; i < tabela.rows.length; i++) {

        if (FindControl('drpCidadeCarga', 'select', tabela.rows[i]).value != '') {
            cidadeCarga = cidadeCarga + ';' + FindControl('drpCidadeCarga', 'select', tabela.rows[i].cells[1]).value;
        }
    }
    FindControl('hdfCidadesCarga', 'input').value = cidadeCarga;
}