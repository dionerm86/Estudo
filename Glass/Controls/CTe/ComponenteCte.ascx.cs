using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class ComponenteCte : CteBaseUserControl
    {
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.ComponenteValorCte> ObjComponenteValorCte
        {
            get
            {
                var listaComponente = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.ComponenteValorCte>();
    
                string[] nomeComponente = hdfNomeComponente.Value.Split(';');
                string[] valorComponente = hdfValorComponente.Value.Split(';');            
    
                for (int i = 0; i < nomeComponente.Length; i++)
                {
                    if (nomeComponente[i] != "")
                    {
                        listaComponente.Add(
                            new WebGlass.Business.ConhecimentoTransporte.Entidade.ComponenteValorCte(
                                new Glass.Data.Model.Cte.ComponenteValorCte
                                {
                                    NomeComponente = nomeComponente[i],
                                    ValorComponente = Glass.Conversoes.StrParaDecimal(valorComponente[i])                                
                                }));
                    }
                }
                return listaComponente;            
            }
            set
            {
                string nomeComponente = string.Empty;
                string valorComponente = string.Empty;            
    
                foreach (var i in value)
                {
                    nomeComponente = nomeComponente + ";" + i.NomeComponente;
                    valorComponente = valorComponente + ";" + i.ValorComponente.ToString();                
                }
    
                Page.ClientScript.RegisterStartupScript(GetType(), "iniciar", 
                    string.Format("carregaComponenteInicial('{0}', '{1}', '{2}');", 
                    this.ClientID, nomeComponente, valorComponente), true);
    
                imgAdicionar.OnClientClick = "adicionarLinhaComponente('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaComponente('" + this.ClientID + "'); return false";
            }
        }
    
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            txtNomeComponente.Attributes.Add("onblur", "pegarValorComponente('" + this.ClientID + "')");
            txtValorComponente.Attributes.Add("onblur", "pegarValorComponente('" + this.ClientID + "')");
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ComponenteCte_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ComponenteCte_script", @"
                    function atualizaBotoesComponente(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        for (i = 0; i < tabela.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabela.rows.length;
                            FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }                              
    
                    function adicionarLinhaComponente(nomeControle) {
    
                        var tabela = document.getElementById(nomeControle + '_tabela');
        
                        if(FindControl('txtNomeComponente', 'input', tabela.rows[tabela.rows.length - 1]).value == '' ||
                           FindControl('txtValorComponente', 'input', tabela.rows[tabela.rows.length - 1]).value == '' )
                        {
                          alert('para criar nova linha, a atual deve estar preenchida');
                        }
                        else
                        {                        
                            tabela.insertRow(tabela.rows.length);
                            tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                            
                            FindControl('txtNomeComponente', 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;
                            FindControl('txtValorComponente', 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;
    
                            FindControl('txtNomeComponente', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
                            FindControl('txtValorComponente', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;                        
                            
                            atualizaBotoesComponente(nomeControle);
                        }
                    }
    
                    function pegarValorComponente(nomeControle)
                    {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        var nomeComponente = '';
                        var valorComponente = '';
    
                        for(var i = 0; i < tabela.rows.length; i++)
                        {
    
                            if(FindControl('txtNomeComponente', 'input', tabela.rows[i]).value != '' &&
                               FindControl('txtValorComponente', 'input', tabela.rows[i]).value != '')
                            {
                                nomeComponente = nomeComponente +';'+ FindControl('txtNomeComponente', 'input', tabela.rows[i].cells[1]).value;
                            
                                valorComponente = valorComponente +';'+ FindControl('txtValorComponente', 'input', tabela.rows[i].cells[3]).value;                        
                            }
                        }           
                        FindControl('hdfNomeComponente', 'input').value = nomeComponente;
                        FindControl('hdfValorComponente', 'input').value = valorComponente;
                    }
    
                    function removerLinhaComponente(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        tabela.deleteRow(tabela.rows.length - 1);                    
                        atualizaBotoesComponente(nomeControle);
                        
                        FindControl('txtNomeComponente', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
                        FindControl('txtValorComponente', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;                    
                        
                        pegarValorComponente(nomeControle);
                    }                
    
                    function carregaComponenteInicial(nomeControle, nomeComponente, valorComponente)
                    {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        
                        if(nomeComponente.indexOf(';') > -1)
                        {
                            var _nomeComponente = nomeComponente.split(';');
                            var _valorComponente = valorComponente.split(';');
                            var primeiroItem = true;
                            for(var i = 0; i < _nomeComponente.length; i++)
                               {
                                  if(_nomeComponente[i] != '')
                                  {
                                     if(primeiroItem)
                                     {
                                        FindControl('txtNomeComponente', 'input', tabela.rows[tabela.rows.length - 1]).value = _nomeComponente[i];
                                        FindControl('txtValorComponente', 'input', tabela.rows[tabela.rows.length - 1]).value = _valorComponente[i];                                    
                                        primeiroItem = false;
                                     }
                                     else
                                     {
                                        tabela.insertRow(tabela.rows.length);
                                        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                                        FindControl('txtNomeComponente', 'input', tabela.rows[i - 1]).value = _nomeComponente[i];
                                        FindControl('txtValorComponente', 'input', tabela.rows[i - 1]).value = _valorComponente[i];
    
                                        atualizaBotoesComponente(nomeControle);
                                     }
                                  }
                                }
                        }
                        else
                        {
                            FindControl('txtNomeComponente', 'input', tabela.rows[tabela.rows.length - 1]).value = nomeComponente;
                            FindControl('txtValorComponente', 'input', tabela.rows[tabela.rows.length - 1]).value = valorComponente;                        
                        }
                        
                    }", true);
            }
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinhaComponente('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaComponente('" + this.ClientID + "'); return false";
            }
        }
    }
}
