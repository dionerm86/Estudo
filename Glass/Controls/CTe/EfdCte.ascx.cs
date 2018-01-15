using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class EfdCte : CteBaseUserControl
    {
        #region Propriedade

        public WebGlass.Business.ConhecimentoTransporte.Entidade.EfdCte ObjEfdCte
        {
            get
            {
                return new WebGlass.Business.ConhecimentoTransporte.Entidade.EfdCte()
                {
                    NaturezaBcCred = Glass.Conversoes.StrParaIntNullable(selNatBcCred.Valor),
                    IndNaturezaFrete = Glass.Conversoes.StrParaIntNullable(selIndNatFrete.Valor),
                    CodCont = Glass.Conversoes.StrParaIntNullable(selCodCont.Valor),
                    CodCred = Glass.Conversoes.StrParaIntNullable(selCodCred.Valor),
                    IdContaContabil = Glass.Conversoes.StrParaUintNullable(drpContaContabil.SelectedValue)
                };
            }
            set
            {
                selNatBcCred.Valor = value.NaturezaBcCred != null ? value.NaturezaBcCred.ToString() : null;
                selNatBcCred.Descricao = value.DescrNaturezaBcCred;
    
                selIndNatFrete.Valor = value.IndNaturezaFrete != null ? value.IndNaturezaFrete.ToString() : null;
                selIndNatFrete.Descricao = value.DescrIndNaturezaFrete;
    
                selCodCont.Valor = value.CodCont != null ? value.CodCont.Value.ToString() : null;
                selCodCont.Descricao = value.DescrCodCont;
    
                selCodCred.Valor = value.CodCred != null ? value.CodCred.Value.ToString() : null;
                selCodCred.Descricao = value.DescrCodCred;
    
                drpContaContabil.SelectedValue = value.IdContaContabil != null ? value.IdContaContabil.Value.ToString() : null;
            }
        }
    
        #endregion
    
        public EfdCte()
        {
            this.AlterouTipoDocumentoCte += new EventHandler(EfdCte_AlterouTipoDocumentoCte);
        }
    
        private void EfdCte_AlterouTipoDocumentoCte(object sender, EventArgs e)
        {
            //if (TipoDocumentoCte == Cte.TipoDocumentoCteEnum.Saida)
            //{
            //    selCodCred.Validador.Enabled = false;
            //    selCodCred.Validador.Visible = false;
            //}
        }
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }// new[] { selNatBcCred.Validador, selIndNatFrete.Validador, selCodCred.Validador }; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            selNatBcCred.Validador.Enabled = false;
            selNatBcCred.Validador.Visible = false;
    
            selIndNatFrete.Validador.Enabled = false;
            selIndNatFrete.Validador.Visible = false;
    
            selCodCont.Validador.Enabled = false;
            selCodCont.Validador.Visible = false;
    
            selCodCred.Validador.Enabled = false;
            selCodCred.Validador.Visible = false;
        }
    }
}
