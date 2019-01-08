using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios
{
    public interface IDominioFluxo
    {
        Entidades.Dominio.Arquivo GerarArquivoRecebidas(int? idPedido, int? idLiberarPedido, int? idAcerto, int? idAcertoParcial, int? idTrocaDevolucao,
            int? numeroNFe, int? idLoja, int? idFunc, int? idFuncRecebido, int? idCli, int? tipoEntrega,
            string nomeCli, DateTime? dtIniVenc, DateTime? dtFimVenc, DateTime? dtIniRec, DateTime? dtFimRec,
            DateTime? dataIniCad, DateTime? dataFimCad,
            int? idFormaPagto, int? idTipoBoleto, decimal? precoInicial, decimal? precoFinal, int? idContaBancoRecebimento, bool? renegociadas,
            bool? recebida, int? idComissionado, int? idRota, string obs, int? ordenacao, IEnumerable<int> tipoContaContabil,
            int? numArqRemessa, bool refObra, int? contasCnab, int? idVendedorAssociado, int? idVendedorObra, int? idComissao, int? numCte,
            bool protestadas, bool contasVinculadas);

        Entidades.Dominio.Arquivo GerarArquivoPagas(int? idContaPg, int? idCompra, string nf, int? idLoja, int? idCustoFixo, int? idImpostoServ, int? idFornec, string nomeFornec, int? formaPagto,
            DateTime? dataInicioCadastro, DateTime? dataFimCadastro, DateTime? dtIniPago, DateTime? dtFimPago, DateTime? dtIniVenc, DateTime? dtFimVenc, decimal? valorInicial, decimal? valorFinal,
            int? tipo, bool comissao, bool renegociadas, string planoConta, bool custoFixo, bool exibirAPagar, int? idComissao, int? numeroCte, string observacao);
    }
}
