using System;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass;
using System.Linq;
using GDA;
using System.Collections.Generic;

namespace WebGlass.Business.Boleto.Fluxo
{
    public sealed class Impresso : BaseFluxo<Impresso>
    {
        private Impresso() { }

        public void IndicarBoletoImpresso(int codigoContaReceber, int? codigoNotaFiscal, int? codigoLiberacao, int? codigoCte, int codigoContaBancaria, LoginUsuario login)
        {
            var boleto = new BoletoImpresso()
            {
                IdContaR = (uint)codigoContaReceber,
                IdNf = codigoNotaFiscal > 0 ? codigoNotaFiscal.Value : (int?)null,
                IdLiberarPedido = codigoLiberacao > 0 ? codigoLiberacao.Value : (int?)null,
                IdContaBanco = codigoContaBancaria,
                IdCte = codigoCte > 0 ? codigoCte.Value : (int?)null,
                Usucad = login.CodUser,
            };

            BoletoImpressoDAO.Instance.InserirBoletoImpresso(null, boleto);
        }

        private BoletoImpresso ObterBoletoImpresso(GDASession session, int codigoContaReceber)
        {
            return BoletoImpressoDAO.Instance.ObterBoletoImpresso(session, codigoContaReceber, null);
        }

        public bool VerificarPossuiBoletoImpresso(GDASession session, int codigoContaReceber)
        {
            return BoletoImpressoDAO.Instance.VerificarPossuiBoletoImpresso(session, codigoContaReceber);
        }

        public string MensagemBoletoImpresso(int? codigoContaReceber, int? codigoNotaFiscal, int? codigoLiberacao, int? codigoCte)
        {
            IEnumerable<int> idsContasReceber = null;

            if (codigoContaReceber > 0)
            {
                var ultimoBoletoImpresso = this.ObterBoletoImpresso(null, codigoContaReceber.Value);

                if (ultimoBoletoImpresso?.IdContaBanco > 0)
                {
                    var descricaoContaBanco = ContaBancoDAO.Instance.GetDescricao(null, (uint)ultimoBoletoImpresso.IdContaBanco);
                    return $"Boleto impresso na conta bancária: {descricaoContaBanco}";
                }
                else
                {
                    return $"Boleto: já impresso";
                }
            }
            else if (codigoNotaFiscal > 0)
            {
                idsContasReceber = ContasReceberDAO.Instance.ObtemPelaNfe((uint)codigoNotaFiscal.Value)?.Select(f => (int)f);
            }
            else if (codigoLiberacao > 0)
            {
                idsContasReceber = ContasReceberDAO.Instance.GetByPedidoLiberacao(0, (uint)codigoLiberacao, null).Select(f => f.IdContaR)?.Select(f => (int)f);
            }
            else if (codigoCte > 0)
            {
                idsContasReceber = ContasReceberDAO.Instance.ObterIdContaRPeloIdCte((uint)codigoCte)?.Select(f => (int)f);
            }

            if (idsContasReceber?.Any(f => f > 0) ?? false)
            {
                idsContasReceber = idsContasReceber.Where(f => f > 0).ToList();

                if (idsContasReceber.Count() == 1)
                {
                    return this.MensagemBoletoImpresso(idsContasReceber.First(), null, null, null);
                }
                else
                {
                    return this.ObterMensagemBoletosImpressos(idsContasReceber);
                }
            }

            return string.Empty;
        }

        private string ObterMensagemBoletosImpressos(IEnumerable<int> idsContaReceber)
        {
            var impressos = 0;
            var descricoesContaBanco = new List<string>();

            foreach (var id in idsContaReceber)
            {
                var ultimoBoletoImpresso = this.ObterBoletoImpresso(null, (int)id);

                if (ultimoBoletoImpresso?.IdContaBanco > 0)
                {
                    var descricaoContaBanco = ContaBancoDAO.Instance.GetDescricao(null, (uint)ultimoBoletoImpresso.IdContaBanco);
                    descricoesContaBanco.Add(descricaoContaBanco);
                    impressos++;
                }
            }

            if (impressos > 0)
            {
                return $"Boleto(s) {impressos}/{idsContaReceber.Count()} impresso(s) na conta bancária: {string.Join(" | ", descricoesContaBanco)}";
            }

            return string.Empty;
        }
    }
}
