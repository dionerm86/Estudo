var ctrlFilhoFerragem = new (function () {})();

function atualizaBotoes(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        var admSync = isAdminSync();
        var codFerragem = nomeControle.indexOf("ctrlCodigoFerragem") != -1;
        var podeAlterar = admSync || codFerragem;

        FindControl('imgAdicionar', 'input', tabela.rows[i].cells[5]).style.display = isUltimaLinha && podeAlterar ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i].cells[5]).style.display = i > 0 && isUltimaLinha && podeAlterar ? '' : 'none';
    }
}

function alteraTexto(textoOriginal, textoBuscar, textoAlterar) {
    var retorno = textoOriginal;
    var pos = 0;

    if (retorno != null)
        while ((pos = retorno.indexOf(textoBuscar, pos)) > -1) {
            var inicio = retorno.substr(0, pos);
            var fim = retorno.substr(pos + textoBuscar.length);

            retorno = inicio + textoAlterar + fim;
            pos += textoAlterar.length;
        }

    return retorno;
}

function carregaFilhoInicial(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    // Recupera os HiddenField com os valores
    var hdfIdsfilhosFerragem = document.getElementById(nomeControle + '_hdfIdsfilhosFerragem').value;
    var hdfDescricoes = document.getElementById(nomeControle + '_hdfDescricoes').value;
    var hdfValores = document.getElementById(nomeControle + '_hdfValores').value;

    var numeroItens = hdfIdsfilhosFerragem.split(';').length;
    for (iLoadProd = 0; iLoadProd < numeroItens; iLoadProd++) {
        // Cria uma linha para cada valor
        if (iLoadProd > 0)
            adicionarLinha(nomeControle);
        // Adiciona o valor nos campos
        FindControl('hdfIdFilhoFerragem', 'input', tabela.rows[iLoadProd].cells[0]).value = hdfIdsfilhosFerragem.split(';')[iLoadProd];
        FindControl('txtDescricao', 'input', tabela.rows[iLoadProd].cells[2]).value = hdfDescricoes.split(';')[iLoadProd];
        //if (FindControl('txtValor', 'input', tabela.rows[iLoadProd].cells[4]) != null)
            FindControl('txtValor', 'input', tabela.rows[iLoadProd].cells[4]).value = hdfValores.split(';')[iLoadProd] != null ? hdfValores.split(';')[iLoadProd].replace(".", ",") : "";
    }
}

function adicionarLinha(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.insertRow(tabela.rows.length);

    // Clona a primeira linha e adiciona ao final da tabela
    var pos = tabela.rows.length - 1;
    tabela.rows[pos].innerHTML = tabela.rows[0].innerHTML;

    // Recupera os valores da linha clonada
    var hdfIdFilhoFerragem = FindControl('hdfIdFilhoFerragem', 'input', tabela.rows[pos]);
    var txtDescricao = FindControl('txtDescricao', 'input', tabela.rows[pos]);
    var txtValor = FindControl('txtValor', 'input', tabela.rows[pos]);

    // Altera o ID dos novos campos para não gerar comflito ao recuperar
    hdfIdFilhoFerragem.id = alteraTexto(hdfIdFilhoFerragem.id, 'hdfIdFilhoFerragem', 'hdfIdFilhoFerragem' + pos);
    txtDescricao.id = alteraTexto(txtDescricao.id, 'txtDescricao', 'txtDescricao' + pos);
    //if (txtValor != null)
        txtValor.id = alteraTexto(txtValor.id, 'txtValor', 'txtValor' + pos);

    // Limpa os dados dos campos
    hdfIdFilhoFerragem.value = '';
    txtDescricao.value = '';
    //if (txtValor != null)
        txtValor.value = '';

    atualizaBotoes(nomeControle);
}

function removerLinha(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');

    // Recupera os HiddenField
    var hdfIdFilhoFerragem = document.getElementById(nomeControle + '_hdfIdFilhoFerragem');
    var hdfDescricoes = document.getElementById(nomeControle + '_hdfDescricoes');
    var hdfValores = document.getElementById(nomeControle + '_hdfValores');
    // Remove o valor do mesmo
    hdfIdFilhoFerragem.value = hdfIdFilhoFerragem.value.slice(0, hdfIdFilhoFerragem.value.lastIndexOf(';'));
    hdfDescricoes.value = hdfDescricoes.value.slice(0, hdfDescricoes.value.lastIndexOf(';'));
    hdfValores.value = hdfValores.value.slice(0, hdfValores.value.lastIndexOf(';'));

    // Remove a linha e atualiza os botões
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoes(nomeControle);
}

function selFilhoCallback(nomeControleFilho) {
    var tabela = document.getElementById(nomeControleFilho.replace('_ctrlSelProduto', '_txtDescricao')).parentNode;
    while (tabela.nodeName.toLowerCase() != 'table')
        tabela = tabela.parentNode;

    var idsfilhosFerragem = new Array();
    var descricoes = new Array();
    var valores = new Array();

    for (i = 0; i < tabela.rows.length; i++) {
        var idFilhoFerragem = FindControl('hdfIdFilhoFerragem', 'input', tabela.rows[i].cells[0]).value;
        var descricao = FindControl('txtDescricao', 'input', tabela.rows[i].cells[2]).value;
        var valor = FindControl('txtValor', 'input', tabela.rows[i].cells[4]).value;

        idsfilhosFerragem.push(idFilhoFerragem);
        descricoes.push(descricao);
        valores.push(valor);
    }

    var nomeControle = tabela.id.substr(0, tabela.id.lastIndexOf('_'));
    document.getElementById(nomeControle + '_hdfIdsfilhosFerragem').value = idsfilhosFerragem.join(';');
    document.getElementById(nomeControle + '_hdfDescricoes').value = descricoes.join(';');
    document.getElementById(nomeControle + '_hdfValores').value = valores.join(';');
}

function isAdminSync()
{
    var isAdminSync = CadFerragem.IsAdminSync().value == "true";

    return isAdminSync;
}