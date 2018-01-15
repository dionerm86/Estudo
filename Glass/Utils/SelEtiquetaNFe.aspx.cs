using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SelEtiquetaNFe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            uint idNf = !String.IsNullOrEmpty(txtNumeroNFe.Text) ? Glass.Conversoes.StrParaUint(txtNumeroNFe.Text) : 0;
            var lstProd = ProdutosNfDAO.Instance.GetForImpressaoEtiquetaOrdered(idNf, 
                ctrlSelFornecedor.IdFornec.GetValueOrDefault(), txtDescrProduto.Text,
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text, ((TextBox)ctrlDataFim.FindControl("txtData")).Text);
    
            string script = String.Empty;
    
            // Chama a função de buscar etiquetas desta página (popup)
            foreach (ProdutosNf pnf in lstProd)
            {
                float totM2 = pnf.TotM / pnf.Qtde * (pnf.Qtde - pnf.QtdImpresso);

                script += "setProdEtiqueta(" + pnf.IdProdNf + "," + pnf.NumeroNfe + ",'" + pnf.DescrProduto + "'," +
                    pnf.Qtde + "," + pnf.QtdImpresso + "," + pnf.Altura + "," + pnf.Largura + ",'" + totM2 + "','" + pnf.Lote + "');";
            }
    
            script += "closeWindow();";
    
            ClientScript.RegisterStartupScript(typeof(string), "addAll", script, true);
        }
    }
}
