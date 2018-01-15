using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WebGlass.Business.ConhecimentoTransporte.Entidade;
using System.Collections.Generic;
using Glass.Data.Helper;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class ConhecimentoTransRod : CteBaseUserControl
    {
        #region Propriedades

        public WebGlass.Business.ConhecimentoTransporte.Entidade.ConhecimentoTransporteRodoviario ObjCteRod
        {
            get
            {
                var obj = new WebGlass.Business.ConhecimentoTransporte.Entidade.ConhecimentoTransporteRodoviario(
                    new Glass.Data.Model.Cte.ConhecimentoTransporteRodoviario
                    {
                        CIOT = txtCIOT.Text,
                        Lotacao = chkLotacao.Checked
                    });
    
                obj.ObjLacreCteRod = ctrlLacreRod.ObjLacreCteRod;
                obj.ObjOrdemColetaCteRod = ctrlOrdem.ObjOrdemColetaCteRod;
                obj.ObjValePedagioCteRod = ctrlValePedagio.ObjValePedagioCteRod;
    
                return obj;
            }
            set
            {
                txtCIOT.Text = value.CIOT;
                chkLotacao.Checked = value.Lotacao;
                ctrlLacreRod.ObjLacreCteRod = value.ObjLacreCteRod;
                ctrlOrdem.ObjOrdemColetaCteRod = value.ObjOrdemColetaCteRod;
                ctrlValePedagio.ObjValePedagioCteRod = value.ObjValePedagioCteRod;
            }
        }
    
        public override Cte.TipoDocumentoCteEnum TipoDocumentoCte
        {
            get { return base.TipoDocumentoCte; }
            set
            {
                base.TipoDocumentoCte = value;
                ctrlLacreRod.TipoDocumentoCte = value;
                ctrlOrdem.TipoDocumentoCte = value;
                ctrlValePedagio.TipoDocumentoCte = value;
            }
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get
            {
                List<BaseValidator> val = new List<BaseValidator>();
    
                if (ctrlLacreRod.ValidadoresObrigatoriosEntrada != null)
                    val.AddRange(ctrlLacreRod.ValidadoresObrigatoriosEntrada);
    
                if (ctrlOrdem.ValidadoresObrigatoriosEntrada != null)
                    val.AddRange(ctrlOrdem.ValidadoresObrigatoriosEntrada);
    
                if (ctrlValePedagio.ValidadoresObrigatoriosEntrada != null)
                    val.AddRange(ctrlValePedagio.ValidadoresObrigatoriosEntrada);
    
                return val;
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ObjCteRod.IdCte == 0 && TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
                chkLotacao.Checked = Configuracoes.FiscalConfig.TelaCadastroCTe.LotacaoConhecimentoTransRodPadraoCteSaida;
        }

        protected void div1_Load(object sender, EventArgs e)
        {
            if (TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida && Configuracoes.FiscalConfig.ConhecimentoTransporte.EsconderComplEDadosTransRodCteSaida)
            {
                if (sender is HtmlContainerControl)
                    ((HtmlContainerControl)sender).Style.Add("display", "none");
            }
        }
    }
}
