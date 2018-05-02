using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadExpressaoRentabilidade : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdExpressoesRentabilidade.Register(true, true);
            odsExpressoesRentabilidade.Register();
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var expressao = new Glass.Rentabilidade.Negocios.Entidades.ExpressaoRentabilidade();
                expressao.Nome = ((TextBox)grdExpressoesRentabilidade.FooterRow.FindControl("txtNome")).Text;
                expressao.Descricao = ((TextBox)grdExpressoesRentabilidade.FooterRow.FindControl("txtDescricao")).Text;
                expressao.Expressao = ((TextBox)grdExpressoesRentabilidade.FooterRow.FindControl("txtExpressao")).Text;
                expressao.Formatacao = ((TextBox)grdExpressoesRentabilidade.FooterRow.FindControl("txtFormatacao")).Text;
                expressao.SomaFormulaRentabilidade = ((CheckBox)grdExpressoesRentabilidade.FooterRow.FindControl("chkSomaFormulaRentabilidade")).Checked;

                if (string.IsNullOrEmpty(expressao.Nome))
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o nome da expressão.", Page);
                    return;
                }

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Rentabilidade.Negocios.IRentabilidadeFluxo>();

                var resultado = fluxo.SalvarExpressaoRentabilidade(expressao);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir a expressão de rentabilidade.", resultado);
                else
                    grdExpressoesRentabilidade.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir a expressão de rentabilidade.", ex, Page);
            }
        }
    }
}