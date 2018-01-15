using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class CadProdutoProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.CadProdutoProjeto));
    
            if (Session["pgIndProdProj"] != null)
                grdProdProj.PageIndex = Glass.Conversoes.StrParaInt(Session["pgIndProdProj"].ToString());
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod()]
        public string AplicarConfig(string idProdProj, string listaIdCor, string listaIdProd)
        {
            try
            {
                ProdutoProjetoConfigDAO.Instance.AplicarConfig(Glass.Conversoes.StrParaUint(idProdProj), listaIdCor, listaIdProd);
    
                return "ok\tProdutos configurados!.";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    
        [Ajax.AjaxMethod()]
        public string Associar(string idProdProj, string codInterno)
        {
            try
            {
                ProdutoProjetoDAO.Instance.AssociaProduto(Glass.Conversoes.StrParaUint(idProdProj), codInterno);
    
                return "ok\tProduto associado.";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    
        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string codInterno)
        {
            Produto prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);
    
            if (prod == null)
                return "Erro;Não existe produto com o código informado.";
            else if (prod.Situacao == Glass.Situacao.Inativo)
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(prod.Obs) ? " Obs: " + prod.Obs : "");
            else
                return "Prod;" + prod.IdProd + ";" + prod.Descricao;
        }
    
        #endregion
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProdProj.PageIndex = 0;
    
            if (Session["pgIndProdProj"] != null)
                Session["pgIndProdProj"] = "0";
        }
    
        protected void drpTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProdProj.PageIndex = 0;
    
            if (Session["pgIndProdProj"] != null)
                Session["pgIndProdProj"] = "0";
        }
    
        protected void grdProdProj_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow r in grdProdProj.Rows)
            {
                if (r.FindControl("odsConfigProd") == null)
                    continue;
    
                ((Colosoft.WebControls.VirtualObjectDataSource)r.FindControl("odsConfigProd")).SelectParameters[0].DefaultValue = ((HiddenField)r.FindControl("hdfIdProdProj")).Value;
                ((Colosoft.WebControls.VirtualObjectDataSource)r.FindControl("odsConfigProd")).DataBind();
            }
        }
    
        protected void grdProdProj_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Desvincular")
            {
                try
                {
                    // Desvincula itens do produto selecionado
                    ProdutoProjetoDAO.Instance.DesvincularItens(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    grdProdProj.DataBind();
    
                    Glass.MensagemAlerta.ShowMsg("Itens desvinculados.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao desvincular itens deste produto.", ex, Page);
                }
            }
        }
    
        protected void lnkInsProd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ProdutoProjeto prodProj = new ProdutoProjeto();
                prodProj.CodInterno = ((TextBox)grdProdProj.FooterRow.FindControl("txtCodInterno")).Text;
                prodProj.Descricao = ((TextBox)grdProdProj.FooterRow.FindControl("txtDescricao")).Text;
                prodProj.Tipo = Glass.Conversoes.StrParaInt(((DropDownList)grdProdProj.FooterRow.FindControl("drpTipo")).SelectedValue);
    
                if (String.IsNullOrEmpty(prodProj.CodInterno))
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o código do produto.", Page);
                    return;
                }
    
                if (String.IsNullOrEmpty(prodProj.Descricao))
                {
                    Glass.MensagemAlerta.ShowMsg("Informe a descrição do produto.", Page);
                    return;
                }
    
                ProdutoProjetoDAO.Instance.Insert(prodProj);
    
                grdProdProj.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir produto.", ex, Page);
            }
        }
    
        protected void odsProdProj_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar produto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsProdProj_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir produto.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
