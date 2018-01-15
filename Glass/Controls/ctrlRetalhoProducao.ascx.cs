using System;
using System.Collections.Generic;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlRetalhoProducao : BaseUserControl
    {
        public List<RetalhoProducaoAuxiliar> Dados
        {
            get
            {
                if (!PCPConfig.Etiqueta.UsarControleRetalhos)
                    return new List<RetalhoProducaoAuxiliar>();
    
                string[] altura = hdfAltura.Value.Split(';');
                string[] largura = hdfLargura.Value.Split(';');
                string[] qtde = hdfQtde.Value.Split(';');
                string[] observacao = !string.IsNullOrEmpty(hdfObservacao.Value) ? hdfObservacao.Value.Split(';') : null;

                List<RetalhoProducaoAuxiliar> retorno = new List<RetalhoProducaoAuxiliar>();
    
                for (int i = 0; i < altura.Length; i++)
                {
                    if (Conversoes.ConverteValor<decimal>(altura[i]) > 0 && Conversoes.ConverteValor<decimal>(largura[i]) > 0 &&
                        Conversoes.ConverteValor<int>(qtde[i]) > 0)
                    {
                        retorno.Add(new RetalhoProducaoAuxiliar(Convert.ToUInt32(i + 1), Conversoes.ConverteValor<decimal>(altura[i]),
                            Conversoes.ConverteValor<decimal>(largura[i]), Conversoes.ConverteValor<int>(qtde[i]),
                            observacao != null && observacao.Length > 0 && observacao.Length >= i - 1 ? observacao[i] : null));
                    }
                }
    
                return retorno;
            }
    
            set
            {
                if (value.Count == 0)
                    return;
    
                string altura = "";
                string largura = "";
                string qtde = "";
                var observacao = "";

                foreach (RetalhoProducaoAuxiliar r in value)
                {
                    altura += "'" + r.Altura + "',";
                    largura += "'" + r.Largura + "',";
                    qtde += "'" + r.Quantidade + "',";
                    observacao += "'" + r.Observacao + "',";
                }
    
                Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                    string.Format("carregaRetalhoInicial('{0}', new Array({1}), new Array({2}), new Array({3}), new Array({4}));",
                         this.ClientID, altura.TrimEnd(',', ' '), largura.TrimEnd(',', ' '), qtde.TrimEnd(',', ' '), observacao.TrimEnd(',', ' ')), true);
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlRetalhoProducao_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlRetalhoProducao_script", @"
    
                    function inserir() {
    
                        var nomeControle = '" + this.ClientID + @"';
                        var tabela = document.getElementById(nomeControle + '_tblRetalhos');
                        var $retalhos = $('input:hidden[id$=hdfRetalhos]');
                        var items = [];
                        for (i = 0; tabela.rows.length > i; i++)
                        {
                            var altura = FindControl('txtAltura', 'input', tabela.rows[i]).value;
                            var largura = FindControl('txtLargura', 'input', tabela.rows[i]).value;
                            var qtd = FindControl('txtQuantidade', 'input', tabela.rows[i]).value;
                            var observacao = FindControl('txtObservacao', 'input', tabela.rows[i]).value;
    
                            var $retalho = new Object();
                            $retalho.IdRetalhoProducao = i + 1;
                            $retalho.Altura = altura;
                            $retalho.Largura = largura;
                            $retalho.Quantidade = qtd;
                            $retalho.Observacao = observacao;
    
                            items.push($retalho);
                        }
    
                        $retalhos.val(JSON.stringify(items));
                    }
    
                    function atualizaDados() {
                        var nomeControle = '" + this.ClientID + @"';
    
                        var tabela = document.getElementById(nomeControle + '_tblRetalhos');
    
                        var altura = document.getElementById(nomeControle + '_hdfAltura').value;
                        var largura = document.getElementById(nomeControle + '_hdfLargura').value;
                        var qtde = document.getElementById(nomeControle + '_hdfQtde').value;
                        var observacao = document.getElementById(nomeControle + '_hdfObservacao').value;
                        
                        var numeroItens = altura.split(';').length;
                        for (iLoadProd = 0; iLoadProd < numeroItens; iLoadProd++)
                        {
                            if (iLoadProd > 0) adicionarLinha(nomeControle);
                            if (FindControl('txtAltura', 'input', tabela.rows[iLoadProd]) == null)
                                continue;
    
                            FindControl('txtAltura', 'input', tabela.rows[iLoadProd]).value = altura.split(';')[iLoadProd];
                            FindControl('txtLargura', 'input', tabela.rows[iLoadProd]).value = largura.split(';')[iLoadProd];
                            FindControl('txtQuantidade', 'input', tabela.rows[iLoadProd]).value = qtde.split(';')[iLoadProd];
                            FindControl('txtObservacao', 'input', tabela.rows[iLoadProd]).value = observacao.split(';')[iLoadProd];
                        }
    
                        inserir();
                    }
    
                    function limpaDadosRetalho() {
                        var tabela = document.getElementById('" + this.ClientID + @"_tblRetalhos');
                        for (i = 0; i < tabela.rows.length; i++) {
                            if (FindControl('txtAltura', 'input', tabela.rows[i]) == null)
                                continue;

                            FindControl('txtAltura', 'input', tabela.rows[i]).value = '';
                            FindControl('txtLargura', 'input', tabela.rows[i]).value = '';
                            FindControl('txtQuantidade', 'input', tabela.rows[i]).value = '';
                            FindControl('txtObservacao', 'input', tabela.rows[i]).value = '';
                        }
                    }
    
                    function atualizaBotoes(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tblRetalhos');
                        for (i = 0; i < tabela.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabela.rows.length;
                            FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }
                    
                    function alteraTexto(textoOriginal, textoBuscar, textoAlterar) {
                        var retorno = textoOriginal;
                        var pos = 0;
                        
                        while ((pos = retorno.indexOf(textoBuscar, pos)) > -1)
                        {
                            var inicio = retorno.substr(0, pos);
                            var fim = retorno.substr(pos + textoBuscar.length);
    
                            retorno = inicio + textoAlterar + fim;
                            pos += textoAlterar.length;
                        }
                        
                        return retorno;
                    }
    
                    function validar(tabela) {
                        var pos = tabela.rows.length - 1;
                        var valida = FindControl('txtAltura', 'input', tabela.rows[pos]).value != '' && 
                            FindControl('txtLargura', 'input', tabela.rows[pos]).value != '' && 
                            FindControl('txtQuantidade', 'input', tabela.rows[pos]).value != '';
    
                        if(!valida) {
                            alert('O preenchimento de altura, largura e quantidade é obrigatório.');
                            return false;
                        }
                        
                        return true;
                    }
                    
                    function adicionarLinha(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tblRetalhos');   
    
                        if(validar(tabela)) {
             
                            tabela.insertRow(tabela.rows.length);
                            tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
    
                            var pos = tabela.rows.length - 1;
                            var txtAltura = FindControl('txtAltura', 'input', tabela.rows[pos]);
                            var txtLargura = FindControl('txtLargura', 'input', tabela.rows[pos]);
                            var txtQuantidade = FindControl('txtQuantidade', 'input', tabela.rows[pos]);
                            var txtObservacao = FindControl('txtObservacao', 'input', tabela.rows[pos]);
                            
                            var tabela = txtAltura;
                            while (tabela.nodeName.toLowerCase() != 'table')
                                tabela = tabela.parentNode;
                            
                            var textoInicial = 'ctrlRetalhoProducao', textoFinal = 'ctrlRetalhoProducao' + pos;
                            
                            //tabela.id = alteraTexto(tabela.id, textoInicial, textoFinal);
                            txtAltura.id = alteraTexto(txtAltura.id, textoInicial, textoFinal);
                            txtAltura.setAttribute('onchange', alteraTexto(txtAltura.getAttribute('onchange'), textoInicial, textoFinal));
                            txtLargura.id = alteraTexto(txtLargura.id, textoInicial, textoFinal);
                            txtLargura.setAttribute('onchange', alteraTexto(txtLargura.getAttribute('onchange'), textoInicial, textoFinal));
                            txtQuantidade.id = alteraTexto(txtQuantidade.id, textoInicial, textoFinal);
                            txtQuantidade.setAttribute('onchange', alteraTexto(txtQuantidade.getAttribute('onchange'), textoInicial, textoFinal));
                            txtObservacao.id = alteraTexto(txtObservacao.id, textoInicial, textoFinal);
                            txtObservacao.setAttribute('onchange', alteraTexto(txtObservacao.getAttribute('onchange'), textoInicial, textoFinal));
                            
                            txtAltura.value = '';
                            txtLargura.value = '';
                            txtQuantidade.value = '';
                            txtObservacao.value = '';
                            
                            atualizaBotoes(nomeControle);
                        }
                    }
    
                    function removerLinha(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tblRetalhos');
                        tabela.deleteRow(tabela.rows.length - 1);
                        atualizaBotoes(nomeControle);
    
                        inserir();
                    }
                    
                    function addDataCallback(nomeControle, idProd) {
                        var tabela = document.getElementById(nomeControle).parentNode;
                        while (tabela.nodeName.toLowerCase() != 'table')
                            tabela = tabela.parentNode;
                        
                        var alturas = new Array();
                        var larguras = new Array();
                        var qtdes = new Array();
                        var observacoes = new Array();
    
                        for (i = 0; i < tabela.rows.length; i++)
                        {
                            var altura = FindControl('txtAltura', 'input', tabela.rows[i]).value;
                            var largura = FindControl('txtLargura', 'input', tabela.rows[i]).value;
                            var qtd = FindControl('txtQuantidade', 'input', tabela.rows[i]).value;
                            var observacao = FindControl('txtObservacao', 'input', tabela.rows[i]).value;

                            alturas.push(altura);
                            larguras.push(largura);
                            qtdes.push(qtd);
                            observacoes.push(observacao);
                        }
                        
                        var nomeControle = tabela.id.substr(0, tabela.id.lastIndexOf('_'));
                        document.getElementById(nomeControle + '_hdfAltura').value = alturas.join(';');
                        document.getElementById(nomeControle + '_hdfLargura').value = larguras.join(';');
                        document.getElementById(nomeControle + '_hdfQtde').value = qtdes.join(';');
                        document.getElementById(nomeControle + '_hdfObservacao').value = observacoes.join(';');
    
                        inserir();
                    }
    
                    function carregaRetalhoInicial(nomeControle, alturas, larguras, quantidades, observacoes)
                    {
                        var tabela = document.getElementById(nomeControle + '_tblRetalhos');
    
                        var altura = document.getElementById(nomeControle + '_hdfAltura').value;
                        var largura = document.getElementById(nomeControle + '_hdfLargura').value;
                        var qtde = document.getElementById(nomeControle + '_hdfQtde').value;
                        var observacao = document.getElementById(nomeControle + '_hdfObservacao').value;
                        
                        var numeroItens = altura.split(';').length;
                        for (iLoadProd = 0; iLoadProd < numeroItens; iLoadProd++)
                        {
                            if (iLoadProd > 0) adicionarLinha(nomeControle);
        
                            FindControl('txtAltura', 'input', tabela.rows[iLoadProd]).value = altura.split(';')[iLoadProd];
                            FindControl('txtLargura', 'input', tabela.rows[iLoadProd]).value = largura.split(';')[iLoadProd];
                            FindControl('txtQuantidade', 'input', tabela.rows[iLoadProd]).value = qtde.split(';')[iLoadProd];
                            FindControl('txtObservacao', 'input', tabela.rows[iLoadProd]).value = observacao.split(';')[iLoadProd];
                        }
                    }
                    ", true);
            }
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinha('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinha('" + this.ClientID + "'); return false";
            }
    
            // Esconde o controle se não usar controle de retalhos
            if (!IsPostBack)
                this.PreRender += delegate(object s, EventArgs a)
                {
                    this.Visible = PCPConfig.Etiqueta.UsarControleRetalhos;
                };
        }
    }
}
