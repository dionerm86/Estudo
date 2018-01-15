using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlLojaNCM : System.Web.UI.UserControl
    {
        #region Variáveis Locais

        private IList<Glass.Global.Negocios.Entidades.ProdutoNCM> _ncms;

        #endregion

        #region Propriedades

        /// <summary>
        /// NCM
        /// </summary>
        public IList<Glass.Global.Negocios.Entidades.ProdutoNCM> NCMs
        {
            get 
            {
                // Recupera os ncms
                var ncms = hdfNCM.Value.Split(';')
                    .Where(f => !string.IsNullOrEmpty(f))
                    .Select(f => f)
                    .ToArray();

                if (ncms.Count() == 0)
                    hdfIdLoja.Value = "";

                // Recupera os identificadores das lojas
                var idLojas = hdfIdLoja.Value.Split(';')
                    .Where(f => !string.IsNullOrEmpty(f))
                    .Select(f => int.Parse(f));

                var index = 0;
                var lojasNcms = idLojas.Select(f => new { IdLoja = f, NCM = ncms[index++] }).ToList();

                if (_ncms != null)
                {
                    // Carrega as baixas do estoque
                    for (var i = 0; i < _ncms.Count; i++)
                    {
                        var n = _ncms[i];

                        var ncm = lojasNcms.FirstOrDefault(f => f.IdLoja == n.IdLoja && f.NCM == n.NCM);
                        if (ncm == null)
                        {
                            _ncms.RemoveAt(i--);
                        }
                        else
                        {
                            n.NCM = ncm.NCM;
                            n.IdLoja = ncm.IdLoja;
                            lojasNcms.Remove(ncm);
                        }
                    }

                    foreach (var n in lojasNcms)
                        _ncms.Add(new Glass.Global.Negocios.Entidades.ProdutoNCM
                        {
                            IdLoja = n.IdLoja,
                            NCM = n.NCM
                        });
                }
                else
                {
                    _ncms = new List<Glass.Global.Negocios.Entidades.ProdutoNCM>
                    (lojasNcms.Select(f => new Glass.Global.Negocios.Entidades.ProdutoNCM
                    {
                        IdLoja = f.IdLoja,
                        NCM = f.NCM
                    }));
                }
 
                return _ncms;
            }
            set { _ncms = value; }
        }

        #endregion

        #region Métodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlLojaNCM_script")){
                Page.ClientScript.RegisterClientScriptInclude("ctrlLojaNCM_script", ResolveUrl("~/Scripts/ctrlLojaNCM.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterStartupScript(typeof(string), this.ClientID, "<script>var " + this.ID + " = new ctrlLojaNCM('" + this.ClientID + "');</script>");
            }

            if (!IsPostBack)
            {
                imgAdicionar.OnClientClick = this.ID + ".adicionarLinha(); return false";
                imgRemover.OnClientClick = this.ID + ".removerLinha(); return false";
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            IEnumerable<int> idLojas = null;
            IEnumerable<string> ncms = null;

            if (_ncms != null && _ncms.Count > 0)
            {
                idLojas = _ncms.Select(f => f.IdLoja);
                ncms = _ncms.Select(f => f.NCM);
            }
            else
                return;

            hdfIdLoja.Value = string.Join(";", idLojas.Select(f => f.ToString()).ToArray());
            hdfNCM.Value = string.Join(";", ncms.Select(f => f).ToArray());

            Page.ClientScript.RegisterStartupScript(GetType(), "iniciar" + new Random().Next(0, 1000), this.ID + ".carregaNCMs();", true);
        }

        #endregion
    }
}