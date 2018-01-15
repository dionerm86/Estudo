using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class CobrancaCte : CteBaseUserControl
    {
        #region Propriedade

        public WebGlass.Business.ConhecimentoTransporte.Entidade.CobrancaCte ObjCobrancaCte
        {
            get
            {
                var obj = new WebGlass.Business.ConhecimentoTransporte.Entidade.CobrancaCte()
                {
                    DescontoFatura = Glass.Conversoes.StrParaDecimal(txtDescontoFatura.Text),
                    NumeroFatura = txtNumFatura.Text,
                    ValorLiquidoFatura = Glass.Conversoes.StrParaDecimal(txtValorLiquidoFatura.Text),
                    ValorOrigFatura = Glass.Conversoes.StrParaDecimal(txtValorOrigFatura.Text),
                    GerarContasPagar = chkGerarContasPagar.Checked,
                    IdConta = Glass.Conversoes.StrParaUintNullable(drpPlanoContas.SelectedValue)
                };
    
                obj.ObjCobrancaDuplCte = ctrlCobrancaDuplCte.ObjCobrancaDuplCte;
                return obj;
            }
            set
            {
                txtDescontoFatura.Text = value.DescontoFatura == 0 ? "" : Glass.Conversoes.StrParaFloat(value.DescontoFatura.ToString()).ToString();
                txtNumFatura.Text = value.NumeroFatura;
                txtValorLiquidoFatura.Text = value.ValorLiquidoFatura == 0 ? "" : Glass.Conversoes.StrParaFloat(value.ValorLiquidoFatura.ToString()).ToString();
                txtValorOrigFatura.Text = value.ValorOrigFatura == 0 ? "" : Glass.Conversoes.StrParaFloat(value.ValorOrigFatura.ToString()).ToString();
                ctrlCobrancaDuplCte.ObjCobrancaDuplCte = value.ObjCobrancaDuplCte;
                chkGerarContasPagar.Checked = value.GerarContasPagar;
                drpPlanoContas.SelectedValue = value.IdConta != null ? value.IdConta.Value.ToString() : "Selecione";
    
                odsPlanoContas.SelectParameters["idCte"].DefaultValue = value.IdCte.ToString();
            }
        }

        public override WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum TipoDocumentoCte
        {
            get { return base.TipoDocumentoCte; }
            set
            {
                base.TipoDocumentoCte = value;
                ctrlCobrancaDuplCte.TipoDocumentoCte = value;
            }
        }
    
        #endregion
    
        public CobrancaCte()
        {
            this.AlterouTipoDocumentoCte += new EventHandler(CobrancaCte_AlterouTipoDocumentoCte);
        }
    
        private void CobrancaCte_AlterouTipoDocumentoCte(object sender, EventArgs e)
        {
            contasPagar.Visible = TipoDocumentoCte == 
                WebGlass.Business.ConhecimentoTransporte.Entidade.Cte.TipoDocumentoCteEnum.EntradaTerceiros;
        }
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return new[] { cvdrpPlanoContas }; }
        }
    }
}
