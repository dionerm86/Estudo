using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlUFPercursoMDFe : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdManifestoEletronico { get; set; }
        public string ValorPadrao { get; set; }

        public List<UFPercursoMDFe> UFsPercurso
        {
            get
            {
                var uFsPercurso = new List<UFPercursoMDFe>();
                var uFsPercursoString = hdfUFsPercurso.Value.Split(';').ToList();

                foreach (var uFPercurso in uFsPercursoString)
                {
                    if (uFPercurso != "")
                        uFsPercurso.Add(new UFPercursoMDFe
                        {
                            IdManifestoEletronico = IdManifestoEletronico,
                            UFPercurso = uFPercurso
                        });
                }

                return uFsPercurso;
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    var uFsPercurso = string.Empty;
                    foreach (var i in value)
                    {
                        uFsPercurso += ";" + i.UFPercurso;
                    }

                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaUFPercursoInicial('{0}', '{1}');", this.ClientID, uFsPercurso), true);

                    imgAdicionar.OnClientClick = "adicionarLinhaUFPercurso('" + this.ClientID + "'); return false";
                    imgRemover.OnClientClick = "removerLinhaUFPercurso('" + this.ClientID + "'); return false";
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            drpUFPercurso.Attributes.Add("onchange", "pegarValorUFPercurso('" + this.ClientID + "')");

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlUFPercursoMDFe_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlUFPercursoMDFe_script",
                    ResolveUrl("~/Scripts/MDFe/ctrlUFPercursoMDFe.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinhaUFPercurso('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinhaUFPercurso('" + this.ClientID + "'); return false";
        }

        protected void drpUFPercurso_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
                drpUFPercurso.SelectedValue = Glass.Data.Helper.UserInfo.GetUserInfo.UfLoja;
        }
    }
}