using Glass.Fiscal.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRegraNaturezaOperacao : System.Web.UI.Page
    {
        #region Variáveis Locais

        /// <summary>
        /// Instancia da rota do cliente que está sendo inserida.
        /// </summary>
        private Glass.Fiscal.Negocios.Entidades.RegraNaturezaOperacao regraNaturezaOperacao;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            odsRegraNaturezaOperacao.Register();
            dtvRegraNaturezaOperacao.Register("~/Listas/LstRegraNaturezaOperacao.aspx");

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request["id"] != null)
                dtvRegraNaturezaOperacao.ChangeMode(DetailsViewMode.Edit);
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstRegraNaturezaOperacao.aspx");
        }
    
        protected void ctrlSelCorProd_Load(object sender, EventArgs e)
        {
            var grupoSubgrupo = dtvRegraNaturezaOperacao.FindControl("ctrlSelGrupoSubgrupoProd");
            if (grupoSubgrupo == null)
                return;
    
            var grupo = grupoSubgrupo.FindControl("drpGrupoProd");
            
            var ctrl = sender as Glass.UI.Web.Controls.ctrlSelCorProd;
            ctrl.ControleGrupo = grupo;
        }

        protected void dtvRegraNaturezaOperacao_DataBound(object sender, EventArgs e)
        {
            var dtvRegrasNatureza = (DetailsView)sender;

            if (dtvRegrasNatureza.DataItem == null)
                return;

            var regra = (RegraNaturezaOperacao)dtvRegrasNatureza.DataItem;

            if (string.IsNullOrWhiteSpace(regra.UfDest))
                return;

            var ufs = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.ILocalizacaoFluxo>().ObtemUfs();

            var idsUf = new List<int>();

            foreach (var uf in regra.UfDest.Split(','))
            {
                idsUf.Add(ufs.Where(f => f.Name == uf).FirstOrDefault().Id);
            }

            if (dtvRegrasNatureza.CurrentMode != DetailsViewMode.ReadOnly)
                ((Sync.Controls.CheckBoxListDropDown)dtvRegrasNatureza.FindControl("drpUfDestino")).SelectedValues = idsUf.ToArray();
        }
    }
}
