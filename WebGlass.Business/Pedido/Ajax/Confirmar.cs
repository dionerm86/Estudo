using System;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace WebGlass.Business.Pedido.Ajax
{
    public interface IConfirmar
    {
        string ConfirmarPedido(string idPedido, string fPagtos, string tpCartoes, string valores,
            string contasBanco, string depositoNaoIdentificado, string gerarCredito, string creditoUtilizado, string numAutConstrucard,
            string parcCreditos, string chequesPagto, string descontarComissao, string tipoVendaObraStr, string numAutCartao);

        string ConfirmarPrazo(string idPedidoStr, string tipoVendaObraStr, string verificarParcelas);

        string ConfirmarObra(string idPedidoStr, string fPagtos, string tpCartoes, string valores,
            string contasBanco, string depositoNaoIdentificado, string gerarCredito, string creditoUtilizado, string numAutConstrucard,
            string parcCreditos, string chequesPagto, string descontarComissao, string fPagto, string tipoCartao,
            string valoresParcelas, string datasParcelas, string tipoVendaObraStr, string numAutCartao);

        string ConfirmarFunc(string idPedidoStr);
    }

    internal class Confirmar : IConfirmar
    {
        public string ConfirmarPedido(string idPedido, string fPagtos, string tpCartoes, string valores,
            string contasBanco, string depositoNaoIdentificado, string gerarCredito, string creditoUtilizado, string numAutConstrucard,
            string parcCreditos, string chequesPagto, string descontarComissao, string tipoVendaObraStr, string numAutCartao)
        {
            try
            {
                string[] sFormasPagto = fPagtos.Split(';');
                string[] sValoresReceb = valores.Split(';');
                string[] sIdContasBanco = contasBanco.Split(';');
                string[] sTiposCartao = tpCartoes.Split(';');
                string[] sParcCartoes = parcCreditos.Split(';');
                string[] sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
                string[] sNumAutCartao = numAutCartao.Split(';');

                uint[] formasPagto = new uint[sFormasPagto.Length];
                decimal[] valoresReceb = new decimal[sValoresReceb.Length];
                uint[] idContasBanco = new uint[sIdContasBanco.Length];
                uint[] tiposCartao = new uint[sTiposCartao.Length];
                uint[] parcCartoes = new uint[sParcCartoes.Length];
                uint[] depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];

                for (int i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = !string.IsNullOrEmpty(sFormasPagto[i]) ? Convert.ToUInt32(sFormasPagto[i]) : 0;
                    valoresReceb[i] = !string.IsNullOrEmpty(sValoresReceb[i])
                        ? Convert.ToDecimal(sValoresReceb[i].Replace('.', ','))
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

                decimal creditoUtil = Glass.Conversoes.StrParaDecimal(creditoUtilizado);
                int tipoVendaObra = !string.IsNullOrEmpty(tipoVendaObraStr)
                    ? Glass.Conversoes.StrParaInt(tipoVendaObraStr)
                    : 0;

                // Confirma pedido
                string msg = PedidoDAO.Instance.ConfirmarPedido(Glass.Conversoes.StrParaUint(idPedido), formasPagto,
                    tiposCartao, valoresReceb, idContasBanco, depNaoIdentificado,  gerarCredito == "true",
                    creditoUtil, numAutConstrucard, parcCartoes, chequesPagto, descontarComissao == "true",
                    tipoVendaObra, true, sNumAutCartao);

                return "ok\t" + msg + "\t" +
                       PedidoDAO.Instance.ExibirNotaPromissoria(Glass.Conversoes.StrParaUint(idPedido))
                           .ToString()
                           .ToLower();
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }

        public string ConfirmarPrazo(string idPedidoStr, string tipoVendaObraStr, string verificarParcelas)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                int tipoVendaObra = !String.IsNullOrEmpty(tipoVendaObraStr)
                    ? Glass.Conversoes.StrParaInt(tipoVendaObraStr)
                    : 0;

                // Confirma pedido
                string msg = PedidoDAO.Instance.ConfirmarPedido(idPedido, new uint[] {0}, new uint[] {0},
                    new decimal[] {0}, new uint[] {0}, new uint[] {0}, false,
                    0, null, new uint[] {0}, null, false, tipoVendaObra, bool.Parse(verificarParcelas), new string[] { "" });

                return "ok\t" + msg + "\t" + PedidoDAO.Instance.ExibirNotaPromissoria(idPedido).ToString().ToLower();
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }

        public string ConfirmarObra(string idPedidoStr, string fPagtos, string tpCartoes, string valores,
            string contasBanco, string depositoNaoIdentificado, string gerarCredito, string creditoUtilizado, string numAutConstrucard,
            string parcCreditos, string chequesPagto, string descontarComissao, string fPagto, string tipoCartao,
            string valoresParcelas, string datasParcelas, string tipoVendaObraStr, string numAutCartao)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
                int tipoVendaObra = !String.IsNullOrEmpty(tipoVendaObraStr)
                    ? Glass.Conversoes.StrParaInt(tipoVendaObraStr)
                    : 0;
                string[] confirmar;

                if (tipoVendaObra > 0)
                {
                    string retornoConfirmar = null;

                    if (tipoVendaObra == (int) Glass.Data.Model.Pedido.TipoVendaPedido.AVista)
                    {
                        retornoConfirmar = ConfirmarPedido(idPedidoStr, fPagtos, tpCartoes, valores, contasBanco,
                            depositoNaoIdentificado, gerarCredito, creditoUtilizado, numAutConstrucard,
                            parcCreditos, chequesPagto, descontarComissao, tipoVendaObraStr, numAutCartao);
                    }
                    else if (tipoVendaObra == (int) Glass.Data.Model.Pedido.TipoVendaPedido.APrazo)
                    {
                        uint idFormaPagto = Glass.Conversoes.StrParaUint(fPagto);
                        uint idTipoCartao = Glass.Conversoes.StrParaUint(tipoCartao);

                        var ped = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);
                        ped.IdFormaPagto = idFormaPagto > 0 ? (uint?) idFormaPagto : null;
                        ped.IdTipoCartao = idFormaPagto == (uint) Glass.Data.Model.Pagto.FormaPagto.Cartao &&
                                           idTipoCartao > 0
                            ? (uint?) idTipoCartao
                            : null;

                        PedidoDAO.Instance.UpdateBase(ped);

                        ParcelasPedidoDAO.Instance.DeleteFromPedido(idPedido);

                        string[] sValoresParc = valoresParcelas.Split(';');
                        string[] sDatasParc = datasParcelas.Split(';');

                        for (int i = 0; i < sValoresParc.Length; i++)
                        {
                            decimal valor = !String.IsNullOrEmpty(sValoresParc[i])
                                ? Convert.ToDecimal(sValoresParc[i].Replace('.', ','))
                                : 0;
                            DateTime data = !String.IsNullOrEmpty(sDatasParc[i])
                                ? Convert.ToDateTime(sDatasParc[i])
                                : new DateTime();

                            ParcelasPedido nova = new ParcelasPedido();
                            nova.IdPedido = idPedido;
                            nova.Data = data;
                            nova.Valor = valor;

                            ParcelasPedidoDAO.Instance.Insert(nova);
                        }

                        retornoConfirmar = ConfirmarPrazo(idPedidoStr, tipoVendaObraStr, "true");
                    }

                    confirmar = retornoConfirmar.Split('\t');
                    if (confirmar[0] == "Erro")
                        throw new Exception(confirmar[1]);
                }

                // Confirma pedido
                PedidoDAO.Instance.ConfirmarPedidoObra(idPedido, tipoVendaObra == 0);
                return "ok\tPedido confirmado.\t" +
                       PedidoDAO.Instance.ExibirNotaPromissoria(idPedido).ToString().ToLower();
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }

        public string ConfirmarFunc(string idPedidoStr)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);

                // Confirma pedido
                string msg = PedidoDAO.Instance.ConfirmarPedido(idPedido, new uint[] {0}, new uint[] {0},
                    new decimal[] {0}, new uint[] {0}, new uint[] {0}, false, 0, null, new uint[] {0}, null, false, 0,
                    false, new string[] { "" });

                return "ok\t" + msg + "\t" + PedidoDAO.Instance.ExibirNotaPromissoria(idPedido).ToString().ToLower();
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg(null, ex);
            }
        }
    }
}
