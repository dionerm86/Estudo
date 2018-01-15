using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class EtiquetaArquivoOtimizacaoDAO : BaseDAO<EtiquetaArquivoOtimizacao, EtiquetaArquivoOtimizacaoDAO>
    {
        //private EtiquetaArquivoOtimizacaoDAO() { }

        /// <summary>
        /// Verifica se a etiqueta tem um arquivo SAG gerado.
        /// </summary>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool TemArquivoSAG(string numEtiqueta)
        {
            return TemArquivoSAG(null, numEtiqueta);
        }

        /// <summary>
        /// Verifica se a etiqueta tem um arquivo SAG gerado.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="numEtiqueta"></param>
        /// <returns></returns>
        public bool TemArquivoSAG(GDASession sessao, string numEtiqueta)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, @"select count(*)
                from etiqueta_arquivo_otimizacao where numEtiqueta=?num and temArquivoOtimizacao",
                new GDAParameter("?num", numEtiqueta)) > 0;
        }
    }
}
