namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa o destinatário de uma mensagem.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(DestinatarioLoader))]
    public class Destinatario : Colosoft.Business.Entity<Glass.Data.Model.Destinatario>
    {
        #region Tipos Aninhados

        class DestinatarioLoader : Colosoft.Business.EntityLoader<Destinatario, Glass.Data.Model.Destinatario>
        {
            public DestinatarioLoader()
            {
                Configure()
                    .Keys(f => f.IdMensagem, f => f.IdFunc)
                    .Creator(f => new Destinatario(f));

            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da mensagem.
        /// </summary>
        public int IdMensagem
        {
            get { return DataModel.IdMensagem; }
            set
            {
                if (DataModel.IdMensagem != value &&
                    RaisePropertyChanging("IdMensagem", value))
                {
                    DataModel.IdMensagem = value;
                    RaisePropertyChanged("IdMensagem");
                }
            }
        }

        /// <summary>
        /// Identificador do funcionário associado.
        /// </summary>
        public int IdFunc
        {
            get { return DataModel.IdFunc; }
            set
            {
                if (DataModel.IdFunc != value &&
                    RaisePropertyChanging("IdFunc", value))
                {
                    DataModel.IdFunc = value;
                    RaisePropertyChanged("IdFunc");
                }
            }
        }

        /// <summary>
        /// Identifica se a mensagem foi lida pelo destinatário.
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

        /// <summary>
        /// Identifica se a mensagem foi cancelada pelo destinatário, ou seja, não será exibida na lista de mensagens.
        /// </summary>
        public bool Cancelada
        {
            get { return DataModel.Cancelada; }
            set
            {
                if (DataModel.Cancelada != value &&
                    RaisePropertyChanging("Cancelada", value))
                {
                    DataModel.Cancelada = value;
                    RaisePropertyChanged("Cancelada");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Destinatario()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Destinatario(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Destinatario> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Destinatario(Glass.Data.Model.Destinatario dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
