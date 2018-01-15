using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlVeiculoRodoviarioMDFe : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdRodoviario { get; set; }

        public List<VeiculoRodoviarioMDFe> VeiculoRodoviario
        {
            get
            {
                var veiculosRodoviarios = new List<VeiculoRodoviarioMDFe>();
                var veiculosRodoviariosString = hdfVeiculosReboque.Value.Split(';').ToList();

                foreach (var veiculoRodoviario in veiculosRodoviariosString)
                {
                    if (veiculoRodoviario != "")
                        veiculosRodoviarios.Add(new VeiculoRodoviarioMDFe
                        {
                            IdRodoviario = IdRodoviario,
                            Placa = veiculoRodoviario
                        });
                }

                return veiculosRodoviarios;
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    var veiculosRodoviarios = string.Empty;
                    foreach (var i in value)
                    {
                        veiculosRodoviarios += ";" + i.Placa;
                    }

                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaVeiculoInicial('{0}', '{1}');", this.ClientID, veiculosRodoviarios), true);

                    imgAdicionar.OnClientClick = "adicionarLinhaVeiculo('" + this.ClientID + "'); return false";
                    imgRemover.OnClientClick = "removerLinhaVeiculo('" + this.ClientID + "'); return false";
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            drpVeiculoReboque.Attributes.Add("onchange", "pegarValorVeiculo('" + this.ClientID + "')");

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlVeiculoRodoviarioMDFe_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlVeiculoRodoviarioMDFe_script", ResolveUrl("~/Scripts/MDFe/ctrlVeiculoRodoviarioMDFe.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinhaVeiculo('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinhaVeiculo('" + this.ClientID + "'); return false";
        }
    }
}