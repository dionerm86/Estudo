namespace Glass.Global.Negocios.Entidades
{
    [Colosoft.Business.EntityLoader(typeof(ProdutoNCMLoader))]
    public class ProdutoNCM : Colosoft.Business.Entity<Data.Model.ProdutoNCM>
    {
        #region Tipos Aninhados

        class ProdutoNCMLoader : Colosoft.Business.EntityLoader<ProdutoNCM, Data.Model.ProdutoNCM>
        {
            public ProdutoNCMLoader()
            {
                Configure()
                    .Keys(f => f.IdLoja, f => f.IdProd)
                    .Creator(f => new ProdutoNCM(f));
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int IdProd
        {
            get { return DataModel.IdProd; }
            set
            {
                if (DataModel.IdProd != value &&
                    RaisePropertyChanging("IdProd", value))
                {
                    DataModel.IdProd = value;
                    RaisePropertyChanged("IdProd");
                }
            }
        }

        /// <summary>
        /// Identificador da Loja.
        /// </summary>
        public int IdLoja
        {
            get { return DataModel.IdLoja; }
            set
            {
                if (DataModel.IdLoja != value &&
                    RaisePropertyChanging("IdLoja", value))
                {
                    DataModel.IdLoja = value;
                    RaisePropertyChanged("IdLoja");
                }
            }
        }

        /// <summary>
        /// NCM.
        /// </summary>
        public string NCM
        {
            get { return DataModel.NCM; }
            set
            {
                if (DataModel.NCM != value &&
                    RaisePropertyChanging("NCM", value))
                {
                    DataModel.NCM = value;
                    RaisePropertyChanged("NCM");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ProdutoNCM()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected ProdutoNCM(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.ProdutoNCM> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public ProdutoNCM(Data.Model.ProdutoNCM dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
