using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios
{
    public interface ICartaoNaoIdentificadoFluxo
    {
        /// <summary>
        /// Recupera as informações do CNI para exibição na listagem/Relatório
        /// </summary>
        IList<Entidades.CartaoNaoIdentificadoPesquisa> PesquisarCartoesNaoIdentificados(int? idCartaoNaoIdentificado,
            int? idContaBanco, decimal? valorInicio, decimal? valorFim, Data.Model.SituacaoCartaoNaoIdentificado? situacao, int? tipoCartao,
            DateTime? dataCadInicio, DateTime? dataCadFim, DateTime? dataVendaInicio, DateTime? dataVendaFim, string nAutorizacao, string numEstabelecimento,
            string ultimosDigitosCartao, int? codArquivo, DateTime? dataImportacao);

        /// <summary>
        /// Recupera o Cartão não identificado com base no Id
        /// </summary>
        Entidades.CartaoNaoIdentificado ObterCartaoNaoIdentificado(int idCartaoNaoIdentificado);

        /// <summary>
        /// Salva a instância do cartão não identificado
        /// </summary>
        Colosoft.Business.SaveResult SalvarCartaoNaoIdentificado(Entidades.CartaoNaoIdentificado cartaoNaoIdentificado);

        /// <summary>
        /// Salva a instância do cartão não identificado
        /// </summary>
        Colosoft.Business.DeleteResult ApagarCartaoNaoIdentificado(Entidades.CartaoNaoIdentificado cartaoNaoIdentificado);

        /// <summary>
        /// Cria uma nova instância de CNI
        /// </summary>
        Entidades.CartaoNaoIdentificado CriarCartaoNaoIdentificado();

        /// <summary>
        /// Recupera os Ids das parcelas geradas para um CNI
        /// </summary>
        /// <returns></returns>
        IList<int> PesquisarIdsParcelasCNI(int idCNI);

        /// <summary>
        /// Verifica se o cni passado pode ser inserido no banco
        /// </summary>
        bool VerificarPodeInserir(string numAutCartao, int tipoCartao, out string msgErro);

        /// <summary>
        /// Recupera todos os CNIs de débito que estão gerados sem movimentações bancárias.
        /// </summary>
        IList<Entidades.CartaoNaoIdentificado> ObterCartoesNaoIdentificadosDebitoSemMovimentacao();
    }
}
