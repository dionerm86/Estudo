using Glass.Data.DAL;
using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDadosPadraoCnab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void ddlBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            CtrlChanged(0);
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                var dados = ctrlDadosCnab.DadosCnab;

                DadosCnabDAO.Instance.SalvarValorPadrao(dados);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao salvar valor padrão.", ex, Page);
            }
        }

        protected void ctrlDadosCnab_CtrlDadosCnabChange(object sender, EventArgs e)
        {
            CtrlChanged(((DropDownList)sender).SelectedValue.StrParaInt());
        }

        private void CtrlChanged(int tipoCnab)
        {
            var codBanco = ddlBanco.SelectedValue.StrParaInt();

            ((HiddenField)ctrlDadosCnab.FindControl("hdfCodBanco")).Value = codBanco.ToString();

            ctrlDadosCnab.CtrlCarregado = true;
            ctrlDadosCnab.LimpaControles();

            if (tipoCnab > 0)
            {
                ctrlDadosCnab.AlterarTipoCnab(tipoCnab);

                if (DadosCnabDAO.Instance.ObterTipoCnabPadrao(null, codBanco) == tipoCnab)
                    ctrlDadosCnab.DadosCnab = DadosCnabDAO.Instance.ObtemValorPadrao(null, codBanco, tipoCnab);
            }
            else
            {
                var tipoCnabPadrao = DadosCnabDAO.Instance.ObterTipoCnabPadrao(null, codBanco);

                if (tipoCnabPadrao > 0)
                    ctrlDadosCnab.DadosCnab = DadosCnabDAO.Instance.ObtemValorPadrao(null, codBanco, tipoCnabPadrao);
            }

            ctrlDadosCnab.DataBind();

            ctrlDadosCnab.Visible = true;
            btnSalvar.Visible = true;
        }
    }
}