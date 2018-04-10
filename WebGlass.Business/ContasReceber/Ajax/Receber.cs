using System;
using Glass.Data.DAL;
using System.Linq;
using Glass;

namespace WebGlass.Business.ContasReceber.Ajax
{
    public interface IReceber
    {
        string ReceberConta(string idPedidoStr, string idContaR, string dataRecebido, string fPagtos, string valores, string contas, string tpCartoes, string tpBoleto, string txAntecip, string juros,
            string parcial, string gerarCredito, string creditoUtilizado, string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto, string descontarComissao,
            string depositoNaoIdentificado, string cartaoNaoIdentificado, string numAutCartao, string receberCappta);

        string Renegociar(string idPedido, string idContaR, string idFormaPagto, string numParc, string parcelas, string multa);
    }

    internal class Receber : IReceber
    {
        public string ReceberConta(string idPedidoStr, string idContaR, string dataRecebido, string fPagtos, string valores, string contas, string tpCartoes, string tpBoleto, string txAntecip,
            string juros, string parcial, string gerarCredito, string creditoUtilizado, string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto, string descontarComissao,
            string depositoNaoIdentificado, string cartaoNaoIdentificado, string numAutCartao, string receberCappta)
        {
            try
            {
                FilaOperacoes.ReceberContaReceber.AguardarVez();

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

                for (var i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = !string.IsNullOrEmpty(sFormasPagto[i]) ? sFormasPagto[i].StrParaUint() : 0;
                    valoresReceb[i] = !string.IsNullOrEmpty(sValoresReceb[i]) ? sValoresReceb[i].Replace('.', ',').StrParaDecimal() : 0;
                    idContasBanco[i] = !string.IsNullOrEmpty(sIdContasBanco[i]) ? sIdContasBanco[i].StrParaUint() : 0;
                    tiposCartao[i] = !string.IsNullOrEmpty(sTiposCartao[i]) ? sTiposCartao[i].StrParaUint() : 0;
                    tiposBoleto[i] = !string.IsNullOrEmpty(sTiposBoleto[i]) ? sTiposBoleto[i].StrParaUint() : 0;
                    taxasAntecip[i] = !string.IsNullOrEmpty(sTaxaAntecip[i]) ? sTaxaAntecip[i].StrParaDecimal() : 0;
                    parcCartoes[i] = !string.IsNullOrEmpty(sParcCartoes[i]) ? sParcCartoes[i].StrParaUint() : 0;
                    depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i]) ? sDepositoNaoIdentificado[i].StrParaUint() : 0;
                }                

                for (var i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? sCartaoNaoIdentificado[i].StrParaUint() : 0;
                }

                var valorJuros = juros.StrParaDecimal();
                var creditoUtil = creditoUtilizado.StrParaDecimal();
                uint? idPedido = null;
                var mensagemRetorno = string.Empty;

                if (!string.IsNullOrEmpty(idPedidoStr))
                {
                    idPedido = idPedidoStr.StrParaUint();
                }
                
                if (receberCappta == "true")
                {
                    ContasReceberDAO.Instance.CriarPreRecebimentoContaComTransacao(cxDiario == "1", creditoUtil, chequesPagto?.Split('|'), dataRecebido.StrParaDate().GetValueOrDefault(DateTime.Now),
                        descontarComissao == "true", gerarCredito == "true", idContaR.StrParaInt(), (int?)idPedido, cartNaoIdentificado.Select(f => ((int?)f).GetValueOrDefault()),
                        idContasBanco.Select(f => ((int?)f).GetValueOrDefault()), depNaoIdentificado.Select(f => ((int?)f).GetValueOrDefault()), formasPagto.Select(f => ((int?)f).GetValueOrDefault()),
                        tiposCartao.Select(f => ((int?)f).GetValueOrDefault()), valorJuros, sNumAutCartao, numAutConstrucard, parcCartoes.Select(f => ((int?)f).GetValueOrDefault()), parcial == "true",
                        taxasAntecip, tiposBoleto.Select(f => ((int?)f).GetValueOrDefault()), valoresReceb.Select(f => f));

                    return "ok\t";
                }

                // Recebe Contas.
                mensagemRetorno = ContasReceberDAO.Instance.ReceberContaComTransacao(cxDiario == "1", creditoUtil, chequesPagto?.Split('|'), dataRecebido.StrParaDate().GetValueOrDefault(DateTime.Now),
                    descontarComissao == "true", gerarCredito == "true", idContaR.StrParaInt(), (int?)idPedido, cartNaoIdentificado.Select(f => ((int?)f).GetValueOrDefault()),
                    idContasBanco.Select(f => ((int?)f).GetValueOrDefault()), depNaoIdentificado.Select(f => ((int?)f).GetValueOrDefault()), formasPagto.Select(f => ((int?)f).GetValueOrDefault()),
                    tiposCartao.Select(f => ((int?)f).GetValueOrDefault()), valorJuros, sNumAutCartao, numAutConstrucard, parcCartoes.Select(f => ((int?)f).GetValueOrDefault()), parcial == "true",
                    taxasAntecip, tiposBoleto.Select(f => ((int?)f).GetValueOrDefault()), valoresReceb.Select(f => f));

                return string.Format("ok\t{0}", mensagemRetorno);
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
