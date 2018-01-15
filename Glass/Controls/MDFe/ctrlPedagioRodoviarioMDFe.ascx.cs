using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.MDFe
{
    public partial class ctrlPedagioRodoviarioMDFe : System.Web.UI.UserControl
    {
        #region Propriedades

        public int IdRodoviario { get; set; }

        public List<PedagioRodoviarioMDFe> PedagioRodoviario
        {
            get
            {
                var pedagiosRodoviarios = new List<PedagioRodoviarioMDFe>();
                var fornecedores = hdfFornecedor.Value.Split(';').ToList();
                var responsaveis = hdfResponsavel.Value.Split(';').ToList();
                var numerosCompra = hdfNumeroCompra.Value.Split(';').ToList();
                var valoresPedagio = hdfValorPedagio.Value.Split(';').ToList();

                for (var i = 0; i < fornecedores.Count; i++)
                {
                    if (fornecedores[i] != "" && responsaveis[i] != "" && numerosCompra[i] != "" && valoresPedagio[i] != "")
                        pedagiosRodoviarios.Add(new PedagioRodoviarioMDFe
                        {
                            IdRodoviario = IdRodoviario,
                            IdFornecedor = Glass.Conversoes.StrParaInt(fornecedores[i]),
                            ResponsavelPedagio = Glass.Conversoes.StrParaEnum<ResponsavelEnum>(responsaveis[i]),
                            NumeroCompra = numerosCompra[i],
                            ValorValePedagio = Glass.Conversoes.StrParaDecimal(valoresPedagio[i])
                        });
                }

                return pedagiosRodoviarios;
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    var fornecedores = string.Empty;
                    var responsaveis = string.Empty;
                    var numerosCompra = string.Empty;
                    var valoresPedagio = string.Empty;
                    foreach (var i in value)
                    {
                        fornecedores += ";" + i.IdFornecedor;
                        responsaveis += ";" + i.ResponsavelPedagio;
                        numerosCompra += ";" + i.NumeroCompra;
                        valoresPedagio += ";" + i.ValorValePedagio;
                    }

                    Page.ClientScript.RegisterStartupScript(GetType(), "iniciar",
                        string.Format("carregaPedagioInicial('{0}', '{1}', '{2}', '{3}', '{4}');", this.ClientID, fornecedores, responsaveis, numerosCompra, valoresPedagio), true);

                    imgAdicionar.OnClientClick = "adicionarLinhaPedagio('" + this.ClientID + "'); return false";
                    imgRemover.OnClientClick = "removerLinhaPedagio('" + this.ClientID + "'); return false";
                }
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            drpFornecedor.Attributes.Add("onchange", "pegarValorPedagio('" + this.ClientID + "')");
            drpResponsavelPedagio.Attributes.Add("onchange", "pegarValorPedagio('" + this.ClientID + "')");
            txtNumeroCompra.Attributes.Add("onblur", "pegarValorPedagio('" + this.ClientID + "')");
            txtValorPedagio.Attributes.Add("onblur", "pegarValorPedagio('" + this.ClientID + "')");

            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlPedagioRodoviarioMDFe_script"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlPedagioRodoviarioMDFe_script",
                    ResolveUrl("~/Scripts/MDFe/ctrlPedagioRodoviarioMDFe.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()));

            imgAdicionar.OnClientClick = "adicionarLinhaPedagio('" + this.ClientID + "'); return false";
            imgRemover.OnClientClick = "removerLinhaPedagio('" + this.ClientID + "'); return false";
        }
    }
}