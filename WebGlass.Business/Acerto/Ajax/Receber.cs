using System;
using Glass.Data.DAL;
using Glass;
using System.Linq;
using System.Collections.Generic;

namespace WebGlass.Business.Acerto.Ajax
{
    public interface IReceber
    {
        string ReceberAcerto(string idCliente, string contas, string dataRecebido, string totalASerPago, string fPagtos, string valores, string contasBanco, string depositoNaoIdentificado,
            string cartaoNaoIdentificado, string tpCartoes, string tpBoletos, string txAntecip, string juros, string parcial, string gerarCredito, string creditoUtilizado, string cxDiario,
            string numAutConstrucard, string parcCredito, string chequesPagto, string descontarComissao, string obs, string numAutCartao, string receberCappta);

        string Renegociar(string idCliente, string idContasR, string idFormaPagto, string numParc, string parcelas,
            string multa, string creditoUtilizado, string obs);
    }

    internal class Receber : IReceber
    {
        public string ReceberAcerto(string idCliente, string contas, string dataRecebido, string totalASerPago, string fPagtos, string valores, string contasBanco, string depositoNaoIdentificado,
            string cartaoNaoIdentificado, string tpCartoes, string tpBoletos, string txAntecip, string juros, string parcial, string gerarCredito, string creditoUtilizado, string cxDiario,
            string numAutConstrucard, string parcCredito, string chequesPagto, string descontarComissao, string obs, string numAutCartao, string receberCappta)
        {
            try
            {
                FilaOperacoes.ReceberAcerto.AguardarVez();

                string[] sFormasPagto = fPagtos.Split(';');
                string[] sValoresReceb = valores.Split(';');
                string[] sIdContasBanco = contasBanco.Split(';');
                string[] sTiposCartao = tpCartoes.Split(';');
                string[] sParcCartoes = parcCredito.Split(';');
                string[] sTiposBoleto = tpBoletos.Split(';');
                string[] sTaxasAntecip = txAntecip.Split(';');
                string[] sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
                string[] sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(';');
                string[] sNumAutCartao = numAutCartao.Split(';');

                uint[] formasPagto = new uint[sFormasPagto.Length];
                decimal[] valoresReceb = new decimal[sValoresReceb.Length];
                uint[] idContasBanco = new uint[sIdContasBanco.Length];
                uint[] tiposCartao = new uint[sTiposCartao.Length];
                uint[] parcCartoes = new uint[sParcCartoes.Length];
                uint[] tiposBoleto = new uint[sTiposBoleto.Length];
                decimal[] taxasAntecip = new decimal[sTaxasAntecip.Length];
                uint[] depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
                var cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];

                for (var i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = !string.IsNullOrEmpty(sFormasPagto[i]) ? sFormasPagto[i].StrParaUint() : 0;
                    valoresReceb[i] = !string.IsNullOrEmpty(sValoresReceb[i]) ? sValoresReceb[i].Replace('.', ',').StrParaDecimal() : 0;
                    idContasBanco[i] = !string.IsNullOrEmpty(sIdContasBanco[i]) ? sIdContasBanco[i].StrParaUint() : 0;
                    tiposCartao[i] = !string.IsNullOrEmpty(sTiposCartao[i]) ? sTiposCartao[i].StrParaUint() : 0;
                    parcCartoes[i] = !string.IsNullOrEmpty(sParcCartoes[i]) ? sParcCartoes[i].StrParaUint() : 0;
                    tiposBoleto[i] = !string.IsNullOrEmpty(sTiposBoleto[i]) ? sTiposBoleto[i].StrParaUint() : 0;
                    taxasAntecip[i] = !string.IsNullOrEmpty(sTaxasAntecip[i]) ? sTaxasAntecip[i].StrParaDecimal() : 0;
                    depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i]) ? sDepositoNaoIdentificado[i].StrParaUint() : 0;                    
                }

                for (var i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? sCartaoNaoIdentificado[i].StrParaUint() : 0;
                }

                var valorJuros = juros.StrParaDecimal();
                var creditoUtil = creditoUtilizado.StrParaDecimal();

