namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócios do CEST.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(CestLoader))]
    public class Cest : Colosoft.Business.Entity<Data.Model.Cest>
    {
        #region Tipos Aninhados

        class CestLoader : Colosoft.Business.EntityLoader<Cest, Data.Model.Cest>
        {
            public CestLoader()
            {
                Configure()
                    .Uid(f => f.IdCest)
                    .FindName(f => f.Codigo)
                    .Creator(f => new Cest(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do CEST.
        /// </summary>
        public int IdCest
        {
            get { return DataModel.IdCest; }
            set
            {
                if (DataModel.IdCest != value &&
                    RaisePropertyChanging("IdCest", value))
                {
                    DataModel.IdCest = value;
                    RaisePropertyChanged("IdCest");
                }
            }
        }

        /// <summary>
        /// Código do CEST.
        /// </summary>
        public string Codigo
        {
            get { return DataModel.Codigo; }
            set
            {
                if (DataModel.Codigo != value &&
                    RaisePropertyChanging("Codigo", value))
                {
                    DataModel.Codigo = value;
                    RaisePropertyChanged("Codigo");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Cest()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Cest(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Cest> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Cest(Data.Model.Cest dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
