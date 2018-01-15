using System;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.ContasPagar.Ajax
{
    public interface IPagar
    {
        string PagarContas(string idPagtoStr, string idFornecStr, string contas, string chequesAssoc, string vetJurosMulta, string dataPagto, string datasFormasPagtoStr,
            string valoresStr, string formasPagtoStr, string tiposCartaoStr, string numParcCartaoStr, string chequesPagto, string idContasBancoStr, string boletosStr,
            string antecipFornecStr, string desconto, string obs, string gerarCredito, string creditoUtilizado, string pagtoParcial, string numAutConstrucard, 
            string retificarStr, string gerarCreditoRetificarStr, string idsContasRemovidas, string lixo);

        string Renegociar(string idPagtoStr, string idFornecStr, string contas, string numParcelasStr, string datasStr, string valoresStr,
            string vetJurosMulta, string retificarStr, string lixo);
    }

    internal class Pagar : IPagar
    {
        public string PagarContas(string idPagtoStr, string idFornecStr, string contas, string chequesAssoc, string vetJurosMulta, string dataPagto, string datasFormasPagtoStr,
            string valoresStr, string formasPagtoStr, string tiposCartaoStr, string numParcCartaoStr, string chequesPagto, string idContasBancoStr, string boletosStr,
            string antecipFornecStr, string desconto, string obs, string gerarCredito, string creditoUtilizado, string pagtoParcial, string numAutConstrucard, 
            string retificarStr, string gerarCreditoRetificarStr, string idsContasRemovidas, string lixo)
        {
            try
            {
                FilaOperacoes.Pagamento.AguardarVez();
                string[] vPagto = valoresStr.Split(';');
                string[] fPagto = formasPagtoStr.Split(';');
                string[] cPagto = idContasBancoStr.Split(';');
                string[] tPagto = tiposCartaoStr.Split(';');
                string[] pPagto = numParcCartaoStr.Split(';');
                string[] dPagto = datasFormasPagtoStr.Split(';');
                string[] aPagto = antecipFornecStr.Split(';');

                string[] boletos = boletosStr.Split(';');
                decimal[] valores = new decimal[vPagto.Length];
                uint[] formasPagto = new uint[fPagto.Length];
                uint[] contasBanco = new uint[cPagto.Length];
                uint[] tiposCartao = new uint[tPagto.Length];
                uint[] numParcCartao = new uint[pPagto.Length];
                uint[] antecipFornec = new uint[aPagto.Length];
                DateTime[] datasFormasPagto = new DateTime[dPagto.Length];

                for (int i = 0; i < vPagto.Length; i++)
                {
                    valores[i] = Glass.Conversoes.StrParaDecimal(vPagto[i]);
                    formasPagto[i] = Glass.Conversoes.StrParaUint(fPagto[i]);
                    contasBanco[i] = Glass.Conversoes.StrParaUint(cPagto[i]);
                    tiposCartao[i] = Glass.Conversoes.StrParaUint(tPagto[i]);
                    numParcCartao[i] = Glass.Conversoes.StrParaUint(pPagto[i]);
                    datasFormasPagto[i] = !String.IsNullOrEmpty(dPagto[i])
                        ? Conversoes.ConverteDataNotNull(dPagto[i])
                        : DateTime.Now;
                    antecipFornec[i] = Glass.Conversoes.StrParaUint(aPagto[i]);
                }

                contas = contas.Trim(' ').Trim(',');
                chequesAssoc = chequesAssoc.TrimEnd('|');

                if (obs != null && obs.Length > 500)
                    return "Erro\tCampo OBS excedeu o limite de caracteres, o máximo permitido são 500 caracteres.";

                uint idPagto = Glass.Conversoes.StrParaUint(idPagtoStr);
                decimal descontoPagto = Glass.Conversoes.StrParaDecimal(desconto);
                decimal creditoUtil = Glass.Conversoes.StrParaDecimal(creditoUtilizado);
                uint idFornec = !String.IsNullOrEmpty(idFornecStr) && idFornecStr != "null"
                    ? Glass.Conversoes.StrParaUint(idFornecStr)
                    : 0;

                bool retificar = retificarStr == "true";
                if (!retificar)
                {
                    // Paga Contas
                    idPagto = ContasPagarDAO.Instance.PagarContasComTransacao(idFornec, contas, chequesAssoc, vetJurosMulta,
                        DateTime.Parse(dataPagto), datasFormasPagto, valores, formasPagto, contasBanco, tiposCartao,
                        numParcCartao, boletos,
                        antecipFornec, chequesPagto, descontoPagto, obs, gerarCredito == "true", creditoUtil,
                        pagtoParcial == "true", numAutConstrucard);

                    return "ok\tPagamento efetuado com sucesso. Num. Pagto. Gerado: " + idPagto + ".\t" + idPagto;
                }
                else
                {
                    bool gerarCreditoRetificar = gerarCreditoRetificarStr == "true";

                    // Retifica contas
                    idPagto = ContasPagarDAO.Instance.RetificarPagto(idPagto, idFornec, contas.TrimEnd(' ').TrimEnd(','),
                        chequesAssoc.TrimEnd('|'),
                        vetJurosMulta, DateTime.Parse(dataPagto), datasFormasPagto, valores, formasPagto, contasBanco,
                        tiposCartao, numParcCartao, boletos,
                        antecipFornec, chequesPagto, descontoPagto, obs, gerarCredito == "true", creditoUtil,
                        pagtoParcial == "true", numAutConstrucard,
                        gerarCreditoRetificar, idsContasRemovidas.Replace(",,", ",").TrimEnd(','));

                    return "ok\tPagamento retificado com sucesso. Num. Pagto.: " + idPagto + ".\t" + idPagto;
                }
            }
            catch (Exception ex)
            {
                return "Erro\t" + ex.Message;
            }
            finally
            {
                FilaOperacoes.Pagamento.ProximoFila();
            }
        }

        public string Renegociar(string idPagtoStr, string idFornecStr, string contas, string numParcelasStr, string datasStr, string valoresStr,
            string vetJurosMulta, string retificarStr, string lixo)
        {
            try
            {
             
                uint idFornec = !String.IsNullOrEmpty(idFornecStr) && idFornecStr != "null"
                    ? Glass.Conversoes.StrParaUint(idFornecStr)
                    : 0;
                int numParcelas = Glass.Conversoes.StrParaInt(numParcelasStr);

                string[] d = datasStr.Split(';');
                string[] v = valoresStr.Split(';');

                DateTime[] datas = new DateTime[d.Length];
                decimal[] valores = new decimal[v.Length];

                for (int i = 0; i < d.Length; i++)
                {
                    datas[i] = Conversoes.ConverteDataNotNull(d[i]);
                    valores[i] = Glass.Conversoes.StrParaDecimal(v[i]);
                }

                uint idPagto = Glass.Conversoes.StrParaUint(idPagtoStr);
                bool retificar = retificarStr == "true";

                if (!retificar)
                {
                    idPagto = ContasPagarDAO.Instance.RenegociarContasComTransacao(idFornec, contas.TrimEnd(' ').TrimEnd(','),
                        numParcelas, datas, valores, vetJurosMulta, "");
                    return "Ok\tContas renegociadas com sucesso. Num. Pagto. Gerado: " + idPagto + ".\t" + idPagto;
                }
                else
                {
                    idPagto = ContasPagarDAO.Instance.RetificarRenegociando(idPagto, idFornec,
                        contas.TrimEnd(' ').TrimEnd(','), numParcelas, datas, valores, vetJurosMulta, "");
                    return "Ok\tContas retificadas com sucesso. Num. Pagto.: " + idPagto + ".\t" + idPagto;
                }
            }
            catch (Exception ex)
            {
                return "Erro\t" + ex.Message;
            }
           
        }
    }
}
