using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstAntecipFornec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstAntecipFornec));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdAntecipFornec.PageIndex = 0;
        }
    
        protected void odsAntecipacaoFornecedor_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar antecipa��o.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Glass.MensagemAlerta.ShowMsg("Antecipa��o cancelada.", Page);
        }
    
        protected void grdAntecipFornec_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Finalizar")
            {
                try
                {
                    uint idAntecipFornec = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    decimal creditoGerado;
                    AntecipacaoFornecedorDAO.Instance.Finalizar(null, idAntecipFornec, out creditoGerado);
    
                    string msg = "Antecipa��o finalizada.";
                    if (creditoGerado > 0)
                        msg += " Foi gerado um cr�dito de " + creditoGerado.ToString("C") + " para o fornecedor " + AntecipacaoFornecedorDAO.Instance.GetNomeFornec(idAntecipFornec, true) + ".";
    
                    Glass.MensagemAlerta.ShowMsg(msg, Page);
                    grdAntecipFornec.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar antecipa��o.", ex, Page);
                }
            }
            else if (e.CommandName == "Reabrir")
            {
                try
                {
                    uint idObra = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    //ObraDAO.Instance.Reabrir(idObra);
    
                    Glass.MensagemAlerta.ShowMsg("Antecipa��o reaberta!", Page);
                    grdAntecipFornec.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar antecipa��o.", ex, Page);
                }
            }
        }
    
        #region M�todos AJAX
    
        /// <summary>
        /// Busca o fornecedor
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetFornec(string idFornec)
        {
            if (!FornecedorDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idFornec)))
                return "Erro;Fornecedor n�o encontrado.";
            else
                return "Ok;" + FornecedorDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idFornec));
        }
    
        #endregion
    }
}
