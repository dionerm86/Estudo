using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public class EtiquetaArquivoCorteDAO : BaseDAO<EtiquetaArquivoCorte, EtiquetaArquivoCorteDAO>
    {
        #region Métodos Públicos

        /// <summary>
        /// Recupera o nome do arquivo associado ao número da etiqueta informado.
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public string ObtemNomeArquivo(GDASession session, string numEtiqueta)
        {
            return CurrentPersistenceObject.ExecuteScalar(session,
                "SELECT NomeArquivo FROM etiqueta_arquivo_corte WHERE NumEtiqueta=?numEtiqueta",
                new GDA.GDAParameter("?numEtiqueta", numEtiqueta)) as string;
        }

        /// <summary>
        /// Registra o arquivo da etiqueta.
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <param name="nomeArquivo"></param>
        public void RegistrarArquivo(string numEtiqueta, string nomeArquivo)
        {
            RegistrarArquivo(null, numEtiqueta, nomeArquivo);
        }

        /// <summary>
        /// Registra o arquivo da etiqueta.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="numEtiqueta"></param>
        /// <param name="nomeArquivo"></param>
        public void RegistrarArquivo(GDASession session, string numEtiqueta, string nomeArquivo)
        {
            var etiquetaArquivo = new EtiquetaArquivoCorte
            {
                NumEtiqueta = numEtiqueta,
                NomeArquivo = nomeArquivo
            };

            Insert(session, etiquetaArquivo);
        }

        #endregion
    }
}
