using System.Collections.Generic;

namespace Glass.PCP.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio dos detalhes da reposição de peça.
    /// </summary>
    public interface IDetalhesReposicaoPeca
    {
        /// <summary>
        /// Recupera os detalhes referentes às reposições de peça.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.DetalhesReposicaoPeca> PesquisarDetalhesReposicaoPeca(int idProdPedProducao);
    }
}
