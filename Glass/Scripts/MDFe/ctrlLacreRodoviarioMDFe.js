
function carregaLacreInicial(nomeControle, lacre) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    if (lacre.indexOf(';') > -1) {
        var _lacre = lacre.split(';');
        var primeiroItem = true;
        for (var i = 0; i < _lacre.length; i++) {
            if (_lacre[i] != '') {
                if (primeiroItem) {
                    FindControl('txtLacre', 'input', tabela.rows[tabela.rows.length - 1]).value = _lacre[i];
                    primeiroItem = false;
                }
                else {
                    tabela.insertRow(tabela.rows.length);
                    tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                    FindControl('txtLacre', 'input', tabela.rows[i - 1]).value = _lacre[i];

                    atualizaBotoesLacre(nomeControle);
                }
            }
        }
    }
    else {
        FindControl('txtLacre', 'input', tabela.rows[tabela.rows.length - 1]).value = lacre;
    }

    pegarValorLacre(nomeControle);
}

function atualizaBotoesLacre(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
    }
}

function removerLinhaLacre(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoesLacre(nomeControle);

    FindControl('txtLacre', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;

    pegarValorLacre(nomeControle);
}

function adicionarLinhaLacre(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    var lacreAtual = FindControl('txtLacre', 'input', tabela.rows[tabela.rows.length - 1]).value;
    if (lacreAtual == '' || lacreAtual == null) {
        alert('para criar nova linha, a atual deve estar preenchida');
    }
    else {
        tabela.insertRow(tabela.rows.length);
        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;

        FindControl('txtLacre', 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;
        FindControl('txtLacre', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;

        atualizaBotoesLacre(nomeControle);
    }
}

function pegarValorLacre(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    var lacre = '';

    for (var i = 0; i < tabela.rows.length; i++) {

        if (FindControl('txtLacre', 'input', tabela.rows[i]).value != '') {
            lacre = lacre + ';' + FindControl('txtLacre', 'input', tabela.rows[i].cells[1]).value;
        }
    }
    FindControl('hdfLacres', 'input').value = lacre;
}