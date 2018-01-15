using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Armazena o resulta da operacao de login
    /// </summary>
    public class ResultadoLogin
    {
        #region Propriedades

        /// <summary>
        /// Identifica se o login foi efetuado com sucesso.
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Mensagem de retorno da autenticacao.
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Informações do usuario logado,
        /// </summary>
        public InfoUsuario Usuario { get; set; }

        #endregion
    }
}
