
function carregaPedagioInicial(nomeControle, fornecedor, responsavel, numeroCompra, valorPedagio) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    if (fornecedor.indexOf(';') > -1) {
        var _fornecedor = fornecedor.split(';');
        var _responsavel = responsavel.split(';');
        var _numeroCompra = numeroCompra.split(';');
        var _valorPedagio = valorPedagio.split(';');
        var primeiroItem = true;
        for (var i = 0; i < _fornecedor.length; i++) {
            if (_fornecedor[i] != '') {
                if (primeiroItem) {
                    FindControl('drpFornecedor', 'select', tabela.rows[tabela.rows.length - 2]).value = _fornecedor[i];
                    FindControl('drpResponsavelPedagio', 'select', tabela.rows[tabela.rows.length - 2]).value = _responsavel[i];
                    FindControl('txtNumeroCompra', 'input', tabela.rows[tabela.rows.length - 1]).value = _numeroCompra[i];
                    FindControl('txtValorPedagio', 'input', tabela.rows[tabela.rows.length - 1]).value = _valorPedagio[i];
                    primeiroItem = false;
                }
                else {
                    tabela.insertRow(tabela.rows.length);
                    tabela.insertRow(tabela.rows.length);
                    tabela.rows[tabela.rows.length - 2].innerHTML = tabela.rows[0].innerHTML;
                    tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[1].innerHTML;
                    FindControl('drpFornecedor', 'select', tabela.rows[tabela.rows.length - 2]).value = _fornecedor[i];
                    FindControl('drpResponsavelPedagio', 'select', tabela.rows[tabela.rows.length - 2]).value = _responsavel[i];
                    FindControl('txtNumeroCompra', 'input', tabela.rows[tabela.rows.length - 1]).value = _numeroCompra[i];
                    FindControl('txtValorPedagio', 'input', tabela.rows[tabela.rows.length - 1]).value = _valorPedagio[i];

                    atualizaBotoesPedagio(nomeControle);
                }
            }
        }
    }
    else {
        FindControl('drpFornecedor', 'select', tabela.rows[tabela.rows.length - 2]).value = fornecedor;
        FindControl('drpResponsavelPedagio', 'select', tabela.rows[tabela.rows.length - 2]).value = responsavel;
        FindControl('txtNumeroCompra', 'input', tabela.rows[tabela.rows.length - 1]).value = numeroCompra;
        FindControl('txtValorPedagio', 'input', tabela.rows[tabela.rows.length - 1]).value = valorPedagio;
    }

    pegarValorPedagio(nomeControle);
}

function atualizaBotoesPedagio(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 1; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
        i++;
    }
}

function removerLinhaPedagio(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.deleteRow(tabela.rows.length - 1);
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoesPedagio(nomeControle);

    FindControl('drpFornecedor', 'select', tabela.rows[tabela.rows.length - 2]).disabled = false;
    FindControl('drpResponsavelPedagio', 'select', tabela.rows[tabela.rows.length - 2]).disabled = false;
    FindControl('txtNumeroCompra', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
    FindControl('txtValorPedagio', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;

    pegarValorPedagio(nomeControle);
}

function adicionarLinhaPedagio(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    var fornecedor = FindControl('drpFornecedor', 'select', tabela.rows[tabela.rows.length - 2]).value;
    var responsavel = FindControl('drpResponsavelPedagio', 'select', tabela.rows[tabela.rows.length - 2]).value;
    var numeroCompra = FindControl('txtNumeroCompra', 'input', tabela.rows[tabela.rows.length - 1]).value;
    var valorPedagio = FindControl('txtValorPedagio', 'input', tabela.rows[tabela.rows.length - 1]).value;
    if (fornecedor == '' || fornecedor == null || responsavel == '' || responsavel == null ||
        numeroCompra == '' || numeroCompra == null || valorPedagio == '' || valorPedagio == null) {
        alert('para criar nova linha, a atual deve estar preenchida');
    }
    else {
        tabela.insertRow(tabela.rows.length);
        tabela.insertRow(tabela.rows.length);
        tabela.rows[tabela.rows.length - 2].innerHTML = tabela.rows[0].innerHTML;
        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[1].innerHTML;

        FindControl('drpFornecedor', 'select', tabela.rows[tabela.rows.length - 4]).disabled = true;
        FindControl('drpFornecedor', 'select', tabela.rows[tabela.rows.length - 2]).disabled = false;
        FindControl('drpResponsavelPedagio', 'select', tabela.rows[tabela.rows.length - 4]).disabled = true;
        FindControl('drpResponsavelPedagio', 'select', tabela.rows[tabela.rows.length - 2]).disabled = false;
        FindControl('txtNumeroCompra', 'input', tabela.rows[tabela.rows.length - 3]).disabled = true;
        FindControl('txtNumeroCompra', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
        FindControl('txtValorPedagio', 'input', tabela.rows[tabela.rows.length - 3]).disabled = true;
        FindControl('txtValorPedagio', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;

        atualizaBotoesPedagio(nomeControle);
    }
}

function pegarValorPedagio(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    var fornecedor = '';
    var responsavel = '';
    var numeroCompra = '';
    var valorPedagio = '';

    for (var i = 0; i < tabela.rows.length; i++) {
        if (FindControl('drpFornecedor', 'select', tabela.rows[i]).value != '' &&
            FindControl('drpResponsavelPedagio', 'select', tabela.rows[i]).value != '' &&
            FindControl('txtNumeroCompra', 'input', tabela.rows[i + 1]).value != '' &&
            FindControl('txtValorPedagio', 'input', tabela.rows[i + 1]).value != '') {
            fornecedor = fornecedor + ';' + FindControl('drpFornecedor', 'select', tabela.rows[i].cells[1]).value;
            responsavel = responsavel + ';' + FindControl('drpResponsavelPedagio', 'select', tabela.rows[i].cells[3]).value;
            numeroCompra = numeroCompra + ';' + FindControl('txtNumeroCompra', 'input', tabela.rows[i + 1].cells[1]).value;
            valorPedagio = valorPedagio + ';' + FindControl('txtValorPedagio', 'input', tabela.rows[i + 1].cells[3]).value;
        }
        i++;
    }

    FindControl('hdfFornecedor', 'input').value = fornecedor;
    FindControl('hdfResponsavel', 'input').value = responsavel;
    FindControl('hdfNumeroCompra', 'input').value = numeroCompra;
    FindControl('hdfValorPedagio', 'input').value = valorPedagio;
}