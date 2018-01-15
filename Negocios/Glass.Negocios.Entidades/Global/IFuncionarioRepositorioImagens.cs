namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do repositório de imagens dos funcionários.
    /// </summary>
    public interface IFuncionarioRepositorioImagens
    {
        /// <summary>
        /// Recupera a Url da imagem do funcionário.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        string ObtemUrl(int idFunc);

        /// <summary>
        /// Salva a imagem do funcionário.
        /// </summary>
        /// <param name="idFuncionario">Identificador do funcionário.</param>
        /// <param name="stream">Stream contendo os dados da imagem que será salva.</param>
        bool SalvarImagem(int idFuncionario, System.IO.Stream stream);
    }
}
