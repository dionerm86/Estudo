using System.Collections.Generic;

namespace Glass.PCP.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negocio da otimização de vidros e alumínios
    /// </summary>
    public interface IOtimizacaoFluxo
    {
        /// <summary>
        /// Pequisa as otimizacoes do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Otimizacao> PesquisarOtimizacoes();

        /// <summary>
        /// Recupera os dados da otimização.
        /// </summary>
        /// <param name="idOtimizacao"></param>
        /// <returns></returns>
        Entidades.Otimizacao ObterOtimizacao(int idOtimizacao);

        /// <summary>
        /// Gera uma otimização linear (Alumínio)
        /// </summary>
        /// <param name="lstProdPed"></param>
        /// <param name="lstIdProd"></param>
        /// <param name="lstComprimento"></param>
        /// <param name="lstAngEsq"></param>
        /// <param name="lstAngDir"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult GerarOtimizacaoLinear(int[] lstProdPed, int[] lstProdOrca, int[] lstIdProd, decimal[] lstComprimento, int[] lstGrau, bool projEsquadria);
    }
}
