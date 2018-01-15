using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Fiscal.Negocios.Entidades;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlIcmsProdutoPorUf : BaseUserControl
    {
        #region Variáveis Locais

        private IList<IcmsProdutoUf> _icmsProduto;

        #endregion

        #region Propriedades

        public IList<IcmsProdutoUf> AliquotasIcms
        {
            get { return _icmsProduto; }
            set { _icmsProduto = value; }
        }
    
        public string ValidationGroup
        {
            get { return item0_ctvExcecao.ValidationGroup; }
            set { item0_ctvExcecao.ValidationGroup = value; }
        }
    
        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera os Mva's gerais.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Glass.Fiscal.Negocios.Entidades.IcmsProdutoUf> ObtemIcmsGerais()
        {
            if (_icmsProduto == null)
                return new Glass.Fiscal.Negocios.Entidades.IcmsProdutoUf[0];

            IEnumerable<Glass.Fiscal.Negocios.Entidades.IcmsProdutoUf> resultado = null;

            int maiorQtd = 0;

            // Localiza o Icms padrão, pelo maior agrupamento do IdTipoCliente, AliquotaIntraestadual e AliquotaInterestadual
            foreach (var i in _icmsProduto
                .Where(f => !f.IdTipoCliente.HasValue)
                .GroupBy(f => new { f.AliquotaIntraestadual, f.AliquotaInterestadual, f.AliquotaInternaDestinatario, f.IdTipoCliente }))
            {
                var count = i.Count();

                if (count > maiorQtd)
                {
                    resultado = i;
                    maiorQtd = count;
                }
            }

            return resultado ?? _icmsProduto;
        }

        #endregion

        #region Métodos Protegidos

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) return;

            var localizacaoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.ILocalizacaoFluxo>();

            if (_icmsProduto == null)
                _icmsProduto = new List<IcmsProdutoUf>();

            var aliquotaIntraestadual = txtIcmsIntra.Text.StrParaFloat();
            var aliquotaInterestadual = txtIcmsInter.Text.StrParaFloat();
            var aliquotaInternaDestinatario = txtIcmsInternaDest.Text.StrParaFloat();
            var ufs = localizacaoFluxo.ObtemUfs().ToArray();
            var atualizados = new List<IcmsProdutoUf>();

            // Processa o icms geral
            foreach (var origem in ufs)
                foreach (var destino in ufs)
                {
                    var icms = _icmsProduto.FirstOrDefault(f => 
                        !f.IdTipoCliente.HasValue && 
                        f.UfOrigem == origem.Name && 
                        f.UfDestino == destino.Name);

                    if (icms == null)
                    {
                        icms = new IcmsProdutoUf
                        {
                            UfOrigem = origem.Name,
                            UfDestino = destino.Name
                        };
                        _icmsProduto.Add(icms);
                    }

                    icms.AliquotaIntraestadual = aliquotaIntraestadual;
                    icms.AliquotaInterestadual = aliquotaInterestadual;
                    icms.AliquotaInternaDestinatario = aliquotaInternaDestinatario;
                    atualizados.Add(icms);
                }
            
            // Exceções
            if (!String.IsNullOrEmpty(hdfExcecoes.Value))
            {
                string[] dadosItens = hdfExcecoes.Value.Split('/');
                foreach (string dadosItem in dadosItens)
                {
                    if (String.IsNullOrEmpty(dadosItem))
                        continue;

                    string[] item = dadosItem.Split('|');

                    var idTipoCliente = item[5].StrParaIntNullable();
                    var ufOrigem = item[0];
                    var ufDestino = item[1];

                    var icms = _icmsProduto != null ?
                        _icmsProduto.FirstOrDefault(f =>
                            f.IdTipoCliente == idTipoCliente &&
                            f.UfOrigem == ufOrigem &&
                            f.UfDestino == ufDestino) : null;

                    if (icms == null)
                    {
                        icms = new IcmsProdutoUf()
                        {
                            IdTipoCliente = idTipoCliente,
                            UfOrigem = ufOrigem,
                            UfDestino = ufDestino
                        };
                        _icmsProduto.Add(icms);
                    }

                    icms.AliquotaIntraestadual = item[2].StrParaFloat();
                    icms.AliquotaInterestadual = item[3].StrParaFloat();
                    icms.AliquotaInternaDestinatario = item[4].StrParaFloat();
                    atualizados.Add(icms);
                }
            }

            // Recupera os Icms que devem ser apagados
            foreach (var i in _icmsProduto.Where(f => !atualizados.Exists(x => f.Equals(x))).ToArray())
                _icmsProduto.Remove(i);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (_icmsProduto == null || _icmsProduto.Count == 0)
                return;

            var geral = ObtemIcmsGerais().FirstOrDefault();

            if (geral != null)
            {
                txtIcmsIntra.Text = geral.AliquotaIntraestadual.ToString();
                txtIcmsInter.Text = geral.AliquotaInterestadual.ToString();
                txtIcmsInternaDest.Text = geral.AliquotaInternaDestinatario.ToString();
            }

            var itensExcecao = geral != null ?
                _icmsProduto.Where(f => 
                    f.AliquotaIntraestadual != geral.AliquotaIntraestadual ||
                    f.AliquotaInterestadual != geral.AliquotaInterestadual ||
                    f.AliquotaInternaDestinatario != geral.AliquotaInternaDestinatario ||
                    f.IdTipoCliente != geral.IdTipoCliente).ToList() : _icmsProduto;

            for (int i = 0; i < itensExcecao.Count; i++)
            {
                if (i > 0)
                    Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_exibe_item" + i,
                        this.ClientID + ".AdicionarItemExcecao();\n", true);
                
                Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_preenche_item" + i,
                    this.ClientID + ".PreencheItemExcecao(" + i + ", '" + itensExcecao[i].UfOrigem + "', '" +
                    itensExcecao[i].UfDestino + "', '" + itensExcecao[i].AliquotaIntraestadual + "', '" +
                    itensExcecao[i].AliquotaInterestadual + "', '" + itensExcecao[i].AliquotaInternaDestinatario + "', '" +
                    itensExcecao[i].IdTipoCliente + "');\n", true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "script"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "script", this.ResolveClientUrl("~/Scripts/ctrlIcmsProdutoPorUf.js"));
    
            // Registra a função de submit
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), this.ID + "submit", this.ClientID + ".PreparaSubmit();\n");
    
            ctvGeral.ClientValidationFunction = this.ClientID + ".ValidaItemGeral";
            item0_drpTipoCliente.Attributes.Add("OnChange", this.ClientID + ".BloqueiaUfsSelecionadas()");
            item0_drpUfOrigemExcecao.Attributes.Add("OnChange", this.ClientID + ".BloqueiaUfsSelecionadas()");
            item0_drpUfDestinoExcecao.Attributes.Add("OnChange", this.ClientID + ".BloqueiaUfsSelecionadas()");
            item0_ctvExcecao.ClientValidationFunction = this.ClientID + ".ValidaItemExcecao";
            item0_imgAdicionarExcecao.OnClientClick = this.ClientID + ".AdicionarItemExcecao(); return false";
            item0_imgExcluirExcecao.OnClientClick = this.ClientID + ".RemoverItemExcecao(); return false";
        }

        #endregion
    }
}
