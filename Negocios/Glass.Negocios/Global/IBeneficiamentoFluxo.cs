using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de beneficiamentos do sistema.
    /// </summary>
    public interface IBeneficiamentoFluxo
    {
        #region BenefConfig

        /// <summary>
        /// Pesquisa os beneficiamentos.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.BenefConfigPesquisa> PesquisarConfiguracoesBeneficiamento();

        /// <summary>
        /// Recupera os descritores dos beneficiamentos.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemBenefConfig();

        /// <summary>
        /// Recupera os descritores dos beneficiamentos.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemBenefConfigAtivos();

        /// <summary>
        /// Recupera os dados da configuração do beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        Entidades.BenefConfig ObtemBenefConfig(int idBenefConfig);

        /// <summary>
        /// Salva as configurações do beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarBenefConfig(Entidades.BenefConfig benefConfig);

        /// <summary>
        /// Apaga os dados do beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarBenefConfig(Entidades.BenefConfig benefConfig);

        /// <summary>
        /// Altera a posição da configuração do beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig">Identificador da configuração.</param>
        /// <param name="paraCima">Identifica se é para mover para cima.</param>
        /// <returns></returns>
        Colosoft.Business.SaveResult AlterarPosicaoBenefConfig(int idBenefConfig, bool paraCima);

        #endregion

        #region BenefConfigPreco

        /// <summary>
        /// Pesquisa os preços padrão dos beneficiamentos.
        /// </summary>
        /// <param name="descricao">Descricação do beneficiamento que será filtrado.</param>
        /// <returns></returns>
        IList<Entidades.BenefConfigPrecoPadrao> PesquisarPrecosPadraoBeneficiamentos(string descricao);

        /// <summary>
        /// Pesquisa os preços padrão dos beneficiamentos.
        /// </summary>
        /// <param name="descricao">Descricação do beneficiamento que será filtrado.</param>
        /// <returns></returns>
        IList<Entidades.BenefConfigPrecoPesquisa> PesquisarResumoPrecosPadraoBeneficiamentos(string descricao);

        /// <summary>
        /// Recupera os preços das configurações de beneficiamentos pelos
        /// identificadores informados.
        /// </summary>
        /// <param name="idsBenefConfigPreco"></param>
        /// <returns></returns>
        IEnumerable<Entidades.BenefConfigPreco> ObtemPrecosBeneficiamento(IEnumerable<int> idsBenefConfigPreco);

        /// <summary>
        /// Recupera o preço do beneficiamento.
        /// </summary>
        /// <param name="idBenefConfigPreco"></param>
        /// <returns></returns>
        Entidades.BenefConfigPreco ObtemPrecoBeneficiamento(int idBenefConfigPreco);

        /// <summary>
        /// Salva o preço do beneficiamento.
        /// </summary>
        /// <param name="preco"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarPrecoBeneficiamento(Entidades.BenefConfigPreco preco);

        /// <summary>
        /// Salva os preços dos beneficiamentos.
        /// </summary>
        /// <param name="precos">Preços que serão atualizados.</param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarPrecosBeneficiamentos(IEnumerable<Entidades.BenefConfigPreco> precos);

        #endregion
    }
}
