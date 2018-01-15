using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Data.Model;
using Glass.Configuracoes;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaContasPagar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                if (FinanceiroConfig.FinanceiroPagto.ImpedirPagamentoPorLoja && UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.Administrador)
                {
                    drpLoja.SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
                    lblLoja.Style.Add("display", "none");
                    drpLoja.Style.Add("display", "none");
                }
            }
        }

        protected bool EditarDataVencimento()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.EditarDataVencimentoContaPagar);
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    
        protected void odsContasPagar_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
    
                if (e.Exception.InnerException != null)
                    Glass.MensagemAlerta.ShowMsg(e.Exception.InnerException.Message, Page);
                else
                    Glass.MensagemAlerta.ErrorMsg("Falha ao alterar data de vencimento.", e.Exception, Page);
            }
        }
    
        protected void grdConta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            if (((ContasPagar)e.Row.DataItem).PrevisaoCustoFixo)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Blue;
            else if(((ContasPagar)e.Row.DataItem).Paga)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.MediumSeaGreen;
            else if (((ContasPagar)e.Row.DataItem).PossuiNfDevolucao)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Red;
        }
    
        protected void grdConta_Sorted(object sender, EventArgs e)
        {
            hdfOrdenar.Value = grdConta.SortExpression + (grdConta.SortDirection == SortDirection.Descending ? " Desc" : "");
        }
    
        protected void drpTipo_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                drpTipo.Items.Add(new ListItem(FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil, "1"));
                drpTipo.Items.Add(new ListItem(FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil, "2"));
            }
        }

        protected void drpPlanoConta_DataBinding(object sender, EventArgs e)
        {
            var idContaPg = ((HiddenField)((DropDownList)sender).Parent.FindControl("hdfIdContaPg")).Value.StrParaUint();
            var planoConta = PlanoContasDAO.Instance.GetByIdConta((uint)ContasPagarDAO.Instance.ObtemIdConta(null, (int)idContaPg));

            // Se o funcionário deste pedido estiver inativo, inclui o mesmo na listagem para não ocorrer erro
            if (!PlanoContasDAO.Instance.GetPlanoContas(2).Any(f => f.IdConta == planoConta.IdConta))
            {                
                ((DropDownList)sender).Items.Add(new ListItem(planoConta.DescrPlanoGrupo, planoConta.IdConta.ToString()));
            }
        }
    }
}
