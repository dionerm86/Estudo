using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;


namespace Glass.UI.Web.Controls.CTe
{
    public partial class VeiculoCte : CteBaseUserControl
    {
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.VeiculoCte> ObjVeiculoCte
        {
            get
            {
                var listaVeiculos = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.VeiculoCte>();
    
                string[] placa = hdfPlaca.Value.Split(';');
                //string[] valorFrete = hdfValorFrete.Value.Split(';');            
    
                for (int i = 0; i < placa.Length; i++)
                {
                    if (placa[i] != "" && placa[i] != "selecione")
                    {
                        listaVeiculos.Add(
                            new WebGlass.Business.ConhecimentoTransporte.Entidade.VeiculoCte(
                                new Glass.Data.Model.Cte.VeiculoCte
                                {
                                    Placa = placa[i]
                                    //ValorFrete = Glass.Conversoes.StrParaDecimal(valorFrete[i])                                
                                }));
                    }
                }
                return listaVeiculos;            
            }
            set
            {
                string placa = string.Empty;
                //string valorFrete = string.Empty;            
    
                foreach (var i in value)
                {
                    placa = placa + ";" + i.Placa;
                    //valorFrete = valorFrete + ";" + i.ValorFrete.ToString();                
                }
                Page.ClientScript.RegisterStartupScript(GetType(), "iniciarasdas",
                    string.Format("carregaVeiculoInicial('{0}', '{1}');", this.ClientID, placa), true);
    
                imgAdicionar.OnClientClick = "adicionarLinhaVeiculo('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaVeiculo('" + this.ClientID + "'); return false";
            }        
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            drpPlaca.Attributes.Add("onblur", "pegarValorVeiculo('" + this.ClientID + "')");
            //txtValorFrete.Attributes.Add("onblur", "pegarValorVeiculo('" + this.ClientID + "')");
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "VeiculoCte_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "VeiculoCte_script", @"
                    function atualizaBotoesVeiculo(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        for (i = 0; i < tabela.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabela.rows.length;
                            FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }                              
    
                    function adicionarLinhaVeiculo(nomeControle) {
    
                        var tabela = document.getElementById(nomeControle + '_tabela');
    
                        if(tabela.rows.length == 4)
                            alert('São permitidas, no máximo, 4 registros de veículos');
        
                        if(FindControl('drpPlaca', 'select', tabela.rows[tabela.rows.length - 1]).value == 'selecione')
                        {
                          alert('para criar nova linha, a atual deve estar preenchida');
                        }
                        if(tabela.rows.length == 4)
                        {
                          alert('Não é possível referenciar mais de 4 veículos.');
                        }                    
                        else
                        {                        
                            tabela.insertRow(tabela.rows.length);
                            tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                            
                            FindControl('drpPlaca', 'select', tabela.rows[tabela.rows.length - 2]).disabled = true;
                            
                            FindControl('drpPlaca', 'select', tabela.rows[tabela.rows.length - 1])[FindControl('drpPlaca', 'select', tabela.rows[tabela.rows.length - 2]).selectedIndex].remove();
    
                            FindControl('drpPlaca', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;
                            
                            atualizaBotoesVeiculo(nomeControle);
                        }
                    }
    
                    function pegarValorVeiculo(nomeControle)
                    {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        var placa = '';
                        var valorFrete = '';
    
                        for(var i = 0; i < tabela.rows.length; i++)
                        {
                            if(FindControl('drpPlaca', 'select', tabela.rows[i]).value != '')
                            {
                                placa = placa +';'+ FindControl('drpPlaca', 'select', tabela.rows[i]).value;                                                    
                            }
                        }           
                        FindControl('hdfPlaca', 'input').value = placa;                    
                    }
    
                    function removerLinhaVeiculo(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        tabela.deleteRow(tabela.rows.length - 1);                    
                        atualizaBotoesVeiculo(nomeControle);
                        
                        FindControl('drpPlaca', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;                    
                        
                        pegarValorVeiculo(nomeControle);
                    }                
    
                    function carregaVeiculoInicial(nomeControle, placa)
                    {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        
                        if(placa.indexOf(';') > -1)
                        {
                            var _placa = placa.split(';');
                            var primeiroItem = true;
                            for(var i = 0; i < _placa.length; i++)
                               {
                                  if(_placa[i] != '')
                                  {
                                     if(primeiroItem)
                                     {
                                        FindControl('drpPlaca', 'select', tabela.rows[tabela.rows.length - 1]).value = _placa[i];
                                        primeiroItem = false;
                                     }
                                     else
                                     {
                                        tabela.insertRow(tabela.rows.length);
                                        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                                        FindControl('drpPlaca', 'select', tabela.rows[i - 1]).value = _placa[i];
    
                                        atualizaBotoesVeiculo(nomeControle);
                                     }
                                  }
                                }
                        }
                        else
                        {
                            FindControl('drpPlaca', 'select', tabela.rows[tabela.rows.length - 1]).value = placa[i];                        
                        }
                        
                    }", true);
            }
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinhaVeiculo('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaVeiculo('" + this.ClientID + "'); return false";
            }
        }
    }
}
