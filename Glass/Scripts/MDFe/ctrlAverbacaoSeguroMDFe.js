
function carregaNumeroAverbacaoInicial(nomeControle, numeroAverbacao) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    if (numeroAverbacao.indexOf(';') > -1) {
        var _numeroAverbacao = numeroAverbacao.split(';');
        var primeiroItem = true;
        for (var i = 0; i < _numeroAverbacao.length; i++) {
            if (_numeroAverbacao[i] != '') {
                if (primeiroItem) {
                    FindControl('txtNumeroAverbacao', 'input', tabela.rows[tabela.rows.length - 1]).value = _numeroAverbacao[i];
                    primeiroItem = false;
                }
                else {
                    tabela.insertRow(tabela.rows.length);
                    tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                    FindControl('txtNumeroAverbacao', 'input', tabela.rows[i - 1]).value = _numeroAverbacao[i];

                    atualizaBotoesNumeroAverbacao(nomeControle);
                }
            }
        }
    }
    else {
        FindControl('txtNumeroAverbacao', 'input', tabela.rows[tabela.rows.length - 1]).value = numeroAverbacao;
    }

    pegarValorNumeroAverbacao(nomeControle);
}

function atualizaBotoesNumeroAverbacao(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
    }
}

function removerLinhaNumeroAverbacao(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoesNumeroAverbacao(nomeControle);

    FindControl('txtNumeroAverbacao', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;

    pegarValorNumeroAverbacao(nomeControle);
}

function adicionarLinhaNumeroAverbacao(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    var numeroAverbacaoAtual = FindControl('txtNumeroAverbacao', 'input', tabela.rows[tabela.rows.length - 1]).value;
    if (numeroAverbacaoAtual == '' || numeroAverbacaoAtual == null) {
        alert('para criar nova linha, a atual deve estar preenchida');
    }
    else {
        tabela.insertRow(tabela.rows.length);
        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;

        FindControl('txtNumeroAverbacao', 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;
        FindControl('txtNumeroAverbacao', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;

        atualizaBotoesNumeroAverbacao(nomeControle);
    }
}

function pegarValorNumeroAverbacao(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    var numeroAverbacao = '';

    for (var i = 0; i < tabela.rows.length; i++) {

        if (FindControl('txtNumeroAverbacao', 'input', tabela.rows[i]).value != '') {
            numeroAverbacao = numeroAverbacao + ';' + FindControl('txtNumeroAverbacao', 'input', tabela.rows[i].cells[1]).value;
        }
    }
    FindControl('hdfNumerosAverbacao', 'input').value = numeroAverbacao;
}