using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlConfigFaixasRentabilidadeComissao : System.Web.UI.UserControl
    {
        #region Propriedades

        /// <summary>
        /// Obtém ou define o identificador do funcionário
        /// para o qual serão filtradas as faixas.
        /// </summary>
        public int? IdFunc
        {
            get
            {
                int value;

                if (int.TryParse(hdfIdFuncFaixaRentabilidadeComissao.Value, out value))
                    return value;

                return null;
            }
            set
            {
                hdfIdFuncFaixaRentabilidadeComissao.Value = value?.ToString();
            }
        }

        /// <summary>
        /// Obtém ou define o identificador da loja
        /// para o qual serão filtradas as faixas.
        /// </summary>
        public int IdLoja
        {
            get
            {
                int value;
                if (int.TryParse(hdfIdLojaFaixaRentabilidadeComissao.Value, out value))
                    return value;

                return 0;
            }
            set
            {
                hdfIdLojaFaixaRentabilidadeComissao.Value = value.ToString();
            }
        }

        #endregion

        #region Métodos Protegidos

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFaixasRentabilidadeComissao.Register(true, true);
            odsFaixaRentabilidadeComissao.Register();
        }

        protected void odsFaixaRentabilidadeComissao_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao configurar a faixa de rentabilidade para a comissão.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Glass.MensagemAlerta.ShowMsg("Faixa de rentabilidade para a comissão configurada!", Page);
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var faixaRentabilidadeComissao = new Glass.Rentabilidade.Negocios.Entidades.FaixaRentabilidadeComissao();

                decimal percentualRentabilidade;
                decimal percentualComissao;

                if (!decimal.TryParse(((TextBox)grdFaixasRentabilidadeComissao.FooterRow.FindControl("txtPercentualRentabilidade")).Text,
                        System.Globalization.NumberStyles.Number, Glass.Globalizacao.Cultura.CulturaSistema, out percentualRentabilidade) ||
                    !decimal.TryParse(((TextBox)grdFaixasRentabilidadeComissao.FooterRow.FindControl("txtPercentualComissao")).Text,
                        System.Globalization.NumberStyles.Number, Glass.Globalizacao.Cultura.CulturaSistema, out percentualComissao))
                {
                    Glass.MensagemAlerta.ShowMsg("Os valores informados não são numéricos válidos", Page);
                    return;
                }

                faixaRentabilidadeComissao.IdLoja = IdLoja;
                faixaRentabilidadeComissao.IdFunc = IdFunc;
                faixaRentabilidadeComissao.PercentualRentabilidade = percentualRentabilidade;
                faixaRentabilidadeComissao.PercentualComissao = percentualComissao;

                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Glass.Rentabilidade.Negocios.IRentabilidadeFluxo>();

                var resultado = fluxo.SalvarFaixaRentabilidadeComissao(faixaRentabilidadeComissao);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir a faixa de rentabilidade em relação ao comissão.", resultado);
                else
                    grdFaixasRentabilidadeComissao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir a faixa de rentabilidade em relação ao comissão.", ex, Page);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion
    }
}