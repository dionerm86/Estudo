using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Possíveis tipos da mensagem da transação.
    /// </summary>
    public enum TipoMensagemTransacao
    {
        /// <summary>
        /// Informação.
        /// </summary>
        Informacao,
        /// <summary>
        /// Alerta.
        /// </summary>
        Alerta,
        /// <summary>
        /// Erro.
        /// </summary>
        Erro
    }

    /// <summary>
    /// Representa a mensagem da transação de otimização.
    /// </summary>
    public class MensagemTransacao
    {
        #region Propriedades

        /// <summary>
        /// Título da mensagem.
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Tipo da mensagem.
        /// </summary>
        public TipoMensagemTransacao Tipo { get; set; }

        /// <summary>
        /// Mensagem.
        /// </summary>
        public string Mensagem { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public MensagemTransacao()
        {
        }

        /// <summary>
        /// Cria a instancia com os valores iniciais.
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensagem"></param>
        /// <param name="tipo"></param>
        public MensagemTransacao(string titulo, string mensagem, TipoMensagemTransacao tipo)
        {
            Titulo = titulo;
            Mensagem = mensagem;
            Tipo = tipo;
        }

        #endregion
    }
}
