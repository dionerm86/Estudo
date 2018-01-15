using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class CobrancaDuplCte : CteBaseUserControl
    {
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.CobrancaDuplCte> ObjCobrancaDuplCte
        {
            get
            {
                var listaCobrancaDupl = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.CobrancaDuplCte>();
    
                if (hdfNumDupl.Value.Contains(';'))
                {
                    string[] numDupl = hdfNumDupl.Value.Split(';');
                    string[] dataVenc = hdfDataVenc.Value.Split(';');
                    string[] valorDupl = hdfValorDupl.Value.Split(';');
    
                    for (int i = 0; i < numDupl.Length; i++)
                    {
                        if (numDupl[i] != "")
                        {
                            listaCobrancaDupl.Add(
                                new WebGlass.Business.ConhecimentoTransporte.Entidade.CobrancaDuplCte(
                                    new Glass.Data.Model.Cte.CobrancaDuplCte
                                    {
                                        NumeroDupl = numDupl[i],
                                        DataVenc = Glass.Conversoes.StrParaDate(dataVenc[i]),
                                        ValorDupl = Glass.Conversoes.StrParaDecimal(valorDupl[i])
                                    }));
                        }
                    }
                    return listaCobrancaDupl;
                }
                else
                {
                    var cobrancaDuplCte = new WebGlass.Business.ConhecimentoTransporte.Entidade
                        .CobrancaDuplCte(new Glass.Data.Model.Cte.CobrancaDuplCte
                    {
                        NumeroDupl = hdfNumDupl.Value,
                        DataVenc = Glass.Conversoes.StrParaDate(hdfDataVenc.Value),
                        ValorDupl = Glass.Conversoes.StrParaDecimal(hdfValorDupl.Value)
                    });
                    listaCobrancaDupl.Add(cobrancaDuplCte);
                    return listaCobrancaDupl;
                }
            }
            set
            {
                string numDupl = string.Empty;
                string dataVenc = string.Empty;
                string valorDupl = string.Empty;
    
                foreach (var i in value)
                {
                    numDupl = numDupl + ";" + i.NumeroDupl;
                    dataVenc = dataVenc + ";" + i.DataVenc.ToString();
                    valorDupl = valorDupl + ";" + i.ValorDupl.ToString();
                }
    
                if (value.Count > 0)
                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaCobrancaInicial('{0}', '{1}', '{2}', '{3}');", this.ClientID, numDupl, dataVenc, valorDupl), true);
    
                imgAdicionar.OnClientClick = "adicionarLinhaCobranca('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaCobranca('" + this.ClientID + "'); return false";
            }
        }
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.CTe.CobrancaDuplCte));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            imgData.OnClientClick = "return SelecionaData('" + this.ID + "_txtData0', this)";
            txtnumDupl.Attributes.Add("onblur", "pegarValorCobranca('" + this.ClientID + "')");
            txtData0.Attributes.Add("onchange", "pegarValorCobranca('" + this.ClientID + "')");
            txtValorDupl.Attributes.Add("onblur", "pegarValorCobranca('" + this.ClientID + "')");
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "CobrancaDuplCte_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "CobrancaDuplCte_script", @"
                    function atualizaBotoesCobranca(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        for (i = 0; i < tabela.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabela.rows.length;
                            FindControl('imgAdicionar', 'input', tabela.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabela.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }                              
    
                    function adicionarLinhaCobranca(nomeControle) {
    
                        var tabela = document.getElementById(nomeControle + '_tabela');
        
                        if(FindControl('txtnumDupl', 'input', tabela.rows[tabela.rows.length - 1]).value == '' ||                       
                           FindControl('txtValorDupl', 'input', tabela.rows[tabela.rows.length - 1]).value == '' ||
                           FindControl('txtData' + (tabela.rows.length - 1), 'input', tabela.rows[tabela.rows.length - 1]).value == '')
                        {
                          alert('para criar nova linha, a atual deve estar preenchida');
                        }
                        else
                        {                        
                            tabela.insertRow(tabela.rows.length);
                            tabela.rows[tabela.rows.length - 1].innerHTML = tabela.rows[0].innerHTML;                        
                            var listaCelulas = CobrancaDuplCte.adicionarLinha(tabela.rows.length - 1, nomeControle);
    
                            FindControl('txtnumDupl', 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;                        
                            FindControl('txtValorDupl', 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;
                            FindControl('txtData' + (tabela.rows.length - 2), 'input', tabela.rows[tabela.rows.length - 2]).disabled = true;
    
                            tabela.rows[tabela.rows.length - 1].cells[4].innerHTML = listaCelulas.value[0]; 
                            tabela.rows[tabela.rows.length - 1].cells[5].innerHTML = listaCelulas.value[1] + listaCelulas.value[2] + listaCelulas.value[3] + listaCelulas.value[4] ;
    
                            FindControl('txtnumDupl', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
                            FindControl('txtValorDupl', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
                            FindControl('txtData' + (tabela.rows.length - 1), 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
                            
                            atualizaBotoesCobranca(nomeControle);
                        }
                    }
    
                    function pegarValorCobranca(nomeControle)
                    {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        var numDupl = '';
                        var dataVenc = '';
                        var valorDupl = '';
    
                        for(var i = 0; i < tabela.rows.length; i++)
                        {
    
                            if(FindControl('txtnumDupl', 'input', tabela.rows[i]).value != '' &&
                               FindControl('txtValorDupl', 'input', tabela.rows[i]).value != '' &&
                               FindControl('txtData', 'input', tabela.rows[i]).value != '')
                            {
                                numDupl = numDupl +';'+ FindControl('txtnumDupl', 'input', tabela.rows[i]).value;
    
                                valorDupl = valorDupl +';'+ FindControl('txtValorDupl', 'input', tabela.rows[i]).value;
                            
                                dataVenc = dataVenc +';'+ FindControl('txtData' + i, 'input', tabela.rows[i]).value;                                                    
                            }
                        }           
                        FindControl('hdfNumDupl', 'input').value = numDupl;
                        FindControl('hdfDataVenc', 'input').value = dataVenc;
                        FindControl('hdfValorDupl', 'input').value = valorDupl;
                    }
    
                    function removerLinhaCobranca(nomeControle) {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        tabela.deleteRow(tabela.rows.length - 1);                    
                        atualizaBotoesCobranca(nomeControle);
                        
                        FindControl('txtnumDupl', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
                        FindControl('txtValorDupl', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
                        FindControl('txtData', 'input', tabela.rows[tabela.rows.length - 1]).disabled = false;
                        
                        pegarValorCobranca(nomeControle);
                    }                
    
                    function carregaCobrancaInicial(nomeControle, numDupl, dataVenc, valorDupl)
                    {
                        var tabela = document.getElementById(nomeControle + '_tabela');
                        
                        FindControl('hdfNumDupl', 'input').value = numDupl;
                        FindControl('hdfDataVenc', 'input').value = dataVenc;
                        FindControl('hdfValorDupl', 'input').value = valorDupl;                       

                        if(numDupl.indexOf(';') > -1)
                        {
                            var _numDupl = numDupl.split(';');
                            var _dataVenc = dataVenc.split(';');
                            var _valorDupl = valorDupl.split(';');
                            var primeiroItem = true;
                            for(var i = 0; i < _numDupl.length; i++)
                               {
                                  if(_numDupl[i] != '')
                                  {
                                     if(!primeiroItem)
                                     {
                                        var botao = FindControl('imgAdicionar', 'input', tabela.rows[tabela.rows.length - 1]);
                                        botao.click();
                                     }  
    
                                     FindControl('txtnumDupl', 'input', tabela.rows[tabela.rows.length - 1]).value = _numDupl[i];
                                     FindControl('txtValorDupl', 'input', tabela.rows[tabela.rows.length - 1]).value = _valorDupl[i];
                                     FindControl('txtData', 'input', tabela.rows[tabela.rows.length - 1]).value = _dataVenc[i];
                                     primeiroItem = false;
                                  }
                                }
                        }
                        else
                        {
                            FindControl('txtnumDupl', 'input', tabela.rows[tabela.rows.length - 1]).value = numDupl;
                            FindControl('txtValorDupl', 'input', tabela.rows[tabela.rows.length - 1]).value = valorDupl;
                            FindControl('txtData', 'input', tabela.rows[tabela.rows.length - 1]).value = dataVenc;
                        }
                        
                    }", true);
            }
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinhaCobranca('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaCobranca('" + this.ClientID + "'); return false";
            }
        }
    
        [Ajax.AjaxMethod()]
        public string[] adicionarLinha(int numLinha, string controle)
        {
            var listaCelulas = new List<string>();
            string nameCtrl = controle + '$' + "_txtData" + numLinha;
            string idTextBox = controle + "_txtData" + numLinha;
            string metodoOnblur = "pegarValorCobranca('" + controle + "')";
    
            listaCelulas.Add("Data Venc:");
            listaCelulas.Add(string.Format("<input name=\"{0}\" id=\"{1}\" onkeypress=\"return mascara_data" +
                "(event, this), soNumeros(event, true, true);\" maxlength=\"10\" onblur=\"{2}\" style=\"width:70px;\" type=\"text\">", nameCtrl, idTextBox, metodoOnblur));
    
            nameCtrl = controle + '$' + "_imgData";
            string idImagemCalendario = controle + numLinha + "_imgData";
    
            listaCelulas.Add(string.Format("<input name=\"{0}\" id=\"{1}\" onclick=\"return SelecionaData('{2}', this);\"" +
                "maxlength=\"10\" onblur=\"{2}\" style=\"border-width:0px;padding-left:3px;margin-bottom:-3px;\" type=\"image\" src=\"../Images/calendario.gif\" title=\"Selecionar\" >", nameCtrl, idImagemCalendario, idTextBox));
    
            listaCelulas.Add(string.Format("&nbsp&nbsp<input type=\"image\" id=\"imgAdicionar\" onclick=\"adicionarLinhaCobranca('{0}', this); return false;\"" +
                "class=\"img-linha\" src=\"../Images/Insert.gif\" title=\"Adicionar Campos de Dados de Duplicata\" >", controle));
    
            listaCelulas.Add(string.Format("<input type=\"image\" id=\"imgRemover\" onclick=\"removerLinhaCobranca('{0}', this); return false;\"" +
                "class=\"img-linha\" src=\"../Images/ExcluirGrid.gif\" title=\"Remover Campos de Dados de Duplicata\" >", controle));
    
            return listaCelulas.ToArray();
        }
    }
}

