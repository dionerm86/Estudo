using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetInfComplProdNf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ProdutosNf prodNf = ProdutosNfDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idProdNf"]));
                lblProdNf.Text = prodNf.DescrProduto;
                txtInfo.Text = prodNf.InfAdic;
            }
        }
    
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (txtInfo.Text.Length > 500)
            {
                Glass.MensagemAlerta.ShowMsg("Este campo deve ter no máximo 500 caracteres. Quantidade de caracteres digitados: " + txtInfo.Text.Length, Page);
                return;
            }
    
            ProdutosNfDAO.Instance.SalvaInfAdic(Glass.Conversoes.StrParaUint(Request["idProdNf"]), txtInfo.Text);
    
            ClientScript.RegisterClientScriptBlock(typeof(string), "ok", "alert('Informação adicional salva com sucesso.');closeWindow();", true);
        }
    }
}
