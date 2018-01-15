namespace Glass.Projeto.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do repositório de imagens das ferragens
    /// </summary>
    public interface IFerragemRepositorioImagens
    {
        /// <summary>
        /// Recupera a Url da imagem da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador do ferragem.</param>
        string ObterUrl(int idFerragem);

        /// <summary>
        /// Salva a imagem da ferragem.
        /// </summary>
        /// <param name="idFerragem">Identificador da ferragem.</param>
        /// <param name="stream">Stream contendo os dados da imagem que será salva.</param>
        bool SalvarImagem(int idFerragem, System.IO.Stream stream);
    }
}
