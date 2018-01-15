using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstCfop : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdCfop.Register(true, true);
            odsCfop.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grdCfop.Columns[6].Visible = EstoqueConfig.ControlarEstoqueVidrosClientes;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var cfop = new Glass.Fiscal.Negocios.Entidades.Cfop();
                cfop.Descricao = ((TextBox)grdCfop.FooterRow.FindControl("txtDescricaoIns")).Text;
                cfop.CodInterno = ((TextBox)grdCfop.FooterRow.FindControl("txtCodInternoIns")).Text;
                cfop.IdTipoCfop = (((DropDownList)grdCfop.FooterRow.FindControl("drpTipoCfop")).SelectedValue).StrParaIntNullable();

                var tipoMercadoria = ((DropDownList)grdCfop.FooterRow.FindControl("drpTipoMercadoria")).SelectedValue;

                if (!string.IsNullOrEmpty(tipoMercadoria))
                    cfop.TipoMercadoria = (Data.Model.TipoMercadoria)Enum.Parse(typeof(Data.Model.TipoMercadoria), tipoMercadoria);

                cfop.AlterarEstoqueTerceiros = ((CheckBox)grdCfop.FooterRow.FindControl("chkAlterarEstoqueTerceiros")).Checked;
                cfop.AlterarEstoqueCliente = ((CheckBox)grdCfop.FooterRow.FindControl("chkAlterarEstoqueCliente")).Checked;
                cfop.Obs = ((TextBox)grdCfop.FooterRow.FindControl("txtObs")).Text;

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Fiscal.Negocios.ICfopFluxo>();

                var resultado = fluxo.SalvarCfop(cfop);
                if (resultado)
                    grdCfop.DataBind();
                else
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir CFOP.", resultado);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir CFOP.", ex, Page);
            }
        }
        
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCfop.PageIndex = 0;
        }
    }
}
