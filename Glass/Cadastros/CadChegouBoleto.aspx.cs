using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadChegouBoleto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    
        protected void grdConta_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Pega a data de vencimento do boleto, que pode ter sido alterado
            bool boletoChegou = ((HiddenField)((GridView)sender).SelectedRow.FindControl("hdfBoletoChegou")).Value.ToLower() == "true";
            string idContaPg = ((HiddenField)((GridView)sender).SelectedRow.FindControl("hdfIdContaPg")).Value;
            string dataVenc = null;
    
            if (!boletoChegou)
            {
                dataVenc = ((TextBox)((GridView)sender).SelectedRow.FindControl("txtDataVenc")).Text;
    
                if (!FuncoesData.ValidaData(dataVenc))
                {
                    Glass.MensagemAlerta.ShowMsg("Data inválida.", Page);
                    return;
                }
            }
    
            ContasPagarDAO.Instance.BoletoChegou(!boletoChegou, 0, Glass.Conversoes.StrParaUint(idContaPg), dataVenc);
    
            // Desmarca linha da grid como selecionada
            ((GridView)sender).SelectedIndex = -1;
    
            grdConta.DataBind();
        }
    }
}
