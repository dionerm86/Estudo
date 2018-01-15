using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using System.Drawing;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class LstAlterarSituacaoProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Producao.LstAlterarSituacaoProducao));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void lnkPesquisar_Click(object sender, EventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void lblSitProd_Load(object sender, EventArgs e)
        {
            // Mostra o link para visualizar as peças deste pedido na produção se a situação não for "-"
            // e se a empresa controla produção
            // TODO: O texto está vindo vazio
            if (((WebControl)sender).ID == "lnkSitProd")
                ((WebControl)sender).Visible = ((LinkButton)sender).Text != "-" && PCPConfig.ControlarProducao;
            else
                ((WebControl)sender).Visible = ((Label)sender).Text == "-" || !PCPConfig.ControlarProducao;
        }
    
        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Producao")
            {
                Response.Redirect("~/Cadastros/Producao/LstProducao.aspx?idPedido=" + e.CommandArgument);
            }
        }
    
        protected void grdPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            Glass.Data.Model.Pedido item = e.Row.DataItem as Glass.Data.Model.Pedido;
            if (item == null)
                return;
    
            if (item.CorLinhaLista != Color.Black)
                foreach (TableCell c in e.Row.Cells)
                {
                    c.ForeColor = item.CorLinhaLista;
    
                    foreach (Control c1 in c.Controls)
                        if (c1 is WebControl)
                            ((WebControl)c1).ForeColor = c.ForeColor;
                }
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (String.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }
    
        #endregion
    
        protected void odsPedido_Updated(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar pedido.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                grdPedido.DataBind();
        }
    }
}
