namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio da seguradora.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(SeguradoraLoader))]
    public class Seguradora : Colosoft.Business.Entity<Glass.Data.Model.Cte.Seguradora>
    {
        #region Tipos Aninhados

        class SeguradoraLoader : Colosoft.Business.EntityLoader<Seguradora, Glass.Data.Model.Cte.Seguradora>
        {
            public SeguradoraLoader()
            {
                Configure()
                    .Uid(f => f.IdSeguradora)
                    .FindName(f => f.NomeSeguradora)
                    .Creator(f => new Seguradora(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da seguradora.
        /// </summary>
        public int IdSeguradora
        {
            get { return DataModel.IdSeguradora; }
            set
            {
                if (DataModel.IdSeguradora != value &&
                    RaisePropertyChanging("IdSeguradora", value))
                {
                    DataModel.IdSeguradora = value;
                    RaisePropertyChanged("IdSeguradora");
                }
            }
        }

        /// <summary>
        /// Nome da seguradora.
        /// </summary>
        public string NomeSeguradora
        {
            get { return DataModel.NomeSeguradora; }
            set
            {
                if (DataModel.NomeSeguradora != value &&
                    RaisePropertyChanging("NomeSeguradora", value))
                {
                    DataModel.NomeSeguradora = value;
                    RaisePropertyChanged("NomeSeguradora");
                }
            }
        }

        /// <summary>
        /// CNPJ da seguradora.
        /// </summary>
        public string CNPJ
        {
            get { return DataModel.CNPJ; }
            set
            {
                if (DataModel.CNPJ != value &&
                    RaisePropertyChanging("CNPJ", value))
                {
                    DataModel.CNPJ = value;
                    RaisePropertyChanged("CNPJ");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Seguradora()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Seguradora(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.Cte.Seguradora> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Seguradora(Glass.Data.Model.Cte.Seguradora dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
