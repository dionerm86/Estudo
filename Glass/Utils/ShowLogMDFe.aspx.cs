using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class ShowLogMDFe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var idManifestoEletronico = Glass.Conversoes.StrParaInt(Request["IdManifestoEletronico"]);
            int situacao = Glass.Conversoes.StrParaInt(Request["Situacao"]);

            if (!IsPostBack)
                Page.Title += Request["Numero"].ToString();

            if (situacao == (int)SituacaoEnum.Cancelado)
            {
                lblTituloMotivo.Text = "Motivo do Cancelamento:";
                lblTextoMotivo.Text = ManifestoEletronicoDAO.Instance.ObterMotivoCancelamento(null, idManifestoEletronico);
            }
        }
    }
}