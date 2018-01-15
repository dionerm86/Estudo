function atualizaBotoes(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    for (i = 0; i < tabela.rows.length; i++) {
        var isUltimaLinha = (i + 1) == tabela.rows.length;
        FindControl('imgAdicionar', 'input', tabela.rows[i].cells[9]).style.display = isUltimaLinha ? '' : 'none';
        FindControl('imgRemover', 'input', tabela.rows[i].cells[9]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
    }
}
                    
function alteraTexto(textoOriginal, textoBuscar, textoAlterar) {
    var retorno = textoOriginal;
    var pos = 0;
                        
    if (retorno != null )
        while ((pos = retorno.indexOf(textoBuscar, pos)) > -1)
        {
            var inicio = retorno.substr(0, pos);
            var fim = retorno.substr(pos + textoBuscar.length);
    
            retorno = inicio + textoAlterar + fim;
            pos += textoAlterar.length;
        }
                        
    return retorno;
}
    
function adicionarLinha(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
    tabela.insertRow(tabela.rows.length);
    
    var pos = tabela.rows.length - 1;
    tabela.rows[pos].innerHTML = tabela.rows[0].innerHTML;
    
    var txtDescr = FindControl('txtDescr', 'input', tabela.rows[pos]);
    var imgPesq = FindControl('imgPesq', 'input', tabela.rows[pos]);
    var containerItemGenerico = FindControl('containerItemGenerico', 'span', tabela.rows[pos]);
    var txtDescricaoItemGenerico = FindControl('txtDescricaoItemGenerico', 'input', tabela.rows[pos]);
    var lblDescricaoProd = FindControl('lblDescricaoProd', 'span', tabela.rows[pos]);
    var hdfTipoCalculo = FindControl('hdfTipoCalculo', 'input', tabela.rows[pos]);
    var hdfValor = FindControl('hdfValor', 'input', tabela.rows[pos]);
    var txtQtde = FindControl('txtQtde', 'input', tabela.rows[pos]);
    var ctvValidaQtde = FindControl('ctvValidaQtde', 'span', tabela.rows[pos]);

    var txtProc = FindControl('txtProc', 'input', tabela.rows[pos]);
    var hdfIdProc = FindControl('hdfIdProc', 'input', tabela.rows[pos]);
    var txtApl = FindControl('txtApl', 'input', tabela.rows[pos]);
    var hdfIdApl = FindControl('hdfIdApl', 'input', tabela.rows[pos]);
    var psqProc = FindControl('', 'input', tabela.rows[pos].cells[5]);
    var psqApl = FindControl('', 'input', tabela.rows[pos].cells[8]);
                        
    var tabela = txtDescr;
    while (tabela.nodeName.toLowerCase() != 'table')
        tabela = tabela.parentNode;
                        
    var textoInicial = 'ctrlSelProduto', textoFinal = 'ctrlSelProduto' + pos;
    
    var controleAtual = txtDescr.id.substr(0, txtDescr.id.lastIndexOf('_'));
    controleAtual = controleAtual.substr(0, controleAtual.lastIndexOf('_'));
    
    var novoControle = alteraTexto(controleAtual, textoInicial, textoFinal);
                        
    tabela.id = alteraTexto(tabela.id, textoInicial, textoFinal);
    txtDescr.id = alteraTexto(txtDescr.id, textoInicial, textoFinal);
    txtDescr.setAttribute('onblur', alteraTexto(txtDescr.getAttribute('onblur'), textoInicial, textoFinal));
    imgPesq.setAttribute('onclick', alteraTexto(imgPesq.getAttribute('onclick'), textoInicial, textoFinal));
    containerItemGenerico.id = alteraTexto(containerItemGenerico.id, textoInicial, textoFinal);
    txtDescricaoItemGenerico.id = alteraTexto(txtDescricaoItemGenerico.id, textoInicial, textoFinal);
    lblDescricaoProd.id = alteraTexto(lblDescricaoProd.id, textoInicial, textoFinal);
    hdfTipoCalculo.id = alteraTexto(hdfTipoCalculo.id, textoInicial, textoFinal);
    
    hdfValor.id = alteraTexto(hdfValor.id, textoInicial, textoFinal);
    txtQtde.id = alteraTexto(txtQtde.id, 'txtQtde', 'txtQtde' + pos);
    ctvValidaQtde.id = alteraTexto(ctvValidaQtde.id, 'ctvValidaQtde', 'ctvValidaQtde' + pos);

    txtProc.id = alteraTexto(txtProc.id, 'txtProc', 'txtProc' + pos);
    hdfIdProc.id = alteraTexto(hdfIdProc.id, 'hdfIdProc', 'hdfIdProc' + pos);
    txtApl.id = alteraTexto(txtApl.id, 'txtApl', 'txtApl' + pos);
    hdfIdApl.id = alteraTexto(hdfIdApl.id, 'hdfIdApl', 'hdfIdApl' + pos);
    psqProc.id = pos;
    psqApl.id = pos;
                        
    txtDescr.value = '';
    lblDescricaoProd.innerHTML = '';
    hdfValor.value = '';
    txtQtde.value = '';
    hdfTipoCalculo.value = '';

    txtProc.value = '';
    txtApl.value = '';
    hdfIdProc.value = '';
    hdfIdApl.value = '';
    
    eval('window[\'' + novoControle + '\'] = ' + controleAtual + '.Clonar(\'' + novoControle + '\', \'' + textoInicial + '\', \'' + textoFinal + '\')');
    eval('window[\'' + novoControle + '_ctrlSelProdBuscar\'] = ' + controleAtual + '_ctrlSelProdBuscar.Clonar(\'' + novoControle + '_ctrlSelProdBuscar\', \'' + textoInicial + '\', \'' + textoFinal + '\')');
                        
    atualizaBotoes(nomeControle);
}
    
function removerLinha(nomeControle) {
    var tabela = document.getElementById(nomeControle + '_tabela');
                        
    // Retira do hiddenfield que salva os produtos associados este produto que está sendo removido
    var hdfIdProd = document.getElementById(nomeControle + '_hdfIdProd');
    var hdfQtde = document.getElementById(nomeControle + '_hdfQtde');
    hdfIdProd.value = hdfIdProd.value.slice(0, hdfIdProd.value.lastIndexOf(';'));
    hdfQtde.value = hdfQtde.value.slice(0, hdfQtde.value.lastIndexOf(';'));

    var hdfProc = document.getElementById(nomeControle + '_hdfProc');
    var hdfApl = document.getElementById(nomeControle + '_hdfApl');
    hdfProc.value = hdfProc.value.slice(0, hdfProc.value.lastIndexOf(';'));
    hdfApl.value = hdfApl.value.slice(0, hdfApl.value.lastIndexOf(';'));
    
    tabela.deleteRow(tabela.rows.length - 1);
    atualizaBotoes(nomeControle);
}
    
function selProdutoCallback(nomeControleProd, idProd) {

    var tabela = document.getElementById(nomeControleProd.replace('_ctrlSelProduto', '_txtQtde')).parentNode;
    while (tabela.nodeName.toLowerCase() != 'table')
        tabela = tabela.parentNode;
                        
    var idsProd = new Array();
    var qtdes = new Array();
    var procs = new Array();
    var apls = new Array();

    for (i = 0; i < tabela.rows.length; i++)
    {
        var idProd = FindControl('hdfValor', 'input', tabela.rows[i].cells[0]).value;
        var qtd = FindControl('txtQtde', 'input', tabela.rows[i].cells[2]).value;
        var proc = FindControl('hdfIdProc', 'input', tabela.rows[i].cells[4]).value;
        var apl = FindControl('hdfIdApl', 'input', tabela.rows[i].cells[7]).value;
                            
        var add = true;
        for (j = 0; j < idsProd.length; j++)
            if (idsProd[j] == idProd)
            {
                add = false;
                break;
            }
                            
        if (add)
        {
            idsProd.push(idProd);
            qtdes.push(qtd);
            procs.push(proc);
            apls.push(apl);
        }
    }
                        
    var nomeControle = tabela.id.substr(0, tabela.id.lastIndexOf('_'));
    document.getElementById(nomeControle + '_hdfIdProd').value = idsProd.join(';');
    document.getElementById(nomeControle + '_hdfQtde').value = qtdes.join(';');
    document.getElementById(nomeControle + '_hdfProc').value = procs.join(';');
    document.getElementById(nomeControle + '_hdfApl').value = apls.join(';');
}
    
function carregaProdInicial(nomeControle, codigos, descricoes)
{
    var tabela = document.getElementById(nomeControle + '_tabela');
    var idProd = document.getElementById(nomeControle + '_hdfIdProd').value;
    var qtde = document.getElementById(nomeControle + '_hdfQtde').value;
    var proc = document.getElementById(nomeControle + '_hdfProc').value;
    var apl = document.getElementById(nomeControle + '_hdfApl').value;
                        
    var numeroItens = idProd.split(';').length;
    for (iLoadProd = 0; iLoadProd < numeroItens; iLoadProd++)
    {
        if (iLoadProd > 0)
            adicionarLinha(nomeControle);

        FindControl('hdfValor', 'input', tabela.rows[iLoadProd].cells[0]).value = idProd.split(';')[iLoadProd];
        FindControl('txtQtde', 'input', tabela.rows[iLoadProd].cells[2]).value = qtde.split(';')[iLoadProd].replace(".", ",");
        FindControl('txtDescr', 'input', tabela.rows[iLoadProd].cells[0]).value = codigos[iLoadProd];
        FindControl('lblDescricaoProd', 'span', tabela.rows[iLoadProd].cells[0]).innerHTML = descricoes[iLoadProd];

        var idProc = proc.split(';')[iLoadProd] != null ? proc.split(';')[iLoadProd].replace(".", ",") : "";
        var idApl = apl.split(';')[iLoadProd] != null ? apl.split(';')[iLoadProd].replace(".", ",") : "";

        if (idProc != "" && idProc != "0") {
            var codInternoProc = MetodosAjax.GetCodInternoEtiqProcesso(idProc);

            if (codInternoProc.error != null) {
                alert(codInternoProc.error.description);
                return;
            }

            FindControl('hdfIdProc', 'input', tabela.rows[iLoadProd].cells[4]).value = idProc;
            FindControl('txtProc', 'input', tabela.rows[iLoadProd].cells[4]).value = codInternoProc.value;
        }

        if (idApl != "" && idApl != "0") {
            var codInternoApl = MetodosAjax.GetCodInternoEtiqAplicacao(idApl);

            if (codInternoApl.error != null) {
                alert(codInternoApl.error.description);
                return;
            }


            FindControl('hdfIdApl', 'input', tabela.rows[iLoadProd].cells[7]).value = idApl;
            FindControl('txtApl', 'input', tabela.rows[iLoadProd].cells[7]).value = codInternoApl.value;
        }
    }
}
                    
function validaQtdeProdutoBaixa(val, args)
{
    var nomeControle = val.id.substr(0, val.id.lastIndexOf('_'));
    var index = val.id.substr(val.id.lastIndexOf('_') + 1);
    index = index.substr('ctvValidaQtde'.length);
    index = index != '' ? index : '0';
    
    selProdutoCallback(nomeControle + '_txtQtde', 0);
                        
    var idProd = document.getElementById(nomeControle + '_hdfIdProd').value.split(';');
    var qtde = document.getElementById(nomeControle + '_hdfQtde').value.split(';');
                        
    args.IsValid = idProd[index] == '' || parseFloat(qtde[index].replace(',', '.')) > 0;
}

var ctrlProdutoBaixaEst = new (function () {

    var vm = this;

    vm.AtualizaVisibilidadeProcApl = function (idSubGrupoProd) {

        if (idSubGrupoProd == "")
            return;

        var subgrupoLaminado = ctrlProdutoBaixaEstoque.VerificaSubgrupoLaminado(idSubGrupoProd);

        if (subgrupoLaminado.error != null) {
            alert('Falha ao verificar subgrupo do tipo vidro laminado. ' + subgrupoLaminado.error.description);
            $(".tdProcApl_ctrlProdutoBaixaEstoque1").css("display", "none");
            return;
        }

        if (subgrupoLaminado.value)
        {
            $(".tdProcApl_ctrlProdutoBaixaEstoque1").css("display", "");
        }
        else
        {
            $(".tdProcApl_ctrlProdutoBaixaEstoque1").css("display", "none");

            var tabela = document.getElementById("ctl00_ctl00_Pagina_Conteudo_dtvProduto_ctrlProdutoBaixaEstoque1_tabela");
            var div = FindControl("divProdBaixaEstoque", "div");

            FindControl("hdfProc", "input", div).value = "";
            FindControl("hdfApl", "input", div).value = "";

            for (i = 0; i < tabela.rows.length; i++) {
                FindControl('hdfIdProc', 'input', tabela.rows[i].cells[4]).value = "";
                FindControl('txtProc', 'input', tabela.rows[i].cells[4]).value = "";
                FindControl('hdfIdApl', 'input', tabela.rows[i].cells[7]).value = "";
                FindControl('txtApl', 'input', tabela.rows[i].cells[7]).value = "";
            }
        }
    }

    vm.loadProc = function (codInterno, idControle) {

        if (codInterno == "")
            return;

        var response = MetodosAjax.GetEtiqProcesso(codInterno);

        if (response.error != null || response.value.split("\t")[0] == "Erro") {
            alert(response.error != null ? response.error.description : response.value[1]);
            return;
        }

        vm.setProc(response.value.split("\t")[1], response.value.split("\t")[2], response.value.split("\t")[3]);

        if (response.value.split("\t")[3] != "")
            vm.loadApl(response.value.split("\t")[3], idControle);

    }

    vm.setProc = function (idProcesso, codInterno, codAplicacao, idControle) {

        idControle = idControle.replace('-', '');

        $('#divProdBaixaEstoque #hdfIdProc' + idControle).val(idProcesso);
        $('#divProdBaixaEstoque #txtProc' + idControle).val(codInterno);

        if (codAplicacao != null)
            vm.loadApl(codAplicacao, idControle);

        selProdutoCallback('ctl00_ctl00_Pagina_Conteudo_dtvProduto_ctrlProdutoBaixaEstoque1_txtQtde', 0);
    }

    vm.loadApl = function(codInterno, idControle){

        if (codInterno == "")
            return;

        var response = MetodosAjax.GetEtiqAplicacao(codInterno);

        if (response.error != null || response.value.split("\t")[0] == "Erro") {
            alert(response.error != null ? response.error.description : response.value.split("\t")[1]);
            return;
        }

        vm.setApl(response.value.split("\t")[1], response.value.split("\t")[2], idControle);
    }

    vm.setApl = function(idAplicacao, codInterno, idControle){

        idControle = idControle.replace('-', '');

        $('#divProdBaixaEstoque #hdfIdApl' + idControle).val(idAplicacao);
        $('#divProdBaixaEstoque #txtApl' + idControle).val(codInterno);

        selProdutoCallback('ctl00_ctl00_Pagina_Conteudo_dtvProduto_ctrlProdutoBaixaEstoque1_txtQtde', 0);
    }

})();