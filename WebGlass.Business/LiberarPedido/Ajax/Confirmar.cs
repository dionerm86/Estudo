using System;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.LiberarPedido.Ajax
{
    public interface IConfirmar
    {
        string ConfirmarAVista(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar, string fPagtos, string tpCartoes, string totalASerPagoStr,
            string valores, string contas, string depositoNaoIdentificado, string cartaoNaoIdentificado, string gerarCredito, string utilizarCredito, string creditoUtilizado, string numAutConstrucard,
            string cxDiario, string parcCredito, string descontarComissao, string chequesPagto, string tipoDescontoStr,
            string descontoStr, string tipoAcrescimoStr, string acrescimoStr, string valorUtilizadoObraStr, string numAutCartao);

        string ConfirmarAPrazo(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar, string totalASerPagoStr, string numParcelasStr, string diasParcelasStr,
            string idParcelaStr, string valoresParcelasStr, string receberEntradaStr, string fPagtos, string tpCartoes,
            string valores, string contas, string depositoNaoIdentificado, string cartaoNaoIdentificado, string utilizarCredito, string creditoUtilizado, string numAutConstrucard, string cxDiario,
            string parcCredito, string descontarComissao, string tipoDescontoStr, string descontoStr, string tipoAcrescimoStr,
            string acrescimoStr, string formaPagtoPrazoStr, string valorUtilizadoObraStr, string chequesPagto, string numAutCartao);

        string ConfirmarGarantiaReposicao(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar);

        string ConfirmarPedidoFuncionario(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar);
    }

    internal class Confirmar : IConfirmar
    {
        public string ConfirmarAVista(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar, string fPagtos, string tpCartoes, string totalASerPagoStr,
            string valores, string contas, string depositoNaoIdentificado, string cartaoNaoIdentificado, string gerarCredito, string utilizarCredito, string creditoUtilizado, string numAutConstrucard,
            string cxDiario, string parcCredito, string descontarComissao, string chequesPagto, string tipoDescontoStr,
            string descontoStr, string tipoAcrescimoStr, string acrescimoStr, string valorUtilizadoObraStr, string numAutCartao)
        {
            try
            {
                // Verifica se o cliente está ativo
                if (ClienteDAO.Instance.GetSituacao(Glass.Conversoes.StrParaUint(idCliente)) == 2)
                    return "Erro\tCliente inativo. Motivo: " +
                           ClienteDAO.Instance.ObtemObs(Glass.Conversoes.StrParaUint(idCliente))
                               .Replace("'", String.Empty)
                               .Replace("\n", "")
                               .Replace("\r", "");

                string[] sFormasPagto = fPagtos.Split(';');
                string[] sValoresPagos = valores.Split(';');
                string[] sIdContasBanco = contas.Split(';');
                string[] sTiposCartao = tpCartoes.Split(';');
                string[] sParcCartoes = parcCredito.Split(';');
                string[] sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
                string[] sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(';');
                string[] sNumAutCartao = numAutCartao.Split(';');

                uint[] formasPagto = new uint[sFormasPagto.Length];
                decimal[] valoresPagos = new decimal[sValoresPagos.Length];
                uint[] idContasBanco = new uint[sIdContasBanco.Length];
                uint[] tiposCartao = new uint[sTiposCartao.Length];
                uint[] parcCartoes = new uint[sParcCartoes.Length];
                uint[] depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
                uint[] cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];

                for (int i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = !String.IsNullOrEmpty(sFormasPagto[i]) ? Convert.ToUInt32(sFormasPagto[i]) : 0;
                    valoresPagos[i] = !String.IsNullOrEmpty(sValoresPagos[i])
                        ? Convert.ToDecimal(sValoresPagos[i].Replace('.', ','))
                        : 0;
                    idContasBanco[i] = !String.IsNullOrEmpty(sIdContasBanco[i])
                        ? Convert.ToUInt32(sIdContasBanco[i])
                        : 0;
                    tiposCartao[i] = !String.IsNullOrEmpty(sTiposCartao[i]) ? Convert.ToUInt32(sTiposCartao[i]) : 0;
                    parcCartoes[i] = !String.IsNullOrEmpty(sParcCartoes[i]) ? Convert.ToUInt32(sParcCartoes[i]) : 0;
                    depNaoIdentificado[i] = !String.IsNullOrEmpty(sDepositoNaoIdentificado[i])
                        ? Convert.ToUInt32(sDepositoNaoIdentificado[i])
                        : 0;                  
                }

                for (int i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? Convert.ToUInt32(sCartaoNaoIdentificado[i]) : 0;
                }

                decimal totalASerPago = Glass.Conversoes.StrParaDecimal(totalASerPagoStr);
                decimal creditoUtil = Glass.Conversoes.StrParaDecimal(creditoUtilizado);

                int tipoDesconto = Glass.Conversoes.StrParaInt(tipoDescontoStr);
                decimal desconto = !String.IsNullOrEmpty(descontoStr) && descontoStr != "NaN"
                    ? decimal.Parse(descontoStr)
                    : 0;
                int tipoAcrescimo = Glass.Conversoes.StrParaInt(tipoAcrescimoStr);
                decimal acrescimo = !String.IsNullOrEmpty(acrescimoStr) && acrescimoStr != "NaN"
                    ? decimal.Parse(acrescimoStr)
                    : 0;

                string[] sProdutosLiberar = idsProdutosPedido.Split(';');
                string[] sQtdeLiberar = qtdeProdutosLiberar.Split(';');
                string[] sProdutosProducao = idsProdutosProducao.Split(';');

                uint[] produtosLiberar = new uint[sProdutosLiberar.Length];
                float[] qtdeLiberar = new float[sQtdeLiberar.Length];
                uint?[] produtosProducaoLiberar = new uint?[sProdutosProducao.Length];

                for (int i = 0; i < sProdutosLiberar.Length; i++)
                {
                    produtosLiberar[i] = Convert.ToUInt32(sProdutosLiberar[i]);
                    qtdeLiberar[i] = Glass.Conversoes.StrParaFloat(sQtdeLiberar[i]);
                    produtosProducaoLiberar[i] = Glass.Conversoes.StrParaUintNullable(sProdutosProducao[i]);
                }

                foreach (string idPedido in idsPedido.Split(','))
                {
                    if (String.IsNullOrEmpty(idPedido))
                        continue;

                    string[] retorno = Pedido.Fluxo.BuscarEValidar.Ajax.ValidaPedido(idPedido, null, null, cxDiario, null).Split('|');

                    if (retorno[0] != "true")
                        return "Erro\tPedido " + idPedido + ": " + retorno[1];
                }

                decimal valorUtilizadoObra = Glass.Conversoes.StrParaDecimal(valorUtilizadoObraStr);

                // Cria liberação de pedido
                uint idLiberarPedido =
                    LiberarPedidoDAO.Instance.CriarLiberacaoAVista(Glass.Conversoes.StrParaUint(idCliente), idsPedido,
                        produtosLiberar,
                        produtosProducaoLiberar, qtdeLiberar, formasPagto, tiposCartao, totalASerPago, valoresPagos,
                        idContasBanco, depNaoIdentificado, cartNaoIdentificado,
                        gerarCredito == "true", utilizarCredito == "true", creditoUtil, numAutConstrucard,
                        cxDiario == "1",
                        descontarComissao == "true", chequesPagto, parcCartoes, tipoDesconto, desconto, tipoAcrescimo,
                        acrescimo,
                        valorUtilizadoObra, sNumAutCartao);

                return "ok\tPedidos liberados.\t" +
                       LiberarPedidoDAO.Instance.ExibirNotaPromissoria(idLiberarPedido).ToString().ToLower() + "\t" +
                       idLiberarPedido;
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }

        public string ConfirmarAPrazo(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar, string totalASerPagoStr, string numParcelasStr, string diasParcelasStr,
            string idParcelaStr, string valoresParcelasStr, string receberEntradaStr, string fPagtos, string tpCartoes,
            string valores, string contas, string depositoNaoIdentificado, string cartaoNaoIdentificado, string utilizarCredito, string creditoUtilizado, string numAutConstrucard, string cxDiario,
            string parcCredito, string descontarComissao, string tipoDescontoStr, string descontoStr, string tipoAcrescimoStr,
            string acrescimoStr, string formaPagtoPrazoStr, string valorUtilizadoObraStr, string chequesPagto, string numAutCartao)
        {
            try
            {
                // Verifica se o cliente está ativo
                if (ClienteDAO.Instance.GetSituacao(Glass.Conversoes.StrParaUint(idCliente)) == 2)
                    return "Erro\tCliente inativo. Motivo: " +
                           ClienteDAO.Instance.ObtemObs(Glass.Conversoes.StrParaUint(idCliente))
                               .Replace("'", String.Empty)
                               .Replace("\n", "")
                               .Replace("\r", "");

                decimal totalASerPago = Glass.Conversoes.StrParaDecimal(totalASerPagoStr);

                int numParcelas = Glass.Conversoes.StrParaInt(numParcelasStr);
                string[] sDiasParcelas = diasParcelasStr.Split(',');
                string[] sValoresParcelas = valoresParcelasStr.Split(';');
                string[] sNumAutCartao = numAutCartao.Split(';');

                int[] diasParcelas = new int[sDiasParcelas.Length];
                decimal[] valoresParcelas = new decimal[sValoresParcelas.Length];

                for (int i = 0; i < sDiasParcelas.Length; i++)
                {
                    diasParcelas[i] = !String.IsNullOrEmpty(sDiasParcelas[i])
                        ? Glass.Conversoes.StrParaInt(sDiasParcelas[i])
                        : 28;
                    valoresParcelas[i] = !String.IsNullOrEmpty(sValoresParcelas[i])
                        ? decimal.Parse(sValoresParcelas[i].Replace('.', ','))
                        : 0;
                }

                bool receberEntrada = bool.Parse(receberEntradaStr);

                string[] sFormasPagto = fPagtos.Split(';');
                string[] sValoresPagos = valores.Split(';');
                string[] sIdContasBanco = contas.Split(';');
                string[] sTiposCartao = tpCartoes.Split(';');
                string[] sParcCartoes = parcCredito.Split(';');
                string[] sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
                var sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(',');

                uint[] formasPagto = new uint[sFormasPagto.Length];
                decimal[] valoresPagos = new decimal[sValoresPagos.Length];
                uint[] idContasBanco = new uint[sIdContasBanco.Length];
                uint[] tiposCartao = new uint[sTiposCartao.Length];
                uint[] parcCartoes = new uint[sParcCartoes.Length];
                uint[] depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
                var cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];

                for (int i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = !string.IsNullOrEmpty(sFormasPagto[i]) ? Convert.ToUInt32(sFormasPagto[i]) : 0;
                    valoresPagos[i] = !string.IsNullOrEmpty(sValoresPagos[i])
                        ? Convert.ToDecimal(sValoresPagos[i].Replace('.', ','))
                        : 0;
                    idContasBanco[i] = !string.IsNullOrEmpty(sIdContasBanco[i])
                        ? Convert.ToUInt32(sIdContasBanco[i])
                        : 0;
                    tiposCartao[i] = !string.IsNullOrEmpty(sTiposCartao[i]) ? Convert.ToUInt32(sTiposCartao[i]) : 0;
                    parcCartoes[i] = !string.IsNullOrEmpty(sParcCartoes[i]) ? Convert.ToUInt32(sParcCartoes[i]) : 0;
                    depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i])
                        ? Convert.ToUInt32(sDepositoNaoIdentificado[i])
                        : 0;                    
                }

                for (int i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? Convert.ToUInt32(sCartaoNaoIdentificado[i]) : 0;
                }

                decimal creditoUtil = Glass.Conversoes.StrParaDecimal(creditoUtilizado);

                int tipoDesconto = Glass.Conversoes.StrParaInt(tipoDescontoStr);
                decimal desconto = !string.IsNullOrEmpty(descontoStr) && descontoStr != "NaN"
                    ? decimal.Parse(descontoStr.Replace('.', ','))
                    : 0;
                int tipoAcrescimo = Glass.Conversoes.StrParaInt(tipoAcrescimoStr);
                decimal acrescimo = !string.IsNullOrEmpty(acrescimoStr) && acrescimoStr != "NaN"
                    ? decimal.Parse(acrescimoStr.Replace('.', ','))
                    : 0;

                string[] sProdutosLiberar = idsProdutosPedido.Split(';');
                string[] sQtdeLiberar = qtdeProdutosLiberar.Split(';');
                string[] sProdutosProducao = idsProdutosProducao.Split(';');

                uint[] produtosLiberar = new uint[sProdutosLiberar.Length];
                float[] qtdeLiberar = new float[sQtdeLiberar.Length];
                uint?[] produtosProducaoLiberar = new uint?[sProdutosProducao.Length];

                for (int i = 0; i < sProdutosLiberar.Length; i++)
                {
                    produtosLiberar[i] = Convert.ToUInt32(sProdutosLiberar[i]);
                    qtdeLiberar[i] = Glass.Conversoes.StrParaFloat(sQtdeLiberar[i]);
                    produtosProducaoLiberar[i] = Glass.Conversoes.StrParaUintNullable(sProdutosProducao[i]);
                }

                foreach (string idPedido in idsPedido.Split(','))
                {
                    if (String.IsNullOrEmpty(idPedido))
                        continue;

                    string[] retorno = Pedido.Fluxo.BuscarEValidar.Ajax.ValidaPedido(idPedido, null, null, cxDiario, null).Split('|');

                    if (retorno[0] != "true")
                        return "Erro\tPedido " + idPedido + ": " + retorno[1];
                }

                if (String.IsNullOrEmpty(formaPagtoPrazoStr) && totalASerPago > 0)
                {
                    foreach (var id in idsPedido.Split(','))
                    {
                        /* Chamado 15029.
                         * Caso todos os pedidos tenham sido cadastrados com o tipo de venda Obra, então a forma de pagamento não deve ser
                         * solicitada, pois, o valor do pedido foi descontado no valor da obra. */
                        if (Glass.Data.DAL.PedidoDAO.Instance.ObtemTipoVenda(Glass.Conversoes.StrParaUint(id)) !=
                            (int) Glass.Data.Model.Pedido.TipoVendaPedido.Obra)
                            return "Erro\tInforme a forma de pagamento da liberação.";
                    }
                }

                uint formaPagtoPrazo = Glass.Conversoes.StrParaUint(formaPagtoPrazoStr);
                decimal valorUtilizadoObra = Glass.Conversoes.StrParaDecimal(valorUtilizadoObraStr);

                uint? idParcela = null;

                if (!String.IsNullOrEmpty(idParcelaStr))
                    idParcela = Glass.Conversoes.StrParaUintNullable(idParcelaStr);

                // Cria liberação de pedido
                uint idLiberarPedido =
                    LiberarPedidoDAO.Instance.CriarLiberacaoAPrazo(Glass.Conversoes.StrParaUint(idCliente), idsPedido,
                        produtosLiberar, produtosProducaoLiberar, qtdeLiberar, totalASerPago, numParcelas,
                        diasParcelas, valoresParcelas, idParcela, receberEntrada, formasPagto, tiposCartao,
                        valoresPagos, idContasBanco, depNaoIdentificado, cartNaoIdentificado, utilizarCredito == "true", creditoUtil,
                        numAutConstrucard, cxDiario == "1", descontarComissao == "true", parcCartoes, tipoDesconto,
                        desconto, tipoAcrescimo, acrescimo, formaPagtoPrazo, valorUtilizadoObra, chequesPagto, sNumAutCartao);

                return "ok\tPedidos liberados.\t" +
                       LiberarPedidoDAO.Instance.ExibirNotaPromissoria(idLiberarPedido).ToString().ToLower() + "\t" +
                       idLiberarPedido;
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }

        public string ConfirmarGarantiaReposicao(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar)
        {
            try
            {
                // Verifica se o cliente está ativo
                if (ClienteDAO.Instance.GetSituacao(Glass.Conversoes.StrParaUint(idCliente)) == 2)
                    return "Erro\tCliente inativo. Motivo: " +
                           ClienteDAO.Instance.ObtemObs(Glass.Conversoes.StrParaUint(idCliente))
                               .Replace("'", String.Empty)
                               .Replace("\n", "")
                               .Replace("\r", "");

                string[] sProdutosLiberar = idsProdutosPedido.Split(';');
                string[] sQtdeLiberar = qtdeProdutosLiberar.Split(';');
                string[] sProdutosProducao = idsProdutosProducao.Split(';');

                uint[] produtosLiberar = new uint[sProdutosLiberar.Length];
                float[] qtdeLiberar = new float[sQtdeLiberar.Length];
                uint?[] produtosProducaoLiberar = new uint?[sProdutosProducao.Length];

                for (int i = 0; i < sProdutosLiberar.Length; i++)
                {
                    produtosLiberar[i] = Convert.ToUInt32(sProdutosLiberar[i]);
                    qtdeLiberar[i] = Glass.Conversoes.StrParaFloat(sQtdeLiberar[i]);
                    produtosProducaoLiberar[i] = Glass.Conversoes.StrParaUintNullable(sProdutosProducao[i]);
                }

                // Cria liberação de pedido
                uint idLiberarPedido =
                    LiberarPedidoDAO.Instance.CriarLiberacaoGarantiaReposicao(Glass.Conversoes.StrParaUint(idCliente),
                        idsPedido, produtosLiberar,
                        produtosProducaoLiberar, qtdeLiberar);

                return "ok\tPedidos liberados.\t\t" + idLiberarPedido;
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }

        public string ConfirmarPedidoFuncionario(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar)
        {
            try
            {
                // Verifica se o cliente está ativo
                if (ClienteDAO.Instance.GetSituacao(Glass.Conversoes.StrParaUint(idCliente)) == 2)
                    return "Erro\tCliente inativo. Motivo: " +
                           ClienteDAO.Instance.ObtemObs(Glass.Conversoes.StrParaUint(idCliente))
                               .Replace("'", String.Empty)
                               .Replace("\n", "")
                               .Replace("\r", "");

                string[] sProdutosLiberar = idsProdutosPedido.Split(';');
                string[] sQtdeLiberar = qtdeProdutosLiberar.Split(';');
                string[] sProdutosProducao = idsProdutosProducao.Split(';');

                uint[] produtosLiberar = new uint[sProdutosLiberar.Length];
                float[] qtdeLiberar = new float[sQtdeLiberar.Length];
                uint?[] produtosProducaoLiberar = new uint?[sProdutosProducao.Length];

                for (int i = 0; i < sProdutosLiberar.Length; i++)
                {
                    produtosLiberar[i] = Convert.ToUInt32(sProdutosLiberar[i]);
                    qtdeLiberar[i] = Glass.Conversoes.StrParaFloat(sQtdeLiberar[i]);
                    produtosProducaoLiberar[i] = Glass.Conversoes.StrParaUintNullable(sProdutosProducao[i]);
                }

                // Cria liberação de pedido
                uint idLiberarPedido =
                    LiberarPedidoDAO.Instance.CriarLiberacaoPedidoFuncionario(Glass.Conversoes.StrParaUint(idCliente),
                        idsPedido, produtosLiberar,
                        produtosProducaoLiberar, qtdeLiberar);

                return "ok\tPedidos liberados.\t\t" + idLiberarPedido;
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    }
}
