using System;
using System.Web.UI.WebControls;
using Glass.Data.Model.Cte;

namespace Glass.UI.Web.Controls.CTe
{
    public partial class Proprietario : CteBaseUserControl
    {
        #region Propriedades
    
        public ProprietarioVeiculo ProprietarioVeiculo
        {
            get
            {
                return new ProprietarioVeiculo
                {
                    Cpf = !String.IsNullOrEmpty(txtCpf.Text) ? txtCpf.Text.Replace(".", "").Replace("-", "") : "",
                    Cnpj = !String.IsNullOrEmpty(txtCnpj.Text) ? txtCnpj.Text.Replace(".", "").Replace("/", "") : "",
                    RNTRC = txtRntrc.Text,
                    Nome = txtNomeProprietario.Text,
                    IE = txtInscricaoEstadual.Text,
                    UF = drpUf.SelectedValue,
                    TipoProp = Glass.Conversoes.StrParaInt(drpTipoProp.SelectedValue)
                };
            }
            set
            {
                txtCpf.Text = value.Cpf;
                txtCnpj.Text = value.Cnpj;
                txtRntrc.Text = value.RNTRC;
                txtNomeProprietario.Text = value.Nome;
                txtInscricaoEstadual.Text = value.IE;
                drpUf.SelectedValue = value.UF;
                drpTipoProp.SelectedValue = value.TipoProp.ToString();
            }
        }
    
        #endregion
    
        public override System.Collections.Generic.IEnumerable<BaseValidator> ValidadoresObrigatoriosEntrada
        {
            get { return null; }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    }
}
