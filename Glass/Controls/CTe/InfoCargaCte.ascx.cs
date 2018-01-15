using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;
using System.Collections.Generic;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class InfoCargaCte : CteBaseUserControl
    {
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.InfoCargaCte> ObjInfoCarga
        {
            get
            {
                var listaInfoCarga = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.InfoCargaCte>();
    
                if (hdfTipoUnidade.Value.Contains(';'))
                {
                    string[] idInfoCarga = hdfIdInfoCarga1.Value.Split(';');
                    string[] tipoUnidade = hdfTipoUnidade.Value.Split(';');
                    string[] tipoMedida = hdfTipoMedida.Value.Split(';');
                    string[] quantidade = hdfQuantidade.Value.Split(';');
                    
                    for (int i =0; i < tipoUnidade.Length; i++)
                    {
                        if (tipoUnidade[i] != "")
                        {
                            listaInfoCarga.Add(
                                new WebGlass.Business.ConhecimentoTransporte.Entidade.InfoCargaCte(
                                    new Glass.Data.Model.Cte.InfoCargaCte
                                    {
                                        IdInfoCarga = Glass.Conversoes.StrParaUint(idInfoCarga[i]),
                                        Quantidade = Glass.Conversoes.StrParaFloat(quantidade[i]),
                                        TipoMedida = tipoMedida[i],
                                        TipoUnidade = Glass.Conversoes.StrParaInt(tipoUnidade[i])                                        
                                    }));
                        }
                    }
                    return listaInfoCarga;
                }
                else
                {
                    var infoCarga = new WebGlass.Business.ConhecimentoTransporte.Entidade.InfoCargaCte(new Glass.Data.Model.Cte.InfoCargaCte
                    {
                        IdInfoCarga = Glass.Conversoes.StrParaUint(hdfIdInfoCarga1.Value),
                        Quantidade = float.Parse(hdfQuantidade.Value != "" ? hdfQuantidade.Value : "0"),
                        TipoMedida = hdfTipoMedida.Value,
                        TipoUnidade = Glass.Conversoes.StrParaInt(hdfTipoUnidade.Value != "" ? hdfTipoUnidade.Value : "-1")
                    });
                    listaInfoCarga.Add(infoCarga);
                    return listaInfoCarga;
                }
            }
            set
            {
                string idInfoCarga = string.Empty;
                string tipoUnidade = string.Empty;
                string tipoMedida = string.Empty;
                string quantidade = string.Empty;
    
                foreach (var i in value)
                {
                    idInfoCarga = idInfoCarga + ";" + i.IdInfoCarga.ToString();
                    tipoUnidade = tipoUnidade + ";" + i.TipoUnidade.ToString();
                    tipoMedida = tipoMedida + ";" + i.TipoMedida;
                    quantidade = quantidade + ";" + i.Quantidade.ToString();
                }
    
                if(value.Count > 0)
                Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                    string.Format("carregaInfoInicial('{0}', '{1}', '{2}', '{3}', '{4}');", 
                    this.ClientID, tipoUnidade, tipoMedida, quantidade, idInfoCarga), true);            
    
                imgAdicionar.OnClientClick = "adicionarLinhaInfo('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaInfo('" + this.ClientID + "'); return false";
            }
        }
    
        #endregion
    
        public override IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ObjInfoCarga[0].IdCte == 0 && TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
            {
                drpTipoUnidade.SelectedValue = Configuracoes.FiscalConfig.TelaCadastroCTe.TipoUnidadeInfoCargaCtePadraoCteSaida;
                txtQuantidade.Text = Configuracoes.FiscalConfig.TelaCadastroCTe.QuantidadeInfoCargaCtePadraoCteSaida;
                txtTipoMedida.Text = Configuracoes.FiscalConfig.TelaCadastroCTe.TipoMedidaInfoCargaCtePadraoCteSaida;
            }

            drpTipoUnidade.Attributes.Add("onblur", "pegarValorInfo('"+ this.ClientID +"')");
            txtTipoMedida.Attributes.Add("onblur", "pegarValorInfo('" + this.ClientID + "')");
            txtQuantidade.Attributes.Add("onblur", "pegarValorInfo('" + this.ClientID + "')");
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlProdutoBaixaEstoque_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlProdutoBaixaEstoque_script", @"
                    function atualizaBotoesInfo(nomeControle) {
                        var tabelaInfoCargaCte = document.getElementById(nomeControle + '_tabelaInfoCargaCte');
                        for (i = 0; i < tabelaInfoCargaCte.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabelaInfoCargaCte.rows.length;
                            FindControl('imgAdicionar', 'input', tabelaInfoCargaCte.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabelaInfoCargaCte.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }
    
                    function corrigeValidadoresInfo(nomeControle) {
                        var tabelaInfoCargaCte = document.getElementById(nomeControle + '_tabelaInfoCargaCte');
                        if (tabelaInfoCargaCte.rows.length <= 1)
                            return;
    
                        var complID = '_' + (tabelaInfoCargaCte.rows.length - 1);
                        var linhaTabela = tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1];
    
                        var validadores = ['cvdrpTipoUnidade', 'rfvtxtTipoMedida', 'rfvtxtQuantidade'];
                        var controles = ['drpTipoUnidade', 'txtTipoMedida', 'txtQuantidade'];
    
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
    
                    function adicionarLinhaInfo(nomeControle) {
    
                        var tabelaInfoCargaCte = document.getElementById(nomeControle + '_tabelaInfoCargaCte');
        
                        if (FindControl('drpTipoUnidade', 'select', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value == 'selecione' ||
                            FindControl('txtTipoMedida', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value == '' ||
                            FindControl('txtQuantidade', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value == '')
                        {
                            alert('para criar nova linha, Tipo Unidade, Tipo Medida e Quantidade devem estar preenchidos');
                        }
                        else
                        {                        
                            tabelaInfoCargaCte.insertRow(tabelaInfoCargaCte.rows.length);
                            tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1].innerHTML = tabelaInfoCargaCte.rows[0].innerHTML;    
                                           
                            FindControl('hdfIdInfoCarga', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = '';
                            FindControl('drpTipoUnidade', 'select', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).disabled = false;
                            FindControl('txtTipoMedida', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).disabled = false;
                            FindControl('txtQuantidade', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).disabled = false;

                            ValidatorEnable(FindControl('cvdrpTipoUnidade', 'span', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]), true);                
                            ValidatorEnable(FindControl('rfvtxtTipoMedida', 'span', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]), true);                
                            ValidatorEnable(FindControl('rfvtxtQuantidade', 'span', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]), true);                
                            
                            atualizaBotoesInfo(nomeControle);
                            corrigeValidadoresInfo(nomeControle);
                        }
                    }
    
                    function pegarValorInfo(nomeControle)
                    {
                        var tabelaInfoCargaCte = document.getElementById(nomeControle + '_tabelaInfoCargaCte');
                        var tipoUnidade = '';
                        var tipoMedida = '';
                        var quantidade = '';
                        var idInfoCarga = '';
    
                        if (tabelaInfoCargaCte == null)
                            return;

                        for(var i = 0; i < tabelaInfoCargaCte.rows.length; i++)
                        {    
                            if(FindControl('drpTipoUnidade', 'select', tabelaInfoCargaCte.rows[i].cells[1]).value != 'selecione' &&
                               FindControl('txtTipoMedida', 'input', tabelaInfoCargaCte.rows[i].cells[3]).value != '' &&
                               FindControl('txtQuantidade', 'input', tabelaInfoCargaCte.rows[i].cells[5]).value != '')
                            {
                                tipoUnidade = tipoUnidade +';'+ FindControl('drpTipoUnidade', 'select', tabelaInfoCargaCte.rows[i].cells[1]).value;
                            
                                tipoMedida = tipoMedida +';'+ FindControl('txtTipoMedida', 'input', tabelaInfoCargaCte.rows[i].cells[3]).value;
                            
                                quantidade = quantidade +';'+ FindControl('txtQuantidade', 'input', tabelaInfoCargaCte.rows[i].cells[5]).value;

                                idInfoCarga = idInfoCarga +';'+ FindControl('hdfIdInfoCarga', 'input', tabelaInfoCargaCte.rows[i].cells[5]).value;
                            }
                        }           
                        FindControl('hdfTipoUnidade', 'input').value = tipoUnidade;
                        FindControl('hdfTipoMedida', 'input').value = tipoMedida;
                        FindControl('hdfQuantidade', 'input').value = quantidade;
                        FindControl('hdfIdInfoCarga1', 'input').value = idInfoCarga;
                    }
    
                    function removerLinhaInfo(nomeControle) {
                        var tabelaInfoCargaCte = document.getElementById(nomeControle + '_tabelaInfoCargaCte');
                        tabelaInfoCargaCte.deleteRow(tabelaInfoCargaCte.rows.length - 1);                    
                        atualizaBotoesInfo(nomeControle);
                        
                        FindControl('drpTipoUnidade', 'select', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1].cells[1]).disabled = false;
                        FindControl('txtTipoMedida', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1].cells[3]).disabled = false;
                        FindControl('txtQuantidade', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1].cells[5]).disabled = false;
                        
                        pegarValorInfo(nomeControle);
                        corrigeValidadoresInfo(nomeControle);
                    }                
    
                    function carregaInfoInicial(nomeControle, tipoUnidade, tipoMedida, quantidade, idInfoCarga)
                    {
                        var tabelaInfoCargaCte = document.getElementById(nomeControle + '_tabelaInfoCargaCte');
                        
                        if(tipoUnidade.indexOf(';') > -1)
                        {
                            var _tipoUnidade = tipoUnidade.split(';');
                            var _tipoMedida = tipoMedida.split(';');
                            var _quantidade = quantidade.split(';');
                            var _idInfoCarga = idInfoCarga.split(';');
                            var primeiroItem = true;
                            for(var i = 0; i < _tipoUnidade.length; i++)
                               {
                                  if(_tipoUnidade[i] != '')
                                  {
                                     if(primeiroItem)
                                     {
                                        FindControl('drpTipoUnidade', 'select', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = _tipoUnidade[i];
                                        FindControl('txtTipoMedida', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = _tipoMedida[i];
                                        FindControl('txtQuantidade', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = _quantidade[i];
                                        FindControl('hdfIdInfoCarga', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = _idInfoCarga[i];
                                        primeiroItem = false;
                                     }
                                     else
                                     {
                                        tabelaInfoCargaCte.insertRow(tabelaInfoCargaCte.rows.length);
                                        tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1].innerHTML = tabelaInfoCargaCte.rows[0].innerHTML;
                                        FindControl('drpTipoUnidade', 'select', tabelaInfoCargaCte.rows[i - 1]).value = _tipoUnidade[i];
                                        FindControl('txtTipoMedida', 'input', tabelaInfoCargaCte.rows[i - 1]).value = _tipoMedida[i];
                                        FindControl('txtQuantidade', 'input', tabelaInfoCargaCte.rows[i - 1]).value = _quantidade[i];
                                        FindControl('hdfIdInfoCarga', 'input', tabelaInfoCargaCte.rows[i - 1]).value = _idInfoCarga[i];
    
                                        atualizaBotoesInfo(nomeControle);
                                     }
                                  }
                                }
                        }
                        else
                        {
                            FindControl('drpTipoUnidade', 'select', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = tipoUnidade;
                            FindControl('txtTipoMedida', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = tipoMedida;
                            FindControl('txtQuantidade', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = quantidade;
                            FindControl('hdfIdInfoCarga', 'input', tabelaInfoCargaCte.rows[tabelaInfoCargaCte.rows.length - 1]).value = idInfoCarga;
                        }
                        
                    }", true);
            }

            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinhaInfo('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaInfo('" + this.ClientID + "'); return false";
            }
        }
    }
}
