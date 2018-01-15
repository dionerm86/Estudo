using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstDebitosPisCofins : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            odsDebitosPisCofins.Register(grdDebitosPisCofins);
            odsTipoImposto.Register();

            grdDebitosPisCofins.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void drpTipoImposto_DataBound(object sender, EventArgs e)
        {
            var pisCofins = new[] { 
                Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.Pis.ToString(), 
                Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.Cofins.ToString()
            };

            var itens = ((DropDownList)sender).Items;

            for (int i = itens.Count - 1; i >= 0; i--) {
                if (itens[i].Value != "" && !pisCofins.Contains(itens[i].Value))
                    itens.RemoveAt(i);
            }
        }

        protected void imgInserir_Click(object sender, ImageClickEventArgs e)
        {
            var data = (grdDebitosPisCofins.FooterRow.FindControl("ctrlData") as Glass.UI.Web.Controls.ctrlData).Data;
            var imposto = (grdDebitosPisCofins.FooterRow.FindControl("drpTipoImposto") as DropDownList).SelectedValue;
            var codReceita = (grdDebitosPisCofins.FooterRow.FindControl("txtCodReceita") as TextBox).Text;
            var cumulativo = (grdDebitosPisCofins.FooterRow.FindControl("chkCumulativo") as CheckBox).Checked;
            var valorPagto = (grdDebitosPisCofins.FooterRow.FindControl("txtValorPagto") as TextBox).Text;

            var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Fiscal.Negocios.IDetalhamentoDebitosPisCofinsFluxo>(); 

            var novo = fluxo.CriarDebitoPisCofins();

            novo.DataPagamento = data;
            novo.TipoImposto = (Sync.Fiscal.Enumeracao.TipoImposto)Enum.Parse(typeof(Sync.Fiscal.Enumeracao.TipoImposto), imposto);
            novo.CodigoReceita = codReceita;
            novo.Cumulativo = cumulativo;
            novo.ValorPagamento = Glass.Conversoes.ConverteValor<decimal>(valorPagto);

            var result = fluxo.SalvarDebitoPisCofins(novo);

            if (result)
                grdDebitosPisCofins.DataBind();
            else
                MensagemAlerta.ErrorMsg("Falha ao inserir débito de PIS/Cofins.", result);
        }

        protected void grdDebitosPisCofins_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdDebitosPisCofins.ShowFooter = e.CommandName != "Edit";
        }
    }
}