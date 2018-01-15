using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados de um registro da pesquisa de mensagens.
    /// </summary>
    public class MensagemPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da mensagem.
        /// </summary>
        public int IdMensagem { get; set; }

        /// <summary>
        /// Identificador da remetente.
        /// </summary>
        public int IdRemetente { get; set; }

        /// <summary>
        /// Nome do remetente da mensagem.
        /// </summary>
        public string Remetente { get; set; }

        /// <summary>
        /// Destinatários.
        /// </summary>
        public string Destinatarios { get; set; }

        /// <summary>
        /// Assunto da mensagem.
        /// </summary>
        public string Assunto { get; set; }

        /// <summary>
        /// Data de cadastro.
        /// </summary>
        public DateTime DataCad { get; set; }

        /// <summary>
        /// Identifica se mensagem foi lida.
        /// </summary>
        public bool Lida { get; set; }

        #endregion
    }
}
