using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadImpostoServico : System.Web.UI.Page
    {
        private uint idFornec = 0;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadImpostoServico));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (Request["idImpostoServ"] != null && dtvImpostoServ.CurrentMode == DetailsViewMode.Insert)
                dtvImpostoServ.ChangeMode(DetailsViewMode.Edit);
    
            if (!IsPostBack && dtvImpostoServ.CurrentMode == DetailsViewMode.Insert)
                ((DropDownList)dtvImpostoServ.FindControl("drpLoja")).SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstImpostoServico.aspx");
        }
    
        protected void dtvImpostoServ_DataBound(object sender, EventArgs e)
        {

        }
    
        protected void txtValorIns_Load(object sender, EventArgs e)
        {
            Button botao = dtvImpostoServ.FindControl("btnEditar") as Button;
            if (botao != null)
                ((TextBox)sender).ReadOnly = !botao.Visible;
        }
    
        #region Eventos DataSource
    
        protected void odsImpostoServ_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Imposto/Serviço.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                Response.Redirect("../Listas/LstImpostoServico.aspx?IdImpostoServ=" + e.ReturnValue.ToString());
            }
        }
    
        protected void odsImpostoServ_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do Imposto/Serviço.", e.Exception, Page);
                e.ExceptionHandled = true;
                return;
            }
            else
                Response.Redirect("../Listas/LstImpostoServico.aspx?IdImpostoServ=" + Request["idImpostoServ"]);
        }
    
        #endregion
    
        #region Métodos Ajax
    
        /// <summary>
        /// Retorna o código que reprensenta a forma de pagamento "Cheque"
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetChequeCod()
        {
            return ((int)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio).ToString();
        }
    
        /// <summary>
        /// Retorna o código que reprensenta a forma de pagamento "Dinheiro"
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetDinheiroCod()
        {
            return ((int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro).ToString();
        }
    
        /// <summary>
        /// Retorna o Código/Descrição do produto
        /// </summary>
        [Ajax.AjaxMethod()]
        public string GetProduto(string idLoja, string idFornec, string codInterno)
        {
            return WebGlass.Business.Produto.Fluxo.BuscarEValidar.Ajax.GetProduto(idLoja, idFornec, codInterno);
        }
    
        #endregion
    
        #region Parcelas
    
        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;
            HiddenField hdfCalcularParcelas = (HiddenField)dtvImpostoServ.FindControl("hdfCalcularParcelas");
            HiddenField hdfExibirParcelas = (HiddenField)dtvImpostoServ.FindControl("hdfExibirParcelas");
            TextBox txtNumParc = (TextBox)dtvImpostoServ.FindControl("txtNumParc");
            TextBox txtTotal = (TextBox)dtvImpostoServ.FindControl("txtTotal");
            DropDownList drpFormaPagto = (DropDownList)dtvImpostoServ.FindControl("drpFormaPagto");
    
            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcelas;
            ctrlParcelas.CampoExibirParcelas = hdfExibirParcelas;
            ctrlParcelas.CampoParcelasVisiveis = txtNumParc;
            ctrlParcelas.CampoValorTotal = txtTotal;
            ctrlParcelas.NumParcelas = FinanceiroConfig.Compra.NumeroParcelasCompra;
            ctrlParcelas.CampoFormaPagto = drpFormaPagto;
        }
    
        #endregion
    
        protected uint GetIdFornec()
        {
            if (idFornec == 0 && !String.IsNullOrEmpty(Request["idImpostoServ"]))
                idFornec = ImpostoServDAO.Instance.ObtemIdFornec(Glass.Conversoes.StrParaUint(Request["idImpostoServ"]));
    
            return idFornec;
        }
    }
}
