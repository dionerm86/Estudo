namespace Glass.Projeto.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do repositório de CalcPackage das ferragens
    /// </summary>
    public interface IFerragemRepositorioCalcPackage
    {
        /// <summary>
        /// Recupera a Url do CalcPackage da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador do ferragem.</param>
        string ObterUrl(int idFerragem);

        /// <summary>
        /// Recupera o caminho do CalcPackage da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador do ferragem.</param>
        string ObterCaminho(int idFerragem);

        /// <summary>
        /// Salva o CalcPackage da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador da ferragem.</param>
        /// <param name="stream">Stream contendo os dados do CalcPackage que será salva.</param>
        bool SalvarCalcPackage(int idFerragem, System.IO.Stream stream);
    }
}
