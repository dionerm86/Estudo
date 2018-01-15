using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancProj : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserInfo.GetUserInfo.IsAdminSync)
            {
                btnConfirmar_Click(sender, e);
                return;
            }
    
            if (!IsPostBack)
                switch (GetTipo())
                {
                    case 1:
                        Page.Title += "Peça de Modelo de Projeto";
                        break;
    
                    case 2:
                        Page.Title += "Material de Modelo de Projeto";
                        break;
                }
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            // Motivo do cancelamento
            string motivo = "";
            if (!UserInfo.GetUserInfo.IsAdminSync)
            {
                motivo = "Motivo do Cancelamento: " + txtMotivo.Text;
    
                // Se o tamanho do campo Obs exceder 300 caracteres, salva apenas os 300 primeiros, descartando o restante
                motivo = motivo.Length > 300 ? motivo.Substring(0, 300) : motivo;
            }
    
            try
            {
                uint id = Glass.Conversoes.StrParaUint(Request["id"]);
    
                switch (GetTipo())
                {
                    case 1:
                        PecaProjetoModelo ppm = PecaProjetoModeloDAO.Instance.GetElementByPrimaryKey(id);
                        ppm.MotivoCancelamento = motivo;
                        PecaProjetoModeloDAO.Instance.Delete(ppm);
                        break;
    
                    case 2:
                        MaterialProjetoModelo mpm = MaterialProjetoModeloDAO.Instance.GetElement(id);
                        mpm.MotivoCancelamento = motivo;
                        MaterialProjetoModeloDAO.Instance.Delete(mpm);
                        break;
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }
    
        protected int GetTipo()
        {
            int tipo = Glass.Conversoes.StrParaInt(Request["tipo"]);
            return tipo > 0 ? tipo : 0;
        }
    
        protected string GetValidate()
        {
            switch (GetTipo())
            {
                case 1: return "a peça do modelo de projeto";
                case 2: return "e material do modelo de projeto";
                default: return "";
            }
        }
    }
}
