using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlCondutorVeiculoMDFe : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdRodoviario { get; set; }

        public List<CondutorVeiculoMDFe> CondutorVeiculo
        {
            get
            {
                var condutoresVeiculo = new List<CondutorVeiculoMDFe>();
                var condutoresVeiculoString = hdfCondutores.Value.Split(';').ToList();

                foreach (var condutorVeiculo in condutoresVeiculoString)
                {
                    if (condutorVeiculo != "")
                        condutoresVeiculo.Add(new CondutorVeiculoMDFe
                        {
                            IdRodoviario = IdRodoviario,
                            IdCondutor = Glass.Conversoes.StrParaInt(condutorVeiculo)
                        });
                }

                return condutoresVeiculo;
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    var condutoresVeiculo = string.Empty;
                    foreach (var i in value)
                    {
                        condutoresVeiculo += ";" + i.IdCondutor;
                    }

                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaCondutorInicial('{0}', '{1}');", this.ClientID, condutoresVeiculo), true);

                    imgAdicionar.OnClientClick = "adicionarLinhaCondutor('" + this.ClientID + "'); return false";
                    imgRemover.OnClientClick = "removerLinhaCondutor('" + this.ClientID + "'); return false";
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            drpCondutor.Attributes.Add("onchange", "pegarValorCondutor('" + this.ClientID + "')");

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlCondutorVeiculoMDFe_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlCondutorVeiculoMDFe_script", ResolveUrl("~/Scripts/MDFe/ctrlCondutorVeiculoMDFe.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinhaCondutor('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinhaCondutor('" + this.ClientID + "'); return false";
        }
    }
}