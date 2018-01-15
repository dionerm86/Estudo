using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class SetClienteVinculos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "Cliente: " + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(Request["idCliente"]));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVinculados.PageIndex = 0;
        }
    
        protected void grdCli_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CriarVinculo")
            {
                try
                {
                    if(ClienteDAO.Instance.GetVinculados(Glass.Conversoes.StrParaUint(Request["idCliente"]))
                        .Any(f=> f.IdCli == Glass.Conversoes.StrParaUint(e.CommandArgument.ToString())))
                    {
                        MensagemAlerta.ShowMsg("Cliente já vinculado.", Page);
                        return;
                    }
                        
                    ClienteVinculoDAO.Instance.CriarVinculo(Glass.Conversoes.StrParaUint(Request["idCliente"]), Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
    
                    grdVinculados.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao vincular cliente.", ex, Page);
                }
            }
        }
    
        protected void grdVinculados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoverVinculo")
            {
                try
                {
                    ClienteVinculoDAO.Instance.RemoverVinculo(Glass.Conversoes.StrParaUint(Request["idCliente"]), Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
    
                    grdVinculados.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao remover vínculo.", ex, Page);
                }
            }
        }
    }
}
