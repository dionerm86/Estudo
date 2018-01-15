namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do repositório de arquivos do relatório dinâmico.
    /// </summary>
    public interface IRelatorioDinamicoRepositorioArquivos
    {
        /// <summary>
        /// Recupera a Url do arquivo do relatório.
        /// </summary>
        /// <param name="idFunc">Identificador do relatório dinâmico.</param>
        /// <returns></returns>
        string ObterUrl(int idRelatorioDinamico);

        /// <summary>
        /// Salva o arquivo do relatório dinâmico.
        /// </summary>
        /// <param name="idRelatorioDinamico">Identificador do relatório dinâmico.</param>
        /// <param name="stream">Stream contendo os dados do relatório que será salvo.</param>
        bool SalvarArquivo(int idRelatorioDinamico, System.IO.Stream stream);
    }
}
