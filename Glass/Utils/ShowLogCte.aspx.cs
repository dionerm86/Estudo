using System;
using System.Web.UI;
using Glass.Data.Model.Cte;

namespace Glass.UI.Web.Utils
{
    public partial class ShowLogCte : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            uint idCte = Glass.Conversoes.StrParaUint(Request["idCte"]);
            int situacao = Glass.Conversoes.StrParaInt(Request["situacao"]);
    
            if (!IsPostBack)
                Page.Title += Request["numero"].ToString();
    
            if (situacao == (int)ConhecimentoTransporte.SituacaoEnum.Cancelado)
            {
                lblTituloMotivo.Text = "Motivo do Cancelamento:";
                lblTextoMotivo.Text = WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte.Instance.ObtemMotivoCanc(idCte);
            }
            else if (situacao == (int)ConhecimentoTransporte.SituacaoEnum.Inutilizado)
            {
                lblTituloMotivo.Text = "Motivo da Inutilização:";
                lblTextoMotivo.Text = WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte.Instance.ObtemMotivoInut(idCte);
            }
        }
    }
}
