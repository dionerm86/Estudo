using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPerdaChapaVidro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadPerdaChapaVidro));
    
            if (!IsPostBack)
            {
                hdfIdSetorCorte.Value = SetorDAO.Instance.ObtemIdsSetorCorte();
            }
        }
    
        protected void imbPesq_Click(object sender, ImageClickEventArgs e)
        {
            string etiqueta = txtEtiqueta.Text;
    
            if (string.IsNullOrEmpty(etiqueta))
            {
                tbSaida.Visible = false;
                Glass.MensagemAlerta.ShowMsg("Informe o número da etiqueta.", Page);
                return;
            }

            string[] dadosEtiqueta = etiqueta.Split('-', '.', '/');

            if (!dadosEtiqueta.Any() ||
                dadosEtiqueta[0][0].ToString().ToUpper() != "N")
            {
                tbSaida.Visible = false;
                MensagemAlerta.ShowMsg("A etiqueta informada não é de uma nota fiscal.", Page);
                return;
            }

            uint idImpressao = ProdutoImpressaoDAO.Instance.ObtemIdImpressao(etiqueta);
    
            if (idImpressao < 1)
            {
                tbSaida.Visible = false;
                Glass.MensagemAlerta.ShowMsg("A impressão desta etiqueta não foi encontrada.", Page);
                return;
            }
    
            hdfIdImpressao.Value = idImpressao.ToString();
          
            grdProduto.DataBind();
    
            if (grdProduto.Rows.Count > 0)
                tbSaida.Visible = true;
        }
    
    
        protected void grdProduto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Marcar")
                {
                    string etiqueta = e.CommandArgument.ToString().Split(';')[0];
                    uint idProd = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString().Split(';')[1]);
                    uint idProdNf = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString().Split(';')[2]);

                    var dadosRetalho = ctrlRetalhoProducao1.Dados;
                    
                    PerdaChapaVidroDAO.Instance.MarcaPerdaChapaVidroComTransacao(etiqueta, ctrlTipoPerda1.IdTipoPerda.Value, ctrlTipoPerda1.IdSubtipoPerda, txtObsPerda.Text);

                    if (dadosRetalho.Any())
                    {
                        Page.ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "Imprimir",
                            string.Format("imprimirRetalhos('{0}');", idProdNf + ";" + idProd),
                            true);
                    }
                    else
                    {
                        LimpaTela();
                    }
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao marcar perda da chapa de vidro.\n", ex, Page);
            }
    
        }
    
        #region Metodos AJAX
    
        [Ajax.AjaxMethod()]
        public void LimpaTela()
        {
            txtEtiqueta.Text = "";
            hdfIdImpressao.Value = "";
            tbSaida.Visible = false;
        }
    
        [Ajax.AjaxMethod()]
        public string ValidaEtiqueta(string etiqueta)
        {
            
            string retorno = PerdaChapaVidroDAO.Instance.ValidaEtiqueta(null, etiqueta);
    
            if (retorno != "ok")
                return "erro;" + retorno;
    
            if (PerdaChapaVidroDAO.Instance.IsPerda(etiqueta))
                return "erro;Já foi marcado perda na etiqueta informada";
    
            return "ok";
        }
    
        #endregion
    }
}
