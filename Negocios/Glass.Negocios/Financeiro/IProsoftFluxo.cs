using System;

namespace Glass.Financeiro.Negocios
{
    public interface IProsoftFluxo
    {
        Entidades.Prosoft.Arquivo GerarArquivoRecebidas(int? idPedido, int? idLiberarPedido, int? idAcerto, int? idAcertoParcial, int? idTrocaDevolucao, int? numeroNFe,
            int? idLoja, int? idCli, int? idFunc, int? idFuncRecebido, int? tipoEntrega, string nomeCli, DateTime? dtIniVenc, DateTime? dtFimVenc, DateTime? dtIniRec,
            DateTime? dtFimRec, DateTime? dataIniCad, DateTime? dataFimCad, int? idFormaPagto, int? idTipoBoleto, decimal? precoInicial, decimal? precoFinal, bool? renegociadas, int? idComissionado,
            int? idRota, string obs, int? numArqRemessa, int? idVendedorObra, bool refObra, int? contasCnab, bool contasVinculadas);

        Entidades.Prosoft.Arquivo GerarArquivoPagas(int? idCompra, int? numNfPedido, int? idCustoFixo, int? idImpServ, int? idComissao, decimal? valorPagtoIni, decimal? valorPagtoFim,
            DateTime? dataVencIni, DateTime? dataVencFim, DateTime? dataPagtoIni, DateTime? dataPagtoFim, int? idLoja, int? idFornec, string nomeFornec, int? idFormaPagto, int? idConta,
            bool jurosMulta, string observacao);
    }
}
