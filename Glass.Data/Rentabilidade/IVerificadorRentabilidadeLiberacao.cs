using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data
{
    /// <summary>
    /// Assinatura do verificador de rentabilidade para liberação.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IVerificadorRentabilidadeLiberacao<in T>
    {
        /// <summary>
        /// Executa a verificação da rentabilidade para a liberação.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool VerificarRequerLiberacao(GDA.GDASession sessao, int id);

        /// <summary>
        /// Executa a verificação da rentabilidade para a liberação.
        /// </summary>
        /// <param name="instancia">Instância que será usada na verificação.</param>
        /// <returns></returns>
        bool VerificarRequerLiberacao(GDA.GDASession sessao, T instancia);
    }
}
