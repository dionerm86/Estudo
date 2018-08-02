using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa o resultado da autenticação.
    /// </summary>
    public class AutenticacaoProtocolo
    {
        #region Propriedades

        /// <summary>
        /// Obtém se a autenticação foi realizada com sucesso.
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Obtém ou define a mensagem associada.
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Usuário associado com a autenticação.
        /// </summary>
        public string Usuario { get; set; }

        #endregion
    }

    /// <summary>
    /// Assinatura do autenticador do protocolo do eCutter.
    /// </summary>
    public interface IAutenticadorProtocolo
    {
        #region Métodos

        /// <summary>
        /// Realiza a autenticação.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        AutenticacaoProtocolo Autenticar(string usuario, string senha);

        #endregion
    }
}
