using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadGruposConta : System.Web.UI.Page
    {
        #region Propriedades

        /// <summary>
        /// Fluxo de negócio do plano de conta.
        /// </summary>
        public static Glass.Financeiro.Negocios.IPlanoContasFluxo PlanoContaFluxo
        {
            get
            {
                return Microsoft.Practices.ServiceLocation
                    .ServiceLocator.Current
                    .GetInstance<Glass.Financeiro.Negocios.IPlanoContasFluxo>();
            }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            odsGruposConta.Register();
            grdGruposConta.Register(true, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grdGruposConta.RowUpdating += grdGruposConta_RowUpdating;
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadGruposConta));
        }

        private void grdGruposConta_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            string idCategoriaConta = ((DropDownList)grdGruposConta.FooterRow.FindControl("drpCategoria")).SelectedValue;
    
            var grupoConta = new Glass.Financeiro.Negocios.Entidades.GrupoConta();
            grupoConta.Descricao = ((TextBox)grdGruposConta.FooterRow.FindControl("txtDescricao")).Text;
            Glass.Situacao situacao = Situacao.Ativo;

            if (Enum.TryParse<Glass.Situacao>(((DropDownList)grdGruposConta.FooterRow.FindControl("drpSituacao")).SelectedValue, out situacao))
                grupoConta.Situacao = situacao;
    
            grupoConta.IdCategoriaConta = idCategoriaConta.StrParaIntNullable();

            var resultado = PlanoContaFluxo.SalvarGrupoConta(grupoConta);

            if (resultado)
            {
                grdGruposConta.DataBind();
                Glass.MensagemAlerta.ShowMsg("Grupo inserido.", Page);
            }
            else
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Grupo de Conta.", resultado);
        }
    
        [Ajax.AjaxMethod()]
        public static string SetPontoEquilibrio(int idGrupo, bool valor)
        {
            try
            {
                var grupoConta = PlanoContaFluxo.ObtemGrupoConta(idGrupo);

                if (grupoConta == null)
                    return "Erro|O grupo não existe.";

                // Altera o ponto de equilibrio
                grupoConta.PontoEquilibrio = valor;
                var resultado = PlanoContaFluxo.SalvarGrupoConta(grupoConta);

                if (!resultado)
                    return string.Format("Erro|{0}", resultado.Message.Format());

                return "Grupo de contas atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                return string.Format("Erro|{0}", ex.Message);
            }
        }

        protected void grdGruposConta_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Up" || e.CommandName == "Down")
            {
                var resultado = PlanoContaFluxo.AlterarPosicaoGrupoConta(e.CommandArgument.ToString().StrParaInt(), e.CommandName == "Up");

                if (resultado)
                    grdGruposConta.DataBind();

                else
                    Glass.MensagemAlerta.ErrorMsg("Falha ao mudar posição do beneficiamento.", resultado);
            }
        }
    }
}
