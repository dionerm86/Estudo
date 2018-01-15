using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadEncontroContas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadEncontroContas));
    
            if (!IsPostBack && Request["idEncontroContas"] != null)
            {
                dtvEncontroContas.ChangeMode(DetailsViewMode.ReadOnly);
    
                UpdateTotais();
    
                dtvEncontroContas.FindControl("trTotalPagar").Visible = true;
                dtvEncontroContas.FindControl("trTotalReceber").Visible = true;
                dtvEncontroContas.FindControl("trSaldo").Visible = true;
            }
        }
    
        #region Clicks
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstEncontroContas.aspx");
        }
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                uint idEncontroContas = Glass.Conversoes.StrParaUint(Request["idEncontroContas"]);
                string dtVenc = ((TextBox)dtvEncontroContas.FindControl("ctrlDataVenc").FindControl("txtData")).Text;
    
                string msg = WebGlass.Business.EncontroContas.Fluxo.EncontroContas.Instance.Finalizar(idEncontroContas, dtVenc);
    
                Glass.MensagemAlerta.ShowMsg(msg, Page);
                FuncoesGerais.RedirecionaPagina("../Listas/LstEncontroContas.aspx", Page);
                
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar encontro de contas a pagar/receber.", ex, Page);
            }
            
        }
    
        #endregion
    
        #region Metodos AJAX
    
        /// <summary>
        /// Valida se o cliente e o fornecedor são os mesmos.
        /// </summary>
        /// <param name="obj"></param>
        [Ajax.AjaxMethod()]
        public string ValidaClienteFornecedor(string idCli, string idFornec)
        {
            return WebGlass.Business.EncontroContas.Fluxo.BuscarEValidar.Ajax.ValidaClienteFornecedor(idCli, idFornec);
        }
    
        /// <summary>
        /// Adiciona uma conta a receber ao encontro de contas
        /// </summary>
        /// <param name="idContaR"></param>
        [Ajax.AjaxMethod()]
        public string AddContaR(string idEncontroContas, string idContaR)
        {
            return WebGlass.Business.EncontroContas.Fluxo.ContaReceber.Ajax.AddContaR(idEncontroContas, idContaR);
        }
    
        /// <summary>
        /// Adiciona uma conta a pagar ao encontro de contas
        /// </summary>
        /// <param name="idContaPg"></param>
        [Ajax.AjaxMethod()]
        public string AddContaPg(string idEncontroContas, string idContaPg)
        {
            return WebGlass.Business.EncontroContas.Fluxo.ContaPagar.Ajax.AddContaPg(idEncontroContas, idContaPg);
        }
    
        #endregion
    
        #region Metodos
    
        /// <summary>
        /// Atualiza os campos de total a pagar, receber e saldo
        /// </summary>
        public void UpdateTotais()
        {
            if (dtvEncontroContas.CurrentMode != DetailsViewMode.ReadOnly)
                return;
    
            uint idEncontroContas = Glass.Conversoes.StrParaUint(Request["idEncontroContas"]);
    
            decimal tPagar = 0;
            decimal tReceber = 0;
            decimal tSaldo = 0;
    
            WebGlass.Business.EncontroContas.Fluxo.EncontroContas.Instance.GetTotais(idEncontroContas, ref tPagar, ref tReceber, ref tSaldo);
    
            ((Label)dtvEncontroContas.FindControl("lblTotalPagar")).Text = tPagar.ToString("C");
            ((Label)dtvEncontroContas.FindControl("lblTotalReceber")).Text = tReceber.ToString("C");
    
            string saldo = tSaldo.ToString("C");
    
            if (tSaldo == 0)
                return;
    
            if (tPagar > tReceber)
                ((Label)dtvEncontroContas.FindControl("lblSaldo")).Text = saldo + " (a pagar)";
            else
                ((Label)dtvEncontroContas.FindControl("lblSaldo")).Text = saldo + " (a receber)";
        }
    
        #endregion
    
        protected void odsEncontroContas_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
                Response.Redirect("~/Cadastros/CadEncontroContas.aspx?idEncontroContas=" + e.ReturnValue);
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao inserir encontro de contas a pagar/receber.", e.Exception.InnerException, this);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsEncontroContas_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                Response.Redirect(Request.Url.ToString());
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao atualizar pagamento antecipado.", e.Exception.InnerException, this);
                e.ExceptionHandled = true;
            }
        }
    
        protected void grdContaREncontroContas_DataBound(object sender, EventArgs e)
        {
            GridView grdContaREncontroContas = (GridView)sender;
            if (grdContaREncontroContas.Rows.Count != 1)
                return;
    
            uint idEncontroContas = Glass.Conversoes.StrParaUint(Request["idEncontroContas"]);
            if (ContasReceberEncontroContasDAO.Instance.GetListCountReal(idEncontroContas) == 0)
                grdContaREncontroContas.Rows[0].Visible = false;
        }
    
        protected void grdContaPgEncontroContas_DataBound(object sender, EventArgs e)
        {
            GridView grdContaPgEncontroContas = (GridView)sender;
            if (grdContaPgEncontroContas.Rows.Count != 1)
                return;
    
            uint idEncontroContas = Glass.Conversoes.StrParaUint(Request["idEncontroContas"]);
            if (ContasPagarEncontroContasDAO.Instance.GetListCountReal(idEncontroContas) == 0)
                grdContaPgEncontroContas.Rows[0].Visible = false;
        }
    
        protected void odsContaPgEncontroContas_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            UpdateTotais();
        }
    
        protected void odsContaREncontroContas_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            UpdateTotais();
        }
    }
}
