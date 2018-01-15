using System;
using Glass.Data.DAL;
using System.Linq;

namespace WebGlass.Business.ContasReceber.Ajax
{
    public interface IReceber
    {
        string ReceberConta(string idPedidoStr, string idContaR, string dataRecebido, string fPagtos, string valores,
            string contas, string tpCartoes, string tpBoleto, string txAntecip, string juros, string parcial, string gerarCredito,
            string creditoUtilizado, string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto,
            string descontarComissao, string depositoNaoIdentificado, string cartaoNaoIdentificado, string numAutCartao);

        string Renegociar(string idPedido, string idContaR, string idFormaPagto, string numParc, string parcelas, 
            string multa);
    }

    internal class Receber : IReceber
    {
        public string ReceberConta(string idPedidoStr, string idContaR, string dataRecebido, string fPagtos, string valores,
            string contas, string tpCartoes, string tpBoleto, string txAntecip, string juros, string parcial, string gerarCredito,
            string creditoUtilizado, string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto,
            string descontarComissao, string depositoNaoIdentificado, string cartaoNaoIdentificado, string numAutCartao)
        {
            try
            {
                Glass.FilaOperacoes.ReceberContaReceber.AguardarVez();
                string[] sFormasPagto = fPagtos.Split(';');
                string[] sValoresReceb = valores.Split(';');
                string[] sIdContasBanco = contas.Split(';');
                string[] sTiposCartao = tpCartoes.Split(';');
                string[] sTiposBoleto = tpBoleto.Split(';');
                string[] sTaxaAntecip = txAntecip.Split(';');
                string[] sParcCartoes = parcCredito.Split(';');
                string[] sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
                string[] sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(';');
                string[] sNumAutCartao = numAutCartao.Split(';');

                uint[] formasPagto = new uint[sFormasPagto.Length];
                decimal[] valoresReceb = new decimal[sValoresReceb.Length];
                uint[] idContasBanco = new uint[sIdContasBanco.Length];
                uint[] tiposCartao = new uint[sTiposCartao.Length];
                uint[] tiposBoleto = new uint[sTiposBoleto.Length];
                decimal[] taxasAntecip = new decimal[sTaxaAntecip.Length];
                uint[] parcCartoes = new uint[sParcCartoes.Length];
                uint[] depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
                var cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];              

                for (int i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = !string.IsNullOrEmpty(sFormasPagto[i]) ? Convert.ToUInt32(sFormasPagto[i]) : 0;
                    valoresReceb[i] = !string.IsNullOrEmpty(sValoresReceb[i]) ? Convert.ToDecimal(sValoresReceb[i].Replace('.', ',')): 0;
                    idContasBanco[i] = !string.IsNullOrEmpty(sIdContasBanco[i]) ? Convert.ToUInt32(sIdContasBanco[i]) : 0;
                    tiposCartao[i] = !string.IsNullOrEmpty(sTiposCartao[i]) ? Convert.ToUInt32(sTiposCartao[i]) : 0;
                    tiposBoleto[i] = !string.IsNullOrEmpty(sTiposBoleto[i]) ? Convert.ToUInt32(sTiposBoleto[i]) : 0;
                    taxasAntecip[i] = !string.IsNullOrEmpty(sTaxaAntecip[i]) ? Convert.ToDecimal(sTaxaAntecip[i]) : 0;
                    parcCartoes[i] = !string.IsNullOrEmpty(sParcCartoes[i]) ? Convert.ToUInt32(sParcCartoes[i]) : 0;
                    depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i]) ? Convert.ToUInt32(sDepositoNaoIdentificado[i]) : 0;
                }                

                for (int i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? Convert.ToUInt32(sCartaoNaoIdentificado[i]) : 0;
                }

                decimal valorJuros = Glass.Conversoes.StrParaDecimal(juros);
                decimal creditoUtil = Glass.Conversoes.StrParaDecimal(creditoUtilizado);

                uint? idPedido = null;
                if (!String.IsNullOrEmpty(idPedidoStr)) idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);

                var msg = string.Empty;

                // Recebe Contas
                msg = ContasReceberDAO.Instance.ReceberContaComTransacao(idPedido,
                    Glass.Conversoes.StrParaUint(idContaR), dataRecebido, valoresReceb,
                    formasPagto, idContasBanco, depNaoIdentificado, cartNaoIdentificado, tiposCartao, tiposBoleto, taxasAntecip, valorJuros,
                    parcial == "true",
                    gerarCredito == "true", creditoUtil, numAutConstrucard, cxDiario == "1", parcCartoes, chequesPagto,
                    descontarComissao == "true", sNumAutCartao);

                return "ok\t" + msg;
            }
            catch (Exception ex)
            {
                return "Erro\t" + ex.Message;
            }
            finally
            {
                Glass.FilaOperacoes.ReceberContaReceber.ProximoFila();
            }
        }

        public string Renegociar(string idPedido, string idContaR, string idFormaPagto, string numParc, string parcelas, 
            string multa)
        {
            try
            {
                Glass.FilaOperacoes.ReceberContaReceber.AguardarVez();
                ContasReceberDAO.Instance.RenegociarParcela(Glass.Conversoes.StrParaUint(idPedido),
                    Glass.Conversoes.StrParaUint(idContaR), Glass.Conversoes.StrParaUint(idFormaPagto),
                    Glass.Conversoes.StrParaInt(numParc), parcelas, Glass.Conversoes.StrParaDecimal(multa));

                return "ok\tConta renegociada.";
            }
            catch (Exception ex)
            {
                return "Erro\t" + ex.Message;
            }
            finally
            {
                Glass.FilaOperacoes.ReceberContaReceber.ProximoFila();
            }
        }
    }
}
