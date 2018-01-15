using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPlanoConta : System.Web.UI.Page
    {
        #region Propriedades

        /// <summary>
        /// Fluxo de negócio do plano de conta.
        /// </summary>
        public static Glass.Financeiro.Negocios.IPlanoContasFluxo PlanoContasFluxo
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Financeiro.Negocios.IPlanoContasFluxo>();
            }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdPlanoConta.Register(true, true);
            odsPlanoConta.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadPlanoConta));
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            if (drpGrupoContaFiltro.SelectedValue == "0")
            {
                Glass.MensagemAlerta.ShowMsg("Selecione o grupo de contas.", Page);
                return;
            }
    
            var planoContas = new Financeiro.Negocios.Entidades.PlanoContas();
            planoContas.Descricao = ((TextBox)grdPlanoConta.FooterRow.FindControl("txtDescricao")).Text;
            planoContas.IdGrupo = drpGrupoContaFiltro.SelectedValue.StrParaInt();
            var situacao = Situacao.Ativo;
            Enum.TryParse<Glass.Situacao>(((DropDownList)grdPlanoConta.FooterRow.FindControl("drpSituacao")).SelectedValue, out situacao);
            planoContas.Situacao = situacao;

            var resultado = PlanoContasFluxo.SalvarPlanoContas(planoContas);
    
            if (resultado)
            {
                grdPlanoConta.DataBind();
                Glass.MensagemAlerta.ShowMsg("Plano de conta inserido.", Page);
            }
            else
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Plano de Conta.", resultado);
        }
    
        protected void drpGrupoContaFiltro_DataBound(object sender, EventArgs e)
        {
            if (PlanoContasDAO.Instance.GetCountReal(Glass.Conversoes.StrParaUint(drpGrupoContaFiltro.Text), Glass.Conversoes.StrParaInt(drpSituacao.SelectedValue)) == 0)
                foreach (TableCell c in grdPlanoConta.Rows[0].Cells)
                    c.Text = String.Empty;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPlanoConta.PageIndex = 0;
            grdPlanoConta.DataBind();
        }
    
        protected void lblDescrGrupo_Load(object sender, EventArgs e)
        {
            if (drpGrupoContaFiltro.SelectedItem.Text.ToLower() != "todos")
                ((Label)sender).Text = drpGrupoContaFiltro.SelectedItem.Text;
        }

        #region Métodos ajax

        [Ajax.AjaxMethod()]
        public static string SetExibirDre(int idConta, bool valor)
        {
            try
            {
                var planoConta = PlanoContasFluxo.ObtemPlanoContas(idConta);

                if (planoConta == null)
                    return "Erro|O plano de contas não existe.";

                // Altera a propriedade ExibirDre.
                planoConta.ExibirDre = valor;
                var resultado = PlanoContasFluxo.SalvarPlanoContas(planoConta);

                if (!resultado)
                    return string.Format("Erro|{0}", resultado.Message.Format());

                return "Plano de contas atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                return string.Format("Erro|{0}", ex.Message);
            }
        }

        #endregion
    }
}
