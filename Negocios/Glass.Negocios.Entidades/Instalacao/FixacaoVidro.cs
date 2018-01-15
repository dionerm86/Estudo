namespace Glass.Instalacao.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da fixação de vidro.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(FixacaoVidroLoader))]
    public class FixacaoVidro : Colosoft.Business.Entity<Data.Model.FixacaoVidro>
    {
        #region Tipos Aninhados

        class FixacaoVidroLoader : Colosoft.Business.EntityLoader<FixacaoVidro, Data.Model.FixacaoVidro>
        {
            public FixacaoVidroLoader()
            {
                Configure()
                    .Uid(f => f.IdFixacaoVidro)
                    .FindName(f => f.Descricao)
                    .Creator(f => new FixacaoVidro(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da fixação.
        /// </summary>
        public int IdFixacaoVidro
        {
            get { return DataModel.IdFixacaoVidro; }
            set
            {
                if (DataModel.IdFixacaoVidro != value &&
                    RaisePropertyChanging("IdFixacaoVidro", value))
                {
                    DataModel.IdFixacaoVidro = value;
                    RaisePropertyChanged("IdFixacaoVidro");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get { return DataModel.Descricao; }
            set
            {
                if (DataModel.Descricao != value &&
                    RaisePropertyChanging("Descricao", value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged("Descricao");
                }
            }
        }

        /// <summary>
        /// Sigla.
        /// </summary>
        public string Sigla
        {
            get { return DataModel.Sigla; }
            set
            {
                if (DataModel.Sigla != value &&
                    RaisePropertyChanging("Sigla", value))
                {
                    DataModel.Sigla = value;
                    RaisePropertyChanged("Sigla");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FixacaoVidro()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FixacaoVidro(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.FixacaoVidro> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FixacaoVidro(Data.Model.FixacaoVidro dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
