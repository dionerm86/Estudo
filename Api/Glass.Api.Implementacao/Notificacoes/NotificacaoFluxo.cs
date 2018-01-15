using Glass.Data.DAL;
using System;
using System.Collections.Generic;

namespace Glass.Api.Notificacoes.Implementacao
{
    /// <summary>
    /// Implementacao do fluxo de negocio das notificações do sistema
    /// </summary>
    public class NotificacaoFluxo : INotificacaoFluxo
    {
        #region Métodos Publicos

        /// <summary>
        /// Marca as notificações como enviadas
        /// </summary>
        /// <param name="periodo"></param>
        public void MarcarEnviada(DateTime periodo)
        {
            NotificacaoDAO.Instance.MarcarEnviada(periodo);
        }

        /// <summary>
        /// Recupera as notificacoes disponiveis
        /// </summary>
        /// <returns></returns>
        public List<Data.Model.Notificacao> ObtemNotificacoes()
        {
            return NotificacaoDAO.Instance.ObterNaoEnviadas();
        }

        #endregion
    }
}
