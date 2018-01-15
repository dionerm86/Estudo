using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstEtiquetaImpressoes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstEtiquetaImpressoes));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdImpressao.PageIndex = 0;
            grdImpressao.DataBind();
        }
    
        /*[Ajax.AjaxMethod()]
        public string CancelarImpressao(string idImpressao, string idPedido)
        {
            try
            {
                if (idPedido == "0")
                    ImpressaoEtiquetaDAO.Instance.CancelarImpressao(UserInfo.GetUserInfo.CodUser, Glass.Conversoes.StrParaUint(idImpressao), "");
                else
                    ImpressaoEtiquetaDAO.Instance.CancelarPedidoImpresso(UserInfo.GetUserInfo.CodUser, Glass.Conversoes.StrParaUint(idImpressao), Glass.Conversoes.StrParaUint(idPedido), "");
    
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" +Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }*/
    
        protected void grdImpressao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.Cells[0].FindControl("hdfSituacao") == null)
                return;
    
            int situacao = Glass.Conversoes.StrParaInt(((HiddenField)e.Row.Cells[0].FindControl("hdfSituacao")).Value);
            if (situacao == (int)ImpressaoEtiqueta.SituacaoImpressaoEtiqueta.Cancelada)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = System.Drawing.Color.Red;
        }
    
        protected bool PermitirCancelar(object idFunc)
        {
            return 
                (Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra) &&
                Convert.ToUInt32(idFunc) == UserInfo.GetUserInfo.CodUser) ||
                Config.PossuiPermissao(Config.FuncaoMenuPCP.CancelarImpressaoEtiqueta);
        }
    }
}
