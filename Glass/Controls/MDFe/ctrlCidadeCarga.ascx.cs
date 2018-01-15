using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlCidadeCarga : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdManifestoEletronico { get; set; }

        public List<CidadeCargaMDFe> CidadesCarga
        {
            get
            {
                var cidadesCarga = new List<CidadeCargaMDFe>();
                var cidadesCargaString = hdfCidadesCarga.Value.Split(';').ToList();

                foreach (var cidadeCarga in cidadesCargaString)
                {
                    if (cidadeCarga != "")
                        cidadesCarga.Add(new CidadeCargaMDFe
                        {
                            IdManifestoEletronico = IdManifestoEletronico,
                            IdCidade = Glass.Conversoes.StrParaInt(cidadeCarga)
                        });
                }

                return cidadesCarga;
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    var cidadesCarga = string.Empty;
                    foreach (var i in value)
                    {
                        cidadesCarga += ";" + i.IdCidade;
                    }

                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaCidadeCargaInicial('{0}', '{1}');", this.ClientID, cidadesCarga), true);

                    imgAdicionar.OnClientClick = "adicionarLinhaCidadeCarga('" + this.ClientID + "'); return false";
                    imgRemover.OnClientClick = "removerLinhaCidadeCarga('" + this.ClientID + "'); return false";
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            drpCidadeCarga.Attributes.Add("onchange", "pegarValorCidadeCarga('" + this.ClientID + "')");

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlCidadeCarga_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlCidadeCarga_script",
                    ResolveUrl("~/Scripts/MDFe/ctrlCidadeCarga.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinhaCidadeCarga('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinhaCidadeCarga('" + this.ClientID + "'); return false";
        }
    }
}