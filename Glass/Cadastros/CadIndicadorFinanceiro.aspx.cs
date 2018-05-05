using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadIndicadorFinanceiro : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdIndicadoresFinanceiros.Register(true, true);
            odsIndicadoresFinanceiros.Register();
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var indicadorFinanceiro = new Glass.Rentabilidade.Negocios.Entidades.IndicadorFinanceiro();
                indicadorFinanceiro.Nome = ((TextBox)grdIndicadoresFinanceiros.FooterRow.FindControl("txtNome")).Text;
                indicadorFinanceiro.Descricao = ((TextBox)grdIndicadoresFinanceiros.FooterRow.FindControl("txtDescricao")).Text;
                indicadorFinanceiro.Formatacao = ((TextBox)grdIndicadoresFinanceiros.FooterRow.FindControl("txtFormatacao")).Text;

                var txtValor = ((TextBox)grdIndicadoresFinanceiros.FooterRow.FindControl("txtValor"))?.Text;
                decimal valor;

                if (decimal.TryParse(txtValor, System.Globalization.NumberStyles.Number, Glass.Globalizacao.Cultura.CulturaSistema, out valor))
                    indicadorFinanceiro.Valor = valor;


                if (string.IsNullOrEmpty(indicadorFinanceiro.Nome))
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o nome do indicador.", Page);
                    return;
                }

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Rentabilidade.Negocios.IRentabilidadeFluxo>();

                var resultado = fluxo.SalvarIndicadorFinanceiro(indicadorFinanceiro);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir o indicador financeiro.", resultado);
                else
                    grdIndicadoresFinanceiros.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir o indicador financeiro.", ex, Page);
            }
        }
    }
}