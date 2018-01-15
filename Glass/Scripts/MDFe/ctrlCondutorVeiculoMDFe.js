
function carregaCondutorInicial(nomeControle, condutor) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    if (condutor.indexOf(';') > -1) {
        var _condutor = condutor.split(';');
        var primeiroItem = true;
        for (var i = 0; i < _condutor.length; i++) {
            if (_condutor[i] != '') {
                if (primeiroItem) {
                    FindControl('drpCondutor', 'select', tabela.rows[tabela.rows.length - 1]).value = _condutor[i];
                    primeiroItem = false;
                }
                else {
                    tabela.insertRow(tabela.rows.length);
                    tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                    FindControl('drpCondutor', 'select', tabela.rows[i - 1]).value = _condutor[i];

                    atualizaBotoesCondutor(nomeControle);
                }
            }
        }
    }
    else {
        FindControl('drpCondutor', 'select', tabela.rows[tabela.rows.length - 1]).value = condutor;
    }

    pegarValorCondutor(nomeControle);
}

function atualizaBotoesCondutor(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
    }
}

function removerLinhaCondutor(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoesCondutor(nomeControle);

    FindControl('drpCondutor', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;

    pegarValorCondutor(nomeControle);
}

function adicionarLinhaCondutor(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    var condutorAtual = FindControl('drpCondutor', 'select', tabela.rows[tabela.rows.length - 1]).value;
    if (condutorAtual == '' || condutorAtual == null) {
        alert('para criar nova linha, a atual deve estar preenchida');
    }
    else {
        tabela.insertRow(tabela.rows.length);
        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;

        FindControl('drpCondutor', 'select', tabela.rows[tabela.rows.length - 2]).disabled = true;
        FindControl('drpCondutor', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;

        atualizaBotoesCondutor(nomeControle);
    }
}

function pegarValorCondutor(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    var condutor = '';

    for (var i = 0; i < tabela.rows.length; i++) {

        if (FindControl('drpCondutor', 'select', tabela.rows[i]).value != '') {
            condutor = condutor + ';' + FindControl('drpCondutor', 'select', tabela.rows[i].cells[1]).value;
        }
    }
    FindControl('hdfCondutores', 'input').value = condutor;
}