using System.Collections.Generic;

namespace Glass.PCP.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio da classificação dos roteiros de produção
    /// </summary>
    public interface IClassificacaoRoteiroProducaoFluxo
    {
        /// <summary>
        /// Salva os dados da Classificação
        /// </summary>
        /// <param name="classificacao"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarClassificacao(Entidades.ClassificacaoRoteiroProducao classificacao);

        /// <summary>
        /// Apaga os dados da Classificação.
        /// </summary>
        /// <param name="classificacao"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarClassificacao(Entidades.ClassificacaoRoteiroProducao classificacao);

        /// <summary>
        /// Pequisa as classificações de roteiro do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.ClassificacaoRoteiroProducao> PesquisarClassificacao();

        /// <summary>
        /// Recupera os descritores das classificações de roteiro do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemClassificacao();

        /// <summary>
        /// Recupera os dados da classificação de roteiro.
        /// </summary>
        /// <param name="IdClassificacaoRoteiroProducao"></param>
        /// <returns></returns>
        Entidades.ClassificacaoRoteiroProducao ObtemClassificacao(int IdClassificacaoRoteiroProducao);

        /// <summary>
        /// Obtem a capacidade diária padrão de uma classificação
        /// </summary>
        /// <param name="idClassificacao"></param>
        /// <returns></returns>
        int ObtemCapacidadeDiaria(int idClassificacao);

        /// <summary>
        /// Obtem a descricao de uma classificação
        /// </summary>
        /// <param name="idClassificacao"></param>
        /// <returns></returns>
        string ObtemDescricao(int idClassificacao);

        /// <summary>
        ///  Obtem a descricao de uma classificação
        /// </summary>
        /// <param name="idsClassificacoes"></param>
        /// <returns></returns>
        List<Entidades.ClassificacaoRoteiroProducao> ObtemClassificacoes(string idsClassificacoes);
    }
}
