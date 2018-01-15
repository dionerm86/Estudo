namespace Glass.Negocios
{
    /// <summary>
    /// Provedor do controle de alteração.
    /// </summary>
    public static class ProvedorControleAlteracao
    {
        #region Variáveis Locais

        private static object _objLock = new object();
        private static IControleAlteracao _controleAlteracao;

        #endregion

        #region Propriedades

        /// <summary>
        /// Instancia do controle de alteração.
        /// </summary>
        private static IControleAlteracao ControleAlteracao
        {
            get
            {
                if (_controleAlteracao == null)
                    lock (_objLock)
                    {
                        if (_controleAlteracao == null)
                        {
                            _controleAlteracao = Microsoft.Practices.ServiceLocation.ServiceLocator
                                .Current.GetInstance<IControleAlteracao>();
                        }
                    }

                return _controleAlteracao;
            }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Configura o provedor.
        /// </summary>
        public static void Configurar()
        {
            var persistingEvent = Colosoft.Domain.DomainEvents.Instance.GetEvent<Colosoft.Business.EntityPersistingEvent>();
            var deletingEvent = Colosoft.Domain.DomainEvents.Instance.GetEvent<Colosoft.Business.EntityDeletingWithPersistenceSessionEvent>();

            // Registra o método acionado pelo evento persistencia da entidade.
            persistingEvent.Subscribe(OnPersistingEntity);

            // Registra o método acionado pelo evento de exclusão da entidade.
            deletingEvent.Subscribe(OnDeletingEntity);
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Método acionado quando uma entidade estiver sendo persistida.
        /// </summary>
        /// <param name="e"></param>
        private static void OnPersistingEntity(Colosoft.Business.EntityPersistingEventArgs e)
        {
            ControleAlteracao.RegistraAlteracoes(e.AfterSession, e.Entity);
        }

        /// <summary>
        /// Método acionado quando uma entidade estiver sendo apagada.
        /// </summary>
        /// <param name="e"></param>
        private static void OnDeletingEntity(Colosoft.Business.EntityDeletingWithPersistenceSessionEventArgs e)
        {
            ControleAlteracao.RegistraExclusao(e.AfterSession, e.Entity);
        }

        #endregion
    }
}
