using System;
using System.Collections.Generic;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class CadRetalhoProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void imbPesq_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void odsProdutos_Selecting(object sender, Colosoft.WebControls.VirtualObjectDataSourceSelectingEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNumeroNFE.Text))
                return;
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            List<RetalhoProducaoAuxiliar> dadosRetalho = ctrlRetalhoProducao1.Dados;
            string[] idsP = hdfIdProdNF.Value.Split(';');

            try
            {
                if (!RetalhoProducaoDAO.Instance.ValidaRetalhos(dadosRetalho, idsP[1].StrParaUint(), idsP[0].StrParaUint()))
                    throw new Exception();
            }
            catch
            {
                MensagemAlerta.ShowMsg("Erro de validação de retalhos.\r\nVerifique se a altura e largura do retalho são menores que a chapa assim como a metragem total.", this.Page);
                return;
            }


            Page.ClientScript.RegisterStartupScript(this.GetType(), "Imprimir", string.Format("imprimirRetalhos('{0}');", hdfIdProdNF.Value), true);
        }
    }
}
