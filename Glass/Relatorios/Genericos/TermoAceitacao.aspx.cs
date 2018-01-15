using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Relatorios.Genericos
{
    public partial class TermoAceitacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNumPedido.Text) || !PedidoDAO.Instance.PedidoExists(Glass.Conversoes.StrParaUint(txtNumPedido.Text)))
            {
                tbInfPedido.Visible = false;
                tbInfAdicional.Visible = false;
                lnkImprimir.Visible = false;

                if (!string.IsNullOrEmpty(txtNumPedido.Text))
                    MensagemAlerta.ShowMsg("O pedido informado não existe.", Page);

                return;
            }

            if (!string.IsNullOrEmpty(txtNumPedido.Text) && Glass.Conversoes.StrParaInt(txtNumPedido.Text) > 0)
            {
                Glass.Data.Model.Pedido ped = PedidoDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(txtNumPedido.Text));

                if (ped == null)
                    return;

                lblCli.Text = ped.NomeCliente;
                lblVend.Text = ped.NomeFunc;
                lblOrca.Text = ped.IdOrcamento.ToString();
                lblPed.Text = ped.IdPedido.ToString();

                tbInfPedido.Visible = true;
                tbInfAdicional.Visible = true;
                lnkImprimir.Visible = true;
            }
            else
            {
                tbInfPedido.Visible = false;
                tbInfAdicional.Visible = false;
                lnkImprimir.Visible = false;
            }
        }
    }
}
