
function carregaVeiculoInicial(nomeControle, veiculo) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    if (veiculo.indexOf(';') > -1) {
        var _veiculo = veiculo.split(';');
        var primeiroItem = true;
        for (var i = 0; i < _veiculo.length; i++) {
            if (_veiculo[i] != '') {
                if (primeiroItem) {
                    FindControl('drpVeiculoReboque', 'select', tabela.rows[tabela.rows.length - 1]).value = _veiculo[i];
                    primeiroItem = false;
                }
                else {
                    tabela.insertRow(tabela.rows.length);
                    tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                    FindControl('drpVeiculoReboque', 'select', tabela.rows[i - 1]).value = _veiculo[i];

                    atualizaBotoesVeiculo(nomeControle);
                }
            }
        }
    }
    else {
        FindControl('drpVeiculoReboque', 'select', tabela.rows[tabela.rows.length - 1]).value = veiculo;
    }

    pegarValorVeiculo(nomeControle);
}

function atualizaBotoesVeiculo(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
    }
}

function removerLinhaVeiculo(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoesVeiculo(nomeControle);

    FindControl('drpVeiculoReboque', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;

    pegarValorVeiculo(nomeControle);
}

function adicionarLinhaVeiculo(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    var veiculoAtual = FindControl('drpVeiculoReboque', 'select', tabela.rows[tabela.rows.length - 1]).value;
    if (veiculoAtual == '' || veiculoAtual == null) {
        alert('para criar nova linha, a atual deve estar preenchida');
    }
    else {
        tabela.insertRow(tabela.rows.length);
        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;

        FindControl('drpVeiculoReboque', 'select', tabela.rows[tabela.rows.length - 2]).disabled = true;
        FindControl('drpVeiculoReboque', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;

        atualizaBotoesVeiculo(nomeControle);
    }
}

function pegarValorVeiculo(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    var veiculo = '';

    for (var i = 0; i < tabela.rows.length; i++) {

        if (FindControl('drpVeiculoReboque', 'select', tabela.rows[i]).value != '') {
            veiculo = veiculo + ';' + FindControl('drpVeiculoReboque', 'select', tabela.rows[i].cells[1]).value;
        }
    }
    FindControl('hdfVeiculosReboque', 'input').value = veiculo;
}