using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.EFD;

namespace Glass.UI.Web.Listas
{
    public partial class LstCentroCusto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstCentroCusto));
        }
    
        protected void grdCentroCusto_DataBound(object sender, EventArgs e)
        {
            if (grdCentroCusto.Rows.Count == 1)
                grdCentroCusto.Rows[0].Visible = CentroCustoDAO.Instance.GetCountReal() > 0;
            else
                grdCentroCusto.Rows[0].Visible = true;
        }
    
        protected void grdCentroCusto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdCentroCusto.ShowFooter = e.CommandName != "Edit";
        }

        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            string idLoja = ((DropDownList)grdCentroCusto.FooterRow.FindControl("drpLoja")).SelectedValue;
            string descricao = ((TextBox)grdCentroCusto.FooterRow.FindControl("txtDescricao")).Text;
            string codigoTipo = ((HiddenField)grdCentroCusto.FooterRow.FindControl("hdfCodigoTipo")).Value;

            CentroCusto novo = new CentroCusto();
            novo.IdLoja = Glass.Conversoes.StrParaInt(idLoja);
            novo.Descricao = descricao;
            novo.CodigoTipo = Glass.Conversoes.StrParaInt(codigoTipo);

            novo.Tipo = (Data.Model.TipoCentroCusto)Enum.Parse(typeof(Data.Model.TipoCentroCusto),
                    ((DropDownList)grdCentroCusto.FooterRow.FindControl("drpTipo")).SelectedValue);

            CentroCustoDAO.Instance.Insert(novo);

            grdCentroCusto.DataBind();
        }
    
        protected void odsCentroCusto_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir centro de custo.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsCentroCusto_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir centro de custo.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsCentroCusto_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar centro de custo.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetTipos(string idLoja)
        {
            if (idLoja == "")
                return "";
    
            string retorno = "";
            foreach (GenericModel g in DataSourcesEFD.Instance.GetTipoCentroCusto(Glass.Conversoes.StrParaUint(idLoja)))
                retorno += String.Format("<option value='{0}'>{1}</option>", g.Id, g.Descr);
    
            return retorno;
        }
    
        #endregion
    }
}
