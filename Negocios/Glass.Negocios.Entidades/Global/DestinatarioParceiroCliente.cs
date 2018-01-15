namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa o cliente como destinatário de uma mensagem para um parceiro.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(DestinatarioParceiroClienteLoader))]
    public class DestinatarioParceiroCliente : Colosoft.Business.Entity<Glass.Data.Model.DestinatarioParceiroCliente>
    {
        #region Tipos Aninhados

        class DestinatarioParceiroClienteLoader : Colosoft.Business.EntityLoader<DestinatarioParceiroCliente, Glass.Data.Model.DestinatarioParceiroCliente>
        {
            public DestinatarioParceiroClienteLoader()
            {
                Configure()
                    .Keys(f => f.IdMensagemParceiro, f => f.IdCli)
                    .Creator(f => new DestinatarioParceiroCliente(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da mensagem do parceiro.
        /// </summary>
        public int IdMensagemParceiro
        {
            get { return DataModel.IdMensagemParceiro; }
            set
            {
                if (DataModel.IdMensagemParceiro != value &&
                    RaisePropertyChanging("IdMensagemParceiro", value))
                {
                    DataModel.IdMensagemParceiro = value;
                    RaisePropertyChanged("IdMensagemParceiro");
                }
            }
        }

        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public int IdCli
        {
            get { return DataModel.IdCli; }
            set
            {
                if (DataModel.IdCli != value &&
                    RaisePropertyChanging("IdCli", value))
                {
                    DataModel.IdCli = value;
                    RaisePropertyChanged("IdCli");
                }
            }
        }

        /// <summary>
        /// Identificado se a mensagem foi lida pelo destinatário.
        /// </summary>
        public bool Lida
        {
            get { return DataModel.Lida; }
            set
            {
                if (DataModel.Lida != value &&
                    RaisePropertyChanging("Lida", value))
                {
                    DataModel.Lida = value;
                    RaisePropertyChanged("Lida");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public DestinatarioParceiroCliente()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected DestinatarioParceiroCliente(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.DestinatarioParceiroCliente> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public DestinatarioParceiroCliente(Glass.Data.Model.DestinatarioParceiroCliente dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
