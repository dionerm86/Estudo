using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlLacreRodoviarioMDFe : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdRodoviario { get; set; }

        public List<LacreRodoviarioMDFe> LacreRodoviario
        {
            get
            {
                var lacres = new List<LacreRodoviarioMDFe>();
                var lacresString = hdfLacres.Value.Split(';').ToList();

                foreach(var lacre in lacresString)
                {
                    if (lacre != "")
                        lacres.Add(new LacreRodoviarioMDFe
                        {
                            IdRodoviario = IdRodoviario,
                            Lacre = lacre
                        });
                }

                return lacres;
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    var lacres = string.Empty;
                    foreach (var l in value)
                    {
                        lacres += ";" + l.Lacre;
                    }

                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaLacreInicial('{0}', '{1}');", this.ClientID, lacres), true);

                    imgAdicionar.OnClientClick = "adicionarLinhaLacre('" + this.ClientID + "'); return false";
                    imgRemover.OnClientClick = "removerLinhaLacre('" + this.ClientID + "'); return false";
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            txtLacre.Attributes.Add("onblur", "pegarValorLacre('" + this.ClientID + "')");

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlLacreRodoviarioMDFe_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlLacreRodoviarioMDFe_script", ResolveUrl("~/Scripts/MDFe/ctrlLacreRodoviarioMDFe.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinhaLacre('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinhaLacre('" + this.ClientID + "'); return false";
        }
    }
}