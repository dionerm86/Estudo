
function carregaUFPercursoInicial(nomeControle, uFPercurso) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    if (uFPercurso.indexOf(';') > -1) {
        var _uFPercurso = uFPercurso.split(';');
        var primeiroItem = true;
        for (var i = 0; i < _uFPercurso.length; i++) {
            if (_uFPercurso[i] != '') {
                if (primeiroItem) {
                    FindControl('drpUFPercurso', 'select', tabela.rows[tabela.rows.length - 1]).value = _uFPercurso[i];
                    primeiroItem = false;
                }
                else {
                    tabela.insertRow(tabela.rows.length);
                    tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                    FindControl('drpUFPercurso', 'select', tabela.rows[i - 1]).value = _uFPercurso[i];

                    atualizaBotoesUFPercurso(nomeControle);
                }
            }
        }
    }
    else {
        FindControl('drpUFPercurso', 'select', tabela.rows[tabela.rows.length - 1]).value = uFPercurso;
    }

    pegarValorUFPercurso(nomeControle);
}

function atualizaBotoesUFPercurso(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
    }
}

function removerLinhaUFPercurso(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoesUFPercurso(nomeControle);

    FindControl('drpUFPercurso', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;

    pegarValorUFPercurso(nomeControle);
}

function adicionarLinhaUFPercurso(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    var uFPercursoAtual = FindControl('drpUFPercurso', 'select', tabela.rows[tabela.rows.length - 1]).value;
    if (uFPercursoAtual == '' || uFPercursoAtual == null) {
        alert('para criar nova linha, a atual deve estar preenchida');
    }
    else {
        tabela.insertRow(tabela.rows.length);
        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;

        FindControl('drpUFPercurso', 'select', tabela.rows[tabela.rows.length - 2]).disabled = true;
        FindControl('drpUFPercurso', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;

        atualizaBotoesUFPercurso(nomeControle);
    }
}

function pegarValorUFPercurso(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    var uFPercurso = '';

    for (var i = 0; i < tabela.rows.length; i++) {

        if (FindControl('drpUFPercurso', 'select', tabela.rows[i]).value != '') {
            uFPercurso = uFPercurso + ';' + FindControl('drpUFPercurso', 'select', tabela.rows[i].cells[1]).value;
        }
    }
    FindControl('hdfUFsPercurso', 'input').value = uFPercurso;
}