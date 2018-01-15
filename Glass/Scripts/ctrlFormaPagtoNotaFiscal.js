function ctrlFormaPagtoNotaFiscal(nomeControle) {

    this.nomeControle = nomeControle;

    this.adicionarLinha = function () {

        var tabela = document.getElementById(this.nomeControle + '_tabela');
        tabela.insertRow(tabela.rows.length);
        tabela.insertRow(tabela.rows.length);

        var pos = tabela.rows.length - 2;
        tabela.rows[pos].innerHTML = tabela.rows[0].innerHTML;
        tabela.rows[pos + 1].innerHTML = tabela.rows[1].innerHTML;


        var txtValorReceb = FindControl('txtValorReceb', 'input', tabela.rows[pos]);
        var drpFormaPagto = FindControl('drpFormaPagto', 'select', tabela.rows[pos]);

        var txtCnpjCredenciadora = FindControl('txtCnpjCredenciadora', 'input', tabela.rows[pos + 1]);
        var drpBandeira = FindControl('drpBandeira', 'select', tabela.rows[pos + 1]);
        var txtNumAut = FindControl('txtNumAut', 'input', tabela.rows[pos + 1]);

        var tabela = drpFormaPagto;
        while (tabela.nodeName.toLowerCase() != 'table')
            tabela = tabela.parentNode;

        var textoInicial = 'drpFormaPagto', textoFinal = 'drpFormaPagto' + (pos / 2);

        var controleAtual = drpFormaPagto.id.substr(0, drpFormaPagto.id.lastIndexOf('_'));
        controleAtual = controleAtual.substr(0, controleAtual.lastIndexOf('_'));

        var novoControle = this.alteraTexto(controleAtual, textoInicial, textoFinal);

        drpFormaPagto.id = this.alteraTexto(drpFormaPagto.id, textoInicial, textoFinal);
        txtValorReceb.id = this.alteraTexto(txtValorReceb.id, 'txtValorReceb', 'txtValorReceb' + (pos / 2));

        tabela.rows[pos + 1].id = "dadosCartao" + (pos / 2);
        tabela.rows[pos + 1].style.display = "none";
        drpBandeira.id = this.alteraTexto(drpBandeira.id, 'drpBandeira', 'drpBandeira' + (pos / 2));
        txtCnpjCredenciadora.id = this.alteraTexto(txtCnpjCredenciadora.id, 'txtCnpjCredenciadora', 'txtCnpjCredenciadora' + (pos / 2));
        txtNumAut.id = this.alteraTexto(txtNumAut.id, 'txtNumAut', 'txtNumAut' + (pos / 2));

        txtValorReceb.value = '';

        txtCnpjCredenciadora.value = '';
        txtNumAut.value = '';

        this.atualizaBotoes();
    }

    this.atualizaBotoes = function () {
        var tabela = document.getElementById(this.nomeControle + '_tabela');
        for (i = 0; i < tabela.rows.length; i++) {
            var isUltimaLinha = (i + 2) == tabela.rows.length;

            var imgAdd = FindControl('imgAdicionar', 'input', tabela.rows[i].cells[4])
            var imgRem = FindControl('imgRemover', 'input', tabela.rows[i].cells[4]);

            if (imgAdd != null)
                imgAdd.style.display = isUltimaLinha ? '' : 'none';

            if (imgRem != null)
                imgRem.style.display = i > 0 && isUltimaLinha ? '' : 'none';
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

        
        var _hdfValoreReceb = document.getElementById(this.nomeControle + '_hdfValoreReceb');
        var _hdfFormaPagto = document.getElementById(this.nomeControle + '_hdfFormaPagto');
        var _hdfCnpj = document.getElementById(this.nomeControle + '_hdfCnpj');
        var _hdfBandeira = document.getElementById(this.nomeControle + '_hdfBandeira');
        var _hdfNumAut = document.getElementById(this.nomeControle + '_hdfNumAut');

        _hdfValoreReceb.value = _hdfValoreReceb.value.slice(0, _hdfValoreReceb.value.lastIndexOf(';'));
        _hdfFormaPagto.value = _hdfFormaPagto.value.slice(0, _hdfFormaPagto.value.lastIndexOf(';'));
        _hdfCnpj.value = _hdfCnpj.value.slice(0, _hdfCnpj.value.lastIndexOf(';'));
        _hdfBandeira.value = _hdfBandeira.value.slice(0, _hdfBandeira.value.lastIndexOf(';'));
        _hdfNumAut.value = _hdfNumAut.value.slice(0, _hdfNumAut.value.lastIndexOf(';'));

        tabela.deleteRow(tabela.rows.length - 1);
        tabela.deleteRow(tabela.rows.length - 1);

        this.atualizaBotoes(this.nomeControle);
    }

    this.carregaPagamentos = function () {
        var tabela = document.getElementById(this.nomeControle + '_tabela');

        var valoreReceb = document.getElementById(this.nomeControle + '_hdfValoreReceb').value;
        var formaPagto = document.getElementById(this.nomeControle + '_hdfFormaPagto').value;
        var cnpj = document.getElementById(this.nomeControle + '_hdfCnpj').value;
        var bandeira = document.getElementById(this.nomeControle + '_hdfBandeira').value;
        var numAut = document.getElementById(this.nomeControle + '_hdfNumAut').value;


        var numeroItens = formaPagto.split(';').length;
        var row = 0;

        for (qtdePagto = 0; qtdePagto < numeroItens; qtdePagto++) {

            if (qtdePagto > 0)
                this.adicionarLinha();

            FindControl('txtValorReceb', 'input', tabela.rows[row].cells[1]).value = valoreReceb.split(';')[qtdePagto];
            FindControl('drpFormaPagto', 'select', tabela.rows[row].cells[3]).value = formaPagto.split(';')[qtdePagto];
            $(FindControl('drpFormaPagto', 'select', tabela.rows[row].cells[3])).change();

            row += 1;

            FindControl('txtCnpjCredenciadora', 'input', tabela.rows[row].cells[1]).value = cnpj.split(';')[qtdePagto];
            FindControl('drpBandeira', 'select', tabela.rows[row].cells[3]).value = bandeira.split(';')[qtdePagto];
            FindControl('txtNumAut', 'input', tabela.rows[row].cells[5]).value = numAut.split(';')[qtdePagto];

            row += 1;
        }

        this.pagamentoCallback();
    }

    this.pagamentoCallback = function () {

        var tabela = document.getElementById(this.nomeControle + '_tabela');

        var lstValoreReceb = new Array();
        var lstFormaPagto = new Array();
        var lstCnpj = new Array();
        var lstBandeira = new Array();
        var lstNumAut = new Array();

        var row = 0;

        for (i = 0; i < tabela.rows.length; i += 2) {

            var valorReb = FindControl('txtValorReceb', 'input', tabela.rows[row].cells[1]).value;
            var formaPagto = FindControl('drpFormaPagto', 'select', tabela.rows[row].cells[3]).value;

            row += 1;

            var cnpj = FindControl('txtCnpjCredenciadora', 'input', tabela.rows[row].cells[1]).value;
            var bandeira = FindControl('drpBandeira', 'select', tabela.rows[row].cells[3]).value;
            var numAut = FindControl('txtNumAut', 'input', tabela.rows[row].cells[5]).value;

            row += 1;

            lstValoreReceb.push(valorReb);
            lstFormaPagto.push(formaPagto);
            lstCnpj.push(cnpj);
            lstBandeira.push(bandeira);
            lstNumAut.push(numAut);
        }

        var nomeControle = tabela.id.substr(0, tabela.id.lastIndexOf('_'));

        document.getElementById(this.nomeControle + '_hdfValoreReceb').value = lstValoreReceb.join(';');
        document.getElementById(this.nomeControle + '_hdfFormaPagto').value = lstFormaPagto.join(';');
        document.getElementById(this.nomeControle + '_hdfCnpj').value = lstCnpj.join(';');
        document.getElementById(this.nomeControle + '_hdfBandeira').value = lstBandeira.join(';');
        document.getElementById(this.nomeControle + '_hdfNumAut').value = lstNumAut.join(';');
    }
}

function drpFormaPagtoChanged(control) {

    //var pos = parseInt(control.id.substr(control.id.length -1));
    
    //var dadosCartao = FindControl("dadosCartao" + (isNaN(pos) ? "" : pos), "tr");

    //if (dadosCartao != null)
    //    dadosCartao.style.display = control.value == "3" || control.value == "4" ? "none" : "none";

}