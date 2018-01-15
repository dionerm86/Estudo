using System;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadLiberarInst : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            odsInstalacao.DataBind();
        }
    
        protected void btnLiberar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum))
                    throw new Exception("Para liberar instalações comum para serem executadas por equipes de instalação temperado é preciso ter permissão de controle de instalação comum.");
    
                uint idInstalacao = Glass.Conversoes.StrParaUint(dtvInstalacao.SelectedValue.ToString());
    
                InstalacaoDAO.Instance.LiberaInstalacao(idInstalacao);
    
                Glass.MensagemAlerta.ShowMsg("Instalação liberada.", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
            }
        }
    
        protected void odsInstalacao_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
    
                btnLiberar.Visible = false;
            }
            else if (txtNumPedido.Text != String.Empty)
                btnLiberar.Visible = true;
        }
    }
}
