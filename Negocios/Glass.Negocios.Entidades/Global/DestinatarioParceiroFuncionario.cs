namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados do destinatário que é funcionário parceiro.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(DestinatarioParceiroFuncionarioLoader))]
    public class DestinatarioParceiroFuncionario : Colosoft.Business.Entity<Glass.Data.Model.DestinatarioParceiroFuncionario>
    {
        #region Tipos Aninhados

        class DestinatarioParceiroFuncionarioLoader : Colosoft.Business.EntityLoader<DestinatarioParceiroFuncionario, Glass.Data.Model.DestinatarioParceiroFuncionario>
        {
            public DestinatarioParceiroFuncionarioLoader()
            {
                Configure()
                    .Keys(f => f.IdMensagemParceiro, f => f.IdFunc)
                    .Creator(f => new DestinatarioParceiroFuncionario(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da mensagem.
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
        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public DestinatarioParceiroFuncionario()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected DestinatarioParceiroFuncionario(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.DestinatarioParceiroFuncionario> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public DestinatarioParceiroFuncionario(Glass.Data.Model.DestinatarioParceiroFuncionario dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
