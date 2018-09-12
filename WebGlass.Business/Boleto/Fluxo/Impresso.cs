using System;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass;
using System.Linq;
using System.Collections.Generic;

namespace WebGlass.Business.Boleto.Fluxo
{
    public sealed class Impresso : BaseFluxo<Impresso>
    {
        private Impresso() { }

        public void IndicarBoletoImpresso(int codigoContaReceber, int? codigoNotaFiscal, int? codigoLiberacao, int? codigoCte, int codigoContaBancaria, LoginUsuario login)
        {
            FilaOperacoes.BoletoImpresso.AguardarVez();

            try
            {
                if (BoletoFoiImpresso(codigoContaReceber))
                    return;

                var boleto = new BoletoImpresso()
                {
                    IdContaR = (uint)codigoContaReceber,
                    IdNf = codigoNotaFiscal > 0 ? codigoNotaFiscal.Value : (int?)null,
                    IdLiberarPedido = codigoLiberacao > 0 ? codigoLiberacao.Value : (int?)null,
                    IdContaBanco = codigoContaBancaria,
                    IdCte = codigoCte > 0 ? codigoCte.Value : (int?)null,
                };

                boleto.Usucad = login.CodUser;
                BoletoImpressoDAO.Instance.Insert(boleto);
            }
            finally
            {
                FilaOperacoes.BoletoImpresso.ProximoFila();
            }
        }

        public bool BoletoFoiImpresso(int codigoContaReceber)
        {
            return BoletoImpressoDAO.Instance.BoletoFoiImpresso(codigoContaReceber, null);
        }

        public string MensagemBoletoImpresso(int? codigoContaReceber, int? codigoNotaFiscal, int? codigoLiberacao)
        {
            if (codigoContaReceber > 0)
                return BoletoFoiImpresso(codigoContaReceber.Value) ? "já impresso" : null;

            else if (codigoNotaFiscal > 0)
            {
                var idsContasReceber = ContasReceberDAO.Instance.ObtemPelaNfe((uint)codigoNotaFiscal.Value);

                if (idsContasReceber.Count == 0)
                    return null;
                else if (idsContasReceber.Count == 1)
                    return MensagemBoletoImpresso((int)idsContasReceber[0], null, null);

                string mensagem = "{0}/{1} já impresso{2}";

                int impressos = 0;
                foreach (var id in idsContasReceber)
                    impressos += BoletoFoiImpresso((int)id) ? 1 : 0;

                return impressos == 0 ? null :
                    String.Format(mensagem, impressos, idsContasReceber.Count, idsContasReceber.Count > 1 ? "s" : String.Empty);
            }
            else if (codigoLiberacao > 0)
            {
                var contasReceberLiberacao = ContasReceberDAO.Instance.GetByPedidoLiberacao(0, (uint)codigoLiberacao, null)?.ToList() ?? new List<Glass.Data.Model.ContasReceber>();

                var idsContasReceber = new List<uint>();

                if (Glass.Configuracoes.FinanceiroConfig.EmitirBoletoApenasContaTipoPagtoBoleto)
                {
                    var contasRecebimentoBoleto = UtilsPlanoConta.ContasRecebimentoBoleto().Split(',').Where(f => f != "0").Select(f => f.StrParaInt()).ToList();

                    var idsContaR = contasReceberLiberacao.Where(f => contasRecebimentoBoleto.Contains((int)f.IdConta.GetValueOrDefault()))?.Select(f => f.IdContaR).ToList() ?? new List<uint>();

                    idsContasReceber.AddRange(idsContaR);
                }
                else
                {
                    idsContasReceber.AddRange(contasReceberLiberacao.Select(f => f.IdContaR).ToList());
                }

                if (idsContasReceber.Count == 0)
                    return null;
                else if (idsContasReceber.Count == 1)
                    return MensagemBoletoImpresso((int)idsContasReceber[0], null, null);

                string mensagem = "{0}/{1} já impresso{2}";

                int impressos = 0;
                foreach (var id in idsContasReceber)
                    impressos += BoletoFoiImpresso((int)id) ? 1 : 0;

                return impressos == 0 ? null :
                    String.Format(mensagem, impressos, idsContasReceber.Count, idsContasReceber.Count > 1 ? "s" : String.Empty);
            }

            return null;
        }
    }
}
