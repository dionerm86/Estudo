using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetObsLancFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            nf.Visible = Request["idNf"] != null;
            ct.Visible = Request["idCte"] != null;
        }
    
        protected void grdObsLancFiscal_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Adicionar")
            {
                if (nf.Visible)
                {
                    ObsLancFiscalNf nova = new ObsLancFiscalNf();
                    nova.IdNf = Glass.Conversoes.StrParaUint(Request["idNf"]);
                    nova.IdObsLancFiscal = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
    
                    ObsLancFiscalNfDAO.Instance.Insert(nova);
                    grdObsLancFiscalNf.DataBind();
                }
    
                if (ct.Visible)
                {
                    ObsLancFiscalCte nova = new ObsLancFiscalCte();
                    nova.IdCte = Glass.Conversoes.StrParaUint(Request["idCte"]);
                    nova.IdObsLancFiscal = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
    
                    ObsLancFiscalCteDAO.Instance.Insert(nova);
                    grdObsLancFiscalCte.DataBind();
                }
    
                grdObsLancFiscal.DataBind();
            }
        }
    
        protected void odsObsLancFiscalNf_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            grdObsLancFiscal.DataBind();
            grdObsLancFiscalNf.DataBind();
        }
    
        protected void odsObsLancFiscalCte_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            grdObsLancFiscal.DataBind();
            grdObsLancFiscalCte.DataBind();
        }
    }
}
