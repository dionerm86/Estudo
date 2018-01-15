using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class MotoristaCteRod : CteBaseUserControl
    {
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.MotoristaCteRod> ObjMotoristaCteRod
        {
            get
            {
                var listaMotorista = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.MotoristaCteRod>();
    
                string[] idFunc = hdfIdFunc.Value.Split(';');            
    
                for (int i = 0; i < idFunc.Length; i++)
                {
                    if (idFunc[i] != "")
                    {
                        listaMotorista.Add(
                            new WebGlass.Business.ConhecimentoTransporte.Entidade.MotoristaCteRod(
                                new Glass.Data.Model.Cte.MotoristaCteRod
                                {
                                    IdFunc = Glass.Conversoes.StrParaUint(idFunc[i])
                                }));
                    }
                }
                return listaMotorista;
            }
            set
            {
                string idFunc = string.Empty;            
    
                foreach (var i in value)
                {
                    idFunc = idFunc + ";" + i.IdFunc;                
                }
    
                if(value.Count > 0)
                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaMotoristaInicial('{0}', '{1}');",
                        this.ClientID, idFunc), true);
    
                imgAdicionar.OnClientClick = "adicionarLinhaMotorista('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaMotorista('" + this.ClientID + "'); return false";
            }
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            drpMotorista.Attributes.Add("onblur", "pegarValorMotorista('" + this.ClientID + "')");
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "MotoristaCteRod_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "MotoristaCteRod_script", @"
                    function atualizaBotoesMotorista(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        for (i = 0; i < tabela.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabela.rows.length;
                            FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }                              
    
                    function adicionarLinhaMotorista(nomeControle) {
    
                        var tabela = document.getElementById(nomeControle + '_tabela');
        
                        if(FindControl('drpMotorista', 'select', tabela.rows[tabela.rows.length - 1].cells[1]).value == 'selecione')
                        {
                          alert('para criar nova linha, a atual deve estar preenchida.');
                        }
                        else
                        {                        
                            tabela.insertRow(tabela.rows.length);
                            tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                            
                            FindControl('drpMotorista', 'select', tabela.rows[tabela.rows.length - 2]).disabled = true;
    
                            FindControl('drpMotorista', 'select', tabela.rows[tabela.rows.length - 1])[FindControl('drpMotorista', 'select', tabela.rows[tabela.rows.length - 2]).selectedIndex].remove();
    
                            FindControl('drpMotorista', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;
                            
                            atualizaBotoesMotorista(nomeControle);
                        }
                    }
    
                    function pegarValorMotorista(nomeControle)
                    {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        var idFunc = '';                    
    
                        for(var i = 0; i < tabela.rows.length; i++)
                        {
                            if(FindControl('drpMotorista', 'select', tabela.rows[i].cells[1]).value != 'selecione')
                            {
                                idFunc = idFunc +';'+ FindControl('drpMotorista', 'select', tabela.rows[i]).value;                        
                            }
                        }           
                        FindControl('hdfIdFunc', 'input').value = idFunc;
                    }
    
                    function removerLinhaMotorista(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        tabela.deleteRow(tabela.rows.length - 1);                    
                        atualizaBotoesMotorista(nomeControle);
                        
                        FindControl('drpMotorista', 'select', tabela.rows[tabela.rows.length - 1]).disabled = false;
                        
                        pegarValorMotorista(nomeControle);
                    }                
    
                    function carregaMotoristaInicial(nomeControle, idFunc)
                    {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        
                        if(idFunc.indexOf(';') > -1)
                        {
                            var lstFunc = idFunc.split(';');
                            
                            var primeiroItem = true;
                            for(var i = 0; i < lstFunc.length; i++)
                               {
                                  if(lstFunc[i] != '')
                                  {
                                     if(primeiroItem)
                                     {
                                        FindControl('drpMotorista', 'select', tabela.rows[tabela.rows.length - 1]).value = lstFunc[i];                                    
                                        primeiroItem = false;
                                     }
                                     else
                                     {
                                        tabela.insertRow(tabela.rows.length);
                                        tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;
                                        FindControl('drpMotorista', 'select', tabela.rows[i - 1]).value = lstFunc[i];
    
                                        atualizaBotoesMotorista(nomeControle);
                                     }
                                  }
                                }
                        }
                        else
                        {
                            FindControl('drpMotorista', 'select', tabela.rows[tabela.rows.length - 1]).value = idFunc;                        
                        }
                        
                    }", true);
            }
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinhaMotorista('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaMotorista('" + this.ClientID + "'); return false";
            }
        }
    }
}
