using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios
{
    /// <summary>
    /// Representa o resultado da importação de uma otimização.
    /// </summary>
    public class ImportacaoOtimizacao
    {
        #region Propriedades

        /// <summary>
        /// Identificador do arquivo de otimização importado.
        /// </summary>
        public int IdArquivoOtimizacao { get; set; }

        #endregion
    }

    /// <summary>
    /// Assinatura do fluxo de negócio da otimização.
    /// </summary>
    public interface IOtimizacaoFluxo
    {
        #region Métodos

        /// <summary>
        /// Recupera a sessão de otimização associado com o identificador do arquivo de otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao"></param>
        /// <returns></returns>
        ISessaoOtimizacao ObterSessaoOtimizacao(int idArquivoOtimizacao);

        /// <summary>
        /// Realiza a importação do resultado de uma otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquivo de otimização.</param>
        /// <param name="arquivos">Arquivos da otimização.</param>
        /// <returns></returns>
        ImportacaoOtimizacao Importar(int idArquivoOtimizacao, IEnumerable<IConteudoArquivoOtimizacao> arquivos);

        /// <summary>
        /// Recupera os itens da otimização.
        /// </summary>
        /// <param name="idArquivoOtimizacao">Identificador do arquivo da otimização no qual os itens estão associados.</param>
        /// <returns></returns>
        IEnumerable<ItemOtimizacao> ObterItens(int idArquivoOtimizacao);
        
        #endregion
    }
}
