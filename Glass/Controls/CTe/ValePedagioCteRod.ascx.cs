using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class ValePedagioCteRod : CteBaseUserControl
    {
        #region Propriedades

        public List<WebGlass.Business.ConhecimentoTransporte.Entidade.ValePedagioCteRod> ObjValePedagioCteRod
        {
            get
            {
                var listaValePedagio = new List<WebGlass.Business.ConhecimentoTransporte.Entidade.ValePedagioCteRod>();
    
                if (hdfIdFornecedor.Value.Contains(';'))
                {
                    string[] idFornecedor = hdfIdFornecedor.Value.Split(';');
                    string[] numeroCompra = hdfNumeroCompra.Value.Split(';');
                    string[] cnpjComprador = hdfCnpjComprador.Value.Split(';');                
    
                    for (int i = 0; i < idFornecedor.Length; i++)
                    {
                        if (idFornecedor[i] != "")
                        {
                            listaValePedagio.Add(
                                new WebGlass.Business.ConhecimentoTransporte.Entidade.ValePedagioCteRod(
                                    new Glass.Data.Model.Cte.ValePedagioCteRod
                                    {
                                        IdFornec = Glass.Conversoes.StrParaUint(idFornecedor[i]),
                                        NumeroCompra = numeroCompra[i],
                                        CnpjComprador = cnpjComprador[i]
                                    }));
                        }
                    }
                }
                    return listaValePedagio;
            }
            set
            {
                string idFornecedor = string.Empty;
                string numeroCompra = string.Empty;
                string cnpjComprador = string.Empty;            
    
                foreach (var i in value)
                {
                    idFornecedor = idFornecedor + ";" + i.IdFornec.ToString();
                    numeroCompra = numeroCompra + ";" + i.NumeroCompra.ToString();
                    cnpjComprador = cnpjComprador + ";" + i.CnpjComprador;                
                }
    
                if (value.Count > 0)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaPedagioInicial('{0}', '{1}', '{2}', '{3}');", this.ClientID,
                        idFornecedor, numeroCompra, cnpjComprador), true);
                }
                imgAdicionar.OnClientClick = "adicionarLinhaPedagio('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaPedagio('" + this.ClientID + "'); return false";
            }
        }
    
        #endregion
    
        public override IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            drpFornecedor.Attributes.Add("onblur", "pegarValorPedagio('" + this.ClientID + "')");        
            txtNumeroCompraPedagio.Attributes.Add("onblur", "pegarValorPedagio('" + this.ClientID + "')");
            txtCnpjComprador.Attributes.Add("onblur", "pegarValorPedagio('" + this.ClientID + "')");
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ValePedagioCteRod_script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ValePedagioCteRod_script", @"
                    function atualizaBotoesPedagio(nomeControle) {
                        var tabelaValePedagio = document.getElementById(nomeControle + '_tabelaValePedagio');
                        for (i = 0; i < tabelaValePedagio.rows.length; i++) {
                            var isUltimaLinha = (i + 1) == tabelaValePedagio.rows.length;
                            FindControl('imgAdicionar', 'input', tabelaValePedagio.rows[i]).style.display = isUltimaLinha ? '' : 'none';
                            FindControl('imgRemover', 'input', tabelaValePedagio.rows[i]).style.display = i > 0 && isUltimaLinha ? '' : 'none';
                        }
                    }
    
                    function todosOsCamposVaziosPedagio(nomeControle) {
                        var tabelaValePedagio = document.getElementById(nomeControle + '_tabelaValePedagio');
                        var controles = ['drpFornecedor', 'txtNumeroCompraPedagio'];
    
                        for (var c in controles) {
                            var cont = FindControl(controles[c], 'input', tabelaValePedagio);
                            if (!cont)
                                cont = FindControl(controles[c], 'select', tabelaValePedagio);
    
                            if (!!cont && cont.value != '')
                                return false;
                        }
    
                        return true;
                    }
    
                    function validaCampoVazioPedagio(val, args) {
                        var ind = val.id.indexOf('_ctv') > -1 ? val.id.indexOf('_ctv') : val.id.indexOf('_cmp');
                        var nomeControle = val.id.substr(0, ind);
                        args.IsValid = todosOsCamposVaziosPedagio(nomeControle) || args.Value != '';
                    }
    
                    function corrigeValidadoresPedagio(nomeControle) {
                        var tabelaValePedagio = document.getElementById(nomeControle + '_tabelaValePedagio');
                        if (tabelaValePedagio.rows.length <= 1)
                            return;
    
                        var complID = '_' + (tabelaValePedagio.rows.length - 1);
                        var linhaTabela = tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1];
    
                        var validadores = ['cmpFornecedor', 'ctvNumeroCompraPedagio'];
                        var controles = ['drpFornecedor', 'txtNumeroCompraPedagio'];
    
                        for (var v in validadores) {
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
    
                    function adicionarLinhaPedagio(nomeControle) {
    
                        var tabelaValePedagio = document.getElementById(nomeControle + '_tabelaValePedagio');
                        var a = 1;
    
                        if (FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value == '0' ||
                            FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value == '')
                        {                        
                            alert('Para entrar com dados de novo seguro, os campos Fornecedor e Número Compra devem estar preenchidos.');
                        }
                        else
                        {       
                            FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).style.border = '';
                            FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).style.border = '';
    
                            tabelaValePedagio.insertRow(tabelaValePedagio.rows.length);
                            tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1].innerHTML = tabelaValePedagio.rows[0].innerHTML;                                         
                            
                            FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 2]).disabled = true;                        
                            FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 2]).disabled = true;                        
                            FindControl('txtCnpjComprador', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 2]).disabled = true;                        
                            
                            FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).disabled = false;                        
                            FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).disabled = false;                        
                            FindControl('txtCnpjComprador', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).disabled = false;                        
                            
                            atualizaBotoesPedagio(nomeControle);
                            corrigeValidadoresPedagio(nomeControle);
                        }
                    }
                    
                    function pegarValorPedagio(nomeControle)
                    {
                        var tabelaValePedagio = document.getElementById(nomeControle + '_tabelaValePedagio');
                        var idFornecedor = '';
                        var numCompraPedagio = '';
                        var cnpjComprador = '';
                        
                        for(var i = 0; i < tabelaValePedagio.rows.length; i++)
                        {
    
                            if(FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[i]).value != 'selecione'&&
                               FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value != '')
                            {
                                idFornecedor = idFornecedor +';'+ FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[i]).value;                            
                            
                                numCompraPedagio = numCompraPedagio +';'+ FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[i]).value;
    
                                cnpjComprador = cnpjComprador +';'+ FindControl('txtCnpjComprador', 'input', tabelaValePedagio.rows[i]).value;
                            }
                        }           
                        FindControl('hdfIdFornecedor', 'input').value = idFornecedor;
                        FindControl('hdfNumeroCompra', 'input').value = numCompraPedagio;
                        FindControl('hdfCnpjComprador', 'input').value = cnpjComprador;
                        
                    }
    
                    function removerLinhaPedagio(nomeControle) {
                        var tabelaValePedagio = document.getElementById(nomeControle + '_tabelaValePedagio');
                        tabelaValePedagio.deleteRow(tabelaValePedagio.rows.length - 1);                    
                        atualizaBotoesPedagio(nomeControle);
    
                        FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).disabled = false;
                        FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).disabled = false;
                        FindControl('txtCnpjComprador', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).disabled = false;
                                            
                        pegarValorPedagio(nomeControle);
                        corrigeValidadoresPedagio(nomeControle);
                    }                
    
                    function carregaPedagioInicial(nomeControle, idFornecedor, numCompraPedagio, cnpjComprador)
                    {
                        var tabelaValePedagio = document.getElementById(nomeControle + '_tabelaValePedagio');
                        
                        if(numCompraPedagio.indexOf(';') > -1)
                        {
                            var _idFornecedor = idFornecedor.split(';');
                            var _numCompraPedagio = numCompraPedagio.split(';');
                            var _cnpjComprador = cnpjComprador.split(';');                        
    
                            var primeiroItem = true;
    
                            for(var i = 0; i < _idFornecedor.length; i++)
                               {
                                  if(_idFornecedor[i] != '')
                                  {
                                     if(primeiroItem)
                                     {
                                        FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = _idFornecedor[i];                                    
                                        FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = _numCompraPedagio[i];
                                        FindControl('txtCnpjComprador', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = _cnpjComprador[i];                                    
    
                                        primeiroItem = false;
                                     }
                                     else
                                     {
                                        tabelaValePedagio.insertRow(tabelaValePedagio.rows.length);
                                        tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1].innerHTML = tabelaValePedagio.rows[i-1].innerHTML;
                                        FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = _idFornecedor[i];                                    
                                        FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = _numCompraPedagio[i];
                                        FindControl('txtCnpjComprador', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = _cnpjComprador[i];
    
                                        atualizaBotoesPedagio(nomeControle);
                                     }
                                  }
                                }
                        }
                        else
                        {
                            FindControl('drpFornecedor', 'select', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = idFornecedor;                        
                            FindControl('txtNumeroCompraPedagio', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = numCompraPedagio;
                            FindControl('txtCnpjComprador', 'input', tabelaValePedagio.rows[tabelaValePedagio.rows.length - 1]).value = cnpjComprador;
                        }
                        
                    }", true);
            }
    
            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = "adicionarLinhaPedagio('" + this.ClientID + "'); return false";
                imgRemover.OnClientClick = "removerLinhaPedagio('" + this.ClientID + "'); return false";
            }
        }
    }
}
