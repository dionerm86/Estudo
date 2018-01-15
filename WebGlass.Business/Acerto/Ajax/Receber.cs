using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Acerto.Ajax
{
    public interface IReceber
    {
        string ReceberAcerto(string idCliente, string contas, string dataRecebido, string totalASerPago, string fPagtos, string valores,
            string contasBanco, string depositoNaoIdentificado, string cartaoNaoIdentificado, string tpCartoes, string tpBoletos, string txAntecip, string juros, string parcial, string gerarCredito,
            string creditoUtilizado, string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto,
            string descontarComissao, string obs, string numAutCartao);

        string Renegociar(string idCliente, string idContasR, string idFormaPagto, string numParc, string parcelas,
            string multa, string creditoUtilizado, string obs);
    }

    internal class Receber : IReceber
    {
        public string ReceberAcerto(string idCliente, string contas, string dataRecebido, string totalASerPago, string fPagtos, string valores,
            string contasBanco, string depositoNaoIdentificado, string cartaoNaoIdentificado, string tpCartoes, string tpBoletos, string txAntecip, string juros, string parcial, string gerarCredito,
            string creditoUtilizado, string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto,
            string descontarComissao, string obs, string numAutCartao)
        {
            try
            {
                Glass.FilaOperacoes.ReceberAcerto.AguardarVez();
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

                for (int i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = !String.IsNullOrEmpty(sFormasPagto[i]) ? Convert.ToUInt32(sFormasPagto[i]) : 0;
                    valoresReceb[i] = !String.IsNullOrEmpty(sValoresReceb[i])
                        ? Convert.ToDecimal(sValoresReceb[i].Replace('.', ','))
                        : 0;
                    idContasBanco[i] = !String.IsNullOrEmpty(sIdContasBanco[i])
                        ? Convert.ToUInt32(sIdContasBanco[i])
                        : 0;
                    tiposCartao[i] = !String.IsNullOrEmpty(sTiposCartao[i]) ? Convert.ToUInt32(sTiposCartao[i]) : 0;
                    parcCartoes[i] = !String.IsNullOrEmpty(sParcCartoes[i]) ? Convert.ToUInt32(sParcCartoes[i]) : 0;
                    tiposBoleto[i] = !String.IsNullOrEmpty(sTiposBoleto[i]) ? Convert.ToUInt32(sTiposBoleto[i]) : 0;
                    taxasAntecip[i] = !String.IsNullOrEmpty(sTaxasAntecip[i]) ? Convert.ToDecimal(sTaxasAntecip[i]) : 0;
                    depNaoIdentificado[i] = !String.IsNullOrEmpty(sDepositoNaoIdentificado[i])
                        ? Convert.ToUInt32(sDepositoNaoIdentificado[i])
                        : 0;                    
                }

                for (int i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? Convert.ToUInt32(sCartaoNaoIdentificado[i]) : 0;
                }

                decimal valorJuros = Glass.Conversoes.StrParaDecimal(juros);
                decimal creditoUtil = Glass.Conversoes.StrParaDecimal(creditoUtilizado);

                // Recebe Contas
                string msg = ContasReceberDAO.Instance.ReceberContaComposto(Glass.Conversoes.StrParaUint(idCliente),
                    contas.TrimEnd(','), dataRecebido,
                    Glass.Conversoes.StrParaDecimal(totalASerPago), valoresReceb, formasPagto, idContasBanco,
                    depNaoIdentificado, cartNaoIdentificado, tiposCartao, tiposBoleto, taxasAntecip,
                    valorJuros, parcial == "true", gerarCredito == "true", creditoUtil, cxDiario == "1",
                    numAutConstrucard, parcCartoes,
                    chequesPagto, descontarComissao == "true", obs, sNumAutCartao);

                // O idAcerto está sendo retornado no final da variável "msg"
                return "ok\t" + msg;
            }
            catch (Exception ex)
            {
                return "Erro\t" + ex.Message;
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
