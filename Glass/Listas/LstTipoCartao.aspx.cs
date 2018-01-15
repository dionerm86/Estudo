using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstTipoCartao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (TipoCartaoCreditoDAO.Instance.GetCountReal() == 0)
                foreach (TableCell c in grdTipoCartao.Rows[0].Cells)
                    c.Text = String.Empty;
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var operadora = Conversoes.StrParaUint(((DropDownList)grdTipoCartao.FooterRow.FindControl("drpOperadora")).SelectedValue);
                var bandeira = Conversoes.StrParaUint(((DropDownList)grdTipoCartao.FooterRow.FindControl("drpBandeira")).SelectedValue);
                var tipo = ((TipoCartaoEnum)Enum.Parse(typeof(TipoCartaoEnum), ((DropDownList)grdTipoCartao.FooterRow.FindControl("drpTipo")).SelectedValue));

                TipoCartaoCreditoDAO.Instance.Insert(new TipoCartaoCredito()
                {
                    Operadora = operadora,
                    Bandeira = bandeira,
                    Tipo = tipo,
                    /* Chamado 47874. */
                    NumParc = tipo == TipoCartaoEnum.Debito ? 1 : 0
                });

                grdTipoCartao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Tipo de Cartão.", ex, Page);
            }
        }

        protected void odsTipoCartao_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
