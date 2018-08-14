using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Assinatura do repositório dos arquivos das soluções de otimização.
    /// </summary>
    public interface IRepositorioSolucaoOtimizacao
    {
        #region Métodos

        /// <summary>
        /// Verifica se existe o arquivo com o nome informado associado com a solução.
        /// </summary>
        /// <param name="solucaoOtimizacao"></param>
        /// <param name="nome">Nome do arquivo que será pesquisado.</param>
        /// <returns></returns>
        bool ArquivoExiste(ISolucaoOtimizacao solucaoOtimizacao, string nome);

        /// <summary>
        /// Obtém os arquivos da solução.
        /// </summary>
        /// <param name="solucaoOtimizacao"></param>
        /// <returns></returns>
        IEnumerable<IArquivoSolucaoOtimizacao> ObterArquivos(ISolucaoOtimizacao solucaoOtimizacao);

        /// <summary>
        /// Salva os arquivos da solução de otimização.
        /// </summary>
        /// <param name="solucaoOtimizacao">Dados da solução para onde os arquivos serão salvos.</param>
        /// <param name="arquivos">Arquivos que serão salvos.</param>
        void SalvarArquivos(ISolucaoOtimizacao solucaoOtimizacao, IEnumerable<IArquivoSolucaoOtimizacao> arquivos);

        #endregion
    }
}
