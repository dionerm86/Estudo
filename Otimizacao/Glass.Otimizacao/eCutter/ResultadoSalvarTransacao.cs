using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa o resultado da operação de salvar a transação.
    /// </summary>
    public class ResultadoSalvarTransacao
    {
        #region Propriedades

        /// <summary>
        /// Identifica se a operação foi executada com sucesso.
        /// </summary>
        public bool Sucesso { get; }

        /// <summary>
        /// Uri que deve redicionanado quando a transação for finalizada.
        /// </summary>
        public Uri UriRedirecionar { get; }

        /// <summary>
        /// Mensagens associadas.
        /// </summary>
        public IEnumerable<MensagemTransacao> Mensagens { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="sucesso"></param>
        /// <param name="uriRedirecionar"></param>
        /// <param name="mensagens"></param>
        public ResultadoSalvarTransacao(bool sucesso, Uri uriRedirecionar, IEnumerable<MensagemTransacao> mensagens)
        {
            Sucesso = sucesso;
            UriRedirecionar = uriRedirecionar;
            Mensagens = mensagens ?? new MensagemTransacao[0];
        }

        #endregion
    }
}
