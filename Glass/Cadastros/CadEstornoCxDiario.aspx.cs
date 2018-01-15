using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadEstornoCxDiario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkBuscarCreditoRetirada_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(txtCodMov.Text))
            {
                Glass.MensagemAlerta.ShowMsg("Informe o código da movimentação de crédito/retirada.", Page);
                return;
            }
    
            try
            {
                var idCaixaDiario = txtCodMov.Text.StrParaInt();
                lblMovimentacao.Text = WebGlass.Business.CaixaDiario.Fluxo.BuscarEValidar.Instance.BuscarCreditoRetirada(idCaixaDiario);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao buscar movimentação.", ex, Page);
            }
        }
    
        protected void btnEstornar_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtCodMov.Text))
            {
                Glass.MensagemAlerta.ShowMsg("Informe o código da movimentação de crédito/retirada.", Page);
                return;
            }
    
            if (String.IsNullOrEmpty(txtObs.Text))
            {
                Glass.MensagemAlerta.ShowMsg("Informe o motivo do estorno.", Page);
                return;
            }
    
            try
            {
                var idCaixaDiario = txtCodMov.Text.StrParaInt();
                WebGlass.Business.CaixaDiario.Fluxo.Estornar.Instance.EstornarCreditoRetirada(idCaixaDiario, txtObs.Text);
    
                txtObs.Text = String.Empty;
                txtCodMov.Text = String.Empty;
                lblMovimentacao.Text = String.Empty;
    
                Glass.MensagemAlerta.ShowMsg("Estorno efetuado com sucesso.", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao estornar movimentação.", ex, Page);
            }
        }
    }
}
