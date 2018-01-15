using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlCIOTRodoviarioMDFe : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdRodoviario { get; set; }

        public List<CiotRodoviarioMDFe> CiotRodoviario
        {
            get
            {
                var CiotsRodoviarios = new List<CiotRodoviarioMDFe>();
                var CIOTsString = hdfCIOTs.Value.Split(';').ToList();
                var CPFCNPJsString = hdfCPFCNPJs.Value.Split(';').ToList();

                for (var i = 0; i < CIOTsString.Count; i++)
                {
                    if(CIOTsString[i] != "" && CPFCNPJsString[i] != "")
                        CiotsRodoviarios.Add(new CiotRodoviarioMDFe
                        {
                            IdRodoviario = IdRodoviario,
                            CIOT = CIOTsString[i],
                            CPFCNPJCIOT = CPFCNPJsString[i]
                        });
                }

                return CiotsRodoviarios;
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    var CIOTs = string.Empty;
                    var CPFCNPJs = string.Empty;
                    foreach (var i in value)
                    {
                        CIOTs += ";" + i.CIOT;
                        CPFCNPJs += ";" + i.CPFCNPJCIOT;
                    }

                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaCIOTInicial('{0}', '{1}', '{2}');", this.ClientID, CIOTs, CPFCNPJs), true);

                    imgAdicionar.OnClientClick = "adicionarLinhaCIOT('" + this.ClientID + "'); return false";
                    imgRemover.OnClientClick = "removerLinhaCIOT('" + this.ClientID + "'); return false";
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            txtCIOT.Attributes.Add("onblur", "pegarValorCIOT('" + this.ClientID + "')");
            txtCPFCNPJ.Attributes.Add("onblur", "pegarValorCIOT('" + this.ClientID + "')");

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlCIOTRodoviarioMDFe_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlCIOTRodoviarioMDFe_script", ResolveUrl("~/Scripts/MDFe/ctrlCIOTRodoviarioMDFe.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinhaCIOT('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinhaCIOT('" + this.ClientID + "'); return false";
        }
    }
}