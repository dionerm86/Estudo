using System;
using System.Web.UI;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadConciliacaoBancaria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.RealizarConciliacaoBancaria))
                Response.Redirect("~/Listas/LstConciliacaoBancaria.aspx", true);
    
            if (!IsPostBack)
                ctrlData.Data = DateTime.Now;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void grdMovBanco_DataBound(object sender, EventArgs e)
        {
            hdfCodigoContaBancaria.Value = drpContaBanco.SelectedValue;
            hdfDataConciliacao.Value = ctrlData.DataString;
    
            btnConciliar.Enabled = grdMovBanco.Rows.Count > 0;
        }
    
        protected void btnConciliar_Click(object sender, EventArgs e)
        {
            try
            {
                WebGlass.Business.ConciliacaoBancaria.Fluxo.CRUD.Instance.NovaConciliacaoBancaria(
                    Glass.Conversoes.StrParaUint(hdfCodigoContaBancaria.Value), Conversoes.ConverteDataNotNull(hdfDataConciliacao.Value));
    
                Response.Redirect("~/Listas/LstConciliacaoBancaria.aspx");
            }
            catch (Exception ex)
            {
                drpContaBanco.SelectedValue = hdfCodigoContaBancaria.Value;
                ctrlData.DataString = hdfDataConciliacao.Value;
    
                Glass.MensagemAlerta.ErrorMsg("Falha ao conciliar conta bancária.", ex, Page);
            }
        }
    }
}
