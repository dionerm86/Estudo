using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadEstornoCxGeral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void lnkBuscarRetirada_Click(object sender, ImageClickEventArgs e)
        {
            if (String.IsNullOrEmpty(txtCodMov.Text))
            {
                Glass.MensagemAlerta.ShowMsg("Informe o código da movimentação de retirada/crédito.", Page);
                return;
            }
    
            try
            {
                uint idCxGeral = Glass.Conversoes.StrParaUint(txtCodMov.Text);
                lblMovimentacao.Text = WebGlass.Business.CaixaGeral.Fluxo.BuscarEValidar.Instance.Buscar(idCxGeral);
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
                Glass.MensagemAlerta.ShowMsg("Informe o código da movimentação de retirada/crédito.", Page);
                return;
            }
    
            if (String.IsNullOrEmpty(txtObs.Text))
            {
                Glass.MensagemAlerta.ShowMsg("Informe o motivo do cancelamento.", Page);
                return;
            }
    
            try
            {
                uint idCxGeral = Glass.Conversoes.StrParaUint(txtCodMov.Text);
                WebGlass.Business.CaixaGeral.Fluxo.Estornar.Instance.EstornarRetirada(idCxGeral, txtObs.Text);
    
                txtObs.Text = String.Empty;
                txtCodMov.Text = String.Empty;
                lblMovimentacao.Text = String.Empty;
    
                Glass.MensagemAlerta.ShowMsg("Cancelamento efetuado com sucesso.", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao estornar movimentação.", ex, Page);
            }
        }
    }
}
