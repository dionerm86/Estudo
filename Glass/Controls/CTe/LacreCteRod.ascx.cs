using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class LacreCteRod : CteBaseUserControl
    {
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.LacreCteRod> ObjLacreCteRod
        {
            get
            {
                var listaLacre = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.LacreCteRod>();
    
                string[] numLacre = hdfNumLacre.Value.Split(';');
    
                for (int i = 0; i < numLacre.Length; i++)
                {
                    if (numLacre[i] != "")
                    {
                        listaLacre.Add(
                            new WebGlass.Business.ConhecimentoTransporte.Entidade.LacreCteRod(
                                new Glass.Data.Model.Cte.LacreCteRod
                                {
                                    NumeroLacre = numLacre[i]
                                }));
                    }
                }
                return listaLacre;
            }
            set
            {
                string numLacre = string.Empty;
    
                foreach (var i in value)
                {
                    numLacre = numLacre + ";" + i.NumeroLacre;
                }
    
                if (!string.IsNullOrEmpty(numLacre))
                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
             string.Format("carregaNumLacreInicial('{0}', '{1}');", this.ClientID, numLacre), true);
    
                imgAdicionar.OnClientClick = "adicionarLinhaLacre('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinha('" + this.ClientID + "'); return false";
            }
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            txtNumLacre.Attributes.Add("onblur", "pegarValor('" + this.ClientID + "')");
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "LacreCteRod_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "LacreCteRod_script", @"
                    function atualizaBotoes(nomeControle) {
                        var tabela_lacre = document.getElementById(nomeControle + '_tabela_lacre');
                        for (i = 0; i < tabela_lacre.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabela_lacre.rows.length;
                            FindControl('imgAdicionar', 'input', tabela_lacre.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabela_lacre.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }
    
                    function corrigeValidadoresLacre(nomeControle) {
                        var tabela_lacre = document.getElementById(nomeControle + '_tabela_lacre');
                        if (tabela_lacre.rows.length <= 1)
                            return;
    
                        var complID = '_' + (tabela_lacre.rows.length - 1);
                        var linhaTabela = tabela_lacre.rows[tabela_lacre.rows.length - 1];
    
                        var validadores = ['rfvValidaQtde'];
                        var controles = ['txtNumLacre'];
    
                        for (var v in validadores)
                        {
                            var val = FindControl(validadores[v], 'span', linhaTabela);
                            if (!val) continue;
    
                            var nomeAtual = val.id;
    
                            if (val.id.lastIndexOf(complID) != (val.id.length - complID.length))
                                val.id += complID;
    
                            ajustaValidador(val, nomeAtual, controles[v], complID);
    
                            var controle = FindControl(controles[v], 'input', linhaTabela);
                            if (!controle)
                                controle = FindControl(controles[v], 'select', linhaTabela);
    
                            if (!!controle) {
                                if (controle.id.lastIndexOf(complID) != (controle.id.length - complID.length)) {
                                    controle.id += complID;
                                    controle.name += complID.replace(/_/g, '$');
                                }
    
                                ValidatorHookupControl(controle, val);
                            }
                        }
                    }
                    
                    function adicionarLinhaLacre(nomeControle) {                    
    
                        var tabela_lacre = document.getElementById(nomeControle + '_tabela_lacre');
    
                        if(tabela_lacre.rows.length == 20)
                        {
                            alert('São permitidas, no máximo, 20 registros de número de lacre.');
                            return;
                        }
                        if(FindControl('txtNumLacre', 'input', tabela_lacre.rows[tabela_lacre.rows.length - 1]).value == '')
                        {
                            alert('para criar nova linha, a atual deve estar preenchida.');
                            return;
                        }
                        tabela_lacre.insertRow(tabela_lacre.rows.length);
                        tabela_lacre.rows[tabela_lacre.rows.length - 1].innerHTML = tabela_lacre.rows[0].innerHTML;
    
                        var pos = tabela_lacre.rows.length - 1;
                        var txtDescr = FindControl('txtNumLacre', 'input', tabela_lacre.rows[pos]);
                        var hdfNumLacre = FindControl('hdfNumLacre', 'input', tabela_lacre.rows[pos]);
    
                        atualizaBotoes(nomeControle);
                        corrigeValidadoresLacre(nomeControle);
                    }
    
                    function pegarValor(nomeControleNumLacre)
                    {
                        var tabela_lacre = document.getElementById(nomeControleNumLacre + '_tabela_lacre');
                        var b = '';
                        for(var i = 0; i < tabela_lacre.rows.length; i++)
                        {
                            if(FindControl('txtNumLacre', 'input', tabela_lacre.rows[i]).value != '')
                            {
                                b = b +';'+ FindControl('txtNumLacre', 'input', tabela_lacre.rows[i]).value;
                            }
                        }           
                        FindControl('hdfNumLacre', 'input').value = b;
                    }
    
                    function removerLinha(nomeControle) {
                        var tabela_lacre = document.getElementById(nomeControle + '_tabela_lacre');
                        tabela_lacre.deleteRow(tabela_lacre.rows.length - 1);                    
                        atualizaBotoes(nomeControle);
                        corrigeValidadoresLacre(nomeControle);
                        pegarValor(nomeControle);
                    }                
    
                    function carregaNumLacreInicial(nomeControle, valor)
                    {
                        var tabela_lacre = document.getElementById(nomeControle + '_tabela_lacre');
                        
                        if(valor.indexOf(';') > -1)
                        {
                            var item = valor.split(';');
                            var primeiroItem = true;
                            for(var i = 0; i < item.length; i++)
                            {
                                if(item[i] != '')
                                {
                                    if(primeiroItem)
                                    {
                                        FindControl('txtNumLacre', 'input', tabela_lacre.rows[tabela_lacre.rows.length - 1]).value = item[i];
                                        primeiroItem = false;
                                    }
                                    else
                                    {
                                        tabela_lacre.insertRow(tabela_lacre.rows.length);
                                        tabela_lacre.rows[tabela_lacre.rows.length - 1].innerHTML = tabela_lacre.rows[0].innerHTML;
                                        FindControl('txtNumLacre', 'input', tabela_lacre.rows[tabela_lacre.rows.length - 1]).value = item[i];
                                
                                        atualizaBotoes(nomeControle);
                                    }
                                }
                            }
                        }
                        else
                        {
                            FindControl('txtNumLacre', 'input', tabela_lacre.rows[0]).value = valor;
                        }
                        
                    }", true);
            }
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinhaLacre('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinha('" + this.ClientID + "'); return false";
            }
        }
    }
}
