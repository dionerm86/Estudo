namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Representa a entidade de negócio do país.
    /// </summary>
    [Colosoft.Business.EntityLoader(typeof(PaisLoader))]
    public class Pais : Colosoft.Business.Entity<Data.Model.Pais>
    {
        #region Tipos Aninhados

        class PaisLoader : Colosoft.Business.EntityLoader<Pais, Data.Model.Pais>
        {
            public PaisLoader()
            {
                Configure()
                    .Uid(f => f.IdPais)
                    .FindName(f => f.NomePais)
                    .Creator(f => new Pais(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do país.
        /// </summary>
        public int IdPais
        {
            get { return DataModel.IdPais; }
            set
            {
                if (DataModel.IdPais != value &&
                    RaisePropertyChanging("IdPais", value))
                {
                    DataModel.IdPais = value;
                    RaisePropertyChanged("IdPais");
                }
            }
        }

        /// <summary>
        /// Nome do país.
        /// </summary>
        public string NomePais
        {
            get { return DataModel.NomePais; }
            set
            {
                if (DataModel.NomePais != value &&
                    RaisePropertyChanging("NomePais", value))
                {
                    DataModel.NomePais = value;
                    RaisePropertyChanged("NomePais");
                }
            }
        }

        /// <summary>
        /// Código do páis.
        /// </summary>
        public string CodPais
        {
            get { return DataModel.CodPais; }
            set
            {
                if (DataModel.CodPais != value &&
                    RaisePropertyChanging("CodPais", value))
                {
                    DataModel.CodPais = value;
                    RaisePropertyChanged("CodPais");
                }
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public Pais()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected Pais(Colosoft.Business.EntityLoaderCreatorArgs<Data.Model.Pais> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public Pais(Data.Model.Pais dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion
    }
}