                // Recebe Contas
                if (receberCappta == "true")
                {
                    var idAcerto = ContasReceberDAO.Instance.CriarPreRecebimentoAcertoComTransacao(cxDiario == "1", creditoUtil, ((chequesPagto?.Split('|').Select(f => f)) ?? new List<string>()),
                        dataRecebido.StrParaDate().GetValueOrDefault(DateTime.Now), descontarComissao == "true", gerarCredito == "true", idCliente.StrParaIntNullable().GetValueOrDefault(),
                        cartNaoIdentificado.Select(f => ((int?)f).GetValueOrDefault()).ToArray(), idContasBanco.Select(f => ((int?)f).GetValueOrDefault()).ToArray(),
                        (contas.TrimEnd(',')?.Split(',').Select(f => f.StrParaIntNullable().GetValueOrDefault()) ?? new List<int>()), depNaoIdentificado.Select(f => ((int?)f).GetValueOrDefault()).ToArray(),
                        formasPagto.Select(f => ((int?)f).GetValueOrDefault()).ToArray(), tiposCartao.Select(f => ((int?)f).GetValueOrDefault()).ToArray(), valorJuros, sNumAutCartao, numAutConstrucard, obs,
                        parcCartoes.Select(f => ((int?)f).GetValueOrDefault()).ToArray(), parcial == "true", taxasAntecip, tiposBoleto.Select(f => ((int?)f).GetValueOrDefault()).ToArray(),
                        totalASerPago.StrParaDecimalNullable().GetValueOrDefault(), valoresReceb);

                    return string.Format("ok\t\t{0}", idAcerto);
                }

                var mensagemRetorno = ContasReceberDAO.Instance.ReceberAcerto(cxDiario == "1", creditoUtil, ((chequesPagto?.Split('|').Select(f => f)) ?? new List<string>()),
                    dataRecebido.StrParaDate().GetValueOrDefault(DateTime.Now), descontarComissao == "true", gerarCredito == "true", idCliente.StrParaIntNullable().GetValueOrDefault(),
                    cartNaoIdentificado.Select(f => ((int?)f).GetValueOrDefault()).ToArray(), idContasBanco.Select(f => ((int?)f).GetValueOrDefault()).ToArray(),
                    (contas.TrimEnd(',')?.Split(',').Select(f => f.StrParaIntNullable().GetValueOrDefault()) ?? new List<int>()), depNaoIdentificado.Select(f => ((int?)f).GetValueOrDefault()).ToArray(),
                    formasPagto.Select(f => ((int?)f).GetValueOrDefault()).ToArray(), tiposCartao.Select(f => ((int?)f).GetValueOrDefault()).ToArray(), valorJuros, sNumAutCartao, numAutConstrucard, obs,
                    parcCartoes.Select(f => ((int?)f).GetValueOrDefault()).ToArray(), parcial == "true", taxasAntecip, tiposBoleto.Select(f => ((int?)f).GetValueOrDefault()).ToArray(),
                    totalASerPago.StrParaDecimalNullable().GetValueOrDefault(), valoresReceb);
                
                // O idAcerto está sendo retornado no final da variável "msg"
                return string.Format("ok\t{0}", mensagemRetorno);
            }
            catch (Exception ex)
            {
                return string.Format("Erro\t{0}", ex.Message);
            }
            finally
            {
                Glass.FilaOperacoes.ReceberAcerto.ProximoFila();
            }
        }

        public string Renegociar(string idCliente, string idContasR, string idFormaPagto, string numParc, string parcelas,
            string multa, string creditoUtilizado, string obs)
        {
            try
            {
                Glass.FilaOperacoes.RenegociarAcerto.AguardarVez();
                ContasReceberDAO.Instance.RenegociarParcela(Glass.Conversoes.StrParaUint(idCliente),
                    idContasR.TrimEnd(','),
                    Glass.Conversoes.StrParaUint(idFormaPagto), Glass.Conversoes.StrParaInt(numParc), parcelas,
                    Glass.Conversoes.StrParaDecimal(multa),
                    Glass.Conversoes.StrParaDecimal(creditoUtilizado), obs);

                string idAcerto = ContasReceberDAO.Instance.ObtemIdsAcerto(idContasR.TrimEnd(','));

                return "ok\tContas renegociadas.\t" + idAcerto;
            }
            catch (Exception ex)
            {
                return "Erro\t" + ex.Message;
            }
            finally
            {
                Glass.FilaOperacoes.RenegociarAcerto.ProximoFila();
            }
        }
    }
}
