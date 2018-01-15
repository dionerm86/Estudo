using System;
using System.Linq;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlBoleto : BaseUserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Ajax.Utility.RegisterTypeForAjax(typeof(ctrlBoleto));
        }

        #region Metodos Ajax

        [Ajax.AjaxMethod]
        public string ValidaImpressaoBoleto(string codigoNotaFiscal)
        {
            if (!FinanceiroConfig.UsarNumNfBoletoSemSeparacao)
                return null;

            var idsPedidosNf = PedidosNotaFiscalDAO.Instance
                .GetByNf(codigoNotaFiscal.StrParaUint())
                .Where(f => f.IdPedido.GetValueOrDefault(0) > 0)
                .Select(f => f.IdPedido.Value);

            foreach (var idPedNf in idsPedidosNf)
            {
                var idLib = LiberarPedidoDAO.Instance.GetIdLiberacao(idPedNf.ToString()).GetValueOrDefault(0);

                if(idLib == 0)
                    throw new Exception("N�o � poss�vel gerar o boleto desta NF-e, pois o pedido: " + idPedNf + " n�o possui uma libera��o vinculada.");

                if(PedidosNotaFiscalDAO.Instance.GetByLiberacaoPedido(idLib).Length == 0)
                    throw new Exception("N�o � poss�vel gerar o boleto desta NF-e, pois a libera��o: " + idLib + " n�o esta vinculada a mesma.");

                foreach (var idPedLib in ProdutosLiberarPedidoDAO.Instance.GetIdsPedidoByLiberacao(idLib))
                {
                    if(!idsPedidosNf.Contains(idPedLib))
                        throw new Exception("N�o � poss�vel gerar o boleto desta NF-e, pois o pedido: " + idPedLib + " n�o esta vinculado a mesma.");

                }
            }

            return null;
        }

        #endregion

        private int? _codigoNotaFiscal, _codigoContaReceber, _codigoLiberacao;

        public int? CodigoNotaFiscal
        {
            get
            {
                if (_codigoNotaFiscal.GetValueOrDefault() == 0 && _codigoContaReceber > 0)
                {
                    var idsNf = NotaFiscalDAO.Instance.ObtemIdNfByContaR((uint)_codigoContaReceber.Value, true);
                    _codigoNotaFiscal = idsNf != null && idsNf.Count > 0 ? (int)idsNf[0] : (int?)null;
                    return _codigoNotaFiscal;
                }
                else
                    return _codigoNotaFiscal;
            }
            set { _codigoNotaFiscal = value; }
        }
    
        public int? CodigoContaReceber
        {
            get { return _codigoContaReceber; }
            set { _codigoContaReceber = value; }
        }

        public int? CodigoLiberacao
        {
            get { return _codigoLiberacao; }
            set { _codigoLiberacao = value; }
        }
    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            imgBoleto.OnClientClick = "abreBoleto(" + (CodigoNotaFiscal ?? 0) + ", " +
                (CodigoContaReceber ?? 0) + ", " + (CodigoLiberacao ?? 0) + "); return false";
    
            string jaImpresso = WebGlass.Business.Boleto.Fluxo.Impresso.Instance.MensagemBoletoImpresso(CodigoContaReceber, CodigoNotaFiscal, CodigoLiberacao);
            imgBoleto.ToolTip = "Boleto" + (!String.IsNullOrEmpty(jaImpresso) ? String.Format(" ({0})", jaImpresso) : String.Empty);
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlBoleto"))
            {
                string relative = this.ResolveUrl("~/");
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlBoleto", @"
                    function abreBoleto(codigoNotaFiscal, codigoContaReceber, codigoLiberacao)
                    {
                        var validacao = ctrlBoleto.ValidaImpressaoBoleto(codigoNotaFiscal);
                        
                        if(validacao.error != null){
                            alert(validacao.error.description);
                            return false;
                        }

                        openWindow(400, 600, '" + relative + @"Relatorios/Boleto/Imprimir.aspx?codigoNotaFiscal=' + codigoNotaFiscal + 
                            '&codigoContaReceber=' + codigoContaReceber + '&codigoLiberacao=' + codigoLiberacao);
                    }", true);
            }
        }
    }
}
