namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Reprsenta o Margem de valor agregado do produto por estado.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(MvaProdutoUfLoader))]
    public class MvaProdutoUf : Colosoft.Business.Entity<Data.Model.MvaProdutoUf>
    {
        #region Tipos Aninhados

        class MvaProdutoUfLoader : Colosoft.Business.EntityLoader<MvaProdutoUf, Data.Model.MvaProdutoUf>
        {
            public MvaProdutoUfLoader()
            {
                Configure()
                    .Keys(f => f.IdProd, f => f.UfOrigem, f => f.UfDestino)
                    .Creator(f => new MvaProdutoUf(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do produto associado.
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
        /// Estado de Origem.
        /// </summary>
        public string UfOrigem
        {
            get { return DataModel.UfOrigem; }
            set
            {
                if (DataModel.UfOrigem != value &&
                    RaisePropertyChanging("UfOrigem", value))
                {
                    DataModel.UfOrigem = value;
                    RaisePropertyChanged("UfOrigem");
                }
            }
        }

        /// <summary>
        /// Estado de destinho.
        /// </summary>
        public string UfDestino
        {
            get { return DataModel.UfDestino; }
            set
            {
                if (DataModel.UfDestino != value &&
                    RaisePropertyChanging("UfDestino", value))
                {
                    DataModel.UfDestino = value;
                    RaisePropertyChanged("UfDestino");
                }
            }
        }

        /// <summary>
        /// Mva Original.
        /// </summary>
        public float MvaOriginal
        {
            get { return DataModel.MvaOriginal; }
            set
            {
                if (DataModel.MvaOriginal != value &&
                    RaisePropertyChanging("MvaOriginal", value))
                {
                    DataModel.MvaOriginal = value;
                    RaisePropertyChanged("MvaOriginal");
                }
            }
        }

        /// <summary>
        /// Mva Simples.
        /// </summary>
        public float MvaSimples
        {
            get { return DataModel.MvaSimples; }
            set
            {
                if (DataModel.MvaSimples != value &&
                    RaisePropertyChanging("MvaSimples", value))
                {
                    DataModel.MvaSimples = value;
                    RaisePropertyChanged("MvaSimples");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public MvaProdutoUf()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected MvaProdutoUf(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.MvaProdutoUf> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public MvaProdutoUf(Data.Model.MvaProdutoUf dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
