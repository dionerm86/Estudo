using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Fiscal.Negocios.Entidades;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlMvaProdutoPorUf : BaseUserControl
    {
        #region Variáveis Locais

        private IList<Glass.Fiscal.Negocios.Entidades.MvaProdutoUf> _mva;

        #endregion

        #region Propriedades
    
        public IList<Glass.Fiscal.Negocios.Entidades.MvaProdutoUf> Mva
        {
            get { return _mva; }
            set { _mva = value; }
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
        private IEnumerable<Glass.Fiscal.Negocios.Entidades.MvaProdutoUf> ObtemMvasGerais()
        {
            if (_mva == null)
                return new Glass.Fiscal.Negocios.Entidades.MvaProdutoUf[0];

            IEnumerable<Glass.Fiscal.Negocios.Entidades.MvaProdutoUf> resultado = null;

            int maiorQtd = 0;
            // Localiza o MVA padrão, pelo maior agrupamento do MvaOriginal e do MvaSimples
            foreach (var i in _mva.GroupBy(f => string.Format("{0}:{1}", f.MvaOriginal, f.MvaSimples)))
            {
                var count = i.Count();
                if (count > maiorQtd)
                {
                    resultado = i;
                    maiorQtd = count;
                }
            }

            return resultado ?? _mva;
        }

        #endregion

        #region Métodos Protegidos

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (_mva == null || _mva.Count == 0) return;

            var geral = ObtemMvasGerais().FirstOrDefault();

            // Verifica se o mva geral foi encontrado
            if (geral != null)
            {
                txtMvaOriginal.Text = geral.MvaOriginal.ToString();
                txtMvaSimples.Text = geral.MvaSimples.ToString();
            }
    
            var itensExcecao = geral != null ? 
                _mva.Where(f => f.MvaOriginal != geral.MvaOriginal || f.MvaSimples != geral.MvaSimples).ToList() : _mva;

            for (int i = 0; i < itensExcecao.Count; i++)
            {
                if (i > 0)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_exibe_item" + i,
                        this.ClientID + ".AdicionarItemExcecao();\n", true);
                }

                Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_preenche_item" + i,
                    this.ClientID + ".PreencheItemExcecao(" + i + ", '" + itensExcecao[i].UfOrigem + "', '" +
                    itensExcecao[i].UfDestino + "', '" + itensExcecao[i].MvaOriginal + "', '" +
                    itensExcecao[i].MvaSimples + "');\n", true);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) return;

            var localizacaoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.ILocalizacaoFluxo>();

            if (_mva == null) 
                _mva = new List<Glass.Fiscal.Negocios.Entidades.MvaProdutoUf>();

            var mvaOriginal = txtMvaOriginal.Text.StrParaFloat();
            var mvaSimples = txtMvaSimples.Text.StrParaFloat();
            var ufs = localizacaoFluxo.ObtemUfs().ToArray();
            var atualizados = new List<MvaProdutoUf>();

            foreach (var origem in ufs)
                foreach (var destino in ufs)
                {
                    var mva = _mva.FirstOrDefault(f => f.UfOrigem == origem.Name && f.UfDestino == destino.Name);
                    if (mva == null)
                    {
                        mva = new Glass.Fiscal.Negocios.Entidades.MvaProdutoUf()
                        {
                            UfOrigem = origem.Name,
                            UfDestino = destino.Name,
                        };
                        _mva.Add(mva);
                    }

                    mva.MvaOriginal = txtMvaOriginal.Text.StrParaFloat();
                    mva.MvaSimples = txtMvaSimples.Text.StrParaFloat();
                    atualizados.Add(mva);
                }
            
            // Exceções
            if (!string.IsNullOrEmpty(hdfExcecoes.Value))
            {
                string[] dadosItens = hdfExcecoes.Value.Split('/');
                foreach (string dadosItem in dadosItens)
                {
                    if (string.IsNullOrEmpty(dadosItem))
                        continue;

                    string[] item = dadosItem.Split('|');

                    var ufOrigem = item[0];
                    var ufDestino = item[1];

                    // Tenta localiza o Mva existem para a origem e o destino
                    var mva = _mva != null ?
                        _mva.FirstOrDefault(f => f.UfOrigem == ufOrigem && f.UfDestino == ufDestino) : null;

                    if (mva == null)
                    {
                        // Adiciona o novo Mva
                        mva = new MvaProdutoUf
                        {
                            UfOrigem = ufOrigem,
                            UfDestino = ufDestino,
                        };
                        _mva.Add(mva);
                    }
                    
                    mva.MvaOriginal = Glass.Conversoes.StrParaFloat(item[2]);
                    mva.MvaSimples = Glass.Conversoes.StrParaFloat(item[3]);
                    atualizados.Add(mva);
                }
            }

            // Recupera os mvas que devem ser apagados
            foreach (var i in _mva.Where(f => !atualizados.Exists(x => f.Equals(x))).ToArray())
                _mva.Remove(i);
        }
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "script"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "script", this.ResolveClientUrl("~/Scripts/ctrlMvaProdutoPorUf.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
    
            // Registra a função de submit
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), this.ID + "submit", this.ClientID + ".PreparaSubmit();\n");
    
            ctvGeral.ClientValidationFunction = this.ClientID + ".ValidaItemGeral";
            item0_drpUfOrigemExcecao.Attributes.Add("OnChange", this.ClientID + ".BloqueiaUfsSelecionadas()");
            item0_drpUfDestinoExcecao.Attributes.Add("OnChange", this.ClientID + ".BloqueiaUfsSelecionadas()");
            item0_ctvExcecao.ClientValidationFunction = this.ClientID + ".ValidaItemExcecao";
            item0_imgAdicionarExcecao.OnClientClick = this.ClientID + ".AdicionarItemExcecao(); return false";
            item0_imgExcluirExcecao.OnClientClick = this.ClientID + ".RemoverItemExcecao(); return false";
        }

        #endregion
    }
}
