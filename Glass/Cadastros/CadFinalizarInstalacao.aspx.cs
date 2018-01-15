using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFinalizarInstalacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadFinalizarInstalacao));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdColocacoes.PageIndex = 0;
    
            grdColocacoes.DataBind();
        }
    
        /// <summary>
        /// Função para continuar a instalação
        /// </summary>
        [Ajax.AjaxMethod()]
        public string Continuar(string idInstalacao, string obs)
        {
            try
            {
                InstalacaoDAO.Instance.ContinuarComTransacao(idInstalacao.StrParaInt(), obs);
    
                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao continuar Instalação.", ex);
            }
        }
    
        /// <summary>
        /// Função para cancelar a instalação
        /// </summary>
        /// <param name="idInstalacao"></param>
        /// <param name="obs"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string Cancelar(string idInstalacao, string obs)
        {
            try
            {
                InstalacaoDAO.Instance.Cancelar(Glass.Conversoes.StrParaUint(idInstalacao), obs);
    
                return "ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar Instalação.", ex);
            }
        }
    
        protected void grdColocacoes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Finalizar")
            {
                try
                {
                    InstalacaoDAO.Instance.FinalizarComTransacao(e.CommandArgument.ToString().StrParaInt());
                    grdColocacoes.DataBind();
                    Glass.MensagemAlerta.ShowMsg("Instalação finalizada com sucesso.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar Instalação.", ex, Page);
                }
            }
        }
    }
}
