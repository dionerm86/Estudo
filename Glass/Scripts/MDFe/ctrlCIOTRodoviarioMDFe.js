
function carregaCIOTInicial(nomeControle, ciot, cpfCnpj) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    if (ciot.indexOf(';') > -1) {
        var _ciot = ciot.split(';');
        var _cpfCnpj = cpfCnpj.split(';');
        var primeiroItem = true;
        for (var i = 0; i < _ciot.length; i++) {
            if (_ciot[i] != '') {
                if (primeiroItem) {
                    FindControl('txtCIOT', 'input', tabela.rows[tabela.rows.length - 1]).value = _ciot[i];
                    FindControl('txtCPFCNPJ', 'input', tabela.rows[tabela.rows.length - 1]).value = _cpfCnpj[i];
                    primeiroItem = false;
                }
                else {
                    tabela.insertRow(tabela.rows.length);
                    tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                    FindControl('txtCIOT', 'input', tabela.rows[i - 1]).value = _ciot[i];
                    FindControl('txtCPFCNPJ', 'input', tabela.rows[i - 1]).value = _cpfCnpj[i];

                    atualizaBotoesCIOT(nomeControle);
                }
            }
        }
    }
    else {
        FindControl('txtCIOT', 'input', tabela.rows[tabela.rows.length - 1]).value = ciot;
        FindControl('txtCPFCNPJ', 'input', tabela.rows[tabela.rows.length - 1]).value = cpfCnpj;
    }

    pegarValorCIOT(nomeControle);
}

function atualizaBotoesCIOT(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
    }
}

function removerLinhaCIOT(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoesCIOT(nomeControle);

    FindControl('txtCIOT', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
    FindControl('txtCPFCNPJ', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;

    pegarValorCIOT(nomeControle);
}

function adicionarLinhaCIOT(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    var CiotAtual = FindControl('txtCIOT', 'input', tabela.rows[tabela.rows.length - 1]).value;
    var CpfCnpjAtual = FindControl('txtCPFCNPJ', 'input', tabela.rows[tabela.rows.length - 1]).value;
    if (CiotAtual == '' || CiotAtual == null || CpfCnpjAtual == '' || CpfCnpjAtual == null) {
        alert('para criar nova linha, a atual deve estar preenchida');
    }
    else {
        tabela.insertRow(tabela.rows.length);
        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;

        FindControl('txtCIOT', 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;
        FindControl('txtCIOT', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
        FindControl('txtCPFCNPJ', 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;
        FindControl('txtCPFCNPJ', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;

        atualizaBotoesCIOT(nomeControle);
    }
}

function pegarValorCIOT(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    var ciot = '';
    var cpfCnpj = '';

    for (var i = 0; i < tabela.rows.length; i++) {

        if (FindControl('txtCIOT', 'input', tabela.rows[i]).value != '' &&
            FindControl('txtCPFCNPJ', 'input', tabela.rows[i]).value != '') {
            ciot = ciot + ';' + FindControl('txtCIOT', 'input', tabela.rows[i].cells[1]).value;
            cpfCnpj = cpfCnpj + ';' + FindControl('txtCPFCNPJ', 'input', tabela.rows[i].cells[3]).value;
        }
    }
    FindControl('hdfCIOTs', 'input').value = ciot;
    FindControl('hdfCPFCNPJs', 'input').value = cpfCnpj;
}