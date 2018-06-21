using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios
{
    /// <summary>
    /// Assinatura do verifcador que usa como base a rentabilidade
    /// para definir a liberação.
    /// </summary>
    public interface IVerificadorRentabilidadeLiberacao
    {
        #region Métodos

        /// <summary>
        /// Verifica se é necessário liberação para o item informado.
        /// </summary>
        /// <param name="item">Item que será verificado.</param>
        /// <returns></returns>
        bool VerificarRequerLiberacao(IItemRentabilidade item);

        /// <summary>
        /// Atualiza os dados do verificador.
        /// </summary>
        void AtualizarDados();

        #endregion
    }
}
