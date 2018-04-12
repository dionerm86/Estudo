using System;
using System.Linq;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Relatorios.Boleto
{
    public partial class Imprimir : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    
        protected void drpContaBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(drpContaBanco.SelectedValue))
            {
                drpCarteira.SelectedIndex = 0;
                drpEspecieDocumento.SelectedIndex = 0;
                txtInstrucoes.Text = "";
                return;
            }

            var idContaBanco = drpContaBanco.SelectedValue.StrParaUint();

            if (idContaBanco == 0)
            {
                var codigoBancos = string.Join(", ", Array.ConvertAll(Enum.GetValues(typeof(Sync.Utils.CodigoBanco))
                    .Cast<int>().ToList().ToArray(), x => x.ToString()));

                throw new Exception(string.Format("Não há conta bancária válida para a emissão do boleto. " +
                    "A conta bancária utilizada na geração do boleto deve possuir código do convênio e " +
                    "estar associada à um dos seguintes bancos: {0}.", codigoBancos));
            }

            var codBanco = ContaBancoDAO.Instance.ObtemCodigoBanco(idContaBanco);

            hdfBanco.Value = codBanco.ToString();

            var valorPadrao = DadosCnabDAO.Instance.ObtemValorPadrao((int)codBanco, Request["codigoNotaFiscal"].StrParaInt(), Request["codigoContaReceber"].StrParaInt());

            drpCarteira.DataBind();
            drpEspecieDocumento.DataBind();

            if (valorPadrao != null)
            {
                try
                {
                    drpCarteira.SelectedValue = valorPadrao.CodCarteira.ToString();
                    txtInstrucoes.Text = valorPadrao.DescInstrucoes;
                    hdfTipoArquivo.Value = valorPadrao.TipoCnab.ToString();
                    drpEspecieDocumento.DataBind();
                    drpEspecieDocumento.SelectedValue = valorPadrao.CodEspecieDocumento.ToString();
                }
                catch
                {
                }
            }
            else
            {
                drpCarteira.SelectedIndex = 0;
                drpEspecieDocumento.SelectedIndex = 0;
                txtInstrucoes.Text = "";
            }

            if (Configuracoes.FinanceiroConfig.ContasReceber.DesabilitarCamposImpessaoBoletoBancoDoBrasil && codBanco == (int)Sync.Utils.CodigoBanco.BancoBrasil)
            {
                drpContaBanco.Enabled = false;
                drpCarteira.Enabled = false;
                drpEspecieDocumento.Enabled = false;
                txtInstrucoes.Enabled = false;
            }
        }
    
        protected void drpContaBanco_DataBound(object sender, EventArgs e)
        {
            btnGerar.Enabled = drpContaBanco.Items.Count > 0;
            lblSemConta.Visible = !btnGerar.Enabled;
    
            drpContaBanco_SelectedIndexChanged(sender, e);
        }
    
        protected string BoletoJaImpresso()
        {
            var codigoNotaFiscal = Conversoes.StrParaInt(Request["codigoNotaFiscal"]);
            var codigoContaReceber = Conversoes.StrParaInt(Request["codigoContaReceber"]);
            var codigoLiberacao = Conversoes.StrParaInt(Request["codigoLiberacao"]);

            string mensagem = WebGlass.Business.Boleto.Fluxo.Impresso.Instance.MensagemBoletoImpresso(codigoContaReceber, codigoNotaFiscal, codigoLiberacao);
            return !String.IsNullOrEmpty(mensagem) ? String.Format("Boleto{0}: {1}",
                mensagem[mensagem.Length - 1] == 's' ? "s" : String.Empty, mensagem) : String.Empty;
        }
    }
}
