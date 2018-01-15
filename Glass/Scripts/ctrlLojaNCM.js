function ctrlLojaNCM(nomeControle) {

    this.nomeControle = nomeControle;

    this.adicionarLinha = function () {

        var tabela = document.getElementById(this.nomeControle + '_tabela');
        tabela.insertRow(tabela.rows.length);

        var pos = tabela.rows.length - 1;
        tabela.rows[pos].innerHTML = tabela.rows[0].innerHTML;

        var drpLoja = FindControl('drpLoja', 'select', tabela.rows[pos]);
        var txtNCM = FindControl('txtNCM', 'input', tabela.rows[pos]);
        //var ctvValidaQtde = FindControl('ctvValidaNCM', 'span', tabela.rows[pos]);

        var tabela = txtNCM;
        while (tabela.nodeName.toLowerCase() != 'table')
            tabela = tabela.parentNode;

        var textoInicial = 'drpLoja', textoFinal = 'drpLoja' + pos;

        var controleAtual = drpLoja.id.substr(0, drpLoja.id.lastIndexOf('_'));
        controleAtual = controleAtual.substr(0, controleAtual.lastIndexOf('_'));

        var novoControle = alteraTexto(controleAtual, textoInicial, textoFinal);

        drpLoja.id = alteraTexto(drpLoja.id, textoInicial, textoFinal);
        txtNCM.id = this.alteraTexto(txtNCM.id, 'txtNCM', 'txtNCM' + pos);
        //ctvValidaQtde.id = alteraTexto(ctvValidaQtde.id, 'ctvValidaNCM', 'ctvValidaNCM' + pos);

        txtNCM.value = '';

        this.atualizaBotoes();
    }

    this.atualizaBotoes = function () {
        var tabela = document.getElementById(this.nomeControle + '_tabela');
        for (i = 0; i < tabela.rows.length; i++) {
            var isUltimaLinha = (i + 1) == tabela.rows.length;
            FindControl('imgAdicionar', 'input', tabela.rows[i].cells[3]).style.display = isUltimaLinha ? '' : 'none';
            FindControl('imgRemover', 'input', tabela.rows[i].cells[3]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
        }
    }

    this.alteraTexto = function (textoOriginal, textoBuscar, textoAlterar) {
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

    this.removerLinha = function () {
        var tabela = document.getElementById(this.nomeControle + '_tabela');

        // Retira do hiddenfield que salva os produtos associados este produto que está sendo removido
        var hdfIdLoja = document.getElementById(this.nomeControle + '_hdfIdLoja');
        var hdfNcm = document.getElementById(this.nomeControle + '_hdfNCM');
        hdfIdLoja.value = hdfIdLoja.value.slice(0, hdfIdLoja.value.lastIndexOf(';'));
        hdfNcm.value = hdfNcm.value.slice(0, hdfNcm.value.lastIndexOf(';'));

        tabela.deleteRow(tabela.rows.length - 1);
        atualizaBotoes(this.nomeControle);
    }

    this.carregaNCMs = function () {
        var tabela = document.getElementById(this.nomeControle + '_tabela');
        var idsLojas = document.getElementById(this.nomeControle + '_hdfIdLoja').value;
        var ncms = document.getElementById(this.nomeControle + '_hdfNCM').value;

        var numeroItens = idsLojas.split(';').length;
        for (qtdeNcm = 0; qtdeNcm < numeroItens; qtdeNcm++) {

            if (qtdeNcm > 0)
                this.adicionarLinha();

            FindControl('drpLoja', 'select', tabela.rows[qtdeNcm].cells[0]).value = idsLojas.split(';')[qtdeNcm];
            FindControl('txtNCM', 'input', tabela.rows[qtdeNcm].cells[2]).value = ncms.split(';')[qtdeNcm];
        }
    }

    this.ncmCallback = function () {

        var tabela = document.getElementById(this.nomeControle + '_tabela');

        var idsLojas = new Array();
        var ncms = new Array();
        for (i = 0; i < tabela.rows.length; i++) {
            var idLoja = FindControl('drpLoja', 'select', tabela.rows[i].cells[0]).value;
            var ncm = FindControl('txtNCM', 'input', tabela.rows[i].cells[2]).value;

            //var add = true;
            //for (j = 0; j < idsLojas.length; j++)
            //    if (idsLojas[j] == idLoja) {
            //        add = false;
            //        break;
            //    }

            //if (add) {
                idsLojas.push(idLoja);
                ncms.push(ncm);
            //}
        }

        var nomeControle = tabela.id.substr(0, tabela.id.lastIndexOf('_'));
        document.getElementById(this.nomeControle + '_hdfIdLoja').value = idsLojas.join(';');
        document.getElementById(this.nomeControle + '_hdfNCM').value = ncms.join(';');
    }
}