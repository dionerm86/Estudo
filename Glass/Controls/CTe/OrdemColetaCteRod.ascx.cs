using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;
using System.Collections.Generic;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class OrdemColetaCteRod : CteBaseUserControl
    {
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.OrdemColetaCteRod> ObjOrdemColetaCteRod
        {
            get
            {
                var listaOrdemColeta = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.OrdemColetaCteRod>();
    
                if (hdfIdTransportador.Value.Contains(';'))
                {
                    string[] idTransportador = hdfIdTransportador.Value.Split(';');
                    string[] numOrdemColeta = hdfNumOrdemColeta.Value.Split(';');
                    string[] serieTrans = hdfSerie.Value.Split(';');
                    string[] dataEmissao = hdfDataEmissao.Value.Split(';');                
    
                    for (int i = 0; i < idTransportador.Length; i++)
                    {
                        if (idTransportador[i] != "")
                        {
                            listaOrdemColeta.Add(
                                new WebGlass.Business.ConhecimentoTransporte.Entidade.OrdemColetaCteRod(
                                    new Glass.Data.Model.Cte.OrdemColetaCteRod
                                    {
                                        IdTransportador = Glass.Conversoes.StrParaUint(idTransportador[i]),
                                        Numero = Glass.Conversoes.StrParaInt(numOrdemColeta[i]),
                                        Serie = serieTrans[i],
                                        DataEmissao = Convert.ToDateTime(dataEmissao[i])
                                    }));
                        }
                    }
                    return listaOrdemColeta;
                }
                else
                {
                    DateTime? dataEmissao = null;
                    listaOrdemColeta.Add(
                        new WebGlass.Business.ConhecimentoTransporte.Entidade.OrdemColetaCteRod(
                            new Glass.Data.Model.Cte.OrdemColetaCteRod
                            {
                                IdTransportador = Glass.Conversoes.StrParaUint(drpTransportador.SelectedValue != "" ? drpTransportador.SelectedValue : "0"),
                                Numero = Glass.Conversoes.StrParaInt(txtNumeroOrdColeta.Text != "" ? txtNumeroOrdColeta.Text : "0"),
                                Serie  = hdfSerie.Value,
                                DataEmissao = dataEmissao ?? (!string.IsNullOrEmpty(txtData0.Text) ? Convert.ToDateTime(txtData0.Text) : dataEmissao)
                            }));
    
                    return listaOrdemColeta;
                }
            }
            set
            {
                string idTransportador = string.Empty;
                string numOrdemColeta = string.Empty;
                string serieTrans = string.Empty;
                string dataEmissao = string.Empty;
                
                foreach (var i in value)
                {
                    idTransportador = idTransportador + ";" + i.IdTransportador.ToString();
                    numOrdemColeta = numOrdemColeta + ";" + i.Numero.ToString();
                    serieTrans = serieTrans + ";" + i.Serie;
                    dataEmissao = dataEmissao + ";" + i.DataEmissao.ToString();
                }
    
                if (value.Count > 0 && value[0].Numero > 0 && !string.IsNullOrEmpty(value[0].DataEmissao.ToString()))
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaOrdemColetaInicial('{0}', '{1}', '{2}', '{3}', '{4}');", this.ClientID, idTransportador, numOrdemColeta, serieTrans, dataEmissao), true);
                }
                imgAdicionar.OnClientClick = "adicionarLinhaOrdemColeta('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaOrdemColeta('" + this.ClientID + "'); return false";
            }
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.CTe.OrdemColetaCteRod));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!IsPostBack && ObjOrdemColetaCteRod[0].IdCte == 0 && TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                drpTransportador.SelectedValue = Configuracoes.FiscalConfig.TelaCadastroCTe.TransportadorOrdemColetaCteRodPadraoCteSaida;
    
            imgData.OnClientClick = "return SelecionaData('" + this.ID + "_txtData0', this)";
            drpTransportador.Attributes.Add("onblur", "pegarValorOrdemColeta('" + this.ClientID + "')");
            txtNumeroOrdColeta.Attributes.Add("onblur", "pegarValorOrdemColeta('" + this.ClientID + "')");
            txtSerieTrans.Attributes.Add("onblur", "pegarValorOrdemColeta('" + this.ClientID + "')");
            txtData0.Attributes.Add("onblur", "pegarValorOrdemColeta('" + this.ClientID + "')");
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "OrdemColetaCteRod_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "OrdemColetaCteRod_script", @"
                    function atualizaBotoesOrdemColeta(nomeControle) {
                        var tabelaOrdemColeta = document.getElementById(nomeControle + '_tabelaOrdemColeta');
                        for (i = 0; i < tabelaOrdemColeta.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabelaOrdemColeta.rows.length;
                            FindControl('imgAdicionar', 'input', tabelaOrdemColeta.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabelaOrdemColeta.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }
    
                    function adicionarLinhaOrdemColeta(nomeControle) {
                        
                        var tabelaOrdemColeta = document.getElementById(nomeControle + '_tabelaOrdemColeta');
                        
                        if(tabelaOrdemColeta.rows.length == 10)
                            alert('São permitidas, no máximo, 10 registros de Ordem Coleta');
              
                        if(FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value == 'selecione' ||
                           FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value == '' ||                       
                           FindControl('txtData' + (tabelaOrdemColeta.rows.length - 1), 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value == '')
                        {
                          alert('para criar nova linha, os campos Transportador, Número Ordem Coleta e Data Emissão devem estar preenchidos');
                        }
                        else
                        {                        
                            FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).style.border = '';                        
                            FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).style.border = '';                        
                            FindControl('txtData' + (tabelaOrdemColeta.rows.length - 1), 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).style.border = '';
    
                            tabelaOrdemColeta.insertRow(tabelaOrdemColeta.rows.length);
                            tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1].innerHTML = tabelaOrdemColeta.rows[0].innerHTML;                        
                            var listaCelulas = OrdemColetaCteRod.adicionarLinha(tabelaOrdemColeta.rows.length - 1, nomeControle);
    
                            FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 2]).disabled = true;                        
                            FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 2]).disabled = true;                        
                            FindControl('txtSerieTrans', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 2]).disabled = true;                       
                            
                            FindControl('txtData' + (tabelaOrdemColeta.rows.length - 2), 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 2]).disabled = true;
    
                            tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1].cells[6].innerHTML = listaCelulas.value[0]; 
                            tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1].cells[7].innerHTML = listaCelulas.value[1] + listaCelulas.value[2] + listaCelulas.value[3] + listaCelulas.value[4];
    
                            FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1])[FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 2]).selectedIndex].remove();
    
                            FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).disabled = false;                        
                            FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).disabled = false;                        
                            FindControl('txtSerieTrans', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).disabled = false;
                            FindControl('txtData' + (tabelaOrdemColeta.rows.length - 1), 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).disabled = false;
                            
                            atualizaBotoesOrdemColeta(nomeControle);
                        }
                    }
    
                    function pegarValorOrdemColeta(nomeControle)
                    {
                        var tabelaOrdemColeta = document.getElementById(nomeControle + '_tabelaOrdemColeta');
                        var idTrasnportador = '';
                        var numOrdemColeta = '';
                        var serieTrans = '';
                        var dataEmissao = '';                    
    
                        for(var i = 0; i < tabelaOrdemColeta.rows.length; i++)
                        {
                            if(FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value != 'selecione' &&
                                FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value != '' &&                
                                FindControl('txtData' + (tabelaOrdemColeta.rows.length - 1), 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value != '')
                            {
                                idTrasnportador = idTrasnportador +';'+ FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[i]).value;
    
                                numOrdemColeta = numOrdemColeta +';'+ FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[i]).value;
    
                                serieTrans = serieTrans +';'+ FindControl('txtSerieTrans', 'input', tabelaOrdemColeta.rows[i]).value;
                            
                                dataEmissao = dataEmissao +';'+ FindControl('txtData' + i, 'input', tabelaOrdemColeta.rows[i]).value;                                                    
                            }
                        }           
                        FindControl('hdfIdTransportador', 'input').value = idTrasnportador;
                        FindControl('hdfNumOrdemColeta', 'input').value = numOrdemColeta;
                        FindControl('hdfSerie', 'input').value = serieTrans;
                        FindControl('hdfDataEmissao', 'input').value = dataEmissao;
                    }
    
                    function removerLinhaOrdemColeta(nomeControle) {
                        var tabelaOrdemColeta = document.getElementById(nomeControle + '_tabelaOrdemColeta');
                        tabelaOrdemColeta.deleteRow(tabelaOrdemColeta.rows.length - 1);                    
                        atualizaBotoesOrdemColeta(nomeControle);
    
                        FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).disabled = false;                        
                        FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).disabled = false;                        
                        FindControl('txtSerieTrans', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).disabled = false;
                        FindControl('txtData' + (tabelaOrdemColeta.rows.length - 1), 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).disabled = false;                    
                        
                        pegarValorOrdemColeta(nomeControle);
                    }                
    
                    function carregaOrdemColetaInicial(nomeControle, idTransportador, numeroOrdemColeta, serieTrans, dataEmissao)
                    {
                        var tabelaOrdemColeta = document.getElementById(nomeControle + '_tabelaOrdemColeta');
                        
                        if(idTransportador.indexOf(';') > -1)
                        {
                            var _idTransportador = idTransportador.split(';');
                            var _numeroOrdemColeta = numeroOrdemColeta.split(';');
                            var _serieTrans = serieTrans.split(';');
                            var _dataEmissao = dataEmissao.split(';');
    
                            var primeiroItem = true;
                            for(var i = 0; i < _idTransportador.length; i++)
                               {
                                  if(_idTransportador[i] != '')
                                  {
                                     if(primeiroItem)
                                     {
                                        FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value = _idTransportador[i];
                                        FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value = _numeroOrdemColeta[i];
                                        FindControl('txtSerieTrans', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value = _serieTrans[i];
                                        FindControl('txtData0', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value = _dataEmissao[i];
                                        primeiroItem = false;
                                     }
                                     else
                                     {
                                        tabelaOrdemColeta.insertRow(tabelaOrdemColeta.rows.length);
                                        tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1].innerHTML = tabelaOrdemColeta.rows[i-1].innerHTML;
                                        FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[i - 1]).value = _idTransportador[i];
                                        FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[i - 1]).value = _numeroOrdemColeta[i];
                                        FindControl('txtSerieTrans', 'input', tabelaOrdemColeta.rows[i - 1]).value = _serieTrans[i];
                                        FindControl('txtData' + i, 'input', tabelaOrdemColeta.rows[i - 1]).value = _dataEmissao[i]; 
    
                                        atualizaBotoesOrdemColeta(nomeControle);                                   
                                     }
                                  }
                                }
                        }
                        else
                        {
                            FindControl('drpTransportador', 'select', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value = idTransportador;
                            FindControl('txtNumeroOrdColeta', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value = numeroOrdemColeta;
                            FindControl('txtSerieTrans', 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value = serieTrans;
                            FindControl('txtData' + i, 'input', tabelaOrdemColeta.rows[tabelaOrdemColeta.rows.length - 1]).value = dataEmissao;
                        }
                        
                    }", true);
            }
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinhaOrdemColeta('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaOrdemColeta('" + this.ClientID + "'); return false";
            }
        }
    
        [Ajax.AjaxMethod()]
        public string[] adicionarLinha(int numLinha, string controle)
        {
            var listaCelulas = new List<string>();
            string nameCtrl = controle + '$' + "_txtData" + numLinha;
            string idTextBox = controle + "_txtData" + numLinha;
            string metodoOnblur = "pegarValorOrdemColeta('" + controle + "')";
    
            listaCelulas.Add("Data Emissão:");
            listaCelulas.Add(string.Format("<input name=\"{0}\" id=\"{1}\" onkeypress=\"return mascara_data" +
                "(event, this), soNumeros(event, true, true);\" maxlength=\"10\" onblur=\"{2}\" style=\"width:70px;\" type=\"text\">", nameCtrl, idTextBox, metodoOnblur));
    
            nameCtrl = controle + '$' + "_imgData";
            string idImagemCalendario = controle + numLinha + "_imgData";
    
            listaCelulas.Add(string.Format("<input name=\"{0}\" id=\"{1}\" onclick=\"return SelecionaData('{2}', this);\"" +
                "maxlength=\"10\" onblur=\"{2}\" style=\"border-width:0px;padding-left:3px;margin-bottom:-3px;\" type=\"image\" src=\"../Images/calendario.gif\" title=\"Selecionar\" >", nameCtrl, idImagemCalendario, idTextBox));
    
            listaCelulas.Add(string.Format("&nbsp&nbsp<input type=\"image\" id=\"imgAdicionar\" onclick=\"adicionarLinhaCobranca('{0}', this); return false;\"" +
                "class=\"img-linha\" src=\"../Images/Insert.gif\" title=\"Adicionar Campos de Dados de Duplicata\" >", controle));
    
            listaCelulas.Add(string.Format("<input type=\"image\" id=\"imgRemover\" onclick=\"removerLinhaOrdemColeta('{0}', this); return false;\"" +
                "class=\"img-linha\" src=\"../Images/ExcluirGrid.gif\" title=\"Remover Campos de Dados de Duplicata\" >", controle));
    
            return listaCelulas.ToArray();
        }
    }
}
