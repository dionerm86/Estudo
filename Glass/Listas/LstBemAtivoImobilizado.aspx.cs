using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstBemAtivoImobilizado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstBemAtivoImobilizado));
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            string idLoja = ((DropDownList)grdBemAtivoImobilizado.FooterRow.FindControl("drpLoja")).SelectedValue;
            string idProd = ((HiddenField)grdBemAtivoImobilizado.FooterRow.FindControl("hdfIdProd")).Value;
            string grupo = ((Glass.UI.Web.Controls.ctrlSelPopup)grdBemAtivoImobilizado.FooterRow.FindControl("selGrupo")).Valor;
            string codContaContabil = ((DropDownList)grdBemAtivoImobilizado.FooterRow.FindControl("drpPlanoContaContabil")).SelectedValue;
            string tipo = ((DropDownList)grdBemAtivoImobilizado.FooterRow.FindControl("drpTipo")).SelectedValue;
            string idParent = ((TextBox)grdBemAtivoImobilizado.FooterRow.FindControl("txtCodBem")).Text;
            string numParc = ((TextBox)grdBemAtivoImobilizado.FooterRow.FindControl("txtNumParc")).Text;
            string codCentroCusto = ((HiddenField)grdBemAtivoImobilizado.FooterRow.FindControl("hdfIdCentroCusto")).Value;
            string descricao = ((TextBox)grdBemAtivoImobilizado.FooterRow.FindControl("txtDescricao")).Text;
            string vidaUtil = ((TextBox)grdBemAtivoImobilizado.FooterRow.FindControl("txtVidaUtil")).Text;
    
            BemAtivoImobilizado novo = new BemAtivoImobilizado();
            novo.IdLoja = Glass.Conversoes.StrParaUint(idLoja);
            novo.IdProd = Glass.Conversoes.StrParaUint(idProd);
            novo.Grupo = Glass.Conversoes.StrParaInt(grupo);
            novo.IdContaContabil = Glass.Conversoes.StrParaUint(codContaContabil);
            novo.Tipo = Glass.Conversoes.StrParaInt(tipo);
            novo.NumParc = Glass.Conversoes.StrParaInt(numParc);
            novo.IdBemAtivoImobilizadoPrinc = Glass.Conversoes.StrParaUintNullable(idParent);
            novo.IdCentroCusto = Glass.Conversoes.StrParaUintNullable(codCentroCusto);
            novo.Descricao = descricao;
            novo.VidaUtil = Glass.Conversoes.StrParaIntNullable(vidaUtil);
    
            BemAtivoImobilizadoDAO.Instance.Insert(novo);
            grdBemAtivoImobilizado.DataBind();
        }
    
        protected void grdBemAtivoImobilizado_DataBound(object sender, EventArgs e)
        {
            if (grdBemAtivoImobilizado.Rows.Count == 1)
                grdBemAtivoImobilizado.Rows[0].Visible = BemAtivoImobilizadoDAO.Instance.GetCountReal(0) > 0;
            else
                grdBemAtivoImobilizado.Rows[0].Visible = true;
        }
    
        protected void grdBemAtivoImobilizado_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdBemAtivoImobilizado.ShowFooter = e.CommandName != "Edit";
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string Validar(string idLojaStr, string idProdStr, string idContaContabilStr, 
            string idBemPrincStr, string idBemIgnorar)
        {
            string mensagemErro;
            if (!BemAtivoImobilizadoDAO.Instance.Validar(Glass.Conversoes.StrParaUint(idLojaStr), Glass.Conversoes.StrParaUint(idProdStr), 
                Glass.Conversoes.StrParaUint(idContaContabilStr), Glass.Conversoes.StrParaUintNullable(idBemPrincStr),
                Glass.Conversoes.StrParaUintNullable(idBemIgnorar), out mensagemErro))
            {
                return "Erro;" + mensagemErro;
            }
            else
                return "Ok";
        }
    
        [Ajax.AjaxMethod]
        public string GetCentrosCustos(string idLoja)
        {
            string retorno = "<option value=''></option>";
            if (idLoja == "")
                return retorno;
    
            foreach (CentroCusto c in CentroCustoDAO.Instance.GetByLoja(Glass.Conversoes.StrParaUint(idLoja)))
                retorno += String.Format("<option value='{0}'>{1}</option>", c.IdCentroCusto, c.Descricao);
    
            return retorno;
        }
    
        #endregion
    
        protected void odsBemAtivoImobilizado_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir bem/componente ativo imobilizado.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsBemAtivoImobilizado_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir bem/componente ativo imobilizado.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsBemAtivoImobilizado_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar bem/componente ativo imobilizado.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
