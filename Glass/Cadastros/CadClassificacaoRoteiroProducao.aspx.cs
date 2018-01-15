using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadClassificacaoRoteiroProducao : System.Web.UI.Page
    {
        #region Variaveis Locais

        private Glass.PCP.Negocios.Entidades.ClassificacaoRoteiroProducao _classificacao;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadClassificacaoRoteiroProducao));

            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["idClassificacao"]))
                {
                    dtvClassificacao.ChangeMode(DetailsViewMode.Insert);
                    lblSubtitle.Visible = false;
                    lnkAssociarRoteiro.Visible = false;
                    grdRoteiroProducao.Visible = false;
                }

                hdfIdClassificacao.Value = Request["idClassificacao"];
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstClassificacaoRoteiroProducao.aspx");
        }

        protected void odsClassificacao_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
                Response.Redirect(Request.Url + "?idClassificacao=" + _classificacao.IdClassificacaoRoteiroProducao);
        }

        protected void odsClassificacao_Inserting(object sender, Colosoft.WebControls.VirtualObjectDataSourceMethodEventArgs e)
        {
            _classificacao = (Glass.PCP.Negocios.Entidades.ClassificacaoRoteiroProducao)e.InputParameters[0];
        }

        #region Metodos Ajax

        [Ajax.AjaxMethod()]
        public void AssociaRoteiro(string idRoteiro, string idClassificacao)
        {
            WebGlass.Business.RoteiroProducao.Fluxo.RoteiroProducao.Instance
                .AssociaRoteiroClassificacao(Conversoes.StrParaInt(idRoteiro), Conversoes.StrParaInt(idClassificacao));
        }

        [Ajax.AjaxMethod()]
        public void AssociaSubgrupo(string idSubgrupo, string idClassificacao)
        {
            WebGlass.Business.RoteiroProducao.Fluxo.RoteiroProducao.Instance
                .AssociaSubgrupoClassificacao(Conversoes.StrParaInt(idSubgrupo), Conversoes.StrParaInt(idClassificacao));
        }


        #endregion
    }
}