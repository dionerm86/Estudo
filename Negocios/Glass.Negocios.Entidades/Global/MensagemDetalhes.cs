using System;
using System.Collections.Generic;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os detalhes da mensagem.
    /// </summary>
    public class MensagemDetalhes
    {
        #region Tipos Aninhados

        /// <summary>
        /// Nome do destinatário.
        /// </summary>
        public class Destinatario
        {
            /// <summary>
            /// Identificador do destinatário.
            /// </summary>
            public int IdDestinatario { get; set; }

            /// <summary>
            /// Nome do destinatário.
            /// </summary>
            public string Nome { get; set; }

            /// <summary>
            /// Identifica se já foi lida pelo destinatário.
            /// </summary>
            public bool Lida { get; set; }
        }

        #endregion

        #region Variáveis Locais

        private List<Destinatario> _destinatarios = new List<Destinatario>();

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da mensagem.
        /// </summary>
        public int IdMensagem { get; set; }

        /// <summary>
        /// Nome do remetente.
        /// </summary>
        public string Remetente { get; set; }

        /// <summary>
        /// Destinatários.
        /// </summary>
        public List<Destinatario> Destinatarios
        {
            get { return _destinatarios; }
        }

        /// <summary>
        /// Assunto.
        /// </summary>
        public string Assunto { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Data de cadastro.
        /// </summary>
        public DateTime DataCad { get; set; }

        #endregion
    }
}
