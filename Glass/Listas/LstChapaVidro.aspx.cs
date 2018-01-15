using System;
using System.Web.UI;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstChapaVidro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstChapaVidro));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdChapaVidro.PageIndex = 0;
        }
    
        protected void odsSubgrupo_Load(object sender, EventArgs e)
        {
            odsSubgrupo.SelectParameters["idGrupo"].DefaultValue = ((int)Glass.Data.Model.NomeGrupoProd.Vidro).ToString();
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdChapaVidro.PageIndex = 0;
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetProduto(string codInterno)
        {
            try
            {
                Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);
                
                if (prod == null)
                    throw new Exception("Produto não encontrado.");
                else if (prod.Situacao == Glass.Situacao.Inativo)
                    return "Erro|Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
    
                if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(prod.IdGrupoProd))
                    return "Ok|" + prod.IdProd + "|" + prod.Descricao;
    
                throw new Exception("Esse produto não é um vidro.");
            }
            catch (Exception ex)
            {
                return "Erro|" + ex.Message;
            }
        }
    
        [Ajax.AjaxMethod]
        public string CalcM2(string idProd, string altura, string largura)
        {
            return MetodosAjax.CalcM2(((int)Glass.Data.Model.TipoCalculoGrupoProd.M2).ToString(), altura, largura, "1", idProd, "false", "0", "false");
        }
    
        #endregion
    
        protected void odsChapaVidro_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir chapa de vidro.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsChapaVidro_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar chapa de vidro.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
