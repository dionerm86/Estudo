using System;
using System.Web.UI;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlNaturezaOperacao : BaseUserControl
    {
        private string _callback;

        #region Propriedades

        /// <summary>
        /// TODO: Método usado para auxiliar a migração.
        /// </summary>
        public int? IdNaturezaOperacao
        {
            get { return (int?)CodigoNaturezaOperacao; }
            set { CodigoNaturezaOperacao = (uint?)value; }
        }

        public uint? CodigoNaturezaOperacao
        {
            get { return Glass.Conversoes.StrParaUintNullable(selNaturezaOperacao.Valor); }
            set
            {
                using (var inst = WebGlass.Business.NaturezaOperacao.Fluxo.BuscarEValidar.Instance)
                {
                    if (value != null && inst.Existe(value.Value))
                    {
                        selNaturezaOperacao.Valor = value.ToString();
                        selNaturezaOperacao.Descricao = inst.ObtemCodigoCompleto(value.Value);

                        Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_valor",
                            "document.getElementById('" + selNaturezaOperacao.FindControl("txtDescr").
                            ClientID + "').onblur();\n", true);
                    }
                    else
                    {
                        selNaturezaOperacao.Valor = null;
                        selNaturezaOperacao.Descricao = null;
                    }
                }
            }
        }

        public bool PermitirVazio
        {
            get { return selNaturezaOperacao.PermitirVazio; }
            set { selNaturezaOperacao.PermitirVazio = value; }
        }

        public bool FazerPostBackBotaoPesquisar
        {
            get { return selNaturezaOperacao.FazerPostBackBotaoPesquisar; }
            set { selNaturezaOperacao.FazerPostBackBotaoPesquisar = value; }
        }

        public bool Enabled
        {
            get { return selNaturezaOperacao.Enabled; }
            set { selNaturezaOperacao.Enabled = value; }
        }

        public string FuncaoValidacaoJavaScript
        {
            get { return selNaturezaOperacao.Validador.ClientValidationFunction; }
            set { selNaturezaOperacao.Validador.ClientValidationFunction = value; }
        }

        public string ValidationGroup
        {
            get { return selNaturezaOperacao.ValidationGroup; }
            set { selNaturezaOperacao.ValidationGroup = value; }
        }

        public string ErrorMessage
        {
            get { return selNaturezaOperacao.ErrorMessage; }
            set { selNaturezaOperacao.ErrorMessage = value; }
        }

        public Control CampoCstIcms { get; set; }
        public Control CampoPercReducaoBcIcms { get; set; }
        public Control CampoCstIpi { get; set; }
        public Control CampoCstPisCofins { get; set; }
        public Control CampoCsosn { get; set; }

        public string Callback
        {
            get { return _callback ?? String.Empty; }
            set { _callback = value; }
        }

        #endregion

        #region Métodos Ajax

        [Ajax.AjaxMethod]
        public string ObtemDadosComplementares(string codigoNaturezaOperacao)
        {
            return WebGlass.Business.NaturezaOperacao.Fluxo.BuscarEValidar.Ajax.ObtemDadosComplementares(codigoNaturezaOperacao);
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlNaturezaOperacao));

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlNaturezaOperacao"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlNaturezaOperacao",
                    this.ResolveClientUrl("~/Scripts/ctrlNaturezaOperacao.js"));

            selNaturezaOperacao.CallbackSelecao = this.ClientID + ".Callback";

            // Este atributo deve ser repassado até chegar no campo txtDescr do controle SelPopup que é referenciado neste controle.
            if (this.Attributes["onchange"] != null)
                selNaturezaOperacao.Attributes.Add("onchange", this.Attributes["onchange"]);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string dadosControle = @"
                CampoCstIcms: {0},
                CampoPercReducaoBcIcms: {1},
                CampoCstIpi: {2},
                CampoCstPisCofins: {3},
                Callback: {4},
                CampoCsosn: {5},
                Atual: {6},
                CfopDevolucao: {7}
            ";

            var naturezaOperacao = CodigoNaturezaOperacao.HasValue && CodigoNaturezaOperacao > 0 ?
                Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Fiscal.Negocios.ICfopFluxo>()
                    .ObtemNaturezaOperacao((int)CodigoNaturezaOperacao.Value) :
                    null;

            dadosControle = String.Format(dadosControle,
                CampoCstIcms != null ? "'" + CampoCstIcms.ClientID + "'" : "null",
                CampoPercReducaoBcIcms != null ? "'" + CampoPercReducaoBcIcms.ClientID + "'" : "null",
                CampoCstIpi != null ? "'" + CampoCstIpi.ClientID + "'" : "null",
                CampoCstPisCofins != null ? "'" + CampoCstPisCofins.ClientID + "'" : "null",
                Callback != null ? "'" + Callback + "'" : "null",
                CampoCsosn != null ? "'" + CampoCsosn.ClientID + "'" : "null",
                CodigoNaturezaOperacao.GetValueOrDefault(),
                (naturezaOperacao != null && naturezaOperacao.Cfop != null ? naturezaOperacao.Cfop.VerificaCfopDevolucao() : false).ToString().ToLower()
            );

            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID,
            String.Format("var {0} = new NaturezaOperacaoType('{0}', {1});", this.ClientID, "{" + dadosControle + "}"), true);
        }
    }
}
