using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetReajustePreco : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProdutosReajustados.PageIndex = 0;
        }
    
        protected void drpGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdfNaoReajustados.Value = String.Empty;
        }
    
        protected void drpSubgrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdfNaoReajustados.Value = String.Empty;
        }
    
        /// <summary>
        /// Evento da grid de produtos reajustados
        /// </summary>
        protected void grdProdutosReajustados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemReajuste")
            {
                string novoId = e.CommandArgument.ToString();
    
                if (hdfNaoReajustados.Value == String.Empty)
                    hdfNaoReajustados.Value = novoId;
                else
                {
                    string[] idsProd = hdfNaoReajustados.Value.Split(',');
    
                    // Se produto já tiver sido removido, ignora
                    foreach (string id in idsProd)
                        if (id == novoId) return;
    
                    // Remove o produto da lista de reajustados
                    hdfNaoReajustados.Value += "," + novoId;
                }
            }
        }
    
        /// <summary>
        /// Evento da grid de produtos não reajustados
        /// </summary>
        protected void grdProdutosNaoReajustados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddReajuste")
            {
                string novoId = e.CommandArgument.ToString();
    
                string[] idsProd = hdfNaoReajustados.Value.Split(',');
    
                // Limpa hiddenfield de produtos não reajustados
                hdfNaoReajustados.Value = String.Empty;
    
                // Readiciona valores no hiddenfield, excluindo este valor que será reajustado
                foreach (string id in idsProd)
                    if (id != novoId) hdfNaoReajustados.Value += id + ",";
    
                hdfNaoReajustados.Value = hdfNaoReajustados.Value.TrimEnd(',');
            }
        }
    
        protected void btnReajustar_Click(object sender, EventArgs e)
        {
            try
            {
                // Aplica reajuste nos produtos e nos preços informados
                ProdutoDAO.Instance.AplicaReajuste(Glass.Conversoes.StrParaUint(drpGrupo.SelectedValue), Glass.Conversoes.StrParaUint(drpSubgrupo.SelectedValue), hdfNaoReajustados.Value,
                    decimal.Parse(txtReajuste.Text), radPercent.Checked ? 1 : 2, chkCustoFabBase.Checked, chkCustoCompra.Checked, 
                    chkBalcao.Checked, chkObra.Checked, chkAtacado.Checked, chkReposicao.Checked, chkFiscal.Checked, txtCodProd.Text, txtDescr.Text,
                    Glass.Conversoes.StrParaInt(drpSituacao.SelectedValue));
    
                txtReajuste.Text = String.Empty;
                hdfNaoReajustados.Value = String.Empty;
    
                grdProdutosReajustados.DataBind();
    
                Glass.MensagemAlerta.ShowMsg("Preços reajustados", Page);

                Response.Redirect("SetReajustePreco.aspx");
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao reajustar preços.", ex, Page);
            }
        }
    }
}
