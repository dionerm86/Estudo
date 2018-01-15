using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlAverbacaoSeguroMDFe : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdManifestoEletronico { get; set; }

        public List<AverbacaoSeguroMDFe> AverbacaoSeguro
        {
            get
            {
                var averbacoes = new List<AverbacaoSeguroMDFe>();
                var numerosAverbacao = hdfNumerosAverbacao.Value.Split(';').ToList();

                foreach(var numeroAverbacao in numerosAverbacao)
                {
                    if (numeroAverbacao != "")
                        averbacoes.Add(new AverbacaoSeguroMDFe
                        {
                            IdManifestoEletronico = IdManifestoEletronico,
                            NumeroAverbacao = numeroAverbacao
                        });
                }

                return averbacoes;
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    var numerosAverbacao = string.Empty;
                    foreach (var t in value)
                    {
                        numerosAverbacao += ";" + t.NumeroAverbacao;
                    }

                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaNumeroAverbacaoInicial('{0}', '{1}');", this.ClientID, numerosAverbacao), true);

                    imgAdicionar.OnClientClick = "adicionarLinhaNumeroAverbacao('" + this.ClientID + "'); return false";
                    imgRemover.OnClientClick = "removerLinhaNumeroAverbacao('" + this.ClientID + "'); return false";
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            txtNumeroAverbacao.Attributes.Add("onblur", "pegarValorNumeroAverbacao('" + this.ClientID + "')");

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlAverbacaoSeguroMDFe_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlAverbacaoSeguroMDFe_script", ResolveUrl("~/Scripts/MDFe/ctrlAverbacaoSeguroMDFe.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinhaNumeroAverbacao('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinhaNumeroAverbacao('" + this.ClientID + "'); return false";
        }
    }
}