using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlDadosCnab : System.Web.UI.UserControl
    {
        #region Variaveis Locais

        private bool _ctrlCarregado = false;

        #endregion

        #region Propiedades

        public Data.Model.DadosCnab DadosCnab
        {
            get
            {
                var dados = new Data.Model.DadosCnab()
                {
                    CodBanco = hdfCodBanco.Value.StrParaInt(),
                    TipoCnab = ddlTipoCnab.SelectedValue.StrParaInt(),
                    CodOcorrencia = ddlCodigoOcorrencia.SelectedValue.StrParaInt(),
                    CodMovRemessa = ddlCodigoMovimento.SelectedValue.StrParaInt(),
                    CodCarteira = ddlCarteira.SelectedValue.StrParaInt(),
                    CodCadastramento = ddlCadastramento.SelectedValue.StrParaInt(),
                    CodEspecieDocumento = ddlEspecie.SelectedValue.StrParaInt(),
                    Aceite = ddlAceite.SelectedValue.StrParaInt(),
                    JurosMoraCod = ddlCodigoJurosMora.SelectedValue.StrParaInt(),
                    JurosMoraDias = txtDataJuroMora.Text.StrParaInt(),
                    JurosMoraValor = txtValorJuroMora.Text.StrParaDecimal(),
                    DescontoCod = ddlCodigoDesconto.SelectedValue.StrParaInt(),
                    DescontoDias = txtDataDesconto.Text.StrParaInt(),
                    DescontoValor = txtValorDesconto.Text.StrParaDecimal(),
                    ProtestoCod = ddlCodigoProtesto.SelectedValue.StrParaInt(),
                    ProtestoDias = txtDiasProtesto.Text.StrParaInt(),
                    MultaCod = ddlCodigoMulta.SelectedValue.StrParaInt(),
                    MultaValor = txtMultaValor.Text.StrParaDecimal(),
                    BaixaDevolucaoCod = ddlCodigoBaixaDevolucao.SelectedValue.StrParaInt(),
                    BaixaDevolucaoDias = txtDiasBaixaDevolucao.Text.StrParaInt(),
                    Instrucao1 = ddlInstrucao.SelectedValue.StrParaInt(),
                    Instrucao2 = ddlInstrucao2.SelectedValue.StrParaInt(),
                    Mensagem = txtMensagem.Text,
                    EmissaoBloqueto = ddlEmissaoBloqueto.SelectedValue.StrParaInt(),
                    DistribuicaoBloqueto = ddlDistribuicaoBloqueto.SelectedValue.StrParaInt(),
                    TipoDocumento = ddlTipoDocumento.SelectedValue.StrParaInt(),
                    CodMoeda = ddlCodigoMoeda.SelectedValue.StrParaInt()
                };

                return dados;
            }

            set
            {
                var dados = value;

                if (dados == null)
                {
                    foreach (var item in Controls)
                        if (item is TextBox)
                            ((TextBox)item).Text = "";

                    return;
                }

                hdfCodBanco.Value = dados.CodBanco.ToString();
                ddlTipoCnab.SelectedValue = dados.TipoCnab.ToString();
                ddlCodigoOcorrencia.SelectedValue = dados.CodOcorrencia.ToString();
                ddlCodigoMovimento.SelectedValue = dados.CodMovRemessa.ToString();
                ddlCarteira.SelectedValue = dados.CodCarteira.ToString();
                ddlCadastramento.SelectedValue = dados.CodCadastramento.ToString();
                ddlEspecie.SelectedValue = dados.CodEspecieDocumento.ToString();
                ddlAceite.SelectedValue = dados.Aceite.ToString();
                ddlCodigoJurosMora.SelectedValue = dados.JurosMoraCod.ToString();
                txtDataJuroMora.Text = dados.JurosMoraDias.ToString();
                txtValorJuroMora.Text = dados.JurosMoraValor.ToString();
                ddlCodigoDesconto.SelectedValue = dados.DescontoCod.ToString();
                txtDataDesconto.Text = dados.DescontoDias.ToString();
                txtValorDesconto.Text = dados.DescontoValor.ToString();
                ddlCodigoProtesto.SelectedValue = dados.ProtestoCod.ToString();
                txtDiasProtesto.Text = dados.ProtestoDias.ToString();
                ddlCodigoMulta.SelectedValue = dados.MultaCod.ToString();
                txtMultaValor.Text = dados.MultaValor.ToString();
                ddlCodigoBaixaDevolucao.SelectedValue = dados.BaixaDevolucaoCod.ToString();
                txtDiasBaixaDevolucao.Text = dados.BaixaDevolucaoDias.ToString();
                txtMensagem.Text = dados.Mensagem;
                ddlEmissaoBloqueto.SelectedValue = dados.EmissaoBloqueto.ToString();
                ddlDistribuicaoBloqueto.SelectedValue = dados.DistribuicaoBloqueto.ToString();
                ddlTipoDocumento.SelectedValue = dados.TipoDocumento.ToString();
                ddlCodigoMoeda.SelectedValue = dados.CodMoeda.ToString();

                // Verifica se a instrução padrão existe na lista, se não existir, exibe um alertar para o usuário
                var instrucoes = (List<KeyValuePair<int, string>>)odsInstrucao.Select();
                if (instrucoes.Any(f => f.Key == dados.Instrucao1))
                {
                    ddlInstrucao.SelectedValue = dados.Instrucao1.ToString();
                    imgAlertaInstrucao1.Visible = false;
                }
                else
                    imgAlertaInstrucao1.Visible = true;

                if (instrucoes.Any(f => f.Key == dados.Instrucao2))
                {
                    ddlInstrucao2.SelectedValue = dados.Instrucao2.ToString();
                    imgAlertaInstrucao2.Visible = false;
                }
                else
                    imgAlertaInstrucao2.Visible = true;
            }
        }

        public bool CtrlCarregado
        {
            get { return _ctrlCarregado; }

            set { _ctrlCarregado = value; }
        }

        public event EventHandler CtrlDadosCnabChange;

        #endregion

        #region Métodos Protegidos

        protected void Page_Load(object sender, EventArgs e)
        {
            ddlTipoCnab.SelectedIndexChanged += new EventHandler(ddlTipoCnab_SelectedIndexChanged);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (var item in Controls)
            {
                if (item is Colosoft.WebControls.VirtualObjectDataSource)
                    ((Colosoft.WebControls.VirtualObjectDataSource)item).Selecting += CtrlDadosCnab_Selecting;
            }
        }

        protected void ddlTipoCnab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CtrlDadosCnabChange != null)
                CtrlDadosCnabChange(sender, e);
        }

        public void AlterarTipoCnab(int tipoCnab)
        {
            ddlTipoCnab.SelectedValue = tipoCnab.ToString();
        }

        #endregion

        #region Métodos Privados

        private void CtrlDadosCnab_Selecting(object sender, Colosoft.WebControls.VirtualObjectDataSourceSelectingEventArgs e)
        {
            if (!_ctrlCarregado)
                e.Cancel = true;
        }

        #endregion

        public void LimpaControles()
        {
            foreach (var item in Controls)
            {
                if (item is DropDownList)
                {
                    if (((DropDownList)item).ID == "ddlTipoCnab")
                        continue;

                    ((DropDownList)item).Items.Clear();
                }
            }
        }
    }
}